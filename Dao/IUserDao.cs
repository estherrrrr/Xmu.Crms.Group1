using System;
using System.Collections.Generic;
using System.Text;
using Xmu.Crms.Shared.Models;
namespace Xmu.Crms.Services.Group1.Dao
{
    interface IUserDao
    {
        //通过Id查用户
        UserInfo Find(long userId);
        //查找这个班所有缺勤学生
        List<UserInfo> FindAbsenceStudents(long seminarId, long classId);
        //根据班级号和讨论课号查找出勤记录
        IList<Attendance> FindAttendanceById(long seminarId, long classId);
        //根据班级号和讨论课号查找迟到学生
        IList<UserInfo> FindLateStudents(long seminarId, long classId);
        //根据班级号和讨论课号查找老师位置
        Location FindTeacherLocation(long classId, long seminarId);
        //插入attendance记录
        void AddAttendance(Attendance attendance);

        //按班级ID、学号开头、姓名开头获取学生列表
        IList<UserInfo> ListUserByClassId(long classId, string numBeginWith, string nameBeginWith);

        //根据用户名获取用户列表
        IList<UserInfo> ListUserByUserName(string userName);

        //根据用户名获取用户列表
        IList<long> ListUserIdByUserName(string userName);

        //修改个人信息
        void UpdateUserByUserId(long userId, UserInfo userInfo);
    }
}
