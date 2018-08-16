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

		private static IKeyStore _keyStore  = new InMemoryKeyStore();
		public static bool TrustOnFirstUse { get; set; }

		public static CertificateAuthorityMode CertificateAuthorityMode { get; set; } =
			CertificateAuthorityMode.Distrust;


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
			var defaultResult = sslpolicyerrors == SslPolicyErrors.None &&
			                    (CertificateAuthorityMode == CertificateAuthorityMode.AlwaysTrust
			                     || CertificateAuthorityMode == CertificateAuthorityMode.TrustIfNotPinned && !_keyStore.IsPinned(request.Host));
			if (TrustOnFirstUse)
			{
				// Check default result 2nd in order to ensure key is added to store on first request
				return _keyStore.MatchesExistingOrAddIfNew(request.Host, certificate.GetPublicKey()) || defaultResult;
			}

			return defaultResult || _keyStore.MatchesExisting(request.Host, certificate.GetPublicKey());
		}
	}
}
