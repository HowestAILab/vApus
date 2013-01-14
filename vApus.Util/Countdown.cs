/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.Timers;

namespace vApus.Util
{
    public class Countdown : IDisposable
    {
        #region Events

        /// <summary>
        ///     Occurs when started;
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        ///     Occurs when stopped;
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        ///     Occurs when the time period has elapsed.
        /// </summary>
        public event EventHandler Tick;

        #endregion

        private readonly int _toCountdownTime;
        private int _countdownTime;

        private bool _isdisposed;
        private Timer _tmr;

        /// <summary>
        ///     Inits and starts the countdown.
        /// </summary>
        /// <param name="countdownTime">In ms.</param>
        /// <param name="reportProgressTime">In ms. Min 100</param>
        public Countdown(int countdownTime, int reportProgressTime = 1000)
        {
            _countdownTime = countdownTime;
            _toCountdownTime = countdownTime;

            _tmr = new Timer(reportProgressTime);
            _tmr.Elapsed += _tmr_Elapsed;
        }

        public int CountdownTime
        {
            get { return _countdownTime; }
        }

        public int ToCountdownTime
        {
            get { return _toCountdownTime; }
        }

        public void Dispose()
        {
            if (!_isdisposed)
            {
                _isdisposed = true;
                _tmr.Dispose();
                _tmr = null;
            }
        }

        public void Start()
        {
            _tmr.Start();

            if (Started != null)
                foreach (EventHandler del in Started.GetInvocationList())
                    del.BeginInvoke(this, null, null, null);
        }

        public void Stop()
        {
            _tmr.Stop();
            Dispose();

            if (Stopped != null)
                foreach (EventHandler del in Stopped.GetInvocationList())
                    del.BeginInvoke(this, null, null, null);
        }

        private void _tmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            _countdownTime -= (int) _tmr.Interval;
            if (_countdownTime <= 0)
            {
                _tmr.Stop();
                _countdownTime = 0;
            }

            if (Tick != null)
                foreach (EventHandler del in Tick.GetInvocationList())
                    del.BeginInvoke(this, e, null, null);

            if (_countdownTime == 0)
                Stop();
        }
    }
}