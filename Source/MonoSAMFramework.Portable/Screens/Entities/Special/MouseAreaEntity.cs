using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;

namespace MonoSAMFramework.Portable.Screens.Entities.Special
{
	public class MouseAreaEntity : GameEntity, IGameEntityMouseAreaListener
	{
		public override FPoint Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Color.Violet * 0.1f;

		public Action Click = null;
		public Action MouseDown = null;
		public Action MouseUp = null;

		public MouseAreaEntity(GameScreen scrn, FPoint pos, FSize width, int order) : base(scrn, order)
		{
			Position = pos;
			DrawingBoundingBox = width;
		}

		public override void OnInitialize(EntityManager manager)
		{
			AddMouseArea(FRectangle.CreateByCenter(Position, DrawingBoundingBox), this);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			//
		}

		public void OnMouseEnter(GameEntityMouseArea sender, SAMTime gameTime, InputState istate)
		{
			//
		}

		public void OnMouseLeave(GameEntityMouseArea sender, SAMTime gameTime, InputState istate)
		{
			//
		}

		public void OnMouseDown(GameEntityMouseArea sender, SAMTime gameTime, InputState istate)
		{
			MouseDown?.Invoke();
		}

		public void OnMouseUp(GameEntityMouseArea sender, SAMTime gameTime, InputState istate)
		{
			MouseUp?.Invoke();
		}

		public void OnMouseMove(GameEntityMouseArea sender, SAMTime gameTime, InputState istate)
		{
			//
		}

		public void OnMouseClick(GameEntityMouseArea sender, SAMTime gameTime, InputState istate)
		{
			Click?.Invoke();
		}
	}
}
