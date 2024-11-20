using PeriodicBatching.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeriodicBatching.Interfaces
{
    public interface IPeriodicBatching<TEvent> : IDisposable
    {
#if true
        Action<List<TEvent>> BatchingFunc { get; }

        void Setup(PeriodicBatchingConfiguration<TEvent> config);

        void Add(TEvent _event);

        void Flush();

#else
        Func<List<TEvent>, Task> BatchingFunc { get; }

        void Setup(PeriodicBatchingConfiguration<TEvent> config);

        void Add(TEvent _event);

        void Flush();
#endif
    }
}
