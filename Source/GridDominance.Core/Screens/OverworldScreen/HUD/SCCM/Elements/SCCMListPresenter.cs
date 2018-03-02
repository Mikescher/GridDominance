using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM
{
	class SCCMListPresenter : HUDContainer
	{
		private const float ENTRY_HEIGHT = 66.666f;
		private const float PAD_MIN = 12f;

		public override int Depth => 0;

		public int Offset = 0;
		public SCCMListScrollbar Scrollbar;

		private readonly List<SCCMListElement> _entries = new List<SCCMListElement>();
		private bool relayout = false;

		private int _entryCount;
		private float _pad;

		public SCCMListPresenter()
		{

		}

		public override void OnInitialize()
		{
			_entryCount = (int)((Height + PAD_MIN)/(ENTRY_HEIGHT + PAD_MIN));
			_pad = (Height - _entryCount * ENTRY_HEIGHT) / (_entryCount - 1);
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
			if (Scrollbar != null) Scrollbar.ScrollPosition = Offset;
			if (Scrollbar != null) Scrollbar.ScrollMax      = ChildrenCount;
			if (Scrollbar != null) Scrollbar.ScrollPageSize = _entryCount;


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

		public void Scroll(int delta)
		{
			Offset += delta;
			if (Offset < 0) Offset = 0;
			if (Offset + (_entryCount-1) >= _entries.Count) Offset = _entries.Count - _entryCount;
			relayout = true;
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
