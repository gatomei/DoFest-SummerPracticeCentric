﻿using System;

namespace DoFest.Business.Models.Content.Ratings
{
    public class CreateRatingModel
    {
        public Guid UserId { get; set; }

        public int Stars { get; set; }
    }
}