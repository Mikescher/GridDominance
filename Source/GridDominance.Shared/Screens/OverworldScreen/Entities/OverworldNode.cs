using System;
using GridDominance.Graphfileformat.Parser;
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
	public class OverworldNode : GameEntity
	{
		public const float SIZE = 3 * GDConstants.TILE_WIDTH;
		public const float INNERSIZE = 2 * GDConstants.TILE_WIDTH;

		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; } = new FSize(SIZE, SIZE);
		public override Color DebugIdentColor { get; } = Color.Blue;

		private readonly string _description;
		private readonly WorldGraphFile _graph;
		private readonly GameEntityMouseArea clickArea;

		public float AlphaOverride = 1f;

		public OverworldNode(GDOverworldScreen scrn, Vector2 pos, string text, WorldGraphFile graph) : base(scrn, GDConstants.ORDER_WORLD_NODE)
		{
			_description = text;
			_graph = graph;
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

			ownr.AddAgent(new TransitionZoomInAgent(ownr, this, _graph));

			MainGame.Inst.GDSound.PlayEffectZoomIn();
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			var outerBounds = FRectangle.CreateByCenter(Position, DrawingBoundingBox);
			var innerBounds = FRectangle.CreateByCenter(Position, new FSize(INNERSIZE, INNERSIZE));

			var scoreRectSize = innerBounds.Width / 8f;

			FlatRenderHelper.DrawRoundedBlurPanel(sbatch, outerBounds, clickArea.IsMouseDown() ? FlatColors.ButtonPressedHUD : FlatColors.Asbestos, 0.5f * GDConstants.TILE_WIDTH);
			SimpleRenderHelper.DrawRoundedRectOutline(sbatch, outerBounds.AsInflated(1f, 1f), FlatColors.MidnightBlue, 8, 2f, 0.5f * GDConstants.TILE_WIDTH);

			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					var col = ColorMath.Blend(FlatColors.Background, GetCellColor(x, y), AlphaOverride);
					sbatch.FillRectangle(new FRectangle(innerBounds.X + scoreRectSize * x, innerBounds.Y + scoreRectSize * y, scoreRectSize, scoreRectSize), col);
				}
			}

			for (int i = 0; i <= 8; i++)
			{
				sbatch.DrawLine(innerBounds.Left, innerBounds.Top + i*scoreRectSize, innerBounds.Right, innerBounds.Top + i * scoreRectSize, Color.Black * AlphaOverride);
				sbatch.DrawLine(innerBounds.Left + i*scoreRectSize, innerBounds.Top, innerBounds.Left + i*scoreRectSize, innerBounds.Bottom, Color.Black * AlphaOverride);
			}

			FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH, _description, FlatColors.TextHUD, Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH));
		}

		private Color GetCellColor(int x, int y)
		{
			switch (y)
			{
				case 0: return IsCellActive(FractionDifficulty.DIFF_0, 0 + x) ? GDColors.COLOR_DIFFICULTY_0 : FlatColors.Background;
				case 1: return IsCellActive(FractionDifficulty.DIFF_0, 8 + x) ? GDColors.COLOR_DIFFICULTY_0 : FlatColors.Background;
				case 2: return IsCellActive(FractionDifficulty.DIFF_1, 0 + x) ? GDColors.COLOR_DIFFICULTY_1 : FlatColors.Background;
				case 3: return IsCellActive(FractionDifficulty.DIFF_1, 8 + x) ? GDColors.COLOR_DIFFICULTY_1 : FlatColors.Background;
				case 4: return IsCellActive(FractionDifficulty.DIFF_2, 0 + x) ? GDColors.COLOR_DIFFICULTY_2 : FlatColors.Background;
				case 5: return IsCellActive(FractionDifficulty.DIFF_2, 8 + x) ? GDColors.COLOR_DIFFICULTY_2 : FlatColors.Background;
				case 6: return IsCellActive(FractionDifficulty.DIFF_3, 0 + x) ? GDColors.COLOR_DIFFICULTY_3 : FlatColors.Background;
				case 7: return IsCellActive(FractionDifficulty.DIFF_3, 8 + x) ? GDColors.COLOR_DIFFICULTY_3 : FlatColors.Background;
			}

			throw new ArgumentOutOfRangeException(nameof(y), y, null);
		}

		private bool IsCellActive(FractionDifficulty d, int idx) // TODO Get correct score for this map
		{
			return FloatMath.GetRandomSign() > 0;

			//switch (d)
			//{
			//	case FractionDifficulty.KI_EASY:
			//		return idx < 14;
			//	case FractionDifficulty.KI_NORMAL:
			//		return idx < 8;
			//	case FractionDifficulty.KI_HARD:
			//		return idx < 6;
			//	case FractionDifficulty.KI_IMPOSSIBLE:
			//		return idx < 3;
			//	default:
			//		throw new ArgumentOutOfRangeException(nameof(d), d, null);
			//}
		}
	}
}
