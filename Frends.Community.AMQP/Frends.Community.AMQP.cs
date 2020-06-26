using System;
using Amqp;
using Amqp.Framing;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Amqp.Sasl;

using System.Linq;


using Frends.Community.Amqp.Definitions;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Threading;

#pragma warning disable 1591

namespace Frends.Community.Amqp
{
    public static class Amqp
    {
        static readonly RemoteCertificateValidationCallback noneCertValidator = (a, b, c, d) => true;

        /// <summary>
        /// This is task
        /// Documentation: https://github.com/FrendsPlatform/Frends.Amqp
        /// </summary>
        /// <param name="input">Defines how to connect AMQP queue and message being sent.</param>
        /// <param name="options">Defines additional properties of connection.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { bool Success, JToken Message}</returns>
        public static async Task<ReceiveMessageResult> AmqpReceiver([PropertyTab] InputReceiver input, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            var conn = await CreateConnection(input.BusUri, options.SearchClientCertificateBy, options.DisableServerCertValidation, options.Issuer, options.PfxFilePath, options.PfxPassword);
            var session = new Session(conn);

            ReceiverLink receiver = new ReceiverLink(session, input.LinkName, input.QueueOrTopicName);

            var ret = new ReceiveMessageResult();

            cancellationToken.ThrowIfCancellationRequested();

            var message = await receiver.ReceiveAsync(new TimeSpan(0, 0, 0, options.Timeout));
            if (message != null)
            {
                receiver.Accept(message);
                ret.Success = true;
                ret.Body = JToken.FromObject(message.Body);
            }
            else
            {
                ret.Success = false;
                ret.Body = null;
            }
            await receiver.CloseAsync();
            await session.CloseAsync();
            await conn.CloseAsync();


            return ret;
        }

        /// <summary>
        /// This is task
        /// Documentation: https://github.com/FrendsPlatform/Frends.Amqp
        /// </summary>
        /// <param name="input">Defines how to connect AMQP queue and message being sent.</param>
        /// <param name="options">Defines additional properties of connection.</param>
        /// <param name="messageProperties">Defines additional properties of messgae.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { bool Success }</returns>
        public static async Task<SendMessageResult> AmqpSender([PropertyTab] InputSender input, [PropertyTab] Options options, [PropertyTab] AmqpMessageProperties messageProperties, CancellationToken cancellationToken)
        {
            var conn = await CreateConnection(input.BusUri, options.SearchClientCertificateBy, options.DisableServerCertValidation, options.Issuer, options.PfxFilePath, options.PfxPassword);
            var session = new Session(conn);
            var sender = new SenderLink(session, input.LinkName, input.QueueOrTopicName);

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await sender.SendAsync(CreateMessage(input.Message, messageProperties), new TimeSpan(0, 0, 0, options.Timeout));
            }
            finally
            {

                await sender.CloseAsync();
                await session.CloseAsync();
                await conn.CloseAsync();
            }

            var ret = new SendMessageResult
            {
                Success = true
            };
            return ret;
        }

        private static Message CreateMessage(AmqpMessage amqpMessage, AmqpMessageProperties messageProperties)
        {
            var message = new Message(amqpMessage.BodyAsString)
            {
                Properties = new Properties(),
                ApplicationProperties = new ApplicationProperties()
            };

            try
            {
                foreach (var applicationProperty in messageProperties.ApplicationProperties)
                {
                    message.ApplicationProperties.Map.Add(applicationProperty.Name, applicationProperty.Value);
                }

                message.Properties.MessageId = messageProperties.Properties.MessageId;
                message.Properties.AbsoluteExpiryTime = messageProperties.Properties.AbsoluteExpiryTime ?? DateTime.MaxValue;
                message.Properties.ContentEncoding = messageProperties.Properties.ContentEncoding;
                message.Properties.ContentType = messageProperties.Properties.ContentType;
                message.Properties.CorrelationId = messageProperties.Properties.CorrelationId;
                message.Properties.CreationTime = messageProperties.Properties.CreationTime ?? DateTime.UtcNow;
                message.Properties.GroupId = messageProperties.Properties.GroupId;
                message.Properties.GroupSequence = messageProperties.Properties.GroupSequence;
                message.Properties.ReplyToGroupId = messageProperties.Properties.ReplyToGroupId;
                message.Properties.ReplyTo = messageProperties.Properties.ReplyTo;
                message.Properties.Subject = messageProperties.Properties.Subject;
                message.Properties.UserId = messageProperties.Properties.UserId;
                message.Properties.To = messageProperties.Properties.To;
            }
            catch 
            {
                throw new Exception("Error in message properties.");
            }
            return message;
        }

        // TODO funcion is intentionally not used (calling code is commneted out)
        /*
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }
            else
            {
                throw new ArgumentException("Bad cert"); // TODO error message
            }
        }
        */

        // Fetches the first certificate whose CN contains the given string, another way https://github.com/Azure/amqpnetlite/blob/master/Examples/PeerToPeer/PeerToPeer.Certificate/Program.cs (note apache licence)
        public static X509Certificate2 FindCertificateByCn(string issuedBy)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            var certificate = store.Certificates.Cast<X509Certificate2>().FirstOrDefault(c => c.Issuer.Contains($"CN={issuedBy}"));

            store.Close();

            return certificate;
        }

        public static X509Certificate2 FindCertificateByFile(string pfxFilePath, string pfxPassword)
        {
            // Read certificate from file,
            // for reference: https://stackoverflow.com/questions/9951729/x509certificate-constructor-exception

            var certificate = new X509Certificate2(System.IO.File.ReadAllBytes(pfxFilePath)
                , pfxPassword
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

        public static async Task<Connection> CreateConnection(string busUri, SearchCertificateBy searchClientCertificateBy, bool disableServerCertValidation, string issuer, string pfxFilePath, string pfxPassword)
        {
            var factory = new ConnectionFactory();
            var brokerAddress = new Address(busUri);

            if (searchClientCertificateBy == SearchCertificateBy.DontUseCertificate)
            {
                // Don't authenticate with client cert

                if (disableServerCertValidation == true)
                {
                    factory.SSL.RemoteCertificateValidationCallback = noneCertValidator;
                }
                var conn = await factory.CreateAsync(brokerAddress); //.ConfigureAwait(false).GetAwaiter().GetResult();
                return conn;
            }
            else
            {
                // Do authenticate with client cert

                if (disableServerCertValidation == true) // Do NOT validate server certificate
                {
                    factory.SSL.RemoteCertificateValidationCallback = noneCertValidator;
                }

                // TODO how is cert validated when following code is commented out
                /*
                 else
                {
                    factory.SSL.RemoteCertificateValidationCallback = ValidateServerCertificate;
                }
                */


                X509Certificate2 certificate;
                if (searchClientCertificateBy == SearchCertificateBy.File)
                {
                    certificate = FindCertificateByFile(pfxFilePath, pfxPassword);
                }
                else if (searchClientCertificateBy == SearchCertificateBy.Issuer)
                {
                    certificate = FindCertificateByCn(issuer);
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

                var conn = await factory.CreateAsync(brokerAddress);

                return conn;
            }
        }
    }
}
