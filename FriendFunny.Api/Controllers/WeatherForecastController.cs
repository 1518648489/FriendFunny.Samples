using FriendFunny.Application.Contracts.Interface.Student;
using FriendFunny.Application.Contracts.Interface.Student.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FriendFunny.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private readonly IStudentAppService _studentAppService;

        public StudentController(ILogger<StudentController> logger, IStudentAppService studentAppService)
        {
            _logger = logger;
            _studentAppService = studentAppService;
        }


        /// <summary>
        /// 创建学生
        /// </summary>
        /// <returns></returns>
        [HttpPost("student")]
        public async Task CreateAsync(CreateStudentDto dto) 
        {
            await _studentAppService.CreateAsync(dto);
        }
    }
}
