using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CertPinnerRestSharp
{
	public class InMemoryKeyStore : IKeyStore
	{
		private readonly ConcurrentDictionary<string, byte[]> _backingStore = new ConcurrentDictionary<string, byte[]>();
		public bool MatchesExistingPinOrIsNew(string host, byte[] publicKey)
		{
			var normalizedHost = host.ToUpperInvariant();
			byte[] oldKey;
			if (_backingStore.TryGetValue(normalizedHost, out oldKey))
			{
				return publicKey.SequenceEqual(oldKey);
			}
			else
			{
				if (_backingStore.TryAdd(normalizedHost, publicKey))
				{
					return true;
				}
				else
				{
					// two different request attempted to set value for same key
					// we only consider the first which makes it in.
					return MatchesExistingPinOrIsNew(host, publicKey);
				}
			}

			return false;
		}

		public bool MatchesExisting(string host, byte[] publicKey)
		{
			var normalizedHost = host.ToUpperInvariant();
			byte[] oldKey;
			if (_backingStore.TryGetValue(normalizedHost, out oldKey))
			{
				return publicKey.SequenceEqual(oldKey);
			}

			return false;
		}
	}
}
