using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atron.MessageBus
{
    /// <summary>
    /// 消息总线
    /// </summary>
    public class MessageBus : IMessageBus
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 发布策略
        /// </summary>
        public PublishStrategy Strategy { get; set; } = PublishStrategy.SyncContinueOnException;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MessageBus()
            : this(Guid.NewGuid().ToString())
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        public MessageBus(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="TMsg">消息类型</typeparam>
        /// <param name="msg">消息对象</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task PublishAsync<TMsg>(TMsg msg, CancellationToken cancellationToken = default)
        {
            if (msg == null)
            {
                throw new ArgumentNullException(nameof(msg));
            }

            await HandlerRepository<TMsg>.Invoke(msg, cancellationToken, Strategy).ConfigureAwait(false);
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam name="TMsg"></typeparam>
        /// <param name="handler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Subscribe<TMsg>(MessageHandler<TMsg> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            HandlerRepository<TMsg>.Register(handler);
        }

        /// <summary>
        /// 取消订阅消息
        /// </summary>
        /// <typeparam name="TMsg"></typeparam>
        /// <param name="handler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void UnSubscribe<TMsg>(MessageHandler<TMsg> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            HandlerRepository<TMsg>.UnRegister(handler);
        }
    }
}
