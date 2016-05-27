using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Beatshake.Core
{
    public class Cooldown
    {
        private bool _isCoolingDown;
        private Func<Task> _request;
        private Stopwatch _stopwatch = new Stopwatch();

        private int minElapsedSecondsForRequest = BeatshakeSettings.InstrumentalCooldown -
                                                  BeatshakeSettings.MaxInstrumentalRequestDelay;

        public bool IsCoolingDown;

        public async Task Activate()
        {
            if (IsCoolingDown) // a new activation ist not supported, while old one is running
            {
                throw new InvalidOperationException("Cooldown is in progress");
            }

            while (_request != null)
            {
                IsCoolingDown = true;
                await HandleRequest();
            }

            IsCoolingDown = false;
        }

        private async Task HandleRequest()
        {
            // Start request as soon as possible
            var delay = Task.Delay(BeatshakeSettings.InstrumentalCooldown);
            var task = _request.Invoke();

            // Set timer and cooldown Data
            _stopwatch.Start();

            // Wait for task finishing
            //Task.WaitAll(Task.Delay(BeatshakeSettings.InstrumentalCooldown), task);
            _request = null; // delete handled request
            await delay;
            _stopwatch.Stop();
            _stopwatch.Reset();
        }

        /// <summary>
        /// Returns true, if the request has been added
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public bool TryAddAsyncRequest(Func<Task> func)
        {
            if (_request == null && (!_stopwatch.IsRunning || _stopwatch.ElapsedMilliseconds > minElapsedSecondsForRequest))
            {
                _request = func;
                return true;
            }
            return false;
        }



    }
}