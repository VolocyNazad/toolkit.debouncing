using System;
using System.Windows.Threading;
using Xunit;

namespace Toolkit.Debouncing.Tests;

public sealed class DebounceDispatcherTests
{
    [Fact]
    public void Throttle_invokes_first_action_immediately()
    {
        using var dispatcher = new DebounceDispatcher();
        var calls = 0;

        dispatcher.Throttle(100, () => calls++);

        Assert.Equal(1, calls);
    }

    [Fact]
    public void Throttle_passes_parameter_to_action()
    {
        using var dispatcher = new DebounceDispatcher();
        string? value = null;

        dispatcher.Throttle(100, parameter => value = parameter, "expected");

        Assert.Equal("expected", value);
    }

    [Fact]
    public void Debounce_invokes_only_latest_action()
    {
        using var dispatcher = new DebounceDispatcher();
        var firstCalls = 0;
        var secondCalls = 0;
        var frame = new DispatcherFrame();
        var timeout = new DispatcherTimer(TimeSpan.FromSeconds(2), DispatcherPriority.Send,
            (_, _) => frame.Continue = false, Dispatcher.CurrentDispatcher);

        dispatcher.Debounce(20, () => firstCalls++);
        dispatcher.Debounce(20, () =>
        {
            secondCalls++;
            frame.Continue = false;
        });

        try
        {
            timeout.Start();
            Dispatcher.PushFrame(frame);
        }
        finally
        {
            timeout.Stop();
        }

        Assert.Equal(0, firstCalls);
        Assert.Equal(1, secondCalls);
    }
}
