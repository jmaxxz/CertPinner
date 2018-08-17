using System.Collections.Concurrent;

namespace CertPinner
{
	public class WhitelistAutoPin : IAutomaticPinPolicy
	{
		private readonly ConcurrentDictionary<string, bool> _whiteList = new ConcurrentDictionary<string, bool>();
		public bool CanPin(string hostname)
		{
			return _whiteList.ContainsKey(hostname.ToUpperInvariant());
		}

		public void AddToWhitelist(string anything)
		{
			_whiteList[anything.ToUpperInvariant()] = true;
		}
	}
}
