using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace CertPinner
{
	[TestFixture]
	[NonParallelizable]
	public class PinnerTests
	{
		private static HttpClient GetClient(string baseUrl)
		{
			return new HttpClient {BaseAddress = new Uri(baseUrl, UriKind.Absolute)};
		}

		[OneTimeSetUp]
		public void OnTimeSetup()
		{
			CertificatePinner.Enable();
		}

		[SetUp]
		public void ResetToDefaults()
		{
			CertificatePinner.CertificateAuthorityMode = CertificateAuthorityMode.Distrust;
			CertificatePinner.TrustOnFirstUse = false;
			CertificatePinner.KeyStore = new InMemoryKeyStore();
		}

		[Test]
		public void Constructor_AllowsOverrideOfDefaults()
		{
			// Arrange
			// Act
			CertificatePinner.CertificateAuthorityMode = CertificateAuthorityMode.AlwaysTrust;
			CertificatePinner.TrustOnFirstUse = true;

			// Assert
			Assert.AreEqual(CertificatePinner.CertificateAuthorityMode, CertificateAuthorityMode.AlwaysTrust);
			Assert.IsTrue(CertificatePinner.TrustOnFirstUse);
		}


		[Category("Integration")]
		[TestCase("https://expired.badssl.com")] // Known bad cert
		[TestCase("https://google.com")] // Known good cert
		public void OnRequest_WhenDontTrustOnFirstUse_ResultsInError(string url)
		{
			// Arrange
			var restClient = GetClient(url);
			CertificatePinner.TrustOnFirstUse = false;

			// Act
			// Assert
			Assert.ThrowsAsync<HttpRequestException>(()=>restClient.GetAsync(""));
		}

		[Test]
		public async Task WhenTrustOnFirstUse_FirstRequest_ResultsInSuccess()
		{
			// Arrange
			using (var restClient = GetClient("https://google.com"))
			{
				CertificatePinner.TrustOnFirstUse = true;

				// Act
				// Assert
				Assert.DoesNotThrowAsync(()=>restClient.GetAsync(""));
			}
		}


		[Test]
		public void WhenTrustOnFirstUse_AfterPKChanges_ResultsInFailure()
		{
			// Arrange
			using (var restClient = GetClient("https://google.com"))
			{
				CertificatePinner.KeyStore = new InMemoryKeyStore();
				CertificatePinner.TrustOnFirstUse = true;

				// Act
				// Fake first request by just injecting key into store
				CertificatePinner.KeyStore.PinForHost("google.com", new byte[] {0, 0, 0});
				// Assert
				Assert.ThrowsAsync<HttpRequestException>(()=>restClient.GetAsync(""));
			}
		}

		[Test]
		public async Task WhenDontTrustOnFirstUse_WhenPublicKeyInStore_ResultsInSuccess()
		{
			// Arrange
			using (var restClient = GetClient("https://google.com"))
			{
				CertificatePinner.TrustOnFirstUse = true;
				await restClient.GetAsync("");

				// Act
				// Fake first request by just injecting key into store
				CertificatePinner.TrustOnFirstUse = false;
				// Assert
				Assert.DoesNotThrowAsync(()=>restClient.GetAsync(""));
			}
		}

		[TestCase(false)]
		[TestCase(true)]
		public void WhenAlwaysTrustCAs_WhenPinSaysNoButCaSaysYes_ResultsInSuccess(bool trustOnFirstUse)
		{
			// Arrange
			using (var restClient = GetClient("https://google.com"))
			{
				CertificatePinner.KeyStore = new InMemoryKeyStore();
				CertificatePinner.TrustOnFirstUse = trustOnFirstUse;
				CertificatePinner.CertificateAuthorityMode = CertificateAuthorityMode.AlwaysTrust;

				// Act
				// Fake first request by just injecting key into store
				CertificatePinner.KeyStore.PinForHost("google.com", new byte[] {0, 0, 0});
				// Assert
				Assert.DoesNotThrowAsync(()=>restClient.GetAsync(""));
			}
		}

		[Test]
		public void WhenAlwaysTrustCA_WhenPinSaysNoButCaSaysYes_ResultsInFailure()
		{
			// Arrange
			using (var restClient = GetClient("https://google.com"))
			{
				CertificatePinner.KeyStore = new InMemoryKeyStore();
				CertificatePinner.TrustOnFirstUse = false;
				CertificatePinner.CertificateAuthorityMode = CertificateAuthorityMode.TrustIfNotPinned;

				// Act
				// Fake first request by just injecting key into store
				CertificatePinner.KeyStore.PinForHost("google.com", new byte[] {0, 0, 0});
				// Assert
				Assert.ThrowsAsync<HttpRequestException>(()=>restClient.GetAsync(""));
			}
		}

		[Test]
		public void WhenAlwaysTrustCase_WhenNotPinnedButCaSaysYes_ResultsInSuccess()
		{
			// Arrange
			using (var restClient = GetClient("https://google.com"))
			{
				CertificatePinner.KeyStore = new InMemoryKeyStore();
				CertificatePinner.TrustOnFirstUse = false;
				CertificatePinner.CertificateAuthorityMode = CertificateAuthorityMode.TrustIfNotPinned;

				// Act + Assert
				Assert.DoesNotThrowAsync(()=>restClient.GetAsync(""));
			}
		}
	 }
}
