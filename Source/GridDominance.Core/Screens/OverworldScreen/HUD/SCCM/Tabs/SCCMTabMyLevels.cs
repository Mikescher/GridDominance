using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Network.Backend;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM.Elements;
using GridDominance.Shared.SCCM;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM.Tabs
{
	class SCCMTabMyLevels : HUDContainer
	{
		public override int Depth => 0;

		private SCCMListPresenter _presenter;
		private SCCMListScrollbar _scrollbar;
		private HUDImage _waitingCog;

		public SCCMTabMyLevels()
		{
			//
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

			_presenter.Load(QueryData, _scrollbar, _waitingCog, false);
		}

		private Task<SCCMListPresenter.LoadFuncResult> QueryData(SCCMListPresenter list, int page, int reqid)
		{
			list.AddEntry(new SCCMListElementNewUserLevel());

			foreach (var userlevel in SCCMUtils.ListUserLevelsUnfinished())
			{
				if (userlevel.AuthorUserID != MainGame.Inst.Profile.OnlineUserID) continue;
				list.AddEntry(new SCCMListElementEditable(userlevel));
			}

			var localLevels = new List<Tuple<long, string, SCCMListElementLocalPlayable, string>>();

			foreach (var userleveltuple in SCCMUtils.ListUserLevelsFinished())
			{
				var filename = userleveltuple.Item1;
				var userlevel = userleveltuple.Item2;
				var levelhash = userleveltuple.Item3;

				if (userlevel.CustomMeta_UserID == MainGame.Inst.Profile.OnlineUserID)
				{
					var entry = new SCCMListElementLocalPlayable(userlevel);
					localLevels.Add(Tuple.Create(userlevel.CustomMeta_LevelID, levelhash, entry, filename));

					list.AddEntry(entry);
				}
				else
				{
					SAMLog.Info("SCCMTML::DelFinLevel", $"Level {userlevel.UniqueID:B} deleted cause wrong username {userlevel.CustomMeta_UserID} <> {MainGame.Inst.Profile.OnlineUserID}");
					SCCMUtils.DeleteUserLevelFinished(filename);
				}
			}
				
			QueryMetaFromServer(localLevels).EnsureNoError();

			return Task.FromResult(SCCMListPresenter.LoadFuncResult.LastPage);
		}

		private async Task QueryMetaFromServer(List<Tuple<long, string, SCCMListElementLocalPlayable, string>> userlevelsLocal)
		{
			var userlevel_online = await MainGame.Inst.Backend.QueryUserLevel(MainGame.Inst.Profile, QueryUserLevelCategory.AllLevelsOfUserid, MainGame.Inst.Profile.OnlineUserID.ToString(), 0);

			var redownload = new List<Tuple<long, SCCMLevelMeta>>();

			foreach (var lvlonline in userlevel_online)
			{
				var match = userlevelsLocal.FirstOrDefault(loc => loc.Item1==lvlonline.OnlineID);
				if (match != null)
				{
					if (lvlonline.Hash.ToUpper() != match.Item2.ToUpper())
					{
						// Hash mismatch - redownload

						MainGame.Inst.DispatchBeginInvoke(() => 
						{
							match.Item3.Remove();
							SCCMUtils.DeleteUserLevelFinished(match.Item4);
						});
						
						SAMLog.Info("SCCMTML::QMFS-1", $"Hash-mismatch local user level {lvlonline.OnlineID} - redownload");

						redownload.Add(Tuple.Create(lvlonline.OnlineID, lvlonline));
					}
					else
					{
						// all ok - update meta

						MainGame.Inst.DispatchBeginInvoke(() => 
						{
							match.Item3.SetMeta(lvlonline);
						});
					}
				}
				else
				{
					// missing - download

					SAMLog.Info("SCCMTML::QMFS-2", $"Missing local user level {lvlonline.OnlineID} - redownload");
					redownload.Add(Tuple.Create(lvlonline.OnlineID, lvlonline));
				}
			}

			foreach (var dl in redownload)
			{
				var levelcontent = await MainGame.Inst.Backend.DownloadUserLevel(MainGame.Inst.Profile, dl.Item1);
				if (levelcontent == null) continue;
								
				try
				{
					var dat = new LevelBlueprint();
					dat.BinaryDeserialize(new BinaryReader(new MemoryStream(levelcontent)));
					
					MainGame.Inst.DispatchBeginInvoke(() => 
					{
						SCCMUtils.UpdateUserLevelsFinished(dl.Item2.OnlineID, levelcontent);
						var entry = new SCCMListElementLocalPlayable(dat);
						_presenter.AddEntry(entry);
						entry.SetMeta(dl.Item2);
					});
				}
				catch (Exception e)
				{
					SAMLog.Error("SCCMTML::COMPILEFAIL_QMFS", "Could not compile dowbnloaded level", $"Exception: {e}\n\n\nLevel: {dl.Item1}\n\n\nContent:{ByteUtils.ByteToHexBitFiddle(levelcontent)}");
				}
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
