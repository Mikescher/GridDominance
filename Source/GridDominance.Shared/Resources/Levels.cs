using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Levelformat.Parser;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace GridDominance.Shared.Resources
{
	public static class Levels
	{
		public static LevelFile LEVEL_003;

		public static void LoadContent(ContentManager content)
		{
			LEVEL_003 = content.Load<LevelFile>("levels/lvl003");
		}
	}
}
