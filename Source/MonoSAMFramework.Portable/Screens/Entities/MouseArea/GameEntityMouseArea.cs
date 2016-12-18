using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Screens.Entities.MouseArea
{
	public class GameEntityMouseArea : ISAMUpdateable
	{
		public readonly IFShape RelativeShape;
		public readonly GameEntity Owner;

		private readonly List<IGameEntityMouseAreaListener> listener = new List<IGameEntityMouseAreaListener>(); 

		private Vector2 ownerPositionCache = Vector2.Zero;
		private IFShape absoluteShapeCache = null;

		private bool isInShape = false;
		private Vector2 pointerPosition = Vector2.Zero;
		private bool isClickDown = false;

		public IFShape AbsoluteShape
		{
			get
			{
				if (absoluteShapeCache == null || ownerPositionCache != Owner.Position)
				{
					ownerPositionCache = Owner.Position;
					absoluteShapeCache = RelativeShape.AsTranslated(ownerPositionCache);
				}
				return absoluteShapeCache;
			}
		}

		public GameEntityMouseArea(GameEntity owner, IFShape shape)
		{
			RelativeShape = shape;
			Owner = owner;
		}

		public void AddListener(IGameEntityMouseAreaListener l)
		{
			listener.Add(l);
		}

		public void Update(GameTime gameTime, InputState istate)
		{
			var prevPointerPos = pointerPosition;
			var prevInShape = isInShape;

			pointerPosition = istate.PointerPositionOnMap;
			isInShape = AbsoluteShape.Contains(pointerPosition);

			var hasMoved = !pointerPosition.EpsilonEquals(prevPointerPos, 0.5f);


			if (isInShape && !prevInShape) foreach (var lst in listener) lst.OnMouseEnter(this, gameTime, istate);

			if (! isInShape && prevInShape) foreach (var lst in listener) lst.OnMouseEnter(this, gameTime, istate);

			if (isInShape && hasMoved) foreach (var lst in listener) lst.OnMouseMove(this, gameTime, istate);

			if (isInShape && istate.IsJustDown) foreach (var lst in listener) lst.OnMouseDown(this, gameTime, istate);

			if (isInShape && istate.IsJustUp) foreach (var lst in listener) lst.OnMouseUp(this, gameTime, istate);

			if (isClickDown && isInShape && istate.IsJustUp) foreach (var lst in listener) lst.OnMouseClick(this, gameTime, istate);


			if (!isInShape) isClickDown = false;
			if (hasMoved) isClickDown = false;
			if (!istate.IsDown) isClickDown = false;
			if (isInShape && istate.IsJustDown) isClickDown = true;
		}
	}
}
