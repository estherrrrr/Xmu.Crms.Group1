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
    class UserDao:IUserDao
    {
        private readonly CrmsContext _db;

        public UserDao(CrmsContext db)
        {
            _db = db;
        }

        //通过Id查找用户
        public UserInfo Find(long userId)
        {
            UserInfo userInfo = _db.UserInfo.Include(u=>u.School).FirstOrDefault(u => u.Id == userId);
            if (userInfo == null) throw new UserNotFoundException();
            return userInfo;
        }
        //查找这个班所有缺勤学生
        public List<UserInfo> FindAbsenceStudents(long seminarId, long classId)
        {
            List<UserInfo> list = (from s in _db.Attendences.Where(s => s.ClassInfo.Id == classId && s.Seminar.Id == seminarId && s.AttendanceStatus == AttendanceStatus.Absent)
                                   select s.Student).ToList<UserInfo>();
            return list;
        }
        //根据班级号和讨论课号查找出勤记录
        public IList<Attendance> FindAttendanceById(long seminarId, long classId)
        {
            List<Attendance> list =  _db.Attendences.Include(a=>a.Student).Where(s => s.ClassInfo.Id == classId && s.Seminar.Id == seminarId).ToList<Attendance>();
            return list;
        }

       
        //根据班级号和讨论课号查找迟到学生
        public IList<UserInfo> FindLateStudents(long seminarId, long classId)
        {
            List<UserInfo> list = (from s in _db.Attendences.Where(s => s.ClassInfo.Id == classId && s.Seminar.Id == seminarId && s.AttendanceStatus== AttendanceStatus.Late)
                                  select s.Student).ToList<UserInfo>();
            return list;
        }
        //根据班级号和讨论课号查找老师位置
        public Location FindTeacherLocation(long classId, long seminarId)
        {
            Location location = _db.Location.FirstOrDefault(l => l.ClassInfo.Id == classId && l.Seminar.Id == seminarId);
            if (location == null)
            {
                throw new ClassNotFoundException();
                throw new SeminarNotFoundException();
            }
            return location;
        }
        //插入attendance记录
        public void AddAttendance(Attendance attendance)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Attendences.Add(attendance);
                    _db.SaveChanges();
                }
                catch(System.Exception e)
                {
                    scope.Rollback();
                    throw e;
                }
            }
        }


        public IList<UserInfo> ListUserByClassId(long classId, string numBeginWith, string nameBeginWith)
        {
            List<CourseSelection> CourseSelectionList = _db.CourseSelection.Include(u => u.ClassInfo).Include(u => u.Student).Where(u => u.ClassInfo.Id == classId && u.Student.Number.StartsWith(numBeginWith) && u.Student.Name.StartsWith(nameBeginWith)).ToList<CourseSelection>();
            List<UserInfo> StudentInfoList = new List<UserInfo>();
            UserInfo tempUserInfo = new UserInfo();
            foreach (CourseSelection temp in CourseSelectionList)
            {
                //tempUserInfo = _db.UserInfo.First(u => u.Id == temp.Student.Id);

                StudentInfoList.Add(temp.Student);
            }
            return StudentInfoList;

        }

        public IList<UserInfo> ListUserByUserName(string userName)
        {
            List<UserInfo> list = _db.UserInfo.Where(u => u.Name == userName).ToList<UserInfo>();
            return list;
        }

        public IList<long> ListUserIdByUserName(string userName)
        {
            List<long> list = _db.UserInfo.Where(u => u.Name == userName).Select(u => u.Id).ToList<long>();
            return list;
        }

        public void UpdateUserByUserId(long userId, UserInfo newUserInfo)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    UserInfo userInfo = _db.UserInfo.First(u => u.Id == userId);
                    if (userInfo == null) throw new UserNotFoundException();
                    //用修改后的值给修改前的值赋值
                    userInfo.Avatar = newUserInfo.Avatar;
                    userInfo.Education = newUserInfo.Education;
                    userInfo.Email = newUserInfo.Email;
                    userInfo.Gender = newUserInfo.Gender;
                    userInfo.Name = newUserInfo.Name;
                    userInfo.Number = newUserInfo.Number;
                    userInfo.Password = newUserInfo.Password;
                    userInfo.Phone = newUserInfo.Phone;
                    userInfo.School = newUserInfo.School;
                    userInfo.Title = newUserInfo.Title;
                    userInfo.Type = newUserInfo.Type;
                    _db.Entry(userInfo).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                catch(System.Exception e)
                {
                    scope.Rollback();
                    throw e;
                }
            }
            
        }

       
    }
}
