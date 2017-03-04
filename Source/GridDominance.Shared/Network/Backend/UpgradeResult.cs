using System;
using System.Collections.Generic;
using System.Text;

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
