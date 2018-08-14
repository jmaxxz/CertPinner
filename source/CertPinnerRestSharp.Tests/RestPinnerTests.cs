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
			var instance = new RestPinner();
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
			var instance = new RestPinner(trustOnFirstuse: true,
				trustCertificateAuthorities: true,
				trustExpired: true);
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
			var restPinner = new RestPinner();

			// Act
			restPinner.EnablePinning(restClient);

			// Assert
			Assert.IsNotNull(restClient.RemoteCertificateValidationCallback);
		}
	 }
 }
