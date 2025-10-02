#if NET9_0

using System.Windows;
using System.Windows.Threading;
using Toolkit.Debouncing.Abstractions;

namespace Toolkit.Debouncing
{
    /// <summary>
    /// Provides Debounce() and Throttle() methods.
    /// Use these methods to ensure that events aren't handled too frequently.
    /// 
    /// Throttle() ensures that events are throttled by the interval specified.
    /// Only the last event in the interval sequence of events fires.
    /// 
    /// Debounce() fires an event only after the specified interval has passed
    /// in which no other pending event has fired. Only the last event in the
    /// sequence is fired.
    /// </summary>
    public sealed class DebounceDispatcher : IDebounceDispatcher, IDisposable
    {
        private readonly Lock _lock = new();
        private DispatcherTimer? timer;
        private DateTime lastExecution = DateTime.UtcNow.AddYears(-1);

        public void Debounce(int interval, Action action,
            DispatcherPriority priority = DispatcherPriority.Normal,
            Dispatcher? dispatcher = default)
        {
            using (_lock.EnterScope())
            {
                timer?.Stop();

                dispatcher ??= Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

                if (dispatcher.CheckAccess() && !dispatcher.HasShutdownStarted)
                    return;

                timer = new(TimeSpan.FromMilliseconds(interval), priority, (s, e) =>
                {
                    using (_lock.EnterScope())
                    {
                        timer?.Stop();
                    }
                    action.Invoke();
                }, dispatcher);

                timer.Start();
            }
        }

        public void Debounce<TParameter>(int interval, Action<TParameter?> action,
            TParameter? parameter = default,
            DispatcherPriority priority = DispatcherPriority.Normal,
            Dispatcher? dispatcher = default)
        {
            using (_lock.EnterScope())
            {
                timer?.Stop();

                dispatcher ??= Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

                if (dispatcher.CheckAccess() && !dispatcher.HasShutdownStarted)
                    return;

                timer = new(TimeSpan.FromMilliseconds(interval), priority, (s, e) =>
                {
                    using (_lock.EnterScope())
                    {
                        timer?.Stop();
                    }
                    action.Invoke(parameter);
                }, dispatcher);

                timer.Start();
            }
        }

        public void Throttle(int interval, Action action,
            DispatcherPriority priority = DispatcherPriority.Normal,
            Dispatcher? dispatcher = default)
        {
            using (_lock.EnterScope())
            {
                timer?.Stop();

                dispatcher ??= Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

                if (dispatcher.CheckAccess() && !dispatcher.HasShutdownStarted)
                    return;

                var currentTime = DateTime.UtcNow;
                var timeSinceLast = currentTime - lastExecution;

                if (timeSinceLast.TotalMilliseconds < interval)
                {
                    interval -= (int)timeSinceLast.TotalMilliseconds;
                }
                else
                {
                    action.Invoke();
                    lastExecution = currentTime;
                    return;
                }

                timer = new(TimeSpan.FromMilliseconds(interval), priority, (s, e) =>
                {
                    using (_lock.EnterScope())
                    {
                        timer?.Stop();
                        lastExecution = DateTime.UtcNow;
                    }
                    action.Invoke();
                }, dispatcher);

                timer.Start();
            }
        }

        public void Throttle<TParameter>(int interval, Action<TParameter?> action,
            TParameter? parameter = default,
            DispatcherPriority priority = DispatcherPriority.Normal,
            Dispatcher? dispatcher = default)
        {
            using (_lock.EnterScope())
            {
                timer?.Stop();

                dispatcher ??= Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

                if (dispatcher.CheckAccess() && !dispatcher.HasShutdownStarted)
                    return;

                var currentTime = DateTime.UtcNow;
                var timeSinceLast = currentTime - lastExecution;

                if (timeSinceLast.TotalMilliseconds < interval)
                {
                    interval -= (int)timeSinceLast.TotalMilliseconds;
                }
                else
                {
                    action.Invoke(parameter);
                    lastExecution = currentTime;
                    return;
                }

                timer = new(TimeSpan.FromMilliseconds(interval), priority, (s, e) =>
                {
                    using (_lock.EnterScope())
                    {
                        timer?.Stop();
                        lastExecution = DateTime.UtcNow;
                    }
                    action.Invoke(parameter);
                }, dispatcher);

                timer.Start();
            }
        }

        public void Dispose()
        {
            using (_lock.EnterScope())
            {
                timer?.Stop();
                timer = null;
            }
        }
    }
}

#else
using System.Windows;
using System.Windows.Threading;
using Toolkit.Debouncing.Abstractions;

public sealed class DebounceDispatcher : IDebounceDispatcher, IDisposable
{
    private readonly object _lockObject = new object();
    private DispatcherTimer? timer;
    private DateTime lastExecution = DateTime.UtcNow.AddYears(-1);

    public void Debounce<TParameter>(int interval, Action<TParameter?> action,
        TParameter? parameter = default,
        DispatcherPriority priority = DispatcherPriority.Normal,
        Dispatcher? dispatcher = default)
    {
        lock (_lockObject)
        {
            timer?.Stop();

            dispatcher ??= Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

            if (dispatcher.HasShutdownStarted || dispatcher.HasShutdownFinished)
                return;

            timer = new(TimeSpan.FromMilliseconds(interval), priority, (s, e) =>
            {
                lock (_lockObject)
                {
                    timer?.Stop();
                }
                action.Invoke(parameter);
            }, dispatcher);

            timer.Start();
        }
    }

    public void Throttle<TParameter>(int interval, Action<TParameter?> action,
        TParameter? parameter = default,
        DispatcherPriority priority = DispatcherPriority.Normal,
        Dispatcher? dispatcher = default)
    {
        lock (_lockObject)
        {
            timer?.Stop();

            dispatcher ??= Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

            if (dispatcher.HasShutdownStarted || dispatcher.HasShutdownFinished)
                return;

            var currentTime = DateTime.UtcNow;
            var timeSinceLast = currentTime - lastExecution;

            if (timeSinceLast.TotalMilliseconds < interval)
            {
                interval -= (int)timeSinceLast.TotalMilliseconds;
            }
            else
            {
                action.Invoke(parameter);
                lastExecution = currentTime;
                return;
            }

            timer = new(TimeSpan.FromMilliseconds(interval), priority, (s, e) =>
            {
                lock (_lockObject)
                {
                    timer?.Stop();
                    lastExecution = DateTime.UtcNow;
                }
                action.Invoke(parameter);
            }, dispatcher);

            timer.Start();
        }
    }

    public void Dispose()
    {
        lock (_lockObject)
        {
            timer?.Stop();
            timer = null;
        }
    }
}
#endif