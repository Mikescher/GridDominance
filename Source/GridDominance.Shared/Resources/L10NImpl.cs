using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;

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

		public const int STR_SSB_ABOUT               = 00;
		public const int STR_SSB_ACCOUNT             = 01;
		public const int STR_SSB_HIGHSCORE           = 02;
		public const int STR_SSB_MUTE                = 03;
		public const int STR_SSB_EFFECTS             = 04;
		public const int STR_SSB_LANGUAGE            = 69;
		public const int STR_SSB_MUSIC               = 111;
		public const int STR_SSB_COLOR               = 185;

		public const int STR_HSP_GLOBALRANKING       = 05;
		public const int STR_HSP_MULTIPLAYERRANKING  = 167;
		public const int STR_HSP_RANKINGFOR          = 06;

		public const int STR_TAB_NAME                = 07;
		public const int STR_TAB_POINTS              = 08;
		public const int STR_TAB_TIME                = 09;

		public const int STR_FAP_ACCOUNT             = 10;
		public const int STR_FAP_USERNAME            = 11;
		public const int STR_FAP_SCORE               = 78;
		public const int STR_FAP_CHANGEPW            = 12;
		public const int STR_FAP_LOGOUT              = 93;
		public const int STR_FAP_WARN1               = 94;
		public const int STR_FAP_WARN2               = 95;
		public const int STR_FAP_LOGOUT_SUCESS       = 96;

		public const int STR_CPP_CHANGEPW            = 13;
		public const int STR_CPP_USERNAME            = 14;
		public const int STR_CPP_NEWPW               = 15;
		public const int STR_CPP_CHANGE              = 16;
		public const int STR_CPP_CHANGING            = 17;
		public const int STR_CPP_CHANGED             = 18;
		public const int STR_CPP_COMERR              = 19;
		public const int STR_CPP_AUTHERR             = 20;
		public const int STR_CPP_CHANGEERR           = 21;

		public const int STR_ATTRIBUTIONS            = 22;
		public const int STR_UNLOCK                  = 85;
		public const int STR_ACKNOWLEDGEMENTS        = 188;

		public const int STR_AAP_HEADER              = 23;
		public const int STR_AAP_USERNAME            = 24;
		public const int STR_AAP_PASSWORD            = 25;
		public const int STR_AAP_CREATEACCOUNT       = 26;
		public const int STR_AAP_LOGIN               = 27;
		public const int STR_AAP_LOGGINGIN           = 28;
		public const int STR_AAP_WRONGPW             = 29;
		public const int STR_AAP_USERNOTFOUND        = 30;
		public const int STR_AAP_NOCOM               = 31;
		public const int STR_AAP_LOGINSUCCESS        = 32;
		public const int STR_AAP_NOLOGIN             = 33;
		public const int STR_AAP_ACCCREATING         = 34;
		public const int STR_AAP_ACCCREATED          = 35;
		public const int STR_AAP_USERTAKEN           = 36;
		public const int STR_AAP_ALREADYCREATED      = 37;
		public const int STR_AAP_AUTHERROR           = 38;
		public const int STR_AAP_COULDNOTCREATE      = 39;

		public const int STR_PAUS_RESUME             = 40;
		public const int STR_PAUS_RESTART            = 41;
		public const int STR_PAUS_EXIT               = 42;

		public const int STR_HSP_LEVEL               = 43;
		public const int STR_HSP_POINTS              = 44;
		public const int STR_HSP_MPPOINTS            = 160;
		public const int STR_HSP_PROGRESS            = 45;
		public const int STR_HSP_TIME_NOW            = 183;
		public const int STR_HSP_TIME_BEST           = 184;
		public const int STR_HSP_BACK                = 46;
		public const int STR_HSP_NEXT                = 47;
		public const int STR_HSP_AGAIN               = 48;
		public const int STR_HSP_TUTORIAL            = 49;
		public const int STR_HSP_GETSTARTED          = 50;
		public const int STR_HSP_CONERROR            = 65;
		public const int STR_HSP_NEWGAME             = 158;
		public const int STR_HSP_RANDOMGAME          = 159;

		public const int STR_DIFF_0                  = 51;
		public const int STR_DIFF_1                  = 52;
		public const int STR_DIFF_2                  = 53;
		public const int STR_DIFF_3                  = 54;

		public const int STR_TUT_INFO1               = 79;
		public const int STR_TUT_INFO2               = 55;
		public const int STR_TUT_INFO3               = 56;
		public const int STR_TUT_INFO4               = 57;
		public const int STR_TUT_INFO5               = 58;
		public const int STR_TUT_INFO6               = 59;
		public const int STR_TUT_INFO7               = 60;
		public const int STR_TUT_INFO8               = 61;

		public const int STR_API_CONERR              = 62;
		public const int STR_API_COMERR              = 63;

		public const int STR_GLOB_EXITTOAST          = 64;
		public const int STR_GLOB_UNLOCKTOAST1       = 66;
		public const int STR_GLOB_UNLOCKTOAST2       = 67;
		public const int STR_GLOB_UNLOCKTOAST3       = 68;
		public const int STR_GLOB_LEVELLOCK          = 70;
		public const int STR_GLOB_WORLDLOCK          = 71;
		public const int STR_GLOB_OVERWORLD          = 75;
		public const int STR_GLOB_WAITFORSERVER      = 76;
		public const int STR_GLOB_UNLOCKSUCCESS      = 86;

		public const int STR_WORLD_TUTORIAL          = 77;
		public const int STR_WORLD_W1                = 80;
		public const int STR_WORLD_W2                = 81;
		public const int STR_WORLD_W3                = 92;
		public const int STR_WORLD_W4                = 103;
		public const int STR_WORLD_MULTIPLAYER       = 114;
		public const int STR_WORLD_SINGLEPLAYER      = 178;
		public const int STR_WORLD_ONLINE            = 194;

		public const int STR_INF_YOU                 = 72;
		public const int STR_INF_GLOBAL              = 73;
		public const int STR_INF_HIGHSCORE           = 74;

		public const int STR_IAB_TESTERR             = 82;
		public const int STR_IAB_TESTNOCONN          = 83;
		public const int STR_IAB_TESTINPROGRESS      = 84;

		public const int STR_IAB_BUYERR              = 88;
		public const int STR_IAB_BUYNOCONN           = 89;
		public const int STR_IAB_BUYNOTREADY         = 90;
		public const int STR_IAB_BUYSUCESS           = 91;

		public const int STR_PREV_BUYNOW             = 87;
		public const int STR_PREV_FINISHWORLD        = 108;
		public const int STR_PREV_OR                 = 109;
		public const int STR_PREV_MISS_TOAST         = 110;

		public const int STR_HINT_001                = 97;
		public const int STR_HINT_002                = 98;
		public const int STR_HINT_003                = 99;
		public const int STR_HINT_004                = 100;
		public const int STR_HINT_005                = 101;
		public const int STR_HINT_006                = 102;
		public const int STR_HINT_007                = 112;
		public const int STR_HINT_008                = 113;
		public const int STR_HINT_009                = 173;
		public const int STR_HINT_010                = 176;
		public const int STR_HINT_011                = 177;

		public const int STR_INFOTOAST_1             = 104;
		public const int STR_INFOTOAST_2             = 105;
		public const int STR_INFOTOAST_3             = 106;
		public const int STR_INFOTOAST_4             = 107;

		public const int STR_MP_TIMEOUT              = 115;
		public const int STR_MP_TIMEOUT_USER         = 116;
		public const int STR_MP_NOTINLOBBY           = 117;
		public const int STR_MP_SESSIONNOTFOUND      = 118;
		public const int STR_MP_AUTHFAILED           = 119;
		public const int STR_MP_LOBBYFULL            = 120;
		public const int STR_MP_VERSIONMISMATCH      = 121;
		public const int STR_MP_LEVELNOTFOUND        = 122;
		public const int STR_MP_LEVELMISMATCH        = 123;
		public const int STR_MP_USERDISCONNECT       = 124;
		public const int STR_MP_SERVERDISCONNECT     = 125;
		public const int STR_MP_INTERNAL             = 161;
		public const int STR_MP_BTADAPTERNULL        = 162;
		public const int STR_MP_BTADAPTERPERMDENIED  = 168;
		public const int STR_MP_BTDISABLED           = 163;
		public const int STR_MP_DIRECTCONNLOST       = 164;
		public const int STR_MP_DIRECTCONNFAIL       = 165;
		public const int STR_MP_TOAST_CONN_TRY       = 170;
		public const int STR_MP_TOAST_CONN_FAIL      = 171;
		public const int STR_MP_TOAST_CONN_SUCC      = 172;
		public const int STR_MP_NOSERVERCONN         = 182;

		public const int STR_MENU_CANCEL             = 138;
		public const int STR_MENU_DISCONNECT         = 147;

		public const int STR_MENU_CAP_MULTIPLAYER    = 126;
		public const int STR_MENU_CAP_LOBBY          = 127;
		public const int STR_MENU_CAP_CGAME_PROX     = 128;
		public const int STR_MENU_CAP_CGAME_BT       = 169;
		public const int STR_MENU_CAP_AUTH           = 136;
		public const int STR_MENU_CAP_SEARCH         = 166;
		public const int STR_MENU_MP_JOIN            = 132;
		public const int STR_MENU_MP_HOST            = 133;
		public const int STR_MENU_MP_START           = 149;
		public const int STR_MENU_MP_ONLINE          = 135;
		public const int STR_MENU_MP_LOCAL_CLASSIC   = 134;
		public const int STR_MENU_MP_CREATE          = 137;
		public const int STR_MENU_MP_GAMESPEED       = 139;
		public const int STR_MENU_MP_MUSIC           = 140;
		public const int STR_MENU_MP_LOBBYINFO       = 141;
		public const int STR_MENU_MP_LOBBY_USER      = 142;
		public const int STR_MENU_MP_LOBBY_USER_FMT  = 148;
		public const int STR_MENU_MP_LOBBY_LEVEL     = 143;
		public const int STR_MENU_MP_LOBBY_MUSIC     = 144;
		public const int STR_MENU_MP_LOBBY_SPEED     = 145;
		public const int STR_MENU_MP_LOBBY_PING      = 146;
		public const int STR_MENU_MP_LOBBY_COLOR     = 157;

		public const int STR_MP_ONLINE               = 129;
		public const int STR_MP_OFFLINE              = 130;
		public const int STR_MP_CONNECTING           = 131;

		public const int STR_FRAC_N0                 = 150;
		public const int STR_FRAC_P1                 = 151;
		public const int STR_FRAC_A2                 = 152;
		public const int STR_FRAC_A3                 = 153;
		public const int STR_FRAC_A4                 = 154;
		public const int STR_FRAC_A5                 = 155;
		public const int STR_FRAC_A6                 = 156;

		public const int STR_ENDGAME_1               = 174;
		public const int STR_ENDGAME_2               = 175;

		public const int STR_ACCOUNT_REMINDER        = 179;
		public const int STR_BTN_YES                 = 180;
		public const int STR_BTN_NO                  = 181;

		public const int STR_ERR_SOUNDPLAYBACK       = 186;
		public const int STR_ERR_MUSICPLAYBACK       = 187;
		public const int STR_ERR_OUTOFMEMORY         = 189;

		public const int STR_PROFILESYNC_START       = 190;
		public const int STR_PROFILESYNC_ERROR       = 191;
		public const int STR_PROFILESYNC_SUCCESS     = 192;

		public const int STR_AUTHERR_HEADER          = 193;

		public const int STR_LVLED_MOUSE             = 195;
		public const int STR_LVLED_CANNON            = 196;
		public const int STR_LVLED_WALL              = 197;
		public const int STR_LVLED_OBSTACLE          = 198;
		public const int STR_LVLED_SETTINGS          = 199;
		public const int STR_LVLED_PLAY              = 200;
		public const int STR_LVLED_UPLOAD            = 201;
		public const int STR_LVLED_EXIT              = 202;
		public const int STR_LVLED_BTN_FRAC          = 203;
		public const int STR_LVLED_BTN_SCALE         = 204;
		public const int STR_LVLED_BTN_ROT           = 205;
		public const int STR_LVLED_BTN_TYPE          = 206;
		public const int STR_LVLED_BTN_DIAMETER      = 207;
		public const int STR_LVLED_BTN_WIDTH         = 208;
		public const int STR_LVLED_BTN_HEIGHT        = 209;
		public const int STR_LVLED_BTN_POWER         = 210;
		public const int STR_LVLED_BTN_DEL           = 211;
		public const int STR_LVLED_CFG_ID            = 212;
		public const int STR_LVLED_CFG_NAME          = 213;
		public const int STR_LVLED_CFG_SIZE          = 214;
		public const int STR_LVLED_CFG_VIEW          = 215;
		public const int STR_LVLED_CFG_GEOMETRY      = 216;
		public const int STR_LVLED_CFG_WRAP_INFINITY = 217;
		public const int STR_LVLED_CFG_WRAP_DONUT    = 218;
		public const int STR_LVLED_CFG_WRAP_REFLECT  = 219;

		private const int TEXT_COUNT = 220; // = next idx

		public static void Init(int lang)
		{
			L10N.Init(lang, TEXT_COUNT, LANG_COUNT);

			// [en_US] [de-DE] [fr-FR] [it-IT] [es-ES]

			L10N.Add(STR_SSB_ABOUT,
			         "About",
			         "Info",
			         "Info",
			         "Informazioni",
			         "Información");

			L10N.Add(STR_SSB_ACCOUNT,
			         "Account",
			         "Benutzerkonto",
			         "Compte",
			         "Account",
			         "Cuenta");

			L10N.Add(STR_SSB_HIGHSCORE,
			         "Highscore",
			         "Bestenliste",
			         "Tableau d'honneur",
			         "Classifica",
			         "Puntaje alto");

			L10N.Add(STR_SSB_MUTE,
			         "Mute",
			         "Stumm",
			         "Muet",
			         "Muto",
			         "Mute");

			L10N.Add(STR_SSB_EFFECTS,
			         "Effects",
			         "Effekte",
			         "Effet",
			         "Effetti",
			         "Efectos");

			L10N.Add(STR_SSB_MUSIC,
			         "Music",
			         "Musik",
			         "Musique",
			         "Musica",
			         "Música");

			L10N.Add(STR_SSB_COLOR,
			         "Color scheme",
			         "Farbschema",
			         "Schéma de couleurs",
			         "Schema a colori",
			         "Esquema de colores");

			L10N.Add(STR_HSP_GLOBALRANKING,
			         "Global Ranking",
			         "Bestenliste",
			         "Classement globale",
			         "Classifica Globale",
			         "Ranking global");

			L10N.Add(STR_HSP_MULTIPLAYERRANKING,
			         "Multiplayer",
			         "Mehrspieler",
			         "Multijoueur",
			         "Multiplayer",
			         "Multijugador");

			L10N.Add(STR_HSP_RANKINGFOR,
			         "Ranking for \"{0}\"",
			         "Bestenliste für \"{0}\"",
			         "Classement pour \"{0}\"",
			         "Classifica per \"{0}\"",
			         "Ranking");

			L10N.Add(STR_TAB_NAME,
			         "Name",
			         "Name",
			         "Nom",
			         "Nome",
			         "Nombre");

			L10N.Add(STR_TAB_POINTS,
			         "Points",
			         "Punkte",
			         "Points",
			         "Punti",
			         "Puntos");

			L10N.Add(STR_TAB_TIME,
			         "Total Time",
			         "Gesamtzeit",
			         "Temps total",
			         "Tempo Totale",
			         "Tiempo total");

			L10N.Add(STR_FAP_ACCOUNT,
			         "Account",
			         "Benutzerkonto",
			         "Compte d'utilisateur",
			         "Account",
			         "Cuenta");

			L10N.Add(STR_FAP_USERNAME,
			         "Username:",
			         "Benutzername:",
			         "Nom d'utilisateur",
			         "Username:",
			         "Usuario");

			L10N.Add(STR_FAP_SCORE,
			         "Points:",
			         "Punkte:",
			         "Points",
			         "Points:",
			         "Puntos");

			L10N.Add(STR_FAP_CHANGEPW,
			         "Change Password",
			         "Passwort ändern",
			         "Mot de passe",
			         "Cambia Password",
			         "Cambiar clave");

			L10N.Add(STR_FAP_LOGOUT,
			         "Logout",
			         "Ausloggen",
			         "Déconnecter",
			         "Esci",
			         "Cerrar sesión");

			L10N.Add(STR_FAP_WARN1,
			         "This will clear all local data. Press again to log out.",
			         "Dies löscht alle lokalen Daten. Nochmal drücken zum ausloggen.",
			         "Cette opération efface toutes les données locales. Appuyez à nouveau pour vous déconnecter.",
			         "Questo cancellerà tutti i dati locali. Premi di nuovo per uscire.",
			         "Esto borra todos los datos locales. Presiona otra vey para cerrar sesión");

			L10N.Add(STR_FAP_WARN2,
			         "Are you really sure you want to log out?",
			         "Wirklich vom Serverkonto abmelden?",
			         "Êtes-vous vraiment sûr de vouloir vous déconnecter?",
			         "Sei sicuro di voler uscire?",
			         "Estas seguro que quieres cerrar sesión");

			L10N.Add(STR_FAP_LOGOUT_SUCESS,
			         "Logged out from account",
			         "Lokaler Benutzer wurde abgemeldet.",
			         "Déconnecté du compte",
			         "Uscito dall'account",
			         "Usuario desconectado");

			L10N.Add(STR_CPP_CHANGEPW,
			         "Change Password",
			         "Passwort ändern",
			         "Changer mot de passe",
			         "Cambia Password",
			         "Cambiar clave");

			L10N.Add(STR_CPP_USERNAME,
			         "Username:",
			         "Benutzername:",
			         "Nom d'utilisateur",
			         "Username:",
			         "Usuario");

			L10N.Add(STR_CPP_NEWPW,
			         "New Password",
			         "Neues Passwort",
			         "Noveau mot de passe",
			         "Nuova Password",
			         "Nueva clave");

			L10N.Add(STR_CPP_CHANGE,
			         "Change",
			         "Ändern",
			         "Changer mot de passe",
			         "Cambia",
			         "Cambiar");

			L10N.Add(STR_CPP_CHANGING,
			         "Changing password",
			         "Passwort wird geändert",
			         "Changement du mot de passe ",
			         "Cambiando la password",
			         "Coambiando clave");

			L10N.Add(STR_CPP_CHANGED,
			         "Password changed",
			         "Passwort geändert",
			         "Mot de passe est changé",
			         "Password cambiata",
			         "Clave modificada");

			L10N.Add(STR_CPP_COMERR,
			         "Could not communicate with server",
			         "Kommunikation mit Server ist gestört",
			         "La communication avec le serveur est perturbé",
			         "Impossibile comunicare con il server",
			         "No se puede comunicar con Server");

			L10N.Add(STR_CPP_AUTHERR,
			         "Authentication error",
			         "Authentifizierung fehlgeschlagen",
			         "Erreur d'authentification",
			         "Errore di autenticazione",
			         "Error de auntentificación");

			L10N.Add(STR_CPP_CHANGEERR,
			         "Could not change password",
			         "Passwort konnte nicht geändert werden",
			         "Mot de passe ne peut pas être modifié",
			         "Impossibile cambiare password",
			         "No se pudo cambiar la clave");

			L10N.Add(STR_ATTRIBUTIONS,
			         "Attributions",
			         "Lizenzen",
			         "Licences",
			         "Attribuzioni",
			         "Atribuciones");

			L10N.Add(STR_AAP_HEADER,
			         "Sign up / Log in",
			         "Anmelden / Registrieren",
			         "Se connecter",
			         "Iscriviti / Accedi",
			         "Registrarse / Iniciar sesión");

			L10N.Add(STR_AAP_USERNAME,
			         "Username",
			         "Benutzername",
			         "Nom d'utilisateur",
			         "Username",
			         "Usuario");

			L10N.Add(STR_AAP_PASSWORD,
			         "Password",
			         "Passwort",
			         "Mot de passe",
			         "Password",
			         "Clave");

			L10N.Add(STR_AAP_CREATEACCOUNT,
			         "Create Account",
			         "Registrieren",
			         "Registrer",
			         "Crea Account",
			         "Registrarse");

			L10N.Add(STR_AAP_LOGIN,
			         "Login",
			         "Anmelden",
			         "S'inscrire",
			         "Accedi",
			         "Iniciar sesión");

			L10N.Add(STR_AAP_LOGGINGIN,
			         "Logging in",
			         "Wird angemeldet",
			         "Est enregistré",
			         "Accedendo",
			         "Iniciando sesión");

			L10N.Add(STR_AAP_WRONGPW,
			         "Wrong password",
			         "Falsches Passwort",
			         "Mot de passe incorrect",
			         "Password sbagliata",
			         "Clave incorrecta");

			L10N.Add(STR_AAP_USERNOTFOUND,
			         "User not found",
			         "Benutzer nicht gefunden",
			         "Utilisateur introuvable",
			         "Utente non trovato",
			         "Usuario no encontrado");

			L10N.Add(STR_AAP_NOCOM,
			         "Could not communicate with server",
			         "Konnte nicht mit Server kommunizieren",
			         "La communication avec le serveur est perturbé",
			         "Impossibile comunicare con il server",
			         "No se puede comunicar con Server");

			L10N.Add(STR_AAP_LOGINSUCCESS,
			         "Successfully logged in",
			         "Benutzer erfolgreich angemeldet",
			         "Connecté avec succès",
			         "Accesso completo",
			         "Sesión iniciada correctamente");

			L10N.Add(STR_AAP_NOLOGIN,
			         "Could not login",
			         "Anmeldung fehlgeschlagen",
			         "Échec de la connexion",
			         "Impossibile accedere",
			         "No se pudo iniciar sesión");

			L10N.Add(STR_AAP_ACCCREATING,
			         "Creating account",
			         "Konto wird erstellt",
			         "Création du compte",
			         "Creando l'account",
			         "Registrando cuenta de usuario");

			L10N.Add(STR_AAP_ACCCREATED,
			         "Account created",
			         "Konto erfolgreich erstellt",
			         "Compte créé",
			         "Account creato",
			         "Usuario creado");

			L10N.Add(STR_AAP_USERTAKEN,
			         "Username already taken",
			         "Benutzername bereits vergeben",
			         "Nom d'utilisateur déjà pris",
			         "Username già in uso",
			         "Nombre de usuario no disponible");

			L10N.Add(STR_AAP_ALREADYCREATED,
			         "Account already created",
			         "Konto bereits erstellt",
			         "Compte déjà créé",
			         "Account già creato",
			         "Esta cuenta ya está registrada");

			L10N.Add(STR_AAP_AUTHERROR,
			         "Authentication error",
			         "Authentifizierungsfehler",
			         "Erreur d'authentification",
			         "Errore di autenticazione",
			         "Error de auntentificación");

			L10N.Add(STR_AAP_COULDNOTCREATE,
			         "Could not create account",
			         "Konto konnte nicht erstellt werden",
			         "Impossible de créer compte",
			         "Impossibile creare account",
			         "No se pudo crear cuenta");

			L10N.Add(STR_PAUS_RESUME,
			         "RESUME",
			         "WEITER",
			         "REPRENDRE",
			         "CONTINUA",
			         "Continuar");

			L10N.Add(STR_PAUS_RESTART,
			         "RESTART",
			         "NEU STARTEN",
			         "REDÉMARRER",
			         "RICOMINCIA",
			         "Reiniciar");

			L10N.Add(STR_PAUS_EXIT,
			         "EXIT",
			         "BEENDEN",
			         "TERMINER",
			         "ESCI",
			         "Salir");

			L10N.Add(STR_HSP_LEVEL,
			         "Level",
			         "Level",
			         "Niveau",
			         "Livello",
			         "Nivel");

			L10N.Add(STR_HSP_POINTS,
			         "Points",
			         "Punkte",
			         "Ponts",
			         "Punti",
			         "Puntos");

			L10N.Add(STR_HSP_PROGRESS,
			         "Progress",
			         "Fortschritt",
			         "Progrès",
			         "Progresso",
			         "Progreso");

			L10N.Add(STR_HSP_TIME_NOW,
			         "Time (now)",
			         "Levelzeit",
			         "Temps",
			         "Tempo",
			         "Tiempo actual");

			L10N.Add(STR_HSP_TIME_BEST,
			         "Time (best)",
			         "Bestzeit",
			         "Meilleur temps",
			         "Miglior tempo",
			         "Mejor tiempo");

			L10N.Add(STR_HSP_BACK,
			         "Back",
			         "Zurück",
			         "De retour",
			         "Indietro",
			         "Atrás");

			L10N.Add(STR_HSP_NEXT,
			         "Next",
			         "Weiter",
			         "Prochain",
			         "Avanti",
			         "Siguiente");

			L10N.Add(STR_HSP_AGAIN,
			         "Again",
			         "Wiederholen",
			         "Répéter",
			         "Ripeti",
			         "Repetir");

			L10N.Add(STR_HSP_TUTORIAL,
			         "Tutorial",
			         "Tutorial",
			         "Tutorial",
			         "Tutorial",
			         "Tutorial");

			L10N.Add(STR_HSP_GETSTARTED,
			         "Let's get started",
			         "Los gehts",
			         "C'est parti",
			         "Iniziamo",
			         "A comenzar");

			L10N.Add(STR_HSP_CONERROR,
			         "Could not connect to highscore server",
			         "Kommunikation mit Server fehlgeschlagen",
			         "Impossible de se connecter au serveur highscore",
			         "Impossibile connettersi al server della classifica",
			         "No se puede comunicar con Server");

			L10N.Add(STR_DIFF_0,
			         "Easy",
			         "Leicht",
			         "Facile",
			         "Facile",
			         "Fácil");

			L10N.Add(STR_DIFF_1,
			         "Normal",
			         "Normal",
			         "Normal",
			         "Normale",
			         "Normal");

			L10N.Add(STR_DIFF_2,
			         "Hard",
			         "Schwer",
			         "Difficile",
			         "Difficile",
			         "Difícil");

			L10N.Add(STR_DIFF_3,
			         "Extreme",
			         "Extrem",
			         "Extrême",
			         "Estremo",
			         "Extremo");

			L10N.Add(STR_TUT_INFO1,
			         "Drag to rotate your own cannons",
			         "Drücke und Ziehe um deine Kanonen zu drehen",
			         "Tirer pour tourner tes canons",
			         "Trascina per ruotare i tuoi cannoni",
			         "Para rotar tus cañones presiona y tira");

			L10N.Add(STR_TUT_INFO2,
			         "Shoot it until it becomes your cannon",
			         "Schieße bis die feindliche Kanone dir gehört",
			         "Abattre jusque le canon ennemi est à toi",
			         "Spara fino a che non diventa il tuo cannone",
			         "Dispara hasta que se convierta en tu cañón");

			L10N.Add(STR_TUT_INFO3,
			         "Now capture the next cannon",
			         "Erobere nun die nächste Einheit",
			         "Captiver le prochain canon",
			         "Ora cattura il prossimo cannone",
			         "Ahora captura tu próximo cañón");

			L10N.Add(STR_TUT_INFO4,
			         "Keep shooting at the first cannon to increase its fire rate",
			         "Schieß auf deine eigene Kanone um ihre Feuerrate zu erhöhen",
			         "Gardez le tir au premier canon pour augmenter sa cadence de tir",
			         "Continua a sparare al primo cannone per aumentare la cadenza di fuoco",
			         "Continúa disparando con tu primer cañón para aumentar su nivel de tiro");

			L10N.Add(STR_TUT_INFO5,
			         "The enemy has captured a cannon. Attack him!",
			         "Der Gegner hat eine Einheit erobert, greif ihn an!",
			         "L'enemi a conquis  un canon. Attaque.",
			         "Il nemico ha conquistato un cannone. Attaccalo!",
			         "El enemigo ha conquistado un cañón, atácalo");

			L10N.Add(STR_TUT_INFO6,
			         "Speed up the Game with the bottom left button.",
			         "Mit dem Knopf unten links kannst du die Spielgeschwindigkeit erhöhen",
			         "Accélérez le jeu avec le bouton en bas à gauche.",
			         "Accellera il gioco con i pulsanti in basso a sinistra.",
			         "Aumenta la velocidad de juego con el botón de abajo a la izquierda");

			L10N.Add(STR_TUT_INFO7,
			         "Now capture the next cannon",
			         "Erobere jetzt die nächste Einheit",
			         "Maintenant capturez le prochain canon",
			         "Ora cattura il prossimo cannone",
			         "Ahora captura tu próximo cañón");

			L10N.Add(STR_TUT_INFO8,
			         "Win the game by capturing all enemy cannons",
			         "Gewinne die Schlacht indem du alle Einheiten eroberst",
			         "Gagnez le jeu en capturant tous les canons ennemis",
			         "Vinci il gioco catturando tutti i cannoni nemici",
			         "Ganarás cuando hayas capturado todos los cañones");

			L10N.Add(STR_API_CONERR,
			         "Could not connect to highscore server",
			         "Verbindung mit Highscore Server fehlgeschlagen",
			         "Impossible de se connecter au serveur Highscore",
			         "Impossibile connettersi al server della classifica",
			         "No se puede comunicar con Server");

			L10N.Add(STR_API_COMERR,
			         "Could not communicate with highscore server",
			         "Kommunikation mit Highscore Server fehlgeschlagen",
			         "Impossible de communiquer avec le serveur highscore",
			         "Impossibile comunicare al server della classifica",
			         "No se puede comunicar con Server");

			L10N.Add(STR_GLOB_EXITTOAST,
			         "Click again to exit game",
			         "Drücke nochmal \"Zurück\" um das Spiel zu beenden",
			         "Cliquez de noouveau pour quitter le jeu",
			         "Clicca di nuovo per uscire dal gioco",
			         "Presiona otra vez para finalizar el juego");

			L10N.Add(STR_GLOB_UNLOCKTOAST1,
			         "Click two times more to unlock",
			         "Noch zweimal drücken um die Welt freizuschalten",
			         "Cliquez deux fois plus pour débloquer",
			         "Clicca due volte per sbloccare",
			         "Presiona dos veces para desbloquear");

			L10N.Add(STR_GLOB_UNLOCKTOAST2,
			         "Click again to unlock",
			         "Nochmal drücken um die Welt freizuschalten",
			         "Cliquez de nouvea pour quitter le jeu",
			         "Clicca di nuovo per sbloccare",
			         "Presiona otra vez para desbloquear");

			L10N.Add(STR_GLOB_UNLOCKTOAST3,
			         "World unlocked",
			         "Welt freigeschaltet",
			         "Monde débloqué",
			         "Mondo sbloccato",
			         "Mundo desbloqueado");

			L10N.Add(STR_GLOB_WORLDLOCK,
			         "World locked",
			         "Welt noch nicht freigespielt",
			         "Monde bloqué",
			         "Mondo bloccato",
			         "Mundo bloqueado");

			L10N.Add(STR_GLOB_LEVELLOCK,
			         "Level locked",
			         "Level noch nicht freigespielt",
			         "Niveau bloqué",
			         "Livello bloccato",
			         "Nivel bloqueado");

			L10N.Add(STR_INF_YOU,
			         "You",
			         "Du",
			         "Toi",
			         "Tu",
			         "Tú");

			L10N.Add(STR_INF_GLOBAL,
			         "Stats",
			         "Total",
			         "Global",
			         "Statistiche",
			         "Estadística");

			L10N.Add(STR_INF_HIGHSCORE,
			         "Highscore",
			         "Bestzeit",
			         "Highscore",
			         "Highscore",
			         "Puntaje alto");

			L10N.Add(STR_GLOB_OVERWORLD,
			         "Overworld",
			         "Übersicht",
			         "L'aperçu",
			         "Overworld",
			         "Síntesis");

			L10N.Add(STR_GLOB_WAITFORSERVER,
			         "Contacting server",
			         "Server wird kontaktiert",
			         "Contacter le serveur",
			         "Contattando il server",
			         "Conectando con Server");

			L10N.Add(STR_WORLD_TUTORIAL,
			         "Tutorial",
			         "Tutorial",
			         "Didacticiel",
			         "Tutorial",
			         "Tutorial");

			L10N.Add(STR_SSB_LANGUAGE,
			         "Language",
			         "Sprache",
			         "Longue",
			         "Linguaggio",
			         "Idioma");

			L10N.Add(STR_WORLD_W1,
			         "Basic",
			         "Grundlagen",
			         "Niveau base",
			         "Base",
			         "Principios básicos");

			L10N.Add(STR_WORLD_W2,
			         "Professional",
			         "Fortgeschritten",
			         "Niveau avancé",
			         "Professionale",
			         "Profesional");

			L10N.Add(STR_WORLD_W3,
			         "Futuristic",
			         "Futuristisch",
			         "Futuriste",
			         "Futuristico",
			         "Futurístico");

			L10N.Add(STR_WORLD_W4,
			         "Toy Box",
			         "Spielzeugkiste",
			         "Coffre à jouets",
			         "Svago",
			         "Caja de juegos");

			L10N.Add(STR_WORLD_MULTIPLAYER,
			         "Multiplayer",
			         "Mehrspieler",
			         "Multijoueur",
			         "Multiplayer",
			         "Multijugador");

			L10N.Add(STR_WORLD_SINGLEPLAYER,
			         "Singleplayer",
			         "Einzelspieler",
			         "Seul joueur",
			         "Singleplayer",
			         "Juego individual");

			L10N.Add(STR_IAB_TESTERR,
			         "Error connecting to Google Play services",
			         "Fehler beim Versuch mit Google Play zu verbinden",
			         "Erreurde connexion avec Google Play services",
			         "Impossibile connettersi ai servizi Google Play",
			         "Error en conexión con Google Play");

			L10N.Add(STR_IAB_TESTNOCONN,
			         "No connection to Google Play services",
			         "Keine Verbindung zu Google Play services",
			         "Pas de connexion avec Google Play services",
			         "Nessuna connessione ai servizi Google Play",
			         "Sin conexión a Google Play");

			L10N.Add(STR_IAB_TESTINPROGRESS,
			         "Payment in progress",
			         "Zahlung wird verarbeitet",
			         "Paiement en cours",
			         "Pagamento in lavorazione",
			         "Pago en progreso");

			L10N.Add(STR_UNLOCK,
			         "Promotion Code",
			         "Promo Code",
			         "Code promotionnel",
			         "Codice Promozionale",
			         "Código de promoción");

			L10N.Add(STR_ACKNOWLEDGEMENTS,
			         "Acknowledgements",
			         "Danksagungen",
			         "Remerciements",
			         "Ringraziamenti",
			         "Agradecimientos");

			L10N.Add(STR_GLOB_UNLOCKSUCCESS,
			         "Upgraded game to full version!",
			         "Spiel wurde zur Vollversion aufgewertet",
			         "Mise à niveau du jeu en version complète!",
			         "Congratulazioni, hai acquistato la versione completa!",
			         "Has obtenido la versión completa del juego");

			L10N.Add(STR_PREV_BUYNOW,
			         "Unlock now",
			         "Jetzt freischalten",
			         "Débloquer maintenant",
			         "Sblocca ora",
			         "Desbloquear ahora");

			L10N.Add(STR_IAB_BUYERR,
			         "Error connecting to Google Play services",
			         "Fehler beim Versuch mit Google Play zu verbinden",
			         "Erreurde connexion avec Google Play services",
			         "Impossibile connettersi ai servizi Google Play",
			         "Error en conexión con Google Play");

			L10N.Add(STR_IAB_BUYNOCONN,
			         "No connection to Google Play services",
			         "Keine Verbindung zu Google Play services",
			         "Pas de connexion avec Google Play services",
			         "Nessuna connessione ai servizi Google Play",
			         "Sin conexión a Google Play");

			L10N.Add(STR_IAB_BUYNOTREADY,
			         "Connection to Google Play services not ready",
			         "Verbindung zu Google Play services nicht bereit",
			         "La connexion aux services Google Play n'est pas prête",
			         "Connessione ai servizi Google Play non pronta",
			         "Conexión a Google Play no disponible");

			L10N.Add(STR_IAB_BUYSUCESS,
			         "World successfully purchased",
			         "Levelpack wurde erfolgreich erworben",
			         "Le monde a acheté avec succès",
			         "Mondo acquistato!",
			         "Has comprado el mundo exitosamente");

			L10N.Add(STR_HINT_001,
			         "Tip: Shoot stuff to win!",
			         "Tipp: Versuche auf die andere Kanone zu schiessen",
			         "Allusion: tirez des trucs pour gagner!",
			         "Consiglio: Spara per vincere!",
			         "Consejo: dispara al otro cañón");

			L10N.Add(STR_HINT_002,
			         "Bigger Cannon",
			         "Größere Kanone",
			         "Plus grands canons",
			         "Cannone più grande",
			         "Cañón más grande");

			L10N.Add(STR_HINT_003,
			         "More Power",
			         "Mehr Schaden",
			         "Plus d'énergie",
			         "Più potenza",
			         "Más potencia");

			L10N.Add(STR_HINT_004,
			         "Black holes attract your bullets",
			         "Schwarze Löcher saugen deine Kugeln ein",
			         "Les trous noirs attirent vos balles",
			         "I buchi neri attraggono i tuoi proiettili",
			         "Hoyos negros succionan tus balas ");

			L10N.Add(STR_HINT_005,
			         "Lasers!",
			         "Laser!",
			         "Lasers!",
			         "Laser!",
			         "Lásers!");

			L10N.Add(STR_HINT_006,
			         "Try dragging the map around",
			         "Versuche mal die Karte zu verschieben",
			         "Essayez de faire glisser la carte autour",
			         "Prova a trascinare la mappa",
			         "Intenta desplazar el mapa");

			L10N.Add(STR_HINT_007,
			         "Speedy thing goes in,",
			         "Speedy thing goes in,",
			         "Speedy thing goes in,",
			         "Cosina veloce entra,",
			         "Speedy thing entra");

			L10N.Add(STR_HINT_008,
			         "speedy thing comes out.",
			         "speedy thing comes out.",
			         "speedy thing comes out.",
			         "cosina veloce esce.",
			         "Speedy thing sale");

			L10N.Add(STR_HINT_009,
			         "Some cannons only relay",
			         "Manche Kanonen leiten nur weiter",
			         "Certains canons relèvent",
			         "Alcuni cannoni collegano e basta",
			         "Algunos cañones solo redireccionan");

			L10N.Add(STR_HINT_010,
			         "Shields can",
			         "Schilde können",
			         "Les écus peuvent",
			         "Gli scudi",
			         "Los escudos");

			L10N.Add(STR_HINT_011,
			         "protect you",
			         "dich beschützen",
			         "protégez-vous",
			         "ti proteggono",
			         "pueden protegerte");

			L10N.Add(STR_INFOTOAST_1,
			         "Your best time is {0}",
			         "Deine Bestzeit ist {0}",
			         "Votre meilleur temps est {0}",
			         "Il tuo tempo migliore è {0}",
			         "Tu mejor tiempo");

			L10N.Add(STR_INFOTOAST_2,
			         "The global best time is {0}",
			         "Die globale Bestzeit ist {0}",
			         "Le meilleur temps global est {0}",
			         "Il tempo assoluto migliore è {0}",
			         "El récord de tiempo");

			L10N.Add(STR_INFOTOAST_3,
			         "{0} users have completed this level on {1}",
			         "{0} Spieler haben dieses Level auf {1} geschafft",
			         "{0} utilisateurs ont complété ce niveau sur {1}",
			         "{0} utenti hanno completato questo livello in {1}",
			         "{0} Jugadores han completado este nivel en {1}");

			L10N.Add(STR_INFOTOAST_4,
			         "You have not completed this level on {0}",
			         "Du hast dieses Level auf {0} noch nicht geschafft",
			         "Vous n'avez pas terminé ce niveau sur {0}",
			         "Non hai completato questo livello in {0}",
			         "No has completado este nivel en {0}");

			L10N.Add(STR_PREV_FINISHWORLD,
			         "Finish World {0}",
			         "Welt {0}",
			         "Terminer Monde {0}",
			         "Finisci il mondo {0}",
			         "Finalizar el mundo");

			L10N.Add(STR_PREV_OR,
			         "OR",
			         "ODER",
			         "OU",
			         "oppure",
			         "O");

			L10N.Add(STR_PREV_MISS_TOAST,
			         "You are missing {0} points to unlock world {1}",
			         "Dir fehlen noch {0} Punkte um Welt {1} freizuschalten",
			         "Vous manquez de {0} points pour débloquer le monde {1}",
			         "Ti mancano {0} punti per sbloccare il mondo {1}",
			         "Te faltan {0} puntos para desbloquear el mundo {1}");

			L10N.Add(STR_MP_TIMEOUT,
			         "Timeout - Connection to server lost",
			         "Timeout - Verbindung zu server verloren",
			         "Timeout - Connexion au serveur perdu",
			         "Timeout - Connessione al server persa",
			         "Timeout - sin conexión a Server");

			L10N.Add(STR_MP_TIMEOUT_USER,
			         "Timeout - Connection to user [{0}] lost",
			         "Timeout - Verbindung zu Spieler [{0}] verloren",
			         "Timeout - Connexion à l'utilisateur [{0}] perdu",
			         "Timeout - Connection all'utente [{0}] persa",
			         "Timeout - sin conexión a usuario");

			L10N.Add(STR_MP_NOTINLOBBY,
			         "You a not part of this session",
			         "Du bist kein Teilnehmer dieser Sitzung",
			         "Vous ne faites pas partie de cette session",
			         "Non fai parte di questa sessione",
			         "No eres parte de esta sesión");

			L10N.Add(STR_MP_SESSIONNOTFOUND,
			         "Session on server not found",
			         "Sitzung konnte auf dem Server nicht gefunden werden",
			         "Session sur le serveur pas trouvé",
			         "Nessuna sessione trovata su questo server",
			         "Sesión no pudo ser encontrada");

			L10N.Add(STR_MP_AUTHFAILED,
			         "Authentification on server failed",
			         "Authentifizierung auf Server fehlgeschlagen",
			         "L'authentification sur serveur a échoué",
			         "Autenticazione sul server fallita",
			         "Autentificación en Server ha fallado");

			L10N.Add(STR_MP_LOBBYFULL,
			         "Server lobby is full",
			         "Serverlobby ist voll",
			         "Le lobby du serveur est plein",
			         "La lobby del server è piena",
			         "Server lobby está lleno");

			L10N.Add(STR_MP_VERSIONMISMATCH,
			         "Server has a different game version ({0})",
			         "Serverversion unterscheidet sich von lokaler Version ({0})",
			         "Le serveur a une version de jeu différente({0})",
			         "Il server ha una versione diversa del gioco ({0})",
			         "Server tiene una versión de juego diferente ({0})");

			L10N.Add(STR_MP_LEVELNOTFOUND,
			         "Could not find server level locally",
			         "Level konnte lokal nicht gefunden werden",
			         "Impossible de trouver le niveau de serveur localement",
			         "Impossibile trovare il livello nel server sul telefono",
			         "Nivel no pudo ser encontrado localmente");

			L10N.Add(STR_MP_LEVELMISMATCH,
			         "Server has different version of level",
			         "Level auf dem Server unterscheidet sich von lokaler Version",
			         "Le serveur a une version de niveau différente",
			         "Il server ha una versione diversa del livello",
			         "Server tiene un nivel de versión diferente");

			L10N.Add(STR_MP_USERDISCONNECT,
			         "User {0} has disconnected",
			         "Der Benutzer {0} hat die Verbindung getrennt",
			         "L'utilisateur {0} s'est déconnecté",
			         "L'utente {0} si è disconnesso",
			         "Usuario {0} se ha desconectado");

			L10N.Add(STR_MP_SERVERDISCONNECT,
			         "Server has closed this session",
			         "Spiel wurde vom Server geschlossen",
			         "Le serveur a fermé cette session",
			         "Il server ha chiuso questa sessione",
			         "Server ha cerrado esta sesión");

			L10N.Add(STR_MP_INTERNAL,
			         "Internal multiplayer error",
			         "Interner Fehler im Mehrspielermodul",
			         "Error interal au module multiplayer",
			         "Problema intero di multiplayer",
			         "Error en módulo mas de un jugador");

			L10N.Add(STR_MP_BTADAPTERNULL,
			         "No bluetooth hardware found",
			         "Bluetooth Hardware nicht gefunden",
			         "Bluetooth n'a pas été trouvé",
			         "Nessun apparecchio bluetooth trovato",
			         "Bluetooth no encontrado");

			L10N.Add(STR_MP_BTADAPTERPERMDENIED,
			         "Missing bluetooth permission",
			         "Bluetooth Berechtigung wurde nicht gewährt",
			         "Absence d'autorisation de bluetooth",
			         "Permessi bluetooth disattivati",
			         "Autorización de Bluetooth desactivada");

			L10N.Add(STR_MP_BTDISABLED,
			         "Bluetooth is disabled",
			         "Bluetooth ist deaktiviert",
			         "Connexion Bluetooth deactivé",
			         "Bluetooth disattivatp",
			         "Bluetooth no disponible");

			L10N.Add(STR_MP_DIRECTCONNFAIL,
			         "Bluetooth connection failed",
			         "Bluetooth Verbindungsaufbau fehlgeschlagen",
			         "Connexion Bluetooth échoué",
			         "Connessione Bluetooth fallita",
			         "Conexión a Bluetooth ha fallado");

			L10N.Add(STR_MP_DIRECTCONNLOST,
			         "Bluetooth connection lost",
			         "Bluetooth Verbindung verloren",
			         "Connexion Bluetooth perdu",
			         "Connessione Bluetooth persa",
			         "Conexión a Bluetooth perdida");

			L10N.Add(STR_MP_NOSERVERCONN,
			         "No connection to server",
			         "Keine Verbindung zu Server",
			         "Pas de connexion au serveur",
			         "Nessuna connessione al server",
			         "Sin conexión a Server");

			L10N.Add(STR_MENU_CAP_MULTIPLAYER,
			         "Multiplayer",
			         "Mehrspieler",
			         "Multijoueur",
			         "Multiplayer",
			         "Multijugador");

			L10N.Add(STR_MENU_CAP_LOBBY,
			         "Multiplayer Lobby",
			         "Lobby",
			         "Online Lobby",
			         "Multiplayer Lobby",
			         "Lobby mas de un jugador");

			L10N.Add(STR_MENU_CAP_CGAME_PROX,
			         "Create Online Game",
			         "Onlinespiel erstellen",
			         "Creer un jeu en ligne",
			         "Crea una partita Online",
			         "Crear juego online");

			L10N.Add(STR_MENU_CAP_CGAME_BT,
			         "Create Local Game",
			         "Lokales Spiel erstellen",
			         "Creer un jeu local",
			         "Crea una partita Local",
			         "Crear un juego local");

			L10N.Add(STR_MENU_CAP_SEARCH,
			         "Search for local devices",
			         "Suche nach lokalem Spiel",
			         "Cherchez des périphériques locaux",
			         "Cerca dispostivi locali",
			         "Buscando dispositivos locales");

			L10N.Add(STR_MP_ONLINE,
			         "Online",
			         "Online",
			         "En ligne",
			         "Online",
			         "Online");

			L10N.Add(STR_MP_OFFLINE,
			         "Offline",
			         "Offline",
			         "Hors ligne",
			         "Offline",
			         "Offline");

			L10N.Add(STR_MP_CONNECTING,
			         "Connecting",
			         "Verbinden",
			         "Connecter",
			         "Connessione",
			         "Conectando");

			L10N.Add(STR_MENU_MP_JOIN,
			         "Join",
			         "Beitreten",
			         "Joindre",
			         "Partecipa",
			         "Participar");

			L10N.Add(STR_MENU_MP_HOST,
			         "Host",
			         "Erstellen",
			         "Rédiger",
			         "Ospita",
			         "Crear");

			L10N.Add(STR_MENU_MP_CREATE,
			         "Create",
			         "Start",
			         "Créer",
			         "Crea",
			         "Iniciar");

			L10N.Add(STR_MENU_CANCEL,
			         "Cancel",
			         "Abbrechen",
			         "Abandonner",
			         "Cancella",
			         "Cancelar");

			L10N.Add(STR_MENU_DISCONNECT,
			         "Disconnect",
			         "Verbindung trennen",
			         "Déconnecter",
			         "Disconnetti",
			         "Desconectar");

			L10N.Add(STR_MENU_MP_LOCAL_CLASSIC,
			         "Local (Bluetooth)",
			         "Lokal (Bluetooth)",
			         "Local (Bluetooth)",
			         "Locale (Bluetooth)",
			         "Local (Bluetooth)");

			L10N.Add(STR_MENU_MP_ONLINE,
			         "Online (UDP/IP)",
			         "Internet (UDP/IP)",
			         "En ligne (UDP/IP)",
			         "Online (UDP/IP)",
			         "Online (UDP/IP)");

			L10N.Add(STR_MENU_CAP_AUTH,
			         "Enter lobby code",
			         "Lobby Code eingeben",
			         "Entrer lobby code",
			         "Inserisci codice lobby",
			         "Ingresa código lobby");

			L10N.Add(STR_MENU_MP_GAMESPEED,
			         "Game speed:",
			         "Spielgeschwindigkeit:",
			         "La vitesse du jeux",
			         "Velocità gioco:",
			         "Velocidad de juego");

			L10N.Add(STR_MENU_MP_MUSIC,
			         "Background music:",
			         "Hintergrundmusik:",
			         "Musique de fond",
			         "Musica:",
			         "Música de fondo");

			L10N.Add(STR_MENU_MP_LOBBYINFO,
			         "Enter this code on another phone to join this session.",
			         "Gib diesen Code auf einem anderen Smartphone ein, um diesem Spiel beizutreten",
			         "Entrez ce code sur un autre smartphone pour rejoindre ce jeu",
			         "Inserire questo codice su un altro telefono per unirsi alla sessione.",
			         "Ingresa este código en otro Smartphone para unirte al juego");

			L10N.Add(STR_MENU_MP_LOBBY_USER,
			         "Users:",
			         "Mitspieler:",
			         "Coéquipier:",
			         "Utenti:",
			         "Usuarios:");

			L10N.Add(STR_MENU_MP_LOBBY_USER_FMT,
			         "Users: {0}",
			         "Mitspieler: {0}",
			         "Coéquipier: {0}",
			         "Utenti: {0}",
			         "Usuario: {0}");

			L10N.Add(STR_MENU_MP_LOBBY_LEVEL,
			         "Level:",
			         "Level:",
			         "Level:",
			         "Livello:",
			         "Nivel");

			L10N.Add(STR_MENU_MP_LOBBY_MUSIC,
			         "Background music:",
			         "Hintergrundmusik:",
			         "Musique de fond:",
			         "Musica:",
			         "Música de fondo");

			L10N.Add(STR_MENU_MP_LOBBY_SPEED,
			         "Game speed:",
			         "Spielgeschwindigkeit:",
			         "La vitesse de jeu",
			         "Velocità gioco:",
			         "Velocidad de juego");

			L10N.Add(STR_MENU_MP_LOBBY_PING,
			         "Ping",
			         "Ping",
			         "Ping",
			         "Ping",
			         "Ping");

			L10N.Add(STR_MENU_MP_START,
			         "Start",
			         "Start",
			         "Démarrage",
			         "Inizia",
			         "Iniciar");

			L10N.Add(STR_FRAC_N0,
			         "Gray",
			         "Gray",
			         "Gris",
			         "Grigio",
			         "Gris");

			L10N.Add(STR_FRAC_P1,
			         "Green",
			         "Grün",
			         "Vert",
			         "Verde",
			         "Verde");

			L10N.Add(STR_FRAC_A2,
			         "Red",
			         "Rot",
			         "Rouge",
			         "Rosso",
			         "Rojo");

			L10N.Add(STR_FRAC_A3,
			         "Blue",
			         "Blau",
			         "Bleu",
			         "Blu",
			         "Azul");

			L10N.Add(STR_FRAC_A4,
			         "Purple",
			         "Lila",
			         "Violet",
			         "Viola",
			         "Púrpura");

			L10N.Add(STR_FRAC_A5,
			         "Orange",
			         "Orange",
			         "Orange",
			         "Arancione",
			         "Naranja");

			L10N.Add(STR_FRAC_A6,
			         "Teal",
			         "BlauGrün",
			         "Vert bleu",
			         "Turchese",
			         "Turquesa");

			L10N.Add(STR_MENU_MP_LOBBY_COLOR,
			         "Color",
			         "Farbe",
			         "Couleur",
			         "Colore",
			         "Colores");

			L10N.Add(STR_HSP_NEWGAME,
			         "New Game",
			         "Neues Spiel",
			         "Nouveau jeu",
			         "Nuova partita",
			         "Nuevo juego");

			L10N.Add(STR_HSP_RANDOMGAME,
			         "Random level",
			         "Zufälliges Level",
			         "Niveau aléatoire",
			         "Livello casuale",
			         "Nivel aleatorio");

			L10N.Add(STR_HSP_MPPOINTS,
			         "Multiplayer score",
			         "Mehrspieler Punkte",
			         "Multiplayer score",
			         "Punteggio Multiplayer",
			         "Puntaje Multiplayer");

			L10N.Add(STR_MP_TOAST_CONN_TRY,
			         "Connecting to '{0}'",
			         "Verbinden mit '{0}'",
			         "Connecter à '{0}'",
			         "Connessione a '{0}'",
			         "Conectando a");

			L10N.Add(STR_MP_TOAST_CONN_FAIL,
			         "Connection to '{0}' failed",
			         "Verbindung mit '{0}' fehlgeschlagen",
			         "Connexion avec '{0}' est échoué",
			         "Connessione a '{0}' fallita",
			         "Conección a '{0}' ha fallado");

			L10N.Add(STR_MP_TOAST_CONN_SUCC,
			         "Connected to '{0}'",
			         "Verbunden mit '{0}'",
			         "Connecté avec '{0}'",
			         "Connessione a '{0}'",
			         "Conectado a '{0}'");

			L10N.Add(STR_ENDGAME_1,
			         "THANKS FOR",
			         "THANKS FOR",
			         "THANKS FOR",
			         "GRAZIE PER",
			         "GRACIAS POR");

			L10N.Add(STR_ENDGAME_2,
			         "PLAYING",
			         "PLAYING",
			         "PLAYING",
			         "AVER GIOCATO",
			         "JUGANDO");

			L10N.Add(STR_ACCOUNT_REMINDER,
			         "You can create an account to display your name in the highscore and to backup your score online.\nDo you want to create an account now?",
			         "Du kannst einen Onlineaccount anlegen um deinen Namen im Highscore zu sehen und deine Punkte zu sichern.\n Account jetzt erstellen?",
			         "Vous pouvez créer un compte pour afficher votre nom dans les meilleurs scores et sauvegarder vos points en ligne.\nVoulez - vous créer un compte maintenant?",
			         "Puoi creare un account per far apparire il tuo nome nella classifica e per salvare i tuoi dati online.\nVuoi creare un'account?",
			         "Puedes crear una cuenta online para registrar tu puntuación y guardar tus puntajes altos.\nCrear cuenta ahora?");

			L10N.Add(STR_BTN_YES,
			         "Yes",
			         "OK",
			         "OK",
			         "Si",
			         "Sí");

			L10N.Add(STR_BTN_NO,
			         "No",
			         "Nein",
			         "Aucun",
			         "No",
			         "No");

			L10N.Add(STR_ERR_SOUNDPLAYBACK,
			         "Sound playback failed. Disabling sounds ...",
			         "Soundwiedergabe fehlgeschlagen. Sounds werden deaktiviert ...",
			         "La lecture du son a échoué. Désactivation des sons ...",
			         "La riproduzione audio non è riuscita. Disattivazione dei suoni ...",
			         "Reproducción de sonido ha fallado. Deactivando sonidos");

			L10N.Add(STR_ERR_MUSICPLAYBACK,
			         "Music playback failed. Disabling music ...",
			         "Musikwiedergabe fehlgeschlagen. Musik wird deaktiviert ...",
			         "La lecture de musique a échoué. Désactivation de la musique ...",
			         "La riproduzione musicale è fallita. Disattivazione della musica ...",
			         "Reproducción de música ha fallado. Deactivando sonidos");

			L10N.Add(STR_ERR_OUTOFMEMORY,
			         "Saving failed: Disk full",
			         "Speichern fehlgeschlagen: Speicher voll",
			         "Échec échoué: Disque complet",
			         "Salvataggio non riuscito: disco pieno",
			         "Memoria llena: almacenamiento ha fallado ");

			L10N.Add(STR_PROFILESYNC_START,
			         "Starting manual sync",
			         "Manuelle Synchronisation gestartet",
			         "La synchronisation manuelle a commencé",
			         "La sincronizzazione manuale è iniziata",
			         "Inicio de sincronización manual");

			L10N.Add(STR_PROFILESYNC_ERROR,
			         "Manual sync failed",
			         "Manuelle Synchronisation fehlgeschlagen",
			         "La synchronisation manuelle a échoué",
			         "La sincronizzazione manuale non è riuscita",
			         "La sincronización manual falló");

			L10N.Add(STR_PROFILESYNC_SUCCESS,
			         "Manual sync finished",
			         "Manuelle Synchronisation erfolgreich",
			         "Synchronisation manuelle réussie",
			         "La sincronizzazione manuale è riuscita",
			         "Sincronización manual finalizada");

			L10N.Add(STR_AUTHERR_HEADER,
			         "You've been logged out",
			         "Sie wurden ausgeloggt",
			         "Vous avez été déconnecté",
			         "Sei stato disconnesso",
			         "Has sido desconectado");


			L10N.Add(STR_WORLD_ONLINE,
			         "Online",
			         "Online",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO


			L10N.Add(STR_LVLED_MOUSE,
			         "Move",
			         "Verschieben",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_CANNON,
			         "Cannon",
			         "Kanone",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_WALL,
			         "Wall",
			         "Wand",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_OBSTACLE,
			         "Obstacle",
			         "Hindernis",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_SETTINGS,
			         "Settings",
			         "Einstellungen",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_PLAY,
			         "Test",
			         "Testen",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_UPLOAD,
			         "Upload",
			         "Hochladen",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_EXIT,
			         "Exit",
			         "Beenden",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO


			L10N.Add(STR_LVLED_BTN_FRAC,
			         "Fraction",
			         "Fraktion",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_BTN_SCALE,
			         "Scale",
			         "Größe",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_BTN_ROT,
			         "Rotation",
			         "Drehung",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_BTN_TYPE,
			         "Type",
			         "Typ",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_BTN_DIAMETER,
			         "Durchmesser",
			         "Diameter",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_BTN_WIDTH,
			         "Breite",
			         "Width",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_BTN_HEIGHT,
			         "Höhe",
			         "Height",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_BTN_POWER,
			         "Stärke",
			         "Power",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_BTN_DEL,
			         "Delete",
			         "Entfernen",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO
			
			L10N.Add(STR_LVLED_CFG_ID,
			         "ID",
			         "ID",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_CFG_NAME,
			         "Name",
			         "Name",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_CFG_SIZE,
			         "Größe",
			         "Size",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_CFG_VIEW,
			         "Initial view",
			         "Startansicht",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_CFG_GEOMETRY,
			         "Geometrie",
			         "Geometry",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_CFG_WRAP_INFINITY,
			         "Endlos",
			         "Infinity",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_CFG_WRAP_DONUT,
			         "Donut",
			         "Donut",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			L10N.Add(STR_LVLED_CFG_WRAP_REFLECT,
			         "Reflektierend",
			         "Reflect",
			         "?",  //TODO
			         "?",  //TODO
			         "?"); //TODO

			// [en_US] [de-DE] [fr-FR] [it-IT] [es-ES]

#if DEBUG
			L10N.Verify();
#endif
		}

		public static string FormatNetworkErrorMessage(SAMNetworkConnection.ErrorType type, object data)
		{
			switch (type)
			{
				case SAMNetworkConnection.ErrorType.None:
					return string.Empty;

				case SAMNetworkConnection.ErrorType.ProxyServerTimeout:
					return L10N.T(STR_MP_TIMEOUT);

				case SAMNetworkConnection.ErrorType.ServerUserTimeout:
					return L10N.T(STR_MP_TIMEOUT);

				case SAMNetworkConnection.ErrorType.UserTimeout:
					return L10N.TF(STR_MP_TIMEOUT_USER, data);

				case SAMNetworkConnection.ErrorType.NotInLobby:
					return L10N.T(STR_MP_NOTINLOBBY);

				case SAMNetworkConnection.ErrorType.SessionNotFound:
					return L10N.T(STR_MP_SESSIONNOTFOUND);

				case SAMNetworkConnection.ErrorType.AuthentificationFailed:
					return L10N.T(STR_MP_AUTHFAILED);

				case SAMNetworkConnection.ErrorType.LobbyFull:
					return L10N.T(STR_MP_LOBBYFULL);

				case SAMNetworkConnection.ErrorType.GameVersionMismatch:
					return L10N.TF(STR_MP_VERSIONMISMATCH, GDConstants.Version.ToString());

				case SAMNetworkConnection.ErrorType.LevelNotFound:
					return L10N.T(STR_MP_LEVELNOTFOUND);

				case SAMNetworkConnection.ErrorType.LevelVersionMismatch:
					return L10N.T(STR_MP_LEVELMISMATCH);

				case SAMNetworkConnection.ErrorType.UserDisconnect:
					return L10N.TF(STR_MP_USERDISCONNECT, data);

				case SAMNetworkConnection.ErrorType.ServerDisconnect:
					return L10N.T(STR_MP_SERVERDISCONNECT);

				case SAMNetworkConnection.ErrorType.BluetoothAdapterNotFound:
					return L10N.T(STR_MP_BTADAPTERNULL);

				case SAMNetworkConnection.ErrorType.BluetoothAdapterNoPermission:
					return L10N.T(STR_MP_BTADAPTERPERMDENIED);

				case SAMNetworkConnection.ErrorType.NetworkMediumInternalError:
					return L10N.T(STR_MP_INTERNAL);

				case SAMNetworkConnection.ErrorType.BluetoothNotEnabled:
					return L10N.T(STR_MP_BTDISABLED);

				case SAMNetworkConnection.ErrorType.P2PConnectionFailed:
					return L10N.T(STR_MP_DIRECTCONNFAIL);

				case SAMNetworkConnection.ErrorType.P2PConnectionLost:
					return L10N.T(STR_MP_DIRECTCONNLOST);

				case SAMNetworkConnection.ErrorType.P2PNoServerConnection:
					return L10N.T(STR_MP_NOSERVERCONN);

				default:
					SAMLog.Error("L10NI::EnumSwitch_FNEM", "type = "+ type);
					return string.Empty;
			}
		}
	}
}
