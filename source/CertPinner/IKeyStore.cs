using System.Runtime.CompilerServices;

namespace CertPinner
{
	/// <summary>
	/// A mapping of hosts (domains) to their respective
	/// public keys.
	/// </summary>
	public interface IKeyStore
	{
		/// <summary>
		/// Checks if passed public key matches value pinned
		/// for passed host. If no value has yet been pinned
		/// than the passed key will be accepted and pinned.
		/// </summary>
		/// <param name="host">The host which this key is for (e.g. jmaxxz.com)</param>
		/// <param name="publicKey">The public key presented by the host</param>
		/// <returns>True if key matches existing pin, or will be used as the pin.</returns>
		bool MatchesExistingOrAddIfNew(string host, byte[] publicKey);
		/// <summary>
		/// Checks if passed public key matches value pinned
		/// for passed host. If no value is pinned will return
		/// false.
		/// </summary>
		/// <param name="host">The host which this key is for (e.g. jmaxxz.com)</param>
		/// <param name="publicKey">The public key presented by the host</param>
		/// <returns>True if key matches existing pin.</returns>
		bool MatchesExisting(string host, byte[] publicKey);

		/// <summary>
		/// Checks to see if a key is pinned for this host.
		/// NOTE: Key stores are thread safe, but maybe updated
		/// between calls.
		/// </summary>
		/// <param name="host">The host caller wants to know if is pinned on keychain.</param>
		/// <returns>True if public key is pinned for host</returns>
		bool IsPinned(string host);


		/// <summary>
		/// Pins a public key for a host. If key has already been pinned
		/// for host will override existing pin.
		/// </summary>
		/// <param name="host">The host which this key is for (e.g. jmaxxz.com)</param>
		/// <param name="publicKey">The public key of the host</param>
		void PinForHost(string host, byte[] publicKey);
	}
}
