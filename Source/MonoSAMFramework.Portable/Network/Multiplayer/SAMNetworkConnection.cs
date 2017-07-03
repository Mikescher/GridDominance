using System;
using System.Linq;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public abstract class SAMNetworkConnection : ISAMUpdateable
	{
		public const float TIME_BETWEEN_PINGS = 1f;
		public const float TIME_BETWEEN_INGAME_PINGS = 3f;
		public const float TIMEOUT = 10f;
		public const float TIMEOUT_FINAL = 20f;
		public const float RESEND_TIME_RELIABLE = 0.75f;
		public const float TIME_BETWEEN_GAMESTATE = 0.2f;

		public const byte CMD_CREATESESSION = 100;
		public const byte CMD_JOINSESSION = 101;
		public const byte CMD_QUITSESSION = 102;
		public const byte CMD_QUERYSESSION = 103;
		public const byte CMD_PING = 104;
		public const byte CMD_FORWARD = 125;
		public const byte CMD_FORWARDLOBBYSYNC = 126;

		public const byte ACK_SESSIONCREATED = 50;
		public const byte ACK_SESSIONJOINED = 51;
		public const byte ACK_QUERYANSWER = 52;
		public const byte ACK_SESSIONQUITSUCCESS = 53;
		public const byte ACK_PINGRESULT = 54;
		public const byte ACK_SESSIONNOTFOUND = 61;
		public const byte ACK_SESSIONFULL = 62;
		public const byte ACK_SESSIONSECRETWRONG = 63;
		
		public const byte MSG_SESSIONTERMINATED = 71;
		
		public const byte ANS_FORWARDLOBBYSYNC = 81;

		public enum ConnectionState { Offline, Connected }

		public enum ServerMode
		{
			Base,                    // Initial
			
			CreatingSession,         // Server
			JoiningSession,          // Client
			
			InLobby,                 // Server|Client
			
			SyncingAfterLobby,       // Server

			InGame,                  // Server|Client

			Error,                   // See ErrorMessage, superset of [Stopped]
			Stopped,                 // medium is dry
		}

		private   readonly byte[] MSG_PING          = { CMD_PING,             0             };
		private   readonly byte[] MSG_CREATESESSION = { CMD_CREATESESSION,    0, 0          };
		private   readonly byte[] MSG_JOINSESSION   = { CMD_JOINSESSION,      0, 0, 0, 0, 0 };
		private   readonly byte[] MSG_QUITSESSION   = { CMD_QUITSESSION,      0, 0, 0, 0, 0 };
		private   readonly byte[] MSG_QUERYLOBBY    = { CMD_QUERYSESSION,     0, 0, 0, 0, 0 };
		private            byte[] MSG_LOBBYSYNC     = { CMD_FORWARDLOBBYSYNC, 0, 0, 0, 0, 0 };
		protected readonly byte[] MSG_FORWARD       = new byte[61];

		public ConnectionState ConnState = ConnectionState.Offline;
		public ServerMode Mode = ServerMode.Base;
		public string ErrorMessage = "";

		protected byte msgId = 0;
		protected int msgIdWraps = 0;
		public readonly NetworkUserConn[] UserConn = new NetworkUserConn[32];
		private float _lastServerResponse = 0f;

		public readonly float[] MsgSendTime = new float[256];
		public readonly bool[] MsgAcks = new bool[256];

		public readonly PingCounter Ping = new PingCounter(8);
		public float PackageLossPerc = 0;

		private float _lastSendPing = 0f;
		private float _lastSendJoinOrCreateSession = 0f;
		private float _lastSendLobbyQuery = 0f;
		private float _lastSendLobbySync = 0f;
		private float _lastSendGameState = 0f;
		private bool[] _lobbySyncAck;

		private bool _stopped = false;

		public byte SessionCount;
		public byte SessionCapacity;
		public ushort SessionID;
		public ushort SessionSecret;
		public ushort SessionUserID = 0;

		protected readonly INetworkMedium _medium;
		
		protected SAMNetworkConnection(INetworkMedium medium)
		{
			_medium = medium;

			MSG_FORWARD[0] = CMD_FORWARD;

			for (int i = 0; i < 32; i++) UserConn[i] = new NetworkUserConn();
		}

		public void Update(SAMTime gameTime, InputState istate)
		{
			if (Mode == ServerMode.Error) return;
			if (_stopped) return;
			
			try
			{
				var d = _medium.RecieveOrNull();

				if (d != null)
				{
					ProcessMessage(gameTime, d);
				}

				if (Mode == ServerMode.Base)
					UpdatePing(gameTime);
				else if (Mode == ServerMode.CreatingSession)
					UpdateCreateSession(gameTime);
				else if (Mode == ServerMode.JoiningSession)
					UpdateJoinSession(gameTime);
				else if (Mode == ServerMode.InLobby)
					UpdateInLobby(gameTime);
				else if (Mode == ServerMode.SyncingAfterLobby)
					UpdateAfterLobbySync(gameTime);
				else if (Mode == ServerMode.InGame)
					UpdateInGame(gameTime);
			}
			catch (Exception e)
			{
				SAMLog.Error("SNS", e);
			}
		}

		private void UpdatePing(SAMTime gameTime)
		{
			if (gameTime.TotalElapsedSeconds - _lastSendPing > TIME_BETWEEN_PINGS && gameTime.TotalElapsedSeconds - _lastServerResponse > TIME_BETWEEN_PINGS)
			{
				SetSequenceCounter(ref MSG_PING[1]);
				_medium.Send(MSG_PING);
				
				_lastSendPing = gameTime.TotalElapsedSeconds;
			}

			ConnState = (gameTime.TotalElapsedSeconds - _lastServerResponse > TIMEOUT) ? ConnectionState.Offline : ConnectionState.Connected;
		}

		private void UpdateCreateSession(SAMTime gameTime)
		{
			if (gameTime.TotalElapsedSeconds - _lastSendPing > TIME_BETWEEN_PINGS && gameTime.TotalElapsedSeconds - _lastServerResponse > TIME_BETWEEN_PINGS)
			{
				SetSequenceCounter(ref MSG_PING[1]);
				_medium.Send(MSG_PING);
				
				_lastSendPing = gameTime.TotalElapsedSeconds;
			}

			ConnState = (gameTime.TotalElapsedSeconds - _lastServerResponse > TIMEOUT) ? ConnectionState.Offline : ConnectionState.Connected;

			if (gameTime.TotalElapsedSeconds - _lastServerResponse > TIMEOUT_FINAL)
			{
				Mode = ServerMode.Error;
				ErrorMessage = "Timeout"; //TODO L10N
				Stop();
				return;
			}

			if (gameTime.TotalElapsedSeconds - _lastSendJoinOrCreateSession > RESEND_TIME_RELIABLE)
			{
				SetSequenceCounter(ref MSG_CREATESESSION[1]);
				MSG_CREATESESSION[2] = SessionCapacity;
				_medium.Send(MSG_CREATESESSION);
				_lastSendJoinOrCreateSession = gameTime.TotalElapsedSeconds;
			}
		}

		private void UpdateJoinSession(SAMTime gameTime)
		{
			if (gameTime.TotalElapsedSeconds - _lastSendPing > TIME_BETWEEN_PINGS && gameTime.TotalElapsedSeconds - _lastServerResponse > TIME_BETWEEN_PINGS)
			{
				SetSequenceCounter(ref MSG_PING[1]);
				_medium.Send(MSG_PING);
				
				_lastSendPing = gameTime.TotalElapsedSeconds;
			}

			ConnState = (gameTime.TotalElapsedSeconds - _lastServerResponse > TIMEOUT) ? ConnectionState.Offline : ConnectionState.Connected;

			if (gameTime.TotalElapsedSeconds - _lastServerResponse > TIMEOUT_FINAL)
			{
				Mode = ServerMode.Error;
				ErrorMessage = "Timeout"; //TODO L10N
				Stop();
				return;
			}

			if (gameTime.TotalElapsedSeconds - _lastSendJoinOrCreateSession > RESEND_TIME_RELIABLE)
			{
				SetSequenceCounter(ref MSG_JOINSESSION[1]);
				MSG_JOINSESSION[2] = (byte)((SessionID >> 8) & 0xFF);
				MSG_JOINSESSION[3] = (byte)(SessionID & 0xFF);
				MSG_JOINSESSION[4] = (byte)(((SessionUserID & 0xF) << 4) | ((SessionSecret >> 8) & 0x0F));
				MSG_JOINSESSION[5] = (byte)(SessionSecret & 0xFF);
				_medium.Send(MSG_JOINSESSION);
				_lastSendJoinOrCreateSession = gameTime.TotalElapsedSeconds;
			}
		}

		private void UpdateInLobby(SAMTime gameTime)
		{
			if (gameTime.TotalElapsedSeconds - _lastSendLobbyQuery > TIME_BETWEEN_PINGS)
			{
				SetSequenceCounter(ref MSG_QUERYLOBBY[1]);
				MSG_QUERYLOBBY[2] = (byte)((SessionID >> 8) & 0xFF);
				MSG_QUERYLOBBY[3] = (byte)(SessionID & 0xFF);
				MSG_QUERYLOBBY[4] = (byte)(((SessionUserID & 0xF) << 4) | ((SessionSecret >> 8) & 0x0F));
				MSG_QUERYLOBBY[5] = (byte)(SessionSecret & 0xFF);

				_medium.Send(MSG_QUERYLOBBY);
				_lastSendLobbyQuery = gameTime.TotalElapsedSeconds;
			}

			ConnState = (gameTime.TotalElapsedSeconds - _lastServerResponse > TIMEOUT) ? ConnectionState.Offline : ConnectionState.Connected;

			if (gameTime.TotalElapsedSeconds - _lastServerResponse > TIMEOUT_FINAL)
			{
				Mode = ServerMode.Error;
				ErrorMessage = "Timeout"; //TODO L10N
				Stop();
				return;
			}
		}

		private void UpdateAfterLobbySync(SAMTime gameTime)
		{
			if (gameTime.TotalElapsedSeconds - _lastSendPing > TIME_BETWEEN_PINGS && gameTime.TotalElapsedSeconds - _lastServerResponse > TIME_BETWEEN_PINGS)
			{
				SetSequenceCounter(ref MSG_PING[1]);
				_medium.Send(MSG_PING);

				_lastSendPing = gameTime.TotalElapsedSeconds;
			}

			ConnState = (gameTime.TotalElapsedSeconds - _lastServerResponse > TIMEOUT) ? ConnectionState.Offline : ConnectionState.Connected;

			if (gameTime.TotalElapsedSeconds - _lastServerResponse > TIMEOUT_FINAL)
			{
				Mode = ServerMode.Error;
				ErrorMessage = "Timeout"; //TODO L10N
				Stop();
				return;
			}

			if (gameTime.TotalElapsedSeconds - _lastSendLobbySync > RESEND_TIME_RELIABLE)
			{
				SetSequenceCounter(ref MSG_LOBBYSYNC[1]);
				_medium.Send(MSG_LOBBYSYNC);
				_lastSendLobbySync = gameTime.TotalElapsedSeconds;
			}

			if (_lobbySyncAck.All(p => p))
			{
				Mode = ServerMode.InGame;
			}
		}

		private void UpdateInGame(SAMTime gameTime)
		{
			if (gameTime.TotalElapsedSeconds - _lastSendGameState > TIME_BETWEEN_GAMESTATE)
			{
				SendGameStateNow();

				_lastSendGameState = gameTime.TotalElapsedSeconds;
			}

			if (gameTime.TotalElapsedSeconds - _lastSendPing > TIME_BETWEEN_INGAME_PINGS && gameTime.TotalElapsedSeconds - _lastServerResponse > TIME_BETWEEN_PINGS)
			{
				SetSequenceCounter(ref MSG_PING[1]);
				_medium.Send(MSG_PING);

				_lastSendPing = gameTime.TotalElapsedSeconds;
			}
			
			ConnState = (gameTime.TotalElapsedSeconds - _lastServerResponse > TIMEOUT) ? ConnectionState.Offline : ConnectionState.Connected;

			if (gameTime.TotalElapsedSeconds - _lastServerResponse > TIMEOUT_FINAL)
			{
				Mode = ServerMode.Error;
				ErrorMessage = "Timeout"; //TODO L10N
				Stop();
				return;
			}

			for (int i = 0; i < SessionCount; i++)
			{
				if (i == SessionUserID) continue;

				if (gameTime.TotalElapsedSeconds - UserConn[i].LastResponse > TIMEOUT_FINAL)
				{
					Mode = ServerMode.Error;
					ErrorMessage = "Timeout from User " + i; //TODO L10N
					Stop();
					return;
				}
			}
		}

		protected void SetSequenceCounter(ref byte target)
		{
			target = msgId;

			MsgAcks[msgId] = false;
			MsgSendTime[msgId] = MonoSAMGame.CurrentTime.TotalElapsedSeconds;

			msgId++;

			if (msgId == 0) msgIdWraps++;

			if (msgIdWraps == 0)
			{
				int c = 0;
				int m = 0;
				for (int i = 0; i < msgId - 32; i++) { c += MsgAcks[i] ? 0 : 1; m++; }
				PackageLossPerc = c / (m * 1f);
			}
			else
			{
				int c = 0;
				for (int i = 0; i < 128; i++) c += MsgAcks[(msgId + i) % 256] ? 0 : 1;
				PackageLossPerc = c / 128f;
			}
		}

		protected void RecieveAck(byte seq)
		{
			if (! MsgAcks[seq])
			{
				_lastServerResponse = MonoSAMGame.CurrentTime.TotalElapsedSeconds;

				MsgAcks[seq] = true;
				Ping.Inc(_lastServerResponse - MsgSendTime[seq]);
			}
		}
		
		protected void RecieveMsg(byte usr, byte seq)
		{
			if (usr >= 32) return;

			UserConn[usr].LastResponse = MonoSAMGame.CurrentTime.TotalElapsedSeconds;

			if (IsSeqGreater(UserConn[usr].LastRecievedSeq, seq)) UserConn[usr].LastRecievedSeq = seq;
		}

		protected bool IsSeqGreater(int a, int b)
		{
			if (a < b && (b - a) < 128) return true;
			if (a < (b+256) && ((b+256) - a) < 128) return true;

			return false;
		}
		
		public void CreateSession(int size)
		{
			if (Mode == ServerMode.CreatingSession) return;
			if (Mode == ServerMode.JoiningSession) return;

			Mode = ServerMode.CreatingSession;
			SessionCapacity = (byte)size;
		}

		public void JoinSession(ushort sid, ushort sec)
		{
			if (Mode == ServerMode.CreatingSession) return;
			if (Mode == ServerMode.JoiningSession) return;

			Mode = ServerMode.JoiningSession;
			SessionID = sid;
			SessionSecret = sec;
		}

		public void KillSession()
		{
			SetSequenceCounter(ref MSG_QUITSESSION[1]);
			MSG_QUITSESSION[2] = (byte)((SessionID >> 8) & 0xFF);
			MSG_QUITSESSION[3] = (byte)(SessionID & 0xFF);
			MSG_QUITSESSION[4] = (byte)(((SessionUserID & 0xF) << 4) | ((SessionSecret >> 8) & 0x0F));
			MSG_QUITSESSION[5] = (byte)(SessionSecret & 0xFF);
			
			_medium.Send(MSG_QUITSESSION);
		}
		
		private void ProcessMessage(SAMTime gameTime, byte[] d)
		{
			switch (d[0])
			{
				case ACK_PINGRESULT:
					RecieveAck(d[1]);
					
					return;
				
				case ACK_SESSIONCREATED:
					if (Mode == ServerMode.CreatingSession)
					{
						RecieveAck(d[1]);

						Mode = ServerMode.InLobby;
						SessionID = (ushort)(((d[2] << 8) & 0xFF00) | (d[3] & 0xFF));
						SessionSecret = (ushort)((((d[4] << 8) & 0xFF00) | (d[5] & 0xFF)) & 0x0FFF);
						SessionUserID = 0;
						SessionCapacity = d[6];

						SAMLog.Debug($"Session created: {SessionID}:[{SessionSecret}]   (capacity: {SessionCapacity})");
						
						return;
					}
					break;
				
				case ACK_QUERYANSWER:

					if (Mode == ServerMode.InLobby)
					{
						RecieveAck(d[1]);

						bool insession = (d[4] == 1);
						SessionCount = d[6];
						SessionCapacity = d[7];

						if (!insession)
						{
							Mode = ServerMode.Error;
							ErrorMessage = "You are not part of the lobby"; //TODO L10N
							Stop();
							return;
						}

						return;
					}
					break;

				case ACK_SESSIONNOTFOUND:

					if (Mode == ServerMode.InLobby)
					{
						RecieveAck(d[1]);

						Mode = ServerMode.Error;
						ErrorMessage = "Could not find session on server"; //TODO L10N
						Stop();
						return;
					}
					else if (Mode == ServerMode.JoiningSession)
					{
						RecieveAck(d[1]);

						Mode = ServerMode.Error;
						ErrorMessage = "Could not find session on server"; //TODO L10N
						Stop();
						return;
					}
					break;

				case ACK_SESSIONSECRETWRONG:

					if (Mode == ServerMode.InLobby)
					{
						RecieveAck(d[1]);

						Mode = ServerMode.Error;
						ErrorMessage = "Session Authentification failed"; //TODO L10N
						Stop();
						return;
					}
					else if (Mode == ServerMode.JoiningSession)
					{
						RecieveAck(d[1]);

						Mode = ServerMode.Error;
						ErrorMessage = "Could not find session on server"; //TODO L10N
						Stop();
						return;
					}
					break;

				case ACK_SESSIONFULL:

					if (Mode == ServerMode.JoiningSession)
					{
						RecieveAck(d[1]);

						Mode = ServerMode.Error;
						ErrorMessage = "Game lobby already full"; //TODO L10N
						Stop();
						return;
					}
					break;

				case ACK_SESSIONJOINED:

					if (Mode == ServerMode.JoiningSession)
					{
						RecieveAck(d[1]);

						Mode = ServerMode.InLobby;
						SessionID = (ushort)(((d[2] << 8) & 0xFF00) | (d[3] & 0xFF));
						SessionSecret = (ushort)((((d[4] << 8) & 0xFF00) | (d[5] & 0xFF)) & 0x0FFF);
						SessionUserID = d[6];
						SessionCapacity = d[7];

						SAMLog.Debug($"Session joined: {SessionID}:[{SessionSecret}]   (capacity: {SessionCapacity})");

						return;
					}
					break;

				case ANS_FORWARDLOBBYSYNC:

					if (Mode == ServerMode.SyncingAfterLobby)
					{
						var rec_sessionID = (ushort)(((d[2] << 8) & 0xFF00) | (d[3] & 0xFF));
						var rec_sessionSecret = (ushort)((((d[4] << 8) & 0xFF00) | (d[5] & 0xFF)) & 0x0FFF);
						var rec_sessionUserID = d[6];

						UserConn[rec_sessionUserID].LastResponse = gameTime.TotalElapsedSeconds;


						RecieveAck(d[1]);

						if (rec_sessionID == SessionID && rec_sessionSecret == SessionSecret && rec_sessionUserID < SessionCount)
						{
							SAMLog.Debug($"LobbySync Ack: {SessionID}:[{SessionSecret}]   {SessionUserID})");
							_lobbySyncAck[rec_sessionUserID] = true;
						}
						else
						{
							SAMLog.Error("SNS", $"Wrong Validation in ACK_FORWARDLOBBYSYNC   #   Session: {rec_sessionID}:[{rec_sessionSecret}] (real= {SessionID}:[{SessionSecret}]) by user {rec_sessionUserID}");
						}
						

						return;
					}
					break;
			}

			if (ProcessSpecificMessage(d[0], d)) return;

			SAMLog.Error("SNS", "Unknown Server command: " + d[0] + " in mode " + Mode);
		}

		protected abstract bool ProcessSpecificMessage(byte cmd, byte[] data);
		protected abstract void SendGameStateNow();

		public void StartLobbySync(byte[] binData)
		{
			if (Mode == ServerMode.SyncingAfterLobby) return;

			MSG_LOBBYSYNC = new byte[6 + binData.Length];
			MSG_LOBBYSYNC[0] = CMD_FORWARDLOBBYSYNC;
			MSG_LOBBYSYNC[2] = (byte)((SessionID >> 8) & 0xFF);
			MSG_LOBBYSYNC[3] = (byte)(SessionID & 0xFF);
			MSG_LOBBYSYNC[4] = (byte)(((SessionUserID & 0xF) << 4) | ((SessionSecret >> 8) & 0x0F));
			MSG_LOBBYSYNC[5] = (byte)(SessionSecret & 0xFF);

			for (int i = 0; i < binData.Length; i++) MSG_LOBBYSYNC[6 + i] = binData[i];

			_lobbySyncAck = new bool[SessionCount];
			_lobbySyncAck[SessionUserID] = true;
			
			Mode = ServerMode.SyncingAfterLobby;
		}

		public void Stop()
		{
			if (_stopped) return;

			if (Mode != ServerMode.Error) Mode = ServerMode.Stopped;
			
			_stopped = true;
			_medium.Dispose();
		}
	}
}
