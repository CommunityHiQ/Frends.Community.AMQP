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
        /// Start AMQP server by (ensure that you use same addresses in command and in tests:
        /// .\TestAmqpBroker.exe amqp://localhost:5676 amqps://localhost:5677 /creds:guest:guest /cert:localhost
        /// 

        private static string insecureBus = "amqp://guest:guest@localhost:5673";

        private static string secureBus = "amqps://guest:guest@localhost:5678";

        private static string queue = "q1";

        private static bool disableServerCertValidation = true;

        static AmqpProperties properties = new AmqpProperties
        {
            MessageId = Guid.NewGuid().ToString()
        };

        static AmqpMessage message = new AmqpMessage()
        {
            BodyAsString = "Hello AMQP!",

        };

        static AmqpMessageProperties amqpMessageProperties = new AmqpMessageProperties()
        {
            ApplicationProperties = new ApplicationProperty[] { },
            Properties = properties
        };


        static InputReceiver inputReceiver = new InputReceiver
        {
            QueueOrTopicName = queue,
            LinkName = Guid.NewGuid().ToString()
        };

        static InputSender inputSender = new InputSender
        {
            Message = message,
            QueueOrTopicName = queue,
            LinkName = Guid.NewGuid().ToString(),

        };

        static Options optionsDontUseClientCert = new Options
        {
            Timeout = 15,
            SearchClientCertificateBy = SearchCertificateBy.DontUseCertificate,
            DisableServerCertValidation = disableServerCertValidation
        };

        // Intentionally not used, but this way you can use client certificates.
        static Options optionsUseClientCert = new Options
        {
            Timeout = 15,
            SearchClientCertificateBy = SearchCertificateBy.File,
            Issuer = "localhost",
            PfxPassword = "pw",
            PfxFilePath = "\\TestAmqpBroker\\privatekey.pfx"
        };

        [Test]
        public async Task TestInsecure()
        {

            inputSender.BusUri = insecureBus;
            inputReceiver.BusUri = insecureBus;

            var ret = await Amqp.AmqpSender(inputSender, optionsDontUseClientCert, amqpMessageProperties);

            Assert.NotNull(ret);
            Assert.That(ret.Success, Is.True);

            var ret2 = await Amqp.AmqpReceiver(inputReceiver, optionsDontUseClientCert);

            Assert.NotNull(ret2);
            Assert.That(ret2.Body.ToString(), Is.EqualTo("Hello AMQP!"));
        }


        [Test]
        public async Task TestWithSecureConnection()
        {
            inputSender.BusUri = secureBus;
            inputReceiver.BusUri = insecureBus;

            var ret = await Amqp.AmqpSender(inputSender, optionsDontUseClientCert, amqpMessageProperties);

            Assert.NotNull(ret);
            Assert.That(ret.Success, Is.True);

            var ret2 = await Amqp.AmqpReceiver(inputReceiver, optionsDontUseClientCert);
            
            Assert.NotNull(ret2);
            Assert.That(ret2.Body.ToString(), Is.EqualTo("Hello AMQP!"));
        }
    }
}
