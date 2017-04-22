using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	public class OverworldTutorialNode : GameEntity
	{
		public const float SIZE = 3 * GDConstants.TILE_WIDTH;
		public const float INNERSIZE = 2 * GDConstants.TILE_WIDTH;
		public const float ICONSIZE = 1.5f * GDConstants.TILE_WIDTH;
		public const float ICONSIZESWIGGLE = 0.125f * GDConstants.TILE_WIDTH;

		public const float COLLAPSE_TIME = 5f;

		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; } = new FSize(SIZE, SIZE);
		public override Color DebugIdentColor { get; } = Color.Blue;

		private readonly string _description;
		private readonly GameEntityMouseArea clickArea;

		private readonly Dictionary<FractionDifficulty, float> solvedPerc = new Dictionary<FractionDifficulty, float>();

		public float AlphaOverride = 1f;

		public OverworldTutorialNode(GDOverworldScreen scrn, Vector2 pos, string text) : base(scrn, GDConstants.ORDER_WORLD_NODE)
		{
			_description = text;
			Position = pos;

			clickArea = AddClickMouseArea(FRectangle.CreateByCenter(Vector2.Zero, new FSize(SIZE, SIZE)), OnClick);
		}

		public override void OnInitialize(EntityManager manager)
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}

		private void OnClick(GameEntityMouseArea area, SAMTime gameTime, InputState istate)
		{
			var ownr = ((GDOverworldScreen) Owner);
			if (ownr.IsTransitioning) return;
			ownr.IsTransitioning = true;

			ownr.AddAgent(new TransitionZoomInToTutorialAgent(ownr, this));
			
			MainGame.Inst.GDSound.PlayEffectZoomIn();
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			var outerBounds = FRectangle.CreateByCenter(Position, DrawingBoundingBox);
			var innerBounds = FRectangle.CreateByCenter(Position, new FSize(INNERSIZE, INNERSIZE));

			if (! MainGame.Inst.Profile.GetLevelData(Levels.LEVEL_TUTORIAL.UniqueID).HasAnyCompleted())
			{
				var iconBounds = FRectangle.CreateByCenter(Position, new FSize(ICONSIZE + ICONSIZESWIGGLE * FloatMath.Sin(Lifetime), ICONSIZE + ICONSIZESWIGGLE * FloatMath.Sin(Lifetime)));

				FlatRenderHelper.DrawRoundedBlurPanel(sbatch, outerBounds, clickArea.IsMouseDown() ? FlatColors.ButtonPressedHUD : FlatColors.Asbestos, 0.5f * GDConstants.TILE_WIDTH);
				SimpleRenderHelper.DrawRoundedRectOutline(sbatch, outerBounds.AsInflated(1f, 1f), FlatColors.MidnightBlue, 8, 2f, 0.5f * GDConstants.TILE_WIDTH);

				sbatch.FillRectangle(innerBounds, FlatColors.Background);

				sbatch.DrawStretched(Textures.TexIconTutorial, iconBounds, Color.White * AlphaOverride);

				FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH, _description, FlatColors.TextHUD, Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH));
			}
			else
			{
				var iconBounds = FRectangle.CreateByCenter(Position, new FSize(ICONSIZE, ICONSIZE));

				FlatRenderHelper.DrawRoundedBlurPanel(sbatch, outerBounds, clickArea.IsMouseDown() ? FlatColors.ButtonPressedHUD : FlatColors.Asbestos, 0.5f * GDConstants.TILE_WIDTH);
				SimpleRenderHelper.DrawRoundedRectOutline(sbatch, outerBounds.AsInflated(1f, 1f), FlatColors.MidnightBlue, 8, 2f, 0.5f * GDConstants.TILE_WIDTH);

				sbatch.FillRectangle(innerBounds, FlatColors.Background);

				sbatch.DrawStretched(Textures.TexIconTutorial, iconBounds, FlatColors.Emerald * AlphaOverride);

				FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH, _description, FlatColors.TextHUD, Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH));

			}
		}
	}
}
