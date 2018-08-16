namespace CertPinner
{
	/// <summary>
	/// A key store that does not store any keys.
	/// Treats all keys as invalid.
	/// </summary>
	public class NullKeyStore : IKeyStore
	{
		public bool MatchesExistingOrIsNew(string host, byte[] publicKey)
		{
			return false;
		}

		public bool MatchesExisting(string host, byte[] publicKey)
		{
			return false;
		}
	}
}
