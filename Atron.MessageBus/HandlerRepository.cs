using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atron.MessageBus
{
    internal static class HandlerRepository<TMsg>
    {
        private static readonly List<MessageHandler<TMsg>> _handlers = new();
        private static readonly Dictionary<PublishStrategy, Func<TMsg, CancellationToken, Task>> _publishStrategies = new();

        public static IEnumerable<MessageHandler<TMsg>> Handlers => _handlers;

        static HandlerRepository()
        {
            _publishStrategies[PublishStrategy.SyncContinueOnException] = SyncContinueOnException;
            _publishStrategies[PublishStrategy.SyncStopOnException] = SyncStopOnException;
            _publishStrategies[PublishStrategy.Async] = AsyncContinueOnException;
            _publishStrategies[PublishStrategy.ParallelNoWait] = ParallelNoWait;
            _publishStrategies[PublishStrategy.ParallelWhenAll] = ParallelWhenAll;
            _publishStrategies[PublishStrategy.ParallelWhenAny] = ParallelWhenAny;
        }

        public static void Register(MessageHandler<TMsg> handler)
        {
            lock (_handlers)
            {
                if (!_handlers.Contains(handler))
                {
                    _handlers.Add(handler);
                }
            }
        }

        public static void UnRegister(MessageHandler<TMsg> handler)
        {
            lock (_handlers)
            {
                _handlers.Remove(handler);
            }
        }

        public static async Task Invoke(TMsg msg, CancellationToken cancellationToken, PublishStrategy strategy)
        {
            if (_publishStrategies.ContainsKey(strategy))
            {
                await _publishStrategies[strategy].Invoke(msg, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new ArgumentException($"Unknown strategy: {strategy}", nameof(strategy));
            }
        }

        private static Task ParallelWhenAll(TMsg msg, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            foreach (var handler in _handlers)
            {
                tasks.Add(Task.Run(() => handler(msg, cancellationToken), CancellationToken.None));
            }

            return Task.WhenAll(tasks);
        }

        private static Task ParallelWhenAny(TMsg msg, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            foreach (var handler in _handlers)
            {
                tasks.Add(Task.Run(() => handler(msg, cancellationToken), CancellationToken.None));
            }

            return Task.WhenAny(tasks);
        }

        private static Task ParallelNoWait(TMsg msg, CancellationToken cancellationToken)
        {
            foreach (var handler in _handlers)
            {
                Task.Run(() => handler(msg, cancellationToken), CancellationToken.None);
            }

            return Task.CompletedTask;
        }

        private static async Task AsyncContinueOnException(TMsg msg, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            var exceptions = new List<Exception>();

            foreach (var handler in _handlers)
            {
                try
                {
                    tasks.Add(handler(msg, cancellationToken));
                }
                catch (Exception ex) when (!(ex is OutOfMemoryException || ex is StackOverflowException))
                {
                    exceptions.Add(ex);
                }
            }

            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                exceptions.AddRange(ex.Flatten().InnerExceptions);
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException || ex is StackOverflowException))
            {
                exceptions.Add(ex);
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }

        private static async Task SyncStopOnException(TMsg msg, CancellationToken cancellationToken)
        {
            foreach (var handler in _handlers)
            {
                await handler(msg, cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task SyncContinueOnException(TMsg msg, CancellationToken cancellationToken)
        {
            var exceptions = new List<Exception>();

            foreach (var handler in _handlers)
            {
                try
                {
                    await handler(msg, cancellationToken).ConfigureAwait(false);
                }
                catch (AggregateException ex)
                {
                    exceptions.AddRange(ex.Flatten().InnerExceptions);
                }
                catch (Exception ex) when (!(ex is OutOfMemoryException || ex is StackOverflowException))
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}
