using System.Collections.Concurrent;
using System.Linq;

namespace CertPinner
{
	public class InMemoryKeyStore : IKeyStore
	{
		// Right now we store the full public key. If this takes up too much
		// memory can switch to storing a signature.
		private readonly ConcurrentDictionary<string, byte[]> _backingStore = new ConcurrentDictionary<string, byte[]>();
		public bool MatchesExistingOrIsNew(string host, byte[] publicKey)
		{
			var normalizedHost = host.ToUpperInvariant();
			if (_backingStore.TryGetValue(normalizedHost, out var oldKey))
			{
				return publicKey.SequenceEqual(oldKey);
			}

			if (_backingStore.TryAdd(normalizedHost, publicKey))
			{
				return true;
			}

			// two different request attempted to set value for same key
			// we only consider the first which makes it in.
			return MatchesExistingOrIsNew(normalizedHost, publicKey);
		}

		public bool MatchesExisting(string host, byte[] publicKey)
		{
			var normalizedHost = host.ToUpperInvariant();
			if (_backingStore.TryGetValue(normalizedHost, out var oldKey))
			{
				return publicKey.SequenceEqual(oldKey);
			}

			return false;
		}

		public void PinForHost(string host, byte[] publicKey)
		{
			var normalizedHost = host.ToUpperInvariant();
			_backingStore[normalizedHost] = publicKey;
		}
	}
}
