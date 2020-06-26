using System.ComponentModel;
using System;
using Amqp.Types;
using System.ComponentModel.DataAnnotations;

#pragma warning disable 1591

namespace Frends.Community.Amqp.Definitions
{
    public class InputReceiver
    {
        /// <summary>
        /// The URI for the AMQP Message bus, username and key must be url encoded.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("\"amqps://<username>:<key>@<host>:<port>\"")]
        public string BusUri { get; set; }

        /// <summary>
        /// Name of target queue or topic.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string QueueOrTopicName { get; set; }


        /// <summary>
        /// Link name.
        /// </summary>
        [DefaultValue("{{Guid.NewGuid().ToString()}}")]
        [DisplayFormat(DataFormatString = "Text")]
        public string LinkName { get; set; }

    }

    public class InputSender
    {
        public AmqpMessage Message { get; set; }
        /// <summary>
        /// The URI for the AMQP Message bus, username and key must be url encoded.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("\"amqps://<username>:<key>@<host>:<port>\"")]
        public string BusUri { get; set; }

        /// <summary>
        /// Name of target queue or topic.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string QueueOrTopicName { get; set; }


        /// <summary>
        /// Link name.
        /// </summary>
        [DefaultValue("{{Guid.NewGuid().ToString()}}")]
        [DisplayFormat(DataFormatString = "Text")]
        public string LinkName { get; set; }

    }

    public class Options
    {
        /// <summary>
        /// Timeout in seconds for receiving or sending Message to the queue.
        /// </summary>
        [DefaultValue(30)]
        public int Timeout { get; set; }

        /// <summary>
        /// Select whether certificate is used and where it can be found.
        /// </summary>
        [DefaultValue(SearchCertificateBy.DontUseCertificate)]
        public SearchCertificateBy SearchClientCertificateBy { get; set; }

        /// <summary>
        /// Issuer of certificate.
        /// </summary>
        [UIHint(nameof(SearchClientCertificateBy),"", SearchCertificateBy.Issuer)]
        [DisplayFormat(DataFormatString = "Text")]
        public string Issuer { get; set; }

        /// <summary>
        /// Path where .pfx (certificate) file can be found.
        /// </summary>
        [UIHint(nameof(SearchClientCertificateBy), "", SearchCertificateBy.File)]
        [DisplayFormat(DataFormatString = "Text")]
        public string PfxFilePath { get; set; }

        /// <summary>
        /// Password for the certificate.
        /// </summary>
        [UIHint(nameof(SearchClientCertificateBy), "", SearchCertificateBy.File)]
        [DisplayFormat(DataFormatString = "Text")]
        public string PfxPassword { get; set; }

        /// <summary>
        /// Disable server certificate validation when TLS is used. False means certificate is validated. If connection is not secured with tls this option does not do anything.
        /// </summary>
        [DefaultValue(false)]
        public bool DisableServerCertValidation { get; set; }


    }

    /// <summary>
    /// Describes if Message was sent successfully.
    /// </summary>
    public class SendMessageResult
    {
        /// <summary>
        /// True if Message was sent successfully.
        /// </summary>
        public bool Success;
    }

    public class ReceiveMessageResult
    {
        /// <summary>
        /// True if Message was received successfully.
        /// </summary>
        public bool Success;
        /// <summary>
        /// The body (content) of the message.
        /// </summary>
        public object Body;
    }

    public class AmqpMessage
    {

        /// <summary>
        /// The message body.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public object BodyAsString { get; set; }
    }

    public class AmqpMessageProperties
    {

        /// <summary>
        /// Application properties section of an AMQP messages.
        /// </summary>
        public ApplicationProperty[] ApplicationProperties { get; set; }

        /// <summary>
        /// The Immutable properties of the Message.
        /// </summary>
        public AmqpProperties Properties { get; set; }
    }

    /// <summary>
    /// The Immutable properties of the Message. https://azure.github.io/amqpnetlite/api/Amqp.Framing.Properties.html
    /// </summary>
    public class AmqpProperties
    {
        private uint _groupSequence;
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("Guid.NewGuid().ToString()")]
        public string MessageId { get; set; }

        public DateTime? AbsoluteExpiryTime { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public string ContentEncoding { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public string ContentType { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public string CorrelationId { get; set; }

        public DateTime? CreationTime { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public string GroupId { get; set; }

        [DefaultValue(0)]
        public UInt32 GroupSequence
        {
            get { return _groupSequence; }
            set { _groupSequence = value; }
        }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public string ReplyToGroupId { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public string ReplyTo { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public string Subject { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public byte[] UserId { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public string To { get; set; }

    }

    /// <summary>
    /// Application properties section of an AMQP messages. https://azure.github.io/amqpnetlite/api/Amqp.Framing.ApplicationProperties.html
    /// </summary>
    public class ApplicationProperty
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public enum SearchCertificateBy
    {
        DontUseCertificate,
        Issuer,
        File
    };
    // Yet Another way https://github.com/Azure/amqpnetlite/blob/master/Examples/PeerToPeer/PeerToPeer.Certificate/Program.cs



}