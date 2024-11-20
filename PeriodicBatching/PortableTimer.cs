using System;
using System.Threading;
using System.Threading.Tasks;

namespace PeriodicBatching
{
    internal class PortableTimer : IDisposable
    {
#if true 
        readonly object StateLock = new object();

        readonly Action<CancellationToken> OnTickDelegate;

        readonly CancellationTokenSource CancelOperation = new CancellationTokenSource();

        bool IsRunning;

        bool IsDisposed;

        private TimeSpan _interval;
        private Thread _thread;
        public PortableTimer(Action<CancellationToken> onTick)
        {
            this.OnTickDelegate = onTick ?? throw new ArgumentNullException(nameof(onTick));
        
        }

        public void Start(TimeSpan interval)
        {
            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(interval));
            }
            _interval = interval;
            lock (this.StateLock)
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(PortableTimer));
                }
                if (_thread != null)
                {
                    Thread.Sleep(10);
                    _thread.Interrupt(); 
                }
                _thread = new Thread(StartBgThread);
                _thread.Start();
            }
        }

        public void SetNextInterval(TimeSpan _nextInterval)
        {
            if (_nextInterval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(_nextInterval));
            }
            _interval = _nextInterval;
        }

        private void StartBgThread()
        {
            try
            {
                while (_interval > TimeSpan.Zero && !this.CancelOperation.Token.IsCancellationRequested)
                {
                    this.OnTick();
                    Thread.Sleep(_interval);
                }
            }
            catch (ThreadInterruptedException)
            { 
            }
        }

        private void OnTick()
        {
            try
            {
                lock (this.StateLock)
                {
                    if (this.IsDisposed)
                    {
                        return;
                    }

                    if (this.IsRunning)
                    {
                        Monitor.Wait(StateLock);

                        if (this.IsDisposed)
                        {
                            return;
                        }
                    }

                    this.IsRunning = true;
                }

                if (!this.CancelOperation.Token.IsCancellationRequested)
                {
                    this.OnTickDelegate?.Invoke(this.CancelOperation.Token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                lock (this.StateLock)
                {
                    this.IsRunning = false;
                    Monitor.PulseAll(this.StateLock);
                }
            }
        }

        public void Dispose()
        {
            this.CancelOperation.Cancel();

            lock (this.StateLock)
            {
                if (this.IsDisposed)
                {
                    return;
                }

                while (this.IsRunning)
                {
                    Monitor.Wait(StateLock);
                }

                this.IsDisposed = true;
            }
        }
#else 
        readonly object StateLock = new object();

        readonly Func<CancellationToken, Task> OnTickDelegate;

        readonly CancellationTokenSource CancelOperation = new CancellationTokenSource();

        bool IsRunning;

        bool IsDisposed;

        public PortableTimer(Func<CancellationToken, Task> onTick)
        {
            this.OnTickDelegate = onTick ?? throw new ArgumentNullException(nameof(onTick));
        }

        public void Start(TimeSpan interval)
        {
            if (interval < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(interval));
            }

            lock (this.StateLock)
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(PortableTimer));
                }

                Task.Delay(interval, CancelOperation.Token)
                    .ContinueWith(x => OnTick(),
                        CancellationToken.None,
                        TaskContinuationOptions.DenyChildAttach,
                        TaskScheduler.Default);
            }
        }

        async Task OnTick()
        {
            try
            {
                lock (this.StateLock)
                {
                    if (this.IsDisposed)
                    {
                        return;
                    }

                    if (this.IsRunning)
                    {
                        Monitor.Wait(StateLock);

                        if (this.IsDisposed)
                        {
                            return;
                        }
                    }

                    IsRunning = true;
                }

                if (!this.CancelOperation.Token.IsCancellationRequested)
                {
                    await this.OnTickDelegate(CancelOperation.Token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                lock (this.StateLock)
                {
                    this.IsRunning = false;
                    Monitor.PulseAll(this.StateLock);
                }
            }
        }

        public void Dispose()
        {
            this.CancelOperation.Cancel();

            lock (this.StateLock)
            {
                if (this.IsDisposed)
                {
                    return;
                }

                while (this.IsRunning)
                {
                    Monitor.Wait(StateLock);
                }

                this.IsDisposed = true;
            }
        }
    }
#endif
    }
}
