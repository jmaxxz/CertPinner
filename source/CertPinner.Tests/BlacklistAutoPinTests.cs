using CertPinner.AutoPinPolicies;
using NUnit.Framework;

namespace CertPinner
{
	[TestFixture]
	public class BlacklistAutoPinTests
	{
		[Test]
		public void CanPin_WhenNotOnList_ReturnsTrue()
		{
			// Arrange
			var instance = new BlacklistAutoPin();

			// Act
			var result = instance.CanPin("jmaxxz.com");

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void CanPin_WhenOnList_ReturnsFalse()
		{
			// Arrange
			var instance = new BlacklistAutoPin();
			instance.AddHost("jmaxxz.com");

			// Act
			var result = instance.CanPin("jmaxxz.com");
			var resultWithCaseChange = instance.CanPin("Jmaxxz.com");

			// Assert
			Assert.IsFalse(result);
			Assert.IsFalse(resultWithCaseChange, "List should be case insensitive");
		}
	}
}
