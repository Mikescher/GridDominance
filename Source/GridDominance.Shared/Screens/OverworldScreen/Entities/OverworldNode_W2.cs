using Microsoft.Xna.Framework;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer;
using System.Linq;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.ColorHelper;
using System.Collections.Generic;
using GridDominance.Levelfileformat.Blueprint;
using System;
using GridDominance.Shared.Screens.OverworldScreen.HUD;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	class OverworldNode_W2 : OverworldNode_Graph
	{
		public OverworldNode_W2(GDOverworldScreen scrn, Vector2 pos) : base(scrn, pos, Levels.WORLD_002)
		{
			//
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			if (IsUnlocked())
				DrawGridProgress(sbatch);
			else
				DrawLockSwing(sbatch);
		}
		 
		private bool? _isGPUnlocked = null;
		protected override bool IsUnlocked()
		{
			if (_isGPUnlocked == null)
			{
				var supplyNodes = Levels.WORLD_001.Nodes.Where(n => n.OutgoingPipes.Any(p => p.Target == Blueprint.ID));

				_isGPUnlocked = supplyNodes.Any(l => MainGame.Inst.Profile.GetLevelData(l).HasAnyCompleted());
			}

			if (GDConstants.USE_IAB)
			{
				// LIGHT VERSION

				if (!_isGPUnlocked.Value) return false;

				if (MainGame.Inst.Profile.PurchasedWorlds.Contains(Blueprint.ID)) return true;
				
				var ip = MainGame.Inst.Bridge.IAB.IsPurchased(MainGame.IAB_WORLD2);

				switch (ip)
				{
					case PurchaseQueryResult.Purchased:
						MainGame.Inst.Profile.PurchasedWorlds.Add(Blueprint.ID);
						MainGame.Inst.SaveProfile();
						return true;
					case PurchaseQueryResult.NotPurchased:
					case PurchaseQueryResult.Refunded:
					case PurchaseQueryResult.Cancelled:
						return false;
					case PurchaseQueryResult.Error:
						Owner.HUD.ShowToast(L10N.T(L10NImpl.STR_IAB_TESTERR), 40, FlatColors.Pomegranate, FlatColors.Foreground, 2.5f);
						return false;
					case PurchaseQueryResult.NotConnected:
						Owner.HUD.ShowToast(L10N.T(L10NImpl.STR_IAB_TESTNOCONN), 40, FlatColors.Pomegranate, FlatColors.Foreground, 2.5f);
						return false;
					case PurchaseQueryResult.CurrentlyInitializing:
						Owner.HUD.ShowToast(L10N.T(L10NImpl.STR_IAB_TESTINPROGRESS), 40, FlatColors.Pomegranate, FlatColors.Foreground, 2.5f);
						return false;
					default:
						SAMLog.Error("EnumSwitch", "IsUnlocked()", "MainGame.Inst.Bridge.IAB.IsPurchased(MainGame.IAB_WORLD2)) -> " + ip);
						return false;
				}
			}
			else
			{
				// FULL VERSION

				return _isGPUnlocked.Value;
			}
		}

		protected override void OnClickDeny()
		{
			if (_isGPUnlocked == false)
			{
				base.OnClickDeny();
			}
			else
			{
				LevelBlueprint[] previews =
				{
					Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000019")],
					Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000015")],
					Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000012")],
					Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000013")],
					Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000005")],
					Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000025")],
					Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000027")],
				};

				Owner.HUD.AddModal(new WorldPreviewPanel(previews), true);
			}
		}
	}
}
