﻿using System;

namespace DoFest.Business.Models.Activity
{
    public sealed class ActivityModel
    {
        public Guid? ActivityTypeId { get; private set; }

        public string Name { get; private set; }

        public Guid? Id { get; private set; }

        public Guid? LocationId { get; private set; }

        public string Description { get; private set; }

    }
}
