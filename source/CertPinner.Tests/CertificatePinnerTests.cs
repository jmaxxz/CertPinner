using System;
using System.Net.Http;
using System.Threading.Tasks;
using CertPinner.AutoPinPolicies;
using CertPinner.KeyStorage;
using NUnit.Framework;

// ReSharper disable AccessToDisposedClosure
// Disabled because AssertThrowsAsync waits so resharper misidentifies this
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
			CertificatePinner.AutomaticPinPolicy = null;
			CertificatePinner.KeyStore = new InMemoryKeyStore();
		}

		[Test]
		public void Constructor_AllowsOverrideOfDefaults()
		{
			// Arrange
			// Act
			CertificatePinner.CertificateAuthorityMode = CertificateAuthorityMode.AlwaysTrust;
			CertificatePinner.AutomaticPinPolicy = new AlwaysAutoPin();

			// Assert
			Assert.AreEqual(CertificatePinner.CertificateAuthorityMode, CertificateAuthorityMode.AlwaysTrust);
			Assert.IsInstanceOf<AlwaysAutoPin>(CertificatePinner.AutomaticPinPolicy);
		}


		[Category("Integration")]
		[TestCase("https://expired.badssl.com")] // Known bad cert
		[TestCase("https://google.com")] // Known good cert
		public void OnRequest_WhenDontTrustOnFirstUse_ResultsInError(string url)
		{
			// Arrange
			var restClient = GetClient(url);
			CertificatePinner.AutomaticPinPolicy = new NeverAutoPin();

			// Act
			// Assert
			Assert.ThrowsAsync<HttpRequestException>(()=>restClient.GetAsync(""));
		}

		[Category("Integration")]
		[Test]
		public void WhenTrustOnFirstUse_FirstRequest_ResultsInSuccess()
		{
			// Arrange
			using (var restClient = GetClient("https://google.com"))
			{
				CertificatePinner.AutomaticPinPolicy = new AlwaysAutoPin();

				// Act
				// Assert
				Assert.DoesNotThrowAsync(()=>restClient.GetAsync(""));
			}
		}


		[Category("Integration")]
		[Test]
		public void WhenTrustOnFirstUse_AfterPKChanges_ResultsInFailure()
		{
			// Arrange
			using (var restClient = GetClient("https://google.com"))
			{
				CertificatePinner.KeyStore = new InMemoryKeyStore();
				CertificatePinner.AutomaticPinPolicy = new AlwaysAutoPin();

				// Act
				// Fake first request by just injecting key into store
				CertificatePinner.KeyStore.PinForHost("google.com", new byte[] {0, 0, 0});
				// Assert
				Assert.ThrowsAsync<HttpRequestException>(()=>restClient.GetAsync(""));
			}
		}

		[Category("Integration")]
		[Test]
		public async Task WhenDontTrustOnFirstUse_WhenPublicKeyInStore_ResultsInSuccess()
		{
			// Arrange
			using (var restClient = GetClient("https://google.com"))
			{
				CertificatePinner.AutomaticPinPolicy = new AlwaysAutoPin();
				await restClient.GetAsync("");

				// Act
				// Fake first request by just injecting key into store
				CertificatePinner.AutomaticPinPolicy = null;
				// Assert
				Assert.DoesNotThrowAsync(()=>restClient.GetAsync(""));
			}
		}

		[Category("Integration")]
		[TestCase(false)]
		[TestCase(true)]
		public void WhenAlwaysTrustCAs_WhenPinSaysNoButCaSaysYes_ResultsInSuccess(bool trustOnFirstUse)
		{
			// Arrange
			using (var restClient = GetClient("https://google.com"))
			{
				CertificatePinner.KeyStore = new InMemoryKeyStore();
				CertificatePinner.AutomaticPinPolicy = new AlwaysAutoPin();
				CertificatePinner.CertificateAuthorityMode = CertificateAuthorityMode.AlwaysTrust;

				// Act
				// Fake first request by just injecting key into store
				CertificatePinner.KeyStore.PinForHost("google.com", new byte[] {0, 0, 0});
				// Assert
				Assert.DoesNotThrowAsync(()=>restClient.GetAsync(""));
			}
		}

		[Category("Integration")]
		[Test]
		public void WhenAlwaysTrustCA_WhenPinSaysNoButCaSaysYes_ResultsInFailure()
		{
			// Arrange
			using (var restClient = GetClient("https://google.com"))
			{
				CertificatePinner.KeyStore = new InMemoryKeyStore();
				CertificatePinner.AutomaticPinPolicy = new AlwaysAutoPin();
				CertificatePinner.CertificateAuthorityMode = CertificateAuthorityMode.TrustIfNotPinned;

				// Act
				// Fake first request by just injecting key into store
				CertificatePinner.KeyStore.PinForHost("google.com", new byte[] {0, 0, 0});
				// Assert
				Assert.ThrowsAsync<HttpRequestException>(()=>restClient.GetAsync(""));
			}
		}

		[Category("Integration")]
		[Test]
		public void WhenAlwaysTrustCase_WhenNotPinnedButCaSaysYes_ResultsInSuccess()
		{
			// Arrange
			using (var restClient = GetClient("https://google.com"))
			{
				CertificatePinner.KeyStore = new InMemoryKeyStore();
				CertificatePinner.AutomaticPinPolicy = null;
				CertificatePinner.CertificateAuthorityMode = CertificateAuthorityMode.TrustIfNotPinned;

				// Act + Assert
				Assert.DoesNotThrowAsync(()=>restClient.GetAsync(""));
			}
		}
	 }
}
