﻿/*************/
/*************/
/*@author 1-4*/
/*************/
/*************/
using System;
using System.Collections.Generic;
using System.Text;
using Xmu.Crms.Services.Group1.Dao;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Xmu.Crms.Shared.Exceptions;

namespace Xmu.Crms.Services.Group1
{
    class UserService : IUserService
    {
        private readonly IUserDao _userDao;
        private readonly ICourseService _courseService;
        public UserService(IUserDao userDao, ICourseService courseService)
        {
            _userDao = userDao;
            _courseService = courseService;
        }

        // 根据用户Id获取用户的信息
        public UserInfo GetUserByUserId(long userId)
        {
            if (userId.GetType().ToString() != "System.Int64")
                throw new UserNotFoundException();//id格式错误 
            try
            {
                UserInfo userInfo = _userDao.Find(userId);
                return userInfo;
            }
            catch(UserNotFoundException e)
            {
                throw e;
            }            
        }

        // InsertAttendanceById中用到的方法
        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        // 根据班级id，讨论课id，学生id，经度，纬度进行签到 在方法中通过班级id，讨论课id获取当堂课发起签到的位置
        public void InsertAttendanceById(long classId, long seminarId, long userId, double longitude, double latitude)
        {
            if (seminarId.GetType().ToString() != "System.Int64" || classId.GetType().ToString() != "System.Int64"
                || userId.GetType().ToString() != "System.Int64")
                throw new ArgumentException();//id格式错误
            Location location = _userDao.FindTeacherLocation(classId, seminarId);
           
            double tLongtitude = (double)location.Longitude;
            double tLatitude = (double)location.Latitude;
            if((int)tLongtitude == (int)longitude && (int)tLatitude == (int)latitude)
            {
                Attendance attendance = new Attendance();
                if (location.Status == 0)
                    attendance.AttendanceStatus = Shared.Models.AttendanceStatus.Late;
                else
                    attendance.AttendanceStatus = Shared.Models.AttendanceStatus.Present;
                attendance.Seminar = _userDao.GetSeminar(seminarId);
                attendance.ClassInfo = _userDao.GetClass(classId);
                attendance.Student = _userDao.Find(userId);
                _userDao.AddAttendance(attendance);
            }            
        }

        //list 处于缺勤状态的学生列表
        public IList<UserInfo> ListAbsenceStudent(long seminarId, long classId)
        {
            if (seminarId.GetType().ToString() != "System.Int64" || classId.GetType().ToString() != "System.Int64")
                throw new ArgumentException();//id格式错误
            IList<UserInfo> list  = _userDao.FindAbsenceStudents(seminarId, classId);

            return list;
        }

        // 根据班级id，讨论课id获取当堂课签到信息
        public IList<Attendance> ListAttendanceById(long classId, long seminarId)
        {
            if (seminarId.GetType().ToString() != "System.Int64" || classId.GetType().ToString() != "System.Int64")
                throw new ArgumentException();//id格式错误
            IList < Attendance > list= _userDao.FindAttendanceById(seminarId, classId);

            return list;
        }

        //根据教师名称列出课程名称
        public IList<Course> ListCourseByTeacherName(string teacherName)
        {
            IList<UserInfo> users = ListUserByUserName(teacherName);
            IList<Course> list=new List<Course>();
            foreach (UserInfo u in users)
            {
                if (u.Type == Shared.Models.Type.Teacher)
                {
                    IList<Course> temp = _courseService.ListCourseByUserId(u.Id);
                    foreach (var c in temp)
                        list.Add(c);
                }
            }
            return list;
        }

        //list 处于迟到状态的学生的列表
        public IList<UserInfo> ListLateStudent(long seminarId, long classId)
        {
            if (seminarId.GetType().ToString() != "System.Int64" || classId.GetType().ToString() != "System.Int64")
                throw new ArgumentException();//id格式错误
            IList<UserInfo> list = _userDao.FindLateStudents(seminarId, classId);

            return list;
        }


        public IList<UserInfo> ListPresentStudent(long seminarId, long classId)
        {
            if (seminarId.GetType() != typeof(long) || classId.GetType() != typeof(long)) throw new ArgumentException();
            IList<Attendance> AttendanceList = ListAttendanceById(classId, seminarId);
            IList<UserInfo> PresentStudentList = new List<UserInfo>();
            foreach (Attendance Temp in AttendanceList)
            {
                if (Temp.AttendanceStatus == 0)
                    PresentStudentList.Add(GetUserByUserId(Temp.Student.Id));
            }
            if (PresentStudentList == null) throw new SeminarNotFoundException();

            return PresentStudentList;

        }

        public IList<UserInfo> ListUserByClassId(long classId, string numBeginWith, string nameBeginWith)
        {
            if (classId.GetType() != typeof(long) || numBeginWith.GetType() != typeof(string) || nameBeginWith.GetType() != typeof(string)) throw new ArgumentException();
            IList<UserInfo> StudentInfoList = _userDao.ListUserByClassId(classId, numBeginWith, nameBeginWith);
            if (StudentInfoList == null) throw new ClassNotFoundException();
            return StudentInfoList;
            //throw new NotImplementedException();
        }


        public IList<UserInfo> ListUserByUserName(string userName)
        {
            if (userName.GetType() != typeof(string)) throw new ArgumentException();
            IList<UserInfo> userList = _userDao.ListUserByUserName(userName);
            if (userList == null) throw new UserNotFoundException();
            return userList;

        }

        public IList<long> ListUserIdByUserName(string userName)
        {
            if (userName.GetType() != typeof(string)) throw new ArgumentException();
            IList<long> userIdList = _userDao.ListUserIdByUserName(userName);
            if (userIdList == null) throw new UserNotFoundException();
            return userIdList;

        }

        public void UpdateUserByUserId(long userId, UserInfo user)
        {
            if (userId.GetType() != typeof(long) || user.GetType() != typeof(UserInfo)) throw new ArgumentException();
            _userDao.UpdateUserByUserId(userId, user);

        }

        public UserInfo GetUserByUserNumber(string number)
        {
            return _userDao.GetByNumber(number);
        }
    }
}
