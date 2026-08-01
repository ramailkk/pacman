using System;
using System.Diagnostics;
using System.Threading;

namespace PacManGame
{
    public class GameLoop
    {
        public const double TicksPerSecond = 60.0;
        public const double SecondsPerTick = 1.0 / TicksPerSecond;
        private bool running;

        public void Run()
        {
            running = true;
            var stopwatch = Stopwatch.StartNew();
            double previousTime = stopwatch.Elapsed.TotalSeconds;
            double accumulator = 0.0;

            while (running)
            {
                double currentTime = stopwatch.Elapsed.TotalSeconds;
                double frameTime = currentTime - previousTime;
                previousTime = currentTime;
                accumulator += frameTime;

                while (accumulator >= SecondsPerTick)
                {
                    Update();
                    accumulator -= SecondsPerTick;
                }

                Render();

                Thread.Sleep(1);
            }
        }

        public void Stop()
        {
            running = false;
        }

        private void Update()
        {
            // called at a fixed 60 ticks/second, regardless of render speed
        }

        private void Render()
        {
            // called as often as the loop can manage; draw current state here
        }
    }
}