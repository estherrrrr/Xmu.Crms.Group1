using System;
using System.Collections.Generic;
using System.Transactions;
using Xmu.Crms.Services.ViceVersa.Daos;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.ViceVersa
{
    class ClassService : IClassService
    {
        private readonly ISeminarService _seminarService;
        private readonly IUserService _userService;
        private readonly IFixGroupService _fixGroupService;

        private readonly IClassDao _classDao;
        public  ClassService(IClassDao classDao)
        {
            _classDao = classDao;
            //_seminarService = seminarService;
        }


        /// 按班级id删除班级.(包括学生选课表)
        public void DeleteClassByClassId(long classId)
        {
                try
                {
                _classDao.Delete(classId);
                    
                }
                catch (ClassNotFoundException ec)
                {
                    
                    throw ec;
                }
            
        }


        /// 按courseId删除Class.
        public void DeleteClassByCourseId(long courseId)
        {
            try
            {
                //根据课程id获得所有班级信息
                List<ClassInfo> deleteClasses = _classDao.QueryAll(courseId);
                foreach (ClassInfo c in deleteClasses)
                {
                    _fixGroupService.DeleteFixGroupByClassId(c.Id);
                   // 根据class信息删除courseSelection表的记录 并删除班级
                    _classDao.Delete(c.Id);
                }
            }catch(CourseNotFoundException e) { throw e; }
        }


        /// 按classId删除CourseSelection表的一条记录.  ？？和取消选课的区别
        public void DeleteClassSelectionByClassId(long classId)
        {
            _classDao.DeleteSelection(0, classId);
        }


        /// 学生按班级id取消选择班级.
        public void DeleteCourseSelectionById(long userId, long classId)
        {
            try
            {
                //_userService.GetUserByUserId(userId);
                GetClassByClassId(classId);
                _classDao.DeleteSelection(userId, classId);

            }catch(UserNotFoundException eu) { throw eu; }
            catch (ClassNotFoundException ec) { throw ec; }
        }


        /// 按classId删除ScoreRule.
        public void DeleteScoreRuleById(long classId)
        {
            try
            {
                ClassInfo newclass = new ClassInfo() { Id=classId};
                newclass.ReportPercentage = 0;
                newclass.PresentationPercentage = 0;
                newclass.FivePointPercentage = 0;
                newclass.FourPointPercentage = 0;
                newclass.ThreePointPercentage = 0;
                var result = _classDao.Update(newclass);
                _classDao.Update(newclass);
            }
            catch (ClassNotFoundException e) { throw e; }
        }


        /// 老师获取该班级签到、分组状态.
        public Location GetCallStatusById(long seminarId, long classId)
        {
            try
            {
                _seminarService.GetSeminarBySeminarId(seminarId);
                //_classDao.Get(classId);
                return _classDao.GetLocation(seminarId, classId);
            }
            catch (SeminarNotFoundException e) { throw e; }
        }


        /// 按班级id获取班级详情.
         public ClassInfo GetClassByClassId(long classId)
        {

            try
            {
                ClassInfo classinfo = _classDao.Get(classId);
                return classinfo;
            }
            catch (ClassNotFoundException e) { throw e; }
        }


        /// 查询评分规则.
        public ClassInfo GetScoreRule(long classId)
        {
            try
            {
                return _classDao.Get(classId);
            }
            catch (ClassNotFoundException e) { throw e; }
        }

        

        /// 学生按班级id选择班级.成功返回选课记录id 失败返回0
        public long InsertCourseSelectionById(long userId, long classId)
        {
            try
            {
                //_userService.GetUserByUserId(userId);
                ClassInfo classinfo= GetClassByClassId(classId);

                //找到该班级所属课程下的所有班级
                IList<ClassInfo> classList = ListClassByCourseId(classinfo.Course.Id);
                foreach(ClassInfo c in classList)
                {
                    if (_classDao.GetSelection(userId, c.Id) != 0)//学生已选同课程下其他班级
                        return 0;
                }
               CourseSelection coursesele = new CourseSelection();

                
                UserInfo student = _userService.GetUserByUserId(userId);
                coursesele.Student = student;
                coursesele.ClassInfo = classinfo;
                return _classDao.InsertSelection(coursesele);
            }
            catch (UserNotFoundException eu) { throw eu; }
            catch(ClassNotFoundException ec) { throw ec; }
           
        }


        /// 新增评分规则.  返回班级id
        public long InsertScoreRule(long classId, ClassInfo proportions)
        {
            try
            {
                if (proportions.ReportPercentage < 0 || proportions.ReportPercentage > 100 ||
                    proportions.PresentationPercentage < 0 || proportions.PresentationPercentage > 100 ||
                    proportions.ReportPercentage + proportions.PresentationPercentage != 100 ||
                    proportions.FivePointPercentage < 0 || proportions.FivePointPercentage > 10 ||
                    proportions.FourPointPercentage < 0 || proportions.FourPointPercentage > 10 ||
                    proportions.ThreePointPercentage < 0 || proportions.ThreePointPercentage > 10 ||
                    proportions.FivePointPercentage + proportions.FourPointPercentage + proportions.ThreePointPercentage != 10)
                    throw new InvalidOperationException();
                var result = _classDao.Update(proportions);//新建班级时已经建了一个空的
                if (result != 0) return -1;
                return classId;
            }
            catch (InvalidOperationException ei) { throw ei; }
            catch (ClassNotFoundException ec) { throw ec; }

        }


        /// 根据课程ID获得班级列表.
        public IList<ClassInfo> ListClassByCourseId(long courseId)
        {
            try
            {
                List<ClassInfo> list = _classDao.QueryAll(courseId);
                return list;
            }
            catch (CourseNotFoundException e) { throw e; }
            catch (ClassNotFoundException e) { throw e; }
        }
        

        public void UpdateClassByClassId(long classId, ClassInfo newclass)
        {
            try
            {
                var result = _classDao.Update(newclass);  //return 0成功更新
            }
            catch (ClassNotFoundException e) { throw e; }
        }


        /// 修改评分规则.
        public void UpdateScoreRule(long classId, ClassInfo proportions)
        {
            try
            {
                if (proportions.ReportPercentage < 0 || proportions.ReportPercentage > 100 ||
                    proportions.PresentationPercentage < 0 || proportions.PresentationPercentage > 100 ||
                    proportions.ReportPercentage + proportions.PresentationPercentage != 100 ||
                    proportions.FivePointPercentage < 0 || proportions.FivePointPercentage > 10 ||
                    proportions.FourPointPercentage < 0 || proportions.FourPointPercentage > 10 ||
                    proportions.ThreePointPercentage < 0 || proportions.ThreePointPercentage > 10 ||
                    proportions.FivePointPercentage + proportions.FourPointPercentage + proportions.ThreePointPercentage != 10)
                    throw new InvalidOperationException();
                var result = _classDao.Update(proportions);//新建班级时已经建了一个空的
                //if (result != 0) return -1;
                //return classId;
            }
            catch (InvalidOperationException ei) { throw ei; }
            catch (ClassNotFoundException ec){ throw ec; }
        }

        //根据学生ID获取班级列表.
        public List<ClassInfo> ListClassByUserId(long userId)
        {
            try
            {
                return _classDao.ListClassByUserId(userId);
            }
            catch (ClassNotFoundException e) { throw e; }
        }

        //老师发起签到.
        public long CallInRollById(Location location)
        {
            try
            {
                location.Seminar = _seminarService.GetSeminarBySeminarId(location.Seminar.Id);
                location.ClassInfo = GetClassByClassId(location.ClassInfo.Id);
                location.Status = 1;
                return _classDao.InsertLocation(location);

            }
            catch(SeminarNotFoundException es) { throw es; }
            catch(ClassNotFoundException ec) { throw ec; }
        }

        //老师结束签到.
        public void EndCallRollById(long seminarId, long classId)
        {
            try
            {
                //_seminarService.GetSeminarBySeminarId(seminarId);
                GetClassByClassId(classId);
                
               
                _classDao.UpdateLocation(seminarId,classId);

            }
            catch (SeminarNotFoundException es) { throw es; }
            catch (ClassNotFoundException ec) { throw ec; }
        }
    }
}
