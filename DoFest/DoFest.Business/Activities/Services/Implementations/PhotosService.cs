﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using DoFest.Business.Activities.Models.Content.Photos;
using DoFest.Business.Activities.Services.Interfaces;
using DoFest.Business.Errors;
using DoFest.Entities.Activities.Content;
using DoFest.Entities.Authentication.Notification;
using DoFest.Persistence.Activities;
using DoFest.Persistence.Authentication;
using DoFest.Persistence.Notifications;
using Microsoft.AspNetCore.Http;

namespace DoFest.Business.Activities.Services.Implementations
{
    public sealed class PhotosService : IPhotosService
    {
        private readonly IMapper _mapper;
        private readonly IActivitiesRepository _activitiesRepository;
        private readonly IHttpContextAccessor _accessor;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;

        public PhotosService(IActivitiesRepository activitiesRepository, IMapper mapper, IHttpContextAccessor accessor,
            INotificationRepository notificationRepository, IUserRepository userRepository)
        {
            _activitiesRepository = activitiesRepository;
            _mapper = mapper;
            _accessor = accessor;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<IEnumerable<PhotoModel>, Error>> Get(Guid activityId)
        {
            var activityExists = (await _activitiesRepository.GetById(activityId)) != null;
            if (!activityExists)
            {
                return Result.Failure<IEnumerable<PhotoModel>, Error>(ErrorsList.UnavailableActivity);
            }

            var activity = await _activitiesRepository.GetByIdWithPhotos(activityId);

            return Result.Success<IEnumerable<PhotoModel>, Error>(_mapper.Map<IEnumerable<PhotoModel>>(activity.Photos));

        }

        public async Task<Result<PhotoModel, Error>> Add(Guid activityId, CreatePhotoModel model)
        {
            model.UserId = Guid.Parse(_accessor.HttpContext.User.Claims.First(c => c.Type == "userId").Value);

            using var stream = new MemoryStream();
            await model.Image.CopyToAsync(stream);

            var photo = new Photo
            {
                Image = stream.ToArray()
            };

            var activity = await _activitiesRepository.GetById(activityId);
            if (activity == null)
            {
                return Result.Failure<PhotoModel, Error>(ErrorsList.UnavailableActivity);
            }

            activity.AddPhoto(photo);
            _activitiesRepository.Update(activity);

            await _activitiesRepository.SaveChanges();

            var user = await _userRepository.GetById(photo.UserId);
            var notification = new Notification()
            {
                ActivityId = activityId,
                Date = DateTime.Now,
                Description = $"{user.Username} has added a new photo to activity {activity.Name}."
            };

            await _notificationRepository.Add(notification);
            await _notificationRepository.SaveChanges();

            return Result.Success<PhotoModel, Error>(_mapper.Map<PhotoModel>(photo));
        }

        public async Task<Result<string, Error>> Delete(Guid activityId, Guid photoId)
        {
            var activity = await _activitiesRepository.GetByIdWithPhotos(activityId);
            if (activity == null)
            {
                return Result.Failure<string, Error>(ErrorsList.UnavailableActivity);
            }
            var photo = activity.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null)
            {
                return Result.Failure<string, Error>(ErrorsList.UnavailablePhoto);
            }

            var loggedUserId = Guid.Parse(this._accessor.HttpContext.User.Claims.First(c => c.Type == "userId").Value);
            if (loggedUserId != photo.UserId)
            {
                return Result.Failure<string, Error>(ErrorsList.DeleteNotAuthorized);
            }

            activity.RemovePhoto(photoId);
            _activitiesRepository.Update(activity);

            await _activitiesRepository.SaveChanges();

            return Result.Success<string, Error>("Photo deleted successfully");
        }
    }
}
