﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Services.ViceVersa
{
    class ClassDao : IClassDao
    {
        private readonly CrmsContext _db;

        public ClassDao(CrmsContext db)
        {
            _db = db;
        }

        //删除班级和学生选课表
        public void Delete(long id)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    ClassInfo c = _db.ClassInfo.Where(u => u.Id == id).SingleOrDefault<ClassInfo>();
                    if (c == null) throw new ClassNotFoundException();

                    //根据class信息删除courseSelection表
                    DeleteSelection(0, c.Id);

                    _db.ClassInfo.Attach(c);
                    _db.ClassInfo.Remove(c);
                    _db.SaveChanges();
                    scope.Commit();
                }
                catch (ClassNotFoundException e) { scope.Rollback(); throw e; }

            }
        }

        public ClassInfo Get(long id)
        {

            ClassInfo classinfo = _db.ClassInfo.Include(u=>u.Course).Where(u => u.Id == id).SingleOrDefault<ClassInfo>();
            if (classinfo == null)
            {
                throw new ClassNotFoundException();
            }
            return classinfo;

        }


        //根据课程id列出所有班级
        public List<ClassInfo> QueryAll(long id)
        {
            //找到这门课
            Course course= _db.Course.SingleOrDefault(u => u.Id == id);
            if (course == null) { throw new CourseNotFoundException(); }


            List<ClassInfo> list = _db.ClassInfo.Include(u => u.Course).Where(u => u.Course.Id == id).ToList<ClassInfo>();
            if (list == null)
            {
                throw new ClassNotFoundException();
            }
            return list;
        }


       
        //添加学生选课表返回id
        public long InsertSelection(CourseSelection t)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {

                    _db.CourseSelection.Add(t);

                    _db.SaveChanges();

                    scope.Commit();
                    return t.Id;
                }
                catch { scope.Rollback(); throw; }
            }

        }

        //查询学生选课表的记录
        public int GetSelection(long userId, long classId)
        {
           

            CourseSelection courseSelection= _db.CourseSelection.Where(u=>u.ClassInfo.Id==classId&&u.Student.Id==userId).SingleOrDefault<CourseSelection>();
            if (courseSelection != null) return 1;//找到记录
            return 0;
        }

        public int Update(ClassInfo t)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {

                    ClassInfo c = _db.ClassInfo.Where(u => u.Id == t.Id).SingleOrDefault<ClassInfo>();
                    if (c == null) throw new ClassNotFoundException();
                    c.Name = t.Name;
                    c.Course = t.Course;
                    c.Site = t.Site;
                    c.ClassTime = t.ClassTime;
                    c.ReportPercentage = t.ReportPercentage;
                    c.PresentationPercentage = t.PresentationPercentage;
                    c.FivePointPercentage = t.FivePointPercentage;
                    c.FourPointPercentage = t.FourPointPercentage;
                    c.ThreePointPercentage = t.ThreePointPercentage;

                    _db.Entry(c).State = EntityState.Modified;
                    _db.SaveChanges();

                    scope.Commit();
                    return 0;
                }
                catch(ClassNotFoundException e) { scope.Rollback();throw e; }
            }
            
        }


        //根据班级id/学生id删除学生选课表
        public void DeleteSelection(long userId, long classId)
        {
            if (userId != 0)//单个学生取消选课
            {
                using (var scope = _db.Database.BeginTransaction())
                {
                    try
                    {
                        CourseSelection c = _db.CourseSelection.SingleOrDefault<CourseSelection>(u => u.Student.Id == userId && u.ClassInfo.Id == classId);

                        _db.CourseSelection.Attach(c);
                        _db.CourseSelection.Remove(c);
                        _db.SaveChanges();
                        scope.Commit();
                    }
                    catch { scope.Rollback(); }
                }
            }

            else  //删除班级时 批量删除
            {
                List<CourseSelection> t1 = _db.CourseSelection.Where(t => t.ClassInfo.Id == classId).ToList<CourseSelection>();
                foreach (CourseSelection t in t1)
                {
                    _db.CourseSelection.Remove(t);
                }
                
                _db.SaveChanges();
            }
        }

        // 根据学生ID获取班级列表
        public List<ClassInfo> ListClassByUserId(long userId)
        {
            List<CourseSelection> selectionList = _db.CourseSelection.Include(c => c.Student).Include(c => c.ClassInfo).Where(c => c.Student.Id == userId).ToList<CourseSelection>();
            //找不到对应的选课信息
            if (selectionList == null)
                throw new ClassNotFoundException();

            //根据classId获得对应的class
            List<ClassInfo> classList = new List<ClassInfo>();
            foreach (CourseSelection i in selectionList)
                classList.Add(Get(i.ClassInfo.Id));
            return classList;
        }

        // 老师获取该班级签到、分组状态.
        public Location GetLocation(long seminarId, long classId)
        {
            return _db.Location.Include(u=>u.ClassInfo).Include(u=>u.Seminar).SingleOrDefault<Location>(u => u.Seminar.Id == seminarId && u.ClassInfo.Id == classId);
        }

        //添加Location表返回id
        public long InsertLocation(Location t)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Location.Add(t);

                    _db.SaveChanges();

                    scope.Commit();
                    return t.Id;
                }
                catch { scope.Rollback(); throw; }
            }

        }

        //结束签到时修改location
        public int UpdateLocation(long seminarId, long classId)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    Location location= _db.Location.Include(u=>u.Seminar).Include(u=>u.ClassInfo).SingleOrDefault<Location>(u=>u.ClassInfo.Id==classId&&u.Seminar.Id==seminarId);
                    //没有记录
                    if (location == null) throw new ClassNotFoundException();

                    location.Status = 0;
                    _db.Entry(location).State = EntityState.Modified;
                    _db.SaveChanges();

                    scope.Commit();
                    return 0;
                }
                catch { scope.Rollback(); throw new ClassNotFoundException(); }
            }
        }
    }
}