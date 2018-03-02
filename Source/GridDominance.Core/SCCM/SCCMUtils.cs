using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GridDominance.Levelfileformat.Blueprint;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.SCCM
{
	public static class SCCMUtils
	{
		public static List<SCCMLevelData> ListUserLevelsUnfinished()
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
					SAMLog.Error("SCCMU::READFAIL_LULU", "Could not read CustomLevelData", $"Exception: {e}\n\n\nFile: {file}\n\n\nContent:{content}");
				}
			}

			return result.OrderByDescending(r => r.LastChanged).ToList();
		}

		public static List<Tuple<string, LevelBlueprint, string>> ListUserLevelsFinished()
		{
			var allfiles = FileHelper.Inst.ListData();

			var result = new List<Tuple<string, LevelBlueprint, string>>();

			foreach (var file in allfiles)
			{
				if (!file.StartsWith("UPLOADEDLEVELDATA_")) continue;

				byte[] content = null;
				try
				{
					var dat = new LevelBlueprint();
					content = FileHelper.Inst.ReadBinDataOrNull(file);
					dat.BinaryDeserialize(new BinaryReader(new MemoryStream(content)));

					var hash = ByteUtils.ByteToHexBitFiddle(MainGame.Inst.Bridge.DoSHA256(content));

					result.Add(Tuple.Create(file, dat, hash));
				}
				catch (Exception e)
				{
					SAMLog.Error("SCCMU::READFAIL_LULF", "Could not read CustomLevelData", $"Exception: {e}\n\n\nFile: {file}\n\n\nContent:{ByteUtils.ByteToHexBitFiddle(content)}");
				}
			}

			return result;
		}

		public static void UpgradeLevel(SCCMLevelData oldData, byte[] newData)
		{
			FileHelper.Inst.DeleteDataIfExist(oldData.Filename);
			FileHelper.Inst.WriteBinData(oldData.FilenameUploaded, newData);
		}

		internal static void DeleteUserLevelFinished(string filename)
		{
			FileHelper.Inst.DeleteDataIfExist(filename);
		}
	}
}
