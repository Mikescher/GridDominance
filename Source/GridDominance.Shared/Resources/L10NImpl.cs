using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;

namespace GridDominance.Shared.Resources
{
	public static class L10NImpl
	{
		public const int STR_SSB_ABOUT              = 00;
		public const int STR_SSB_ACCOUNT            = 01;
		public const int STR_SSB_HIGHSCORE          = 02;
		public const int STR_SSB_MUTE               = 03;
		public const int STR_SSB_EFFECTS            = 04;
		public const int STR_SSB_LANGUAGE           = 69;
		public const int STR_SSB_MUSIC              = 111;

		public const int STR_HSP_GLOBALRANKING      = 05;
		public const int STR_HSP_RANKINGFOR         = 06;

		public const int STR_TAB_NAME               = 07;
		public const int STR_TAB_POINTS             = 08;
		public const int STR_TAB_TIME               = 09;

		public const int STR_FAP_ACCOUNT            = 10;
		public const int STR_FAP_USERNAME           = 11;
		public const int STR_FAP_SCORE              = 78;
		public const int STR_FAP_CHANGEPW           = 12;
		public const int STR_FAP_LOGOUT             = 93;
		public const int STR_FAP_WARN1              = 94;
		public const int STR_FAP_WARN2              = 95;
		public const int STR_FAP_LOGOUT_SUCESS      = 96;

		public const int STR_CPP_CHANGEPW           = 13;
		public const int STR_CPP_USERNAME           = 14;
		public const int STR_CPP_NEWPW              = 15;
		public const int STR_CPP_CHANGE             = 16;
		public const int STR_CPP_CHANGING           = 17;
		public const int STR_CPP_CHANGED            = 18;
		public const int STR_CPP_COMERR             = 19;
		public const int STR_CPP_AUTHERR            = 20;
		public const int STR_CPP_CHANGEERR          = 21;

		public const int STR_ATTRIBUTIONS           = 22;
		public const int STR_UNLOCK                 = 85;

		public const int STR_AAP_HEADER             = 23;
		public const int STR_AAP_USERNAME           = 24;
		public const int STR_AAP_PASSWORD           = 25;
		public const int STR_AAP_CREATEACCOUNT      = 26;
		public const int STR_AAP_LOGIN              = 27;
		public const int STR_AAP_LOGGINGIN          = 28;
		public const int STR_AAP_WRONGPW            = 29;
		public const int STR_AAP_USERNOTFOUND       = 30;
		public const int STR_AAP_NOCOM              = 31;
		public const int STR_AAP_LOGINSUCCESS       = 32;
		public const int STR_AAP_NOLOGIN            = 33;
		public const int STR_AAP_ACCCREATING        = 34;
		public const int STR_AAP_ACCCREATED         = 35;
		public const int STR_AAP_USERTAKEN          = 36;
		public const int STR_AAP_ALREADYCREATED     = 37;
		public const int STR_AAP_AUTHERROR          = 38;
		public const int STR_AAP_COULDNOTCREATE     = 39;

		public const int STR_PAUS_RESUME            = 40;
		public const int STR_PAUS_RESTART           = 41;
		public const int STR_PAUS_EXIT              = 42;

		public const int STR_HSP_LEVEL              = 43;
		public const int STR_HSP_POINTS             = 44;
		public const int STR_HSP_MPPOINTS           = 160;
		public const int STR_HSP_PROGRESS           = 45;
		public const int STR_HSP_BACK               = 46;
		public const int STR_HSP_NEXT               = 47;
		public const int STR_HSP_AGAIN              = 48;
		public const int STR_HSP_TUTORIAL           = 49;
		public const int STR_HSP_GETSTARTED         = 50;
		public const int STR_HSP_CONERROR           = 65;
		public const int STR_HSP_NEWGAME            = 158;
		public const int STR_HSP_RANDOMGAME         = 159;

		public const int STR_DIFF_0                 = 51;
		public const int STR_DIFF_1                 = 52;
		public const int STR_DIFF_2                 = 53;
		public const int STR_DIFF_3                 = 54;

		public const int STR_TUT_INFO1              = 79;
		public const int STR_TUT_INFO2              = 55;
		public const int STR_TUT_INFO3              = 56;
		public const int STR_TUT_INFO4              = 57;
		public const int STR_TUT_INFO5              = 58;
		public const int STR_TUT_INFO6              = 59;
		public const int STR_TUT_INFO7              = 60;
		public const int STR_TUT_INFO8              = 61;

		public const int STR_API_CONERR             = 62;
		public const int STR_API_COMERR             = 63;

		public const int STR_GLOB_EXITTOAST         = 64;
		public const int STR_GLOB_UNLOCKTOAST1      = 66;
		public const int STR_GLOB_UNLOCKTOAST2      = 67;
		public const int STR_GLOB_UNLOCKTOAST3      = 68;
		public const int STR_GLOB_LEVELLOCK         = 70;
		public const int STR_GLOB_WORLDLOCK         = 71;
		public const int STR_GLOB_OVERWORLD         = 75;
		public const int STR_GLOB_WAITFORSERVER     = 76;
		public const int STR_GLOB_UNLOCKSUCCESS     = 86;

		public const int STR_WORLD_TUTORIAL         = 77;
		public const int STR_WORLD_W1               = 80;
		public const int STR_WORLD_W2               = 81;
		public const int STR_WORLD_W3               = 92;
		public const int STR_WORLD_W4               = 103;
		public const int STR_WORLD_MULTIPLAYER      = 114;

		public const int STR_INF_YOU                = 72;
		public const int STR_INF_GLOBAL             = 73;
		public const int STR_INF_HIGHSCORE          = 74;

		public const int STR_IAB_TESTERR            = 82;
		public const int STR_IAB_TESTNOCONN         = 83;
		public const int STR_IAB_TESTINPROGRESS     = 84;

		public const int STR_IAB_BUYERR             = 88;
		public const int STR_IAB_BUYNOCONN          = 89;
		public const int STR_IAB_BUYNOTREADY        = 90;
		public const int STR_IAB_BUYSUCESS          = 91;

		public const int STR_PREV_BUYNOW            = 87;
		public const int STR_PREV_FINISHWORLD       = 108;
		public const int STR_PREV_OR                = 109;
		public const int STR_PREV_MISS_TOAST        = 110;

		public const int STR_HINT_001               = 97;
		public const int STR_HINT_002               = 98;
		public const int STR_HINT_003               = 99;
		public const int STR_HINT_004               = 100;
		public const int STR_HINT_005               = 101;
		public const int STR_HINT_006               = 102;
		public const int STR_HINT_007               = 112;
		public const int STR_HINT_008               = 113;

		public const int STR_INFOTOAST_1            = 104;
		public const int STR_INFOTOAST_2            = 105;
		public const int STR_INFOTOAST_3            = 106;
		public const int STR_INFOTOAST_4            = 107;

		public const int STR_MP_TIMEOUT             = 115;
		public const int STR_MP_TIMEOUT_USER        = 116;
		public const int STR_MP_NOTINLOBBY          = 117;
		public const int STR_MP_SESSIONNOTFOUND     = 118;
		public const int STR_MP_AUTHFAILED          = 119;
		public const int STR_MP_LOBBYFULL           = 120;
		public const int STR_MP_VERSIONMISMATCH     = 121;
		public const int STR_MP_LEVELNOTFOUND       = 122;
		public const int STR_MP_LEVELMISMATCH       = 123;
		public const int STR_MP_USERDISCONNECT      = 124;
		public const int STR_MP_SERVERDISCONNECT    = 125;
		public const int STR_MP_INTERNAL            = 161; 
		public const int STR_MP_BTADAPTERNULL       = 162;

		public const int STR_MENU_CANCEL            = 138;
		public const int STR_MENU_DISCONNECT        = 147;

		public const int STR_MENU_CAP_MULTIPLAYER   = 126;
		public const int STR_MENU_CAP_LOBBY         = 127;
		public const int STR_MENU_CAP_CGAME         = 128;
		public const int STR_MENU_CAP_AUTH          = 136;
		public const int STR_MENU_MP_JOIN           = 132;
		public const int STR_MENU_MP_HOST           = 133;
		public const int STR_MENU_MP_START          = 149;
		public const int STR_MENU_MP_ONLINE         = 135;
		public const int STR_MENU_MP_LOCAL          = 134;
		public const int STR_MENU_MP_CREATE         = 137;
		public const int STR_MENU_MP_GAMESPEED      = 139;
		public const int STR_MENU_MP_MUSIC          = 140;
		public const int STR_MENU_MP_LOBBYINFO      = 141;
		public const int STR_MENU_MP_LOBBY_USER     = 142;
		public const int STR_MENU_MP_LOBBY_USER_FMT = 148;
		public const int STR_MENU_MP_LOBBY_LEVEL    = 143;
		public const int STR_MENU_MP_LOBBY_MUSIC    = 144;
		public const int STR_MENU_MP_LOBBY_SPEED    = 145;
		public const int STR_MENU_MP_LOBBY_PING     = 146;
		public const int STR_MENU_MP_LOBBY_COLOR    = 157;

		public const int STR_MP_ONLINE              = 129;
		public const int STR_MP_OFFLINE             = 130;
		public const int STR_MP_CONNECTING          = 131;

		public const int STR_FRAC_N0                = 150;
		public const int STR_FRAC_P1                = 151;
		public const int STR_FRAC_A2                = 152;
		public const int STR_FRAC_A3                = 153;
		public const int STR_FRAC_A4                = 154;
		public const int STR_FRAC_A5                = 155;
		public const int STR_FRAC_A6                = 156;

		private const int TEXT_COUNT = 163; // = next idx

		public static void Init(int lang)
		{
			L10N.Init(lang, TEXT_COUNT);

			L10N.Add(STR_SSB_ABOUT,              "About",                                                       "Info");
			L10N.Add(STR_SSB_ACCOUNT,            "Account",                                                     "Benutzerkonto");
			L10N.Add(STR_SSB_HIGHSCORE,          "Highscore",                                                   "Bestenliste");
			L10N.Add(STR_SSB_MUTE,               "Mute",                                                        "Stumm");
			L10N.Add(STR_SSB_EFFECTS,            "Effects",                                                     "Effekte");
			L10N.Add(STR_SSB_MUSIC,              "Music",                                                       "Musik");
			L10N.Add(STR_HSP_GLOBALRANKING,      "Global Ranking",                                              "Globale Bestenliste");
			L10N.Add(STR_HSP_RANKINGFOR,         "Ranking for \"{0}\"",                                         "Bestenliste für \"{0}\"");
			L10N.Add(STR_TAB_NAME,               "Name",                                                        "Name");
			L10N.Add(STR_TAB_POINTS,             "Points",                                                      "Punkte");
			L10N.Add(STR_TAB_TIME,               "Total Time",                                                  "Gesamtzeit");
			L10N.Add(STR_FAP_ACCOUNT,            "Account",                                                     "Benutzerkonto");
			L10N.Add(STR_FAP_USERNAME,           "Username:",                                                   "Benutzername:");
			L10N.Add(STR_FAP_SCORE,              "Points:",                                                     "Punkte:");
			L10N.Add(STR_FAP_CHANGEPW,           "Change Password",                                             "Passwort ändern");
			L10N.Add(STR_FAP_LOGOUT,             "Logout",                                                      "Ausloggen");
			L10N.Add(STR_FAP_WARN1,              "This will clear all local data. Press again to log out.",     "Dies löscht alle lokalen Daten. Nochmal drücken zum ausloggen.");
			L10N.Add(STR_FAP_WARN2,              "Are you really sure you want to log out?",                    "Wirklich vom Serverkonto abmelden?");
			L10N.Add(STR_FAP_LOGOUT_SUCESS,      "Logged out from account",                                     "Lokaler Benutzer wurde abgemeldet.");
			L10N.Add(STR_CPP_CHANGEPW,           "Change Password",                                             "Passwort ändern");
			L10N.Add(STR_CPP_USERNAME,           "Username:",                                                   "Benutzername:");
			L10N.Add(STR_CPP_NEWPW,              "New Password",                                                "Neues Passwort");
			L10N.Add(STR_CPP_CHANGE,             "Change",                                                      "Ändern");
			L10N.Add(STR_CPP_CHANGING,           "Changing password",                                           "Passwort wird geändert");
			L10N.Add(STR_CPP_CHANGED,            "Password changed",                                            "Passwort geändert");
			L10N.Add(STR_CPP_COMERR,             "Could not communicate with server",                           "Kommunikation mit Server ist gestört");
			L10N.Add(STR_CPP_AUTHERR,            "Authentication error",                                        "Authentifizierung fehlgeschlagen");
			L10N.Add(STR_CPP_CHANGEERR,          "Could not change password",                                   "Passwort konnte nicht geändert werden");
			L10N.Add(STR_ATTRIBUTIONS,           "Attributions",                                                "Lizenzen");
			L10N.Add(STR_AAP_HEADER,             "Sign up / Log in",                                            "Anmelden / Registrieren");
			L10N.Add(STR_AAP_USERNAME,           "Username",                                                    "Benutzername");
			L10N.Add(STR_AAP_PASSWORD,           "Password",                                                    "Passwort");
			L10N.Add(STR_AAP_CREATEACCOUNT,      "Create Account",                                              "Registrieren");
			L10N.Add(STR_AAP_LOGIN,              "Login",                                                       "Anmelden");
			L10N.Add(STR_AAP_LOGGINGIN,          "Logging in",                                                  "Wird angemeldet");
			L10N.Add(STR_AAP_WRONGPW,            "Wrong password",                                              "Falsches Passwort");
			L10N.Add(STR_AAP_USERNOTFOUND,       "User not found",                                              "Benutzer nicht gefunden");
			L10N.Add(STR_AAP_NOCOM,              "Could not communicate with server",                           "Konnte nicht mit Server kommunizieren");
			L10N.Add(STR_AAP_LOGINSUCCESS,       "Successfully logged in",                                      "Benutzer erfolgreich angemeldet");
			L10N.Add(STR_AAP_NOLOGIN,            "Could not login",                                             "Anmeldung fehlgeschlagen");
			L10N.Add(STR_AAP_ACCCREATING,        "Creating account",                                            "Konto wird erstellt");
			L10N.Add(STR_AAP_ACCCREATED,         "Account created",                                             "Konto erfolgreich erstellt");
			L10N.Add(STR_AAP_USERTAKEN,          "Username already taken",                                      "Benutzername bereits vergeben");
			L10N.Add(STR_AAP_ALREADYCREATED,     "Account already created",                                     "Konto bereits erstellt");
			L10N.Add(STR_AAP_AUTHERROR,          "Authentication error",                                        "Authentifizierungsfehler");
			L10N.Add(STR_AAP_COULDNOTCREATE,     "Could not create account",                                    "Konto konnte nicht erstellt werden");
			L10N.Add(STR_PAUS_RESUME,            "RESUME",                                                      "WEITER");
			L10N.Add(STR_PAUS_RESTART,           "RESTART",                                                     "NEU STARTEN");
			L10N.Add(STR_PAUS_EXIT,              "EXIT",                                                        "BEENDEN");
			L10N.Add(STR_HSP_LEVEL,              "Level",                                                       "Level");
			L10N.Add(STR_HSP_POINTS,             "Points",                                                      "Punkte");
			L10N.Add(STR_HSP_PROGRESS,           "Progress",                                                    "Fortschritt");
			L10N.Add(STR_HSP_BACK,               "Back",                                                        "Zurück");
			L10N.Add(STR_HSP_NEXT,               "Next",                                                        "Weiter");
			L10N.Add(STR_HSP_AGAIN,              "Again",                                                       "Wiederholen");
			L10N.Add(STR_HSP_TUTORIAL,           "Tutorial",                                                    "Tutorial");
			L10N.Add(STR_HSP_GETSTARTED,         "Let's get started",                                           "Los gehts");
			L10N.Add(STR_HSP_CONERROR,           "Could not connect to highscore server",                       "Kommunikation mit Server fehlgeschlagen");
			L10N.Add(STR_DIFF_0,                 "Easy",                                                        "Leicht");
			L10N.Add(STR_DIFF_1,                 "Normal",                                                      "Normal");
			L10N.Add(STR_DIFF_2,                 "Hard",                                                        "Schwer");
			L10N.Add(STR_DIFF_3,                 "Extreme",                                                     "Extrem");
			L10N.Add(STR_TUT_INFO1,              "Drag to rotate your own cannons",                             "Drücke und Ziehe um deine Kanonen zu drehen");
			L10N.Add(STR_TUT_INFO2,              "Shoot it until it becomes your cannon",                       "Schieße bis die feindliche Kanone dir gehört");
			L10N.Add(STR_TUT_INFO3,              "Now capture the next cannon",                                 "Erobere nun die nächste Einheit");
			L10N.Add(STR_TUT_INFO4,              "Keep shooting at the first cannon to increase its fire rate", "Schieß auf deine eigene Kanone um ihre Feuerrate zu erhöhen");
			L10N.Add(STR_TUT_INFO5,              "The enemy has captured a cannon. Attack him!",                "Der Gegner hat eine Einheit erobert, greif ihn an!");
			L10N.Add(STR_TUT_INFO6,              "Speed up the Game with the bottom left button.",              "Mit dem Knopf unten links kannst du die Spielgeschwindigkeit erhöhen");
			L10N.Add(STR_TUT_INFO7,              "Now capture the next cannon",                                 "Erobere jetzt die nächste Einheit");
			L10N.Add(STR_TUT_INFO8,              "Win the game by capturing all enemy cannons",                 "Gewinne die Schlacht indem du alle Einheiten eroberst");
			L10N.Add(STR_API_CONERR,             "Could not connect to highscore server",                       "Verbindung mit Highscore Server fehlgeschlagen");
			L10N.Add(STR_API_COMERR,             "Could not communicate with highscore server",                 "Kommunikation mit Highscore Server fehlgeschlagen");
			L10N.Add(STR_GLOB_EXITTOAST,         "Click again to exit game",                                    "Drücke \"Zurück\" nochmal um das Spiel zu beenden");
			L10N.Add(STR_GLOB_UNLOCKTOAST1,      "Click two times more to unlock",                              "Noch zweimal drücken um die Welt freizuschalten");
			L10N.Add(STR_GLOB_UNLOCKTOAST2,      "Click again to unlock",                                       "Nochmal drücken um die Welt freizuschalten");
			L10N.Add(STR_GLOB_UNLOCKTOAST3,      "World unlocked",                                              "Welt freigeschaltet");
			L10N.Add(STR_GLOB_WORLDLOCK,         "World locked",                                                "Welt noch nicht freigespielt");
			L10N.Add(STR_GLOB_LEVELLOCK,         "Level locked",                                                "Level noch nicht freigespielt");
			L10N.Add(STR_INF_YOU,                "You",                                                         "Du");
			L10N.Add(STR_INF_GLOBAL,             "Global",                                                      "Global");
			L10N.Add(STR_INF_HIGHSCORE,          "Highscore",                                                   "Bestzeit");
			L10N.Add(STR_GLOB_OVERWORLD,         "Overworld",                                                   "Übersicht");
			L10N.Add(STR_GLOB_WAITFORSERVER,     "Contacting server",                                           "Server wird kontaktiert");
			L10N.Add(STR_WORLD_TUTORIAL,         "Tutorial",                                                    "Tutorial");
			L10N.Add(STR_SSB_LANGUAGE,           "Language",                                                    "Sprache");
			L10N.Add(STR_WORLD_W1,               "Basic",                                                       "Grundlagen");
			L10N.Add(STR_WORLD_W2,               "Professional",                                                "Fortgeschritten");
			L10N.Add(STR_WORLD_W3,               "Futuristic",                                                  "Futuristisch");
			L10N.Add(STR_WORLD_W4,               "Toy Box",                                                     "Spielzeugkiste");
			L10N.Add(STR_WORLD_MULTIPLAYER,      "Multiplayer",                                                 "Mehrspieler");
			L10N.Add(STR_IAB_TESTERR,            "Error connecting to Google Play services",                    "Fehler beim Versuch mit Google Play zu verbinden");
			L10N.Add(STR_IAB_TESTNOCONN,         "No connection to Google Play services",                       "Keine Verbindung zu Google Play services");
			L10N.Add(STR_IAB_TESTINPROGRESS,     "Payment in progress",                                         "Zahlung wird verarbeitet");
			L10N.Add(STR_UNLOCK,                 "Promotion Code",                                              "Freischaltungs Code");
			L10N.Add(STR_GLOB_UNLOCKSUCCESS,     "Upgraded game to full version!",                              "Spiel wurde zur Vollversion aufgewertet");
			L10N.Add(STR_PREV_BUYNOW,            "Unlock now",                                                  "Jetzt freischalten");
			L10N.Add(STR_IAB_BUYERR,             "Error connecting to Google Play services",                    "Fehler beim Versuch mit Google Play zu verbinden");
			L10N.Add(STR_IAB_BUYNOCONN,          "No connection to Google Play services",                       "Keine Verbindung zu Google Play services");
			L10N.Add(STR_IAB_BUYNOTREADY,        "Connection to Google Play services not ready",                "Verbindung zu Google Play services nicht bereit");
			L10N.Add(STR_IAB_BUYSUCESS,          "World sucesfully purchased",                                  "Levelpack wurde erfolgreich erworben");
			L10N.Add(STR_HINT_001,               "Tip: Shoot stuff to win!",                                    "Tipp: Versuch auf die andere Kanone zu schiessen");
			L10N.Add(STR_HINT_002,               "Bigger Cannon",                                               "Größere Kanone");
			L10N.Add(STR_HINT_003,               "More Power",                                                  "Mehr Schaden");
			L10N.Add(STR_HINT_004,               "Black holes attract your bullets",                            "Schwarze Löcher saugen deine Kugeln ein");
			L10N.Add(STR_HINT_005,               "Lasers!",                                                     "Laser!");
			L10N.Add(STR_HINT_006,               "Try dragging the map around",                                 "Versuch mal die Karte zu verschieben");
			L10N.Add(STR_HINT_007,               "Speedy thing goes in,",                                       "Speedy thing goes in,");
			L10N.Add(STR_HINT_008,               "speedy thing comes out.",                                     "speedy thing comes out.");
			L10N.Add(STR_INFOTOAST_1,            "Your best time is {0}",                                       "Deine Bestzeit ist {0}");
			L10N.Add(STR_INFOTOAST_2,            "The global best time is {0}",                                 "Versuch mal das Level zu verschieben");
			L10N.Add(STR_INFOTOAST_3,            "{0} users have completed this level on {1}",                  "{0} Spieler haben dieses Level auf {1} geschafft");
			L10N.Add(STR_INFOTOAST_4,            "You have not completed this level on {0}",                    "Du hast dieses Level auf {0} noch nicht geschafft");
			L10N.Add(STR_PREV_FINISHWORLD,       "Finish World {0}",                                            "Welt {0}");
			L10N.Add(STR_PREV_OR,                "OR",                                                          "ODER");
			L10N.Add(STR_PREV_MISS_TOAST,        "You are missing {0} points ({1} levels) to unlock world {2}", "Dir fehlen noch {0} Punkte ({1} level) um Welt {2} freizuschalten");
			L10N.Add(STR_MP_TIMEOUT,             "Timeout - Connection to server lost",                         "Timeout - Verbindung zu server verloren");
			L10N.Add(STR_MP_TIMEOUT_USER,        "Timeout - Connection to user [{0}] lost",                     "Timeout - Verbindung zu Spieler [{0}] verloren");
			L10N.Add(STR_MP_NOTINLOBBY,          "You a not part of this session",                              "Du bist kein Teilnehmer dieser Sitzung");
			L10N.Add(STR_MP_SESSIONNOTFOUND,     "Session on server not found",                                 "Sitzung konnte auf dem Server nicht gefunden werden");
			L10N.Add(STR_MP_AUTHFAILED,          "Authentification on server failed",                           "Authentifizierung auf Server fehlgeschlagen");
			L10N.Add(STR_MP_LOBBYFULL,           "Server lobby is full",                                        "Serverlobby ist voll");
			L10N.Add(STR_MP_VERSIONMISMATCH,     "Server has a different game version ({0})",                   "Serverversion unterscheidet sich von lokaler Version ({0})");
			L10N.Add(STR_MP_LEVELNOTFOUND,       "Could not find server level locally",                         "Level konnte lokal nicht gefunden werden");
			L10N.Add(STR_MP_LEVELMISMATCH,       "Server has different version of level",                       "Level auf dem Server unterscheidet sich von lokaler Version");
			L10N.Add(STR_MP_USERDISCONNECT,      "User {0} has disconnected",                                   "Der Benutzer {0} hat die Verbindung getrennt");
			L10N.Add(STR_MP_SERVERDISCONNECT,    "Server has closed this session",                              "Spiel wurde vom Server geschlossen");
			L10N.Add(STR_MP_INTERNAL,            "Internal multiplayer error",                                  "Interner Fehler im Mehrspielermodul");
			L10N.Add(STR_MP_BTADAPTERNULL,       "No bluetooth hardware found",                                   "Bluetooth Hardware nicht gefunden");
			L10N.Add(STR_MENU_CAP_MULTIPLAYER,   "Multiplayer",                                                 "Mehrspieler");
			L10N.Add(STR_MENU_CAP_LOBBY,         "Online Lobby",                                                "Internetlobby");
			L10N.Add(STR_MENU_CAP_CGAME,         "Create Online Game",                                          "Onlinespiel erstellen");
			L10N.Add(STR_MP_ONLINE,              "Online",                                                      "Online");
			L10N.Add(STR_MP_OFFLINE,             "Offline",                                                     "Offline");
			L10N.Add(STR_MP_CONNECTING,          "Connecting",                                                  "Verbinden");
			L10N.Add(STR_MENU_MP_JOIN,           "Join",                                                        "Beitreten");
			L10N.Add(STR_MENU_MP_HOST,           "Host",                                                        "Erstellen");
			L10N.Add(STR_MENU_MP_CREATE,         "Create",                                                      "Start");
			L10N.Add(STR_MENU_CANCEL,            "Cancel",                                                      "Abbrechen");
			L10N.Add(STR_MENU_DISCONNECT,        "Disconnect",                                                  "Verbindung trennen");
			L10N.Add(STR_MENU_MP_LOCAL,          "Local (Bluetooth)",                                           "Lokal (Bluetooth)");
			L10N.Add(STR_MENU_MP_ONLINE,         "Online (UDP/IP)",                                             "Internet (UDP/IP)");
			L10N.Add(STR_MENU_CAP_AUTH,          "Enter lobby code",                                            "Lobby Code eingeben");
			L10N.Add(STR_MENU_MP_GAMESPEED,      "Game speed:",                                                 "Spielgeschwindigkeit:");
			L10N.Add(STR_MENU_MP_MUSIC,          "Background music:",                                           "Hintergrundmusik:");
			L10N.Add(STR_MENU_MP_LOBBYINFO,      "Enter this code on another phone to join this session.",      "Gib diesen Code auf einem anderen Smartphones ein um diesem Spiel beizutreten");
			L10N.Add(STR_MENU_MP_LOBBY_USER,     "Users:",                                                      "Mitspieler:");
			L10N.Add(STR_MENU_MP_LOBBY_USER_FMT, "Users: {0}",                                                  "Mitspieler: {0}");
			L10N.Add(STR_MENU_MP_LOBBY_LEVEL,    "Level:",                                                      "Level:");
			L10N.Add(STR_MENU_MP_LOBBY_MUSIC,    "Background music:",                                           "Hintergrundmusik:");
			L10N.Add(STR_MENU_MP_LOBBY_SPEED,    "Game speed:",                                                 "Spielgeschwindigkeit:");
			L10N.Add(STR_MENU_MP_LOBBY_PING,     "Ping",                                                        "Ping");
			L10N.Add(STR_MENU_MP_START,          "Start",                                                       "Start");
			L10N.Add(STR_FRAC_N0,                "Gray",                                                        "Gray");
			L10N.Add(STR_FRAC_P1,                "Green",                                                       "Grün");
			L10N.Add(STR_FRAC_A2,                "Red",                                                         "Rot");
			L10N.Add(STR_FRAC_A3,                "Blue",                                                        "Blau");
			L10N.Add(STR_FRAC_A4,                "Purple",                                                      "Lila");
			L10N.Add(STR_FRAC_A5,                "Orange",                                                      "Orange");
			L10N.Add(STR_FRAC_A6,                "Teal",                                                        "BlauGrün");
			L10N.Add(STR_MENU_MP_LOBBY_COLOR,    "Color",                                                       "Farbe");
			L10N.Add(STR_HSP_NEWGAME,            "New Game",                                                    "Neues Spiel");
			L10N.Add(STR_HSP_RANDOMGAME,         "Random level",                                                "Zufälliges Level");
			L10N.Add(STR_HSP_MPPOINTS,           "Multiplayer score",                                           "Mehrspieler Punkte");

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

				case SAMNetworkConnection.ErrorType.BluetoothInternalError:
					return L10N.T(STR_MP_INTERNAL);

				default:
					SAMLog.Error("L10NI::EnumSwitch_FNEM", "type = "+ type);
					return string.Empty;
			}
		}
	}
}
