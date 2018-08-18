namespace CertPinner.AutoPinPolicies
{
	public class NeverAutoPin : IAutomaticPinPolicy
	{
		public bool CanPin(string hostname)
		{
			return false;
		}
	}
}
