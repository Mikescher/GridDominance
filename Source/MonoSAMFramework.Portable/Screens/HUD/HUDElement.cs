using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.Screens.HUD.Operations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class HUDElement : ISAMLayeredDrawable, ISAMUpdateable
	{
		public HUDContainer Owner = null; // Only set on add to HUD (the OnInitialize is called)
		public GameHUD HUD = null;        // Only set on add to HUD (the OnInitialize is called)

		public bool Alive = true;
		public bool Initialized { get; private set; } = false;
		public bool IsVisible = true;

		public virtual int DeepInclusiveCount => 1;

		private FPoint _relativePosition = FPoint.Zero;
		public FPoint RelativePosition
		{
			get { return _relativePosition;}
			set { _relativePosition = value; InvalidatePosition(); }
		}

		private FSize _size = FSize.Empty;
		public FSize Size
		{
			get { return _size; }
			set { _size = value; InvalidatePosition(); }
		}

		private HUDAlignment _alignment = HUDAlignment.TOPLEFT;
		public HUDAlignment Alignment
		{
			get { return _alignment; }
			set { _alignment = value; InvalidatePosition(); }
		}

		public FPoint Position { get; protected set; } = FPoint.Zero; // _Not_ Center
		public FRectangle BoundingRectangle { get; protected set; } = FRectangle.Empty;

		public Vector2 RelativeCenter
		{
			get { return new Vector2(RelativePosition.X + Size.Width / 2f, RelativePosition.Y + Size.Height / 2f);}
			set { RelativePosition = new FPoint(value.X - Size.Width / 2f, value.Y - Size.Height / 2f);}
		}

		private bool _isEnabled = true;
		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				if (_isEnabled ^ value)
				{
					_isEnabled = value;
					OnEnabledChanged(value);
				}
			}
		}

		public float Top => BoundingRectangle.Top;
		public float Left => BoundingRectangle.Left;

		public float Bottom => BoundingRectangle.Bottom;
		public float Right => BoundingRectangle.Right;

		public float Width => BoundingRectangle.Width;
		public float Height => BoundingRectangle.Height;

		public float CenterX => Position.X + Size.Width / 2f;
		public float CenterY => Position.Y + Size.Height / 2f;

		public Vector2 Center => new Vector2(CenterX, CenterY);

		protected readonly List<IHUDElementOperation> ActiveOperations = new List<IHUDElementOperation>();

		protected bool IsPointerDownOnElement = false;
		protected bool PositionInvalidated = false;

		public abstract int Depth { get; }

		protected HUDElement()
		{
			//
		}

		public void Remove()
		{
			Alive = false;
		}

		public void Initialize()
		{
			Initialized = true;

			InvalidatePosition();

			OnInitialize();
		}

		public virtual void DrawBackground(IBatchRenderer sbatch)
		{
			DoDrawBackground(sbatch, BoundingRectangle);
		}

		public virtual void DrawForeground(IBatchRenderer sbatch)
		{
			DoDraw(sbatch, BoundingRectangle);

#if DEBUG
			if (DebugSettings.Get("DebugHUDBorders"))
			{
				using (sbatch.BeginDebugDraw()) DrawDebugHUDBorders(sbatch);
			}
#endif
		}

		protected abstract void DoDraw(IBatchRenderer sbatch, FRectangle bounds);

		protected virtual void DoDrawBackground(IBatchRenderer sbatch, FRectangle bounds)
		{
			/* OVERRIDE ME */
		}

		protected virtual void DrawDebugHUDBorders(IBatchRenderer sbatch)
		{
			sbatch.DrawRectangle(BoundingRectangle, Color.Magenta, 2f);
		}

		public virtual void Update(SAMTime gameTime, InputState istate)
		{
			if (PositionInvalidated) RecalculatePosition();

			for (int i = ActiveOperations.Count - 1; i >= 0; i--)
			{
				if (!ActiveOperations[i].Update(this, gameTime, istate))
				{
					ActiveOperations[i].OnEnd(this);
					ActiveOperations.RemoveAt(i);
				}
			}

			if (IsPointerDownOnElement && (!istate.IsRealDown || !BoundingRectangle.Contains(istate.PointerPosition)))
			{
				IsPointerDownOnElement = false;
			}

			DoUpdate(gameTime, istate);
		}

		public virtual void InvalidatePosition()
		{
			PositionInvalidated = true;
		}

		public virtual void Revalidate() // Manual RecalculatePosition
		{
			RecalculatePosition();
		}

		protected virtual void RecalculatePosition()
		{
			if (Owner == null) return;
			if (Owner.PositionInvalidated) return;

			OnBeforeRecalculatePosition();

			float px;
			float py;

			switch (Alignment)
			{
				case HUDAlignment.TOPLEFT:
				case HUDAlignment.BOTTOMLEFT:
				case HUDAlignment.CENTERLEFT:
					px = Owner.Left + RelativePosition.X;
					break;
				case HUDAlignment.TOPRIGHT:
				case HUDAlignment.BOTTOMRIGHT:
				case HUDAlignment.CENTERRIGHT:
					px = Owner.Right - Size.Width - RelativePosition.X;
					break;
				case HUDAlignment.TOPCENTER:
				case HUDAlignment.BOTTOMCENTER:
				case HUDAlignment.CENTER:
					px = Owner.CenterX - Size.Width / 2 + RelativePosition.X;
					break;
				case HUDAlignment.ABSOLUTE:
					px = RelativePosition.X;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			switch (Alignment)
			{
				case HUDAlignment.TOPLEFT:
				case HUDAlignment.TOPRIGHT:
				case HUDAlignment.TOPCENTER:
					py = Owner.Top + RelativePosition.Y;
					break;
				case HUDAlignment.BOTTOMLEFT:
				case HUDAlignment.BOTTOMRIGHT:
				case HUDAlignment.BOTTOMCENTER:
					py = Owner.Bottom - Size.Height - RelativePosition.Y;
					break;
				case HUDAlignment.CENTERLEFT:
				case HUDAlignment.CENTERRIGHT:
				case HUDAlignment.CENTER:
					py = Owner.CenterY - Size.Height / 2 + RelativePosition.Y;
					break;
				case HUDAlignment.ABSOLUTE:
					py = RelativePosition.Y;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			Position = new FPoint(px, py);

			BoundingRectangle = new FRectangle(Position, Size);

			PositionInvalidated = false;

			OnAfterRecalculatePosition();
		}

		public void AddHUDOperation(IHUDElementOperation op)
		{
			ActiveOperations.Add(op);
			op.OnStart(this);
		}

		public void AddHUDOperationSequence<TElement>(IHUDElementOperation op1, params IHUDElementOperation[] ops) where TElement : HUDElement
		{
			var seqop = new HUDSequenceElementOperation<TElement>(op1, ops);
			ActiveOperations.Add(seqop);
			seqop.OnStart(this);
		}

		public void AddCagedHUDOperationSequence<TElement>(Action<TElement> init, Action<TElement> finish, IHUDElementOperation op1, params IHUDElementOperation[] ops) where TElement : HUDElement
		{
			var seqop = new HUDSequenceElementOperation<TElement>(init, finish, op1, ops);
			ActiveOperations.Add(seqop);
			seqop.OnStart(this);
		}

		public bool HasHUDOperation<TType>()
		{
			return ActiveOperations.OfType<TType>().Any();
		}

		public void RemoveAllOperations<TType>()
		{
			RemoveAllOperations(p => p is TType);
		}

		public void RemoveAllOperations()
		{
			RemoveAllOperations(p => true);
		}

		public void RemoveAllOperations(Func<IHUDElementOperation, bool> condition)
		{
			for (int i = ActiveOperations.Count - 1; i >= 0; i--)
			{
				if (condition(ActiveOperations[i]))
				{
					ActiveOperations[i].OnEnd(this);
					ActiveOperations.RemoveAt(i);
				}
			}
		}

		public abstract void OnInitialize();
		public abstract void OnRemove();     // Only called on manual remove - not on HUD/Screen remove

		protected abstract void DoUpdate(SAMTime gameTime, InputState istate);

		public virtual bool InternalPointerDown(InputState istate)
		{
			if (!BoundingRectangle.Contains(istate.PointerPosition)) return false;

			var swallow = OnPointerDown(istate.PointerPosition.RelativeTo(Position), istate);

			if (swallow)
			{
				IsPointerDownOnElement = true;
				return true;
			}

			return false;
		}

		public virtual bool InternalPointerUp(InputState istate)
		{
			if (!BoundingRectangle.Contains(istate.PointerPosition)) return false;

			IsPointerDownOnElement = false;

			var mpos = istate.PointerPosition.RelativeTo(Position);

			var swallow = OnPointerUp(mpos, istate);


			if (swallow)
			{
				OnPointerClick(mpos, istate);

				return true;
			}

			return false;
		}

		protected virtual void OnBeforeRecalculatePosition()
		{
			/* OVERRIDE ME */
		}

		protected virtual void OnAfterRecalculatePosition()
		{
			/* OVERRIDE ME */
		}

		protected virtual bool OnPointerUp(FPoint relPositionPoint, InputState istate)
		{
			/* OVERRIDE ME */
			return false;
		}

		protected virtual bool OnPointerDown(FPoint relPositionPoint, InputState istate)
		{
			/* OVERRIDE ME */
			return false;
		}

		protected virtual void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			/* OVERRIDE ME */
		}

		protected virtual void OnEnabledChanged(bool newValue)
		{
			/* OVERRIDE ME */
		}
	}
}
