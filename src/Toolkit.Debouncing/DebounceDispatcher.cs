#if NET9_0

using System.Windows;
using System.Windows.Threading;
using Toolkit.Debouncing.Abstractions;

namespace Toolkit.Debouncing;

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

    /// <summary>
    /// Runs <paramref name="action"/> after <paramref name="interval"/> milliseconds have passed
    /// without another debounced call, replacing any callback that is still pending.
    /// </summary>
    /// <param name="interval">The delay in milliseconds.</param>
    /// <param name="action">The callback to run.</param>
    /// <param name="priority">The dispatcher priority used to schedule the callback.</param>
    /// <param name="dispatcher">The dispatcher to schedule on; the application dispatcher by default.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public void Debounce(int interval, Action action,
        DispatcherPriority priority = DispatcherPriority.Normal,
        Dispatcher? dispatcher = default)
    {
        ArgumentNullException.ThrowIfNull(action);
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

    /// <summary>
    /// Runs <paramref name="action"/> after <paramref name="interval"/> milliseconds have passed
    /// without another debounced call, replacing any callback that is still pending.
    /// </summary>
    /// <typeparam name="TParameter">The type of the value forwarded to the callback.</typeparam>
    /// <param name="interval">The delay in milliseconds.</param>
    /// <param name="action">The callback to run.</param>
    /// <param name="parameter">The value forwarded to <paramref name="action"/>.</param>
    /// <param name="priority">The dispatcher priority used to schedule the callback.</param>
    /// <param name="dispatcher">The dispatcher to schedule on; the application dispatcher by default.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public void Debounce<TParameter>(int interval, Action<TParameter?> action,
        TParameter? parameter = default,
        DispatcherPriority priority = DispatcherPriority.Normal,
        Dispatcher? dispatcher = default)
    {
        ArgumentNullException.ThrowIfNull(action);
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

    /// <summary>
    /// Runs <paramref name="action"/> immediately when the interval since the previous throttled
    /// call has elapsed; otherwise schedules it for the rest of that interval.
    /// </summary>
    /// <param name="interval">The minimum time between two runs, in milliseconds.</param>
    /// <param name="action">The callback to run.</param>
    /// <param name="priority">The dispatcher priority used to schedule the callback.</param>
    /// <param name="dispatcher">The dispatcher to schedule on; the application dispatcher by default.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public void Throttle(int interval, Action action,
        DispatcherPriority priority = DispatcherPriority.Normal,
        Dispatcher? dispatcher = default)
    {
        ArgumentNullException.ThrowIfNull(action);
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

    /// <summary>
    /// Runs <paramref name="action"/> immediately when the interval since the previous throttled
    /// call has elapsed; otherwise schedules it for the rest of that interval.
    /// </summary>
    /// <typeparam name="TParameter">The type of the value forwarded to the callback.</typeparam>
    /// <param name="interval">The minimum time between two runs, in milliseconds.</param>
    /// <param name="action">The callback to run.</param>
    /// <param name="parameter">The value forwarded to <paramref name="action"/>.</param>
    /// <param name="priority">The dispatcher priority used to schedule the callback.</param>
    /// <param name="dispatcher">The dispatcher to schedule on; the application dispatcher by default.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public void Throttle<TParameter>(int interval, Action<TParameter?> action,
        TParameter? parameter = default,
        DispatcherPriority priority = DispatcherPriority.Normal,
        Dispatcher? dispatcher = default)
    {
        ArgumentNullException.ThrowIfNull(action);
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

    /// <summary>
    /// Stops a pending callback and releases the timer.
    /// </summary>
    public void Dispose()
    {
        using (_lock.EnterScope())
        {
            timer?.Stop();
            timer = null;
        }
    }
}

#else
using System.Windows;
using System.Windows.Threading;
using Toolkit.Debouncing.Abstractions;

namespace Toolkit.Debouncing;

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
    private readonly object _lockObject = new();
    private DispatcherTimer? timer;
    private DateTime lastExecution = DateTime.UtcNow.AddYears(-1);

    /// <summary>
    /// Runs <paramref name="action"/> after <paramref name="interval"/> milliseconds have passed
    /// without another debounced call, replacing any callback that is still pending.
    /// </summary>
    /// <param name="interval">The delay in milliseconds.</param>
    /// <param name="action">The callback to run.</param>
    /// <param name="priority">The dispatcher priority used to schedule the callback.</param>
    /// <param name="dispatcher">The dispatcher to schedule on; the application dispatcher by default.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public void Debounce(int interval, Action action,
        DispatcherPriority priority = DispatcherPriority.Normal,
        Dispatcher? dispatcher = default)
    {
        if (action is null)
            throw new ArgumentNullException(nameof(action));

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
                action.Invoke();
            }, dispatcher);

            timer.Start();
        }
    }

    /// <summary>
    /// Runs <paramref name="action"/> after <paramref name="interval"/> milliseconds have passed
    /// without another debounced call, replacing any callback that is still pending.
    /// </summary>
    /// <typeparam name="TParameter">The type of the value forwarded to the callback.</typeparam>
    /// <param name="interval">The delay in milliseconds.</param>
    /// <param name="action">The callback to run.</param>
    /// <param name="parameter">The value forwarded to <paramref name="action"/>.</param>
    /// <param name="priority">The dispatcher priority used to schedule the callback.</param>
    /// <param name="dispatcher">The dispatcher to schedule on; the application dispatcher by default.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public void Debounce<TParameter>(int interval, Action<TParameter?> action,
        TParameter? parameter = default,
        DispatcherPriority priority = DispatcherPriority.Normal,
        Dispatcher? dispatcher = default)
    {
        if (action is null)
            throw new ArgumentNullException(nameof(action));

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

    /// <summary>
    /// Runs <paramref name="action"/> immediately when the interval since the previous throttled
    /// call has elapsed; otherwise schedules it for the rest of that interval.
    /// </summary>
    /// <param name="interval">The minimum time between two runs, in milliseconds.</param>
    /// <param name="action">The callback to run.</param>
    /// <param name="priority">The dispatcher priority used to schedule the callback.</param>
    /// <param name="dispatcher">The dispatcher to schedule on; the application dispatcher by default.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public void Throttle(int interval, Action action,
        DispatcherPriority priority = DispatcherPriority.Normal,
        Dispatcher? dispatcher = default)
    {
        if (action is null)
            throw new ArgumentNullException(nameof(action));

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
                action.Invoke();
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
                action.Invoke();
            }, dispatcher);

            timer.Start();
        }
    }

    /// <summary>
    /// Runs <paramref name="action"/> immediately when the interval since the previous throttled
    /// call has elapsed; otherwise schedules it for the rest of that interval.
    /// </summary>
    /// <typeparam name="TParameter">The type of the value forwarded to the callback.</typeparam>
    /// <param name="interval">The minimum time between two runs, in milliseconds.</param>
    /// <param name="action">The callback to run.</param>
    /// <param name="parameter">The value forwarded to <paramref name="action"/>.</param>
    /// <param name="priority">The dispatcher priority used to schedule the callback.</param>
    /// <param name="dispatcher">The dispatcher to schedule on; the application dispatcher by default.</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    public void Throttle<TParameter>(int interval, Action<TParameter?> action,
        TParameter? parameter = default,
        DispatcherPriority priority = DispatcherPriority.Normal,
        Dispatcher? dispatcher = default)
    {
        if (action is null)
            throw new ArgumentNullException(nameof(action));

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

    /// <summary>
    /// Stops a pending callback and releases the timer.
    /// </summary>
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
