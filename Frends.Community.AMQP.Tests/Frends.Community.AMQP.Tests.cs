using System;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;

using Frends.Community.Amqp.Definitions;

namespace Frends.Community.Amqp.Tests
{
    [TestFixture]
    class TestClass
    {
        ///
        /// Start AMQP server before executing test by (ensure that you use same addresses in command and in tests):
        /// .\TestAmqpBroker.exe amqp://localhost:5676 amqps://localhost:5677 /creds:guest:guest /cert:localhost
        /// 
        /// If you have valid tls cert you may want to enable cert validation.

        private static readonly string insecureBus = "amqp://guest:guest@localhost:5673";
        private static readonly string secureBus = "amqps://guest:guest@localhost:5678";
        private static readonly string queue = "q1";
        private static readonly bool disableServerCertValidation = true;

        static readonly AmqpProperties properties = new AmqpProperties
        {
            MessageId = Guid.NewGuid().ToString()
        };

        static readonly AmqpMessage message = new AmqpMessage()
        {
            BodyAsString = "Hello AMQP!",
        };

        static readonly AmqpMessageProperties amqpMessageProperties = new AmqpMessageProperties
        {
            ApplicationProperties = new ApplicationProperty[] { },
            Properties = properties
        };

        static  InputSender inputSender = new InputSender
        {
            Message = message,
            QueueOrTopicName = queue,
        };

        static InputReceiver inputReceiver = new InputReceiver
        {
            QueueOrTopicName = queue,
        };

        static Options optionsDontUseClientCert = new Options
        {
            Timeout = 15,
            LinkName = Guid.NewGuid().ToString(),
            SearchClientCertificateBy = SearchCertificateBy.DontUseCertificate,
            DisableServerCertValidation = disableServerCertValidation
        };

        // This way you can use client certificates.
        /*
        static Options optionsUseClientCert = new Options
        {
            Timeout = 15,
            SearchClientCertificateBy = SearchCertificateBy.File,
            Issuer = "localhost",
            PfxPassword = "pw",
            PfxFilePath = "\\TestAmqpBroker\\privatekey.pfx"
        };
        */

        [Test]
        public async Task TestInsecure()
        {
            inputSender.BusUri = insecureBus;
            inputReceiver.BusUri = insecureBus;

            var ret = await Amqp.AmqpSender(inputSender, optionsDontUseClientCert, amqpMessageProperties, new System.Threading.CancellationToken());

            Assert.NotNull(ret);
            Assert.That(ret.Success, Is.True);

            var ret2 = await Amqp.AmqpReceiver(inputReceiver, optionsDontUseClientCert, new System.Threading.CancellationToken());

            Assert.NotNull(ret2);
            Assert.NotNull(ret2.Body);
            Assert.That(ret2.Body.ToString(), Is.EqualTo("Hello AMQP!"));
        }


        [Test]
        public async Task TestWithSecureConnection()
        {
            inputSender.BusUri = secureBus;
            inputReceiver.BusUri = secureBus;

            var ret = await Amqp.AmqpSender(inputSender, optionsDontUseClientCert, amqpMessageProperties, new System.Threading.CancellationToken());

            Assert.NotNull(ret);
            Assert.That(ret.Success, Is.True);

            var ret2 = await Amqp.AmqpReceiver(inputReceiver, optionsDontUseClientCert, new System.Threading.CancellationToken());
            
            Assert.NotNull(ret2);
            Assert.NotNull(ret2.Body);
            Assert.That(ret2.Body.ToString(), Is.EqualTo("Hello AMQP!"));
        }
    }
}
