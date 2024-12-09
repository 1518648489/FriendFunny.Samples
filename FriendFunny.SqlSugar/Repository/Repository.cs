using FriendFunny.SqlSugar.Options;
using Microsoft.Extensions.Options;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace FriendFunny.SqlSugar.Extension
{
    /// <summary>
    /// SqlSugar仓储
    /// </summary>
    public class Repository<T> : SimpleClient<T>, IRepository<T>, IDisposable where T : class, new()
    {
        /// <summary>
        /// Sqlsugar Option 
        /// </summary>
        private readonly SqlSugarOptions _options;

        /// <summary>
        /// DI
        /// </summary>
        public Repository(IOptionsMonitor<SqlSugarOptions> option)
        {
            _options = option.CurrentValue;
            base.Context = GetSqlSugarClient();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ISqlSugarClient GetSqlSugarClient()
        {
            if (_options == null || _options.ConnectionConfig == null)
                throw new Exception("SqlSugar Config Not Fund");

            // 从库集合
            var slaveConnectionConfigs = new List<SlaveConnectionConfig>();
            Check.Exception(_options == null && _options.ConnectionConfig.MasterDatabase == null, "ConnectionConfig config is null");
            
            // 主库连接字符串
            string masterConnectionString = GetConnectionString(_options.ConnectionConfig.MasterDatabase, _options.ConnectionConfig.DbType);
            if (_options.ConnectionConfig.SlaveDatabase != null)
            {
                // 从库配置
                foreach (var option in _options.ConnectionConfig.SlaveDatabase)
                {
                    slaveConnectionConfigs.Add(new SlaveConnectionConfig()
                    {
                        // 查询走从库，事务内都走主库，HitRate表示权重 值越大执行的次数越高
                        HitRate = option.HitRate,
                        ConnectionString = GetConnectionString(option, _options.ConnectionConfig.DbType)
                    });
                }
            }

            // 如果配置了 SlaveConnectionConfigs那就是主从模式,所有的写入删除更新都走主库，查询走从库，
            // 事务内都走主库，HitRate表示权重 值越大执行的次数越高，如果想停掉哪个连接可以把HitRate设为0 
            var DbContext = new SqlSugarClient(new ConnectionConfig()
            {
                // 主库连接字符串
                ConnectionString = masterConnectionString,
                // 数据库类型
                DbType = _options.ConnectionConfig.DbType,
                //该设置表示业务完成后是否需要调用connection.close()函数。
                //如果程序并发不大的时候可以考虑设置成False，但要小心Throw Exception：There is already an open DataReader associated with this Connection which must be closed first.
                //但是如果连接字符串中设置pooling=true以后，则ADO.NET底层帮我们开启了连接池，业务完成以后connection.close()也就不是真正的关闭了物理连接，只是把我们的连接放到池里面。
                //当我们下一次connection.open()就会从池中找一个空闲的物理连接给我们，如果没有就会新建。但是超过我们设置max pool size 以后，就会出现wait。
                IsAutoCloseConnection = true,
                //如果配置了 SlaveConnectionConfigs那就是主从模式,所有的写入删除更新都走主库，
                //查询走从库，事务内都走主库，HitRate表示权重 值越大执行的次数越高，
                //如果想停掉哪个连接可以把HitRate设为0
                SlaveConnectionConfigs = slaveConnectionConfigs,
                //IsShardSameThread = true,
                InitKeyType = InitKeyType.Attribute,
                //动态切换数据源的参数配置
                //IsDynamicDataSource = _options.IsDynamicDataSource,
                //散列算法
                //AlgorithmEnum = _options.HashAlgorithmEnum,
                //当使用直接指定的尾缀时，该散列因子可以不用传
                //当时用Hash算法散列数据时，该因子值为散列的取摩的除数
                //HashFactor = _options.HashFactor,
                //DatabaseNamePrefix = _options.DatabaseNamePrefix
            });

            // 设置超时时间
            DbContext.Ado.CommandTimeOut = 60000;
           
            DbContext.Aop.OnLogExecuted = (sql, pars) => 
            {
                // TODO：SQL执行完事件
            };

            DbContext.Aop.OnLogExecuting = (sql, pars) => 
            {
                // TODO：SQL执行前事件
                if (DbContext.TempItems == null) DbContext.TempItems = new Dictionary<string, object>();
            };

            DbContext.Aop.OnError = (exp) =>
            {
                // 执行SQL错误事件
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

            return DbContext;
        }

        /// <summary>
        ///  获得SqlSugarClient
        /// </summary>
        private string GetConnectionString(MasterDbConfig config, DbType dbType)
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


        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Context.Ado.Dispose();
            Context?.Dispose();
        }
    }
}