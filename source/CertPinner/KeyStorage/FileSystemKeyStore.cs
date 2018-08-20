using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Timers;

namespace CertPinner.KeyStorage
{
	public class FileSystemKeyStore : IKeyStore
	{
		private readonly Timer _autoSaveTimer;

		/// <summary>
		/// How often key store should be saved. A timespan of less than or equal to 0 will
		/// result in autosave being disabled. The minimum autosave interval allowed is 500ms.
		/// </summary>
		public TimeSpan AutoSaveInterval
		{
			get => !_autoSaveTimer.Enabled ? TimeSpan.Zero : TimeSpan.FromMilliseconds(_autoSaveTimer.Interval);
			set
			{
				_autoSaveTimer.Enabled = value > TimeSpan.Zero;
				_autoSaveTimer.Interval = Math.Max(500, value.TotalMilliseconds);
			}
		}

		private bool _changesPending;

		// This is not a long term solution, however, it will work for now.
		private readonly InMemoryKeyStore _backingKeyStore = new InMemoryKeyStore();
		public FileSystemKeyStore(string path)
		{
			Path = Environment.ExpandEnvironmentVariables(path);
			_autoSaveTimer = new Timer
			{
				Enabled = false,
				Interval = 500,
				AutoReset = true,
			};
			_autoSaveTimer.Elapsed += (sender, args) =>
			{
				if (_changesPending)
					Save();
			};
		}

		public string Path { get; }
		public bool MatchesExistingOrAddIfNew(string host, byte[] publicKey)
		{
			_changesPending = true;
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
			_changesPending = true;
			_backingKeyStore.PinForHost(host, publicKey);
		}

		public void Save()
		{
			// There is a chance save fails and we end up not writing the file.
			// the chances of this happening are fairly low, for the initial version
			// we shall assume it always saves successfully.
			_changesPending = false;
			var serializer = new DataContractJsonSerializer(typeof(List<HostKeyPair>));
			using (var fileStream = File.OpenWrite(Path))
			{
				serializer.WriteObject(fileStream, _backingKeyStore.ToArray());
			}
		}

		public void Reload()
		{
			var serializer = new DataContractJsonSerializer(typeof(HostKeyPair[]));
			HostKeyPair[] update;
			using (var fileStream = File.OpenRead(Path))
			{
				update = serializer.ReadObject(fileStream) as HostKeyPair[];
			}

			// The current strategy only adds pins, it never removes them
			// this is not a viable long term strategy, but will work for most
			// use cases. Should be changed.
			foreach (var hostKeyPair in update ?? new HostKeyPair[0])
			{
				PinForHost(hostKeyPair.Host, hostKeyPair.PublicKey);
			}
		}
	}
}
