using Xmu.Crms.Services.Group1;
using Xmu.Crms.Services.Group1.Dao;
using Xmu.Crms.Shared.Service;

namespace Microsoft.Extensions.DependencyInjection
{ 
    public static class Group1Extensions
    {
        // 为每一个你写的Service写一个这样的函数
       
        //school 注入
        public static IServiceCollection AddGroup1SchoolService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ISchoolService, SchoolService>();
        }
        public static IServiceCollection AddGroup1SchoolDao(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ISchoolDao, SchoolDao>();
        }

        //Topic注入
        public static IServiceCollection AddGroup1TopicService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ITopicService, TopicService>();
        }
        public static IServiceCollection AddGroup1TopicDao(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ITopicDao, TopicDao>();
        }

        //User注入
        public static IServiceCollection AddGroup1UserService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<IUserService, UserService>();
        }
        public static IServiceCollection AddGroup1UserDao(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<IUserDao, UserDao>();
        }

        //Course注入
        //public static IServiceCollection AddGroup1CourseService(this IServiceCollection serviceCollection)
        //{
        //    return serviceCollection.AddScoped<ICourseService, CourseService>();
        //}
        //public static IServiceCollection AddGroup1CourseDao(this IServiceCollection serviceCollection)
        //{
        //    return serviceCollection.AddScoped<ICourseDao, CourseDao>();
        //}
    }
}
