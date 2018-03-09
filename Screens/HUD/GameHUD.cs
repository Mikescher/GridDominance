using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Keyboard;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class GameHUD : ISAMDrawable, ISAMUpdateable
	{
		public const int MAX_TOAST_COUNT = 4;

		public readonly GameScreen Screen;
		protected readonly HUDRootContainer root;
		public readonly SpriteFont DefaultFont;

		private HUDKeyboard _keyboard = null;
		private readonly List<HUDToast> _toasts = new List<HUDToast>();

		protected GameHUD(GameScreen scrn, SpriteFont font)
		{
			Screen = scrn;
			DefaultFont = font;

			root = new HUDRootContainer { HUD = this };
			root.Initialize();
		}

		public float Left => -Screen.VAdapterHUD.VirtualGuaranteedBoundingsOffsetX;
		public float Top => -Screen.VAdapterHUD.VirtualGuaranteedBoundingsOffsetY;

		public float Right => Screen.VAdapterHUD.VirtualTotalWidth - Screen.VAdapterHUD.VirtualGuaranteedBoundingsOffsetX;
		public float Bottom => Screen.VAdapterHUD.VirtualTotalHeight - Screen.VAdapterHUD.VirtualGuaranteedBoundingsOffsetY;

		public float Width => Right - Left;
		public float Height => Bottom - Top;

		public float CenterX => Left + Width/2;
		public float CenterY => Top + Height / 2;

		public float PixelWidth => Width * 1f / Screen.VAdapterHUD.RealTotalWidth;

		private HUDElement _focusedElement = null;
		public HUDElement FocusedElement
		{
			get { return _focusedElement; }
			set
			{
				if (_focusedElement == value) return;

				_focusedElement?.FocusLoose();
				value?.FocusGain();

				_focusedElement = value;
			}
		}

		public IEnumerable<HUDElement> Enumerate() => Enumerable.Repeat(root, 1).Concat(root.EnumerateElements());

		public void Update(SAMTime gameTime, InputState istate)
		{
			root.Update(gameTime, istate);

			OnUpdate(gameTime, istate);

			if (FocusedElement != null && !FocusedElement.Alive) FocusedElement = null;

			UpdateToasts(gameTime);
		}

		protected virtual void OnUpdate(SAMTime gameTime, InputState istate)
		{
			// override me
		}

		public void Draw(IBatchRenderer sbatch)
		{
			if (root.IsVisible)
			{
				root.DrawBackground(sbatch);
				root.DrawForeground(sbatch);
			}
		}

		public void AddModal(HUDElement e, bool closeOnOutOfBoundsOrBackKey, float dim = 0f, float dimTime = 0f)
		{
			root.AddElement(new HUDModalDialog(root.NonToastChildrenMaxDepth + 1, e, dim, dimTime, closeOnOutOfBoundsOrBackKey));
		}

		public HUDModalDialog GetCurrentModalDialog()
		{
			return root.Children.OfType<HUDModalDialog>().FirstOrDefault();
		}

		public void AddElement(HUDElement e)
		{
			root.AddElement(e);
		}

		public void AddElements(IEnumerable<HUDElement> es)
		{
			root.AddElements(es);
		}

		public void RecalculateAllElementPositions()
		{
			root.InvalidatePosition();
		}

		public int FlatCount()
		{
			return root.ChildrenCount;
		}

		public int DeepCount()
		{
			return root.DeepInclusiveCount;
		}

		public void HideKeyboard()
		{
			_keyboard?.Remove();
			_keyboard = null;
		}

		public HUDKeyboard ShowKeyboard(IKeyboardListener reciever)
		{
			if (_keyboard != null) HideKeyboard();

			_keyboard = new HUDKeyboard(reciever);

			AddModal(_keyboard, true);

			return _keyboard;
		}

		private void UpdateToasts(SAMTime gameTime)
		{
			lock (_toasts)
			{
				for (int i = _toasts.Count - 1; i >= 0; i--)
				{
					if (_toasts[i] == null)
					{
						SAMLog.Error("GHUD::NPE_TOAST_1", $"_toast {i} == null");
						_toasts.RemoveAt(i);
						continue;
					}

					if (!_toasts[i].Alive) _toasts.RemoveAt(i);
				}

				float px = HUDToast.PAD_BOTTOM;
				foreach (var xtoast in _toasts)
				{
					if (xtoast == null)
					{
						SAMLog.Error("GHUD::NPE_TOAST_2", "xtoast == null");
						continue;
					}

					xtoast.PositionY.Set(px);
					px += xtoast.Height + HUDToast.PAD_VERT;
				}
			}
		}

		public HUDToast ShowToast(string id, string text, int size, Color background, Color foreground, float lifetime)
		{
			lock (_toasts)
			{
				while (_toasts.Count >= MAX_TOAST_COUNT)
				{
					_toasts[0].Alive = false;
					_toasts.RemoveAt(0);
				}

				if (id != null)
				{
					foreach (var xtoast in _toasts)
					{
						if (xtoast.ToastID == id)
						{
							xtoast.Reset(text, background, foreground, lifetime);
							return xtoast;
						}
					}
				}

				float px = HUDToast.PAD_BOTTOM;
				foreach (var xtoast in _toasts)
				{
					xtoast.PositionY.SetForce(px);
					px += xtoast.Height + HUDToast.PAD_VERT;
				}

				var toast = new HUDToast(id, lifetime, px);

				toast.Text = text;
				toast.Alignment = HUDAlignment.BOTTOMCENTER;
				toast.RelativePosition = new FPoint(0, px);
				toast.FontSize = size;
				toast.Font = DefaultFont;
				toast.TextColor = foreground;
				toast.Background = HUDBackgroundDefinition.CreateSimpleBlur(background, size / 4f);
				toast.TextPadding = new FSize(size / 5f, size / 5f);
				toast.MaxWidth = Width * 0.8f;
				toast.WordWrap = HUDWordWrap.WrapByWordTrusted;

				AddElement(toast);
				_toasts.Add(toast);
				return toast;
			}
		}

		public void Validate()
		{
			root.ValidateRecursive();
		}

		public void CopyToast(GameHUD phud)
		{
			lock (_toasts)
			{
				bool postreset = _toasts.Any();

				foreach (var t in phud._toasts)
				{
					var copy = HUDToast.Copy(t);
					AddElement(copy);
					_toasts.Add(copy);
				}

				if (postreset)
				{
					float px = HUDToast.PAD_BOTTOM;
					foreach (var xtoast in _toasts)
					{
						xtoast.PositionY.SetForce(px);
						px += xtoast.Height + HUDToast.PAD_VERT;
					}
				}
			}
		}
	}
}
