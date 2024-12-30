using CQRS.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Post.Cmd.Infrastructure.Dispatchers;
using CQRS.Core.Commands;

public class CommandDispatcher:ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CommandDispatcher> _logger;

    // Dictionary to store the Command Type and its Handler.
    private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();

    /**
     * Constructor CommandDispatcher
     *
     * This constructor is used to inject the Service Provider and Logger.
     * The Service Provider is used to get the Handler for the Command.
     * The Logger is used to log the information.
     */
    public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void RegisterHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : BaseCommand
    {
        if (_handlers.ContainsKey(typeof(TCommand)))
        {
            throw new InvalidOperationException($"Handler for {typeof(TCommand).Name} is already registered.");
        }

        // Add the Command Type and its Handler to the Dictionary.
        // Key = Concrete Command Type
        // Value = Handler for the Command (Casting the BaseCommand (command) to the Concrete Command Type (TCommand)
        _handlers[typeof(TCommand)] = command => handler((TCommand)command);
    }

    /**
     * SendAsynch
     *
     * This method is used to send the Command to the Handler.
     * The method checks if the Command is null or not.
     * The method checks if the Handler for the Command is registered or not.
     * The method gets the Handler for  the Command and executes the Handler.
     */
    public async Task SendAsynch(BaseCommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var commandType = command.GetType();
        // Check if the Handler for the Command is registered or not using the Command Type.
        if (!_handlers.TryGetValue(commandType, out var handler))
        {
            throw new InvalidOperationException($"Handler for {commandType.Name} not found.");
        }

        await handler(command);
    }

}