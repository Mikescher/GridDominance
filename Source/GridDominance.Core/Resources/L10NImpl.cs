using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Resources
{
	public static class L10NImpl
	{
		public const int LANG_EN_US = 0;
		public const int LANG_DE_DE = 1;
		public const int LANG_FR_FR = 2;
		public const int LANG_IT_IT = 3;
		public const int LANG_ES_ES = 4;

		public static int LANG_COUNT = 5;

//-----------------------------------------------------------------------

		public const int STR_SSB_ABOUT                   = 0;
		public const int STR_SSB_ACCOUNT                 = 1;
		public const int STR_SSB_HIGHSCORE               = 2;
		public const int STR_SSB_MUTE                    = 3;
		public const int STR_SSB_EFFECTS                 = 4;
		public const int STR_SSB_LANGUAGE                = 69;
		public const int STR_SSB_MUSIC                   = 111;
		public const int STR_SSB_COLOR                   = 185;

		public const int STR_HSP_GLOBALRANKING           = 5;
		public const int STR_HSP_RANKINGFOR              = 6;
		public const int STR_HSP_LEVEL                   = 43;
		public const int STR_HSP_POINTS                  = 44;
		public const int STR_HSP_PROGRESS                = 45;
		public const int STR_HSP_BACK                    = 46;
		public const int STR_HSP_NEXT                    = 47;
		public const int STR_HSP_AGAIN                   = 48;
		public const int STR_HSP_TUTORIAL                = 49;
		public const int STR_HSP_GETSTARTED              = 50;
		public const int STR_HSP_CONERROR                = 65;
		public const int STR_HSP_NEWGAME                 = 158;
		public const int STR_HSP_RANDOMGAME              = 159;
		public const int STR_HSP_MPPOINTS                = 160;
		public const int STR_HSP_MULTIPLAYERRANKING      = 167;
		public const int STR_HSP_TIME_NOW                = 183;
		public const int STR_HSP_TIME_BEST               = 184;
		public const int STR_HSP_STARRANKING             = 264;
		public const int STR_HSP_SCCMRANKING             = 265;
		public const int STR_HSP_AUTHOR                  = 276;
		public const int STR_HSP_TIME_YOU                = 277;

		public const int STR_TAB_NAME                    = 7;
		public const int STR_TAB_POINTS                  = 8;
		public const int STR_TAB_TIME                    = 9;
		public const int STR_TAB_STARS                   = 266;

		public const int STR_FAP_ACCOUNT                 = 10;
		public const int STR_FAP_USERNAME                = 11;
		public const int STR_FAP_CHANGEPW                = 12;
		public const int STR_FAP_SCORE                   = 78;
		public const int STR_FAP_LOGOUT                  = 93;
		public const int STR_FAP_WARN1                   = 94;
		public const int STR_FAP_WARN2                   = 95;
		public const int STR_FAP_LOGOUT_SUCESS           = 96;

		public const int STR_CPP_CHANGEPW                = 13;
		public const int STR_CPP_USERNAME                = 14;
		public const int STR_CPP_NEWPW                   = 15;
		public const int STR_CPP_CHANGE                  = 16;
		public const int STR_CPP_CHANGING                = 17;
		public const int STR_CPP_CHANGED                 = 18;
		public const int STR_CPP_COMERR                  = 19;
		public const int STR_CPP_AUTHERR                 = 20;
		public const int STR_CPP_CHANGEERR               = 21;

		public const int STR_ATTRIBUTIONS                = 22;

		public const int STR_AAP_HEADER                  = 23;
		public const int STR_AAP_USERNAME                = 24;
		public const int STR_AAP_PASSWORD                = 25;
		public const int STR_AAP_CREATEACCOUNT           = 26;
		public const int STR_AAP_LOGIN                   = 27;
		public const int STR_AAP_LOGGINGIN               = 28;
		public const int STR_AAP_WRONGPW                 = 29;
		public const int STR_AAP_USERNOTFOUND            = 30;
		public const int STR_AAP_NOCOM                   = 31;
		public const int STR_AAP_LOGINSUCCESS            = 32;
		public const int STR_AAP_NOLOGIN                 = 33;
		public const int STR_AAP_ACCCREATING             = 34;
		public const int STR_AAP_ACCCREATED              = 35;
		public const int STR_AAP_USERTAKEN               = 36;
		public const int STR_AAP_ALREADYCREATED          = 37;
		public const int STR_AAP_AUTHERROR               = 38;
		public const int STR_AAP_COULDNOTCREATE          = 39;

		public const int STR_PAUS_RESUME                 = 40;
		public const int STR_PAUS_RESTART                = 41;
		public const int STR_PAUS_EXIT                   = 42;

		public const int STR_DIFF_0                      = 51;
		public const int STR_DIFF_1                      = 52;
		public const int STR_DIFF_2                      = 53;
		public const int STR_DIFF_3                      = 54;

		public const int STR_TUT_INFO2                   = 55;
		public const int STR_TUT_INFO3                   = 56;
		public const int STR_TUT_INFO4                   = 57;
		public const int STR_TUT_INFO5                   = 58;
		public const int STR_TUT_INFO6                   = 59;
		public const int STR_TUT_INFO7                   = 60;
		public const int STR_TUT_INFO8                   = 61;
		public const int STR_TUT_INFO1                   = 79;

		public const int STR_API_CONERR                  = 62;
		public const int STR_API_COMERR                  = 63;

		public const int STR_GLOB_EXITTOAST              = 64;
		public const int STR_GLOB_UNLOCKTOAST1           = 66;
		public const int STR_GLOB_UNLOCKTOAST2           = 67;
		public const int STR_GLOB_UNLOCKTOAST3           = 68;
		public const int STR_GLOB_LEVELLOCK              = 70;
		public const int STR_GLOB_WORLDLOCK              = 71;
		public const int STR_GLOB_OVERWORLD              = 75;
		public const int STR_GLOB_WAITFORSERVER          = 76;
		public const int STR_GLOB_UNLOCKSUCCESS          = 86;

		public const int STR_INF_YOU                     = 72;
		public const int STR_INF_GLOBAL                  = 73;
		public const int STR_INF_HIGHSCORE               = 74;
		public const int STR_INF_CLEARS                  = 255;

		public const int STR_WORLD_TUTORIAL              = 77;
		public const int STR_WORLD_W1                    = 80;
		public const int STR_WORLD_W2                    = 81;
		public const int STR_WORLD_W3                    = 92;
		public const int STR_WORLD_W4                    = 103;
		public const int STR_WORLD_MULTIPLAYER           = 114;
		public const int STR_WORLD_SINGLEPLAYER          = 178;
		public const int STR_WORLD_ONLINE                = 194;

		public const int STR_IAB_TESTERR                 = 82;
		public const int STR_IAB_TESTNOCONN              = 83;
		public const int STR_IAB_TESTINPROGRESS          = 84;
		public const int STR_IAB_BUYERR                  = 88;
		public const int STR_IAB_BUYNOCONN               = 89;
		public const int STR_IAB_BUYNOTREADY             = 90;
		public const int STR_IAB_BUYSUCESS               = 91;

		public const int STR_UNLOCK                      = 85;

		public const int STR_PREV_BUYNOW                 = 87;
		public const int STR_PREV_FINISHWORLD            = 108;
		public const int STR_PREV_OR                     = 109;
		public const int STR_PREV_MISS_TOAST             = 110;
		public const int STR_PREV_FINISHGAME             = 267;

		public const int STR_HINT_001                    = 97;
		public const int STR_HINT_002                    = 98;
		public const int STR_HINT_003                    = 99;
		public const int STR_HINT_004                    = 100;
		public const int STR_HINT_005                    = 101;
		public const int STR_HINT_006                    = 102;
		public const int STR_HINT_007                    = 112;
		public const int STR_HINT_008                    = 113;
		public const int STR_HINT_009                    = 173;
		public const int STR_HINT_010                    = 176;
		public const int STR_HINT_011                    = 177;

		public const int STR_INFOTOAST_1                 = 104;
		public const int STR_INFOTOAST_2                 = 105;
		public const int STR_INFOTOAST_3                 = 106;
		public const int STR_INFOTOAST_4                 = 107;

		public const int STR_MP_TIMEOUT                  = 115;
		public const int STR_MP_TIMEOUT_USER             = 116;
		public const int STR_MP_NOTINLOBBY               = 117;
		public const int STR_MP_SESSIONNOTFOUND          = 118;
		public const int STR_MP_AUTHFAILED               = 119;
		public const int STR_MP_LOBBYFULL                = 120;
		public const int STR_MP_VERSIONMISMATCH          = 121;
		public const int STR_MP_LEVELNOTFOUND            = 122;
		public const int STR_MP_LEVELMISMATCH            = 123;
		public const int STR_MP_USERDISCONNECT           = 124;
		public const int STR_MP_SERVERDISCONNECT         = 125;
		public const int STR_MP_ONLINE                   = 129;
		public const int STR_MP_OFFLINE                  = 130;
		public const int STR_MP_CONNECTING               = 131;
		public const int STR_MP_INTERNAL                 = 161;
		public const int STR_MP_BTADAPTERNULL            = 162;
		public const int STR_MP_BTDISABLED               = 163;
		public const int STR_MP_DIRECTCONNLOST           = 164;
		public const int STR_MP_DIRECTCONNFAIL           = 165;
		public const int STR_MP_BTADAPTERPERMDENIED      = 168;
		public const int STR_MP_TOAST_CONN_TRY           = 170;
		public const int STR_MP_TOAST_CONN_FAIL          = 171;
		public const int STR_MP_TOAST_CONN_SUCC          = 172;
		public const int STR_MP_NOSERVERCONN             = 182;

		public const int STR_MENU_CAP_MULTIPLAYER        = 126;
		public const int STR_MENU_CAP_LOBBY              = 127;
		public const int STR_MENU_CAP_CGAME_PROX         = 128;
		public const int STR_MENU_MP_JOIN                = 132;
		public const int STR_MENU_MP_HOST                = 133;
		public const int STR_MENU_MP_LOCAL_CLASSIC       = 134;
		public const int STR_MENU_MP_ONLINE              = 135;
		public const int STR_MENU_CAP_AUTH               = 136;
		public const int STR_MENU_MP_CREATE              = 137;
		public const int STR_MENU_CANCEL                 = 138;
		public const int STR_MENU_MP_GAMESPEED           = 139;
		public const int STR_MENU_MP_MUSIC               = 140;
		public const int STR_MENU_MP_LOBBYINFO           = 141;
		public const int STR_MENU_MP_LOBBY_USER          = 142;
		public const int STR_MENU_MP_LOBBY_LEVEL         = 143;
		public const int STR_MENU_MP_LOBBY_MUSIC         = 144;
		public const int STR_MENU_MP_LOBBY_SPEED         = 145;
		public const int STR_MENU_MP_LOBBY_PING          = 146;
		public const int STR_MENU_DISCONNECT             = 147;
		public const int STR_MENU_MP_LOBBY_USER_FMT      = 148;
		public const int STR_MENU_MP_START               = 149;
		public const int STR_MENU_MP_LOBBY_COLOR         = 157;
		public const int STR_MENU_CAP_SEARCH             = 166;
		public const int STR_MENU_CAP_CGAME_BT           = 169;
		public const int STR_MENU_CAP_SCCM               = 233;

		public const int STR_FRAC_N0                     = 150;
		public const int STR_FRAC_P1                     = 151;
		public const int STR_FRAC_A2                     = 152;
		public const int STR_FRAC_A3                     = 153;
		public const int STR_FRAC_A4                     = 154;
		public const int STR_FRAC_A5                     = 155;
		public const int STR_FRAC_A6                     = 156;

		public const int STR_ENDGAME_1                   = 174;
		public const int STR_ENDGAME_2                   = 175;

		public const int STR_ACCOUNT_REMINDER            = 179;

		public const int STR_BTN_OK                      = 180;
		public const int STR_BTN_NO                      = 181;

		public const int STR_ERR_SOUNDPLAYBACK           = 186;
		public const int STR_ERR_MUSICPLAYBACK           = 187;
		public const int STR_ERR_OUTOFMEMORY             = 189;

		public const int STR_ACKNOWLEDGEMENTS            = 188;

		public const int STR_PROFILESYNC_START           = 190;
		public const int STR_PROFILESYNC_ERROR           = 191;
		public const int STR_PROFILESYNC_SUCCESS         = 192;

		public const int STR_AUTHERR_HEADER              = 193;

		public const int STR_LVLED_MOUSE                 = 195;
		public const int STR_LVLED_CANNON                = 196;
		public const int STR_LVLED_WALL                  = 197;
		public const int STR_LVLED_OBSTACLE              = 198;
		public const int STR_LVLED_SETTINGS              = 199;
		public const int STR_LVLED_PLAY                  = 200;
		public const int STR_LVLED_UPLOAD                = 201;
		public const int STR_LVLED_EXIT                  = 202;
		public const int STR_LVLED_BTN_FRAC              = 203;
		public const int STR_LVLED_BTN_SCALE             = 204;
		public const int STR_LVLED_BTN_ROT               = 205;
		public const int STR_LVLED_BTN_TYPE              = 206;
		public const int STR_LVLED_BTN_DIAMETER          = 207;
		public const int STR_LVLED_BTN_WIDTH             = 208;
		public const int STR_LVLED_BTN_HEIGHT            = 209;
		public const int STR_LVLED_BTN_POWER             = 210;
		public const int STR_LVLED_BTN_DEL               = 211;
		public const int STR_LVLED_CFG_ID                = 212;
		public const int STR_LVLED_CFG_NAME              = 213;
		public const int STR_LVLED_CFG_SIZE              = 214;
		public const int STR_LVLED_CFG_VIEW              = 215;
		public const int STR_LVLED_CFG_GEOMETRY          = 216;
		public const int STR_LVLED_CFG_WRAP_INFINITY     = 217;
		public const int STR_LVLED_CFG_WRAP_DONUT        = 218;
		public const int STR_LVLED_CFG_WRAP_REFLECT      = 219;
		public const int STR_LVLED_PORTAL                = 220;
		public const int STR_LVLED_BTN_CHANNEL           = 221;
		public const int STR_LVLED_BTN_DIR               = 222;
		public const int STR_LVLED_BTN_LEN               = 223;
		public const int STR_LVLED_ERR_NONAME            = 224;
		public const int STR_LVLED_ERR_NOENEMY           = 225;
		public const int STR_LVLED_ERR_NOPLAYER          = 226;
		public const int STR_LVLED_ERR_TOOMANYENTS       = 227;
		public const int STR_LVLED_ERR_COMPILERERR       = 228;
		public const int STR_LVLED_BTN_DELLEVEL          = 229;
		public const int STR_LVLED_BTN_SAVE              = 230;
		public const int STR_LVLED_BTN_DISCARD           = 231;
		public const int STR_LVLED_TOAST_DELLEVEL        = 232;
		public const int STR_LVLED_TAB_MYLEVELS          = 234;
		public const int STR_LVLED_TAB_HOT               = 235;
		public const int STR_LVLED_TAB_TOP               = 236;
		public const int STR_LVLED_TAB_NEW               = 237;
		public const int STR_LVLED_TAB_SEARCH            = 238;
		public const int STR_LVLED_BTN_NEWLVL            = 239;
		public const int STR_LVLED_BTN_EDIT              = 241;
		public const int STR_LVLED_BTN_PLAY              = 242;
		public const int STR_LVLED_COMPILING             = 243;
		public const int STR_LVLED_CFG_MUSIC             = 246;
		public const int STR_LVLED_BTN_UPLOAD            = 247;
		public const int STR_LVLED_UPLOADING             = 248;
		public const int STR_LVLED_UPLOAD_FIN            = 249;
		public const int STR_LVLED_BTN_ABORT             = 272;
		public const int STR_LVLED_BTN_RETRY             = 273;

		public const int STR_GENERIC_SERVER_QUERY        = 240;

		public const int STR_MUSIC_NONE                  = 244;
		public const int STR_MUSIC_INT                   = 245;

		public const int STR_LVLUPLD_ERR_INTERNAL        = 250;
		public const int STR_LVLUPLD_ERR_FILETOOBIG      = 251;
		public const int STR_LVLUPLD_ERR_WRONGUSER       = 252;
		public const int STR_LVLUPLD_ERR_LIDNOTFOUND     = 253;
		public const int STR_LVLUPLD_ERR_INVALIDNAME     = 254;
		public const int STR_LVLUPLD_ERR_DUPLNAME        = 271;

		public const int STR_SCCM_DOWNLOADFAILED         = 256;
		public const int STR_SCCM_DOWNLOADINPROGRESS     = 257;
		public const int STR_SCCM_VERSIONTOOOLD          = 258;
		public const int STR_SCCM_NEEDS_ACC              = 259;
		public const int STR_SCCM_UPLOADINFO             = 278;

		public const int STR_SCOREMAN_INFO_SCORE         = 260;
		public const int STR_SCOREMAN_INFO_MPSCORE       = 261;
		public const int STR_SCOREMAN_INFO_SCCMSCORE     = 262;
		public const int STR_SCOREMAN_INFO_STARS         = 263;

		public const int STR_ACH_UNLOCK_ONLINE           = 268;
		public const int STR_ACH_UNLOCK_MULTIPLAYER      = 269;
		public const int STR_ACH_UNLOCK_WORLD            = 270;
		public const int STR_ACH_FIRSTCLEAR              = 274;
		public const int STR_ACH_WORLDRECORD             = 275;

		private const int TEXT_COUNT = 279; // = next idx

		public static void Init(int lang)
		
			L10N.Init(lang, TEXT_COUNT, LANG_COUNT);

			// [en-US] [de-DE] [fr-FR] [it-IT] [es-ES]

			// Description: 
			L10N.Add(STR_SSB_ABOUT, "About", "Info", "About", "Informazioni", "Información");

			// Description: 
			// Usage:       SubSettingButton.cs:189
			L10N.Add(STR_SSB_ACCOUNT, "Account", "Benutzerkonto", "Account", "Account", "Cuenta");

			// Description: 
			// Usage:       SubSettingButton.cs:204
			L10N.Add(STR_SSB_HIGHSCORE, "Highscore", "Bestenliste", "Highscore", "Classifica", "Puntaje alto");

			// Description: 
			// Usage:       SubSettingButton.cs:219
			L10N.Add(STR_SSB_MUTE, "Mute", "Stumm", "Mute", "Muto", "Mute");

			// Description: 
			// Usage:       SubSettingButton.cs:248
			L10N.Add(STR_SSB_EFFECTS, "Effects", "Effekte", "Effects", "Effetti", "Efectos");

			// Description: 
			// Usage:       HighscorePanel.cs:62
			L10N.Add(STR_HSP_GLOBALRANKING, "Global Ranking", "Bestenliste", "Global Ranking", "Classifica Globale", "Ranking global");

			// Description: 
			// Usage:       HighscorePanel.cs:65
			L10N.Add(STR_HSP_RANKINGFOR, "Ranking for \"{0}\"", "Bestenliste für \"{0}\"", "Ranking for \"{0}\"", "Classifica per \"{0}\"", "Ranking");

			// Description: 
			// Usage:       HighscorePanel.cs:155
			// Usage:       HighscorePanel.cs:161
			// Usage:       HighscorePanel.cs:167
			// Usage:       HighscorePanel.cs:174
			// Usage:       HighscorePanel.cs:180
			L10N.Add(STR_TAB_NAME, "Name", "Name", "Name", "Nome", "Nombre");

			// Description: 
			// Usage:       HighscorePanel.cs:156
			// Usage:       HighscorePanel.cs:162
			// Usage:       HighscorePanel.cs:168
			// Usage:       HighscorePanel.cs:181
			L10N.Add(STR_TAB_POINTS, "Points", "Punkte", "Points", "Punti", "Puntos");

			// Description: 
			// Usage:       HighscorePanel.cs:169
			L10N.Add(STR_TAB_TIME, "Total Time", "Gesamtzeit", "Total Time", "Tempo Totale", "Tiempo total");

			// Description: 
			// Usage:       FullAccountPanel.cs:64
			L10N.Add(STR_FAP_ACCOUNT, "Account", "Benutzerkonto", "Account", "Account", "Cuenta");

			// Description: 
			// Usage:       FullAccountPanel.cs:78
			L10N.Add(STR_FAP_USERNAME, "Username:", "Benutzername:", "Username:", "Username:", "Usuario");

			// Description: 
			// Usage:       FullAccountPanel.cs:145
			L10N.Add(STR_FAP_CHANGEPW, "Change Password", "Passwort ändern", "Change Password", "Cambia Password", "Cambiar clave");

			// Description: 
			// Usage:       ChangePasswordPanel.cs:66
			L10N.Add(STR_CPP_CHANGEPW, "Change Password", "Passwort ändern", "Change Password", "Cambia Password", "Cambiar clave");

			// Description: 
			// Usage:       ChangePasswordPanel.cs:80
			L10N.Add(STR_CPP_USERNAME, "Username:", "Benutzername:", "Username:", "Username:", "Usuario");

			// Description: 
			// Usage:       ChangePasswordPanel.cs:105
			L10N.Add(STR_CPP_NEWPW, "New Password", "Neues Passwort", "New Password", "Nuova Password", "Nueva clave");

			// Description: 
			// Usage:       ChangePasswordPanel.cs:122
			L10N.Add(STR_CPP_CHANGE, "Change", "Ändern", "Change", "Cambia", "Cambiar");

			// Description: 
			// Usage:       ChangePasswordPanel.cs:148
			L10N.Add(STR_CPP_CHANGING, "Changing password", "Passwort wird geändert", "Changing password", "Cambiando la password", "Coambiando clave");

			// Description: 
			// Usage:       ChangePasswordPanel.cs:182
			L10N.Add(STR_CPP_CHANGED, "Password changed", "Passwort geändert", "Password changed", "Password cambiata", "Clave modificada");

			// Description: 
			// Usage:       LevelEditorScreen.cs:302
			// Usage:       LevelEditorScreen.cs:364
			// Usage:       ChangePasswordPanel.cs:211
			// Usage:       HUDSCCMUploadScorePanel.cs:323
			// Usage:       HUDSCCMUploadScorePanel.cs:416
			// Usage:       SCCMListElementNewUserLevel.cs:117
			// Usage:       SCCMListElementNewUserLevel.cs:143
			// Usage:       SCCMTabHot.cs:112
			// Usage:       SCCMTabNew.cs:112
			// Usage:       SCCMTabSearch.cs:201
			// Usage:       SCCMTabTop.cs:112
			L10N.Add(STR_CPP_COMERR, "Could not communicate with server", "Kommunikation mit Server ist gestört", "Could not communicate with server", "Impossibile comunicare con il server", "No se puede comunicar con Server");

			// Description: 
			// Usage:       ChangePasswordPanel.cs:225
			L10N.Add(STR_CPP_AUTHERR, "Authentication error", "Authentifizierung fehlgeschlagen", "Authentication error", "Errore di autenticazione", "Error de auntentificación");

			// Description: 
			// Usage:       ChangePasswordPanel.cs:246
			L10N.Add(STR_CPP_CHANGEERR, "Could not change password", "Passwort konnte nicht geändert werden", "Could not change password", "Impossibile cambiare password", "No se pudo cambiar la clave");

			// Description: 
			// Usage:       AttributionsPanel.cs:45
			// Usage:       InfoPanel.cs:64
			L10N.Add(STR_ATTRIBUTIONS, "Attributions", "Lizenzen", "Attributions", "Attribuzioni", "Atribuciones");

			// Description: 
			// Usage:       AccountReminderPanel.cs:70
			// Usage:       AnonymousAccountPanel.cs:67
			L10N.Add(STR_AAP_HEADER, "Sign up / Log in", "Anmelden / Registrieren", "Sign up / Log in", "Iscriviti / Accedi", "Registrarse / Iniciar sesión");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:80
			// Usage:       AuthErrorPanel.cs:80
			L10N.Add(STR_AAP_USERNAME, "Username", "Benutzername", "Username", "Username", "Usuario");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:100
			// Usage:       AuthErrorPanel.cs:101
			L10N.Add(STR_AAP_PASSWORD, "Password", "Passwort", "Password", "Password", "Clave");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:117
			L10N.Add(STR_AAP_CREATEACCOUNT, "Create Account", "Registrieren", "Create Account", "Crea Account", "Registrarse");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:135
			// Usage:       AuthErrorPanel.cs:118
			L10N.Add(STR_AAP_LOGIN, "Login", "Anmelden", "Login", "Accedi", "Iniciar sesión");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:164
			// Usage:       AuthErrorPanel.cs:169
			L10N.Add(STR_AAP_LOGGINGIN, "Logging in", "Wird angemeldet", "Logging in", "Accedendo", "Iniciando sesión");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:195
			// Usage:       AuthErrorPanel.cs:200
			L10N.Add(STR_AAP_WRONGPW, "Wrong password", "Falsches Passwort", "Wrong password", "Password sbagliata", "Clave incorrecta");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:196
			// Usage:       AuthErrorPanel.cs:201
			L10N.Add(STR_AAP_USERNOTFOUND, "User not found", "Benutzer nicht gefunden", "User not found", "Utente non trovato", "Usuario no encontrado");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:197
			// Usage:       AnonymousAccountPanel.cs:353
			// Usage:       AuthErrorPanel.cs:202
			L10N.Add(STR_AAP_NOCOM, "Could not communicate with server", "Konnte nicht mit Server kommunizieren", "Could not communicate with server", "Impossibile comunicare con il server", "No se puede comunicar con Server");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:223
			// Usage:       AuthErrorPanel.cs:228
			L10N.Add(STR_AAP_LOGINSUCCESS, "Successfully logged in", "Benutzer erfolgreich angemeldet", "Successfully logged in", "Accesso completo", "Sesión iniciada correctamente");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:243
			// Usage:       AuthErrorPanel.cs:248
			L10N.Add(STR_AAP_NOLOGIN, "Could not login", "Anmeldung fehlgeschlagen", "Could not login", "Impossibile accedere", "No se pudo iniciar sesión");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:264
			L10N.Add(STR_AAP_ACCCREATING, "Creating account", "Konto wird erstellt", "Creating account", "Creando l'account", "Registrando cuenta de usuario");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:297
			L10N.Add(STR_AAP_ACCCREATED, "Account created", "Konto erfolgreich erstellt", "Account created", "Account creato", "Usuario creado");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:312
			L10N.Add(STR_AAP_USERTAKEN, "Username already taken", "Benutzername bereits vergeben", "Username already taken", "Username già in uso", "Nombre de usuario no disponible");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:325
			L10N.Add(STR_AAP_ALREADYCREATED, "Account already created", "Konto bereits erstellt", "Account already created", "Account già creato", "Esta cuenta ya está registrada");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:367
			L10N.Add(STR_AAP_AUTHERROR, "Authentication error", "Authentifizierungsfehler", "Authentication error", "Errore di autenticazione", "Error de auntentificación");

			// Description: 
			// Usage:       AnonymousAccountPanel.cs:388
			L10N.Add(STR_AAP_COULDNOTCREATE, "Could not create account", "Konto konnte nicht erstellt werden", "Could not create account", "Impossibile creare account", "No se pudo crear cuenta");

			// Description: 
			// Usage:       HUDPauseButton.cs:139
			L10N.Add(STR_PAUS_RESUME, "RESUME", "WEITER", "RESUME", "CONTINUA", "Continuar");

			// Description: 
			// Usage:       HUDPauseButton.cs:140
			L10N.Add(STR_PAUS_RESTART, "RESTART", "NEU STARTEN", "RESTART", "RICOMINCIA", "Reiniciar");

			// Description: 
			// Usage:       HUDPauseButton.cs:141
			L10N.Add(STR_PAUS_EXIT, "EXIT", "BEENDEN", "EXIT", "ESCI", "Salir");

			// Description: 
			// Usage:       HUDMultiplayerScorePanel.cs:137
			// Usage:       HUDScorePanel.cs:130
			// Usage:       HUDTutorialScorePanel.cs:115
			L10N.Add(STR_HSP_LEVEL, "Level", "Level", "Level", "Livello", "Nivel");

			// Description: 
			// Usage:       HUDSCCMScorePanel_Lost.cs:204
			// Usage:       HUDSCCMScorePanel_Won.cs:298
			// Usage:       HUDScorePanel.cs:143
			// Usage:       HUDTutorialScorePanel.cs:128
			L10N.Add(STR_HSP_POINTS, "Points", "Punkte", "Points", "Punti", "Puntos");

			// Description: 
			L10N.Add(STR_HSP_PROGRESS, "Progress", "Fortschritt", "Progress", "Progresso", "Progreso");

			// Description: 
			// Usage:       HUDMultiplayerScorePanel.cs:206
			// Usage:       HUDSCCMScorePanel_Won.cs:153
			// Usage:       HUDSCCMTestScorePanel.cs:159
			// Usage:       HUDSCCMUploadScorePanel.cs:169
			// Usage:       HUDScorePanel.cs:210
			L10N.Add(STR_HSP_BACK, "Back", "Zurück", "Back", "Indietro", "Atrás");

			// Description: 
			// Usage:       HUDScorePanel.cs:236
			L10N.Add(STR_HSP_NEXT, "Next", "Weiter", "Next", "Avanti", "Siguiente");

			// Description: 
			// Usage:       HUDSCCMScorePanel_Lost.cs:126
			// Usage:       HUDSCCMScorePanel_Won.cs:173
			// Usage:       HUDSCCMScorePanel_Won.cs:422
			// Usage:       HUDSCCMTestScorePanel.cs:180
			// Usage:       HUDSCCMUploadScorePanel.cs:215
			// Usage:       HUDScorePanel.cs:261
			L10N.Add(STR_HSP_AGAIN, "Again", "Wiederholen", "Again", "Ripeti", "Repetir");

			// Description: 
			// Usage:       HUDTutorialScorePanel.cs:153
			L10N.Add(STR_HSP_TUTORIAL, "Tutorial", "Tutorial", "Tutorial", "Tutorial", "Tutorial");

			// Description: 
			// Usage:       HUDTutorialScorePanel.cs:215
			L10N.Add(STR_HSP_GETSTARTED, "Let's get started", "Los gehts", "Let's get started", "Iniziamo", "A comenzar");

			// Description: 
			// Usage:       FractionDifficulty.cs:180
			L10N.Add(STR_DIFF_0, "Easy", "Leicht", "Easy", "Facile", "Fácil");

			// Description: 
			// Usage:       FractionDifficulty.cs:182
			L10N.Add(STR_DIFF_1, "Normal", "Normal", "Normal", "Normale", "Normal");

			// Description: 
			// Usage:       FractionDifficulty.cs:184
			L10N.Add(STR_DIFF_2, "Hard", "Schwer", "Hard", "Difficile", "Difícil");

			// Description: 
			// Usage:       FractionDifficulty.cs:186
			L10N.Add(STR_DIFF_3, "Extreme", "Extrem", "Extreme", "Estremo", "Extremo");

			// Description: 
			// Usage:       TutorialAgent.cs:175
			L10N.Add(STR_TUT_INFO2, "Shoot it until it becomes your cannon", "Schieße bis die feindliche Kanone dir gehört", "Shoot it until it becomes your cannon", "Spara fino a che non diventa il tuo cannone", "Dispara hasta que se convierta en tu cañón");

			// Description: 
			// Usage:       TutorialAgent.cs:191
			L10N.Add(STR_TUT_INFO3, "Now capture the next cannon", "Erobere nun die nächste Einheit", "Now capture the next cannon", "Ora cattura il prossimo cannone", "Ahora captura tu próximo cañón");

			// Description: 
			// Usage:       TutorialAgent.cs:202
			L10N.Add(STR_TUT_INFO4, "Keep shooting at the first cannon to increase its fire rate", "Schieß auf deine eigene Kanone um ihre Feuerrate zu erhöhen", "Keep shooting at the first cannon to increase its fire rate", "Continua a sparare al primo cannone per aumentare la cadenza di fuoco", "Continúa disparando con tu primer cañón para aumentar su nivel de tiro");

			// Description: 
			// Usage:       TutorialAgent.cs:220
			L10N.Add(STR_TUT_INFO5, "The enemy has captured a cannon. Attack him!", "Der Gegner hat eine Einheit erobert, greif ihn an!", "The enemy has captured a cannon. Attack him!", "Il nemico ha conquistato un cannone. Attaccalo!", "El enemigo ha conquistado un cañón, atácalo");

			// Description: 
			// Usage:       TutorialAgent.cs:231
			L10N.Add(STR_TUT_INFO6, "Speed up the Game with the bottom left button.", "Mit dem Knopf unten links kannst du die Spielgeschwindigkeit erhöhen", "Speed up the Game with the bottom left button.", "Accellera il gioco con i pulsanti in basso a sinistra.", "Aumenta la velocidad de juego con el botón de abajo a la izquierda");

			// Description: 
			// Usage:       TutorialAgent.cs:247
			L10N.Add(STR_TUT_INFO7, "Now capture the next cannon", "Erobere jetzt die nächste Einheit", "Now capture the next cannon", "Ora cattura il prossimo cannone", "Ahora captura tu próximo cañón");

			// Description: 
			// Usage:       TutorialAgent.cs:262
			L10N.Add(STR_TUT_INFO8, "Win the game by capturing all enemy cannons", "Gewinne die Schlacht indem du alle Einheiten eroberst", "Win the game by capturing all enemy cannons", "Vinci il gioco catturando tutti i cannoni nemici", "Ganarás cuando hayas capturado todos los cañones");

			// Description: 
			// Usage:       GDServerAPI.cs:1557
			L10N.Add(STR_API_CONERR, "Could not connect to highscore server", "Verbindung mit Highscore Server fehlgeschlagen", "Could not connect to highscore server", "Impossibile connettersi al server della classifica", "No se puede comunicar con Server");

			// Description: 
			// Usage:       GDServerAPI.cs:1565
			L10N.Add(STR_API_COMERR, "Could not communicate with highscore server", "Kommunikation mit Highscore Server fehlgeschlagen", "Could not communicate with highscore server", "Impossibile comunicare al server della classifica", "No se puede comunicar con Server");

			// Description: 
			// Usage:       ExitAgent.cs:45
			L10N.Add(STR_GLOB_EXITTOAST, "Click again to exit game", "Drücke nochmal \"Zurück\" um das Spiel zu beenden", "Click again to exit game", "Clicca di nuovo per uscire dal gioco", "Presiona otra vez para finalizar el juego");

			// Description: 
			L10N.Add(STR_HSP_CONERROR, "Could not connect to server", "Kommunikation mit Server fehlgeschlagen", "Could not connect to server", "Impossibile connettersi al server della classifica", "No se puede comunicar con Server");

			// Description: 
			// Usage:       OverworldNode_W1.cs:30
			L10N.Add(STR_GLOB_UNLOCKTOAST1, "Click two times more to unlock", "Noch zweimal drücken um die Welt freizuschalten", "Click two times more to unlock", "Clicca due volte per sbloccare", "Presiona dos veces para desbloquear");

			// Description: 
			// Usage:       OverworldNode_W1.cs:38
			L10N.Add(STR_GLOB_UNLOCKTOAST2, "Click again to unlock", "Nochmal drücken um die Welt freizuschalten", "Click again to unlock", "Clicca di nuovo per sbloccare", "Presiona otra vez para desbloquear");

			// Description: 
			// Usage:       OverworldNode_W1.cs:47
			L10N.Add(STR_GLOB_UNLOCKTOAST3, "World unlocked", "Welt freigeschaltet", "World unlocked", "Mondo sbloccato", "Mundo desbloqueado");

			// Description: 
			// Usage:       SubSettingButton.cs:262
			L10N.Add(STR_SSB_LANGUAGE, "Language", "Sprache", "Language", "Linguaggio", "Idioma");

			// Description: 
			// Usage:       LevelNode.cs:244
			L10N.Add(STR_GLOB_LEVELLOCK, "Level locked", "Level noch nicht freigespielt", "Level locked", "Livello bloccato", "Nivel bloqueado");

			// Description: 
			// Usage:       OverworldNode_Graph.cs:253
			// Usage:       OverworldNode_Graph.cs:262
			// Usage:       OverworldNode_SCCM.cs:117
			// Usage:       WarpGameEndNode.cs:99
			// Usage:       WarpNode.cs:102
			L10N.Add(STR_GLOB_WORLDLOCK, "World locked", "Welt noch nicht freigespielt", "World locked", "Mondo bloccato", "Mundo bloqueado");

			// Description: 
			// Usage:       InformationDisplay.cs:46
			// Usage:       SCCMLevelPreviewDialog.cs:136
			// Usage:       SCCMListElementLocalPlayable.cs:91
			L10N.Add(STR_INF_YOU, "You", "Du", "You", "Tu", "Tú");

			// Description: 
			// Usage:       InformationDisplay.cs:48
			L10N.Add(STR_INF_GLOBAL, "Stats", "Total", "Stats", "Statistiche", "Estadística");

			// Description: 
			// Usage:       InformationDisplay.cs:47
			// Usage:       HUDSCCMScorePanel_Won.cs:440
			// Usage:       SCCMLevelPreviewDialog.cs:151
			L10N.Add(STR_INF_HIGHSCORE, "Highscore", "Bestzeit", "Highscore", "Highscore", "Puntaje alto");

			// Description: 
			// Usage:       RootNode.cs:165
			L10N.Add(STR_GLOB_OVERWORLD, "Overworld", "Übersicht", "Overworld", "Overworld", "Síntesis");

			// Description: 
			// Usage:       GDGameEndHUD.cs:67
			// Usage:       GDWorldHUD.cs:96
			// Usage:       SCCMListElementNewUserLevel.cs:166
			L10N.Add(STR_GLOB_WAITFORSERVER, "Contacting server", "Server wird kontaktiert", "Contacting server", "Contattando il server", "Conectando con Server");

			// Description: 
			// Usage:       Levels.cs:164
			// Usage:       OverworldNode_Tutorial.cs:22
			L10N.Add(STR_WORLD_TUTORIAL, "Tutorial", "Tutorial", "Tutorial", "Tutorial", "Tutorial");

			// Description: 
			// Usage:       FullAccountPanel.cs:104
			L10N.Add(STR_FAP_SCORE, "Points:", "Punkte:", "Points:", "Points:", "Puntos");

			// Description: 
			// Usage:       TutorialAgent.cs:165
			L10N.Add(STR_TUT_INFO1, "Drag to rotate your own cannons", "Drücke und Ziehe um deine Kanonen zu drehen", "Drag to rotate your own cannons", "Trascina per ruotare i tuoi cannoni", "Para rotar tus cañones presiona y tira");

			// Description: 
			// Usage:       Levels.cs:165
			L10N.Add(STR_WORLD_W1, "Basic", "Grundlagen", "Basic", "Base", "Principios básicos");

			// Description: 
			// Usage:       Levels.cs:166
			L10N.Add(STR_WORLD_W2, "Professional", "Fortgeschritten", "Professional", "Professionale", "Profesional");

			// Description: 
			// Usage:       UnlockManager.cs:264
			L10N.Add(STR_IAB_TESTERR, "Error connecting to Google Play services", "Fehler beim Versuch mit Google Play zu verbinden", "Error connecting to Google Play services", "Impossibile connettersi ai servizi Google Play", "Error en conexión con Google Play");

			// Description: 
			// Usage:       UnlockManager.cs:277
			L10N.Add(STR_IAB_TESTNOCONN, "No connection to Google Play services", "Keine Verbindung zu Google Play services", "No connection to Google Play services", "Nessuna connessione ai servizi Google Play", "Sin conexión a Google Play");

			// Description: 
			// Usage:       UnlockManager.cs:281
			L10N.Add(STR_IAB_TESTINPROGRESS, "Payment in progress", "Zahlung wird verarbeitet", "Payment in progress", "Pagamento in lavorazione", "Pago en progreso");

			// Description: 
			// Usage:       InfoPanel.cs:114
			// Usage:       UnlockPanel.cs:48
			L10N.Add(STR_UNLOCK, "Promotion Code", "Promo Code", "Promotion Code", "Codice Promozionale", "Código de promoción");

			// Description: 
			// Usage:       UnlockSucessOperation.cs:45
			L10N.Add(STR_GLOB_UNLOCKSUCCESS, "Upgraded game to full version!", "Spiel wurde zur Vollversion aufgewertet", "Upgraded game to full version!", "Congratulazioni, hai acquistato la versione completa!", "Has obtenido la versión completa del juego");

			// Description: 
			// Usage:       SCCMPreviewPanel.cs:93
			// Usage:       WorldPreviewPanel.cs:93
			L10N.Add(STR_PREV_BUYNOW, "Unlock now", "Jetzt freischalten", "Unlock now", "Sblocca ora", "Desbloquear ahora");

			// Description: 
			// Usage:       MultiplayerMainPanel.cs:274
			// Usage:       SCCMPreviewPanel.cs:170
			// Usage:       WorldPreviewPanel.cs:200
			L10N.Add(STR_IAB_BUYERR, "Error connecting to Google Play services", "Fehler beim Versuch mit Google Play zu verbinden", "Error connecting to Google Play services", "Impossibile connettersi ai servizi Google Play", "Error en conexión con Google Play");

			// Description: 
			// Usage:       MultiplayerMainPanel.cs:277
			// Usage:       SCCMPreviewPanel.cs:173
			// Usage:       WorldPreviewPanel.cs:203
			L10N.Add(STR_IAB_BUYNOCONN, "No connection to Google Play services", "Keine Verbindung zu Google Play services", "No connection to Google Play services", "Nessuna connessione ai servizi Google Play", "Sin conexión a Google Play");

			// Description: 
			// Usage:       MultiplayerMainPanel.cs:280
			// Usage:       SCCMPreviewPanel.cs:176
			// Usage:       WorldPreviewPanel.cs:206
			L10N.Add(STR_IAB_BUYNOTREADY, "Connection to Google Play services not ready", "Verbindung zu Google Play services nicht bereit", "Connection to Google Play services not ready", "Connessione ai servizi Google Play non pronta", "Conexión a Google Play no disponible");

			// Description: 
			// Usage:       SCCMPreviewPanel.cs:147
			// Usage:       SCCMPreviewPanel.cs:156
			// Usage:       WorldPreviewPanel.cs:170
			// Usage:       WorldPreviewPanel.cs:179
			L10N.Add(STR_IAB_BUYSUCESS, "World successfully purchased", "Levelpack wurde erfolgreich erworben", "World successfully purchased", "Mondo acquistato!", "Has comprado el mundo exitosamente");

			// Description: 
			// Usage:       Levels.cs:167
			L10N.Add(STR_WORLD_W3, "Futuristic", "Futuristisch", "Futuristic", "Futuristico", "Futurístico");

			// Description: 
			// Usage:       FullAccountPanel.cs:126
			L10N.Add(STR_FAP_LOGOUT, "Logout", "Ausloggen", "Logout", "Esci", "Cerrar sesión");

			// Description: 
			// Usage:       FullAccountPanel.cs:256
			L10N.Add(STR_FAP_WARN1, "This will clear all local data. Press again to log out.", "Dies löscht alle lokalen Daten. Nochmal drücken zum ausloggen.", "This will clear all local data. Press again to log out.", "Questo cancellerà tutti i dati locali. Premi di nuovo per uscire.", "Esto borra todos los datos locales. Presiona otra vey para cerrar sesión");

			// Description: 
			// Usage:       FullAccountPanel.cs:265
			L10N.Add(STR_FAP_WARN2, "Are you really sure you want to log out?", "Wirklich vom Serverkonto abmelden?", "Are you really sure you want to log out?", "Sei sicuro di voler uscire?", "Estas seguro que quieres cerrar sesión");

			// Description: 
			// Usage:       FullAccountPanel.cs:287
			L10N.Add(STR_FAP_LOGOUT_SUCESS, "Logged out from account", "Lokaler Benutzer wurde abgemeldet.", "Logged out from account", "Uscito dall'account", "Usuario desconectado");

			// Description: 
			L10N.Add(STR_HINT_001, "Tip: Shoot stuff to win!", "Tipp: Versuche auf die andere Kanone zu schiessen", "Tip: Shoot stuff to win!", "Consiglio: Spara per vincere!", "Consejo: dispara al otro cañón");

			// Description: 
			L10N.Add(STR_HINT_002, "Bigger Cannon", "Größere Kanone", "Bigger Cannon", "Cannone più grande", "Cañón más grande");

			// Description: 
			L10N.Add(STR_HINT_003, "More Power", "Mehr Schaden", "More Power", "Più potenza", "Más potencia");

			// Description: 
			L10N.Add(STR_HINT_004, "Black holes attract your bullets", "Schwarze Löcher saugen deine Kugeln ein", "Black holes attract your bullets", "I buchi neri attraggono i tuoi proiettili", "Hoyos negros succionan tus balas ");

			// Description: 
			L10N.Add(STR_HINT_005, "Lasers!", "Laser!", "Lasers!", "Laser!", "Lásers!");

			// Description: 
			L10N.Add(STR_HINT_006, "Try dragging the map around", "Versuche mal die Karte zu verschieben", "Try dragging the map around", "Prova a trascinare la mappa", "Intenta desplazar el mapa");

			// Description: 
			// Usage:       Levels.cs:168
			L10N.Add(STR_WORLD_W4, "Toy Box", "Spielzeugkiste", "Toy Box", "Svago", "Caja de juegos");

			// Description: 
			// Usage:       InformationDisplay.cs:193
			// Usage:       InformationDisplay.cs:219
			// Usage:       InformationDisplay.cs:246
			// Usage:       InformationDisplay.cs:273
			L10N.Add(STR_INFOTOAST_1, "Your best time is {0}", "Deine Bestzeit ist {0}", "Your best time is {0}", "Il tuo tempo migliore è {0}", "Tu mejor tiempo");

			// Description: 
			// Usage:       InformationDisplay.cs:199
			// Usage:       InformationDisplay.cs:225
			// Usage:       InformationDisplay.cs:252
			// Usage:       InformationDisplay.cs:279
			L10N.Add(STR_INFOTOAST_2, "The global best time is {0}", "Die globale Bestzeit ist {0}", "The global best time is {0}", "Il tempo assoluto migliore è {0}", "El récord de tiempo");

			// Description: 
			// Usage:       InformationDisplay.cs:203
			// Usage:       InformationDisplay.cs:229
			// Usage:       InformationDisplay.cs:256
			// Usage:       InformationDisplay.cs:283
			L10N.Add(STR_INFOTOAST_3, "{0} users have completed this level on {1}", "{0} Spieler haben dieses Level auf {1} geschafft", "{0} users have completed this level on {1}", "{0} utenti hanno completato questo livello in {1}", "{0} Jugadores han completado este nivel en {1}");

			// Description: 
			// Usage:       InformationDisplay.cs:195
			// Usage:       InformationDisplay.cs:221
			// Usage:       InformationDisplay.cs:248
			// Usage:       InformationDisplay.cs:275
			L10N.Add(STR_INFOTOAST_4, "You have not completed this level on {0}", "Du hast dieses Level auf {0} noch nicht geschafft", "You have not completed this level on {0}", "Non hai completato questo livello in {0}", "No has completado este nivel en {0}");

			// Description: 
			// Usage:       WorldPreviewPanel.cs:126
			L10N.Add(STR_PREV_FINISHWORLD, "Finish World {0}", "Welt {0}", "Finish World {0}", "Finisci il mondo {0}", "Finalizar el mundo");

			// Description: 
			// Usage:       SCCMPreviewPanel.cs:112
			// Usage:       WorldPreviewPanel.cs:112
			L10N.Add(STR_PREV_OR, "OR", "ODER", "OR", "oppure", "O");

			// Description: 
			// Usage:       WorldPreviewPanel.cs:233
			L10N.Add(STR_PREV_MISS_TOAST, "You are missing {0} points to unlock world {1}", "Dir fehlen noch {0} Punkte um Welt {1} freizuschalten", "You are missing {0} points to unlock world {1}", "Ti mancano {0} punti per sbloccare il mondo {1}", "Te faltan {0} puntos para desbloquear el mundo {1}");

			// Description: 
			// Usage:       SubSettingButton.cs:233
			L10N.Add(STR_SSB_MUSIC, "Music", "Musik", "Music", "Musica", "Música");

			// Description: 
			L10N.Add(STR_HINT_007, "Speedy thing goes in,", "Speedy thing goes in,", "Speedy thing goes in,", "Cosina veloce entra,", "Speedy thing entra");

			// Description: 
			L10N.Add(STR_HINT_008, "speedy thing comes out.", "speedy thing comes out.", "speedy thing comes out.", "cosina veloce esce.", "Speedy thing sale");

			// Description: 
			// Usage:       OverworldNode_MP.cs:38
			L10N.Add(STR_WORLD_MULTIPLAYER, "Multiplayer", "Mehrspieler", "Multiplayer", "Multiplayer", "Multijugador");

			// Description: 
			// Usage:       L10NImplHelper.cs:16
			// Usage:       L10NImplHelper.cs:19
			L10N.Add(STR_MP_TIMEOUT, "Timeout - Connection to server lost", "Timeout - Verbindung zu server verloren", "Timeout - Connection to server lost", "Timeout - Connessione al server persa", "Timeout - sin conexión a Server");

			// Description: 
			// Usage:       L10NImplHelper.cs:22
			// Usage:       HUDMultiplayerScorePanel.cs:328
			L10N.Add(STR_MP_TIMEOUT_USER, "Timeout - Connection to user [{0}] lost", "Timeout - Verbindung zu Spieler [{0}] verloren", "Timeout - Connection to user [{0}] lost", "Timeout - Connection all'utente [{0}] persa", "Timeout - sin conexión a usuario");

			// Description: 
			// Usage:       L10NImplHelper.cs:25
			L10N.Add(STR_MP_NOTINLOBBY, "You a not part of this session", "Du bist kein Teilnehmer dieser Sitzung", "You a not part of this session", "Non fai parte di questa sessione", "No eres parte de esta sesión");

			// Description: 
			// Usage:       L10NImplHelper.cs:28
			L10N.Add(STR_MP_SESSIONNOTFOUND, "Session on server not found", "Sitzung konnte auf dem Server nicht gefunden werden", "Session on server not found", "Nessuna sessione trovata su questo server", "Sesión no pudo ser encontrada");

			// Description: 
			// Usage:       L10NImplHelper.cs:31
			L10N.Add(STR_MP_AUTHFAILED, "Authentification on server failed", "Authentifizierung auf Server fehlgeschlagen", "Authentification on server failed", "Autenticazione sul server fallita", "Autentificación en Server ha fallado");

			// Description: 
			// Usage:       L10NImplHelper.cs:34
			L10N.Add(STR_MP_LOBBYFULL, "Server lobby is full", "Serverlobby ist voll", "Server lobby is full", "La lobby del server è piena", "Server lobby está lleno");

			// Description: 
			// Usage:       L10NImplHelper.cs:37
			L10N.Add(STR_MP_VERSIONMISMATCH, "Server has a different game version ({0})", "Serverversion unterscheidet sich von lokaler Version ({0})", "Server has a different game version ({0})", "Il server ha una versione diversa del gioco ({0})", "Server tiene una versión de juego diferente ({0})");

			// Description: 
			// Usage:       L10NImplHelper.cs:40
			L10N.Add(STR_MP_LEVELNOTFOUND, "Could not find server level locally", "Level konnte lokal nicht gefunden werden", "Could not find server level locally", "Impossibile trovare il livello nel server sul telefono", "Nivel no pudo ser encontrado localmente");

			// Description: 
			// Usage:       L10NImplHelper.cs:43
			L10N.Add(STR_MP_LEVELMISMATCH, "Server has different version of level", "Level auf dem Server unterscheidet sich von lokaler Version", "Server has different version of level", "Il server ha una versione diversa del livello", "Server tiene un nivel de versión diferente");

			// Description: 
			// Usage:       L10NImplHelper.cs:46
			L10N.Add(STR_MP_USERDISCONNECT, "User {0} has disconnected", "Der Benutzer {0} hat die Verbindung getrennt", "User {0} has disconnected", "L'utente {0} si è disconnesso", "Usuario {0} se ha desconectado");

			// Description: 
			// Usage:       L10NImplHelper.cs:49
			L10N.Add(STR_MP_SERVERDISCONNECT, "Server has closed this session", "Spiel wurde vom Server geschlossen", "Server has closed this session", "Il server ha chiuso questa sessione", "Server ha cerrado esta sesión");

			// Description: 
			// Usage:       MultiplayerMainPanel.cs:53
			L10N.Add(STR_MENU_CAP_MULTIPLAYER, "Multiplayer", "Mehrspieler", "Multiplayer", "Multiplayer", "Multijugador");

			// Description: 
			// Usage:       MultiplayerClientLobbyPanel.cs:71
			// Usage:       MultiplayerServerLobbyPanel.cs:68
			L10N.Add(STR_MENU_CAP_LOBBY, "Multiplayer Lobby", "Lobby", "Multiplayer Lobby", "Multiplayer Lobby", "Lobby mas de un jugador");

			// Description: 
			// Usage:       MultiplayerHostPanel.cs:86
			// Usage:       MultiplayerRehostPanel.cs:86
			L10N.Add(STR_MENU_CAP_CGAME_PROX, "Create Online Game", "Onlinespiel erstellen", "Create Online Game", "Crea una partita Online", "Crear juego online");

			// Description: 
			// Usage:       MultiplayerConnectionStateControl.cs:71
			L10N.Add(STR_MP_ONLINE, "Online", "Online", "Online", "Online", "Online");

			// Description: 
			// Usage:       MultiplayerConnectionStateControl.cs:68
			L10N.Add(STR_MP_OFFLINE, "Offline", "Offline", "Offline", "Offline", "Offline");

			// Description: 
			// Usage:       OverworldNode_SCCM.cs:100
			// Usage:       MultiplayerConnectionStateControl.cs:74
			L10N.Add(STR_MP_CONNECTING, "Connecting", "Verbinden", "Connecting", "Connessione", "Conectando");

			// Description: 
			// Usage:       MultiplayerMainPanel.cs:127
			// Usage:       MultiplayerMainPanel.cs:146
			L10N.Add(STR_MENU_MP_JOIN, "Join", "Beitreten", "Join", "Partecipa", "Participar");

			// Description: 
			// Usage:       MultiplayerMainPanel.cs:167
			// Usage:       MultiplayerMainPanel.cs:186
			// Usage:       MultiplayerMainPanel.cs:207
			// Usage:       MultiplayerMainPanel.cs:226
			L10N.Add(STR_MENU_MP_HOST, "Host", "Erstellen", "Host", "Ospita", "Crear");

			// Description: 
			// Usage:       MultiplayerMainPanel.cs:85
			L10N.Add(STR_MENU_MP_LOCAL_CLASSIC, "Local (Bluetooth)", "Lokal (Bluetooth)", "Local (Bluetooth)", "Locale (Bluetooth)", "Local (Bluetooth)");

			// Description: 
			// Usage:       MultiplayerMainPanel.cs:99
			L10N.Add(STR_MENU_MP_ONLINE, "Online (UDP/IP)", "Internet (UDP/IP)", "Online (UDP/IP)", "Online (UDP/IP)", "Online (UDP/IP)");

			// Description: 
			// Usage:       MultiplayerJoinLobbyScreen.cs:63
			L10N.Add(STR_MENU_CAP_AUTH, "Enter lobby code", "Lobby Code eingeben", "Enter lobby code", "Inserisci codice lobby", "Ingresa código lobby");

			// Description: 
			// Usage:       MultiplayerHostPanel.cs:380
			L10N.Add(STR_MENU_MP_CREATE, "Create", "Start", "Create", "Crea", "Iniciar");

			// Description: 
			// Usage:       AuthErrorPanel.cs:136
			// Usage:       MultiplayerFindLobbyScreen.cs:95
			// Usage:       MultiplayerHostPanel.cs:399
			// Usage:       MultiplayerRehostPanel.cs:399
			// Usage:       LevelEditorConfigPanel.cs:276
			L10N.Add(STR_MENU_CANCEL, "Cancel", "Abbrechen", "Cancel", "Cancella", "Cancelar");

			// Description: 
			// Usage:       MultiplayerHostPanel.cs:308
			// Usage:       MultiplayerRehostPanel.cs:308
			L10N.Add(STR_MENU_MP_GAMESPEED, "Game speed:", "Spielgeschwindigkeit:", "Game speed:", "Velocità gioco:", "Velocidad de juego");

			// Description: 
			// Usage:       MultiplayerHostPanel.cs:229
			// Usage:       MultiplayerRehostPanel.cs:229
			L10N.Add(STR_MENU_MP_MUSIC, "Background music:", "Hintergrundmusik:", "Background music:", "Musica:", "Música de fondo");

			// Description: 
			// Usage:       MultiplayerServerLobbyPanel.cs:126
			L10N.Add(STR_MENU_MP_LOBBYINFO, "Enter this code on another phone to join this session.", "Gib diesen Code auf einem anderen Smartphone ein, um diesem Spiel beizutreten", "Enter this code on another phone to join this session.", "Inserire questo codice su un altro telefono per unirsi alla sessione.", "Ingresa este código en otro Smartphone para unirte al juego");

			// Description: 
			// Usage:       MultiplayerClientLobbyPanel.cs:101
			// Usage:       MultiplayerServerLobbyPanel.cs:151
			L10N.Add(STR_MENU_MP_LOBBY_USER, "Users:", "Mitspieler:", "Users:", "Utenti:", "Usuarios:");

			// Description: 
			// Usage:       MultiplayerClientLobbyPanel.cs:113
			// Usage:       MultiplayerServerLobbyPanel.cs:163
			L10N.Add(STR_MENU_MP_LOBBY_LEVEL, "Level:", "Level:", "Level:", "Livello:", "Nivel");

			// Description: 
			// Usage:       MultiplayerClientLobbyPanel.cs:125
			// Usage:       MultiplayerServerLobbyPanel.cs:175
			L10N.Add(STR_MENU_MP_LOBBY_MUSIC, "Background music:", "Hintergrundmusik:", "Background music:", "Musica:", "Música de fondo");

			// Description: 
			// Usage:       MultiplayerClientLobbyPanel.cs:137
			// Usage:       MultiplayerServerLobbyPanel.cs:187
			L10N.Add(STR_MENU_MP_LOBBY_SPEED, "Game speed:", "Spielgeschwindigkeit:", "Game speed:", "Velocità gioco:", "Velocidad de juego");

			// Description: 
			// Usage:       MultiplayerClientLobbyPanel.cs:89
			// Usage:       MultiplayerServerLobbyPanel.cs:139
			L10N.Add(STR_MENU_MP_LOBBY_PING, "Ping", "Ping", "Ping", "Ping", "Ping");

			// Description: 
			// Usage:       MultiplayerClientLobbyPanel.cs:266
			// Usage:       MultiplayerServerLobbyPanel.cs:306
			L10N.Add(STR_MENU_DISCONNECT, "Disconnect", "Verbindung trennen", "Disconnect", "Disconnetti", "Desconectar");

			// Description: 
			// Usage:       MultiplayerHostPanel.cs:215
			// Usage:       MultiplayerRehostPanel.cs:215
			L10N.Add(STR_MENU_MP_LOBBY_USER_FMT, "Users: {0}", "Mitspieler: {0}", "Users: {0}", "Utenti: {0}", "Usuario: {0}");

			// Description: 
			// Usage:       MultiplayerRehostPanel.cs:380
			// Usage:       MultiplayerServerLobbyPanel.cs:325
			L10N.Add(STR_MENU_MP_START, "Start", "Start", "Start", "Inizia", "Iniciar");

			// Description: 
			// Usage:       Fraction.cs:21
			L10N.Add(STR_FRAC_N0, "Gray", "Gray", "Gray", "Grigio", "Gris");

			// Description: 
			// Usage:       Fraction.cs:22
			L10N.Add(STR_FRAC_P1, "Green", "Grün", "Green", "Verde", "Verde");

			// Description: 
			// Usage:       Fraction.cs:23
			L10N.Add(STR_FRAC_A2, "Red", "Rot", "Red", "Rosso", "Rojo");

			// Description: 
			// Usage:       Fraction.cs:24
			L10N.Add(STR_FRAC_A3, "Blue", "Blau", "Blue", "Blu", "Azul");

			// Description: 
			// Usage:       Fraction.cs:25
			L10N.Add(STR_FRAC_A4, "Purple", "Lila", "Purple", "Viola", "Púrpura");

			// Description: 
			// Usage:       Fraction.cs:26
			L10N.Add(STR_FRAC_A5, "Orange", "Orange", "Orange", "Arancione", "Naranja");

			// Description: 
			// Usage:       Fraction.cs:27
			L10N.Add(STR_FRAC_A6, "Teal", "BlauGrün", "Teal", "Turchese", "Turquesa");

			// Description: 
			// Usage:       MultiplayerClientLobbyPanel.cs:149
			// Usage:       MultiplayerServerLobbyPanel.cs:199
			L10N.Add(STR_MENU_MP_LOBBY_COLOR, "Color", "Farbe", "Color", "Colore", "Colores");

			// Description: 
			// Usage:       HUDMultiplayerScorePanel.cs:248
			L10N.Add(STR_HSP_NEWGAME, "New Game", "Neues Spiel", "New Game", "Nuova partita", "Nuevo juego");

			// Description: 
			// Usage:       HUDMultiplayerScorePanel.cs:228
			L10N.Add(STR_HSP_RANDOMGAME, "Random level", "Zufälliges Level", "Random level", "Livello casuale", "Nivel aleatorio");

			// Description: 
			// Usage:       HUDMultiplayerScorePanel.cs:163
			L10N.Add(STR_HSP_MPPOINTS, "Multiplayer score", "Mehrspieler Punkte", "Multiplayer score", "Punteggio Multiplayer", "Puntaje Multiplayer");

			// Description: 
			// Usage:       L10NImplHelper.cs:58
			L10N.Add(STR_MP_INTERNAL, "Internal multiplayer error", "Interner Fehler im Mehrspielermodul", "Internal multiplayer error", "Problema intero di multiplayer", "Error en módulo mas de un jugador");

			// Description: 
			// Usage:       L10NImplHelper.cs:52
			L10N.Add(STR_MP_BTADAPTERNULL, "No bluetooth hardware found", "Bluetooth Hardware nicht gefunden", "No bluetooth hardware found", "Nessun apparecchio bluetooth trovato", "Bluetooth no encontrado");

			// Description: 
			// Usage:       L10NImplHelper.cs:61
			L10N.Add(STR_MP_BTDISABLED, "Bluetooth is disabled", "Bluetooth ist deaktiviert", "Bluetooth is disabled", "Bluetooth disattivatp", "Bluetooth no disponible");

			// Description: 
			// Usage:       L10NImplHelper.cs:67
			L10N.Add(STR_MP_DIRECTCONNLOST, "Bluetooth connection lost", "Bluetooth Verbindung verloren", "Bluetooth connection lost", "Connessione Bluetooth persa", "Conexión a Bluetooth perdida");

			// Description: 
			// Usage:       L10NImplHelper.cs:64
			L10N.Add(STR_MP_DIRECTCONNFAIL, "Bluetooth connection failed", "Bluetooth Verbindungsaufbau fehlgeschlagen", "Bluetooth connection failed", "Connessione Bluetooth fallita", "Conexión a Bluetooth ha fallado");

			// Description: 
			// Usage:       MultiplayerFindLobbyScreen.cs:61
			L10N.Add(STR_MENU_CAP_SEARCH, "Search for local devices", "Suche nach lokalem Spiel", "Search for local devices", "Cerca dispostivi locali", "Buscando dispositivos locales");

			// Description: 
			// Usage:       HighscorePanel.cs:68
			L10N.Add(STR_HSP_MULTIPLAYERRANKING, "Multiplayer", "Mehrspieler", "Multiplayer", "Multiplayer", "Multijugador");

			// Description: 
			// Usage:       L10NImplHelper.cs:55
			L10N.Add(STR_MP_BTADAPTERPERMDENIED, "Missing bluetooth permission", "Bluetooth Berechtigung wurde nicht gewährt", "Missing bluetooth permission", "Permessi bluetooth disattivati", "Autorización de Bluetooth desactivada");

			// Description: 
			// Usage:       MultiplayerHostPanel.cs:86
			// Usage:       MultiplayerRehostPanel.cs:86
			L10N.Add(STR_MENU_CAP_CGAME_BT, "Create Local Game", "Lokales Spiel erstellen", "Create Local Game", "Crea una partita Local", "Crear un juego local");

			// Description: 
			// Usage:       GDMultiplayerCommon.cs:94
			L10N.Add(STR_MP_TOAST_CONN_TRY, "Connecting to '{0}'", "Verbinden mit '{0}'", "Connecting to '{0}'", "Connessione a '{0}'", "Conectando a");

			// Description: 
			// Usage:       GDMultiplayerCommon.cs:98
			L10N.Add(STR_MP_TOAST_CONN_FAIL, "Connection to '{0}' failed", "Verbindung mit '{0}' fehlgeschlagen", "Connection to '{0}' failed", "Connessione a '{0}' fallita", "Conección a '{0}' ha fallado");

			// Description: 
			// Usage:       GDMultiplayerCommon.cs:102
			L10N.Add(STR_MP_TOAST_CONN_SUCC, "Connected to '{0}'", "Verbunden mit '{0}'", "Connected to '{0}'", "Connessione a '{0}'", "Conectado a '{0}'");

			// Description: 
			L10N.Add(STR_HINT_009, "Some cannons only relay", "Manche Kanonen leiten nur weiter", "Some cannons only relay", "Alcuni cannoni collegano e basta", "Algunos cañones solo redireccionan");

			// Description: 
			// Usage:       GDEndGameScreen.cs:60
			L10N.Add(STR_ENDGAME_1, "THANKS FOR", "THANKS FOR", "THANKS FOR", "GRAZIE PER", "GRACIAS POR");

			// Description: 
			// Usage:       GDEndGameScreen.cs:67
			L10N.Add(STR_ENDGAME_2, "PLAYING", "PLAYING", "PLAYING", "AVER GIOCATO", "JUGANDO");

			// Description: 
			L10N.Add(STR_HINT_010, "Shields can", "Schilde können", "Shields can", "Gli scudi", "Los escudos");

			// Description: 
			L10N.Add(STR_HINT_011, "protect you", "dich beschützen", "protect you", "ti proteggono", "pueden protegerte");

			// Description: 
			L10N.Add(STR_WORLD_SINGLEPLAYER, "Singleplayer", "Einzelspieler", "Singleplayer", "Singleplayer", "Juego individual");

			// Description: 
			// Usage:       AccountReminderPanel.cs:87
			L10N.Add(STR_ACCOUNT_REMINDER, "You can create an account to display your name in the highscore and to backup your score online.\nDo you want to create an account now?", "Du kannst einen Onlineaccount anlegen um deinen Namen im Highscore zu sehen und deine Punkte zu sichern.\n Account jetzt erstellen?", "You can create an account to display your name in the highscore and to backup your score online.\nDo you want to create an account now?", "Puoi creare un account per far apparire il tuo nome nella classifica e per salvare i tuoi dati online.\nVuoi creare un'account?", "Puedes crear una cuenta online para registrar tu puntuación y guardar tus puntajes altos.\nCrear cuenta ahora?");

			// Description: 
			// Usage:       AccountReminderPanel.cs:96
			// Usage:       LevelEditorConfigPanel.cs:295
			L10N.Add(STR_BTN_OK, "OK", "OK", "OK", "OK", "OK");

			// Description: 
			// Usage:       AccountReminderPanel.cs:115
			L10N.Add(STR_BTN_NO, "No", "Nein", "No", "No", "No");

			// Description: 
			// Usage:       L10NImplHelper.cs:70
			L10N.Add(STR_MP_NOSERVERCONN, "No connection to server", "Keine Verbindung zu Server", "No connection to server", "Nessuna connessione al server", "Sin conexión a Server");

			// Description: 
			// Usage:       HUDSCCMScorePanel_Won.cs:311
			// Usage:       HUDSCCMScorePanel_Won.cs:445
			// Usage:       HUDScorePanel.cs:156
			// Usage:       HUDScorePanel.cs:347
			L10N.Add(STR_HSP_TIME_NOW, "Time (now)", "Levelzeit", "Time (now)", "Tempo", "Tiempo actual");

			// Description: 
			// Usage:       HUDScorePanel.cs:348
			// Usage:       HUDScorePanel.cs:352
			// Usage:       HUDTutorialScorePanel.cs:141
			L10N.Add(STR_HSP_TIME_BEST, "Time (best)", "Bestzeit", "Time (best)", "Miglior tempo", "Mejor tiempo");

			// Description: 
			// Usage:       SubSettingButton.cs:316
			L10N.Add(STR_SSB_COLOR, "Color scheme", "Farbschema", "Color scheme", "Schema a colori", "Esquema de colores");

			// Description: 
			// Usage:       GDSounds.cs:109
			L10N.Add(STR_ERR_SOUNDPLAYBACK, "Sound playback failed. Disabling sounds ...", "Soundwiedergabe fehlgeschlagen. Sounds werden deaktiviert ...", "Sound playback failed. Disabling sounds ...", "La riproduzione audio non è riuscita. Disattivazione dei suoni ...", "Reproducción de sonido ha fallado. Deactivando sonidos");

			// Description: 
			// Usage:       GDSounds.cs:121
			L10N.Add(STR_ERR_MUSICPLAYBACK, "Music playback failed. Disabling music ...", "Musikwiedergabe fehlgeschlagen. Musik wird deaktiviert ...", "Music playback failed. Disabling music ...", "La riproduzione musicale è fallita. Disattivazione della musica ...", "Reproducción de música ha fallado. Deactivando sonidos");

			// Description: 
			// Usage:       AcknowledgementsPanel.cs:54
			// Usage:       InfoPanel.cs:89
			L10N.Add(STR_ACKNOWLEDGEMENTS, "Acknowledgements", "Danksagungen", "Acknowledgements", "Ringraziamenti", "Agradecimientos");

			// Description: 
			// Usage:       MainGame.cs:357
			// Usage:       SCCMLevelData.cs:189
			L10N.Add(STR_ERR_OUTOFMEMORY, "Saving failed: Disk full", "Speichern fehlgeschlagen: Speicher voll", "Saving failed: Disk full", "Salvataggio non riuscito: disco pieno", "Memoria llena: almacenamiento ha fallado ");

			// Description: 
			// Usage:       FullAccountPanel.cs:184
			L10N.Add(STR_PROFILESYNC_START, "Starting manual sync", "Manuelle Synchronisation gestartet", "Starting manual sync", "La sincronizzazione manuale è iniziata", "Inicio de sincronización manual");

			// Description: 
			// Usage:       FullAccountPanel.cs:204
			// Usage:       FullAccountPanel.cs:220
			L10N.Add(STR_PROFILESYNC_ERROR, "Manual sync failed", "Manuelle Synchronisation fehlgeschlagen", "Manual sync failed", "La sincronizzazione manuale non è riuscita", "La sincronización manual falló");

			// Description: 
			// Usage:       FullAccountPanel.cs:233
			L10N.Add(STR_PROFILESYNC_SUCCESS, "Manual sync finished", "Manuelle Synchronisation erfolgreich", "Manual sync finished", "La sincronizzazione manuale è riuscita", "Sincronización manual finalizada");

			// Description: 
			// Usage:       AuthErrorPanel.cs:67
			L10N.Add(STR_AUTHERR_HEADER, "You've been logged out", "Sie wurden ausgeloggt", "You've been logged out", "Sei stato disconnesso", "Has sido desconectado");

			// Description: 
			// Usage:       OverworldNode_SCCM.cs:35
			L10N.Add(STR_WORLD_ONLINE, "Online", "Online", "Online", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorModePanel.cs:73
			L10N.Add(STR_LVLED_MOUSE, "Move", "Verschieben", "Move", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorModePanel.cs:92
			L10N.Add(STR_LVLED_CANNON, "Cannon", "Kanone", "Cannon", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorModePanel.cs:111
			L10N.Add(STR_LVLED_WALL, "Wall", "Wand", "Wall", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorModePanel.cs:130
			L10N.Add(STR_LVLED_OBSTACLE, "Obstacle", "Hindernis", "Obstacle", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorModePanel.cs:168
			L10N.Add(STR_LVLED_SETTINGS, "Settings", "Einstellungen", "Settings", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorModePanel.cs:187
			L10N.Add(STR_LVLED_PLAY, "Test", "Testen", "Test", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorModePanel.cs:206
			L10N.Add(STR_LVLED_UPLOAD, "Upload", "Hochladen", "Upload", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorModePanel.cs:244
			L10N.Add(STR_LVLED_EXIT, "Exit", "Beenden", "Exit", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       CannonStub.cs:269
			L10N.Add(STR_LVLED_BTN_FRAC, "Fraction", "Fraktion", "Fraction", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       CannonStub.cs:251
			L10N.Add(STR_LVLED_BTN_SCALE, "Scale", "Größe", "Scale", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       CannonStub.cs:260
			// Usage:       ObstacleStub.cs:263
			// Usage:       PortalStub.cs:172
			L10N.Add(STR_LVLED_BTN_ROT, "Rotation", "Drehung", "Rotation", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       CannonStub.cs:242
			// Usage:       ObstacleStub.cs:222
			// Usage:       WallStub.cs:159
			L10N.Add(STR_LVLED_BTN_TYPE, "Type", "Typ", "Type", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       ObstacleStub.cs:233
			L10N.Add(STR_LVLED_BTN_DIAMETER, "Diameter", "Durchmesser", "Diameter", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       ObstacleStub.cs:245
			L10N.Add(STR_LVLED_BTN_WIDTH, "Width", "Breite", "Width", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       ObstacleStub.cs:254
			L10N.Add(STR_LVLED_BTN_HEIGHT, "Height", "Höhe", "Height", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       ObstacleStub.cs:275
			L10N.Add(STR_LVLED_BTN_POWER, "Power", "Stärke", "Power", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorAttrPanel.cs:91
			// Usage:       LevelEditorModePanel.cs:225
			L10N.Add(STR_LVLED_BTN_DEL, "Delete", "Entfernen", "Delete", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorConfigPanel.cs:89
			L10N.Add(STR_LVLED_CFG_ID, "ID", "ID", "ID", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorConfigPanel.cs:118
			L10N.Add(STR_LVLED_CFG_NAME, "Name", "Name", "Name", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorConfigPanel.cs:150
			L10N.Add(STR_LVLED_CFG_SIZE, "Size", "Größe", "Size", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorConfigPanel.cs:182
			L10N.Add(STR_LVLED_CFG_VIEW, "Initial view", "Startansicht", "Initial view", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorConfigPanel.cs:214
			L10N.Add(STR_LVLED_CFG_GEOMETRY, "Geometry", "Geometrie", "Geometry", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorConfigPanel.cs:385
			L10N.Add(STR_LVLED_CFG_WRAP_INFINITY, "Infinity", "Endlos", "Infinity", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorConfigPanel.cs:388
			L10N.Add(STR_LVLED_CFG_WRAP_DONUT, "Donut", "Donut", "Donut", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorConfigPanel.cs:391
			L10N.Add(STR_LVLED_CFG_WRAP_REFLECT, "Reflect", "Reflektierend", "Reflect", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorModePanel.cs:149
			L10N.Add(STR_LVLED_PORTAL, "Portal", "Portal", "Portal", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       PortalStub.cs:145
			L10N.Add(STR_LVLED_BTN_CHANNEL, "Channel", "Kanal", "Channel", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       PortalStub.cs:154
			L10N.Add(STR_LVLED_BTN_DIR, "Direction", "Richtung", "Direction", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       PortalStub.cs:163
			L10N.Add(STR_LVLED_BTN_LEN, "Length", "Länge", "Length", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMLevelData.cs:243
			// Usage:       LevelEditorConfigPanel.cs:407
			L10N.Add(STR_LVLED_ERR_NONAME, "Level has no name set", "Level hat keinen Namen gesetzt", "Level has no name set", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMLevelData.cs:267
			L10N.Add(STR_LVLED_ERR_NOENEMY, "You need at least two player", "Ein Level braucht mindestens zwei Spieler", "You need at least two player", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMLevelData.cs:261
			L10N.Add(STR_LVLED_ERR_NOPLAYER, "At least one cannon must be owned by the player", "Mindestens eine Kanone muss dem Spieler gehören", "At least one cannon must be owned by the player", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMLevelData.cs:237
			L10N.Add(STR_LVLED_ERR_TOOMANYENTS, "Level has too many entities", "Zu viele Elemente im Level", "Level has too many entities", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMLevelData.cs:308
			L10N.Add(STR_LVLED_ERR_COMPILERERR, "Level compilation failed internally", "Level konnte nicht kompiliert werden", "Level compilation failed internally", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorDeleteConfirmPanel.cs:46
			L10N.Add(STR_LVLED_BTN_DELLEVEL, "Delete level", "Level löschen", "Delete level", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorSaveConfirmPanel.cs:46
			L10N.Add(STR_LVLED_BTN_SAVE, "Save changes", "Änderungen speichern", "Save changes", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorSaveConfirmPanel.cs:65
			L10N.Add(STR_LVLED_BTN_DISCARD, "Discard changes", "Änderungen verwerfen", "Discard changes", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorDeleteConfirmPanel.cs:64
			L10N.Add(STR_LVLED_TOAST_DELLEVEL, "Level deleted.", "Level gelöscht.", "Level deleted.", "?", "?"); //TODO translate me

			// Description: 
			L10N.Add(STR_MENU_CAP_SCCM, "User levels", "Benutzerlevel", "User levels", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMMainPanel.cs:59
			L10N.Add(STR_LVLED_TAB_MYLEVELS, "My levels", "Meine Level", "My levels", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMMainPanel.cs:75
			L10N.Add(STR_LVLED_TAB_HOT, "Hot", "Beliebt", "Hot", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMMainPanel.cs:91
			L10N.Add(STR_LVLED_TAB_TOP, "Top", "Top", "Top", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMMainPanel.cs:107
			L10N.Add(STR_LVLED_TAB_NEW, "New", "Neu", "New", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMMainPanel.cs:123
			L10N.Add(STR_LVLED_TAB_SEARCH, "Search", "Suche", "Search", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMListElementNewUserLevel.cs:39
			L10N.Add(STR_LVLED_BTN_NEWLVL, "Create new level", "Neues Level erstellen", "Create new level", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMListElementNewUserLevel.cs:88
			L10N.Add(STR_GENERIC_SERVER_QUERY, "Processing your request", "Anfrage wird bearbeitet", "Processing your request", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMListElementEditable.cs:43
			L10N.Add(STR_LVLED_BTN_EDIT, "Edit", "Edit", "Edit", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMScorePanel_Won.cs:422
			// Usage:       SCCMListElementLocalPlayable.cs:52
			// Usage:       SCCMListElementOnlinePlayable.cs:36
			L10N.Add(STR_LVLED_BTN_PLAY, "Play", "Play", "Play", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorScreen.cs:260
			// Usage:       LevelEditorScreen.cs:325
			L10N.Add(STR_LVLED_COMPILING, "Compiling", "Erzeugen", "Compiling", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorConfigPanel.cs:381
			L10N.Add(STR_MUSIC_NONE, "None", "Keine", "None", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorConfigPanel.cs:381
			L10N.Add(STR_MUSIC_INT, "Song {0}", "Lied {0}", "Song {0}", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       LevelEditorConfigPanel.cs:246
			L10N.Add(STR_LVLED_CFG_MUSIC, "Song:", "Lied:", "Song:", "Musica:", "Música:");

			// Description: 
			// Usage:       HUDSCCMUploadScorePanel.cs:192
			L10N.Add(STR_LVLED_BTN_UPLOAD, "Upload", "Veröffentlichen", "Upload", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMScorePanel_Transmit.cs:95
			// Usage:       HUDSCCMUploadScorePanel.cs:273
			L10N.Add(STR_LVLED_UPLOADING, "Uploading", "Wird hochgeladen", "Uploading", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMUploadScorePanel.cs:392
			L10N.Add(STR_LVLED_UPLOAD_FIN, "Level successfully published", "Level wurde veröffentlicht", "Level successfully published", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMUploadScorePanel.cs:312
			L10N.Add(STR_LVLUPLD_ERR_INTERNAL, "Server error while uploading level", "Im Server trat ein Fehler während des Uploads auf", "Server error while uploading level", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMUploadScorePanel.cs:334
			L10N.Add(STR_LVLUPLD_ERR_FILETOOBIG, "Levelfile too big", "Leveldatei zu groß", "Levelfile too big", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMUploadScorePanel.cs:345
			L10N.Add(STR_LVLUPLD_ERR_WRONGUSER, "Level was created by a different account", "Level wurde von einem anderen Benutzer erstellt", "Level was created by a different account", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMUploadScorePanel.cs:356
			L10N.Add(STR_LVLUPLD_ERR_LIDNOTFOUND, "Level was deleted on the server", "Level wurde auf dem Server gelöscht", "Level was deleted on the server", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMUploadScorePanel.cs:367
			L10N.Add(STR_LVLUPLD_ERR_INVALIDNAME, "Ungültiger Name", "Invalid name for level", "Ungültiger Name", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMLevelPreviewDialog.cs:166
			L10N.Add(STR_INF_CLEARS, "Clears", "Gesamt", "Clears", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMLevelPreviewDialog.cs:574
			// Usage:       SCCMLevelPreviewDialog.cs:597
			// Usage:       SCCMLevelPreviewDialog.cs:616
			// Usage:       SCCMLevelPreviewDialog.cs:629
			L10N.Add(STR_SCCM_DOWNLOADFAILED, "Download failed", "Dowload fehlgeschlagen", "Download failed", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMLevelPreviewDialog.cs:743
			L10N.Add(STR_SCCM_DOWNLOADINPROGRESS, "Downloading...", "Downloading...", "Downloading...", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMLevelPreviewDialog.cs:751
			L10N.Add(STR_SCCM_VERSIONTOOOLD, "This level needs a newer version of the game", "Level benötigt eine neue Version der App", "This level needs a newer version of the game", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMListElementNewUserLevel.cs:82
			L10N.Add(STR_SCCM_NEEDS_ACC, "You need a registered account to create levels", "Ein Account wird benötigt um Level zu erstellen", "You need a registered account to create levels", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       ScoreDisplay.cs:106
			L10N.Add(STR_SCOREMAN_INFO_SCORE, "You get these tokens by completing normal levels.\nHigher difficulties result in more points.", "Diese Punkte erlangt man durch das Spielen normaler Level.\nHöhere Schwierigkeitsstufen geben mehr Punkte.", "You get these tokens by completing normal levels.\nHigher difficulties result in more points.", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       MultiplayerScoreDisplay.cs:104
			L10N.Add(STR_SCOREMAN_INFO_MPSCORE, "You can get these tokens by winning multiplayer games against your friends.", "Diese Punkte erlangt man durch gewinnen von lokalen Mehrspieler-Partien", "You can get these tokens by winning multiplayer games against your friends.", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMScoreDisplay.cs:119
			L10N.Add(STR_SCOREMAN_INFO_SCCMSCORE, "You can get these tokens by winning user generated levels from other players", "Dise Punkte erlangt man durch das Spielen von Leveln die von anderen Spielern erstellt wurden", "You can get these tokens by winning user generated levels from other players", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       StarsScoreDisplay.cs:105
			L10N.Add(STR_SCOREMAN_INFO_STARS, "You get these stars when other players give a level you created a star", "Du bekommst Sterne wenn andere Spieler ein Level, das du erstellt hast, positiv bewerten", "You get these stars when other players give a level you created a star", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HighscorePanel.cs:71
			L10N.Add(STR_HSP_STARRANKING, "Stars", "Sterne", "Stars", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HighscorePanel.cs:74
			L10N.Add(STR_HSP_SCCMRANKING, "User level points", "Benutzerlevel Punkte", "User level points", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HighscorePanel.cs:175
			L10N.Add(STR_TAB_STARS, "Stars", "Sterne", "Stars", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       SCCMPreviewPanel.cs:125
			L10N.Add(STR_PREV_FINISHGAME, "Finish the game", "Gewinne das Spiel", "Finish the game", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       GDGameScreen_SP.cs:72
			L10N.Add(STR_ACH_UNLOCK_ONLINE, "User levels unlocked", "Benutzerlevel entsperrt", "User levels unlocked", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       GDGameScreen_SP.cs:73
			L10N.Add(STR_ACH_UNLOCK_MULTIPLAYER, "Local multiplayer unlocked", "Lokaler Mehrspieler entsperrt", "Local multiplayer unlocked", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       GDGameScreen_SP.cs:74
			// Usage:       GDGameScreen_SP.cs:75
			// Usage:       GDGameScreen_SP.cs:76
			// Usage:       GDGameScreen_SP.cs:77
			L10N.Add(STR_ACH_UNLOCK_WORLD, "Unlocked world {0}", "Welt {0} entsperrt", "Unlocked world {0}", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMUploadScorePanel.cs:378
			L10N.Add(STR_LVLUPLD_ERR_DUPLNAME, "Name is already used by another level", "Levelname wird schon verwendet", "Name is already used by another level", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMScorePanel_Transmit.cs:108
			L10N.Add(STR_LVLED_BTN_ABORT, "Abort", "Abbrechen", "Abort", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMScorePanel_Transmit.cs:128
			L10N.Add(STR_LVLED_BTN_RETRY, "Retry", "Wiederholen", "Retry", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMScorePanel_Transmit.cs:181
			L10N.Add(STR_ACH_FIRSTCLEAR, "First Clear", "Erster", "First Clear", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMScorePanel_Transmit.cs:185
			L10N.Add(STR_ACH_WORLDRECORD, "World record", "Weltrekord", "World record", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMScorePanel_Won.cs:285
			L10N.Add(STR_HSP_AUTHOR, "Author", "Ersteller", "Author", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       HUDSCCMScorePanel_Won.cs:435
			// Usage:       HUDSCCMTestScorePanel.cs:130
			// Usage:       HUDSCCMUploadScorePanel.cs:140
			L10N.Add(STR_HSP_TIME_YOU, "Time (you)", "Deine Bestzeit", "Time (you)", "?", "?"); //TODO translate me

			// Description: 
			// Usage:       GDGameScreen_SCCMUpload.cs:168
			L10N.Add(STR_SCCM_UPLOADINFO, "Beat your own level to upload it.", "Gewinne dein eigenes Level um es hochzuladen", "Beat your own level to upload it.", "?", "?"); //TODO translate me


			// [en-US] [de-DE] [fr-FR] [it-IT] [es-ES]

#if DEBUG
			L10N.Verify();
#endif
		}
	}
}
