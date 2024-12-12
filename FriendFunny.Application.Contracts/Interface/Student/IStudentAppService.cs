using FriendFunny.Application.Contracts.Interface.Student.Dto;
using FriendFunny.DependencyInjection.Inteface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendFunny.Application.Contracts.Interface.Student
{
    /// <summary>
    /// 学生相关接口
    /// </summary>
    public interface IStudentAppService
    {
        /// <summary>
        /// 创建学生
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task CreateAsync(CreateStudentDto dto);
    }
}
