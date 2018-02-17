using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.Leveleditor.HUD.Elements
{
	class LevelEditorAttrPanel : HUDContainer
	{
		private const float HEIGHT = GDConstants.TILE_WIDTH * 4;

		public override int Depth => 0;

		public LevelEditorScreen GDScreen => (LevelEditorScreen) HUD.Screen;

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			FlatRenderHelper.DrawEdgeAlignedBlurPanel_Opaque(sbatch, bounds, FlatColors.BackgroundHUD2, FlatAlign5.BOTTOM, 16);
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate)   => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		public override void OnInitialize()
		{
			RelativePosition = new FPoint(GDConstants.TILE_WIDTH/2f, 0);
			Size = new FSize(HUD.Width - 5 * GDConstants.TILE_WIDTH, HEIGHT);
			Alignment = HUDAlignment.BOTTOMLEFT;
			
		}
		
		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}
	}
}
