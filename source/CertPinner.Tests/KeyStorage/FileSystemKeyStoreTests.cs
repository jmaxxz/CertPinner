using System;
using NUnit.Framework;

namespace CertPinner.KeyStorage
{
	[TestFixture]
	public class FileSystemKeyStoreTests
	{
		[Test]
		public void Constructor_Path_ExpandsEnvironmentVariables()
		{
			// Arrange
			// Act
			var instance = new FileSystemKeyStore(@"%temp%\foo\pins.json");
			// Assert
			Assert.AreEqual(Environment.GetEnvironmentVariable(@"temp")+@"\foo\pins.json", instance.Path);
		}
	}
}
