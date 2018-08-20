using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace CertPinner.KeyStorage
{
	[TestFixture]
	public class FileSystemKeyStoreTests
	{
		[Test]
		public void Constructor_Path_ExpandsEnvironmentVariables()
		{
			// Arrange
			var tempDir = Environment.OSVersion.Platform == PlatformID.Win32Windows ? "%temp%" : "$TMPDIR";
			// Act

			var instance = new FileSystemKeyStore(tempDir+@"\foo\pins.json");

			// Assert
			Assert.AreEqual(Environment.ExpandEnvironmentVariables(tempDir)+@"\foo\pins.json", instance.Path);
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

		[Test]
		public void Save_WritesConfigurationToDist()
		{
			// Arrange
			var path = Path.GetTempFileName() + ".json";
			var pk = new byte[] {0, 1, 2, 3, 4};
			var instance = new FileSystemKeyStore(path);
			instance.PinForHost("jmaxxz.com", pk);

			// Act
			instance.Save();

			// Assert
			var secondInstance = new FileSystemKeyStore(path);
			secondInstance.Reload();
			Assert.IsTrue(secondInstance.MatchesExisting("jmaxxz.com", pk));
		}

		[Test]
		public async Task AutoSave_OnInterval_CommitsToDisk()
		{
			// Arrange
			var path = Path.GetTempFileName() + ".json";
			var pk = new byte[] {0, 1, 2, 3, 4};
			var instance = new FileSystemKeyStore(path);
			instance.PinForHost("jmaxxz.com", pk);

			// Act
			instance.AutoSaveInterval = TimeSpan.FromSeconds(1);
			await Task.Delay(5000);

			// Assert
			var secondInstance = new FileSystemKeyStore(path);
			secondInstance.Reload();
			Assert.IsTrue(secondInstance.MatchesExisting("jmaxxz.com", pk));
		}
	}
}
