using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoSAMFramework.Portable.Input;
using System;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class GameHUDElement : ISAMDrawable, ISAMUpdateable
	{
		public GameHUD Owner = null; // Only set on add to HUD (the OnInitialize is called)
		public bool Alive = true;

		private Point _relativePosition = Point.Zero;
		public Point RelativePosition
		{
			get { return _relativePosition;}
			set { _relativePosition = value; RecalculatePosition(); }
		}

		private Size _size = Size.Empty;
		public Size Size
		{
			get { return _size; }
			set { _size = value; RecalculatePosition(); }
		}

		private HUDAlignment _alignment = HUDAlignment.TOPLEFT;
		public HUDAlignment Alignment
		{
			get { return _alignment; }
			set { _alignment = value; RecalculatePosition(); }
		}

		public Point Position { get; private set; } = Point.Zero;
		public Rectangle BoundingRectangle { get; private set; } = Rectangle.Empty;

		public void Remove()
		{
			Alive = false;
		}

		public void Initialize()
		{
			RecalculatePosition();
		}

		public void Draw(SpriteBatch sbatch)
		{
			
		}

		public void Update(GameTime gameTime, InputState istate)
		{

		}

		public void RecalculatePosition()
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
				default:
					throw new ArgumentOutOfRangeException();
			}

			BoundingRectangle = new Rectangle(Position, Size);
		}

		public abstract void OnInitialize();
		public abstract void OnRemove();
	}
}
