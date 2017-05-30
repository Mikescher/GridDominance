using MonoSAMFramework.Portable.Localization;

namespace GridDominance.Shared.Resources
{
	public static class L10NImpl
	{
		public const int STR_SSB_ABOUT          = 100;
		public const int STR_SSB_ACCOUNT        = 101;
		public const int STR_SSB_HIGHSCORE      = 102;
		public const int STR_SSB_MUTE           = 103;
		public const int STR_SSB_EFFECTS        = 104;

		public const int STR_HSP_GLOBALRANKING  = 105;
		public const int STR_HSP_RANKINGFOR     = 106;

		public const int STR_TAB_NAME           = 107;
		public const int STR_TAB_POINTS         = 108;
		public const int STR_TAB_TIME           = 109;
		
		public const int STR_FAP_ACCOUNT        = 110;
		public const int STR_FAP_USERNAME       = 111;
		public const int STR_FAP_SCORE          = 178;
		public const int STR_FAP_CHANGEPW       = 112;

		public const int STR_CPP_CHANGEPW       = 113;
		public const int STR_CPP_USERNAME       = 114;
		public const int STR_CPP_NEWPW          = 115;
		public const int STR_CPP_CHANGE         = 116;
		public const int STR_CPP_CHANGING       = 117;
		public const int STR_CPP_CHANGED        = 118;
		public const int STR_CPP_COMERR         = 119;
		public const int STR_CPP_AUTHERR        = 120;
		public const int STR_CPP_CHANGEERR      = 121;

		public const int STR_ATTRIBUTIONS       = 122;

		public const int STR_AAP_HEADER         = 123;
		public const int STR_AAP_USERNAME       = 124;
		public const int STR_AAP_PASSWORD       = 125;
		public const int STR_AAP_CREATEACCOUNT  = 126;
		public const int STR_AAP_LOGIN          = 127;
		public const int STR_AAP_LOGGINGIN      = 128;
		public const int STR_AAP_WRONGPW        = 129;
		public const int STR_AAP_USERNOTFOUND   = 130;
		public const int STR_AAP_NOCOM          = 131;
		public const int STR_AAP_LOGINSUCCESS   = 132;
		public const int STR_AAP_NOLOGIN        = 133;
		public const int STR_AAP_ACCCREATING    = 134;
		public const int STR_AAP_ACCCREATED     = 135;
		public const int STR_AAP_USERTAKEN      = 136;
		public const int STR_AAP_ALREADYCREATED = 137;
		public const int STR_AAP_AUTHERROR      = 138;
		public const int STR_AAP_COULDNOTCREATE = 139;

		public const int STR_PAUS_RESUME        = 140;
		public const int STR_PAUS_RESTART       = 141;
		public const int STR_PAUS_EXIT          = 142;

		public const int STR_HSP_LEVEL          = 143;
		public const int STR_HSP_POINTS         = 144;
		public const int STR_HSP_PROGRESS       = 145;
		public const int STR_HSP_BACK           = 146;
		public const int STR_HSP_NEXT           = 147;
		public const int STR_HSP_AGAIN          = 148;
		public const int STR_HSP_TUTORIAL       = 149;
		public const int STR_HSP_GETSTARTED     = 150;
		public const int STR_HSP_CONERROR       = 165;
		
		public const int STR_DIFF_0             = 151;
		public const int STR_DIFF_1             = 152;
		public const int STR_DIFF_2             = 153;
		public const int STR_DIFF_3             = 154;

		public const int STR_TUT_INFO1          = 179;
		public const int STR_TUT_INFO2          = 155;
		public const int STR_TUT_INFO3          = 156;
		public const int STR_TUT_INFO4          = 157;
		public const int STR_TUT_INFO5          = 158;
		public const int STR_TUT_INFO6          = 159;
		public const int STR_TUT_INFO7          = 160;
		public const int STR_TUT_INFO8          = 161;
		
		public const int STR_API_CONERR         = 162;
		public const int STR_API_COMERR         = 163;

		public const int STR_GLOB_EXITTOAST     = 164;
		public const int STR_GLOB_UNLOCKTOAST1  = 166;
		public const int STR_GLOB_UNLOCKTOAST2  = 167;
		public const int STR_GLOB_UNLOCKTOAST3  = 168;
		public const int STR_GLOB_LEVELLOCK     = 170;
		public const int STR_GLOB_WORLDLOCK     = 171;
		public const int STR_GLOB_OVERWORLD     = 175;
		public const int STR_GLOB_WAITFORSERVER = 176;
		public const int STR_GLOB_TUTORIAL      = 177;

		public const int STR_INF_YOU            = 172;
		public const int STR_INF_GLOBAL         = 173;
		public const int STR_INF_HIGHSCORE      = 174;

		public static void Init()
		{
			L10N.LANGUAGE = L10N.LANG_DE_DE;

			L10N.Dictionary.Add(STR_SSB_ABOUT,          new[] { "About",                                                       "Info"                                                                 });
			L10N.Dictionary.Add(STR_SSB_ACCOUNT,        new[] { "Account",                                                     "Benutzerkonto"                                                        });
			L10N.Dictionary.Add(STR_SSB_HIGHSCORE,      new[] { "Highscore",                                                   "Bestenliste"                                                          });
			L10N.Dictionary.Add(STR_SSB_MUTE,           new[] { "Mute",                                                        "Stumm"                                                                });
			L10N.Dictionary.Add(STR_SSB_EFFECTS,        new[] { "Effects",                                                     "Effekte"                                                              });
			L10N.Dictionary.Add(STR_HSP_GLOBALRANKING,  new[] { "Global Ranking",                                              "Globale Bestenliste"                                                  });
			L10N.Dictionary.Add(STR_HSP_RANKINGFOR,     new[] { "Ranking for \"{0}\"",                                         "Bestenliste für \"{0}\""                                              });
			L10N.Dictionary.Add(STR_TAB_NAME,           new[] { "Name",                                                        "Name"                                                                 });
			L10N.Dictionary.Add(STR_TAB_POINTS,         new[] { "Points",                                                      "Punkte"                                                               });
			L10N.Dictionary.Add(STR_TAB_TIME,           new[] { "Total Time",                                                  "Gesamtzeit"                                                           });
			L10N.Dictionary.Add(STR_FAP_ACCOUNT,        new[] { "Account",                                                     "Benutzerkonto"                                                        });
			L10N.Dictionary.Add(STR_FAP_USERNAME,       new[] { "Username:",                                                   "Benutzername:"                                                        });
			L10N.Dictionary.Add(STR_FAP_SCORE,          new[] { "Points:",                                                     "Punkte:"                                                              });
			L10N.Dictionary.Add(STR_FAP_CHANGEPW,       new[] { "Change Password",                                             "Passwort ändern"                                                      });
			L10N.Dictionary.Add(STR_CPP_CHANGEPW,       new[] { "Change Password",                                             "Passwort ändern"                                                      });
			L10N.Dictionary.Add(STR_CPP_USERNAME,       new[] { "Username:",                                                   "Benutzername:"                                                        });
			L10N.Dictionary.Add(STR_CPP_NEWPW,          new[] { "New Password",                                                "Neues Passwort"                                                       });
			L10N.Dictionary.Add(STR_CPP_CHANGE,         new[] { "Change",                                                      "Ändern"                                                               });
			L10N.Dictionary.Add(STR_CPP_CHANGING,       new[] { "Changing password",                                           "Passwort wird geändert"                                               });
			L10N.Dictionary.Add(STR_CPP_CHANGED,        new[] { "Password changed",                                            "Passwort geändert"                                                    });
			L10N.Dictionary.Add(STR_CPP_COMERR,         new[] { "Could not communicate with server",                           "Kommunikation mit Server ist gestört"                                 });
			L10N.Dictionary.Add(STR_CPP_AUTHERR,        new[] { "Authentication error",                                        "Authentifizierung fehlgeschlagen"                                     });
			L10N.Dictionary.Add(STR_CPP_CHANGEERR,      new[] { "Could not change password",                                   "Passwort konnte nicht geändert werden"                                });
			L10N.Dictionary.Add(STR_ATTRIBUTIONS,       new[] { "Attributions",                                                "Info"                                                                 });
			L10N.Dictionary.Add(STR_AAP_HEADER,         new[] { "Sign up / Log in",                                            "Anmelden / Registrieren"                                              });
			L10N.Dictionary.Add(STR_AAP_USERNAME,       new[] { "Username",                                                    "Benutzername"                                                         });
			L10N.Dictionary.Add(STR_AAP_PASSWORD,       new[] { "Password",                                                    "Passwort"                                                             });
			L10N.Dictionary.Add(STR_AAP_CREATEACCOUNT,  new[] { "Create Account",                                              "Registrieren"                                                         });
			L10N.Dictionary.Add(STR_AAP_LOGIN,          new[] { "Login",                                                       "Anmelden"                                                             });
			L10N.Dictionary.Add(STR_AAP_LOGGINGIN,      new[] { "Logging in",                                                  "Wird angemeldet"                                                      });
			L10N.Dictionary.Add(STR_AAP_WRONGPW,        new[] { "Wrong password",                                              "Falsches Passwort"                                                    });
			L10N.Dictionary.Add(STR_AAP_USERNOTFOUND,   new[] { "User not found",                                              "Benutzer nicht gefunden"                                              });
			L10N.Dictionary.Add(STR_AAP_NOCOM,          new[] { "Could not communicate with server",                           "Konnte nicht mit Server kommunizieren"                                });
			L10N.Dictionary.Add(STR_AAP_LOGINSUCCESS,   new[] { "Successfully logged in",                                      "Benutzer erfolgreich angemeldet"                                      });
			L10N.Dictionary.Add(STR_AAP_NOLOGIN,        new[] { "Could not login",                                             "Anmeldung fehlgeschlagen"                                             });
			L10N.Dictionary.Add(STR_AAP_ACCCREATING,    new[] { "Creating account",                                            "Konto wird erstellt"                                                  });
			L10N.Dictionary.Add(STR_AAP_ACCCREATED,     new[] { "Account created",                                             "Konto erfolgreich erstellt"                                           });
			L10N.Dictionary.Add(STR_AAP_USERTAKEN,      new[] { "Username already taken",                                      "Benutzername bereits vergeben"                                        });
			L10N.Dictionary.Add(STR_AAP_ALREADYCREATED, new[] { "Account already created",                                     "Konto bereits erstellt"                                               });
			L10N.Dictionary.Add(STR_AAP_AUTHERROR,      new[] { "Authentication error",                                        "Authentifizierungsfehler"                                             });
			L10N.Dictionary.Add(STR_AAP_COULDNOTCREATE, new[] { "Could not create account",                                    "Konto konnte nicht erstellt werden"                                   });
			L10N.Dictionary.Add(STR_PAUS_RESUME,        new[] { "RESUME",                                                      "WEITER"                                                               });
			L10N.Dictionary.Add(STR_PAUS_RESTART,       new[] { "RESTART",                                                     "NEU STARTEN"                                                          });
			L10N.Dictionary.Add(STR_PAUS_EXIT,          new[] { "EXIT",                                                        "BEENDEN"                                                              });
			L10N.Dictionary.Add(STR_HSP_LEVEL,          new[] { "Level",                                                       "Level"                                                                });
			L10N.Dictionary.Add(STR_HSP_POINTS,         new[] { "Points",                                                      "Punkte"                                                               });
			L10N.Dictionary.Add(STR_HSP_PROGRESS,       new[] { "Progress",                                                    "Fortschritt"                                                          });
			L10N.Dictionary.Add(STR_HSP_BACK,           new[] { "Back",                                                        "Zurück"                                                               });
			L10N.Dictionary.Add(STR_HSP_NEXT,           new[] { "Next",                                                        "Weiter"                                                               });
			L10N.Dictionary.Add(STR_HSP_AGAIN,          new[] { "Again",                                                       "Wiederholen"                                                          });
			L10N.Dictionary.Add(STR_HSP_TUTORIAL,       new[] { "Tutorial",                                                    "Tutorial"                                                             });
			L10N.Dictionary.Add(STR_HSP_GETSTARTED,     new[] { "Let's get started",                                           "Los gehts"                                                            });
			L10N.Dictionary.Add(STR_HSP_CONERROR,       new[] { "Could not connect to highscore server",                       "Kommunikation mit Server fehlgeschlagen"                              });
			L10N.Dictionary.Add(STR_DIFF_0,             new[] { "Easy",                                                        "Leicht"                                                               });
			L10N.Dictionary.Add(STR_DIFF_1,             new[] { "Normal",                                                      "Normal"                                                               });
			L10N.Dictionary.Add(STR_DIFF_2,             new[] { "Hard",                                                        "Schwer"                                                               });
			L10N.Dictionary.Add(STR_DIFF_3,             new[] { "Extreme",                                                     "Extrem"                                                               });
			L10N.Dictionary.Add(STR_TUT_INFO1,          new[] { "Drag to rotate your own cannons",                             "Drücke und Ziehe um deine Kanonen zu drehen"                          });
			L10N.Dictionary.Add(STR_TUT_INFO2,          new[] { "Shoot it until it becomes your cannon",                       "Schieße bis die feindliche Kanone dir gehört"                        });
			L10N.Dictionary.Add(STR_TUT_INFO3,          new[] { "Now capture the next cannon",                                 "Erobere nun die nächste Einheit"                                      });
			L10N.Dictionary.Add(STR_TUT_INFO4,          new[] { "Keep shooting at the first cannon to increase its fire rate", "Schieß auf deine eigene Kanone um ihre Feuerrate zu erhöhen"         });
			L10N.Dictionary.Add(STR_TUT_INFO5,          new[] { "The enemy has captured a cannon. Attack him!",                "Der Gegner hat eine Einheit erobert, greif ihn an!"                   });
			L10N.Dictionary.Add(STR_TUT_INFO6,          new[] { "Speed up the Game with the bottom left button.",              "Mit dem Knopf unten links kannst du die Spielgeschwindigkeit erhöhen" });
			L10N.Dictionary.Add(STR_TUT_INFO7,          new[] { "Now capture the next cannon",                                 "Erobere jetzt die nächste Einheit"                                    });
			L10N.Dictionary.Add(STR_TUT_INFO8,          new[] { "Win the game by capturing all enemy cannons",                 "Gewinne die Schlacht indem du alle Einheiten eroberst"                });
			L10N.Dictionary.Add(STR_API_CONERR,         new[] { "Could not connect to highscore server",                       "Verbindung mit Highscore Server fehlgeschlagen"                       });
			L10N.Dictionary.Add(STR_API_COMERR,         new[] { "Could not communicate with highscore server",                 "Kommunikation mit Highscore Server fehlgeschlagen"                    });
			L10N.Dictionary.Add(STR_GLOB_EXITTOAST,     new[] { "Click again to exit game",                                    "Drücke \"Zurück\" nochmal um das Spiel zu beenden"                    });
			L10N.Dictionary.Add(STR_GLOB_UNLOCKTOAST1,  new[] { "Click two times more to unlock",                              "Noch zweimal drücken um die Welt freizuschalten"                      });
			L10N.Dictionary.Add(STR_GLOB_UNLOCKTOAST2,  new[] { "Click again to unlock",                                       "Nochmal drücken um die Welt freizuschalten"                           });
			L10N.Dictionary.Add(STR_GLOB_UNLOCKTOAST3,  new[] { "World unlocked",                                              "Welt freigeschaltet"                                                  }); 
			L10N.Dictionary.Add(STR_GLOB_WORLDLOCK,     new[] { "World locked",                                                "Welt noch nicht freigespielt"                                         });
			L10N.Dictionary.Add(STR_GLOB_LEVELLOCK,     new[] { "Level locked",                                                "Level noch nicht freigespielt"                                        });
			L10N.Dictionary.Add(STR_INF_YOU,            new[] { "You",                                                         "Du"                                                                   });
			L10N.Dictionary.Add(STR_INF_GLOBAL,         new[] { "Global",                                                      "Global"                                                               });
			L10N.Dictionary.Add(STR_INF_HIGHSCORE,      new[] { "Highscore",                                                   "Bestzeit"                                                             });
			L10N.Dictionary.Add(STR_GLOB_OVERWORLD,     new[] { "Overworld",                                                   "Übersicht"                                                            });
			L10N.Dictionary.Add(STR_GLOB_WAITFORSERVER, new[] { "Contacting server",                                           "Server wird kontaktiert"                                              });
			L10N.Dictionary.Add(STR_GLOB_TUTORIAL,      new[] { "Tutorial",                                                    "Tutorial"                                                             });
		}
	}
}
