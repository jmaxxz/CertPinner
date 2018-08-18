namespace CertPinner.KeyStorage
{
	/// <summary>
	/// A key store that does not store any keys.
	/// Treats all keys as invalid.
	/// </summary>
	public class NullKeyStore : IKeyStore
	{
		public bool MatchesExistingOrAddIfNew(string host, byte[] publicKey)
		{
			return false;
		}

		public bool MatchesExisting(string host, byte[] publicKey)
		{
			return false;
		}

		public bool IsPinned(string host)
		{
			return false;
		}

		public void PinForHost(string host, byte[] publicKey)
		{
		}
	}
}
