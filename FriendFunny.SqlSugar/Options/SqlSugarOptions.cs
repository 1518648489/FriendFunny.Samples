using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendFunny.SqlSugar.Options
{
    /// <summary>
    /// SqlSugar 配置
    /// </summary>
    public class SqlSugarOptions
    {
        /// <summary>
        /// 是否启用SQL日志
        /// </summary>
        public bool EnableSqlLogger { get; set; }

        /// <summary>
        /// SQL执行Time超时打印Log配置秒
        /// </summary>
        public int SqlExecutionSecond { get; set; }

        /// <summary>
        /// 连接集合
        /// </summary>
        public List<ConnectionConfig> ConnectionConfigs { get; set; }
    }
}
