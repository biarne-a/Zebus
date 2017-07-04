using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abc.Zebus.Util;

namespace Abc.Zebus.Core
{
    public class SendHistoryAwareBus : IBus, IBusSendHistoryContainer
    {
        private readonly IBus _bus;
        private readonly ConcurrentDictionary<string, DateTime> _sentMessages;
        
        public IDictionary<string, DateTime> GetSentMessages() => _sentMessages.ToDictionary(x => x.Key, x => x.Value);

        public SendHistoryAwareBus(IBus bus)
        {
            _bus = bus;
            _sentMessages = new ConcurrentDictionary<string, DateTime>();
        }

        public PeerId PeerId => _bus.PeerId;
        public string Environment => _bus.Environment;
        public bool IsRunning => _bus.IsRunning;

        public void Configure(PeerId peerId, string environment) => _bus.Configure(peerId, environment);

        public void Publish(IEvent message)
        {
            PersistSentMessage(message);
            _bus.Publish(message);
        }

        public Task<CommandResult> Send(ICommand message) => _bus.Send(message);

        public Task<CommandResult> Send(ICommand message, Peer peer)
        {
            PersistSentMessage(message);
            return _bus.Send(message, peer);
        }

        public IDisposable Subscribe(Subscription subscription, SubscriptionOptions options = SubscriptionOptions.Default)
            => _bus.Subscribe(subscription);

        public IDisposable Subscribe(Subscription[] subscriptions, SubscriptionOptions options = SubscriptionOptions.Default)
            => _bus.Subscribe(subscriptions);

        public IDisposable Subscribe<T>(Action<T> handler) where T : class, IMessage
            => _bus.Subscribe(handler);

        public IDisposable Subscribe(Subscription[] subscriptions, Action<IMessage> handler)
            => _bus.Subscribe(subscriptions, handler);

        public IDisposable Subscribe(Subscription subscription, Action<IMessage> handler)
            => _bus.Subscribe(subscription, handler);

        public void Reply(int errorCode) => _bus.Reply(errorCode);

        public void Reply(int errorCode, string message) => _bus.Reply(errorCode, message);

        public void Reply(IMessage response) => _bus.Reply(response);

        public void Start() => _bus.Start();
        public void Stop() => _bus.Stop();

        public event Action Starting
        {
            add => _bus.Starting += value;
            remove => _bus.Starting -= value;
        }
        public event Action Started
        {
            add => _bus.Started += value;
            remove => _bus.Started -= value;
        }
        public event Action Stopping
        {
            add => _bus.Stopping += value;
            remove => _bus.Stopping -= value;
        }

        public event Action Stopped
        {
            add => _bus.Stopped += value;
            remove => _bus.Stopped -= value;
        }

        public void Dispose() => _bus.Dispose();

        private void PersistSentMessage(IMessage message)
        {
            _sentMessages.AddOrUpdate(message.GetType().FullName, SystemDateTime.UtcNow, (fullName, oldTimeStamp) => SystemDateTime.UtcNow);
        }
    }
}
