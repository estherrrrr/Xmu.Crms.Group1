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
using System.Transactions;
using Xmu.Crms.Services.Group1.Dao;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
namespace Xmu.Crms.Services.Group1
{
    class SchoolDao : ISchoolDao
    {
        private readonly CrmsContext _db;

        public SchoolDao(CrmsContext db)
        {
            _db = db;
        }

        //插入学校
        public long AddSchool(School school)
        {
            using (var scope = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.School.Add(school);
                    _db.SaveChanges();
                    return school.Id;
                }
                catch(System.Exception e)
                {
                    scope.Rollback();
                    throw e;
                }
               
            }
                
                
        }


        //通过Id查找学校
        public School Find(long id)
        {
            School school = _db.School.FirstOrDefault(s => s.Id == id);
            return school;
        }

        //找出所有学校
        public List<School> FindAll()
        {
            List<School> list = _db.School.ToList<School>();
            return list;
        }
        //按照城市名字列出所有学校
        public List<School> FindAllByCity(string city)
        {
            List<School> list = _db.School.Where(u => u.City == city).ToList<School>();
            return list;
        }

        //按照省份名字列出所有学校
        public List<School> FindAllByProvince(string province)
        {
            List<School> list = _db.School.Where(u => u.Province == province).ToList<School>();
            return list;
        }

        
    }
}
