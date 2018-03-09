using System;
using GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.Operations
{
	class SCCMListDragAgent : SAMUpdateOp<SCCMListPresenter>
	{
		public override string Name => "SCCMList_Drag";
		
		private bool _isDragging = false;
		private FPoint _mouseStartPos;
		private int _startOffset;

		protected override void OnUpdate(SCCMListPresenter owner, SAMTime gameTime, InputState istate)
		{
			if (_isDragging)
			{
				if (istate.IsRealDown)
				{
					UpdateDrag(owner, gameTime, istate);
				}
				else
				{
					EndDrag(owner, gameTime, istate);
				}
			}
			else
			{
				if (istate.IsExclusiveJustDown && owner.BoundingRectangle.Contains(istate.HUDPointerPosition))
				{
					istate.Swallow(InputConsumer.HUDElement);
					StartDrag(owner, istate);
				}
			}
		}

		public void StartDrag(SCCMListPresenter owner, InputState istate)
		{
			_isDragging = true;
			_mouseStartPos = istate.HUDPointerPosition;
			_startOffset = owner.Offset;
		}

		private void UpdateDrag(SCCMListPresenter owner, SAMTime gameTime, InputState istate)
		{
			var delta = FloatMath.Round((_mouseStartPos - istate.HUDPointerPosition).Y / owner.EntryDistance);

			owner.SetOffset(_startOffset + delta);

		}

		private void EndDrag(SCCMListPresenter owner, SAMTime gameTime, InputState istate)
		{
			UpdateDrag(owner, gameTime, istate);
			_isDragging = false;
		}
	}
}
