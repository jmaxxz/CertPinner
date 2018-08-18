namespace CertPinner.AutoPinPolicies
{
	public interface IAutomaticPinPolicy
	{
		bool CanPin(string hostname);
	}
}
