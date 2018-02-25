using GridDominance.Shared.SCCM;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM
{
	class SCCMTabMyLevels : HUDContainer
	{
		public override int Depth => 0;

		private SCCMListPresenter _presenter;
		private SCCMListScrollbar _scrollbar;

		public SCCMTabMyLevels()
		{
			//TODO stuff for userid changes
			// - remeber userid in not uploaded (only show level with my userid)
			// - remeber userid in uploaded     (only show level with my userid)
		}

		public override void OnInitialize()
		{
			AddElement(_presenter = new SCCMListPresenter
			{
				Alignment = HUDAlignment.CENTERLEFT,
				RelativePosition = new FPoint(16, 0),
				Size = Size - new FSize(16 + 16 + 48 + 16, Height - 16 - 16),
			});

			AddElement(_scrollbar = new SCCMListScrollbar
			{
				Alignment = HUDAlignment.CENTERRIGHT,
				RelativePosition = new FPoint(16, 0),
				Size = new FSize(48, Height - 16 - 16),
			});

			_presenter.Scrollbar = _scrollbar;
			_scrollbar.Presenter = _presenter;

			_presenter.AddEntry(new SCCMListElementNewUserLevel());

			foreach (var userlevel in SCCMUtils.ListUserLevelsUnfinished())
			{
				_presenter.AddEntry(new SCCMListElementEditable(userlevel));
			}

			foreach (var userlevel in SCCMUtils.ListUserLevelsFinished())
			{
				_presenter.AddEntry(new SCCMListElementLocalPlayable(userlevel)); //TODO update meta from online
			}
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}
	}
}
