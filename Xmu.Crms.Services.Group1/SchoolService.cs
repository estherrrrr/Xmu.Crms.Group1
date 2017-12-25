/*************/
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

namespace Xmu.Crms.Services.Group1
{
    class SchoolService : ISchoolService
    {
        private readonly ISchoolDao _schoolDao;
        public SchoolService(ISchoolDao schoolDao)
        {
            _schoolDao = schoolDao;
        }
        public School GetSchoolBySchoolId(long schoolId)
        {
            School school = _schoolDao.Find(schoolId);
            return school;
        }

        public long InsertSchool(School school)
        {
            return _schoolDao.AddSchool(school);
        }

        public IList<string> ListCity(string province)
        {
            IList<string> city=new List<string>();
            List<School> school = _schoolDao.FindAllByProvince(province);
            foreach(School s in school)
            {
                if (!city.Contains(s.City))
                    city.Add(s.City);
            }
            return city;
        }

        public IList<string> ListProvince()
        {
            IList<School> school = _schoolDao.FindAll();
            IList<string> province=new List<string>();
            foreach(School s in school)
            {
                if (!province.Contains(s.Province))
                    province.Add(s.Province);
            }
            return province;
        }

        public IList<School> ListSchoolByCity(string city)
        {
            IList<School> list = _schoolDao.FindAllByCity(city);
            return list;
        }
    }
}
