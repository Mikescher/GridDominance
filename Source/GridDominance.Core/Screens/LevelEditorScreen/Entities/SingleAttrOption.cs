using System;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;

namespace GridDominance.Shared.Screens.LevelEditorScreen.Entities
{
	public sealed class SingleAttrOption
	{
		public Func<TextureRegion2D> Icon = () => null;
		public float IconScale = 0.75f;
		public Func<Color> IconColor = () => Color.White;

		public int Description = -1;

		public Func<string> Text = () => null;
		public Func<Color> TextColor = () => FlatColors.TextHUD;

		public Action Action = () => { };
	}
}