using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace CertPinner
{
	public static class CertificatePinner
	{
		public static IKeyStore KeyStore
		{
			get => _keyStore;
			set => _keyStore = value ?? new NullKeyStore();
		}

		private static IKeyStore _keyStore  = new NullKeyStore();
		public static bool TrustOnFirstUse { get; set; }
		public static bool TrustCertificateAuthorities { get; set; }
		public static bool AllowExpired { get; set; }


		public static void Enable()
		{
			ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallback;
		}

		public static bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
		{
			if (sender is HttpWebRequest request)
			{
				return CertificateValidationCallback(request, certificate, chain, sslpolicyerrors);
			}

			return false;
		}

		private static bool CertificateValidationCallback(HttpWebRequest request, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
		{
			if (TrustOnFirstUse)
			{
				return _keyStore.MatchesExistingOrIsNew(request.Host, certificate.GetPublicKey());
			}

			return _keyStore.MatchesExisting(request.Host, certificate.GetPublicKey());
		}
	}
}
