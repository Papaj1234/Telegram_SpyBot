using System.Collections.Concurrent;

namespace TelegramSpyBot.App.Common;

public class ExpiringStore<TValue>
    where TValue : Common.Interfaces.IExpirable
{
    private readonly ConcurrentDictionary<long, TValue> items = new();
    private readonly TimeSpan lifetime;

    public ExpiringStore(TimeSpan lifetime)
    {
        this.lifetime = lifetime;
    }

    public event Func<TValue, Task>? Expired;

    public TValue? Get(long key)
        => items.TryGetValue(key, out var value) ? value : default;

    public bool TryAdd(TValue value)
    {
        if (!items.TryAdd(value.Key, value))
        {
            return false;
        }

        StartTimer(value);
        return true;
    }

    public bool Remove(long key)
    {
        if (items.TryRemove(key, out var value))
        {
            Cancel(value);
            return true;
        }

        return false;
    }

    public void Cancel(TValue value)
    {
        if (!value.TimeoutCts.IsCancellationRequested)
        {
            value.TimeoutCts.Cancel();
        }
    }

    private void StartTimer(TValue value)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(lifetime, value.TimeoutCts.Token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            items.TryRemove(value.Key, out _);

            if (Expired is { } handler)
            {
                await handler(value);
            }
        });
    }
}