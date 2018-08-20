using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using CertPinner.AutoPinPolicies;
using CertPinner.KeyStorage;

namespace CertPinner
{
	public static class CertificatePinner
	{
		private static readonly NeverAutoPin NeverAutoPin = new NeverAutoPin();

		public static IKeyStore KeyStore
		{
			get => _keyStore;
			set => _keyStore = value ?? new NullKeyStore();
		}

		private static IKeyStore _keyStore  = new InMemoryKeyStore();
		private static IAutomaticPinPolicy _automaticPinPolicy = NeverAutoPin;

		public static IAutomaticPinPolicy AutomaticPinPolicy
		{
			get => _automaticPinPolicy;
			set => _automaticPinPolicy = value ?? NeverAutoPin;
		}

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

			if (_automaticPinPolicy.CanPin(request.Host.ToUpperInvariant()))
			{
				// Check default result 2nd in order to ensure key is added to store on first request
				return _keyStore.MatchesExistingOrAddIfNew(request.Host, certificate.GetPublicKey()) || defaultResult;
			}

			return defaultResult || _keyStore.MatchesExisting(request.Host, certificate.GetPublicKey());
		}
	}
}
