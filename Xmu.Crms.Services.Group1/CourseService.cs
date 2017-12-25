using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using Xmu.Crms.Services.ViceVersa.Daos;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.ViceVersa.Services
{
    class CourseService : ICourseService
    {
        private readonly ICourseDao _iCourseDao;
        //private readonly ISeminarService _iSeminarService;
        private readonly IClassService _iClassService;
        //private readonly IUserService _iUserService;

        public CourseService(ICourseDao iCourseDao,IClassService iClassService/*,ISeminarService iSeminarService,IUserService iUserService*/)
        {
            _iCourseDao = iCourseDao;
            //_iSeminarService = iSeminarService;
            _iClassService = iClassService;
            //_iUserService = iUserService;
        }

        public void DeleteCourseByCourseId(long courseId)
        {
            try
            {
                if (courseId < 0)
                    throw new ArgumentException();
                //删除course下的class
                _iClassService.DeleteClassByCourseId(courseId);
                //删除course下的seminar
                //_iSeminarService.DeleteSeminarByCourseId(courseId);
                //删除course
                _iCourseDao.DeleteCourseByCourseId(courseId);
            }
            catch
            {
                throw;
            }
        }

        public Course GetCourseByCourseId(long courseId)
        {
            try
            {
                if (courseId < 0)
                    throw new ArgumentException();
                Course course = _iCourseDao.GetCourseByCourseId(courseId);
                //没查到该门课
                if (course == null)
                {
                    throw new CourseNotFoundException();
                }
                return course;
            }
            catch
            {
                throw;
            }
        }

        public long InsertCourseByUserId(long userId, Course course)
        {
            try
            {
                if (userId < 0)
                    throw new ArgumentException();
                //根据userId找出teacher
                //UserInfo teacher = _iUserService.GetUserByUserId(userId);  //会抛出ArgumentException和UserNotFoundException
                //course.Teacher = teacher;
                long courseId = _iCourseDao.InsertCourseByUserId(course);
                return courseId;
            }catch
            {
                throw;
            }
        }

        public IList<ClassInfo> ListClassByCourseName(string courseName)
        {
            try
            {
                //根据课程名获得对应的课程列表
                IList<Course> courseList = ListCourseByCourseName(courseName);
                //根据课程id获得该课程下的班级
                List<ClassInfo> classList = new List<ClassInfo>();
                foreach (var i in courseList)
                     classList.AddRange( _iClassService.ListClassByCourseId(i.Id));
                return classList;
            }catch
            {
                throw;
            }
        }

        public IList<ClassInfo> ListClassByTeacherName(string teacherName)
        {
            try
            {
                IList<long> idList = null;// _iUserService.ListUserIdByUserName(teacherName);
                if (idList == null || idList.Count == 0)
                    return null;
                List<ClassInfo> classList = new List<ClassInfo>();
                foreach (var i in idList)
                    classList.AddRange(ListClassByUserId(i));
                return classList;
            }catch
            {
                throw;
            }
        }

        //移到classService
        public IList<ClassInfo> ListClassByUserId(long userId)
        {
            try
            {
                if (userId < 0)
                    throw new ArgumentException();
                List<ClassInfo> classList = null;// _iCourseDao.ListClassByUserId(userId);
                //没有查到
                if (classList == null || classList.Count == 0)
                    throw new ClassNotFoundException();
                return classList;
            }
            catch
            {
                throw;
            }
        }

        public IList<Course> ListCourseByCourseName(string courseName)
        {
            try
            {
                IList<Course> courseList = _iCourseDao.ListCourseByCourseName(courseName);
                if (courseList == null || courseList.Count == 0)
                {
                    throw new CourseNotFoundException();
                }
                return courseList;
            }catch
            {
                throw;
            }
        }

        public IList<Course> ListCourseByUserId(long userId)
        {
            try
            {
                if (userId < 0)
                    throw new ArgumentException();
                List<Course> courseList = _iCourseDao.ListCourseByUserId(userId);
                //查不到课程
                if (courseList==null || courseList.Count==0)
                    throw new CourseNotFoundException();
                return courseList;
            }
            catch
            {
                throw;
            }
        }

        public IList<ClassInfo> ListClassByName(string courseName, string teacherName)
        {
            try
            {
                List<ClassInfo> classList = new List<ClassInfo>();
                if (teacherName == null)//根据课程名称查
                {
                    IList<ClassInfo> courseClassList = ListClassByCourseName(courseName);
                    classList.AddRange(courseClassList);
                }
                else if (courseName == null)//根据教师姓名查
                {
                    IList<ClassInfo> teacherClassList = ListClassByTeacherName(teacherName);
                    classList.AddRange(teacherClassList);
                }
                else  //联合查找
                {
                    IList<ClassInfo> courseClassList = ListClassByCourseName(courseName);
                    IList<ClassInfo> teacherClassList = ListClassByTeacherName(teacherName);
                    foreach (ClassInfo cc in courseClassList)
                        foreach (ClassInfo ct in teacherClassList)
                            if (cc.Id== ct.Id) { classList.Add(cc); break; }
                }

                ////该学生已选班级列表
                //List<ClassInfo> studentClass = _classDao.ListClassByUserId(userId);
                //foreach (ClassInfo c in classList)
                //    foreach (ClassInfo cs in studentClass)
                //        if (c.Id == cs.Id) classList.Remove(c);//学生已选的就不列出

                return classList;

            }
            catch (CourseNotFoundException ec) { throw ec; }
            catch (UserNotFoundException eu) { throw eu; }
          
        }

        public void UpdateCourseByCourseId(long courseId, Course course)
        {
            try
            {
                if (courseId < 0)
                    throw new ArgumentException();
                _iCourseDao.UpdateCourseByCourseId(courseId, course);
            }catch
            {
                throw;
            }
        }


        /// 新建班级.
        public long InsertClassById(long courseId, ClassInfo classInfo)
        {
            try
            {
                Course course= GetCourseByCourseId(courseId);
                //检查数据是否合法
                if (classInfo.ReportPercentage < 0 || classInfo.ReportPercentage > 100 ||
                   classInfo.PresentationPercentage < 0 || classInfo.PresentationPercentage > 100 ||
                   classInfo.ReportPercentage + classInfo.PresentationPercentage != 100 ||
                   classInfo.FivePointPercentage < 0 || classInfo.FivePointPercentage > 10 ||
                   classInfo.FourPointPercentage < 0 || classInfo.FourPointPercentage > 10 ||
                   classInfo.ThreePointPercentage < 0 || classInfo.ThreePointPercentage > 10 ||
                   classInfo.FivePointPercentage + classInfo.FourPointPercentage + classInfo.ThreePointPercentage != 10)
                    throw new InvalidOperationException();
                classInfo.Course = course;
                return _iCourseDao.Save(classInfo);    //返回classid

            }
            catch (CourseNotFoundException ec) { throw ec; }
        }

       
    }
}
