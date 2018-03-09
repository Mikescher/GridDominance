using System;
using System.Threading.Tasks;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SCCM;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM
{
	class SCCMTabHot : HUDContainer
	{
		public override int Depth => 0;

		private SCCMListPresenter _presenter;
		private SCCMListScrollbar _scrollbar;
		private HUDImage _waitingCog;

		public SCCMTabHot()
		{

		}

		public override void OnInitialize()
		{
			AddElement(_presenter = new SCCMListPresenter
			{
				Alignment = HUDAlignment.CENTERLEFT,
				RelativePosition = new FPoint(16, 0),
				Size = new FSize(Width - 16 - 16 - 48 - 16, Height - 16 - 16),
			});

			AddElement(_scrollbar = new SCCMListScrollbar
			{
				Alignment = HUDAlignment.CENTERRIGHT,
				RelativePosition = new FPoint(16, 0),
				Size = new FSize(48, Height - 16 - 16),
			});

			AddElement(_waitingCog = new HUDImage
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = FPoint.Zero,
				Image = Textures.CannonCogBig,
				RotationSpeed = 0.35f,
				Color = FlatColors.Clouds,
				Size = new FSize(192, 192)
			});

			_presenter.Load(QueryData, _scrollbar, _waitingCog);
		}

		private async Task<SCCMListPresenter.LoadFuncResult> QueryData(SCCMListPresenter list, int page, int reqid)
		{
			try
			{
				var r = await MainGame.Inst.Backend.QueryUserLevel(MainGame.Inst.Profile, QueryUserLevelCategory.HotLevels, string.Empty, page);

				if (r == null)
				{
					return SCCMListPresenter.LoadFuncResult.Error;
				}
				else
				{
					if (r.Count == 0)
					{
						return SCCMListPresenter.LoadFuncResult.LastPage;
					}
					else
					{
						MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
						{
							if (list.IsCurrentRequest(reqid))
							{
								foreach (var levelmeta in r)
								{
									_presenter.AddEntry(new SCCMListElementOnlinePlayable(levelmeta));
								}
							}
						});
						
						return SCCMListPresenter.LoadFuncResult.Success;
					}
				}
			}
			catch (Exception e)
			{
				SAMLog.Error("SCCMTH::QD", e);

				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					HUD.AddModal(new HUDFadeOutInfoBox(5, 2, 0.3f)
					{
						L10NText = L10NImpl.STR_CPP_COMERR,
						TextColor = FlatColors.Clouds,
						Background = HUDBackgroundDefinition.CreateRounded(FlatColors.Alizarin, 16),

						CloseOnClick = true,

					}, true);
				});
				
				return SCCMListPresenter.LoadFuncResult.Error;
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
