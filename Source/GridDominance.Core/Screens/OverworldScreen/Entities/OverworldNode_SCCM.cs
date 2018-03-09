using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations;
using GridDominance.Shared.Screens.OverworldScreen.HUD;
using GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	public class OverworldNode_SCCM : OverworldNode
	{
		public override bool IsNodeEnabled => true;

		private readonly WorldUnlockState _ustate;

		public bool IsFullyUnlocked => _ustate==WorldUnlockState.OpenAndUnlocked;

		public readonly List<MutableTuple<FRectangle, Color>> Blocks = new List<MutableTuple<FRectangle, Color>>();

		public FRectangle NonTranslatedBounds = FRectangle.CreateByCenter(FPoint.Zero, new FSize(INNERSIZE, INNERSIZE));

		public OverworldNode_SCCM(GDOverworldScreen scrn, FPoint pos) : base(scrn, pos, L10NImpl.STR_WORLD_ONLINE, Levels.WORLD_ID_ONLINE)
		{
			AddOperationDelayed(new TetrisInitialOperation(0.50f), 0.75f);
			AddOperation(new CyclicSequenceOperation<OverworldNode_SCCM>(
				new SleepOperation<OverworldNode_SCCM>(1.50f),
				new TetrisFillOperation(5.50f),
				new SleepOperation<OverworldNode_SCCM>(0.75f),
				new TetrisBlendOperation(0.75f),
				new SleepOperation<OverworldNode_SCCM>(0.25f),
				new TetrisShrinkOperation(2.50f)));

			_ustate = UnlockManager.IsUnlocked(Levels.WORLD_ID_ONLINE, false);
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
						var d = FloatMath.Sqrt((x - 4.5f) * (x - 4.5f) + (y - 4.5f) * (y - 4.5f));

						var p = FloatMath.PercSin(FloatMath.PI * 3 * d / 14f - Lifetime);

						p *= 0.25f;

						bc = ColorMath.Blend(bc, FlatColors.PeterRiver, p);
					}

					var col = ColorMath.Blend(FlatColors.Background, bc, AlphaOverride);
					sbatch.FillRectangle(new FRectangle(innerBounds.X + scoreRectSize * x, innerBounds.Y + scoreRectSize * y, scoreRectSize, scoreRectSize), col);
				}
			}

			foreach (var block in Blocks)
			{
				sbatch.FillRectangle(block.Item1.WithOrigin(Position), block.Item2);

			}

			sbatch.DrawRectangle(innerBounds, Color.Black, Owner.PixelWidth);
			
			FontRenderHelper.DrawTextCentered(sbatch, Textures.HUDFontBold, 0.9f * GDConstants.TILE_WIDTH, L10N.T(_l10ndescription), FlatColors.TextHUD, Position + new Vector2(0, 2.25f * GDConstants.TILE_WIDTH));
		}

		protected override void OnClick(GameEntityMouseArea area, SAMTime gameTime, InputState istate)
		{
			var ownr = ((GDOverworldScreen)Owner);
			if (ownr.IsTransitioning) return;

			if (MainGame.Inst.Profile.AccountType == AccountType.Local)
			{
				MainGame.Inst.ShowToast(null, L10N.T(L10NImpl.STR_MP_CONNECTING), 40, FlatColors.Emerald, FlatColors.Foreground, 2f);
				MainGame.Inst.Backend.CreateUser(MainGame.Inst.Profile).RunAsync();
				return;
			}

			var ustate = UnlockManager.IsUnlocked(Levels.WORLD_ID_ONLINE, true);

			switch (ustate)
			{
				case WorldUnlockState.OpenAndUnlocked:
					Owner.HUD.AddModal(new SCCMMainPanel(), true, 0.5f, 1f);
					break;
				case WorldUnlockState.ReachableButMustBePreviewed:
				case WorldUnlockState.UnreachableButCanBePreviewed:
					Owner.HUD.AddModal(new SCCMPreviewPanel(), true, 0.5f, 1f);
					break;
				case WorldUnlockState.UnreachableAndFullyLocked:
					Owner.HUD.ShowToast("ONSCCM::LOCKED(MULTI)", L10N.T(L10NImpl.STR_GLOB_WORLDLOCK), 40, FlatColors.Pomegranate, FlatColors.Foreground, 1.5f);
					MainGame.Inst.GDSound.PlayEffectError();

					AddOperation(new ShakeNodeOperation());
					break;
				default:
					SAMLog.Error("ONSCCM::EnumSwitch_OC", "ustate: " + ustate);
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
