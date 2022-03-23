using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atron.MessageBus
{
    /// <summary>
    /// 消息处理器
    /// </summary>
    /// <typeparam name="TMsg">消息类型</typeparam>
    /// <param name="msg">消息对象</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public delegate Task MessageHandler<TMsg>(TMsg msg, CancellationToken cancellationToken);
}
