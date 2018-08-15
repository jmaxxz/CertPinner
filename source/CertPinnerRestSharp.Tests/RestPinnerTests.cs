using System;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;

namespace CertPinnerRestSharp.Tests
{
	[TestFixture]
	public class RestPinnerTests
	{

		[Test]
		public void Constructor_DefaultToMostRestricted()
		{
			// Arrange
			// Act
			var instance = new RestPinner(new InMemoryKeyStore());
			// Assert
			Assert.IsFalse(instance.TrustCertificateAuthorities, "Trust no one by default");
			Assert.IsFalse(instance.TrustExpired, "Enforce expiration dates by default");
			Assert.IsFalse(instance.TrustOnFirstUse, "Don't auto trust by default");
		}

		[Test]
		public void Constructor_AllowsOverrideOfDefaults()
		{
			// Arrange
			// Act
			var instance = new RestPinner(new InMemoryKeyStore())
			{
				TrustCertificateAuthorities = true,
				TrustExpired = true,
				TrustOnFirstUse = true,
			};
			// Assert
			Assert.IsTrue(instance.TrustCertificateAuthorities);
			Assert.IsTrue(instance.TrustExpired);
			Assert.IsTrue(instance.TrustOnFirstUse);
		}

		[Test]
		public void EnablePinning_AddsCertificateValidatorToRestClient()
		{
			// Arrange
			var restClient = new RestClient();
			var restPinner = new RestPinner(new InMemoryKeyStore());

			// Act
			restPinner.EnablePinning(restClient);

			// Assert
			Assert.IsNotNull(restClient.RemoteCertificateValidationCallback);
		}

		[Category("Integration")]
		[TestCase("https://expired.badssl.com")] // Known bad cert
		[TestCase("https://google.com")] // Known good cert
		public async Task OnRequest_WhenDontTrustOnFirstUse_ResultsInError(string url)
		{
			// Arrange
			var restClient = new RestClient(url);
			var restPinner = new RestPinner(new InMemoryKeyStore());
			restPinner.EnablePinning(restClient);
			// Act
			var result = await restClient.ExecuteGetTaskAsync(new RestRequest());
			// Assert
			Assert.AreEqual(ResponseStatus.Error, result.ResponseStatus);
			StringAssert.Contains("TrustFailure", result.ErrorMessage);
		}

		[Test]
		public async Task OnRequest_WhenTrustOnFirstUse_ResultsInSuccess()
		{
			// Arrange
			var restClient = new RestClient("https://google.com");
			var restPinner = new RestPinner(new InMemoryKeyStore())
			{
				TrustOnFirstUse = true
			};
			restPinner.EnablePinning(restClient);

			// Act
			var result = await restClient.ExecuteGetTaskAsync(new RestRequest());
			// Assert
			Assert.AreEqual(ResponseStatus.Completed, result.ResponseStatus);
		}
	 }
}
