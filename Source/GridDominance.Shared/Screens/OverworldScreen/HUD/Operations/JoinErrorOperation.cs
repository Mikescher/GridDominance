using GridDominance.Shared.Screens.OverworldScreen.HUD;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Operations;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;

namespace GridDominance.Shared.Screens.Common.HUD.HUDOperations
{
	class JoinErrorOperation : HUDTimedElementOperation<MultiplayerJoinLobbyScreen>
	{
		public override string Name => "JoinErrorOperation";

		private FPoint realPos;

		public JoinErrorOperation() : base(0.85f)
		{

		}

		protected override void OnStart(MultiplayerJoinLobbyScreen element)
		{
			realPos = element.RelativePosition;

			for (int i = 0; i < element.CharDisp.Length; i++) element.CharDisp[i].Background = element.CharDisp[i].Background.WithColor(FlatColors.Alizarin);
		}

 		protected override void OnProgress(MultiplayerJoinLobbyScreen element, float progress, InputState istate)
		{
			var off = Vector2.UnitX * (FloatMath.Sin(progress * FloatMath.TAU * 6) * 32) * (1 - FloatMath.FunctionEaseInCubic(progress));

			element.RelativePosition = realPos + off;
			
			element.HUD.Validate();
		}

		protected override void OnEnd(MultiplayerJoinLobbyScreen element)
		{
			element.Remove();
			element.HUD.AddModal(new MultiplayerJoinLobbyScreen(), true, 0.5f);
		}
	}
}