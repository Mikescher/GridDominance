#if DEBUG
//#define DEBUG_HUDBOUNDS
#endif

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using System;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class HUDElement : ISAMDrawable, ISAMUpdateable
	{
		public GameHUD Owner = null; // Only set on add to HUD (the OnInitialize is called)
		public bool Alive = true;

		private Point _relativePosition = Point.Zero;
		public Point RelativePosition
		{
			get { return _relativePosition;}
			set { _relativePosition = value; RecalculatePositionLater(); }
		}

		private Size _size = Size.Empty;
		public Size Size
		{
			get { return _size; }
			set { _size = value; RecalculatePositionLater(); }
		}

		private HUDAlignment _alignment = HUDAlignment.TOPLEFT;
		public HUDAlignment Alignment
		{
			get { return _alignment; }
			set { _alignment = value; RecalculatePositionLater(); }
		}

		public Point Position { get; private set; } = Point.Zero;
		public Rectangle BoundingRectangle { get; private set; } = Rectangle.Empty;

		public Vector2 RelativeCenter
		{
			get { return new Vector2(RelativePosition.X + Size.Width / 2f, RelativePosition.Y + Size.Height / 2f);}
			set { RelativePosition = new Point((int) (value.X - Size.Width / 2f), (int) (value.Y - Size.Height / 2f));}
		}

		public Vector2 Center => new Vector2(Position.X + Size.Width / 2f, Position.Y + Size.Height / 2f);

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
			RecalculatePositionLater();

			OnInitialize();
		}

		public void DrawBackground(SpriteBatch sbatch)
		{
			DoDrawBackground(sbatch, BoundingRectangle);
		}

		public void Draw(SpriteBatch sbatch)
		{
			DoDraw(sbatch, BoundingRectangle);

#if DEBUG
			if (DebugSettings.Get("DebugHUDBorders"))
			{
				DrawDebugHUDBorders(sbatch);
			}
#endif

		}

		protected virtual void DrawDebugHUDBorders(SpriteBatch sbatch)
		{
			sbatch.DrawRectangle(BoundingRectangle, Color.Magenta, 2f);
		}

		public virtual void Update(GameTime gameTime, InputState istate)
		{
			if (PositionInvalidated) RecalculatePosition();


			if (istate.IsJustDown && BoundingRectangle.Contains(istate.PointerPosition))
			{
				OnPointerDown(istate.PointerPosition - Position, istate);
				IsPointerDownOnElement = true;
			}
			else if (istate.IsJustUp && BoundingRectangle.Contains(istate.PointerPosition))
			{
				OnPointerUp(istate.PointerPosition - Position, istate);

				if (IsPointerDownOnElement)
					OnPointerClick(istate.PointerPosition - Position, istate);
			}

			if (!istate.IsDown) IsPointerDownOnElement = false;

			DoUpdate(gameTime, istate);
		}

		public void RecalculatePositionLater()
		{
			PositionInvalidated = true;
		}

		private void RecalculatePosition()
		{
			if (Owner == null) return;

			switch (Alignment)
			{
				case HUDAlignment.TOPLEFT:
					Position = new Point(Owner.Left + RelativePosition.X, Owner.Top + RelativePosition.Y);
					break;
				case HUDAlignment.TOPRIGHT:
					Position = new Point(Owner.Right - Size.Width - RelativePosition.X, Owner.Top + RelativePosition.Y);
					break;
				case HUDAlignment.BOTTOMLEFT:
					Position = new Point(Owner.Left + RelativePosition.X, Owner.Bottom - Size.Height - RelativePosition.Y);
					break;
				case HUDAlignment.BOTTOMRIGHT:
					Position = new Point(Owner.Right - Size.Width - RelativePosition.X, Owner.Bottom - Size.Height - RelativePosition.Y);
					break;
				case HUDAlignment.CENTER:
					Position = new Point(Owner.CenterX - Size.Width/2 + RelativePosition.X, Owner.CenterY - Size.Height / 2 + RelativePosition.Y);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			BoundingRectangle = new Rectangle(Position, Size);

			PositionInvalidated = false;
		}

		public abstract void OnInitialize();
		public abstract void OnRemove();

		protected abstract void DoDraw(SpriteBatch sbatch, Rectangle bounds);
		protected virtual void DoDrawBackground(SpriteBatch sbatch, Rectangle bounds) { }
		protected abstract void DoUpdate(GameTime gameTime, InputState istate);

		protected virtual void OnPointerUp(Point relPositionPoint, InputState istate) { /* OVERRIDE ME */ }
		protected virtual void OnPointerDown(Point relPositionPoint, InputState istate) { /* OVERRIDE ME */ }
		protected virtual void OnPointerClick(Point relPositionPoint, InputState istate) { /* OVERRIDE ME */ }
	}
}
