using CQRS.Core.Commands;

namespace CQRS.Core.Infrastructure;

/**
 * Interface ICommandDispatcher
 *
 * This interface is used to dispatch commands to their respective handlers.
 * In the Mediator Pattern, this is the Mediator.
 */
public interface ICommandDispatcher
{
    // Register a handler for a command.
    // The Func Delegate<TCommand, Task> is the handler for the command.
    // The Input is Generic Command (BaseCommand) and the Output is an Asynchronous Task.
    // In other words, the handler is a function that takes a command and returns a Task based on the Command
    void RegisterHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : BaseCommand;

    // Dispatch the command to the handler.
    Task SendAsynch(BaseCommand command);

}