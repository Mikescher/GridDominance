using System;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public abstract class SAMNetworkServer : ISAMUpdateable
	{
		public const float TIME_BETWEEN_PINGS = 1f;
		public const float TIMEOUT = 10f;
		public const float TIMEOUT_FINAL = 20f;
		public const float RESEND_TIME_RELIABLE = 0.75f;

		public const byte CMD_CREATESESSION = 100;
		public const byte CMD_JOINSESSION = 101;
		public const byte CMD_QUITSESSION = 102;
		public const byte CMD_QUERYSESSION = 103;
		public const byte CMD_PING = 104;
		public const byte CMD_FORWARD = 125;

		public const byte RET_SESSIONCREATED = 50;
		public const byte RET_SESSIONJOINED = 51;
		public const byte RET_QUERYANSWER = 52;
		public const byte RET_SESSIONQUITSUCCESS = 53;
		public const byte RET_PINGRESULT = 54;
		public const byte RET_SESSIONNOTFOUND = 61;
		public const byte RET_SESSIONFULL = 62;
		public const byte RET_SESSIONSECRETWRONG = 63;
		public const byte RET_SESSIONTERMINATED = 71;

		public enum ConnectionState { Offline, Connected }
		public enum ServerMode { Base, CreatingSession, InLobby, Error, Stopped }

		private static readonly byte[] MSG_PING = { CMD_PING };
		private static readonly byte[] MSG_CREATESESSION = { CMD_CREATESESSION, 0 };
		private static readonly byte[] MSG_QUITSESSION = { CMD_QUITSESSION, 0, 0, 0, 0 };
		private static readonly byte[] MSG_QUERYLOBBY = { CMD_QUERYSESSION, 0, 0, 0, 0 };

		public ConnectionState ConnState = ConnectionState.Offline;
		public ServerMode Mode = ServerMode.Base;
		public string ErrorMessage = "";
		
		private float _lastServerResponse = 0f;
		private float _lastSendPing = 0f;
		private float _lastSendCreateSession = 0f;
		private float _lastSendLobbyQuery = 0f;

		private bool _stopped = false;

		public byte SessionCount;
		public byte SessionCapacity;
		private ushort _sessionID;
		private ushort _sessionSecret;
		private readonly ushort _sessionUserID = 0;

		private readonly INetworkMedium _medium;
		
		protected SAMNetworkServer(INetworkMedium medium)
		{
			_medium = medium;
			
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

				if (Mode == ServerMode.Base) UpdatePing(gameTime);
				if (Mode == ServerMode.CreatingSession) UpdateCreateSession(gameTime);
				if (Mode == ServerMode.InLobby) UpdateInLobby(gameTime);
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
				_medium.Send(MSG_PING);
				_lastSendPing = gameTime.TotalElapsedSeconds;
			}

			ConnState = (gameTime.TotalElapsedSeconds - _lastServerResponse > TIMEOUT) ? ConnectionState.Offline : ConnectionState.Connected;
		}

		private void UpdateCreateSession(SAMTime gameTime)
		{
			if (gameTime.TotalElapsedSeconds - _lastSendPing > TIME_BETWEEN_PINGS && gameTime.TotalElapsedSeconds - _lastServerResponse > TIME_BETWEEN_PINGS)
			{
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

			if (gameTime.TotalElapsedSeconds - _lastSendCreateSession > RESEND_TIME_RELIABLE)
			{
				MSG_CREATESESSION[1] = SessionCapacity;
				_medium.Send(MSG_CREATESESSION);
				_lastSendCreateSession = gameTime.TotalElapsedSeconds;
			}
		}

		private void UpdateInLobby(SAMTime gameTime)
		{
			if (gameTime.TotalElapsedSeconds - _lastSendLobbyQuery > TIME_BETWEEN_PINGS)
			{
				MSG_QUERYLOBBY[1] = (byte)((_sessionID >> 8) & 0xFF);
				MSG_QUERYLOBBY[2] = (byte)(_sessionID & 0xFF);
				MSG_QUERYLOBBY[3] = (byte)(((_sessionUserID & 0xF) << 4) | ((_sessionSecret >> 8) & 0x0F));
				MSG_QUERYLOBBY[4] = (byte)(_sessionSecret & 0xFF);

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

		public void CreateSession(int size)
		{
			if (Mode == ServerMode.CreatingSession) return;
			
			Mode = ServerMode.CreatingSession;
			SessionCapacity = (byte)size;
			
		}

		public void KillSession()
		{
			MSG_QUITSESSION[1] = (byte)((_sessionID >> 8) & 0xFF);
			MSG_QUITSESSION[2] = (byte)(_sessionID & 0xFF);
			MSG_QUITSESSION[3] = (byte)(((_sessionUserID & 0xF) << 4) | ((_sessionSecret >> 8) & 0x0F));
			MSG_QUITSESSION[4] = (byte)(_sessionSecret & 0xFF);
			
			_medium.Send(MSG_QUITSESSION);
		}
		
		private void ProcessMessage(SAMTime gameTime, byte[] d)
		{
			switch (d[0])
			{
				case RET_PINGRESULT:
					_lastServerResponse = gameTime.TotalElapsedSeconds;
					return;
				
				case RET_SESSIONCREATED:
					if (Mode == ServerMode.CreatingSession)
					{
						_lastServerResponse = gameTime.TotalElapsedSeconds;

						Mode = ServerMode.InLobby;
						_sessionID = (ushort)(((d[1] << 8) & 0xFF00) | (d[2] & 0xFF));
						_sessionSecret = (ushort)((((d[3] << 8) & 0xFF00) | (d[4] & 0xFF)) & 0x0FFF);
						SessionCapacity = d[5];

						SAMLog.Debug($"Session created: {_sessionID}:[{_sessionSecret}]   (capacity: {SessionCapacity})");
						
						return;
					}
					break;
				
				case RET_QUERYANSWER:
					_lastServerResponse = gameTime.TotalElapsedSeconds;

					if (Mode == ServerMode.InLobby)
					{
						bool insession = (d[3] == 1);
						SessionCount = d[5];
						SessionCapacity = d[6];

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

				case RET_SESSIONNOTFOUND:
					_lastServerResponse = gameTime.TotalElapsedSeconds;

					if (Mode == ServerMode.InLobby)
					{
						Mode = ServerMode.Error;
						ErrorMessage = "Could not find session on server"; //TODO L10N
						Stop();
						return;
					}
					break;

				case RET_SESSIONSECRETWRONG:
					_lastServerResponse = gameTime.TotalElapsedSeconds;

					if (Mode == ServerMode.InLobby)
					{
						Mode = ServerMode.Error;
						ErrorMessage = "Session Authentification failed"; //TODO L10N
						Stop();
						return;
					}
					break;
			}
			
			SAMLog.Error("SNS", "Unknown Server command: " + d[0]);
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
