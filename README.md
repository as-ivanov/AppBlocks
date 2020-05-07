# AppBlocks

Collection of utils for faster development of cross-cutting concerns in applications by leveraging code generation.
At the moment the following modules are available:

- [x] [AppBlocks.Logging.Sdk](#AppBlocksLoggingSdk)
- [x] [AppBlocks.Monitoring.Sdk](#AppBlocksMonitoringSdk)
- [ ] Object mappings generator
- [ ] Conventional dependency registrator generator
- [ ] Result object generator

## Info

[![Build Status](https://alexey-ivanov.visualstudio.com/AppBlocks/_apis/build/status/as-ivanov.AppBlocks?branchName=master)](https://alexey-ivanov.visualstudio.com/AppBlocks/_build/latest?definitionId=1&branchName=master)

| Package                         | Version                                                                                                                       |
| ------------------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| AppBlocks.Logging.Sdk           | [![NuGet package](https://img.shields.io/nuget/v/AppBlocks.Logging.Sdk.svg)][appblocksloggingsdknugetpkg]                     |
| AppBlocks.Monitoring.Sdk        | [![NuGet package](https://img.shields.io/nuget/v/AppBlocks.Monitoring.Sdk.svg)][appblocksmonitoringsdknugetpkg]               |
| AppBlocks.Monitoring.AppMetrics | [![NuGet package](https://img.shields.io/nuget/v/AppBlocks.Monitoring.AppMetrics.svg)][appblocksmonitoringappmetricsnugetpkg] |

[appblocksloggingsdknugetpkg]: https://nuget.org/packages/AppBlocks.Logging.Sdk
[appblocksmonitoringsdknugetpkg]: https://nuget.org/packages/AppBlocks.Monitoring.Sdk
[appblocksmonitoringappmetricsnugetpkg]: https://nuget.org/packages/AppBlocks.Monitoring.AppMetrics

# AppBlocks.Logging.Sdk

Meta package containing utility for generating high-performance logger types using LoggerMessage pattern: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage.

## Installation

Add on top of your project file the following:

```xml
<Project>
  <Sdk Name="AppBlocks.Logging.Sdk" Version="{Put the last version here}" />
  ...  
</Project>
```

## Usage

Add interface which logger is supposed to implement. Interface should be marked with `GenerateLogger` attribute which optionally accepts a list of extra interfaces that generated logger must also inherit (possible use case is when an extra meta interface is needed).

Methods may optionally contain `LogOptions` attribute to provide extra information: LogLevel (Information by default) and message (Logger method name transformed by [Humanizer](https://github.com/Humanizr/Humanizer) by default).

```cs
using System;
using AppBlocks.Logging.CodeGeneration.Attributes;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.Sample
{
    [GenerateLogger(InheritedInterfaceTypes = new []{ "AppBlocks.Logging.Sample.ISingletonDependency", "AppBlocks.Logging.Sample.ILoggerImplementation"} )]
    public interface IBackgroundTaskManagerLogger
    {
        void ExecutionStarted(string taskName);
        
        void ExecutionFinished(string taskName);
        
        [LogOptions(Level = LogLevel.Warning, Message = "Error occurred")] 
        void ExecutionFailed(string taskName, Exception error);
    }
}
```

On pre-build the following implementation will be generated (some full type names are omitted for clarity):

```cs
using System;
using AppBlocks.Logging.CodeGeneration.Attributes;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Logging.Sample
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute(@"LoggerClassGenerator", @"1.0.0")]
    public sealed partial class BackgroundTaskManagerLogger : AppBlocks.Logging.Sample.IBackgroundTaskManagerLogger, AppBlocks.Logging.Sample.ISingletonDependency, AppBlocks.Logging.Sample.ILoggerImplementation
    {
        private static readonly System.Action<Microsoft.Extensions.Logging.ILogger, object, System.Exception> _executionStartedDelegate = Microsoft.Extensions.Logging.LoggerMessage.Define<object>(Microsoft.Extensions.Logging.LogLevel.Information, new Microsoft.Extensions.Logging.EventId(1, nameof(AppBlocks.Logging.Sample.IBackgroundTaskManagerLogger.ExecutionStarted)), @"Execution started (TaskName: '{TaskName}')");
        
        private static readonly System.Action<Microsoft.Extensions.Logging.ILogger, object, System.Exception> _executionFinishedDelegate = Microsoft.Extensions.Logging.LoggerMessage.Define<object>(Microsoft.Extensions.Logging.LogLevel.Information, new Microsoft.Extensions.Logging.EventId(2, nameof(AppBlocks.Logging.Sample.IBackgroundTaskManagerLogger.ExecutionFinished)), @"Execution finished (TaskName: '{TaskName}')");
        
        private static readonly System.Action<Microsoft.Extensions.Logging.ILogger, object, System.Exception> _executionFailedDelegate = Microsoft.Extensions.Logging.LoggerMessage.Define<object>(Microsoft.Extensions.Logging.LogLevel.Warning, new Microsoft.Extensions.Logging.EventId(3, nameof(AppBlocks.Logging.Sample.IBackgroundTaskManagerLogger.ExecutionFailed)), @"Task execution failed (TaskName: '{TaskName}')");
        
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        
        public BackgroundTaskManagerLogger(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            _logger = Microsoft.Extensions.Logging.LoggerFactoryExtensions.CreateLogger(loggerFactory, GetType());
        }
        
        void AppBlocks.Logging.Sample.IBackgroundTaskManagerLogger.ExecutionStarted(string taskName)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
            {
                _executionStartedDelegate(_logger, taskName, null);
            }
        }
        
        void AppBlocks.Logging.Sample.IBackgroundTaskManagerLogger.ExecutionFinished(string taskName)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
            {
                _executionFinishedDelegate(_logger, taskName, null);
            }
        }
        
        void AppBlocks.Logging.Sample.IBackgroundTaskManagerLogger.ExecutionFailed(string taskName, Exception error)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning))
            {
                _executionFailedDelegate(_logger, taskName, error);
            }
        }
    }
}
```

For details see full sample: [AppBlocks.Logging.Sample](https://github.com/as-ivanov/AppBlocks/tree/master/src/AppBlocks.Logging.Sample).

# AppBlocks.Monitoring.Sdk

Meta package containing utility for generating types which helps collecting metrics in application.

## Installation

Add on top of your project file the following:

```xml
<Project>
  <Sdk Name="AppBlocks.Monitoring.Sdk" Version="{Put the last version here}" />
  ...  
</Project>
```

## Usage

Create an interface which metrics collector is supposed implement. Interface must be marked with `GenerateMetricsCollector` attribute which accepts contextName used as a metrics prefix and policy configuration and optionally a list of extra interfaces generated metrics collector must also inherit (possible use case is when an extra meta interface is needed).

Methods may optionally contain `MetricOptions` attribute to provide extra information: metricName (name of the method by default) and measurementUnitName (null by default).

```cs
using AppBlocks.Monitoring.Abstractions;
using AppBlocks.Monitoring.CodeGeneration.Attributes;

namespace AppBlocks.Monitoring.Sample
{
    [GenerateMetricsCollector(ContextName = "BackgroundTask", InheritedInterfaceTypes = new []{ "AppBlocks.Monitoring.Sample.ISingletonDependency",
        "AppBlocks.Monitoring.Sample.IMetricsCollectorImplementation" })]
    public interface IBackgroundTaskMetricsCollector
    {
        [MetricOptions(MetricName = "execution_count", MeasurementUnitName = "item")]
        IMeter ExecutionTotal(string taskName);
        
        [MetricOptions(MetricName = "execution_active", MeasurementUnitName = "item")]
        ICounter ExecutionActive(string taskName);
        
        [MetricOptions(MetricName = "execution_time", MeasurementUnitName = "ms")]
        ITimer ExecutionTime(string taskName);
        
        [MetricOptions(MetricName = "execution_error", MeasurementUnitName = "item")]
        IMeter ExecutionError(string key, string error);
    }
}
```

On pre-build the following implementation will be generated (some full type names are omitted for clarity):

```cs
using AppBlocks.Monitoring.Abstractions;
using AppBlocks.Monitoring.CodeGeneration.Attributes;

namespace AppBlocks.Monitoring.Sample
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute(@"MetricsCollectorClassGenerator", @"1.0.0")]
    public partial class BackgroundTaskMetricsCollector : IBackgroundTaskMetricsCollector, AppBlocks.Monitoring.Sample.ISingletonDependency, AppBlocks.Monitoring.Sample.IMetricsCollectorImplementation
    {
        private const string _contextName = "BackgroundTask";

        private static readonly string[] _executionError = { @"key", @"error" };
        
        protected readonly IMetricsProvider MetricsProvider;

        protected readonly IMetricsPolicy MetricsPolicy;
        
        public BackgroundTaskMetricsCollector(IMetricsProvider metricsProvider, IMetricsPolicy metricsPolicy = null)
        {
            MetricsProvider = metricsProvider;
            MetricsPolicy = metricsPolicy;
        }
        
        public IMeter ExecutionTotal(string taskName)
        {
            const string metricName = "execution_count";
            const string metricUnit = "item";
            if (MetricsPolicy != null && !MetricsPolicy.IsEnabled(_contextName, metricName))
            {
                return NullMeter.Instance;
            }
        
            var tags = new Tags("taskName", taskName);
            return MetricsProvider.CreateMeter(_contextName, metricName, metricUnit, tags);
        }
        
        public ICounter ExecutionActive(string taskName)
        {
            const string metricName = "execution_active";
            const string metricUnit = "item";
            if (MetricsPolicy != null && !MetricsPolicy.IsEnabled(_contextName, metricName))
            {
                return NullCounter.Instance;
            }
        
            var tags = new Tags("taskName", taskName);
            return MetricsProvider.CreateCounter(_contextName, metricName, metricUnit, tags);
        }
        
        public ITimer ExecutionTime(string taskName)
        {
            const string metricName = "execution_time";
            const string metricUnit = "ms";
            if (MetricsPolicy != null && !MetricsPolicy.IsEnabled(_contextName, metricName))
            {
                return NullTimer.Instance;
            }
        
            var tags = new Tags("taskName", taskName);
            return MetricsProvider.CreateTimer(_contextName, metricName, metricUnit, tags);
        }
        
        public IMeter ExecutionError(string key, string error)
        {
            const string metricName = "execution_error";
            const string metricUnit = "item";
            if (MetricsPolicy != null && !MetricsPolicy.IsEnabled(_contextName, metricName))
            {
                return NullMeter.Instance;
            }
        
            var values = new string[] { key, error };
            var tags = new Tags(_executionError, values);
            return MetricsProvider.CreateMeter(_contextName, metricName, metricUnit, tags);
        }
    }
}
```

It's up to user how to implement `IMetricsProvider` and `IMetricsPolicy` (the last is optional and needed only if you require to turn on/off collection of some metrics dynamically). For implementation which uses [App Metrics library](https://github.com/AppMetrics/AppMetrics) see [AppBlocks.Monitoring.AppMetrics](https://www.nuget.org/packages/AppBlocks.Monitoring.AppMetrics/) NuGet package.

Generated metrics collector can be used as follows:

```cs
    public class MyBackgroundTaskManagerWithMetrics : IHostedService, IDisposable
    {
        private readonly IBackgroundTaskMetricsCollector _metricsCollector;
        private Timer _timer;
        private readonly List<string> _tasks = new List<string>() { "task1", "task2", "task3"};
        
        public MyBackgroundTaskManagerWithMetrics(IBackgroundTaskMetricsCollector metricsCollector)
        {
            _metricsCollector = metricsCollector;
        }
        
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(RunAll, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }
        
        private void RunAll(object state)
        {
            foreach (var task in _tasks)
            {
                Run(task);
            }
        }
        
        private void Run(string name)
        {
            _metricsCollector.ExecutionTotal(name).Mark();
            _metricsCollector.ExecutionActive(name).Increment();
            using (_metricsCollector.ExecutionTime(name).Time())
            {
                try
                {
                    DoWork(name);
                }
                catch (Exception e)
                {
                    _metricsCollector.ExecutionError(name, e.GetType().Name).Mark();
                    throw;
                }
                finally
                {
                    _metricsCollector.ExecutionActive(name).Decrement();
                }
            }
        }
        
        private void DoWork(string name)
        {
            //...
        }
        
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
```

For details see full sample: [AppBlocks.Monitoring.Sample](https://github.com/as-ivanov/AppBlocks/tree/master/src/AppBlocks.Monitoring.Sample).

# Credits

The generator is based on @AArnott work [CodeGeneration.Roslyn](https://github.com/AArnott/CodeGeneration.Roslyn).

Created with a great help of @KirillOsenkov project [RoslynQuoter](https://github.com/KirillOsenkov/RoslynQuoter).

# License

MIT
