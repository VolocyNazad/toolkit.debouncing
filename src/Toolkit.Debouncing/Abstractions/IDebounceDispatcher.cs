using System.Windows.Threading;

namespace Toolkit.Debouncing.Abstractions
{
    /// <summary>
    /// Schedules debounced and throttled callbacks on a WPF <see cref="Dispatcher"/>.
    /// </summary>
    public interface IDebounceDispatcher
    {
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
        void Debounce<TParameter>(int interval, Action<TParameter?> action,
            TParameter? parameter = default,
            DispatcherPriority priority = DispatcherPriority.Normal,
            Dispatcher? dispatcher = null);

        /// <summary>
        /// Runs <paramref name="action"/> after <paramref name="interval"/> milliseconds have passed
        /// without another debounced call, replacing any callback that is still pending.
        /// </summary>
        /// <param name="interval">The delay in milliseconds.</param>
        /// <param name="action">The callback to run.</param>
        /// <param name="priority">The dispatcher priority used to schedule the callback.</param>
        /// <param name="dispatcher">The dispatcher to schedule on; the application dispatcher by default.</param>
        void Debounce(int interval, Action action,
            DispatcherPriority priority = DispatcherPriority.Normal,
            Dispatcher? dispatcher = default);

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
        void Throttle<TParameter>(int interval, Action<TParameter?> action,
            TParameter? parameter = default,
            DispatcherPriority priority = DispatcherPriority.Normal,
            Dispatcher? dispatcher = null);

        /// <summary>
        /// Runs <paramref name="action"/> immediately when the interval since the previous throttled
        /// call has elapsed; otherwise schedules it for the rest of that interval.
        /// </summary>
        /// <param name="interval">The minimum time between two runs, in milliseconds.</param>
        /// <param name="action">The callback to run.</param>
        /// <param name="priority">The dispatcher priority used to schedule the callback.</param>
        /// <param name="dispatcher">The dispatcher to schedule on; the application dispatcher by default.</param>
        void Throttle(int interval, Action action,
            DispatcherPriority priority = DispatcherPriority.Normal,
            Dispatcher? dispatcher = default);
    }
}
