using System;
using System.Threading.Tasks;

namespace CertPinner.KeyStorage
{
	public class FileSystemKeyStore : IKeyStore
	{
		public TimeSpan AutoSaveInterval { get; set; } = TimeSpan.FromMinutes(1);
		private bool _changesPending = false;

		public FileSystemKeyStore(string path)
		{
			Path = Environment.ExpandEnvironmentVariables(path);
		}

		public string Path { get; }
		public bool MatchesExistingOrAddIfNew(string host, byte[] publicKey)
		{
			throw new System.NotImplementedException();
		}

		public bool MatchesExisting(string host, byte[] publicKey)
		{
			throw new System.NotImplementedException();
		}

		public bool IsPinned(string host)
		{
			throw new System.NotImplementedException();
		}

		public void PinForHost(string host, byte[] publicKey)
		{
			throw new System.NotImplementedException();
		}

		public async Task Save()
		{

		}

		public async Task Reload()
		{

		}
	}
}
