using NUnit.Framework;

namespace CertPinner
{
	[TestFixture]
	public class NeverAutoPinTests
	{
		[Test]
		public void CanPin_ReturnsFalse()
		{
			// Arrange
			var instance = new NeverAutoPin();

			// Act
			var result = instance.CanPin("anything");

			// Assert
			Assert.IsFalse(result);
		}
	}
}
