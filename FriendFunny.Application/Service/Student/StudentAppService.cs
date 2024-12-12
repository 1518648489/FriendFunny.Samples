using FriendFunny.Application.Contracts.Interface.Student;
using FriendFunny.Application.Contracts.Interface.Student.Dto;
using FriendFunny.Domain.Entity;
using FriendFunny.SqlSugar.Extension;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendFunny.Application.Service.Student
{
    /// <summary>
    /// 学生服务
    /// </summary>
    public class StudentAppService: IStudentAppService
    {
        private readonly IRepository<SysStudent> _studentRepo;

        /// <summary>
        /// 
        /// </summary>
        public StudentAppService(IRepository<SysStudent> studentRepo) 
        {
            _studentRepo = studentRepo;
        }

        /// <summary>
        /// 创建学生
        /// </summary>
        /// <returns></returns>
        public async Task CreateAsync(CreateStudentDto dto) 
        {
            await _studentRepo.InsertAsync(new SysStudent()
            {
                Name = "张三",
                Sex = Domain.Shared.Enum.StudentSexEnum.Male,
                Age = 18,
                CreateTime = DateTime.Now,
            });
        }
    }
}
