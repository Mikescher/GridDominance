using Microsoft.Xna.Framework;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.Common.HUD.Multiplayer;
using GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations;
using GridDominance.Shared.Screens.OverworldScreen.HUD;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;

// ReSharper disable HeuristicUnreachableCode
#pragma warning disable 162
namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	public class OverworldNode_MP : OverworldNode
	{
		public readonly float[] VertexRotations =
		{
			FloatMath.RAD_POS_315,
			FloatMath.RAD_POS_045,
			FloatMath.RAD_POS_090,
			FloatMath.RAD_POS_135,
			FloatMath.RAD_POS_225,
		};
		
		public override bool IsNodeEnabled => true;

		private float _pulseTimer = 0;

		private readonly WorldUnlockState _ustate;

		public bool IsFullyUnlocked => _ustate==WorldUnlockState.OpenAndUnlocked;

		public OverworldNode_MP(GDOverworldScreen scrn, FPoint pos) : base(scrn, pos, L10NImpl.STR_WORLD_MULTIPLAYER, Levels.WORLD_ID_MULTIPLAYER)
		{
			AddOperationDelayed(new NetworkAnimationTriggerOperation(), NetworkAnimationTriggerOperation.INITIAL_DELAY);

			_ustate = UnlockManager.IsUnlocked(Levels.WORLD_ID_MULTIPLAYER, false);
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			base.OnUpdate(gameTime, istate);

			_pulseTimer += gameTime.ElapsedSeconds;
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			var outerBounds = FRectangle.CreateByCenter(Position, DrawingBoundingBox);
			var innerBounds = FRectangle.CreateByCenter(Position, new FSize(INNERSIZE, INNERSIZE));

			FlatRenderHelper.DrawRoundedBlurPanel(sbatch, outerBounds, clickArea.IsMouseDown() ? FlatColors.ButtonPressedHUD : FlatColors.Asbestos, 0.5f * GDConstants.TILE_WIDTH);
			SimpleRenderHelper.DrawRoundedRectOutline(sbatch, outerBounds.AsInflated(1f, 1f), FlatColors.MidnightBlue, 8, 2f, 0.5f * GDConstants.TILE_WIDTH);

			sbatch.FillRectangle(innerBounds, FlatColors.Background);

			var scoreRectSize = innerBounds.Width / 10f;
			for (int x = 0; x < 10; x++)
			{
				for (int y = 0; y < 10; y++)
				{
					var bc = ((x % 2 == 0) ^ (y % 2 == 0)) ? FlatColors.Background : FlatColors.BackgroundLight;

					if (_ustate == WorldUnlockState.OpenAndUnlocked)
					{
						var d = FloatMath.Sqrt((x - 3.5f) * (x - 3.5f) + (y - 3.5f) * (y - 3.5f));

						var p = 1 - (d / 4.5f);
						if (p < 0) p = 0;

						p *= FloatMath.PercSin(_pulseTimer * FloatMath.TAU * 0.25f);

						bc = ColorMath.Blend(bc, FlatColors.PeterRiver, p);
					}

					var col = ColorMath.Blend(FlatColors.Background, bc, AlphaOverride);
					sbatch.FillRectangle(new FRectangle(innerBounds.X + scoreRectSize * x, innerBounds.Y + scoreRectSize * y, scoreRectSize, scoreRectSize), col);
				}
			}

			sbatch.DrawStretched(Textures.TexIconNetworkBase,    innerBounds, Color.White);
			sbatch.DrawStretched(Textures.TexIconNetworkVertex1, innerBounds, Color.White, VertexRotations[0]);
			sbatch.DrawStretched(Textures.TexIconNetworkVertex2, innerBounds, Color.White, VertexRotations[1]);
			sbatch.DrawStretched(Textures.TexIconNetworkVertex3, innerBounds, Color.White, VertexRotations[2]);
			sbatch.DrawStretched(Textures.TexIconNetworkVertex4, innerBounds, Color.White, VertexRotations[3]);
			sbatch.DrawStretched(Textures.TexIconNetworkVertex5, innerBounds, Color.White, VertexRotations[4]);
			
			sbatch.DrawRectangle(innerBounds, Color.Black, Owner.PixelWidth);
			
			FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH, L10N.T(_l10ndescription), FlatColors.TextHUD, Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH));

		}

		protected override void OnClick(GameEntityMouseArea area, SAMTime gameTime, InputState istate)
		{
			var ownr = ((GDOverworldScreen)Owner);
			if (ownr.IsTransitioning) return;
			
			Owner.HUD.AddModal(new MultiplayerMainPanel(), true, 0.5f, 1f);
		}
	}
}
