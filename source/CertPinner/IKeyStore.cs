using System.Runtime.CompilerServices;

namespace CertPinner
{
	public interface IKeyStore
	{
		bool MatchesExistingOrAddIfNew(string host, byte[] publicKey);
		bool MatchesExisting(string host, byte[] publicKey);
		bool IsPinned(string host);
		void PinForHost(string host, byte[] publicKey);
	}
}
