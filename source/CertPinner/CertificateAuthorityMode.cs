namespace CertPinner
{
	public enum CertificateAuthorityMode
	{
		/// <summary>
		/// Ignore CA signatures entirely. Rely solely on pinning to determine if a public key belongs to a host.
		/// </summary>
		Distrust = 0,

		/// <summary>
		/// Trust CA signatures only for hosts which do not have a public key pinned.
		/// </summary>
		TrustIfNotPinned,

		/// <summary>
		/// Trust CA even if public key does not match the pinned value.
		/// </summary>
		AlwaysTrust
	}
}
