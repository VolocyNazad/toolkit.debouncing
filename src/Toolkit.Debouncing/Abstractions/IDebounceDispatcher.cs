using System;
using System.Windows.Threading;

namespace Toolkit.Debouncing.Abstractions
{
    internal interface IDebounceDispatcher
    {
        void Debounce(int interval, Action<object> action,
            object param = null,
            DispatcherPriority priority = DispatcherPriority.ApplicationIdle,
            Dispatcher disp = null);

        void Throttle(int interval, Action<object> action,
            object param = null,
            DispatcherPriority priority = DispatcherPriority.ApplicationIdle,
            Dispatcher disp = null);
    }
}
