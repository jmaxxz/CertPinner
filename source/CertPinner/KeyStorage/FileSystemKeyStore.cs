using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace CertPinner.KeyStorage
{
	public class FileSystemKeyStore : IKeyStore
	{
		public TimeSpan AutoSaveInterval { get; set; } = TimeSpan.FromMinutes(1);
		private bool _changesPending = false;

		// This is not a long term solution, however, it will work for now.
		private InMemoryKeyStore _backingKeyStore = new InMemoryKeyStore();
		public FileSystemKeyStore(string path)
		{
			Path = Environment.ExpandEnvironmentVariables(path);
		}

		public string Path { get; }
		public bool MatchesExistingOrAddIfNew(string host, byte[] publicKey)
		{
			return _backingKeyStore.MatchesExistingOrAddIfNew(host, publicKey);
		}

		public bool MatchesExisting(string host, byte[] publicKey)
		{
			return _backingKeyStore.MatchesExisting(host, publicKey);
		}

		public bool IsPinned(string host)
		{
			return _backingKeyStore.IsPinned(host);
		}

		public void PinForHost(string host, byte[] publicKey)
		{
			_backingKeyStore.PinForHost(host, publicKey);
		}

		public async Task Save()
		{

		}

		public void Reload()
		{
			var serializer = new DataContractJsonSerializer(typeof(List<HostKeyPair>));
			List<HostKeyPair> update;
			using (var fileStream = File.OpenRead(Path))
			{
				update = serializer.ReadObject(fileStream) as List<HostKeyPair>;
			}

			foreach (var hostKeyPair in update ?? new List<HostKeyPair>())
			{
				this.PinForHost(hostKeyPair.Host, hostKeyPair.PublicKey);
			}
		}
	}
}
