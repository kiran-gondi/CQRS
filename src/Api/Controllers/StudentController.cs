using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Logic.AppServices;
using Logic.Students;
using Logic.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
  [Route("api/students")]
    public sealed class StudentController : BaseController
    {
        private readonly Messages _messages;

        public StudentController(Messages messages)
        {
            _messages = messages;
        }

        [HttpGet]
        public IActionResult GetList(string enrolled, int? number) //Query
        {
           List<StudentDto> list = _messages.Dispatch(new GetListQuery(enrolled, number));
            return Ok(list);
        }

        [HttpPost]
        public IActionResult Register([FromBody] NewStudentDto dto) //Command
        {
          var registerCommand = new RegisterCommand(dto.Name, dto.Email, dto.Course1, dto.Course1Grade,
            dto.Course2, dto.Course2Grade);

          Result result = _messages.Dispatch(registerCommand);
          return FromResult(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Unregister(long id) //Command
        {
          var unregisterCommand = new UnregisterCommand(id);
          Result result = _messages.Dispatch(unregisterCommand);
          return result.IsSuccess ? Ok() : Error(result.Error);
        }

        [HttpPost("{id}/enrollments")]
        public IActionResult Enroll(long id, [FromBody] StudentEnrollmentDto dto) //Command
        {
          var enrollCommand = new EnrollCommand(id, dto.Course, dto.Grade);
          Result result = _messages.Dispatch(enrollCommand);
          return FromResult(result);
        }


        [HttpPut("{id}/enrollments/{enrollmentNumber}")]    
        public IActionResult Transfer(long id, int enrollmentNumber, [FromBody] StudentTransferDto dto) //Command
        {
          var transferCommand = new TransferCommand(id, enrollmentNumber, dto.Course, dto.Grade);
          Result result = _messages.Dispatch(transferCommand);
          return FromResult(result);
        }   

        [HttpPost("{id}/enrollments/{enrollmentNumber}/deletion")]    
        public IActionResult Disenroll(long id, int enrollmentNumber, [FromBody] StudentDisenrollmentDto dto) //Command
        {
          Result result = _messages.Dispatch(new DisenrollCommand(id, enrollmentNumber, dto.Comment));
          return FromResult(result);
        }

        [HttpPut("{id}")]
        public IActionResult EditPersonalInfo(long id, [FromBody] StudentPersonalInfoDto dto) //Command
        {
            var command = new EditPersonalInfoCommand(id, dto.Name, dto.Email);
            Result result = _messages.Dispatch(command);
            return FromResult(result);
        }
      }
}
