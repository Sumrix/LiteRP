using System.Timers;
using Timer = System.Timers.Timer;

namespace LiteRP.WebApp.Utilities;

/// <summary>
/// Provides a debouncing mechanism for any type of action.
/// It raises the Debounced event only after the specified DebounceInterval
/// has passed since the last call to the Update method.
/// </summary>
public class Debouncer<TValue> : IDisposable
{
    private readonly Timer _timer;
    private TValue? _pendingValue;
    private bool _hasPendingUpdate = false;
    private int _debounceInterval;

    /// <summary>
    /// The event that fires after the debounce interval has elapsed.
    /// </summary>
    public event Action<TValue?>? Debounced;

    /// <summary>
    /// Gets or sets the debounce interval in milliseconds.
    /// Changing this value will re-evaluate the timer for any pending update.
    /// </summary>
    public int DebounceInterval
    {
        get => _debounceInterval;
        set
        {
            if (_debounceInterval == value)
            {
                return;
            }

            _timer.Stop();
            _debounceInterval = value;

            if (_hasPendingUpdate)
            {
                DebouncePendingUpdate();
            }
        }
    }
    
    /// <summary>
    /// Initializes a new instance of the Debouncer class.
    /// </summary>
    /// <param name="debounceInterval">The initial debounce interval. Defaults to 0 (disabled).</param>
    public Debouncer(int debounceInterval = 0)
    {
        _debounceInterval = debounceInterval;

        _timer = new Timer();
        _timer.AutoReset = false; // The timer should fire only once per cycle.
        _timer.Elapsed += OnTimerElapsed;
    }

    /// <summary>
    /// Updates the value and restarts the debounce timer.
    /// </summary>
    /// <param name="value">The new value to be processed after the delay.</param>
    public void Update(TValue? value)
    {
        _pendingValue = value;
        _hasPendingUpdate = true;

        DebouncePendingUpdate();
    }
    
    /// <summary>
    /// Encapsulates the logic to either fire the event immediately or restart the timer.
    /// </summary>
    private void DebouncePendingUpdate()
    {
        if (DebounceInterval <= 0)
        {
            // If debouncing is disabled, execute the action immediately.
            OnTimerElapsed(null, null);
        }
        else
        {
            // Otherwise, restart the timer with the current interval.
            _timer.Interval = DebounceInterval;
            _timer.Stop();
            _timer.Start();
        }
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs? e)
    {
        // Prevent firing if there's no longer a pending update (e.g., it was just flushed).
        if (!_hasPendingUpdate)
        {
            return;
        }

        _hasPendingUpdate = false;
        Debounced?.Invoke(_pendingValue);
    }

    /// <summary>
    /// Releases the resources used by the timer.
    /// </summary>
    public void Dispose()
    {
        _timer.Stop();
        _timer.Elapsed -= OnTimerElapsed;
        _timer.Dispose();
        GC.SuppressFinalize(this);
    }
}