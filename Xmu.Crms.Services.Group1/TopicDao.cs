/*************/
/*************/
/*@author 1-4*/
/*************/
/*************/
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xmu.Crms.Services.Group1.Dao;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Services.Group1
{
    class TopicDao : ITopicDao
    {
        private readonly CrmsContext _db;

        public TopicDao(CrmsContext db)
        {
            _db = db;
        }

        //按topicId删除SeminarGroupTopic表信息.
        public void DeleteSeminarGroupTopicByTopicId(long topicId)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    IList<SeminarGroupTopic> list = _db.SeminarGroupTopic.Where(s => s.Topic.Id == topicId).ToList<SeminarGroupTopic>();
                    if (list == null) throw new TopicNotFoundException();
                    foreach (var l in list)
                    {
                        List<StudentScoreGroup> list2 = _db.StudentScoreGroup.Include(s => s.SeminarGroupTopic).Where(t => t.SeminarGroupTopic.Id == l.Id).ToList<StudentScoreGroup>();
                        foreach (var l2 in list2)
                        {
                            _db.StudentScoreGroup.Remove(l2);
                        }
                        _db.SeminarGroupTopic.Remove(l);
                        _db.SaveChanges();
                    }
                    scope.Commit();
                }
                catch(System.Exception re)
                {
                    scope.Rollback();
                    throw re;
                }
            }
            
             
        }

        //按SeminarGroupTopicId删除StudentScoreGroup记录
        public void DeleteScoreBySeminarGroupId(long id)
        {
            using (var scope = _db.Database.BeginTransaction())
            {       
                try
                {
                    List<StudentScoreGroup> list = _db.StudentScoreGroup.Include(s => s.SeminarGroupTopic).Where(t => t.SeminarGroupTopic.Id == id).ToList<StudentScoreGroup>();
                    if (list == null) throw new System.Exception();
                    foreach (var l in list)
                    {
                        _db.StudentScoreGroup.Attach(l);
                        _db.StudentScoreGroup.Remove(l);
                        _db.SaveChanges();
                    }
                    scope.Commit();
                }
                catch(System.Exception e)
                {
                    scope.Rollback();
                    throw e;
                }
            }
                

        }
        
        //删除Topic表中的记录
        public void DeleteTopic(long topicId)
        {
            using (var scope = _db.Database.BeginTransaction())
            {  
                try
                {
                    Topic t = _db.Topic.Include(a => a.Seminar).Where(a => a.Id == topicId).SingleOrDefault<Topic>();
                    if (t == null) throw new TopicNotFoundException();
                    _db.Topic.Remove(t);
                    _db.SaveChanges();
                    scope.Commit();
                }
                catch(System.Exception de)
                {
                    scope.Rollback();
                    throw de;
                }
            }
        }


        //得到SeminarGroupTopic表的记录
        public SeminarGroupTopic GetSeminarGroupTopic(long topicId, long groupId)
        {
            SeminarGroupTopic s = _db.SeminarGroupTopic.Include(t=>t.Topic).Include(t=>t.SeminarGroup).Where(t => t.Topic.Id == topicId && t.SeminarGroup.Id == groupId).SingleOrDefault();
            if (s == null) throw new SeminarNotFoundException();
            return s;
        }


        //得到Topic表的记录
        public Topic GetTopic(long topicId)
        {            
            var topic = _db.Topic.Include(t=>t.Seminar).FirstOrDefault(t => t.Id == topicId);
            if (topic == null)
            {
                throw new TopicNotFoundException();
            }
            return topic;
             
        }

        public IList<SeminarGroup> GetSeminarGroupById(long classId,long seminarId)
        {
            IList<SeminarGroup> list = _db.SeminarGroup.Include(s=>s.Seminar).Include(s=>s.ClassInfo).Where(s => s.ClassInfo.Id == classId & s.Seminar.Id == seminarId).ToList<SeminarGroup>();
            return list;
        }


        //判断该节讨论课是否存在
        public Seminar FindSeminar(long seminarId)
        {
            var seminar = _db.Seminar.FirstOrDefault(s => s.Id == seminarId);
            if(seminar == null)
            {
                throw new SeminarNotFoundException();
            }
            return seminar;
        }


        //插入Topic表一条新纪录
        public long Insert(long seminarId, Topic topic)
        {
            Seminar s = _db.Seminar.SingleOrDefault(c => c.Id == seminarId);
            topic.Seminar = s;
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Topic.Add(topic);
                    _db.SaveChanges();
                    return topic.Id;
                }
                catch(System.Exception e)
                {
                    scope.Rollback();
                    throw e;
                }
            }
        }



        //通过seminarId得到Topic表中topicId
        public List<Topic> GetTopicId(long seminarId)
        {
            List<Topic> idlist = new List<Topic>();
            idlist = _db.Topic.Where(s => s.Seminar.Id == seminarId).ToList<Topic>();
            if(idlist==null)
                throw new TopicNotFoundException();
            else
                return idlist;
        }
       


        //通过seminarId得到Topic表中的记录
        public IList<Topic> List(long seminarId)
        {
            IList<Topic> topiclist = _db.Topic.Where(s => s.Seminar.Id == seminarId).ToList<Topic>();
            if (topiclist==null)
                throw new TopicNotFoundException();
            else
                return topiclist;
        }

        //更新topic表中的记录
        public void Update(long topicId, Topic topic)
        {
            using (var scope = _db.Database.BeginTransaction())
            { 
                var updatetopic = _db.Topic.FirstOrDefault(t => t.Id == topicId);
                if (updatetopic == null)
                {
                    throw new TopicNotFoundException();
                }
                try
                {
                    updatetopic.Description = topic.Description;
                    updatetopic.GroupNumberLimit = topic.GroupNumberLimit;
                    updatetopic.GroupStudentLimit = topic.GroupStudentLimit;
                    updatetopic.Name = topic.Name;
                    _db.Entry(updatetopic).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                catch(System.Exception e)
                {
                    scope.Rollback();
                    throw e;
                }
            }
        }

        //通过topicId得到SeminarGroupTopic表中SeminarGroupTopicId
        public List<long> GetSeminarGroupTopicId(long topicId)
        {
            List<long> idlist = new List<long>();
            foreach (var sgt in _db.SeminarGroupTopic)
            {
                if (sgt.Topic.Id == topicId)
                    idlist.Add(sgt.Id);
            }
            if (idlist.Count == 0)
                throw new TopicNotFoundException();
            else
                return idlist;
        }


        //删除StudentScoreGroup中的记录
        public void DeleteStudentScoreGroup(long seminargrouptopicId)
        {
            foreach(var ssg in _db.StudentScoreGroup)
            {
                if(ssg.SeminarGroupTopic.Id == seminargrouptopicId)
                {
                    _db.StudentScoreGroup.Remove(ssg);
                    _db.SaveChanges();
                }
            }
        }

        //按topicId和小组Id删除SeminarGroupTopic
        public void DeleteSeminarGroupTopic(long topicId, long groupId)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                SeminarGroupTopic seminarGroupTopic = _db.SeminarGroupTopic.Where(s => s.Topic.Id == topicId && s.SeminarGroup.Id==groupId).SingleOrDefault<SeminarGroupTopic>();
                try
                {
                    List<StudentScoreGroup> list = _db.StudentScoreGroup.Include(s => s.SeminarGroupTopic).Where(t => t.SeminarGroupTopic.Id == seminarGroupTopic.Id).ToList<StudentScoreGroup>();
                    foreach (var l in list)
                    {
                        _db.StudentScoreGroup.Remove(l);
                    }
                    _db.SeminarGroupTopic.Remove(seminarGroupTopic);
                    _db.SaveChanges();
                    scope.Commit();
                }
                catch (System.Exception e)
                {
                    scope.Rollback();
                    throw e;
                }
            }
        }

        //根据小组id获取该小组该堂讨论课所有选题信息
        public List<SeminarGroupTopic> FindSeminarGroupTopicByGroupId(long id)
        {
            List<SeminarGroupTopic> list = _db.SeminarGroupTopic.Include(s => s.SeminarGroup).Where(s => s.SeminarGroup.Id == id).ToList<SeminarGroupTopic>();
            if (list == null) throw new SeminarNotFoundException();
            return list;
        }
    }
}
