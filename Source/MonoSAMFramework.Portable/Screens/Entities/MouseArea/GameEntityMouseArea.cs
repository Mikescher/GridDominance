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
		private readonly bool doSwallowEvents;

		private Vector2 ownerPositionCache = Vector2.Zero;
		private IFShape absoluteShapeCache = null;

		private bool isInShape = false;
		private Vector2 pointerPosition = Vector2.Zero;
		private bool isClickDown = false;

		public bool IsEnabled = true;

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

		public GameEntityMouseArea(GameEntity owner, IFShape shape, bool swallowEvents)
		{
			RelativeShape = shape;
			Owner = owner;
			doSwallowEvents = swallowEvents;
		}

		public void AddListener(IGameEntityMouseAreaListener l)
		{
			listener.Add(l);
		}

		public void Update(SAMTime gameTime, InputState istate)
		{
			if (IsEnabled)
			{
				var prevPointerPos = pointerPosition;
				var prevInShape = isInShape;

				pointerPosition = istate.GamePointerPositionOnMap;
				isInShape = AbsoluteShape.Contains(pointerPosition);

				var hasMoved = !pointerPosition.EpsilonEquals(prevPointerPos, 0.5f);

				var iju = istate.IsExclusiveJustUp;
				var ijd = istate.IsExclusiveJustDown;

				if (isInShape && doSwallowEvents) istate.Swallow(InputConsumer.GameEntity);

				if (isInShape && !prevInShape)
				{
					foreach (var lst in listener) lst.OnMouseEnter(this, gameTime, istate);
				}

				if (!isInShape && prevInShape)
				{
					foreach (var lst in listener) lst.OnMouseEnter(this, gameTime, istate);
				}

				if (isInShape && hasMoved)
				{
					foreach (var lst in listener) lst.OnMouseMove(this, gameTime, istate);
				}

				if (isInShape && ijd)
				{
					foreach (var lst in listener) lst.OnMouseDown(this, gameTime, istate);
				}

				if (isInShape && iju)
				{
					foreach (var lst in listener) lst.OnMouseUp(this, gameTime, istate);
				}

				if (isClickDown && isInShape && iju)
				{
					foreach (var lst in listener) lst.OnMouseClick(this, gameTime, istate);
				}

				if (!isInShape) isClickDown = false;
				//if (hasMoved) isClickDown = false;
				if (!istate.IsRealDown) isClickDown = false;
				if (isInShape && ijd) isClickDown = true;
			}
			else
			{
				isClickDown = false;
			}
		}

		public void CancelClick()
		{
			isClickDown = false;
		}

		public bool IsMouseDown() => isClickDown;
	}
}
