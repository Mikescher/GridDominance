using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDominance.Shared.Screens.OverworldScreen.HUD.Operations;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM.Elements
{
	class SCCMListPresenter : HUDContainer
	{
		private const float ENTRY_HEIGHT = 66.666f;
		private const float PAD_MIN = 12f;

		public enum LoadFuncResult { Success, LastPage, Error }
		public enum PresenterMode { Initial, Loading, Normal, FullyLoaded }

		public override int Depth => 0;

		public int Offset = 0;
		private SCCMListScrollbar _scrollbar;
		private HUDImage _waitingCog;

		private readonly List<SCCMListElement> _entries = new List<SCCMListElement>();
		private bool relayout = false;

		private int _entryCount;
		private float _pad;

		public float EntryDistance => ENTRY_HEIGHT+_pad;

		public float MaxOffset => _entries.Count - _entryCount;

		private SCCMListDragAgent _dragAgent;

		private volatile PresenterMode _mode = PresenterMode.Initial;
		private Func<SCCMListPresenter, int, int, Task<LoadFuncResult>> _loadFunc;
		private volatile int _currentPage = -1;
		private int _currentRequest = 101;

		public SCCMListPresenter()
		{

		}

		public override void OnInitialize()
		{
			_entryCount = (int)((Height + PAD_MIN)/(ENTRY_HEIGHT + PAD_MIN));
			_pad = (Height - _entryCount * ENTRY_HEIGHT) / (_entryCount - 1);

			AddOperation(_dragAgent = new SCCMListDragAgent());
		}
		
		public void Load(Func<SCCMListPresenter, int, int, Task<LoadFuncResult>> loadFunc, SCCMListScrollbar sb, HUDImage wc, bool async = true)
		{
			Clear();
			_currentRequest++;

			_scrollbar = sb;
			_waitingCog = wc;

			_scrollbar.Presenter = this;

			this.IsVisible = false;
			_scrollbar.IsVisible = false;
			_waitingCog.IsVisible = true;

			_loadFunc = loadFunc;

			_mode = PresenterMode.Loading;
			if (async) 
				DoLoad(0, _currentRequest, true).RunAsync();
			else 
				DoLoad(0, _currentRequest, false).Wait();
		}

		private async Task DoLoad(int page, int reqid, bool async)
		{
			_mode = PresenterMode.Loading;
			var r = await _loadFunc(this, page, reqid);

			void FinishAction()
			{
				if (IsCurrentRequest(reqid))
				{
					switch (r)
					{
						case LoadFuncResult.Success:
							_mode = PresenterMode.Normal;
							_currentPage = page;
							break;
						case LoadFuncResult.LastPage:
							_mode = PresenterMode.FullyLoaded;
							_currentPage = page;
							break;
						case LoadFuncResult.Error:
							_mode = PresenterMode.Normal;
							break;
						default:
							SAMLog.Error("SCCMLP::EnumSwitch_DoLoad", "r = " + r);
							break;
					}

					this.IsVisible = true;
					_scrollbar.IsVisible = true;
					_waitingCog.IsVisible = false;
				}
			}

			if (async)
				MainGame.Inst.DispatchBeginInvoke(FinishAction);
			else
				FinishAction();
		}

		public bool IsCurrentRequest(int reqid)
		{
			return _currentRequest == reqid;
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => IsVisible;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate)
		{
			if (!IsVisible) return false;

			_dragAgent?.StartDrag(this, istate);

			return true;
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
			if (_scrollbar != null) _scrollbar.ScrollPosition = Offset;
			if (_scrollbar != null) _scrollbar.ScrollMax      = ChildrenCount;
			if (_scrollbar != null) _scrollbar.ScrollPageSize = _entryCount;


			if (relayout)
			{
				RelayoutEntries();
			}
		}

		private void RelayoutEntries()
		{
			foreach (var child in _entries)
			{
				child.IsVisible = false;
				child.RelativePosition = FPoint.Zero;
			}

			int i = 0;
			foreach (var child in _entries.Skip(Offset).Take(_entryCount))
			{
				child.IsVisible = true;
				child.RelativePosition = new FPoint(0, i * (ENTRY_HEIGHT + _pad));
				child.Size = new FSize(Width, ENTRY_HEIGHT);
				child.Alignment = HUDAlignment.TOPLEFT;

				i++;
			}
		}

		public void Scroll(int delta, bool allowNextPage)
		{
			SetOffset(Offset+delta, allowNextPage);
		}

		public void SetOffset(int o, bool allowNextPage)
		{
			var prev = Offset;

			Offset = o;
			if (Offset < 0) Offset = 0;
			if (Offset > _entries.Count - _entryCount)
			{
				if (_mode == PresenterMode.Normal && allowNextPage)
				{
					LoadNextPage();
				}

				Offset = _entries.Count - _entryCount;
			}

			if (prev != Offset) relayout = true;
		}

		private void LoadNextPage()
		{
			AddEntry(new SCCMListElementLoading());

			_currentRequest++;
			DoLoad(_currentPage+1, _currentRequest, true)
				.ContinueWith((a) => MainGame.Inst.DispatchBeginInvoke(RemoveWaiter))
				.RunAsync();
		}

		private void RemoveWaiter()
		{
			var waiter = _entries.OfType<SCCMListElementLoading>().ToList();
			foreach (var w in waiter)
			{
				_entries.Remove(w);
				w.Remove();
			}
			SetOffset(Offset, false);
		}

		public void AddEntry(SCCMListElement e)
		{
			e.Size = new FSize(Width, ENTRY_HEIGHT);
			e.Alignment = HUDAlignment.TOPLEFT;

			AddElement(e);
			_entries.Add(e);
			relayout = true;
		}

		public void Clear()
		{
			foreach (var child in _entries) child.Remove();
			Offset = 0;
			_entries.Clear();

			relayout = true;
		}
	}
}
