namespace CertPinner.AutoPinPolicies
{
	public class AlwaysAutoPin : IAutomaticPinPolicy
	{
		public bool CanPin(string hostname)
		{
			return true;
		}
	}
}
