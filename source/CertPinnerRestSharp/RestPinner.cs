using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using RestSharp;

namespace CertPinnerRestSharp
{
	public class RestPinner
	{
		public bool TrustOnFirstUse { get; set; }
		public bool TrustCertificateAuthorities { get; set; }
		public bool TrustExpired { get; set; }

		public RestPinner(bool trustOnFirstuse = false, bool trustCertificateAuthorities = false, bool trustExpired = false)
		{
			TrustOnFirstUse = trustOnFirstuse;
			TrustCertificateAuthorities = trustCertificateAuthorities;
			TrustExpired = trustExpired;
		}
		public void EnablePinning(IRestClient restClient)
		{
			restClient.RemoteCertificateValidationCallback = RemoteCertificateValidationCallback;
		}

		private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
		{
			return false;
		}
	}
}
