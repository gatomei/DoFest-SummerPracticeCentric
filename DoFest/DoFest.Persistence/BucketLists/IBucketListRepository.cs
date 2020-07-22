﻿using DoFest.Entities.Authentication;
using DoFest.Entities.Lists;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DoFest.Persistence.BucketLists
{
    public interface IBucketListRepository:IRepository<User>
    {
      
        Task<User> GetBucketListandUsernameByUserId(Guid id);
    }
}
