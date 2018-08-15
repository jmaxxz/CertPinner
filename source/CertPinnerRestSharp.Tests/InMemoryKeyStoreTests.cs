using System;
using NUnit.Framework;

namespace CertPinnerRestSharp
{
	[TestFixture]
	public class InMemoryKeyStoreTests
	{
		[Test]
		public void MatchesExistingPinOrIsNew_OnNew_ReturnsTrue()
		{
			// Arrange
			var instance = new InMemoryKeyStore();
			// Act
			var result = instance.MatchesExistingPinOrIsNew("foo.com", new byte[] {0, 1, 2, 3});
			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void MatchesExistingPinOrIsNew_IfPublicKeyMatchesExisting_ReturnsTrue()
		{
			// Arrange
			var instance = new InMemoryKeyStore();
			instance.MatchesExistingPinOrIsNew("foo.com", new byte[] {0, 1, 2, 3});
			// Act
			var result = instance.MatchesExistingPinOrIsNew("foo.com", new byte[] {0, 1, 2, 3});
			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void MatchesExistingPinOrIsNew_IfPublicKeyDoesNotMatchExisting_ReturnsFalse()
		{
			// Arrange
			var instance = new InMemoryKeyStore();
			instance.MatchesExistingPinOrIsNew("foo.com", new byte[] {0, 1, 2, 3});
			// Act
			var result = instance.MatchesExistingPinOrIsNew("foo.com", new byte[] {0, 1, 2, 2});
			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void MatchesExistingPinOrIsNew_IfCaseingChangeOnHost_ItDoesNotMatter()
		{
			// Arrange
			var instance = new InMemoryKeyStore();
			instance.MatchesExistingPinOrIsNew("foo.com", new byte[] {0, 1, 2, 3});
			// Act
			var result = instance.MatchesExistingPinOrIsNew("fOo.com", new byte[] {0, 1, 2, 2});
			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void MatchesExisting_IfNew_ReturnsFalse()
		{
			// Arrange
			var instance = new InMemoryKeyStore();
			// Act
			var result = instance.MatchesExisting("foo.com", new byte[] {1, 2, 3});
			var result2 = instance.MatchesExisting("foo.com", new byte[] {1, 2, 3});
			// Assert
			Assert.IsFalse(result, "First request should return false");
			Assert.IsFalse(result2, "Subsequent requests should return false");
		}
	}
}
