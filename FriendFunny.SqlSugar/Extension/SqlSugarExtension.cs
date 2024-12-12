using FriendFunny.App;
using FriendFunny.SqlSugar.Options;
using FriendFunny.Tool.Common;
using FriendFunny.Tool.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        ///  添加SqlSugar
        /// </summary>
        /// <param name="services"></param>
        public static void AddSqlSugar(this IServiceCollection services)
        {
            var options = GlobalAppSettings.GetConfig<SqlSugarOptions>("SqlSugarOptions");
            if (options == null)
                throw new ArgumentNullException("SqlSugar Config is notfund");


            SqlSugarClient sqlSugar = new(options.ConnectionConfigs, db =>
            {
                options.ConnectionConfigs.ForEach(config =>
                {
                    var dbProvider = db.GetConnectionScope(config.ConfigId);
                    AddDbContextAop(db, options);
                });
            });


            services.AddSingleton<ISqlSugarClient>(sqlSugar);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }


        /// <summary>
        /// 添加SqlSugar Aop
        /// </summary>
        /// <param name="DbContext"></param>
        /// <param name="option"></param>
        private static void AddDbContextAop(this SqlSugarClient DbContext,SqlSugarOptions? option = null) 
        {
            if (DbContext == null)
                throw new ArgumentNullException("SqlSugar DbContext Is NotFund");

            if (option == null)
                throw new ArgumentNullException("SqlSugar Option Is NotFund");

            // 设置超时时间
            DbContext.Ado.CommandTimeOut = 60000;

            // Aop配置
            DbContext.Aop.OnLogExecuted = (sql, pars) =>
            {
                // TODO：SQL执行完事件

                if (DbContext.Ado.SqlExecutionTime.TotalSeconds > option.SqlExecutionSecond)
                {
                    // 执行Code FileName
                    var fileName = DbContext.Ado.SqlStackTrace.FirstFileName;
                    // 执行Code Rows
                    var fileLine = DbContext.Ado.SqlStackTrace.FirstLine;
                    // 执行Code MethodName
                    var firstMethodName = DbContext.Ado.SqlStackTrace.FirstMethodName;

                    var logInfo = $"【SQL执行超时】耗时{DbContext.Ado.SqlExecutionTime.TotalSeconds}秒【{sql}】,fileName={fileName},line={fileLine},methodName={firstMethodName}";
                    ConsoleHelper.WriteLog(logInfo, WriteLogTypeEnum.Error);
                }
            };

            DbContext.Aop.OnLogExecuting = (sql, pars) =>
            {
                // TODO：SQL执行前事件
                if (DbContext.TempItems == null) DbContext.TempItems = new Dictionary<string, object>();
            };

            DbContext.Aop.OnError = (exp) =>
            {
                // 执行SQL错误事件
                ConsoleHelper.WriteLog($"【SQL执行错误】错误SQL：{exp.Sql}", WriteLogTypeEnum.Error);
                throw new Exception(exp.Message);
            };

            DbContext.Aop.OnExecutingChangeSql = (sql, pars) =>
            {
                // TODO：自定义输出SQL语句，并且执行前可以修改SQL
                return new KeyValuePair<string, SugarParameter[]>(sql, pars);
            };

            DbContext.Aop.OnDiffLogEvent = (it) =>
            {
                // TODO：可以方便拿到 数据库操作前和操作后的数据变化，用作DataChange
            };            
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        private static string GetConnectionString(MasterDbConfig config, DbType dbType)
        {
            string connectionString = string.Empty;
            switch (dbType)
            {
                case DbType.SqlServer:
                    connectionString = $"server={config.ServerAddr};user id={config.User};password={config.Password};persistsecurityinfo=True;database={config.DatabaseName}";
                    break;
                case DbType.MySql:
                    //可以在连接字符串中设置连接池pooling=true;表示开启连接池
                    //eg:min pool size=2;max poll size=4;表示最小连接池为2，最大连接池是4；默认是100
                    if (string.IsNullOrEmpty(config.DatabaseName))
                        connectionString = $"Server={config.ServerAddr};port={config.Port};Uid={config.User};Pwd={config.Password};charset='utf8';pooling={config.Pooling};min pool size={config.MinPoolSize};max pool size={config.MaxPoolSize};AllowLoadLocalInfile=true;";
                    else
                        connectionString = $"Server={config.ServerAddr};port={config.Port};Database={config.DatabaseName};Uid={config.User};Pwd={config.Password};charset='utf8';pooling={config.Pooling};min pool size={config.MinPoolSize};max pool size={config.MaxPoolSize};AllowLoadLocalInfile=true;";
                    break;
                case DbType.Oracle:
                    connectionString = $"Server=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={config.ServerAddr})(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={config.DatabaseName})));User Id={config.User};Password={config.Password};Persist Security Info=True;Enlist=true;Max Pool Size=300;Min Pool Size=0;Connection Lifetime=300";
                    break;
                case DbType.PostgreSQL:
                    connectionString = $"PORT=5432;DATABASE={config.DatabaseName};HOST={config.ServerAddr};PASSWORD={config.Password};USER ID={config.User}";
                    break;
                case DbType.Sqlite:
                    connectionString = $"Data Source={config.ServerAddr};Version=3;Password={config.Password};";
                    break;
            }
            return connectionString;
        }
    }
}
