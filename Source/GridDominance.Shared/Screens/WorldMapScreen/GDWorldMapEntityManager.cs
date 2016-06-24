using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;
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

		protected override FRectangle RecalculateBoundingBox()
		{
			return Owner.VAdapter.VirtualTotalBoundingBox;
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
