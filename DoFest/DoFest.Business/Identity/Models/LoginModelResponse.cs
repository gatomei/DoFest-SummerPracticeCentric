﻿using System;

namespace DoFest.Business.Identity.Models
{
    public sealed class LoginModelResponse
    {
        public LoginModelResponse(string username, string email, string token, Guid studentId, bool isAdmin, Guid bucketListId)
        {
            Username = username;
            Email = email;
            Token = token;
            StudentId = studentId;
            IsAdmin = isAdmin;
            BucketListId = bucketListId;
        }

        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Token { get; private set; }
        public Guid StudentId { get; private set; }
        public bool IsAdmin { get; private set; }
        public Guid BucketListId { get; private set; }
    }
}