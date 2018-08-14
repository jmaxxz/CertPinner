using NUnit.Framework;
using RestSharp;

namespace CertPinnerRestSharp.Tests
{
	[TestFixture]
	public class RestPinnerTests
	{
		[Test]
		public void AddsCertificateValidatorToRestClient()
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
