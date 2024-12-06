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
            //// 注册雪花Id
            //var snowIdOpt = App.GetConfig<IdGeneratorOptions>("SnowId", true);
            //YitIdHelper.SetIdGenerator(snowIdOpt);

            //// 自定义 SqlSugar 雪花ID算法
            //SnowFlakeSingle.WorkId = snowIdOpt.WorkerId;
            //StaticConfig.CustomSnowFlakeFunc = () =>
            //{
            //    return YitIdHelper.NextId();
            //};
            //// 动态表达式 SqlFunc 支持，https://www.donet5.com/Home/Doc?typeId=2569
            //StaticConfig.DynamicExpressionParserType = typeof(DynamicExpressionParser);
            //StaticConfig.DynamicExpressionParsingConfig = new ParsingConfig
            //{
            //    CustomTypeProvider = new SqlSugarTypeProvider()
            //};

            //var dbOptions = App.GetConfig<DbConnectionOptions>("DbConnection", true);
            //dbOptions.ConnectionConfigs.ForEach(SetDbConfig);

            //SqlSugarScope sqlSugar = new(dbOptions.ConnectionConfigs.Adapt<List<ConnectionConfig>>(), db =>
            //{
            //    dbOptions.ConnectionConfigs.ForEach(config =>
            //    {
            //        var dbProvider = db.GetConnectionScope(config.ConfigId);
            //        SetDbAop(dbProvider, dbOptions.EnableConsoleSql);
            //        SetDbDiffLog(dbProvider, config);
            //    });
            //});
            //ITenant = sqlSugar;

            //services.AddSingleton<ISqlSugarClient>(sqlSugar); // 单例注册
            //services.AddScoped(typeof(SqlSugarRepository<>)); // 仓储注册
            //services.AddUnitOfWork<SqlSugarUnitOfWork>(); // 事务与工作单元注册

            //// 初始化数据库表结构及种子数据
            //dbOptions.ConnectionConfigs.ForEach(config =>
            //{
            //    InitDatabase(sqlSugar, config);
            //});
        }
    }
}
