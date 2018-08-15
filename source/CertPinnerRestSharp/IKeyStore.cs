using System.Security.Cryptography.X509Certificates;

namespace CertPinnerRestSharp
{
	public interface IKeyStore
	{
		bool MatchesExistingPinOrIsNew(string host, byte[] publicKey);
	}
}
