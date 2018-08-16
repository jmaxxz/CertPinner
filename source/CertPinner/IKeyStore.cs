namespace CertPinner
{
	public interface IKeyStore
	{
		bool MatchesExistingOrIsNew(string host, byte[] publicKey);
		bool MatchesExisting(string host, byte[] publicKey);
		void PinForHost(string host, byte[] publicKey);
	}
}
