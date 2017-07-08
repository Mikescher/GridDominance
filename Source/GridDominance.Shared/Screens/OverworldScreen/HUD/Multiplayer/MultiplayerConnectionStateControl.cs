using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Network.Multiplayer;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.Multiplayer
{
	class MultiplayerConnectionStateControl : HUDLayoutContainer
	{
		public override int Depth { get; }

		private const float ROTATION_SPEED = 0.3f; // rot/sec

		private readonly GDMultiplayerCommon _connection;

		private readonly HUDLabel _label;

		private float _rotation = 0f;

		public MultiplayerConnectionStateControl(GDMultiplayerCommon conn, int depth = 0)
		{
			Depth = depth;

			Size = new FSize(512, 32);

			_connection = conn;

			_label = new HUDLabel
			{
				Alignment = HUDAlignment.TOPLEFT,
				RelativePosition = new FPoint(4 + 24 + 8, 0),
				Size = new FSize(480, 32),
				FontSize = 32,
				Font = Textures.HUDFontBold,
				TextColor = FlatColors.Foreground,
				Text = "Hello World",
			};

			AddElement(_label);
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			base.DoUpdate(gameTime, istate);

			switch (_connection.ConnState)
			{
				case SAMNetworkConnection.ConnectionState.Offline:
					_label.L10NText = L10NImpl.STR_MP_OFFLINE;
					break;
				case SAMNetworkConnection.ConnectionState.Connected:
					_label.L10NText = L10NImpl.STR_MP_ONLINE;
					break;
				case SAMNetworkConnection.ConnectionState.Reconnecting:
					_label.L10NText = L10NImpl.STR_MP_CONNECTING;
					break;
			}

			_rotation += gameTime.ElapsedSeconds * ROTATION_SPEED * FloatMath.TAU;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			base.DoDraw(sbatch, bounds);

			switch (_connection.ConnState)
			{
				case SAMNetworkConnection.ConnectionState.Offline:
					sbatch.DrawCentered(Textures.TexIconConnection0, bounds.TopLeft + new Vector2(4 + 12, 4 + 12), 24, 24, Color.White);
					break;
				case SAMNetworkConnection.ConnectionState.Connected:
					sbatch.DrawCentered(Textures.TexIconConnection1, bounds.TopLeft + new Vector2(4 + 12, 4 + 12), 24, 24, Color.White);
					break;
				case SAMNetworkConnection.ConnectionState.Reconnecting:
					sbatch.DrawCentered(Textures.TexIconConnection2, bounds.TopLeft + new Vector2(4 + 12, 4 + 12), 24, 24, Color.White);
					sbatch.DrawCentered(Textures.TexIconConnection3, bounds.TopLeft + new Vector2(4 + 12, 4 + 12), 24, 24, Color.White, _rotation);
					break;
			}

		}
	}
}
