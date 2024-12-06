using SqlSugar;

namespace FriendFunny.SqlSugar.Extension
{
    /// <summary>
    /// SqlSugar操作接口类
    /// </summary>
    public interface IRepository<T> : ISimpleClient<T> where T : class, new()
    {

    }
}
