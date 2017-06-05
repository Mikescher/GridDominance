using Microsoft.Xna.Framework;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer;
using System.Linq;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.LogProtocol;

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

				var ip = MainGame.Inst.Bridge.IAB.IsPurchased(MainGame.IAB_WORLD2);

				return true;//TODO

				switch (ip) //TODO
				{
					case PurchaseQueryResult.Purchased:
						return true;
					case PurchaseQueryResult.NotPurchased:
						return false;
					case PurchaseQueryResult.Refunded:
						return false;
					case PurchaseQueryResult.Cancelled:
						return false;
					case PurchaseQueryResult.Error:
						return false;
					case PurchaseQueryResult.NotConnected:
						return false;
					case PurchaseQueryResult.CurrentlyInitializing:
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
	}
}
