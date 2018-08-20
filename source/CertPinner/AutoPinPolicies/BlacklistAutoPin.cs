namespace CertPinner.AutoPinPolicies
{
	public class BlacklistAutoPin : IAutomaticPinPolicy
	{
		// A blacklist is nothing more than an inverted whitelist
		// define our blacklist with a whitelist in order to reduce code.
		private readonly WhitelistAutoPin _backingWhitelist = new WhitelistAutoPin();
		public bool CanPin(string hostname)
		{
			return !_backingWhitelist.CanPin(hostname);
		}

		public void AddHost(string host)
		{
			_backingWhitelist.AddHost(host);
		}
	}
}
