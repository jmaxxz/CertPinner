using NUnit.Framework;

namespace CertPinner
{
	[TestFixture]
	public class WhitelistAutoPinTests
	{
		[Test]
		public void CanPin_WhenNotOnList_ReturnsFalse()
		{
			// Arrange
			var instance = new WhitelistAutoPin();

			// Act
			var result = instance.CanPin("anything");

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void CanPin_WhenOnList_ReturnsTrue()
		{
			// Arrange
			var instance = new WhitelistAutoPin();
			instance.AddToWhitelist("anything");

			// Act
			var result = instance.CanPin("anything");
			var resultWithCaseChange = instance.CanPin("AnyThing");

			// Assert
			Assert.IsTrue(result);
			Assert.IsTrue(resultWithCaseChange, "List should be case insensitive");
		}
	}
}
