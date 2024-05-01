﻿using Microsoft.Xna.Framework;
using GridDominance.Graphfileformat.Blueprint;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.ColorHelper;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.BatchRenderer;
using System.Collections.Generic;
using System;
using System.Linq;
using GridDominance.Shared.Screens.Common;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations;
using GridDominance.Shared.Screens.OverworldScreen.Operations;
using MonoSAMFramework.Portable.UpdateAgents.Impl;
using GridDominance.Core.Resources;

// ReSharper disable HeuristicUnreachableCode
#pragma warning disable 162
namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	public abstract class OverworldNode_Graph : OverworldNode
	{
		public readonly GraphBlueprint Blueprint;
		public readonly string IABCode;

		private readonly Dictionary<FractionDifficulty, float> solvedPerc = new Dictionary<FractionDifficulty, float>();

		public float GetPerc(FractionDifficulty d) => solvedPerc[d];
		
		private readonly float _swingPeriode = 4f;

		protected WorldUnlockState _ustate;

		public override bool IsNodeEnabled => _ustate == WorldUnlockState.OpenAndUnlocked;

		public int ForceClickCounter = 0;

		protected OverworldNode_Graph(GDOverworldScreen scrn, FPoint pos, GraphBlueprint world, string iab) 
			: base(scrn, pos, Levels.WORLD_NAMES[world.ID], world.ID)
		{
			Blueprint = world;
			IABCode = iab;

			solvedPerc[FractionDifficulty.DIFF_0] = GetSolvePercentage(FractionDifficulty.DIFF_0);
			solvedPerc[FractionDifficulty.DIFF_1] = GetSolvePercentage(FractionDifficulty.DIFF_1);
			solvedPerc[FractionDifficulty.DIFF_2] = GetSolvePercentage(FractionDifficulty.DIFF_2);
			solvedPerc[FractionDifficulty.DIFF_3] = GetSolvePercentage(FractionDifficulty.DIFF_3);

			_swingPeriode *= FloatMath.GetRangedRandom(0.85f, 1.15f);

			_ustate = UnlockManager.IsUnlocked(world, false);
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			switch (_ustate)
			{
				case WorldUnlockState.OpenAndUnlocked:
					DrawGridProgress(sbatch);
					break;
				case WorldUnlockState.ReachableButMustBePreviewed:
					DrawGridGreenLock(sbatch);
					break;
				case WorldUnlockState.UnreachableButCanBePreviewed:
					DrawLockSwing(sbatch);
					break;
				case WorldUnlockState.UnreachableAndFullyLocked:
					DrawLockSwing(sbatch);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected void DrawGridProgress(IBatchRenderer sbatch)
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
				sbatch.DrawLine(innerBounds.Left, innerBounds.Top + i * scoreRectSize, innerBounds.Right, innerBounds.Top + i * scoreRectSize, Color.Black * AlphaOverride, Owner.PixelWidth);
				sbatch.DrawLine(innerBounds.Left + i * scoreRectSize, innerBounds.Top, innerBounds.Left + i * scoreRectSize, innerBounds.Bottom, Color.Black * AlphaOverride, Owner.PixelWidth);
			}

			FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH, L10N.T(_l10ndescription), FlatColors.TextHUD, Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH));
		}

		protected void DrawGridGreenLock(IBatchRenderer sbatch)
		{
			var outerBounds = FRectangle.CreateByCenter(Position, DrawingBoundingBox);
			var innerBounds = FRectangle.CreateByCenter(Position, new FSize(INNERSIZE, INNERSIZE));

			FlatRenderHelper.DrawRoundedBlurPanel(sbatch, outerBounds, clickArea.IsMouseDown() ? FlatColors.ButtonPressedHUD : FlatColors.Asbestos, 0.5f * GDConstants.TILE_WIDTH);
			SimpleRenderHelper.DrawRoundedRectOutline(sbatch, outerBounds.AsInflated(1f, 1f), FlatColors.MidnightBlue, 8, 2f, 0.5f * GDConstants.TILE_WIDTH);

			sbatch.FillRectangle(innerBounds, FlatColors.Background);

			sbatch.DrawCentered(Textures.TexIconLockOpen, innerBounds.Center, INNERSIZE * 0.75f, INNERSIZE * 0.75f, FlatColors.Nephritis);

			FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH, L10N.T(_l10ndescription), FlatColors.TextHUD, Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH));
		}

		protected void DrawLockSwing(IBatchRenderer sbatch)
		{
			var outerBounds = FRectangle.CreateByCenter(Position, DrawingBoundingBox);
			var innerBounds = FRectangle.CreateByCenter(Position, new FSize(INNERSIZE, INNERSIZE));

			FlatRenderHelper.DrawRoundedBlurPanel(sbatch, outerBounds, clickArea.IsMouseDown() ? FlatColors.ButtonPressedHUD : FlatColors.Asbestos, 0.5f * GDConstants.TILE_WIDTH);
			SimpleRenderHelper.DrawRoundedRectOutline(sbatch, outerBounds.AsInflated(1f, 1f), FlatColors.MidnightBlue, 8, 2f, 0.5f * GDConstants.TILE_WIDTH);

			sbatch.DrawRectangle(innerBounds, Color.Black);

			var rot = FloatMath.Sin(Lifetime * FloatMath.TAU / _swingPeriode) * FloatMath.RAD_POS_005;

			sbatch.DrawCentered(Textures.TexIconLock, innerBounds.Center, INNERSIZE * 0.75f, INNERSIZE * 0.75f, Color.White, rot);

			FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH, L10N.T(_l10ndescription), FlatColors.Asbestos, Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH));
		}

		private Color GetCellColor(int x, int y)
		{
			var bc = ((x % 2 == 0) ^ (y % 2 == 0)) ? FlatColors.Background : FlatColors.BackgroundLight;

			switch (y)
			{
				case 0: return IsCellActive(FractionDifficulty.DIFF_0, 0 + x) ? FractionDifficultyHelper.COLOR_DIFFICULTY_0 : bc;
				case 1: return IsCellActive(FractionDifficulty.DIFF_0, 8 + x) ? FractionDifficultyHelper.COLOR_DIFFICULTY_0 : bc;
				case 2: return IsCellActive(FractionDifficulty.DIFF_1, 0 + x) ? FractionDifficultyHelper.COLOR_DIFFICULTY_1 : bc;
				case 3: return IsCellActive(FractionDifficulty.DIFF_1, 8 + x) ? FractionDifficultyHelper.COLOR_DIFFICULTY_1 : bc;
				case 4: return IsCellActive(FractionDifficulty.DIFF_2, 0 + x) ? FractionDifficultyHelper.COLOR_DIFFICULTY_2 : bc;
				case 5: return IsCellActive(FractionDifficulty.DIFF_2, 8 + x) ? FractionDifficultyHelper.COLOR_DIFFICULTY_2 : bc;
				case 6: return IsCellActive(FractionDifficulty.DIFF_3, 0 + x) ? FractionDifficultyHelper.COLOR_DIFFICULTY_3 : bc;
				case 7: return IsCellActive(FractionDifficulty.DIFF_3, 8 + x) ? FractionDifficultyHelper.COLOR_DIFFICULTY_3 : bc;

				default:
					SAMLog.Error("ONG::EnumSwitch_GCC", "value: " + y);
					break;
			}

			throw new ArgumentOutOfRangeException(nameof(y), y, null);
		}

		private float GetSolvePercentage(FractionDifficulty d)
		{
			int c = Blueprint.LevelNodes.Count(n => MainGame.Inst.Profile.GetLevelData(n.LevelID).HasCompletedOrBetter(d));

			return c * 1f / Blueprint.LevelNodes.Count;
		}

		private bool IsCellActive(FractionDifficulty d, int idx)
		{
			bool active;

			if (solvedPerc[d] < 0.5f)
			{
				active = idx < FloatMath.Ceiling(solvedPerc[d] * 15);
			}
			else
			{
				active = idx <= FloatMath.Floor(solvedPerc[d] * 15);
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
		
		protected override void OnClick(GameEntityMouseArea area, SAMTime gameTime, InputState istate)
		{
#if DEBUG
			if (DebugSettings.Get("UnlockNode")) { OnClick_OpenAndUnlocked(); return; }

			if (DebugSettings.Get("WorldPreview")) { MainGame.Inst.Profile.PurchasedWorlds.Clear(); ShowPreview(); return; }
#endif

			_ustate = UnlockManager.IsUnlocked(Blueprint, true);

			//MainGame.Inst.GDBridge.IAB.SynchronizePurchases(GDConstants.IABList);

			switch (_ustate)
			{
				case WorldUnlockState.OpenAndUnlocked:
					OnClick_OpenAndUnlocked();
					break;
				case WorldUnlockState.ReachableButMustBePreviewed:
					OnClick_ReachableButMustBeBought();
					break;
				case WorldUnlockState.UnreachableButCanBePreviewed:
					OnClick_UnreachableButCanBeBought();
					break;
				case WorldUnlockState.UnreachableAndFullyLocked:
					OnClick_UnreachableAndFullyLocked();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		
		protected abstract void OnClick_OpenAndUnlocked();
		protected abstract void OnClick_ReachableButMustBeBought();
		protected abstract void OnClick_UnreachableButCanBeBought();
		protected abstract void OnClick_UnreachableAndFullyLocked();

		protected void DefaultAction_OpenAndUnlocked()
		{
			var ownr = ((GDOverworldScreen)Owner);

			if (ownr.IsTransitioning) return;

			ownr.IsTransitioning = true;

			ownr.AddAgent(new TransitionZoomInOperation(this, Blueprint));

			MainGame.Inst.GDSound.PlayEffectZoomIn();
		}

		protected void DefaultAction_ReachableButMustBeBought()
		{
			ShowPreview();
		}

		protected void DefaultAction_UnreachableButCanBeBought()
		{
			Owner.HUD.ShowToast("OWNG::LOCKED(MULTI)", L10N.T(L10NImpl.STR_GLOB_WORLDLOCK), 40, FlatColors.Pomegranate, FlatColors.Foreground, 1.5f);
			MainGame.Inst.GDSound.PlayEffectError();

			AddOperation(new ShakeNodeOperation());
			AddOperation(new LambdaOperation<OverworldNode_Graph>("ShowPreviewDelayed", 0.25f, (n, p) => { }, n => { }, n => n.ShowPreview()));
		}

		protected void DefaultAction_UnreachableAndFullyLocked()
		{
			Owner.HUD.ShowToast("OWNG::LOCKED(MULTI)", L10N.T(L10NImpl.STR_GLOB_WORLDLOCK), 40, FlatColors.Pomegranate, FlatColors.Foreground, 1.5f);
			MainGame.Inst.GDSound.PlayEffectError();

			AddOperation(new ShakeNodeOperation());
		}

		public virtual void ShowPreview() { }
	}
}
