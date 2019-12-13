using System;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;

using Frends.Community.AMQP.Definitions;

namespace Frends.Community.AMQP.Tests
{
    [TestFixture]
    class TestClass
    {

        ///
        /// First you need openssl, either install it, or if you have allready installed Git, that includes it, add openssl to the env:
        /// $env:OPENSSL_CONF = "${env:ProgramFiles}\Git\usr\ssl\openssl.cnf"
        /// In folder: Frends.Community.AMQP\TestAmqpBroker
        /// execute: openssl req -x509 -out localhost.crt -keyout localhost.key -newkey rsa:2048 -nodes -sha256 -subj '/CN=localhost' -extensions EXT -config .\cloudflareEsimerkki.cnf
        ///
        /// Then install localhost.crt to cert store, you can use localhost.key to later generate client certificates.
        ///
        /// Client certificates are not how ever tested as Amqp.Net Lite does not supor them. General principle how to test them is, however, given in connection secureConnectionWithClientCert.
        ///
        /// Start AMQP server by (ensure that you use same addresses in command and in tests:
        /// .\TestAmqpBroker.exe amqp://localhost:5676 amqps://localhost:5677 /creds:guest:guest /cert:localhost
        /// 

        private static string insecureBus = "amqp://guest:guest@localhost:5676";

        private static string secureBus = "amqps://guest:guest@localhost:5677";

        private static string queue = "q1";

        // private static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\testikansio\");

        static AmqpConnection insecureConnection = new AmqpConnection
        {
            BusUri = insecureBus,
            QueueOrTopicName = queue,
            LinkName = Guid.NewGuid().ToString(),
            Timeout = 15,
            SearchCertificateBy = SearchCertificateBy.DontUseCertificate
        };

        static AmqpConnection secureConnection = new AmqpConnection
        {
            BusUri = secureBus,
            QueueOrTopicName = queue,
            LinkName = Guid.NewGuid().ToString(),
            Timeout = 15,
            SearchCertificateBy = SearchCertificateBy.DontUseCertificate
        };

        // Intentionally nt used, but this way you can use client certificates.
        static AmqpConnection secureConnectionWithClientCert = new AmqpConnection
        {
            BusUri = secureBus,
            QueueOrTopicName = queue,
            LinkName = Guid.NewGuid().ToString(),
            Timeout = 15,
            SearchCertificateBy = SearchCertificateBy.File,
            Issuer = "localhost",
            PfxPassword = "pw",
            PfxFilePath = "\\TestAmqpBroker\\privatekey.pfx"

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

            Assert.NotNull(ret);
            Assert.That(ret.Success, Is.True);

            var ret2 = await ClassName.AmqpReceiver(insecureReceiverConnection);

            Assert.NotNull(ret2);

            Assert.That(ret2.Body.ToString(), Is.EqualTo("Hello AMQP!"));
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

            Assert.That(ret2.Body.ToString(), Is.EqualTo("Hello AMQP!"));

        }
    }
}
