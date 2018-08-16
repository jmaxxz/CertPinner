namespace CertPinner
{
	public class NeverAutoPin : IAutomaticPinPolicy
	{
		public bool CanPin(string hostname)
		{
			return false;
		}
	}
}
