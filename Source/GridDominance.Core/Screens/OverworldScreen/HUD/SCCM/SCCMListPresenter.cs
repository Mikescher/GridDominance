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
		public override int Depth => 0;

		public int Offset = 0;
		public SCCMListScrollbar Scrollbar;

		private List<SCCMListElement> _entries = new List<SCCMListElement>();
		private bool relayout = false;

		public SCCMListPresenter()
		{

		}

		public override void OnInitialize()
		{

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
			if (Scrollbar!=null) Scrollbar.Position = Offset;
			if (Scrollbar != null) Scrollbar.Max = ChildrenCount;

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
			foreach (var child in _entries)
			{
				child.IsVisible = true;
				child.RelativePosition = new FPoint(0, i * (((Height - 5 * 16) / 6) + 16));
				child.Size = new FSize(Width, (Height - 5 * 16) / 6);
				child.Alignment = HUDAlignment.TOPLEFT;

				i++;
			}
		}

		public void Scroll(int delta)
		{
			relayout = true;
		}

		public void AddEntry(SCCMListElement e)
		{
			e.Size = new FSize(Width, (Height - 5 * 16) / 6);
			e.Alignment = HUDAlignment.TOPLEFT;

			AddElement(e);
			_entries.Add(e);
			relayout = true;
		}
	}
}
