using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.WorldMapScreen
{
	public class GDWorldMapEntityManager : EntityManager
	{
		public GDWorldMapEntityManager(GameScreen screen) : base(screen)
		{
		}

		public override void DrawOuterDebug()
		{
			// NOP
		}

		protected override RectangleF RecalculateBoundingBox()
		{
			return new RectangleF(0, 0, GDWorldMapScreen.VIEW_WIDTH, GDWorldMapScreen.VIEW_HEIGHT);
		}

		protected override void OnAfterUpdate(GameTime gameTime, InputState state)
		{
			// NOP
		}

		protected override void OnBeforeUpdate(GameTime gameTime, InputState state)
		{
			// NOP
		}
	}
}
