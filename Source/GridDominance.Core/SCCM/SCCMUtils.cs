using System;
using System.Collections.Generic;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.SCCM
{
	public static class SCCMUtils
	{
		public static List<SCCMLevelData> ListUserLevels()
		{
			var allfiles = FileHelper.Inst.ListData();

			var result = new List<SCCMLevelData>();

			foreach (var file in allfiles)
			{
				if (!file.StartsWith("CUSTOMLEVELDATA_")) continue;

				string content = null;
				try
				{
					var dat = new SCCMLevelData();
					content = FileHelper.Inst.ReadDataOrNull(file);
					dat.DeserializeFromString(content);

					result.Add(dat);
				}
				catch (Exception e)
				{
					SAMLog.Error("SCCMU::READFAIL", "Could not read CustomLevelData", $"Exception: {e}\n\n\nFile: {file}\n\n\nContent:{content}");
				}
			}

			return result;
		}

	}
}
