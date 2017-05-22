using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations;
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

		public const float COLLAPSE_TIME = 5f;

		public Vector2 NodePos;
		public override Vector2 Position => NodePos;
		public override FSize DrawingBoundingBox { get; } = new FSize(SIZE, SIZE);
		public override Color DebugIdentColor { get; } = Color.Blue;

		private readonly string _description;
		public readonly GraphBlueprint Blueprint;
		private readonly GameEntityMouseArea clickArea;

		private readonly Dictionary<FractionDifficulty, float> solvedPerc = new Dictionary<FractionDifficulty, float>();

		public float AlphaOverride = 1f;

		public float FlickerTime = 0f;

		public bool NodeEnabled = false;
		public int ForceClickCounter = 0;

		public OverworldNode(GDOverworldScreen scrn, Vector2 pos, GraphBlueprint blueprint) : base(scrn, GDConstants.ORDER_WORLD_NODE)
		{
			_description = blueprint.Name;
			Blueprint = blueprint;
			NodePos = pos;

			clickArea = AddClickMouseArea(FRectangle.CreateByCenter(Vector2.Zero, new FSize(SIZE, SIZE)), OnClick);

			solvedPerc[FractionDifficulty.DIFF_0] = GetSolvePercentage(FractionDifficulty.DIFF_0);
			solvedPerc[FractionDifficulty.DIFF_1] = GetSolvePercentage(FractionDifficulty.DIFF_1);
			solvedPerc[FractionDifficulty.DIFF_2] = GetSolvePercentage(FractionDifficulty.DIFF_2);
			solvedPerc[FractionDifficulty.DIFF_3] = GetSolvePercentage(FractionDifficulty.DIFF_3);
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
			FlickerTime += gameTime.ElapsedSeconds;
		}

		private void OnClick(GameEntityMouseArea area, SAMTime gameTime, InputState istate)
		{
			if (NodeEnabled)
			{
				var ownr = ((GDOverworldScreen) Owner);

				if (ownr.IsTransitioning) return;

				ownr.IsTransitioning = true;

				ownr.AddAgent(new TransitionZoomInAgent(ownr, this, Blueprint));

				MainGame.Inst.GDSound.PlayEffectZoomIn();
			}
			else
			{

				if (Blueprint == Levels.WORLD_001)
				{
					if (ForceClickCounter == 0)
					{
						Owner.HUD.ShowToast("Click two times more to unlock", 40, FlatColors.Silver, FlatColors.Foreground, 2f);
						ForceClickCounter++;
					}
					else if (ForceClickCounter == 1)
					{
						Owner.HUD.ShowToast("Click again to unlock", 40, FlatColors.Silver, FlatColors.Foreground, 2f);
						ForceClickCounter++;
					}
					else if (ForceClickCounter == 2)
					{
						Owner.HUD.ShowToast("World unlocked", 40, FlatColors.Silver, FlatColors.Foreground, 2f);

						MainGame.Inst.Profile.SkipTutorial = true;
						MainGame.Inst.SaveProfile();
						NodeEnabled = true;
						return;
					}
				}
				else
				{
					Owner.HUD.ShowToast("World locked", 40, FlatColors.Pomegranate, FlatColors.Foreground, 1.5f);
				}

				// TODO Erro sound
				AddEntityOperation(new ShakeNodeOperation());
			}
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			var outerBounds = FRectangle.CreateByCenter(Position, DrawingBoundingBox);
			var innerBounds = FRectangle.CreateByCenter(Position, new FSize(INNERSIZE, INNERSIZE));

			var scoreRectSize = innerBounds.Width / 8f;

			FlatRenderHelper.DrawRoundedBlurPanel(sbatch, outerBounds, clickArea.IsMouseDown() ? FlatColors.ButtonPressedHUD : FlatColors.Asbestos, 0.5f * GDConstants.TILE_WIDTH);
			SimpleRenderHelper.DrawRoundedRectOutline(sbatch, outerBounds.AsInflated(1f, 1f), FlatColors.MidnightBlue, 8, 2f, 0.5f * GDConstants.TILE_WIDTH);

			if (NodeEnabled)
			{
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
					sbatch.DrawLine(innerBounds.Left, innerBounds.Top + i * scoreRectSize, innerBounds.Right, innerBounds.Top + i * scoreRectSize, Color.Black * AlphaOverride);
					sbatch.DrawLine(innerBounds.Left + i * scoreRectSize, innerBounds.Top, innerBounds.Left + i * scoreRectSize, innerBounds.Bottom, Color.Black * AlphaOverride);
				}

				FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH, _description, FlatColors.TextHUD, Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH));
			}
			else
			{
				sbatch.DrawRectangle(innerBounds, Color.Black);

				sbatch.DrawCentered(Textures.TexIconLock, innerBounds.VecCenter, INNERSIZE * 0.75f, INNERSIZE * 0.75f, Color.White);

				FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH, _description, FlatColors.Asbestos, Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH));
			}

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

		private float GetSolvePercentage(FractionDifficulty d)
		{
			int c = Blueprint.Nodes.Count(n => MainGame.Inst.Profile.GetLevelData(n.LevelID).HasCompleted(d));

			return c * 1f / Blueprint.Nodes.Count;
		}

		private bool IsCellActive(FractionDifficulty d, int idx)
		{
			bool active;

			if (solvedPerc[d] < 0.5f)
			{
				active = idx < FloatMath.Ceiling(solvedPerc[d] * 20);
			}
			else
			{
				active = idx <= FloatMath.Floor(solvedPerc[d] * 20);
			}
			
			if (FlickerTime > COLLAPSE_TIME)
			{
				return active;
			}
			else
			{
				if (active)
				{
					return FloatMath.GetRandom() < (0.5f + FloatMath.FunctionEaseInOutCubic(FlickerTime / COLLAPSE_TIME) / 2f);
				}
				else
				{
					return FloatMath.GetRandom() < (0.5f - FloatMath.FunctionEaseInOutCubic(FlickerTime / COLLAPSE_TIME) / 2f);
				}
			}
		}
	}
}
