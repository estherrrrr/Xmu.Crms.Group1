/*************/
/*************/
/*@author 1-4*/
/*************/
/*************/
using System;
using System.Collections.Generic;
using System.Text;
using Xmu.Crms.Services.Group1.Dao;
using Xmu.Crms.Shared.Exceptions;
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

        //按topicId删除SeminarGroupTopic表信息.
        public void DeleteSeminarGroupTopicByTopicId(long topicId)
        {
            _topicDao.DeleteSeminarGroupTopicByTopicId(topicId);
        }

        //按topicId和小组Id删除SeminarGroupTopic
        public void DeleteSeminarGroupTopicById(long groupId, long topicId)
        {
            _topicDao.DeleteSeminarGroupTopic(topicId,groupId);    
        }

        //按seminarId删除话题
        public void DeleteTopicBySeminarId(long seminarId)
        { 
            IList<Topic> tidlist = _topicDao.List(seminarId);
           foreach(Topic t in tidlist)
            {
                _topicDao.DeleteSeminarGroupTopicByTopicId(t.Id);
                _topicDao.DeleteTopic(t.Id);
            }
        }

        //删除topic
        public void DeleteTopicByTopicId(long topicId)
        {
            _topicDao.DeleteSeminarGroupTopicByTopicId(topicId);
            _topicDao.DeleteTopic(topicId);
        }

        //通过小组Id和Topicid获得小组话题记录
        public SeminarGroupTopic GetSeminarGroupTopicById(long topicId, long groupId)
        {
            try
            {
                return _topicDao.GetSeminarGroupTopic(topicId, groupId);
            }catch(TopicNotFoundException e) { throw e; }
            
        }
       
        //按topicId获取topic
        public Topic GetTopicByTopicId(long topicId)
        {
            try
            {
                return _topicDao.GetTopic(topicId);
            }catch(TopicNotFoundException e) { throw e; }

        }

        //插入新的话题
        public long InsertTopicBySeminarId(long seminarId, Topic topic)
        {
            Seminar s = new Seminar();
            long result;
            try
            {
                s = _topicDao.FindSeminar(seminarId);  //该门讨论课存在
                result = _topicDao.Insert(seminarId, topic);
            }catch(SeminarNotFoundException e) { throw e; }
            return result;
        }

        //通过Seminarid获得该seminar下所有话题
        public IList<Topic> ListTopicBySeminarId(long seminarId)
        {
            try
            {
                return _topicDao.List(seminarId);
            }catch (TopicNotFoundException e) { throw e; }
        }

        //更新
        public void UpdateTopicByTopicId(long topicId, Topic topic)
        {
            try
            {
                _topicDao.Update(topicId, topic);
            }catch(TopicNotFoundException e) { throw e; }
        }

        //根据小组id获取该小组该堂讨论课所有选题信息
        public List<SeminarGroupTopic> ListSeminarGroupTopicByGroupId(long groupId)
        {
            try
            {
                return _topicDao.FindSeminarGroupTopicByGroupId(groupId);
            }
            catch(SeminarNotFoundException e)
            {
                throw e;
            }
        }


        //获取话题选取情况
        public int GetRestTopicById(long topicId,long classId)
        {
            int result = 0;
            int count=0;
            try
            {
                Topic topic = _topicDao.GetTopic(topicId);
                result = topic.GroupNumberLimit;
                IList<SeminarGroup> seminarGroup = _topicDao.GetSeminarGroupById(classId, topic.Seminar.Id);
                foreach (var s in seminarGroup)
                {
                    SeminarGroupTopic seminarGroupTopic = _topicDao.GetSeminarGroupTopic(topicId, s.Id);
                    if(seminarGroupTopic!=null)
                           count++;
                }
            }
            catch(System.Exception e)
            {
                if (e.ToString().Equals("找不到该话题!"))
                    throw e;
            }
            result -= count;
            return result;          
        }

    }
}
