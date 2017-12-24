using System;
using System.Collections.Generic;
using System.Text;
using Xmu.Crms.Services.Group1.Dao;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.Group1
{
    class TopicService : ITopicService
    {
        private readonly ITopicDao _topicDao;

        public TopicService(ITopicDao topicDao)
        {
            _topicDao = topicDao;
        }
        public void DeleteSeminarGroupTopicByTopicId(long topicId)
        {
            throw new NotImplementedException();
        }

        public void DeleteTopicById(long groupId, long topicId)
        {
            throw new NotImplementedException();
        }

        public void DeleteTopicBySeminarId(long seminarId)
        {
            throw new NotImplementedException();
        }

        public void DeleteTopicByTopicId(long topicId, long seminarId)
        {
            throw new NotImplementedException();
        }

        public SeminarGroupTopic GetSeminarGroupTopicById(long topicId, long groupId)
        {
            throw new NotImplementedException();
        }

        public Topic GetTopicByTopicId(long topicId)
        {
            throw new NotImplementedException();
        }

        public long InsertTopicBySeminarId(long seminarId, Topic topic)
        {
            throw new NotImplementedException();
        }

        public IList<Topic> ListTopicBySeminarId(long seminarId)
        {
            throw new NotImplementedException();
        }

        public void UpdateTopicByTopicId(long topicId, Topic topic)
        {
            throw new NotImplementedException();
        }
    }
}
