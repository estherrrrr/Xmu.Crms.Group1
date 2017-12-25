using System;
using System.Collections.Generic;
using System.Text;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Services.Group1.Dao
{
    interface ITopicDao
    {
        Topic GetTopic(long topicId);

        SeminarGroupTopic GetSeminarGroupTopic(long topicId, long groupId);

        //按topicId删除SeminarGroupTopic表信息.
        void DeleteSeminarGroupTopicByTopicId(long topicId);

        //按topicId和小组Id删除SeminarGroupTopic
        void DeleteSeminarGroupTopic(long topicId,long groupId);

        void DeleteTopic(long topicId);

        long Insert(long seminarId, Topic topic);

        IList<Topic> List(long seminarId);

        Seminar FindSeminar(long seminarId);

        List<Topic> GetTopicId(long seminarId);

        void Update(long topicId, Topic topic);

        List<long> GetSeminarGroupTopicId(long topicId);

        void DeleteStudentScoreGroup(long seminargrouptopicId);

        List<SeminarGroupTopic> FindSeminarGroupTopicByGroupId(long id);

        IList<SeminarGroup> GetSeminarGroupById(long classId, long seminarId);
    }
}
