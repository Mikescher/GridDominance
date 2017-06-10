using System;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Other
{
	public sealed class HUDSubScreenProxyRenderer : HUDElement, IProxyScreenProvider
	{
		public override int Depth { get; } = Int32.MaxValue;

		private GameScreen _child;

		private FRectangle _boundsBackup;
		public FRectangle ProxyTargetBounds => _boundsBackup;
		public GameScreen Proxy => _child;

		public HUDSubScreenProxyRenderer(GameScreen child)
		{
			_child = child;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
#if DEBUG
			sbatch.FillRectangle(bounds, Color.Magenta);
#endif
		}

		public void ChangeScreen(GameScreen newchild)
		{
			_child.Remove();
			_child = newchild;

			_child.VAdapterGame = _child.VAdapterHUD.CreateProxyAdapter(this);
			_child.VAdapterHUD  = _child.VAdapterHUD.CreateProxyAdapter(this);
			_child.Show();
		}

		public override void OnInitialize()
		{
			HUD.Screen.RegisterProxyScreenProvider(this);

			_child.VAdapterGame = _child.VAdapterHUD.CreateProxyAdapter(this);
			_child.VAdapterHUD  = _child.VAdapterHUD.CreateProxyAdapter(this);
			_child.Show();

			_boundsBackup = HUD.Screen.VAdapterHUD.ScreenToRect(BoundingRectangle);
		}

		public override void OnRemove()
		{
			_child.Remove();
			HUD.Screen.DeregisterProxyScreenProvider(this);
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			_child.Update(gameTime);
		}

		protected override void OnAfterRecalculatePosition()
		{
			_boundsBackup = HUD.Screen.VAdapterHUD.ScreenToRect(BoundingRectangle);
		}
	}
}
