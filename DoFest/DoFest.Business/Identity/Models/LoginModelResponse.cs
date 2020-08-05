﻿using System;

namespace DoFest.Business.Identity.Models
{
    public sealed class LoginModelResponse
    {
        public LoginModelResponse(string username, string email, string token, Guid studentId)
        {
            Username = username;
            Email = email;
            Token = token;
            StudentId = studentId;
        }

        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Token { get; private set; }
        public Guid StudentId { get; private set; }
    }
}