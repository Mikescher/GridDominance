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
		public const int STR_HSP_MULTIPLAYERRANKING = 167;
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
		public const int STR_WORLD_SINGLEPLAYER     = 178;

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
		public const int STR_HINT_009               = 173;
		public const int STR_HINT_010               = 176;
		public const int STR_HINT_011               = 177;

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
		public const int STR_MP_BTADAPTERPERMDENIED = 168;
		public const int STR_MP_BTDISABLED          = 163;
		public const int STR_MP_DIRECTCONNLOST      = 164;
		public const int STR_MP_DIRECTCONNFAIL      = 165;
		public const int STR_MP_TOAST_CONN_TRY      = 170;
		public const int STR_MP_TOAST_CONN_FAIL     = 171;
		public const int STR_MP_TOAST_CONN_SUCC     = 172;

		public const int STR_MENU_CANCEL            = 138;
		public const int STR_MENU_DISCONNECT        = 147;

		public const int STR_MENU_CAP_MULTIPLAYER   = 126;
		public const int STR_MENU_CAP_LOBBY         = 127;
		public const int STR_MENU_CAP_CGAME_PROX    = 128;
		public const int STR_MENU_CAP_CGAME_P2P     = 169;
		public const int STR_MENU_CAP_AUTH          = 136;
		public const int STR_MENU_CAP_SEARCH        = 166;
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

		public const int STR_ENDGAME_1              = 174;
		public const int STR_ENDGAME_2              = 175;

		public const int STR_ACCOUNT_REMINDER       = 179;
		public const int STR_BTN_YES                = 180;
		public const int STR_BTN_NO                 = 181;

		private const int TEXT_COUNT = 182; // = next idx

		public static void Init(int lang)
		{
			L10N.Init(lang, TEXT_COUNT);

			L10N.Add(STR_SSB_ABOUT,              "About",                                                                                                                                   "Info",                                                                                                                                "Info");
			L10N.Add(STR_SSB_ACCOUNT,            "Account",                                                                                                                                 "Benutzerkonto",                                                                                                                       "Compte");
			L10N.Add(STR_SSB_HIGHSCORE,          "Highscore",                                                                                                                               "Bestenliste",                                                                                                                         "Tableau d'honneur");
			L10N.Add(STR_SSB_MUTE,               "Mute",                                                                                                                                    "Stumm",                                                                                                                               "Muet");
			L10N.Add(STR_SSB_EFFECTS,            "Effects",                                                                                                                                 "Effekte",                                                                                                                             "Effet");
			L10N.Add(STR_SSB_MUSIC,              "Music",                                                                                                                                   "Musik",                                                                                                                               "Musique");
			L10N.Add(STR_HSP_GLOBALRANKING,      "Global Ranking",                                                                                                                          "Bestenliste",                                                                                                                         "Classement globale");
			L10N.Add(STR_HSP_MULTIPLAYERRANKING, "Multiplayer",                                                                                                                             "Mehrspieler",                                                                                                                         "Multijoueur");
			L10N.Add(STR_HSP_RANKINGFOR,         "Ranking for \"{0}\"",                                                                                                                     "Bestenliste für \"{0}\"",                                                                                                             "Classement pour \"{0}\"");
			L10N.Add(STR_TAB_NAME,               "Name",                                                                                                                                    "Name",                                                                                                                                "Nom");
			L10N.Add(STR_TAB_POINTS,             "Points",                                                                                                                                  "Punkte",                                                                                                                              "Points");
			L10N.Add(STR_TAB_TIME,               "Total Time",                                                                                                                              "Gesamtzeit",                                                                                                                          "Temps total");
			L10N.Add(STR_FAP_ACCOUNT,            "Account",                                                                                                                                 "Benutzerkonto",                                                                                                                       "Compte d'utilisateur");
			L10N.Add(STR_FAP_USERNAME,           "Username:",                                                                                                                               "Benutzername:",                                                                                                                       "Nom d'utilisateur");
			L10N.Add(STR_FAP_SCORE,              "Points:",                                                                                                                                 "Punkte:",                                                                                                                             "Points");
			L10N.Add(STR_FAP_CHANGEPW,           "Change Password",                                                                                                                         "Passwort ändern",                                                                                                                     "Mot de passe");
			L10N.Add(STR_FAP_LOGOUT,             "Logout",                                                                                                                                  "Ausloggen",                                                                                                                           "Déconnecter");
			L10N.Add(STR_FAP_WARN1,              "This will clear all local data. Press again to log out.",                                                                                 "Dies löscht alle lokalen Daten. Nochmal drücken zum ausloggen.",                                                                      "Cette opération efface toutes les données locales. Appuyez à nouveau pour vous déconnecter.");
			L10N.Add(STR_FAP_WARN2,              "Are you really sure you want to log out?",                                                                                                "Wirklich vom Serverkonto abmelden?",                                                                                                  "Êtes-vous vraiment sûr de vouloir vous déconnecter?");
			L10N.Add(STR_FAP_LOGOUT_SUCESS,      "Logged out from account",                                                                                                                 "Lokaler Benutzer wurde abgemeldet.",                                                                                                  "Déconnecté du compte");
			L10N.Add(STR_CPP_CHANGEPW,           "Change Password",                                                                                                                         "Passwort ändern",                                                                                                                     "Changer mot de passe");
			L10N.Add(STR_CPP_USERNAME,           "Username:",                                                                                                                               "Benutzername:",                                                                                                                       "Nom d'utilisateur");
			L10N.Add(STR_CPP_NEWPW,              "New Password",                                                                                                                            "Neues Passwort",                                                                                                                      "Noveau mot de passe");
			L10N.Add(STR_CPP_CHANGE,             "Change",                                                                                                                                  "Ändern",                                                                                                                              "Changer mot de passe");
			L10N.Add(STR_CPP_CHANGING,           "Changing password",                                                                                                                       "Passwort wird geändert",                                                                                                              "Changement du mot de passe ");
			L10N.Add(STR_CPP_CHANGED,            "Password changed",                                                                                                                        "Passwort geändert",                                                                                                                   "Mot de passe est changé");
			L10N.Add(STR_CPP_COMERR,             "Could not communicate with server",                                                                                                       "Kommunikation mit Server ist gestört",                                                                                                "La communication avec le serveur est perturbé");
			L10N.Add(STR_CPP_AUTHERR,            "Authentication error",                                                                                                                    "Authentifizierung fehlgeschlagen",                                                                                                    "Erreur d'authentification");
			L10N.Add(STR_CPP_CHANGEERR,          "Could not change password",                                                                                                               "Passwort konnte nicht geändert werden",                                                                                               "Mot de passe ne peut pas être modifié");
			L10N.Add(STR_ATTRIBUTIONS,           "Attributions",                                                                                                                            "Lizenzen",                                                                                                                            "Licences");
			L10N.Add(STR_AAP_HEADER,             "Sign up / Log in",                                                                                                                        "Anmelden / Registrieren",                                                                                                             "Se connecter");
			L10N.Add(STR_AAP_USERNAME,           "Username",                                                                                                                                "Benutzername",                                                                                                                        "Nom d'utilisateur");
			L10N.Add(STR_AAP_PASSWORD,           "Password",                                                                                                                                "Passwort",                                                                                                                            "Mot de passe");
			L10N.Add(STR_AAP_CREATEACCOUNT,      "Create Account",                                                                                                                          "Registrieren",                                                                                                                        "Registrer");
			L10N.Add(STR_AAP_LOGIN,              "Login",                                                                                                                                   "Anmelden",                                                                                                                            "S'inscrire");
			L10N.Add(STR_AAP_LOGGINGIN,          "Logging in",                                                                                                                              "Wird angemeldet",                                                                                                                     "Est enregistré");
			L10N.Add(STR_AAP_WRONGPW,            "Wrong password",                                                                                                                          "Falsches Passwort",                                                                                                                   "Mot de passe incorrect");
			L10N.Add(STR_AAP_USERNOTFOUND,       "User not found",                                                                                                                          "Benutzer nicht gefunden",                                                                                                             "Utilisateur introuvable");
			L10N.Add(STR_AAP_NOCOM,              "Could not communicate with server",                                                                                                       "Konnte nicht mit Server kommunizieren",                                                                                               "La communication avec le serveur est perturbé");
			L10N.Add(STR_AAP_LOGINSUCCESS,       "Successfully logged in",                                                                                                                  "Benutzer erfolgreich angemeldet",                                                                                                     "Connecté avec succès");
			L10N.Add(STR_AAP_NOLOGIN,            "Could not login",                                                                                                                         "Anmeldung fehlgeschlagen",                                                                                                            "Échec de la connexion");
			L10N.Add(STR_AAP_ACCCREATING,        "Creating account",                                                                                                                        "Konto wird erstellt",                                                                                                                 "Création du compte");
			L10N.Add(STR_AAP_ACCCREATED,         "Account created",                                                                                                                         "Konto erfolgreich erstellt",                                                                                                          "Compte créé");
			L10N.Add(STR_AAP_USERTAKEN,          "Username already taken",                                                                                                                  "Benutzername bereits vergeben",                                                                                                       "Nom d'utilisateur déjà pris");
			L10N.Add(STR_AAP_ALREADYCREATED,     "Account already created",                                                                                                                 "Konto bereits erstellt",                                                                                                              "Compte déjà créé");
			L10N.Add(STR_AAP_AUTHERROR,          "Authentication error",                                                                                                                    "Authentifizierungsfehler",                                                                                                            "Erreur d'authentification");
			L10N.Add(STR_AAP_COULDNOTCREATE,     "Could not create account",                                                                                                                "Konto konnte nicht erstellt werden",                                                                                                  "Impossible de créer compte");
			L10N.Add(STR_PAUS_RESUME,            "RESUME",                                                                                                                                  "WEITER",                                                                                                                              "REPRENDRE");
			L10N.Add(STR_PAUS_RESTART,           "RESTART",                                                                                                                                 "NEU STARTEN",                                                                                                                         "REDÉMARRER");
			L10N.Add(STR_PAUS_EXIT,              "EXIT",                                                                                                                                    "BEENDEN",                                                                                                                             "TERMINER");
			L10N.Add(STR_HSP_LEVEL,              "Level",                                                                                                                                   "Level",                                                                                                                               "Niveau");
			L10N.Add(STR_HSP_POINTS,             "Points",                                                                                                                                  "Punkte",                                                                                                                              "Ponts");
			L10N.Add(STR_HSP_PROGRESS,           "Progress",                                                                                                                                "Fortschritt",                                                                                                                         "Progrès");
			L10N.Add(STR_HSP_BACK,               "Back",                                                                                                                                    "Zurück",                                                                                                                              "De retour");
			L10N.Add(STR_HSP_NEXT,               "Next",                                                                                                                                    "Weiter",                                                                                                                              "Prochain");
			L10N.Add(STR_HSP_AGAIN,              "Again",                                                                                                                                   "Wiederholen",                                                                                                                         "Répéter");
			L10N.Add(STR_HSP_TUTORIAL,           "Tutorial",                                                                                                                                "Tutorial",                                                                                                                            "Tutorial");
			L10N.Add(STR_HSP_GETSTARTED,         "Let's get started",                                                                                                                       "Los gehts",                                                                                                                           "C'est parti");
			L10N.Add(STR_HSP_CONERROR,           "Could not connect to highscore server",                                                                                                   "Kommunikation mit Server fehlgeschlagen",                                                                                             "Impossible de se connecter au serveur highscore");
			L10N.Add(STR_DIFF_0,                 "Easy",                                                                                                                                    "Leicht",                                                                                                                              "Facile");
			L10N.Add(STR_DIFF_1,                 "Normal",                                                                                                                                  "Normal",                                                                                                                              "Normal");
			L10N.Add(STR_DIFF_2,                 "Hard",                                                                                                                                    "Schwer",                                                                                                                              "Difficile");
			L10N.Add(STR_DIFF_3,                 "Extreme",                                                                                                                                 "Extrem",                                                                                                                              "Extrême");
			L10N.Add(STR_TUT_INFO1,              "Drag to rotate your own cannons",                                                                                                         "Drücke und Ziehe um deine Kanonen zu drehen",                                                                                         "Tirer pour tourner tes canons");
			L10N.Add(STR_TUT_INFO2,              "Shoot it until it becomes your cannon",                                                                                                   "Schieße bis die feindliche Kanone dir gehört",                                                                                        "Abattre jusque le canon ennemi est à toi");
			L10N.Add(STR_TUT_INFO3,              "Now capture the next cannon",                                                                                                             "Erobere nun die nächste Einheit",                                                                                                     "Captiver le prochain canon");
			L10N.Add(STR_TUT_INFO4,              "Keep shooting at the first cannon to increase its fire rate",                                                                             "Schieß auf deine eigene Kanone um ihre Feuerrate zu erhöhen",                                                                         "Gardez le tir au premier canon pour augmenter sa cadence de tir");
			L10N.Add(STR_TUT_INFO5,              "The enemy has captured a cannon. Attack him!",                                                                                            "Der Gegner hat eine Einheit erobert, greif ihn an!",                                                                                  "L'enemi a conquis  un canon. Attaque.");
			L10N.Add(STR_TUT_INFO6,              "Speed up the Game with the bottom left button.",                                                                                          "Mit dem Knopf unten links kannst du die Spielgeschwindigkeit erhöhen",                                                                "Accélérez le jeu avec le bouton en bas à gauche.");
			L10N.Add(STR_TUT_INFO7,              "Now capture the next cannon",                                                                                                             "Erobere jetzt die nächste Einheit",                                                                                                   "Maintenant capturez le prochain canon");
			L10N.Add(STR_TUT_INFO8,              "Win the game by capturing all enemy cannons",                                                                                             "Gewinne die Schlacht indem du alle Einheiten eroberst",                                                                               "Gagnez le jeu en capturant tous les canons ennemis");
			L10N.Add(STR_API_CONERR,             "Could not connect to highscore server",                                                                                                   "Verbindung mit Highscore Server fehlgeschlagen",                                                                                      "Impossible de se connecter au serveur Highscore");
			L10N.Add(STR_API_COMERR,             "Could not communicate with highscore server",                                                                                             "Kommunikation mit Highscore Server fehlgeschlagen",                                                                                   "Impossible de communiquer avec le serveur highscore");
			L10N.Add(STR_GLOB_EXITTOAST,         "Click again to exit game",                                                                                                                "Drücke nochmal \"Zurück\" um das Spiel zu beenden",                                                                                   "Cliquez de noouveau pour quitter le jeu");
			L10N.Add(STR_GLOB_UNLOCKTOAST1,      "Click two times more to unlock",                                                                                                          "Noch zweimal drücken um die Welt freizuschalten",                                                                                     "Cliquez deux fois plus pour débloquer");
			L10N.Add(STR_GLOB_UNLOCKTOAST2,      "Click again to unlock",                                                                                                                   "Nochmal drücken um die Welt freizuschalten",                                                                                          "Cliquez de nouvea pour quitter le jeu");
			L10N.Add(STR_GLOB_UNLOCKTOAST3,      "World unlocked",                                                                                                                          "Welt freigeschaltet",                                                                                                                 "Monde débloqué");
			L10N.Add(STR_GLOB_WORLDLOCK,         "World locked",                                                                                                                            "Welt noch nicht freigespielt",                                                                                                        "Monde bloqué");
			L10N.Add(STR_GLOB_LEVELLOCK,         "Level locked",                                                                                                                            "Level noch nicht freigespielt",                                                                                                       "Niveau bloqué");
			L10N.Add(STR_INF_YOU,                "You",                                                                                                                                     "Du",                                                                                                                                  "Toi");
			L10N.Add(STR_INF_GLOBAL,             "Stats",                                                                                                                                   "Total",                                                                                                                               "Global");
			L10N.Add(STR_INF_HIGHSCORE,          "Highscore",                                                                                                                               "Bestzeit",                                                                                                                            "Highscore");
			L10N.Add(STR_GLOB_OVERWORLD,         "Overworld",                                                                                                                               "Übersicht",                                                                                                                           "L'aperçu");
			L10N.Add(STR_GLOB_WAITFORSERVER,     "Contacting server",                                                                                                                       "Server wird kontaktiert",                                                                                                             "Contacter le serveur");
			L10N.Add(STR_WORLD_TUTORIAL,         "Tutorial",                                                                                                                                "Tutorial",                                                                                                                            "Didacticiel");
			L10N.Add(STR_SSB_LANGUAGE,           "Language",                                                                                                                                "Sprache",                                                                                                                             "Longue");
			L10N.Add(STR_WORLD_W1,               "Basic",                                                                                                                                   "Grundlagen",                                                                                                                          "Niveau base");
			L10N.Add(STR_WORLD_W2,               "Professional",                                                                                                                            "Fortgeschritten",                                                                                                                     "Niveau avancé");
			L10N.Add(STR_WORLD_W3,               "Futuristic",                                                                                                                              "Futuristisch",                                                                                                                        "Futuriste");
			L10N.Add(STR_WORLD_W4,               "Toy Box",                                                                                                                                 "Spielzeugkiste",                                                                                                                      "Coffre à jouets");
			L10N.Add(STR_WORLD_MULTIPLAYER,      "Multiplayer",                                                                                                                             "Mehrspieler",                                                                                                                         "Multijoueur");
			L10N.Add(STR_WORLD_SINGLEPLAYER,     "Singleplayer",                                                                                                                            "Einzelspieler",                                                                                                                       "Seul joueur");
			L10N.Add(STR_IAB_TESTERR,            "Error connecting to Google Play services",                                                                                                "Fehler beim Versuch mit Google Play zu verbinden",                                                                                    "Erreurde connexion avec Google Play services");
			L10N.Add(STR_IAB_TESTNOCONN,         "No connection to Google Play services",                                                                                                   "Keine Verbindung zu Google Play services",                                                                                            "Pas de connexion avec Google Play services");
			L10N.Add(STR_IAB_TESTINPROGRESS,     "Payment in progress",                                                                                                                     "Zahlung wird verarbeitet",                                                                                                            "Paiement en cours");
			L10N.Add(STR_UNLOCK,                 "Promotion Code",                                                                                                                          "Promo Code",                                                                                                                          "Code promotionnel");
			L10N.Add(STR_GLOB_UNLOCKSUCCESS,     "Upgraded game to full version!",                                                                                                          "Spiel wurde zur Vollversion aufgewertet",                                                                                             "Mise à niveau du jeu en version complète!");
			L10N.Add(STR_PREV_BUYNOW,            "Unlock now",                                                                                                                              "Jetzt freischalten",                                                                                                                  "Débloquer maintenant");
			L10N.Add(STR_IAB_BUYERR,             "Error connecting to Google Play services",                                                                                                "Fehler beim Versuch mit Google Play zu verbinden",                                                                                    "Erreurde connexion avec Google Play services");
			L10N.Add(STR_IAB_BUYNOCONN,          "No connection to Google Play services",                                                                                                   "Keine Verbindung zu Google Play services",                                                                                            "Pas de connexion avec Google Play services");
			L10N.Add(STR_IAB_BUYNOTREADY,        "Connection to Google Play services not ready",                                                                                            "Verbindung zu Google Play services nicht bereit",                                                                                     "La connexion aux services Google Play n'est pas prête");
			L10N.Add(STR_IAB_BUYSUCESS,          "World successfully purchased",                                                                                                            "Levelpack wurde erfolgreich erworben",                                                                                                "Le monde a acheté avec succès");
			L10N.Add(STR_HINT_001,               "Tip: Shoot stuff to win!",                                                                                                                "Tipp: Versuche auf die andere Kanone zu schiessen",                                                                                   "Allusion: tirez des trucs pour gagner!");
			L10N.Add(STR_HINT_002,               "Bigger Cannon",                                                                                                                           "Größere Kanone",                                                                                                                      "Plus grands canons");
			L10N.Add(STR_HINT_003,               "More Power",                                                                                                                              "Mehr Schaden",                                                                                                                        "Plus d'énergie");
			L10N.Add(STR_HINT_004,               "Black holes attract your bullets",                                                                                                        "Schwarze Löcher saugen deine Kugeln ein",                                                                                             "Les trous noirs attirent vos balles");
			L10N.Add(STR_HINT_005,               "Lasers!",                                                                                                                                 "Laser!",                                                                                                                              "Lasers!");
			L10N.Add(STR_HINT_006,               "Try dragging the map around",                                                                                                             "Versuche mal die Karte zu verschieben",                                                                                               "Essayez de faire glisser la carte autour");
			L10N.Add(STR_HINT_007,               "Speedy thing goes in,",                                                                                                                   "Speedy thing goes in,",                                                                                                               "Speedy thing goes in,");
			L10N.Add(STR_HINT_008,               "speedy thing comes out.",                                                                                                                 "speedy thing comes out.",                                                                                                             "speedy thing comes out.");
			L10N.Add(STR_HINT_009,               "Some cannons only relay",                                                                                                                 "Manche Kanonen leiten nur weiter",                                                                                                    "Certains canons relèvent");
			L10N.Add(STR_HINT_010,               "Shields can",                                                                                                                             "Schilde können",                                                                                                                      "Les écus peuvent");
			L10N.Add(STR_HINT_011,               "protect you",                                                                                                                             "dich beschützen",                                                                                                                     "protégez-vous");
			L10N.Add(STR_INFOTOAST_1,            "Your best time is {0}",                                                                                                                   "Deine Bestzeit ist {0}",                                                                                                              "Votre meilleur temps est {0}");
			L10N.Add(STR_INFOTOAST_2,            "The global best time is {0}",                                                                                                             "Die globale Bestzeit ist {0}",                                                                                                        "Le meilleur temps global est {0}");
			L10N.Add(STR_INFOTOAST_3,            "{0} users have completed this level on {1}",                                                                                              "{0} Spieler haben dieses Level auf {1} geschafft",                                                                                    "{0} utilisateurs ont complété ce niveau sur {1}");
			L10N.Add(STR_INFOTOAST_4,            "You have not completed this level on {0}",                                                                                                "Du hast dieses Level auf {0} noch nicht geschafft",                                                                                   "Vous n'avez pas terminé ce niveau sur {0}");
			L10N.Add(STR_PREV_FINISHWORLD,       "Finish World {0}",                                                                                                                        "Welt {0}",                                                                                                                            "Terminer Monde {0}");
			L10N.Add(STR_PREV_OR,                "OR",                                                                                                                                      "ODER",                                                                                                                                "OU");
			L10N.Add(STR_PREV_MISS_TOAST,        "You are missing {0} points to unlock world {1}",                                                                                          "Dir fehlen noch {0} Punkte um Welt {1} freizuschalten",                                                                               "Vous manquez de {0} points pour débloquer le monde {1}");
			L10N.Add(STR_MP_TIMEOUT,             "Timeout - Connection to server lost",                                                                                                     "Timeout - Verbindung zu server verloren",                                                                                             "Timeout - Connexion au serveur perdu");
			L10N.Add(STR_MP_TIMEOUT_USER,        "Timeout - Connection to user [{0}] lost",                                                                                                 "Timeout - Verbindung zu Spieler [{0}] verloren",                                                                                      "Timeout - Connexion à l'utilisateur [{0}] perdu");
			L10N.Add(STR_MP_NOTINLOBBY,          "You a not part of this session",                                                                                                          "Du bist kein Teilnehmer dieser Sitzung",                                                                                              "Vous ne faites pas partie de cette session");
			L10N.Add(STR_MP_SESSIONNOTFOUND,     "Session on server not found",                                                                                                             "Sitzung konnte auf dem Server nicht gefunden werden",                                                                                 "Session sur le serveur pas trouvé");
			L10N.Add(STR_MP_AUTHFAILED,          "Authentification on server failed",                                                                                                       "Authentifizierung auf Server fehlgeschlagen",                                                                                         "L'authentification sur serveur a échoué");
			L10N.Add(STR_MP_LOBBYFULL,           "Server lobby is full",                                                                                                                    "Serverlobby ist voll",                                                                                                                "Le lobby du serveur est plein");
			L10N.Add(STR_MP_VERSIONMISMATCH,     "Server has a different game version ({0})",                                                                                               "Serverversion unterscheidet sich von lokaler Version ({0})",                                                                          "Le serveur a une version de jeu différente({0})");
			L10N.Add(STR_MP_LEVELNOTFOUND,       "Could not find server level locally",                                                                                                     "Level konnte lokal nicht gefunden werden",                                                                                            "Impossible de trouver le niveau de serveur localement");
			L10N.Add(STR_MP_LEVELMISMATCH,       "Server has different version of level",                                                                                                   "Level auf dem Server unterscheidet sich von lokaler Version",                                                                         "Le serveur a une version de niveau différente");
			L10N.Add(STR_MP_USERDISCONNECT,      "User {0} has disconnected",                                                                                                               "Der Benutzer {0} hat die Verbindung getrennt",                                                                                        "L'utilisateur {0} s'est déconnecté");
			L10N.Add(STR_MP_SERVERDISCONNECT,    "Server has closed this session",                                                                                                          "Spiel wurde vom Server geschlossen",                                                                                                  "Le serveur a fermé cette session");
			L10N.Add(STR_MP_INTERNAL,            "Internal multiplayer error",                                                                                                              "Interner Fehler im Mehrspielermodul",                                                                                                 "Error interal au module multiplayer");
			L10N.Add(STR_MP_BTADAPTERNULL,       "No bluetooth hardware found",                                                                                                             "Bluetooth Hardware nicht gefunden",                                                                                                   "Bluetooth n'a pas été trouvé");
			L10N.Add(STR_MP_BTADAPTERPERMDENIED, "Missing bluetooth permission",                                                                                                            "Bluetooth Berechtigung wurde nicht gewährt",                                                                                          "Absence d'autorisation de bluetooth");
			L10N.Add(STR_MP_BTDISABLED,          "Bluetooth is disabled",                                                                                                                   "Bluetooth ist deaktiviert",                                                                                                           "Connexion Bluetooth deactivé");
			L10N.Add(STR_MP_DIRECTCONNFAIL,      "Bluetooth connection failed",                                                                                                             "Bluetooth Verbindungsaufbau fehlgeschlagen",                                                                                          "Connexion Bluetooth échoué");
			L10N.Add(STR_MP_DIRECTCONNLOST,      "Bluetooth connection lost",                                                                                                               "Bluetooth Verbindung verloren",                                                                                                       "Connexion Bluetooth perdu");
			L10N.Add(STR_MENU_CAP_MULTIPLAYER,   "Multiplayer",                                                                                                                             "Mehrspieler",                                                                                                                         "Multijoueur");
			L10N.Add(STR_MENU_CAP_LOBBY,         "Multiplayer Lobby",                                                                                                                       "Lobby",                                                                                                                               "Online Lobby");
			L10N.Add(STR_MENU_CAP_CGAME_PROX,    "Create Online Game",                                                                                                                      "Onlinespiel erstellen",                                                                                                               "Creer un jeu en ligne");
			L10N.Add(STR_MENU_CAP_CGAME_P2P,     "Create Local Game",                                                                                                                       "Lokales Spiel erstellen",                                                                                                             "Creer un jeu local");
			L10N.Add(STR_MENU_CAP_SEARCH,        "Search for local devices",                                                                                                                "Suche nach lokalem Spiel",                                                                                                            "Cherchez des périphériques locaux");
			L10N.Add(STR_MP_ONLINE,              "Online",                                                                                                                                  "Online",                                                                                                                              "En ligne");
			L10N.Add(STR_MP_OFFLINE,             "Offline",                                                                                                                                 "Offline",                                                                                                                             "Hors ligne");
			L10N.Add(STR_MP_CONNECTING,          "Connecting",                                                                                                                              "Verbinden",                                                                                                                           "Connecter");
			L10N.Add(STR_MENU_MP_JOIN,           "Join",                                                                                                                                    "Beitreten",                                                                                                                           "Joindre");
			L10N.Add(STR_MENU_MP_HOST,           "Host",                                                                                                                                    "Erstellen",                                                                                                                           "Rédiger");
			L10N.Add(STR_MENU_MP_CREATE,         "Create",                                                                                                                                  "Start",                                                                                                                               "Créer");
			L10N.Add(STR_MENU_CANCEL,            "Cancel",                                                                                                                                  "Abbrechen",                                                                                                                           "Abandonner");
			L10N.Add(STR_MENU_DISCONNECT,        "Disconnect",                                                                                                                              "Verbindung trennen",                                                                                                                  "Déconnecter");
			L10N.Add(STR_MENU_MP_LOCAL,          "Local (Bluetooth)",                                                                                                                       "Lokal (Bluetooth)",                                                                                                                   "Local (Bluetooth)");
			L10N.Add(STR_MENU_MP_ONLINE,         "Online (UDP/IP)",                                                                                                                         "Internet (UDP/IP)",                                                                                                                   "En ligne (UDP/IP)");
			L10N.Add(STR_MENU_CAP_AUTH,          "Enter lobby code",                                                                                                                        "Lobby Code eingeben",                                                                                                                 "Entrer lobby code");
			L10N.Add(STR_MENU_MP_GAMESPEED,      "Game speed:",                                                                                                                             "Spielgeschwindigkeit:",                                                                                                               "La vitesse du jeux");
			L10N.Add(STR_MENU_MP_MUSIC,          "Background music:",                                                                                                                       "Hintergrundmusik:",                                                                                                                   "Musique de fond");
			L10N.Add(STR_MENU_MP_LOBBYINFO,      "Enter this code on another phone to join this session.",                                                                                  "Gib diesen Code auf einem anderen Smartphone ein, um diesem Spiel beizutreten",                                                       "Entrez ce code sur un autre smartphone pour rejoindre ce jeu");
			L10N.Add(STR_MENU_MP_LOBBY_USER,     "Users:",                                                                                                                                  "Mitspieler:",                                                                                                                         "Coéquipier:");
			L10N.Add(STR_MENU_MP_LOBBY_USER_FMT, "Users: {0}",                                                                                                                              "Mitspieler: {0}",                                                                                                                     "Coéquipier: {0}");
			L10N.Add(STR_MENU_MP_LOBBY_LEVEL,    "Level:",                                                                                                                                  "Level:",                                                                                                                              "Level:");
			L10N.Add(STR_MENU_MP_LOBBY_MUSIC,    "Background music:",                                                                                                                       "Hintergrundmusik:",                                                                                                                   "Musique de fond:");
			L10N.Add(STR_MENU_MP_LOBBY_SPEED,    "Game speed:",                                                                                                                             "Spielgeschwindigkeit:",                                                                                                               "La vitesse de jeu");
			L10N.Add(STR_MENU_MP_LOBBY_PING,     "Ping",                                                                                                                                    "Ping",                                                                                                                                "Ping");
			L10N.Add(STR_MENU_MP_START,          "Start",                                                                                                                                   "Start",                                                                                                                               "Démarrage");
			L10N.Add(STR_FRAC_N0,                "Gray",                                                                                                                                    "Gray",                                                                                                                                "Gris");
			L10N.Add(STR_FRAC_P1,                "Green",                                                                                                                                   "Grün",                                                                                                                                "Vert");
			L10N.Add(STR_FRAC_A2,                "Red",                                                                                                                                     "Rot",                                                                                                                                 "Rouge");
			L10N.Add(STR_FRAC_A3,                "Blue",                                                                                                                                    "Blau",                                                                                                                                "Bleu");
			L10N.Add(STR_FRAC_A4,                "Purple",                                                                                                                                  "Lila",                                                                                                                                "Violet");
			L10N.Add(STR_FRAC_A5,                "Orange",                                                                                                                                  "Orange",                                                                                                                              "Orange");
			L10N.Add(STR_FRAC_A6,                "Teal",                                                                                                                                    "BlauGrün",                                                                                                                            "Vert bleu");
			L10N.Add(STR_MENU_MP_LOBBY_COLOR,    "Color",                                                                                                                                   "Farbe",                                                                                                                               "Couleur");
			L10N.Add(STR_HSP_NEWGAME,            "New Game",                                                                                                                                "Neues Spiel",                                                                                                                         "Nouveau jeu");
			L10N.Add(STR_HSP_RANDOMGAME,         "Random level",                                                                                                                            "Zufälliges Level",                                                                                                                    "Niveau aléatoire");
			L10N.Add(STR_HSP_MPPOINTS,           "Multiplayer score",                                                                                                                       "Mehrspieler Punkte",                                                                                                                  "Multiplayer score");
			L10N.Add(STR_MP_TOAST_CONN_TRY,      "Connecting to '{0}'",                                                                                                                     "Verbinden mit '{0}'",                                                                                                                 "Connecter à '{0}'");
			L10N.Add(STR_MP_TOAST_CONN_FAIL,     "Connection to '{0}' failed",                                                                                                              "Verbindung mit '{0}' fehlgeschlagen",                                                                                                 "Connexion avec '{0}' est échoué");
			L10N.Add(STR_MP_TOAST_CONN_SUCC,     "Connected to '{0}'",                                                                                                                      "Verbunden mit '{0}'",                                                                                                                 "Connecté avec '{0}'");
			L10N.Add(STR_ENDGAME_1,              "THANKS FOR",                                                                                                                              "THANKS FOR",                                                                                                                          "THANKS FOR");
			L10N.Add(STR_ENDGAME_2,              "PLAYING",                                                                                                                                 "PLAYING",                                                                                                                             "PLAYING");
			L10N.Add(STR_ACCOUNT_REMINDER,       "You can create an account to display your name in the highscore and to backup your score online.\nDo you want to create an account now?", "Du kannst einen Onlineaccount anlegen um deinen Namen im Highscore zu sehen und deine Punkte zu sichern.\n Account jetzt erstellen?", "Vous pouvez créer un compte pour afficher votre nom dans les meilleurs scores et sauvegarder vos points en ligne.\nVoulez - vous créer un compte maintenant?");
			L10N.Add(STR_BTN_YES,                "Yes",                                                                                                                                     "OK",                                                                                                                                  "OK");
			L10N.Add(STR_BTN_NO,                 "No",                                                                                                                                      "Nein",                                                                                                                                "Aucun");

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

				case SAMNetworkConnection.ErrorType.BluetoothInternalError:
					return L10N.T(STR_MP_INTERNAL);

				case SAMNetworkConnection.ErrorType.BluetoothNotEnabled:
					return L10N.T(STR_MP_BTDISABLED);

				case SAMNetworkConnection.ErrorType.P2PConnectionFailed:
					return L10N.T(STR_MP_DIRECTCONNFAIL);

				case SAMNetworkConnection.ErrorType.P2PConnectionLost:
					return L10N.T(STR_MP_DIRECTCONNLOST);

				default:
					SAMLog.Error("L10NI::EnumSwitch_FNEM", "type = "+ type);
					return string.Empty;
			}
		}
	}
}
