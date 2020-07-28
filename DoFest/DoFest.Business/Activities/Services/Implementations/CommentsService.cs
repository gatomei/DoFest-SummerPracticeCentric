﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DoFest.Business.Activities.Models.Content.Comment;
using DoFest.Business.Activities.Services.Interfaces;
using DoFest.Entities.Activities.Content;
using DoFest.Persistence.Activities;
using Microsoft.AspNetCore.Http;

namespace DoFest.Business.Activities.Services.Implementations
{
    /// <summary>
    /// Implementarea serviciului pentru comentarii.
    /// </summary>
    public sealed class CommentsService: ICommentsService
    {
        private readonly IMapper _mapper;
        private readonly IActivitiesRepository _repository;
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// Constructor public default.
        /// </summary>
        /// <param name="mapper"> Serviciul de mapare. </param>
        /// <param name="repository"> Repository-ul atribuit activitatilor. </param>
        /// <param name="accessor"> Un accesor pentru accesarea campurilor requestului HTTP. </param>
        public CommentsService(IMapper mapper, IActivitiesRepository repository, IHttpContextAccessor accessor)
        {
            _mapper = mapper;
            _repository = repository;
            _accessor = accessor;
        }

        public async Task<IList<CommentModel>> GetComments(Guid activityId)
        {
            // TODO: exception handle
            var comments = await _repository.GetByIdWithComments(activityId);

            return _mapper.Map<List<CommentModel>>(comments.Comments);
        }

        public async Task<CommentModel> AddComment(Guid activityId, NewCommentModel commentModel)
        {
            // TODO: exception handle
            commentModel.UserId = Guid.Parse(_accessor.HttpContext.User.Claims.First(c => c.Type == "userId").Value);
            var activity = await _repository.GetById(activityId);
            var comment = _mapper.Map<Comment>(commentModel);
            activity.AddComment(comment);
            _repository.Update(activity);
            await _repository.SaveChanges();
            return _mapper.Map<CommentModel>(comment);
        }

        public async Task<CommentModel> DeleteComment(Guid activityId, Guid commentId)
        {
            // TODO: exception handle

            var activity = await _repository.GetByIdWithComments(activityId);
            var comment = activity
                .Comments
                .FirstOrDefault(commentSearch => commentSearch.Id == commentId);
            try
            {
                activity.RemoveComment(commentId);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

            _repository.Update(activity);
            await _repository.SaveChanges();

            return _mapper.Map<CommentModel>(comment);
        }
    }
}