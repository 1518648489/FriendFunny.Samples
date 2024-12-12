using FriendFunny.App;
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
        /// 
        /// </summary>
        public Repository(ISqlSugarClient context) 
        {
            Context = context;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Context.Ado.Dispose();
            Context?.Dispose();
        }
    }
}