using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atron.MessageBus
{
    /// <summary>
    /// 发布策略
    /// </summary>
    public enum PublishStrategy
    {
        /// <summary>
        /// 同步发布，遇到错误继续
        /// </summary>
        SyncContinueOnException = 0,

        /// <summary>
        /// 同步发布，遇到错误停止
        /// </summary>
        SyncStopOnException = 1,

        /// <summary>
        /// 异步发布
        /// </summary>
        Async = 2,

        /// <summary>
        /// 并行发布并直接返回
        /// </summary>
        ParallelNoWait = 3,

        /// <summary>
        /// 并行发布，全部完成时返回
        /// </summary>
        ParallelWhenAll = 4,

        /// <summary>
        /// 并行发布，任意一个完成时返回
        /// </summary>
        ParallelWhenAny = 5,
    }
}
