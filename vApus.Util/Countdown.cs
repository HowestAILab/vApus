/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Threading.Tasks;
using System.Timers;

namespace vApus.Util {
    /// <summary>
    /// A countdown class, can report progress and when the countdown has become 0.
    /// </summary>
    public class Countdown : IDisposable {
        
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


        private int _countdownTime, _reportProgressTime;

        private bool _isdisposed;
        private Timer _tmr;

        public int CountdownTime { get { return _countdownTime; } }

        /// <summary>
        ///     A countdown class, can report progress and when the countdown has become 0.
        ///     Inits and starts the countdown.
        /// </summary>
        /// <param name="countdownTime">In ms.</param>
        /// <param name="reportProgressTime">In ms. Min 100</param>
        public Countdown(int countdownTime, int reportProgressTime = 1000) {
            _countdownTime = countdownTime;
            _reportProgressTime = reportProgressTime;

            _tmr = new Timer(reportProgressTime);
            _tmr.Elapsed += _tmr_Elapsed;
        }

        private void _tmr_Elapsed(object sender, ElapsedEventArgs e) {
            _countdownTime -= _reportProgressTime;
            if (_countdownTime <= 0) {
                _tmr.Stop();
                _countdownTime = 0;
            }

            if (Tick != null) {
                var invocationList = Tick.GetInvocationList();
                Parallel.For(0, invocationList.Length, (i) => { (invocationList[i] as EventHandler).Invoke(this, e); });
            }

            if (_countdownTime == 0) Stop();
        }

        public void Start() {
            _tmr.Start();

            if (Started != null) {
                var invocationList = Started.GetInvocationList();
                Parallel.For(0, invocationList.Length, (i) => { (invocationList[i] as EventHandler).Invoke(this, null); });
            }
        }

        public void Stop() {
            _tmr.Stop();
            Dispose();

            if (Stopped != null) {
                var invocationList = Stopped.GetInvocationList();
                Parallel.For(0, invocationList.Length, (i) => { (invocationList[i] as EventHandler).Invoke(this, null); });
            }
        }

        public void Dispose() {
            if (!_isdisposed) {
                _isdisposed = true;
                _tmr.Dispose();
                _tmr = null;
            }
        }
    }
}