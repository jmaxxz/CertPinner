using System.Runtime.Serialization;

namespace CertPinner.KeyStorage
{
	[DataContract]
	public class HostKeyPair
	{
		[DataMember]
		public string Host { get; set; }
		[DataMember]
		public byte[] PublicKey { get; set; }
	}
}
