using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using RestSharp;

namespace CertPinnerRestSharp
{
	public class RestPinner
	{
		private readonly InMemoryKeyStore _keyStore;
		public bool TrustOnFirstUse { get; set; }

		/// <summary>
		///
		/// </summary>
		public bool TrustCertificateAuthorities { get; set; }
		public bool TrustExpired { get; set; }

		public RestPinner(InMemoryKeyStore keyStore)
		{
			_keyStore = keyStore;
		}
		public void EnablePinning(IRestClient restClient)
		{
			restClient.RemoteCertificateValidationCallback = RemoteCertificateValidationCallback;
		}

		private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
		{
			if (sender is HttpWebRequest request)
			{
				return RemoteCertificateValidationCallback(request, certificate, chain, sslpolicyerrors);
			}

			return false;
		}

		private bool RemoteCertificateValidationCallback(HttpWebRequest request, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
		{
			if (TrustOnFirstUse)
			{
				return _keyStore.MatchesExistingPinOrIsNew(request.Host, certificate);
			}

			return false;
		}
	}
}
