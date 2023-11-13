using CSharpFunctionalExtensions;
using Logic.AppServices;
using Logic.Students;
using System;

namespace Logic.Utils
{
  public sealed class Messages
  {
    private readonly IServiceProvider _provider;

    public Messages(IServiceProvider serviceProvider)
    {
      _provider = serviceProvider;
    }

    public Result Dispatch(ICommand command)
    {
      Type type = typeof(ICommandHandler<>);
      Type[] typeArgs = { command.GetType() };
      Type handlerType = type.MakeGenericType(typeArgs);

      dynamic handler = _provider.GetService(handlerType);
      Result result = handler.Handle((dynamic)command);

      return result;
    }

    public T Dispatch<T>(IQuery<T> query)
    {
      Type type = typeof(IQueryHandler<,>);
      Type[] typeArgs = { query.GetType(), typeof(T) };
      Type handlerType = type.MakeGenericType(typeArgs);

      dynamic handler = _provider.GetService(handlerType);
      T result = handler.Handle((dynamic)query);

      return result;
    }
  }
}
