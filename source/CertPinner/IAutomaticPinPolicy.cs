namespace CertPinner
{
	public interface IAutomaticPinPolicy
	{
		bool CanPin(string hostname);
	}
}
