using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

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

		[Test]
		public void Reload_ReadsConfigurationFromDisk()
		{
			// Arrange
			var path = Path.GetTempFileName() + ".json";
			var existing = new List<HostKeyPair>()
			{
				new HostKeyPair { Host = "example0", PublicKey = new byte[] {0, 0, 0}},
				new HostKeyPair { Host = "example1", PublicKey = new byte[] {1, 1, 1}},
				new HostKeyPair { Host = "example2", PublicKey = new byte[] {2, 2, 2}},
			};

			var serializer = new DataContractJsonSerializer(typeof(List<HostKeyPair>));
			using (var fileStream = File.OpenWrite(path))
			{
				serializer.WriteObject(fileStream, existing);
			}

			var instance = new FileSystemKeyStore(path);

			// Act
			instance.Reload();

			// Assert
			Assert.IsTrue(instance.MatchesExisting(existing[0].Host, existing[0].PublicKey));
			Assert.IsTrue(instance.MatchesExisting(existing[1].Host, existing[1].PublicKey));
			Assert.IsTrue(instance.MatchesExisting(existing[2].Host, existing[2].PublicKey));
		}
	}
}
