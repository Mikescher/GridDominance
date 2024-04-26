using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using MonoSAMFramework.Portable.UpdateAgents;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class HUDElement : ISAMLayeredDrawable, ISAMUpdateable, IUpdateOperationOwner
	{
		public HUDContainer Owner = null; // Only set on add to HUD (the OnInitialize is called)
		public GameHUD HUD = null;        // Only set on add to HUD (the OnInitialize is called)

		private const int MAX_UPDATES_BY_INIT = 3;

		public bool Alive = true;
		public bool Initialized { get; private set; } = false;
		public bool IsVisible = true;

		public virtual int DeepInclusiveCount => 1;

		private FPoint _relativePosition = FPoint.Zero;
		public FPoint RelativePosition
		{
			get { return _relativePosition;}
			set { if (value != _relativePosition) {_relativePosition = value; InvalidatePosition();} }
		}

		private FSize _size = FSize.Empty;
		public FSize Size
		{
			get { return _size; }
			set { if (value != _size) {_size = value; InvalidatePosition();} }
		}

		private HUDAlignment _alignment = HUDAlignment.TOPLEFT;
		public HUDAlignment Alignment
		{
			get { return _alignment; }
			set {if (value != _alignment) {_alignment = value; InvalidatePosition();} }
		}

		public FPoint Position { get; protected set; } = FPoint.Zero; // _Not_ Center
		public FRectangle BoundingRectangle { get; protected set; } = FRectangle.Empty;

		public FPoint RelativeCenter
		{
			get => RelativePosition + Size/2;
			set => RelativePosition = value - Size/2;
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

		public float RelativeTop  => RelativePosition.Y;
		public float RelativeLeft => RelativePosition.X;

		public float RelativeBottom => RelativePosition.Y + Height;
		public float RelativeRight  => RelativePosition.X + Width;

		public float Width => BoundingRectangle.Width;
		public float Height => BoundingRectangle.Height;

		public float CenterX => Position.X + Size.Width / 2f;
		public float CenterY => Position.Y + Size.Height / 2f;

		public FPoint Center    => new FPoint(CenterX, CenterY);

		protected readonly List<IUpdateOperation> ActiveOperations = new List<IUpdateOperation>();

		public IEnumerable<IUpdateOperation> ActiveHUDOperations => ActiveOperations;

		public bool IsPointerDownOnElement = false;

		protected bool PositionInvalidated = false;
		private InputState _lastInputState = null;

		public bool Focusable = true;
		public bool IsFocused => HUD != null && HUD.FocusedElement == this;

		public abstract int Depth { get; }// higher depth => in foreground

		protected HUDElement()
		{
			//
		}

		public virtual void Remove()
		{
			Alive = false;
			if (HUD.FocusedElement == this) HUD.FocusedElement = null;
		}

		public void Initialize()
		{
			Initialized = true;
			if (Owner != null) _lastInputState = Owner._lastInputState;

			InvalidatePosition();

			BoundingRectangle = new FRectangle(Position, Size); // fix that Width+Height is set in OnInintialize, if Size is set in ctr
			OnInitialize();

			if (_lastInputState != null)
			{
				int initSteps = MAX_UPDATES_BY_INIT;
				while (PositionInvalidated && initSteps-- > 0)
				{
					Update(MonoSAMGame.CurrentTime, _lastInputState);
				}
			}
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
			_lastInputState = istate;

			if (PositionInvalidated) RecalculatePosition();

			for (int i = ActiveOperations.Count - 1; i >= 0; i--)
			{
				if (!ActiveOperations[i].UpdateUnchecked(this, gameTime, istate))
				{
					ActiveOperations.RemoveAt(i);
				}
			}

			if (IsPointerDownOnElement && (!istate.IsRealDown || (!BoundingRectangle.Contains(istate.HUDPointerPosition) && !RegisterAllClickEvents())))
			{
				IsPointerDownOnElement = false;
			}

			DoUpdate(gameTime, istate);
		}

		public virtual void InvalidatePosition()
		{
			PositionInvalidated = true;
		}

		public virtual void Revalidate() // Manual RecalculatePosition | RecalculatePosition of Container children
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
				case HUDAlignment.ABSOLUTE_VERTCENTERED:
					px = RelativePosition.X;
					break;
				case HUDAlignment.ABSOLUTE_BOTHCENTERED:
				case HUDAlignment.ABSOLUTE_HORZCENTERED:
					px = RelativePosition.X - Size.Width / 2;
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
				case HUDAlignment.ABSOLUTE_HORZCENTERED:
					py = RelativePosition.Y;
					break;
				case HUDAlignment.ABSOLUTE_BOTHCENTERED:
				case HUDAlignment.ABSOLUTE_VERTCENTERED:
					py = RelativePosition.Y - Size.Height / 2;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			Position = new FPoint(px, py);

			BoundingRectangle = new FRectangle(Position, Size);

			PositionInvalidated = false;

			OnAfterRecalculatePosition();
		}

		public void AddOperation(IUpdateOperation op)
		{
			ActiveOperations.Add(op);
			op.InitUnchecked(this);
		}

		public void AddOperationDelayed<TElement>(SAMUpdateOp<TElement> op, float delay) where TElement : HUDElement
		{
			AddOperation(new DelayedOperation<TElement>(op, delay));
		}
		
		public void AddOperationSequence<TElement>(IUpdateOperation op1, params IUpdateOperation[] ops) where TElement : HUDElement
		{
			AddOperation(new SequenceOperation<TElement>(op1, ops));
		}

		public void AddCagedOperationSequence<TElement>(Action<TElement> init, Action<TElement> finish, IUpdateOperation op1, params IUpdateOperation[] ops) where TElement : HUDElement
		{
			AddOperation(new SequenceOperation<TElement>(init, finish, op1, ops));
		}

		public bool HasOperation<TType>()
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

		public void RemoveAllOperations(Func<IUpdateOperation, bool> condition)
		{
			for (int i = ActiveOperations.Count - 1; i >= 0; i--)
			{
				if (condition(ActiveOperations[i]))
				{
					ActiveOperations[i].Abort();
				}
			}
		}

		public virtual IEnumerable<HUDElement> EnumerateElements()
		{
			return Enumerable.Empty<HUDElement>();
		}

		public abstract void OnInitialize();
		public abstract void OnRemove();     // Only called on manual remove - not on HUD/Screen remove

		protected abstract void DoUpdate(SAMTime gameTime, InputState istate);

		public virtual bool InternalPointerDown(InputState istate)
		{
			if (!BoundingRectangle.Contains(istate.HUDPointerPosition) && !RegisterAllClickEvents()) return false;

			var swallow = OnPointerDown(istate.HUDPointerPosition.RelativeTo(Position), istate);

			if (swallow)
			{
				if (Focusable) HUD.FocusedElement = this;
				IsPointerDownOnElement = true;
				return true;
			}

			return false;
		}

		public virtual bool InternalPointerUp(InputState istate)
		{
			if (!BoundingRectangle.Contains(istate.HUDPointerPosition) && !RegisterAllClickEvents()) return false;

			var wasDown = IsPointerDownOnElement;

			IsPointerDownOnElement = false;

			var mpos = istate.HUDPointerPosition.RelativeTo(Position);

			var swallow = OnPointerUp(mpos, istate);


			if (swallow && wasDown)
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

		public virtual void FocusGain()
		{
			/* OVERRIDE ME */
		}

		public virtual void FocusLoose()
		{
			/* OVERRIDE ME */
		}

		public virtual void ValidateRecursive()
		{
			if (PositionInvalidated) RecalculatePosition();
		}

		protected virtual bool RegisterAllClickEvents() => false; // OVERRIDE ME
	}
}
