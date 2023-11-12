﻿using System;
using System.Collections.Generic;
using System.Linq;
using Api.Dtos;
using CSharpFunctionalExtensions;
using Logic.Students;
using Logic.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/students")]
    public sealed class StudentController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly StudentRepository _studentRepository;
        private readonly CourseRepository _courseRepository;

        public StudentController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _studentRepository = new StudentRepository(unitOfWork);
            _courseRepository = new CourseRepository(unitOfWork);
        }

        [HttpGet]
        public IActionResult GetList(string enrolled, int? number) //Query
        {
            IReadOnlyList<Student> students = _studentRepository.GetList(enrolled, number);
            List<StudentDto> dtos = students.Select(x => ConvertToDto(x)).ToList();
            return Ok(dtos);
        }

        private StudentDto ConvertToDto(Student student)
        {
            return new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Course1 = student.FirstEnrollment?.Course?.Name,
                Course1Grade = student.FirstEnrollment?.Grade.ToString(),
                Course1Credits = student.FirstEnrollment?.Course?.Credits,
                Course2 = student.SecondEnrollment?.Course?.Name,
                Course2Grade = student.SecondEnrollment?.Grade.ToString(),
                Course2Credits = student.SecondEnrollment?.Course?.Credits,
            };
        }

        [HttpPost]
        public IActionResult Register([FromBody] NewStudentDto dto) //Command
        {
            var student = new Student(dto.Name, dto.Email);

            if (dto.Course1 != null && dto.Course1Grade != null)
            {
                Course course = _courseRepository.GetByName(dto.Course1);
                student.Enroll(course, Enum.Parse<Grade>(dto.Course1Grade));
            }

            if (dto.Course2 != null && dto.Course2Grade != null)
            {
                Course course = _courseRepository.GetByName(dto.Course2);
                student.Enroll(course, Enum.Parse<Grade>(dto.Course2Grade));
            }

            _studentRepository.Save(student);
            _unitOfWork.Commit();

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Unregister(long id) //Command
        {
            Student student = _studentRepository.GetById(id);
            if (student == null)
                return Error($"No student found for Id {id}");

            _studentRepository.Delete(student);
            _unitOfWork.Commit();

            return Ok();
        }

        [HttpPost("{id}/enrollments")]
        public IActionResult Enroll(long id, [FromBody] StudentEnrollmentDto dto) //Command
    {
          Student student = _studentRepository.GetById(id);
          if (student == null)
            return Error($"No student found for Id { id }");

          Course course = _courseRepository.GetByName(dto.Course);
          if (course == null)
            return Error($"Course is incorrect: '{ dto.Course }'");

          bool success = Enum.TryParse(dto.Grade, out Grade grade);
          if (!success)
            return Error($"Grade is incorrect: '{ dto.Grade }'");

          student.Enroll(course, grade);
          _unitOfWork.Commit();
          return Ok();
        }


        [HttpPut("{id}/enrollments/{enrollmentNumber}")]    
        public IActionResult Transfer(long id, int enrollmentNumber, [FromBody] StudentTransferDto dto) //Command
    {
          Student student = _studentRepository.GetById(id);
          if (student == null)
            return Error($"No student found for Id { id }");

          Course course = _courseRepository.GetByName(dto.Course);
          if (course == null)
            return Error($"Course is incorrect: '{ dto.Course }'");

          bool success = Enum.TryParse(dto.Grade, out Grade grade);
          if (!success)
            return Error($"Grade is incorrect: '{ dto.Grade }'");

          Enrollment enrollment = student.GetEnrollment(enrollmentNumber);
          if (enrollment == null)
            return Error($"No enrollment found with number ' { enrollmentNumber } '");

          enrollment.Update(course, enrollment.Grade);
          _unitOfWork.Commit();

          return Ok();
        }

        [HttpPost("{id}/enrollments/{enrollmentNumber}/deletion")]    
        public IActionResult Disenroll(long id, int enrollmentNumber, [FromBody] StudentDisenrollmentDto dto) //Command
    {
          Student student = _studentRepository.GetById(id);
          if (student == null)
            return Error($"No student found for Id {id}");

          if (string.IsNullOrWhiteSpace(dto.Comment))
            return Error("Disenrollment comment is required");

          Enrollment enrollment = student.GetEnrollment(enrollmentNumber);
          if (enrollment == null)
            return Error($"No enrollment found with number ' { enrollmentNumber } '");

          student.RemoveEnrollment(enrollment, dto.Comment); 

          _unitOfWork.Commit();

          return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult EditPersonalInfo(long id, [FromBody] StudentPersonalInfoDto dto) //Command
        {
            var command = new EditPersonalInfoCommand()
            {
              Email = dto.Email,
              Name = dto.Name,
              Id = id
            };
            var handler = new EditPersonalInfoCommandHandler(_unitOfWork);
            Result result = handler.Handle(command);

            //Student student = _studentRepository.GetById(id);
            //if (student == null)
            //  return Error($"No student found for Id {id}");

            //student.Name = dto.Name;
            //student.Email = dto.Email;

            //_unitOfWork.Commit();

            return result.IsSuccess ? Ok() : Error(result.Error);
        }
    }
}
