using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace CertPinner
{
	[TestFixture]
	[Parallelizable(ParallelScope.None)]
	public class PinnerTests
	{

		[OneTimeSetUp]
		public void OnTimeSetup()
		{
			CertificatePinner.Enable();
		}

		[SetUp]
		public void ResetToDefaults()
		{
			CertificatePinner.TrustCertificateAuthorities = false;
			CertificatePinner.TrustExpired = false;
			CertificatePinner.TrustOnFirstUse = false;
			CertificatePinner.KeyStore = new InMemoryKeyStore();
		}

		[Test]
		public void Constructor_AllowsOverrideOfDefaults()
		{
			// Arrange
			// Act
			CertificatePinner.TrustCertificateAuthorities = true;
			CertificatePinner.TrustExpired = true;
			CertificatePinner.TrustOnFirstUse = true;

			// Assert
			Assert.IsTrue(CertificatePinner.TrustCertificateAuthorities);
			Assert.IsTrue(CertificatePinner.TrustExpired);
			Assert.IsTrue(CertificatePinner.TrustOnFirstUse);
		}


		[Category("Integration")]
		[TestCase("https://expired.badssl.com")] // Known bad cert
		[TestCase("https://google.com")] // Known good cert
		public async Task OnRequest_WhenDontTrustOnFirstUse_ResultsInError(string url)
		{
			// Arrange
			var restClient = new RestClient(url);
			CertificatePinner.TrustOnFirstUse = false;

			// Act
			var result = await restClient.ExecuteGetTaskAsync(new RestRequest());

			// Assert
			Assert.AreEqual(ResponseStatus.Error, result.ResponseStatus);
		}

		[Test]
		public async Task WhenTrustOnFirstUse_FirstRequest_ResultsInSuccess()
		{
			// Arrange
			var restClient = new RestClient("https://google.com");
			CertificatePinner.TrustOnFirstUse = true;

			// Act
			var result = await restClient.ExecuteGetTaskAsync(new RestRequest());

			// Assert
			Assert.AreEqual(ResponseStatus.Completed, result.ResponseStatus);
		}

		[Test]
		public async Task WhenTrustOnFirstUse_AfterPKChanges_ResultsInFailure()
		{
			// Arrange
			var restClient = new RestClient("https://google.com");
			CertificatePinner.KeyStore = new InMemoryKeyStore();
			CertificatePinner.TrustOnFirstUse = true;

			// Act
			// Fake first request by just injecting key into store
			CertificatePinner.KeyStore.MatchesExistingOrIsNew("google.com", new byte[] {0, 0, 0});
			var result = await restClient.ExecuteGetTaskAsync(new RestRequest());

			// Assert
			Assert.AreEqual(ResponseStatus.Error, result.ResponseStatus);
		}

		[Test]
		public async Task WhenDontTrustOnFirstUse_WhenPublicKeyInStore_ResultsInSuccess()
		{
			// Arrange
			var restClient = new RestClient("https://google.com");
			CertificatePinner.TrustOnFirstUse = true;
			CertificatePinner.Enable();
			await restClient.ExecuteGetTaskAsync(new RestRequest());

			// Act
			// Fake first request by just injecting key into store
			CertificatePinner.TrustOnFirstUse = false;
			var result = await restClient.ExecuteGetTaskAsync(new RestRequest());

			// Assert
			Assert.AreEqual(ResponseStatus.Completed, result.ResponseStatus);
		}
	 }
}
