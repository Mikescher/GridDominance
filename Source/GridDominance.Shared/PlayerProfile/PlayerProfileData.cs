using System.Collections.Generic;

namespace GridDominance.Shared.PlayerProfile
{
	//TODO Only temp - redo proper, guids and security and shit
	public class PlayerProfileData
	{
		private readonly Dictionary<string, PlayerProfileLevelData> levelData = new Dictionary<string, PlayerProfileLevelData>(); 

		public PlayerProfileData()
		{
			// NOP
		}

		public PlayerProfileLevelData GetLevelData(string levelid)
		{
			if (!levelData.ContainsKey(levelid))
				levelData[levelid] = new PlayerProfileLevelData();

			return levelData[levelid];
		}
	}
}
