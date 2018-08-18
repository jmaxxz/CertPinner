using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CertPinner.KeyStorage
{
	public class HostKeyPair
	{
		public string Host { get; }
		public IReadOnlyCollection<byte> PublicKey { get; }

		public HostKeyPair(string host, IReadOnlyCollection<byte> publicKey)
		{
			Host = host;
			PublicKey = publicKey;
		}
	}
}
