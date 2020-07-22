﻿using System;

namespace DoFest.Business.Models.Ratings
{
    public sealed class RatingModel
    {
        public Guid Id { get; private set; }
        public Guid ActivityId { get; private set; }

        public Guid UserId { get; private set; }
        public int Stars { get; private set; }

    }
}
