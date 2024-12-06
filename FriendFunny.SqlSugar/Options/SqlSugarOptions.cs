using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendFunny.SqlSugar.Options
{
    public class SqlSugarOptions
    {
        /// <summary>
        /// 是否启用SQL日志
        /// </summary>
        public bool EnableSqlLogger { get; set; }

        /// <summary>
        /// 连接字符串集合
        /// </summary>
        public DbConnectionConfig ConnectionConfig { get; set; }
    }

    public class DbConnectionConfig
    {
        /// <summary>
        /// 数据库类型 默认值：Mysql
        /// </summary>
        public DbType DbType { get; set; } = DbType.MySql;

        /// <summary>
        /// 是否启用动态数据源，
        /// 默认值：false
        /// </summary>
        public bool IsDynamicDataSource { get; set; } = false;

        /// <summary>
        /// 使用散列算法时，散列的因子
        /// 如：当时用Hash算法散列数据时，该因子值为散列的取摩的除数
        /// 默认值：128
        /// </summary>
        public int HashFactor { get; set; } = 128;

        /// <summary>
        /// 当Connection string 中未指定Database Name时，则该字段必须要赋值
        /// 该字段指定的是需要执行当前数据库T-SQL脚本的数据库名前缀
        /// 该字段主要用于实现Saas系统中，动态数据库的效果
        /// </summary>
        public string DatabaseNamePrefix { get; set; } = string.Empty;

        /// <summary>
        /// 主数据库配置
        /// </summary>
        public MasterDbConfig MasterDatabase { get; set; }

        /// <summary>
        /// 从数据库配置
        /// </summary>
        public IList<SlaveDbConfig> SlaveDatabase { get; set; }
    }

    /// <summary>
    /// 主库配置
    /// </summary>
    public class MasterDbConfig
    {
        private string _serveraddr = string.Empty;

        /// <summary>
        /// 数据库服务地址
        /// 默认值：<Empty>
        /// </summary>
        public string ServerAddr 
        { 
            get { return _serveraddr; } 
            set { _serveraddr = value; } 
        }


        private int _port = 3306;

        /// <summary>
        /// 数据端口
        /// 默认值：3306
        /// </summary>
        public int Port 
        { 
            get { return _port; } 
            set { _port = value; } 
        }

        private string _user = string.Empty;

        /// <summary>
        /// 数据库用户名
        /// 默认值：<Empty>
        /// </summary>
        public string User 
        { 
            get { return _user; } 
            set { _user = value; } 
        }

        private string _password = string.Empty;

        /// <summary>
        /// 数据库密码
        /// 默认值：<Empty>
        /// </summary>
        public string Password 
        { 
            get { return _password; } 
            set { _password = value; } 
        }

        private string _databasename = string.Empty;

        /// <summary>
        /// 需要连接的数据库名称
        /// 默认值：<Empty>
        /// </summary>
        public string DatabaseName 
        { 
            get { return _databasename; } 
            set { _databasename = value; } 
        }

        private bool _pooling = true;

        /// <summary>
        /// 是否开启ADO.NET连接池
        /// 默认值：true
        /// </summary>
        public bool Pooling 
        { 
            get { return _pooling; } 
            set { _pooling = value; } 
        }

        private int _minpoolsize = 2;

        /// <summary>
        /// ADO.NET连接池的最小连接数，5-8没有与SqlServer发生交互时，存活的连接数
        /// 默认值：2
        /// </summary>
        public int MinPoolSize 
        { 
            get { return _minpoolsize; } 
            set { _minpoolsize = value; } 
        }

        private int _maxpoolsize = 5;

        /// <summary>
        /// ADO.NET连接池的最大连接数，可以并发执行的最大命令数,超过则排队，直至超时
        /// 最大连接数的值不是越大越好，算法：连接数 = ((核心数 * 2) + 有效磁盘数)
        /// 核心数不应包含超线程(hyper thread)，即使打开了hyperthreading也是
        /// 最大连接数的值不能小于MinPoolSize
        /// 默认值：5
        /// </summary>
        public int MaxPoolSize 
        { 
            get { return _maxpoolsize; } 
            set { _maxpoolsize = value; }
        }
    }

    /// <summary>
    /// 从库配置
    /// </summary>
    public class SlaveDbConfig : MasterDbConfig
    {
        private int _hitrate = 1;

        /// <summary>
        /// 数据库是主从模式的时候，该属性为从库专用
        /// 查询走从库，事务内都走主库，HitRate表示权重 值越大执行的次数越高
        /// 默认值：1
        /// </summary>
        public int HitRate 
        { 
            get { return _hitrate; }
            set { _hitrate = value; } 
        }
    }
}
