namespace Atron.MessageBus
{
    /// <summary>
    /// 消息总线接口
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="TMsg">消息类型</typeparam>
        /// <param name="msg">消息对象</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync<TMsg>(TMsg msg, CancellationToken cancellationToken = default);

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam name="TMsg">消息类型</typeparam>
        /// <param name="handler">消息处理器</param>
        void Subscribe<TMsg>(MessageHandler<TMsg> handler);

        /// <summary>
        /// 取消订阅消息
        /// </summary>
        /// <typeparam name="TMsg">消息类型</typeparam>
        /// <param name="handler">消息处理器</param>
        void UnSubscribe<TMsg>(MessageHandler<TMsg> handler);
    }
}