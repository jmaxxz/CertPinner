using System.Collections.Concurrent;

namespace CertPinner.AutoPinPolicies
{
	public class WhitelistAutoPin : IAutomaticPinPolicy
	{
		private readonly ConcurrentDictionary<string, bool> _whiteList = new ConcurrentDictionary<string, bool>();
		public bool CanPin(string hostname)
		{
			return _whiteList.ContainsKey(hostname.ToUpperInvariant());
		}

		public void AddHost(string anything)
		{
			_whiteList[anything.ToUpperInvariant()] = true;
		}
	}
}
