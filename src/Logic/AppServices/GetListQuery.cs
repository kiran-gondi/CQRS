using Logic.Students;
using Logic.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Logic.AppServices
{
  public interface IQuery<TResult>
  {

  }
  public interface IQueryHandler<TQuery, TResult>
  where TQuery : IQuery<TResult>
  {
    TResult Handle(TQuery query);
  }

  public sealed class GetListQuery : IQuery<List<StudentDto>>
  {
    public string EnrolledIn { get; }
    public int? NumberOfCourses { get; }

    public GetListQuery(string enrolledIn, int? numberOfCourses)
    {
      EnrolledIn = enrolledIn;
      NumberOfCourses = numberOfCourses;
    }

    internal sealed class GetListQueryHandler : IQueryHandler<GetListQuery, List<StudentDto>>
    {
      private readonly SessionFactory _sessionFactory;

      public GetListQueryHandler(SessionFactory sessionFactory)
      {
        _sessionFactory = sessionFactory;
      }

      public List<StudentDto> Handle(GetListQuery query)
      {
        var unitOfWork = new UnitOfWork(_sessionFactory);

        return new StudentRepository(unitOfWork)
                              .GetList(query.EnrolledIn, query.NumberOfCourses)
                              .Select(x => ConvertToDto(x)).ToList();
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
    }
  }
}
