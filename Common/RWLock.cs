using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common
{
    public class RWLock
    {
        class ReaderLock : IDisposable
        {
            ReaderWriterLock _lock;

            public ReaderLock(ReaderWriterLock @lock)
            {
                _lock = @lock;
                _lock.AcquireReaderLock(Timeout.Infinite);
            }

            public void Dispose()
            {
                _lock.ReleaseReaderLock();
            }
        }

        class WriterLock : IDisposable
        {
            ReaderWriterLock _lock;

            public WriterLock(ReaderWriterLock @lock)
            {
                _lock = @lock;
                _lock.AcquireWriterLock(Timeout.Infinite);
            }

            public void Dispose()
            {
                _lock.ReleaseWriterLock();
            }
        }

        ReaderWriterLock _lock = new ReaderWriterLock();

        public IDisposable Write()
        {
            return new WriterLock(_lock);
        }

        public IDisposable Read()
        {
            return new ReaderLock(_lock);
        }
    }

    public class RWLock<T> : RWLock
    {
        public T Object { get; private set; }

        public RWLock(T obj)
        {
            this.Object = obj;
        }
    }
}
