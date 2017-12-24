using System;
using System.Collections.Generic;
using System.Text;
using Xmu.Crms.Services.Group1.Dao;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Services.Group1
{
    class TopicDao:ITopicDao
    {
        private readonly CrmsContext _db;

        public TopicDao(CrmsContext db)
        {
            _db = db;
        }
    }
}
