using System;
using System.Threading;

namespace WALLE.Link.Utils
{
    internal class DisposableCancellationToken : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public DisposableCancellationToken(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
        }


        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                    _cancellationTokenSource.Cancel();
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
