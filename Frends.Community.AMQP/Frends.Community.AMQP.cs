using System;
using Amqp;
using Amqp.Framing;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Amqp.Sasl;

using System.Linq;

using Frends.Community.AMQP.Definitions;

#pragma warning disable 1591

namespace Frends.Community.AMQP
{

    public class ClassName
    {
        /// <summary>
        /// This is task
        /// Documentation: https://github.com/CommunityHiQ/Frends.Community.AMQP
        /// </summary>
        /// <param name="connection">Defines how to connect AMQP queue.</param>
        /// <returns></returns>
        public static async Task<ReceiveMessageResult> AmqpReceiver(AmqpConnection connection)
        {

            var message = await AmqpReceiverAdvanced(connection);

            var ret = new ReceiveMessageResult
            {
                Success = true,
                Message = message.Body.ToString()
            };
            
            return ret;
        }
        
        /// <summary>
        /// Return Message class from Amqp.Net Lite. 
        /// Documentation: https://github.com/CommunityHiQ/Frends.Community.AMQP
        /// </summary>
        /// <param name="connection">Defines how to connect AMQP queue.</param>
        /// <returns></returns>
        public static async Task<Message> AmqpReceiverAdvanced(AmqpConnection connection)
        {
            var conn = await CreateConnection(connection);
            var session = new Session(conn);

            ReceiverLink receiver = new ReceiverLink(session, connection.LinkName, connection.QueueOrTopicName);

            var message = await receiver.ReceiveAsync(new TimeSpan(0, 0, 0, connection.Timeout));
            if (message != null)
            {
                receiver.Accept(message);
                // TODO mitä jos ei löydy
            }
            await receiver.CloseAsync();
            await session.CloseAsync();
            await conn.CloseAsync();

            // https://azure.github.io/amqpnetlite/api/Amqp.Message.html

            return message;
        }

        /// <summary>
        /// This is task
        /// Documentation: https://github.com/CommunityHiQ/Frends.Community.AMQP
        /// </summary>
        /// <param name="connection">Defines how to connect AMQP queue.</param>
        /// <param name="message">Message to be sent to the queue.</param>
        /// <returns></returns>
        public static async Task<sendMessageResult> AmqpSender(AmqpConnection connection, AmqpMessage message)
        {
            var ret = await AmqpSenderAdvanced(connection, CreateMessage(message));
            
            return ret;
        }

        /// <summary>
        /// This is task
        /// Documentation: https://github.com/CommunityHiQ/Frends.Community.AMQP
        /// </summary>
        /// <param name="connection">Defines how to connect AMQP queue.</param>
        /// <param name="message">Message to be sent to the queue.</param>
        /// <returns></returns>
        public static async Task<sendMessageResult> AmqpSenderAdvanced(AmqpConnection connection, Message message)
        {
            var conn = await CreateConnection(connection);
            var session = new Session(conn);
            var sender = new SenderLink(session, connection.LinkName, connection.QueueOrTopicName);

            try
            {
                await sender.SendAsync(message, new TimeSpan(0, 0, 0, connection.Timeout));
            }
            finally
            {

                await sender.CloseAsync();
                await session.CloseAsync();
                await conn.CloseAsync();
            }

            var ret = new sendMessageResult
            {
                Success = true
            };
            // TODO mitä jos Timeout 
            
            return ret;
        }

        private static Message CreateMessage(AmqpMessage amqpMessage)
        {
            var message = new Message(amqpMessage.BodyAsString)
            {
                Properties = new Properties(),
                ApplicationProperties = new ApplicationProperties()
            };

            foreach (var applicationProperty in amqpMessage.ApplicationProperties)
            {
                message.ApplicationProperties.Map.Add(applicationProperty.Name, applicationProperty.Value);
            }

            message.Properties.MessageId = amqpMessage.Properties.MessageId;
            message.Properties.AbsoluteExpiryTime = amqpMessage.Properties.AbsoluteExpiryTime ?? DateTime.MaxValue;
            message.Properties.ContentEncoding = amqpMessage.Properties.ContentEncoding;
            message.Properties.ContentType = amqpMessage.Properties.ContentType;
            message.Properties.CorrelationId = amqpMessage.Properties.CorrelationId;
            message.Properties.CreationTime = amqpMessage.Properties.CreationTime ?? DateTime.UtcNow;
            message.Properties.GroupId = amqpMessage.Properties.GroupId;
            message.Properties.GroupSequence = amqpMessage.Properties.GroupSequence;
            message.Properties.ReplyToGroupId = amqpMessage.Properties.ReplyToGroupId;
            message.Properties.ReplyTo = amqpMessage.Properties.ReplyTo;
            message.Properties.Subject = amqpMessage.Properties.Subject;
            message.Properties.UserId = amqpMessage.Properties.UserId;
            message.Properties.To = amqpMessage.Properties.To;

            return message;
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            throw new ArgumentException("Bad cert");
        }

        // Fetches the first certificate whose CN contains the given string, another way https://github.com/Azure/amqpnetlite/blob/master/Examples/PeerToPeer/PeerToPeer.Certificate/Program.cs (note apache licence)
        public static X509Certificate2 FindCertificateByCn(string issuedBy)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            var certificate = store.Certificates.Cast<X509Certificate2>().FirstOrDefault(c => c.Issuer.Contains($"CN={issuedBy}"));

            store.Close();

            return certificate;
        }

        public static X509Certificate2 FindCertificateByFile(AmqpConnection connection)
        {
            // Read certificate from file,
            // for reference: https://stackoverflow.com/questions/9951729/x509certificate-constructor-exception

            var certificate = new X509Certificate2(System.IO.File.ReadAllBytes(connection.PfxFilePath)
                , connection.PfxPassword
                , X509KeyStorageFlags.MachineKeySet |
                  X509KeyStorageFlags.PersistKeySet |
                  X509KeyStorageFlags.Exportable);
            /*
                Another way:
                factory.SSL.LocalCertificateSelectionCallback = (a, b, c, d, e) => X509Certificate.CreateFromCertFile(certFile);
                factory.SSL.ClientCertificates.Add(X509Certificate.CreateFromCertFile(certFile));

             */
            return certificate;
        }

        public static async Task<Connection> CreateConnection(AmqpConnection connection)
        {
            if (connection.SearchCertificateBy == SearchCertificateBy.DontUseCertificate)
            {
                Address address = new Address(connection.BusUri);
                var conn = new Connection(address);

                return conn;
            }
            else
            {
                var factory = new ConnectionFactory();

                factory.SSL.RemoteCertificateValidationCallback = ValidateServerCertificate;
                // Connection.DisableServerCertValidation = true; // TODO REMOVE this from production
                factory.SSL.RemoteCertificateValidationCallback = (a, b, c, d) => true;  // TODO REMOVE this from production

                var certificate = new X509Certificate2();

                if (connection.SearchCertificateBy == SearchCertificateBy.File)
                {
                    certificate = FindCertificateByFile(connection);
                }
                else if (connection.SearchCertificateBy == SearchCertificateBy.Issuer)
                {
                    certificate = FindCertificateByCn(connection.Issuer);
                }
                else
                {
                    throw new ArgumentException("You should not be here!");
                }

                if (certificate == null)
                {
                    throw new ArgumentException($"Could not find certificate");
                }

                var expireDate = DateTime.Parse(certificate.GetExpirationDateString());

                if (expireDate < DateTime.Now)
                {
                    throw new Exception($"Certificate has already expired: '{expireDate}'");
                }

                factory.SSL.ClientCertificates.Add(certificate);
                factory.SASL.Profile = SaslProfile.External;

                var brokerAddress = new Address(connection.BusUri);
                var conn = await factory.CreateAsync(brokerAddress);

                return conn;
            }
        }
    }
}
