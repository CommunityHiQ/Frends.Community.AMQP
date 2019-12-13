using System.ComponentModel;
using System;
using Amqp.Types;
using System.ComponentModel.DataAnnotations;

#pragma warning disable 1591

namespace Frends.Community.AMQP.Definitions.vanhako
{
    public class AmqpConnection
    {
        /// <summary>
        /// The URI for the AMQP message bus, username and key must be url encoded.
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
        public string linkName { get; set; }

        /// <summary>
        /// Timeout in seconds for receiving or sending message to the queue.
        /// </summary>
        [DisplayFormat(DataFormatString = "Int")]
        public int timeout;

        /// <summary>
        /// Select whether certificate is used and where it can be found.
        /// </summary>
        [DefaultValue(SearchCertificateBy.dontUseCertificate)]
        public SearchCertificateBy SearchCertificateBy { get; set; }

        /// <summary>
        /// Issuer of certificate.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string issuer;

        /// <summary>
        /// Path where .pfx (certificate) file can be found.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string pfxFilePath;

        /// <summary>
        /// Password for the certificate.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string pfxPassword;



    }

    /// <summary>
    /// Describes if message was sent successfully.
    /// </summary>
    public class sendMessageResult
    {
        /// <summary>
        /// True if message was sent successfully.
        /// </summary>
        public bool Success; 

    }

    public class ReceiveMessageResult
    {
        /// <summary>
        /// True if message was received successfully.
        /// </summary>
        public bool Success;


        public string message;

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
        public Descriptor descriptor{ get; set; }
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
        dontUseCertificate,
        Issuer,
        File
    };
    // Yet Another way https://github.com/Azure/amqpnetlite/blob/master/Examples/PeerToPeer/PeerToPeer.Certificate/Program.cs

}
