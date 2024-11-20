using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeriodicBatching.Models
{
    public class PeriodicBatchingConfiguration<TEvent>
    {
#if true
        /// <summary>
        /// BatchingFunc方法被调用时的列表大小。默认的50。//list size for when BatchingFunc method is called. default 50.
        /// </summary>
        public int BatchSizeLimit { get; set; } = 50;
        /// <summary>
        /// 执行主方法BatchingFunc的时间间隔。默认10秒。//interval to execute main method - BatchingFunc. default 10s.
        /// </summary>
        public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(10);
        /// <summary>
        /// (必需的)委托调用来处理批处理(required) //delegate called to process batch
        /// </summary>
        public Action<List<TEvent>> BatchingFunc { get; set; }
        /// <summary>
        /// 连续失败X次后，当前批将被丢弃。使用-1取消此行为。默认的5。//after X consecutive failures, current batch will be dropped. Use -1 to cancel this behavior. default 5.
        /// </summary>
        public int FailuresBeforeDroppingBatch { get; set; } = 5;
        /// <summary>
        /// 连续失败X次后，所有队列项将被丢弃。使用-1取消此行为。deafult 10。//after X consecutive failures, all queue items will be dropped. Use -1 to cancel this behavior. deafult 10.
        /// </summary>
        public int FailuresBeforeDroppingQueue { get; set; } = 10;
        /// <summary>
        /// (可选)当单个失败发生时调用的委托-异常，失败计数和队列计数作为参数传递。//(optional) delegate called when a single failure happens - exception, failures count and queue count are passed as parameter.
        /// </summary>
        public Action<Exception, int, int> SingleFailureCallback { get; set; }
        /// <summary>
        /// (可选)在批处理被删除时调用的委托-当前批处理项作为参数传递。//(optional) delegate called when a batch is dropped - current batch items are passed as parameter.
        /// </summary>
        public Action<List<TEvent>> DropBatchCallback { get; set; }
        /// <summary>
        /// (可选)当队列被删除时调用的委托-所有队列项都作为参数传递。//(optional) delegate called when a queue is dropped - all queue items are passed as parameter.
        /// </summary>
        public Action<List<TEvent>> DropQueueCallback { get; set; }
        /// <summary>
        /// 故障发生的下一个时间间隔。对于每个错误，间隔根据该值呈指数增长。默认3秒。//interval used for next inteval when a failure happens. for each error, interval is increased exponentially based on this value. default 3s.
        /// </summary>
        public TimeSpan MinimumBackoffPeriod { get; set; } = TimeSpan.FromSeconds(3);
        /// <summary>
        /// 最大时间间隔。这个参数将限制指数区间。默认的5分钟。//max interval. this parameter will limit the exponential interval. default 5m.
        /// </summary>
        public TimeSpan MaximumBackoffInterval { get; set; } = TimeSpan.FromMinutes(5);

        public void Validate()
        {
            if (this.BatchSizeLimit < 0)
            {
                throw new ArgumentOutOfRangeException("BatchSizeLimit must be greater then 0");
            }

            if (this.Period < TimeSpan.FromSeconds(1))
            {
                throw new ArgumentOutOfRangeException($"Period must be greater then or equal to 1 second");
            }

            if (this.MinimumBackoffPeriod < TimeSpan.FromSeconds(1))
            {
                throw new ArgumentOutOfRangeException($"MinimumBackoffPeriod must be greater then or equal to 1 second");
            }

            if (this.MaximumBackoffInterval < this.MinimumBackoffPeriod || this.MaximumBackoffInterval < this.Period)
            {
                throw new ArgumentOutOfRangeException($"MaximumBackoffInterval must be greater then or equal to MinimumBackoffPeriod and Period");
            }

            if (this.BatchingFunc == null)
            {
                throw new ArgumentNullException("BatchingFunc cannot be null");
            }
        }

#else
        public int BatchSizeLimit { get; set; } = 50;

        public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(10);

        public Func<List<TEvent>, Task> BatchingFunc { get; set; }

        public int FailuresBeforeDroppingBatch { get; set; } = 5;

        public int FailuresBeforeDroppingQueue { get; set; } = 10;

        public Func<Exception, int, int, Task> SingleFailureCallback { get; set; }

        public Func<List<TEvent>, Task> DropBatchCallback { get; set; }

        public Func<List<TEvent>, Task> DropQueueCallback { get; set; }

        public TimeSpan MinimumBackoffPeriod { get; set; } = TimeSpan.FromSeconds(3);

        public TimeSpan MaximumBackoffInterval { get; set; } = TimeSpan.FromMinutes(5);

        public void Validate()
        {
            if (this.BatchSizeLimit < 0)
            {
                throw new ArgumentOutOfRangeException("BatchSizeLimit must be greater then 0");
            }

            if (this.Period < TimeSpan.FromSeconds(1))
            {
                throw new ArgumentOutOfRangeException($"Period must be greater then or equal to 1 second");
            }

            if (this.MinimumBackoffPeriod < TimeSpan.FromSeconds(1))
            {
                throw new ArgumentOutOfRangeException($"MinimumBackoffPeriod must be greater then or equal to 1 second");
            }

            if (this.MaximumBackoffInterval < this.MinimumBackoffPeriod || this.MaximumBackoffInterval < this.Period)
            {
                throw new ArgumentOutOfRangeException($"MaximumBackoffInterval must be greater then or equal to MinimumBackoffPeriod and Period");
            }

            if (this.BatchingFunc == null)
            {
                throw new ArgumentNullException("BatchingFunc cannot be null");
            }
        }
    }
#endif
    }
}
