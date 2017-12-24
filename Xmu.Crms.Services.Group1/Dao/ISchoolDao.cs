using System;
using System.Collections.Generic;
using System.Text;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Services.Group1.Dao
{
    interface ISchoolDao
    {
        //按照城市名字列出所有学校
        List<School> FindAllByCity(string city);

        //找出所有学校
        List<School> FindAll();

        //通过Id查找学校
        School Find(long id);

        //按照省份名字列出所有学校
        List<School> FindAllByProvince(string province);

        //插入学校
        long AddSchool(School school);

    }
}
