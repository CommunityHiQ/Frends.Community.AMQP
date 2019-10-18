using System;
using System.Threading.Tasks;
using NUnit.Framework;

using Frends.Community.AMQP.Definitions;

namespace Frends.Community.AMQP.Tests
{
    [TestFixture]
    class TestClass
    {
        static AmqpConnection insecureConnection = new AmqpConnection
        {
            BusUri = "amqp://guest:guest@localhost:5673",
            QueueOrTopicName = "q1",
            LinkName = Guid.NewGuid().ToString(),
            Timeout = 15,
            SearchCertificateBy = SearchCertificateBy.DontUseCertificate
        };

        static AmqpConnection secureConnection = new AmqpConnection
        {
            BusUri = "amqps://guest:guest@localhost:5671",
            QueueOrTopicName = "q1",
            LinkName = Guid.NewGuid().ToString(),
            Timeout = 15,
            SearchCertificateBy = SearchCertificateBy.DontUseCertificate
        };

        static AmqpConnection secureConnectionWithClientCert = new AmqpConnection
        {
            BusUri = "amqps://guest:guest@localhost:5671",
            QueueOrTopicName = "q1",
            LinkName = Guid.NewGuid().ToString(),
            Timeout = 15,
            SearchCertificateBy = SearchCertificateBy.File,
            Issuer = "lisette",
            PfxPassword = "loris",
            PfxFilePath = "C:\\Users\\galkios\\OneDrive - HiQ Finland Oy\\Frends and Co sources\\AMQP test\\TestAmqpBroker\\public_privatekey.pfx"

        };

        static AmqpProperties properties = new AmqpProperties
        {
            MessageId = Guid.NewGuid().ToString()
        };

        static AmqpMessage message = new AmqpMessage()
        {
            BodyAsString = "Hello AMQP!",
            ApplicationProperties = new ApplicationProperty[] { },
            Properties = properties
        };


        [Test]
        public async Task TestInsecure()
        {
            var insecureSenderConnection = insecureConnection;
            var insecureReceiverConnection = insecureConnection;

            insecureSenderConnection.LinkName = Guid.NewGuid().ToString();
            insecureReceiverConnection.LinkName = Guid.NewGuid().ToString();
            
            var ret = await ClassName.AmqpSender(insecureSenderConnection, message);

            Assert.That(ret.Success, Is.True);

            var ret2 = await ClassName.AmqpReceiver(insecureReceiverConnection);

            Assert.That(ret2.Message, Is.EqualTo("Hello AMQP!"));
        }


        [Test]
        public async Task TestWithSecureConnection()
        {
            var secureSenderConnection = secureConnection;
            var secureReceiverConnection = secureConnection;

            secureSenderConnection.LinkName = Guid.NewGuid().ToString();
            secureReceiverConnection.LinkName = Guid.NewGuid().ToString();

            var ret = await ClassName.AmqpSender(secureSenderConnection, message);

            Assert.That(ret, Is.True);

            var ret2 = await ClassName.AmqpReceiver(secureReceiverConnection);

            Assert.That(ret2.Message, Is.EqualTo("Hello AMQP!"));

        }
    }
}
