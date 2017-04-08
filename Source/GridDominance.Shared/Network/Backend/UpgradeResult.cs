namespace GridDominance.Shared.Network.Backend
{
	public enum UpgradeResult
	{
		Success,

		UsernameTaken,
		AlreadyFullAcc,

		AuthError,

		InternalError,
		NoConnection,
	}
}
