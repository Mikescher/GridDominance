using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Other
{
	public sealed class HUDSubScreenProxyRenderer : HUDElement, IProxyScreenProvider
	{
		public override int Depth { get; } = Int32.MaxValue;

		public readonly GameScreen Child;

		private FRectangle _boundsBackup;
		public FRectangle ProxyTargetBounds => _boundsBackup;
		public GameScreen Proxy => Child;

		public HUDSubScreenProxyRenderer(GameScreen child)
		{
			Child = child;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
#if DEBUG
			sbatch.FillRectangle(bounds, Color.Magenta);
#endif
		}

		public override void OnInitialize()
		{
			HUD.Screen.RegisterProxyScreenProvider(this);

			Child.VAdapterGame = Child.VAdapterHUD.CreateProxyAdapter(this);
			Child.VAdapterHUD  = Child.VAdapterHUD.CreateProxyAdapter(this);
			Child.Show();

			_boundsBackup = HUD.Screen.VAdapterHUD.ScreenToRect(BoundingRectangle);
		}

		public override void OnRemove()
		{
			Child.Remove();
			HUD.Screen.DeregisterProxyScreenProvider(this);
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			Child.Update(gameTime);
		}

		protected override void OnAfterRecalculatePosition()
		{
			_boundsBackup = HUD.Screen.VAdapterHUD.ScreenToRect(BoundingRectangle);
		}
	}
}
