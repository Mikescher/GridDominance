using MonoSAMFramework.Portable.Localization;

namespace GridDominance.Shared.Resources
{
	public static class L10NImpl
	{
		public const int STR_SSB_ABOUT          = 00;
		public const int STR_SSB_ACCOUNT        = 01;
		public const int STR_SSB_HIGHSCORE      = 02;
		public const int STR_SSB_MUTE           = 03;
		public const int STR_SSB_EFFECTS        = 04;
		public const int STR_SSB_LANGUAGE       = 69;

		public const int STR_HSP_GLOBALRANKING  = 05;
		public const int STR_HSP_RANKINGFOR     = 06;

		public const int STR_TAB_NAME           = 07;
		public const int STR_TAB_POINTS         = 08;
		public const int STR_TAB_TIME           = 09;
		
		public const int STR_FAP_ACCOUNT        = 10;
		public const int STR_FAP_USERNAME       = 11;
		public const int STR_FAP_SCORE          = 78;
		public const int STR_FAP_CHANGEPW       = 12;
		public const int STR_FAP_LOGOUT         = 93;
		public const int STR_FAP_WARN1          = 94;
		public const int STR_FAP_WARN2          = 95;
		public const int STR_FAP_LOGOUT_SUCESS  = 96;

		public const int STR_CPP_CHANGEPW       = 13;
		public const int STR_CPP_USERNAME       = 14;
		public const int STR_CPP_NEWPW          = 15;
		public const int STR_CPP_CHANGE         = 16;
		public const int STR_CPP_CHANGING       = 17;
		public const int STR_CPP_CHANGED        = 18;
		public const int STR_CPP_COMERR         = 19;
		public const int STR_CPP_AUTHERR        = 20;
		public const int STR_CPP_CHANGEERR      = 21;

		public const int STR_ATTRIBUTIONS       = 22;
		public const int STR_UNLOCK             = 85;

		public const int STR_AAP_HEADER         = 23;
		public const int STR_AAP_USERNAME       = 24;
		public const int STR_AAP_PASSWORD       = 25;
		public const int STR_AAP_CREATEACCOUNT  = 26;
		public const int STR_AAP_LOGIN          = 27;
		public const int STR_AAP_LOGGINGIN      = 28;
		public const int STR_AAP_WRONGPW        = 29;
		public const int STR_AAP_USERNOTFOUND   = 30;
		public const int STR_AAP_NOCOM          = 31;
		public const int STR_AAP_LOGINSUCCESS   = 32;
		public const int STR_AAP_NOLOGIN        = 33;
		public const int STR_AAP_ACCCREATING    = 34;
		public const int STR_AAP_ACCCREATED     = 35;
		public const int STR_AAP_USERTAKEN      = 36;
		public const int STR_AAP_ALREADYCREATED = 37;
		public const int STR_AAP_AUTHERROR      = 38;
		public const int STR_AAP_COULDNOTCREATE = 39;

		public const int STR_PAUS_RESUME        = 40;
		public const int STR_PAUS_RESTART       = 41;
		public const int STR_PAUS_EXIT          = 42;

		public const int STR_HSP_LEVEL          = 43;
		public const int STR_HSP_POINTS         = 44;
		public const int STR_HSP_PROGRESS       = 45;
		public const int STR_HSP_BACK           = 46;
		public const int STR_HSP_NEXT           = 47;
		public const int STR_HSP_AGAIN          = 48;
		public const int STR_HSP_TUTORIAL       = 49;
		public const int STR_HSP_GETSTARTED     = 50;
		public const int STR_HSP_CONERROR       = 65;
		
		public const int STR_DIFF_0             = 51;
		public const int STR_DIFF_1             = 52;
		public const int STR_DIFF_2             = 53;
		public const int STR_DIFF_3             = 54;

		public const int STR_TUT_INFO1          = 79;
		public const int STR_TUT_INFO2          = 55;
		public const int STR_TUT_INFO3          = 56;
		public const int STR_TUT_INFO4          = 57;
		public const int STR_TUT_INFO5          = 58;
		public const int STR_TUT_INFO6          = 59;
		public const int STR_TUT_INFO7          = 60;
		public const int STR_TUT_INFO8          = 61;
		
		public const int STR_API_CONERR         = 62;
		public const int STR_API_COMERR         = 63;

		public const int STR_GLOB_EXITTOAST     = 64;
		public const int STR_GLOB_UNLOCKTOAST1  = 66;
		public const int STR_GLOB_UNLOCKTOAST2  = 67;
		public const int STR_GLOB_UNLOCKTOAST3  = 68;
		public const int STR_GLOB_LEVELLOCK     = 70;
		public const int STR_GLOB_WORLDLOCK     = 71;
		public const int STR_GLOB_OVERWORLD     = 75;
		public const int STR_GLOB_WAITFORSERVER = 76;
		public const int STR_GLOB_UNLOCKSUCCESS = 86;

		public const int STR_WORLD_TUTORIAL     = 77;
		public const int STR_WORLD_W1           = 80;
		public const int STR_WORLD_W2           = 81;
		public const int STR_WORLD_W3           = 92;

		public const int STR_INF_YOU            = 72;
		public const int STR_INF_GLOBAL         = 73;
		public const int STR_INF_HIGHSCORE      = 74;

		public const int STR_IAB_TESTERR        = 82;
		public const int STR_IAB_TESTNOCONN     = 83;
		public const int STR_IAB_TESTINPROGRESS = 84;

		public const int STR_IAB_BUYERR        = 88;
		public const int STR_IAB_BUYNOCONN     = 89;
		public const int STR_IAB_BUYNOTREADY   = 90;
		public const int STR_IAB_BUYSUCESS     = 91;

		public const int STR_PREV_BUYNOW        = 87;

		private const int TEXT_COUNT = 97; // = next idx

		public static void Init(int lang)
		{
			L10N.Init(lang, TEXT_COUNT);

			L10N.Add(STR_SSB_ABOUT,          "About",                                                       "Info"                                                                 );
			L10N.Add(STR_SSB_ACCOUNT,        "Account",                                                     "Benutzerkonto"                                                        );
			L10N.Add(STR_SSB_HIGHSCORE,      "Highscore",                                                   "Bestenliste"                                                          );
			L10N.Add(STR_SSB_MUTE,           "Mute",                                                        "Stumm"                                                                );
			L10N.Add(STR_SSB_EFFECTS,        "Effects",                                                     "Effekte"                                                              );
			L10N.Add(STR_HSP_GLOBALRANKING,  "Global Ranking",                                              "Globale Bestenliste"                                                  );
			L10N.Add(STR_HSP_RANKINGFOR,     "Ranking for \"{0}\"",                                         "Bestenliste für \"{0}\""                                              );
			L10N.Add(STR_TAB_NAME,           "Name",                                                        "Name"                                                                 );
			L10N.Add(STR_TAB_POINTS,         "Points",                                                      "Punkte"                                                               );
			L10N.Add(STR_TAB_TIME,           "Total Time",                                                  "Gesamtzeit"                                                           );
			L10N.Add(STR_FAP_ACCOUNT,        "Account",                                                     "Benutzerkonto"                                                        );
			L10N.Add(STR_FAP_USERNAME,       "Username:",                                                   "Benutzername:"                                                        );
			L10N.Add(STR_FAP_SCORE,          "Points:",                                                     "Punkte:"                                                              );
			L10N.Add(STR_FAP_CHANGEPW,       "Change Password",                                             "Passwort ändern"                                                      );
			L10N.Add(STR_FAP_LOGOUT,         "Logout",                                                      "Ausloggen"                                                            );
			L10N.Add(STR_FAP_WARN1,          "This will clear all local data. Press again to log out.",     "Dies löscht alle lokalen Daten. Nochmal drücken zum ausloggen."       );
			L10N.Add(STR_FAP_WARN2,          "Are you really sure you want to log out?",                    "Wirklich vom Serverkonto abmelden?"                                   );
			L10N.Add(STR_FAP_LOGOUT_SUCESS,  "Logged out from account",                                     "Lokaler Benutzer wurde abgemeldet."                                   );
			L10N.Add(STR_CPP_CHANGEPW,       "Change Password",                                             "Passwort ändern"                                                      );
			L10N.Add(STR_CPP_USERNAME,       "Username:",                                                   "Benutzername:"                                                        );
			L10N.Add(STR_CPP_NEWPW,          "New Password",                                                "Neues Passwort"                                                       );
			L10N.Add(STR_CPP_CHANGE,         "Change",                                                      "Ändern"                                                               );
			L10N.Add(STR_CPP_CHANGING,       "Changing password",                                           "Passwort wird geändert"                                               );
			L10N.Add(STR_CPP_CHANGED,        "Password changed",                                            "Passwort geändert"                                                    );
			L10N.Add(STR_CPP_COMERR,         "Could not communicate with server",                           "Kommunikation mit Server ist gestört"                                 );
			L10N.Add(STR_CPP_AUTHERR,        "Authentication error",                                        "Authentifizierung fehlgeschlagen"                                     );
			L10N.Add(STR_CPP_CHANGEERR,      "Could not change password",                                   "Passwort konnte nicht geändert werden"                                );
			L10N.Add(STR_ATTRIBUTIONS,       "Attributions",                                                "Lizenzen"                                                             );
			L10N.Add(STR_AAP_HEADER,         "Sign up / Log in",                                            "Anmelden / Registrieren"                                              );
			L10N.Add(STR_AAP_USERNAME,       "Username",                                                    "Benutzername"                                                         );
			L10N.Add(STR_AAP_PASSWORD,       "Password",                                                    "Passwort"                                                             );
			L10N.Add(STR_AAP_CREATEACCOUNT,  "Create Account",                                              "Registrieren"                                                         );
			L10N.Add(STR_AAP_LOGIN,          "Login",                                                       "Anmelden"                                                             );
			L10N.Add(STR_AAP_LOGGINGIN,      "Logging in",                                                  "Wird angemeldet"                                                      );
			L10N.Add(STR_AAP_WRONGPW,        "Wrong password",                                              "Falsches Passwort"                                                    );
			L10N.Add(STR_AAP_USERNOTFOUND,   "User not found",                                              "Benutzer nicht gefunden"                                              );
			L10N.Add(STR_AAP_NOCOM,          "Could not communicate with server",                           "Konnte nicht mit Server kommunizieren"                                );
			L10N.Add(STR_AAP_LOGINSUCCESS,   "Successfully logged in",                                      "Benutzer erfolgreich angemeldet"                                      );
			L10N.Add(STR_AAP_NOLOGIN,        "Could not login",                                             "Anmeldung fehlgeschlagen"                                             );
			L10N.Add(STR_AAP_ACCCREATING,    "Creating account",                                            "Konto wird erstellt"                                                  );
			L10N.Add(STR_AAP_ACCCREATED,     "Account created",                                             "Konto erfolgreich erstellt"                                           );
			L10N.Add(STR_AAP_USERTAKEN,      "Username already taken",                                      "Benutzername bereits vergeben"                                        );
			L10N.Add(STR_AAP_ALREADYCREATED, "Account already created",                                     "Konto bereits erstellt"                                               );
			L10N.Add(STR_AAP_AUTHERROR,      "Authentication error",                                        "Authentifizierungsfehler"                                             );
			L10N.Add(STR_AAP_COULDNOTCREATE, "Could not create account",                                    "Konto konnte nicht erstellt werden"                                   );
			L10N.Add(STR_PAUS_RESUME,        "RESUME",                                                      "WEITER"                                                               );
			L10N.Add(STR_PAUS_RESTART,       "RESTART",                                                     "NEU STARTEN"                                                          );
			L10N.Add(STR_PAUS_EXIT,          "EXIT",                                                        "BEENDEN"                                                              );
			L10N.Add(STR_HSP_LEVEL,          "Level",                                                       "Level"                                                                );
			L10N.Add(STR_HSP_POINTS,         "Points",                                                      "Punkte"                                                               );
			L10N.Add(STR_HSP_PROGRESS,       "Progress",                                                    "Fortschritt"                                                          );
			L10N.Add(STR_HSP_BACK,           "Back",                                                        "Zurück"                                                               );
			L10N.Add(STR_HSP_NEXT,           "Next",                                                        "Weiter"                                                               );
			L10N.Add(STR_HSP_AGAIN,          "Again",                                                       "Wiederholen"                                                          );
			L10N.Add(STR_HSP_TUTORIAL,       "Tutorial",                                                    "Tutorial"                                                             );
			L10N.Add(STR_HSP_GETSTARTED,     "Let's get started",                                           "Los gehts"                                                            );
			L10N.Add(STR_HSP_CONERROR,       "Could not connect to highscore server",                       "Kommunikation mit Server fehlgeschlagen"                              );
			L10N.Add(STR_DIFF_0,             "Easy",                                                        "Leicht"                                                               );
			L10N.Add(STR_DIFF_1,             "Normal",                                                      "Normal"                                                               );
			L10N.Add(STR_DIFF_2,             "Hard",                                                        "Schwer"                                                               );
			L10N.Add(STR_DIFF_3,             "Extreme",                                                     "Extrem"                                                               );
			L10N.Add(STR_TUT_INFO1,          "Drag to rotate your own cannons",                             "Drücke und Ziehe um deine Kanonen zu drehen"                          );
			L10N.Add(STR_TUT_INFO2,          "Shoot it until it becomes your cannon",                       "Schieße bis die feindliche Kanone dir gehört"                         );
			L10N.Add(STR_TUT_INFO3,          "Now capture the next cannon",                                 "Erobere nun die nächste Einheit"                                      );
			L10N.Add(STR_TUT_INFO4,          "Keep shooting at the first cannon to increase its fire rate", "Schieß auf deine eigene Kanone um ihre Feuerrate zu erhöhen"          );
			L10N.Add(STR_TUT_INFO5,          "The enemy has captured a cannon. Attack him!",                "Der Gegner hat eine Einheit erobert, greif ihn an!"                   );
			L10N.Add(STR_TUT_INFO6,          "Speed up the Game with the bottom left button.",              "Mit dem Knopf unten links kannst du die Spielgeschwindigkeit erhöhen" );
			L10N.Add(STR_TUT_INFO7,          "Now capture the next cannon",                                 "Erobere jetzt die nächste Einheit"                                    );
			L10N.Add(STR_TUT_INFO8,          "Win the game by capturing all enemy cannons",                 "Gewinne die Schlacht indem du alle Einheiten eroberst"                );
			L10N.Add(STR_API_CONERR,         "Could not connect to highscore server",                       "Verbindung mit Highscore Server fehlgeschlagen"                       );
			L10N.Add(STR_API_COMERR,         "Could not communicate with highscore server",                 "Kommunikation mit Highscore Server fehlgeschlagen"                    );
			L10N.Add(STR_GLOB_EXITTOAST,     "Click again to exit game",                                    "Drücke \"Zurück\" nochmal um das Spiel zu beenden"                    );
			L10N.Add(STR_GLOB_UNLOCKTOAST1,  "Click two times more to unlock",                              "Noch zweimal drücken um die Welt freizuschalten"                      );
			L10N.Add(STR_GLOB_UNLOCKTOAST2,  "Click again to unlock",                                       "Nochmal drücken um die Welt freizuschalten"                           );
			L10N.Add(STR_GLOB_UNLOCKTOAST3,  "World unlocked",                                              "Welt freigeschaltet"                                                  ); 
			L10N.Add(STR_GLOB_WORLDLOCK,     "World locked",                                                "Welt noch nicht freigespielt"                                         );
			L10N.Add(STR_GLOB_LEVELLOCK,     "Level locked",                                                "Level noch nicht freigespielt"                                        );
			L10N.Add(STR_INF_YOU,            "You",                                                         "Du"                                                                   );
			L10N.Add(STR_INF_GLOBAL,         "Global",                                                      "Global"                                                               );
			L10N.Add(STR_INF_HIGHSCORE,      "Highscore",                                                   "Bestzeit"                                                             );
			L10N.Add(STR_GLOB_OVERWORLD,     "Overworld",                                                   "Übersicht"                                                            );
			L10N.Add(STR_GLOB_WAITFORSERVER, "Contacting server",                                           "Server wird kontaktiert"                                              );
			L10N.Add(STR_WORLD_TUTORIAL,     "Tutorial",                                                    "Tutorial"                                                             );
			L10N.Add(STR_SSB_LANGUAGE,       "Language",                                                    "Sprache"                                                              );
			L10N.Add(STR_WORLD_W1,           "Basic",                                                       "Grundlagen"                                                           );
			L10N.Add(STR_WORLD_W2,           "Professional",                                                "Fortgeschritten"                                                      );
			L10N.Add(STR_WORLD_W3,           "Futuristic",                                                  "Futuristisch"                                                         );
			L10N.Add(STR_IAB_TESTERR,        "Error connecting to Google Play services",                    "Fehler beim Versuch mit Google Play zu verbinden"                     );
			L10N.Add(STR_IAB_TESTNOCONN,     "No connection to Google Play services",                       "Keine Verbindung zu Google Play services"                             );
			L10N.Add(STR_IAB_TESTINPROGRESS, "Payment in progress",                                         "Zahlung wird verarbeitet"                                             );
			L10N.Add(STR_UNLOCK,             "Promotion Code",                                              "Freischaltungs Code"                                                  );
			L10N.Add(STR_GLOB_UNLOCKSUCCESS, "Upgraded game to full version!",                              "Spiel wurde zur Vollversion aufgewertet"                              );
			L10N.Add(STR_PREV_BUYNOW,        "Unlock now",                                                  "Jetzt freischalten"                                                   );
			L10N.Add(STR_IAB_BUYERR,         "Error connecting to Google Play services",                    "Fehler beim Versuch mit Google Play zu verbinden"                     );
			L10N.Add(STR_IAB_BUYNOCONN,      "No connection to Google Play services",                       "Keine Verbindung zu Google Play services"                             );
			L10N.Add(STR_IAB_BUYNOTREADY,    "Connection to Google Play services not ready",                "Verbindung zu Google Play services nicht bereit"                      );
			L10N.Add(STR_IAB_BUYSUCESS,      "World sucesfully purchased",                                  "Levelpack wurde erfolgreich erworben"                                 );

#if DEBUG
			L10N.Verify();
#endif
		}
	}
}
