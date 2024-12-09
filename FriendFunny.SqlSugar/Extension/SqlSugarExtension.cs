using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace FriendFunny.SqlSugar.Extension
{
    /// <summary>
    /// SqlSugar Extension
    /// </summary>
    public static class SqlSugarExtension
    {


        /// <summary>
        /// SqlSugar 上下文初始化
        /// </summary>
        /// <param name="services"></param>
        public static void AddSqlSugar(this IServiceCollection services,Action<IdGeneratorOptions> option)
        {
            // 雪花Id
            var snowIdOption = new IdGeneratorOptions()
            {

            };

            YitIdHelper.SetIdGenerator(snowIdOption);

            // 自定义 SqlSugar 雪花ID算法
            SnowFlakeSingle.WorkId = snowIdOption.WorkerId;
            StaticConfig.CustomSnowFlakeFunc = YitIdHelper.NextId;

            services.AddScoped(typeof(IReportable<>), typeof(Repository<>));
        }
    }
}
