using System;
using Abc.Zebus.Core;
using Abc.Zebus.Testing.Extensions;
using Abc.Zebus.Tests.Messages;
using Abc.Zebus.Util;
using Moq;
using NUnit.Framework;

namespace Abc.Zebus.Tests.Core
{
    [TestFixture]
    public class SendHistoryAwareBusTests 
    {
        private Mock<IBus> _busMock;
        private SendHistoryAwareBus _bus;

        [SetUp]
        public void Setup()
        {
            _busMock = new Mock<IBus>();
            var peer = new Peer(new PeerId("Abc.Testing.Peer"), "tcp://abctest:123");
            _busMock.Setup(x => x.Send(It.IsAny<ICommand>()))
                    .Callback<ICommand>(cmd => _bus.Send(new FakeCommand(12), peer));
            _bus = new SendHistoryAwareBus(_busMock.Object);
        }
        
        [Test]
        public void should_be_aware_of_sent_command()
        {
            var now = new DateTime(2017, 01, 02, 10, 30, 27);

            using (SystemDateTime.Set(utcNow: now))
                _bus.Send(new FakeCommand(12));

            var sentMessages = _bus.GetSentMessages();
            sentMessages.Count.ShouldEqual(1);
            var messageTypeFullName = typeof(FakeCommand).FullName;
            sentMessages[messageTypeFullName].ShouldEqual(now);
        }

        [Test]
        public void should_be_aware_of_published_event()
        {
            var now = new DateTime(2017, 01, 02, 10, 30, 27);

            using (SystemDateTime.Set(utcNow: now))
                _bus.Publish(new FakeEvent(12));

            var sentMessages = _bus.GetSentMessages();
            sentMessages.Count.ShouldEqual(1);
            var messageTypeFullName = typeof(FakeEvent).FullName;
            sentMessages[messageTypeFullName].ShouldEqual(now);
        }

        [Test]
        public void should_only_keep_track_of_last_reception_time()
        {
            var now = new DateTime(2017, 01, 02, 10, 30, 27);

            using (SystemDateTime.Set(utcNow: now))
                _bus.Publish(new FakeEvent(12));

            var sentMessages = _bus.GetSentMessages();

            sentMessages.Count.ShouldEqual(1);
            var messageTypeFullName = typeof(FakeEvent).FullName;
            sentMessages[messageTypeFullName].ShouldEqual(now);
        }
    }
}
