/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;

namespace vApus.Util
{
    public class Countdown : IDisposable
    {
        #region Events
        /// <summary>
        /// Occurs when started;
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Occurs when stopped;
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Occurs when the time period has elapsed.
        /// </summary>
        public event EventHandler Tick;
        #endregion

        private System.Timers.Timer _tmr;
        //Use this to stop the timer.
        private int _countdownTime, _toCountdownTime;

        private bool _isdisposed;

        public uint CountdownTime
        {
            get { return (uint)_countdownTime; }
        }

        public uint ToCountdownTime
        {
            get { return (uint)_toCountdownTime; }
        }

        /// <summary>
        /// Inits and starts the countdown.
        /// </summary>
        /// <param name="countdownTime">In ms.</param>
        /// <param name="reportProgressTime">In ms. Min 100</param>
        public Countdown(uint countdownTime, uint reportProgressTime = 1000)
        {
            _countdownTime = (int)countdownTime;
            _toCountdownTime = (int)countdownTime;

            _tmr = new System.Timers.Timer(reportProgressTime);
            _tmr.Elapsed += _tmr_Elapsed;
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
        private void _tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _countdownTime -= (int)_tmr.Interval;
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
        public void Dispose()
        {
            if (!_isdisposed)
            {
                _isdisposed = true;
                _tmr.Dispose();
                _tmr = null;
            }
        }
    }
}
