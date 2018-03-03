namespace GridDominance.Shared.Network.Backend
{
	public enum UploadResult
	{
		InternalError,
		NoConnection,

		FileTooBig,
		WrongUserID,
		LevelIDNotFound,
		AlreadyUploaded,
		InvalidName,
		DuplicateName,

		Success,
	}
}
