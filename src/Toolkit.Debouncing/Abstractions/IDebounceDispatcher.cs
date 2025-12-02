using System.Windows.Threading;

namespace Toolkit.Debouncing.Abstractions
{
    public interface IDebounceDispatcher
    {
        void Debounce<TParameter>(int interval, Action<TParameter?> action,
            TParameter? parameter = default,
            DispatcherPriority priority = DispatcherPriority.ApplicationIdle,
            Dispatcher? dispatcher = null);
        void Debounce(int interval, Action action,
            DispatcherPriority priority = DispatcherPriority.Normal,
            Dispatcher? dispatcher = default);

        void Throttle<TParameter>(int interval, Action<TParameter?> action,
            TParameter? parameter = default,
            DispatcherPriority priority = DispatcherPriority.ApplicationIdle,
            Dispatcher? dispatcher = null);
        void Throttle(int interval, Action action,
            DispatcherPriority priority = DispatcherPriority.Normal,
            Dispatcher? dispatcher = default);
    }
}
