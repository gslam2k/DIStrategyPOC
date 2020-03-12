# Implement the Strategy Pattern with Dependency Injection in .Net Core 3.1

Implementing the strategy pattern with dependency injection framework provided in ASP.Net Core, or through the NuGet package Microsoft.Extensions.DependencyInjection presents some challenges. This article presents one approach for dealing with these opportunities.

The definition for a strategy pattern, as documented [here](https://www.dofactory.com/net/strategy-design-pattern), is

_Define a family of algorithms, encapsulate each one, and make them interchangeable. Strategy lets the algorithm vary independently from clients that use it._

For the example for this article, the algorithms define how to create a chat response to an intent identified by Luis. In this case we have 3 intents:

- Available
- Average
- Delay.

I started to explore implementing the strategy pattern to refactor a large switch statement in a 400+ line class. The redesign POC included creating the following:

- IIntentStrategy

An interface that requires the following to process the intent:
``` C#
public interface IIntentStrategy
{
     Task ProcessIntentAsync(UserIntent userIntent);
}
```

- IStrategyHelper

An interface to ensure that the individual strategies are injected with their dependencies:
``` c#
public abstract class BaseIntentStrategy : IIntentStrategy
{
    public BaseIntentStrategy(IStrategyHelper strategyHelper)
    {
        StrategyHelper = strategyHelper;
    }
    protected IStrategyHelper StrategyHelper { get; }
    public abstract Task ProcessIntentAsync(UserIntent userIntent);
}
```

- BaseIntentStrategy

An abstract class to support scenarios where the differences in the process maybe configurations. It had this initial implementation which includes a protected read only property for the injected Strategy Helper and an abstract method to be implemented by each child:
``` C#
public abstract class BaseIntentStrategy : IIntentStrategy
{
    public BaseIntentStrategy(IStrategyHelper strategyHelper)
    {
        StrategyHelper = strategyHelper;
    }
    protected IStrategyHelper StrategyHelper { get; }
    public abstract Task ProcessIntentAsync(UserIntent userIntent);
}
```

- Individual Strategy implementations

Each implementation extends the base class, instantiating a method for ProcessIntentAsync. For this POC, the method is very simple.
``` C#
public class IntentStrategyAvailable : BaseIntentStrategy
{
    public IntentStrategyAvailable(IStrategyHelper StrategyHelper) 
: base(StrategyHelper) { }

    public override async Task ProcessIntentAsync(UserIntent userIntent) 
        => await StrategyHelper.DisplayAsync(
$"{userIntent.Intent} in {nameof(IntentStrategyAvailable)}");
}
```


The initial POC used Autofac with Keyed registrations and modules that made the implementation a breeze. Migrating the implementation to Microsoft.Extensions.DependencyInjection presented three challenges:

1. Loading all implementations of a given interface
2. Supporting Keyed registrations
3. Supporting Modules

The framework does not support Keyed registration, but it does allow registering a collection of implementations for an interface. This collection can then be injected into a constructor:
``` C# 
public IntentStrategyFactory(IEnumerable<IIntentStrategy> intentStrategies)
```

Microsoft.Extensions.DependencyInjection does not provide a construct that registers all the implementations for a given interface. I create this extension class that exposes a fluent method that registers these implementations:
``` C#
private static readonly Lazy<IList<Type>> ExecutingTypes 
= new Lazy<IList<Type>>(
    		() => Assembly.GetExecutingAssembly().ExportedTypes
        		.Where(t => !t.IsAbstract && !t.IsInterface)
        		.ToList());

public static IServiceCollection AddTransientImplementingTypes<IType>(
    this IServiceCollection services)
{
    var interfaceType = typeof(IType);

    foreach (var type in ExecutingTypes.Value.
            Where(interfaceType.IsAssignableFrom))
    {
        services.AddTransient(interfaceType, type);
    }

    return services;
}
```


There are several examples that handle a &quot;keyed&quot; registration. They all introduce a Key property in the interface:


The key is used to select the appropriate strategy to use. Most examples use a LINQ Single/First query for each request. The POC populates a dictionary and uses a TryGetValue for the strategy lookup.
``` C#
private readonly IDictionary<string, IIntentStrategy> _intentStrategies;

public IntentStrategyFactory(IEnumerable<IIntentStrategy> intentStrategies)
{
    _intentStrategies = intentStrategies.ToDictionary(x => x.Key);
}

public IIntentStrategy GetStrategy(UserIntent userIntent)
{
    if (_intentStrategies.TryGetValue(
            userIntent.Intent.ToString(),
            out var Strategy))
    {
        return Strategy;
    }

    throw new ArgumentException(
        $"The Intent {userIntent.Intent} is not handled",
        nameof(userIntent));
}

```

Where to set the registrations brings us to the third opportunity, separating out the various registrations into modules.

Rather than having a large ServiceCollection build statement, using a fluent custom ServiceCollection extension separate the specific registration details to a focused module. For example:
``` C#
public static class IntentModule
{
    public static IServiceCollection AddIntentServices(
        this IServiceCollection services)
    {
        return services
            .AddTransientImplementingTypes<IIntentStrategy>()
            .AddTransient<IStrategyHelper, StrategyHelper>()
            .AddTransient<IIntentStrategyFactory, IntentStrategyFactory>();
    }
}
```

This simplifies the build statement to:
``` C#
var serviceProvider = new ServiceCollection()
    .AddIntentServices()
    .BuildServiceProvider();
```

So now we have an implementation that supports the strategy pattern without compromising the dependency injection framework, hiding the details from the code using the strategies.

The following console code generates these results:
``` C#
public static async Task Main()
{
    var serviceProvider = new ServiceCollection()
        .AddIntentServices()
        .BuildServiceProvider();

    var factory = serviceProvider.GetService<IIntentProcessorFactory>();

    foreach (var intent in new []
        {
            new UserIntent { Intent = Intent.Delay },
            new UserIntent { Intent = Intent.Available },
            new UserIntent { Intent = Intent.Average },
            new UserIntent { Intent = Intent.Other },
        })
    {
        try
        {
            await factory.GetProcessor(intent).ProcessIntentAsync(intent);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
```

Run Results:
- Display Delay in IntentProcessorDelay
- Display Available in IntentProcessorAvailable
- Display Average in IntentProcessorAverage
- The Intent Other is not handled (Parameter 'userIntent')

