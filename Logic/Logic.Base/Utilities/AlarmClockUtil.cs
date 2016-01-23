namespace s2.s2Utils.Logic.Base.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using s2.s2Utils.Logic.Portable.Utilities;

    /// <summary>
    /// Simulates an alarm clock using events.
    /// </summary>
    public class AlarmClockUtil : IDisposable
    {
        #region member vars

        private long _alarms;
        private List<TimeSpan> _alarmTimes;
        private bool _isDisposed;
        private bool _onlyOnce;
        private Timer _timer;

        #endregion

        #region event declarations

        /// <summary>
        /// Is fired when the clock is started.
        /// </summary>
        public event EventHandler AlarmClockStarted;

        /// <summary>
        /// Is fired when the clock is stopped.
        /// </summary>
        public event EventHandler AlarmClockStopped;

        /// <summary>
        /// Is fired when the clock detects an alarm time.
        /// </summary>
        public event EventHandler AlarmOccured;

        #endregion

        #region properties

        /// <summary>
        /// Indicates whether the alarm clock is running currently.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Retrieves the next start time detected by the timer.
        /// </summary>
        public DateTime NextPlannedStart { get; private set; }

        #endregion

        #region methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts handling of one or many alarms.
        /// </summary>
        /// <param name="alarmTimes">The times when to start alarms.</param>
        /// <param name="onlyOnces">Indicates whether the clock should run forever.</param>
        public void Start(IEnumerable<TimeSpan> alarmTimes, bool onlyOnces = true)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Stop a running alarm clock first.");
            }
            CheckUtil.ThrowIfNull(() => alarmTimes);
            if (!alarmTimes.Any())
            {
                throw new ArgumentException("No times specified.");
            }
            if (AlarmOccured == null)
            {
                throw new InvalidOperationException("No listener on AlarmOccured event. Alarm is useless.");
            }
            IsRunning = true;
            _alarms = 0;
            _alarmTimes = alarmTimes.ToList();
            _onlyOnce = onlyOnces;
            AlarmClockStarted?.Invoke(this, EventArgs.Empty);
            StartNextTimer();
        }

        /// <summary>
        /// Stops all alarm timers.
        /// </summary>
        public void Stop()
        {
            DisposeTimer();
            IsRunning = false;
            if (AlarmClockStopped != null)
            {
                AlarmClockStopped(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Is used to savely dispose all unmanaged resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> if this was called from the internal disposing method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            if (disposing)
            {
                DisposeTimer();
            }
            _isDisposed = true;
        }

        /// <summary>
        /// Is used to dispose the internal timer.
        /// </summary>
        private void DisposeTimer()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }

        /// <summary>
        /// Starts the next applicapable timer.
        /// </summary>
        private void StartNextTimer()
        {
            DisposeTimer();
            var now = DateTime.Now;
            var nextTime = _alarmTimes.Select(t => t.GetTimeByTimespan(now)).Where(t => t > now).Min();
            NextPlannedStart = nextTime;
            var interval = (long)nextTime.Subtract(now).TotalMilliseconds;
            _timer = new Timer(
                o =>
                {
                    _alarms++;                    
                    if (AlarmOccured != null)
                    {
                        Task.Run(() => AlarmOccured(this, EventArgs.Empty));
                    }
                    if (!_onlyOnce ||_alarms < _alarmTimes.Count)
                    {
                        // not all alarms started or revolving handling
                        StartNextTimer();
                    }
                    else
                    {
                        // no more alarms needed
                        Stop();
                    }
                },
                null,
                interval,
                Timeout.Infinite);
        }

        #endregion
    }
}