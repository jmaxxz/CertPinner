using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using RestSharp;

namespace CertPinnerRestSharp
{
	public class RestPinner
	{
		private readonly bool _trustOnFirstuse;
		private readonly bool _trustCertificateAuthorities;
		private readonly bool _trustExpired;

		public RestPinner(bool trustOnFirstuse = false, bool trustCertificateAuthorities = false, bool trustExpired = false)
		{
			_trustOnFirstuse = trustOnFirstuse;
			_trustCertificateAuthorities = trustCertificateAuthorities;
			_trustExpired = trustExpired;
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
