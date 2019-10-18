using System.ComponentModel;
using System;
using Amqp.Types;
using System.ComponentModel.DataAnnotations;



namespace Frends.Community.AMQP.Definitions
{
    public class AmqpConnection
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

        /// <summary>
        /// Timeout in seconds for receiving or sending Message to the queue.
        /// </summary>
        [DisplayFormat(DataFormatString = "Int")]
        public int Timeout;

        /// <summary>
        /// Select whether certificate is used and where it can be found.
        /// </summary>
        [DefaultValue(SearchCertificateBy.DontUseCertificate)]
        public SearchCertificateBy SearchCertificateBy { get; set; }

        /// <summary>
        /// Issuer of certificate.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string Issuer;

        /// <summary>
        /// Path where .pfx (certificate) file can be found.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string PfxFilePath;

        /// <summary>
        /// Password for the certificate.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string PfxPassword;

    }

    /// <summary>
    /// Describes if Message was sent successfully.
    /// </summary>
    public class sendMessageResult
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
        public string Message;
    }

    public class AmqpMessage
    {
        public object BodyAsString { get; set; }
        public ApplicationProperty[] ApplicationProperties { get; set; }
        public AmqpProperties Properties { get; set; }
    }

    public class AmqpMessageInMIKÄTÄMÄON
    {
        // https://azure.github.io/amqpnetlite/api/Amqp.Message.html#Amqp_Message_Body
        public object Body { get; set; }
        // https://azure.github.io/amqpnetlite/api/Amqp.Types.RestrictedDescribed.html
        public Descriptor descriptor { get; set; }
    }

    public class AmqpProperties
    {
        private uint _groupSequence;
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("Guid.NewGuid().ToString()")]
        public string MessageId { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
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

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public DateTime? CreationTime { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("")]
        public string GroupId { get; set; }

        [DisplayFormat(DataFormatString = "Int")]
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