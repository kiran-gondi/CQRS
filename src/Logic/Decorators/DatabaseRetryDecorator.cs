using CSharpFunctionalExtensions;
using Logic.AppServices;
using Logic.Students;
using Logic.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Decorators
{
  //Decorator Definition
  //Decorator is a class or a method that modifies the behavior of an exisiting class or method without changing its public interface.
  public sealed class DatabaseRetryDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
  {
    private readonly ICommandHandler<TCommand> _handler;
    private readonly Config _config;

    public DatabaseRetryDecorator(ICommandHandler<TCommand> handler, Config config)
    {
      _handler = handler;
      _config = config;
    }

    public Result Handle(TCommand command)
    {
      for (int i = 0; ; i++)
      {
        try
        {
          Result result = _handler.Handle(command);
          return result;
        }
        catch (Exception ex)
        {
          if (i >= _config.NumberOfDatabaseRetries || !IsDatabaseException(ex))
            throw;
        }
      }
    }

    private bool IsDatabaseException(Exception exception)
    {
      string message = exception.InnerException?.Message;

      if (message is null)
        return false;

      return message.Contains("The connection is broken and recovery is not possible")
        || message.Contains("error occured while establishing a connection");
    }

  }
}
