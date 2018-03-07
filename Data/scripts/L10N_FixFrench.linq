<Query Kind="Program">
  <Connection>
    <ID>951bc30a-f75f-46a5-b7fa-91398aee62e1</ID>
    <Persist>true</Persist>
    <Driver Assembly="linq2db.LINQPad" PublicKeyToken="f19f8aed7feff67e">LinqToDB.LINQPad.LinqToDBDriver</Driver>
    <CustomCxString>Data Source=..\..\Source\GridDominance.Content\L10N\L10N.sqlite3;</CustomCxString>
    <ExcludeRoutines>true</ExcludeRoutines>
    <DisplayName>L10nTexts</DisplayName>
    <Provider>System.Data.SQLite</Provider>
    <Server>L10N</Server>
    <Database>main</Database>
    <DbVersion>3.19.3</DbVersion>
    <NoPluralization>true</NoPluralization>
    <NoCapitalization>true</NoCapitalization>
    <DriverData>
      <providerName>SQLite</providerName>
      <optimizeJoins>true</optimizeJoins>
      <allowMultipleQuery>false</allowMultipleQuery>
      <commandTimeout>0</commandTimeout>
    </DriverData>
  </Connection>
</Query>

private const string SOURCEDIR  = @"C:\Users\schwoerm\AppData\Local\M\gd\Source\GridDominance.Core\";
private const string SOURCEFILE = @"C:\Users\schwoerm\AppData\Local\M\gd\Source\GridDominance.Core\Resources\L10NImpl.cs";
private const string NAMESPACE = "GridDominance.Shared.Resources";

private readonly Regex REX = new Regex(@"public const int STR_(?<id>[A-Z_0-9]+)\s+=\s+(?<num>[0-9]+);");

void Main()
{
	#region r
	xadd("STR_SSB_ABOUT",
					 "About",
					 "Info",
					 "Info",
					 "Informazioni",
					 "Información");

	xadd("STR_SSB_ACCOUNT",
			 "Account",
			 "Benutzerkonto",
			 "Compte",
			 "Account",
			 "Cuenta");

	xadd("STR_SSB_HIGHSCORE",
			 "Highscore",
			 "Bestenliste",
			 "Tableau d'honneur",
			 "Classifica",
			 "Puntaje alto");

	xadd("STR_SSB_MUTE",
			 "Mute",
			 "Stumm",
			 "Muet",
			 "Muto",
			 "Mute");

	xadd("STR_SSB_EFFECTS",
			 "Effects",
			 "Effekte",
			 "Effet",
			 "Effetti",
			 "Efectos");

	xadd("STR_SSB_MUSIC",
			 "Music",
			 "Musik",
			 "Musique",
			 "Musica",
			 "Música");

	xadd("STR_SSB_COLOR",
			 "Color scheme",
			 "Farbschema",
			 "Schéma de couleurs",
			 "Schema a colori",
			 "Esquema de colores");

	xadd("STR_HSP_GLOBALRANKING",
			 "Global Ranking",
			 "Bestenliste",
			 "Classement globale",
			 "Classifica Globale",
			 "Ranking global");

	xadd("STR_HSP_MULTIPLAYERRANKING",
			 "Multiplayer",
			 "Mehrspieler",
			 "Multijoueur",
			 "Multiplayer",
			 "Multijugador");

	xadd("STR_HSP_RANKINGFOR",
			 "Ranking for \"{0}\"",
			 "Bestenliste für \"{0}\"",
			 "Classement pour \"{0}\"",
			 "Classifica per \"{0}\"",
			 "Ranking");

	xadd("STR_TAB_NAME",
			 "Name",
			 "Name",
			 "Nom",
			 "Nome",
			 "Nombre");

	xadd("STR_TAB_POINTS",
			 "Points",
			 "Punkte",
			 "Points",
			 "Punti",
			 "Puntos");

	xadd("STR_TAB_TIME",
			 "Total Time",
			 "Gesamtzeit",
			 "Temps total",
			 "Tempo Totale",
			 "Tiempo total");

	xadd("STR_FAP_ACCOUNT",
			 "Account",
			 "Benutzerkonto",
			 "Compte d'utilisateur",
			 "Account",
			 "Cuenta");

	xadd("STR_FAP_USERNAME",
			 "Username:",
			 "Benutzername:",
			 "Nom d'utilisateur",
			 "Username:",
			 "Usuario");

	xadd("STR_FAP_SCORE",
			 "Points:",
			 "Punkte:",
			 "Points",
			 "Points:",
			 "Puntos");

	xadd("STR_FAP_CHANGEPW",
			 "Change Password",
			 "Passwort ändern",
			 "Mot de passe",
			 "Cambia Password",
			 "Cambiar clave");

	xadd("STR_FAP_LOGOUT",
			 "Logout",
			 "Ausloggen",
			 "Déconnecter",
			 "Esci",
			 "Cerrar sesión");

	xadd("STR_FAP_WARN1",
			 "This will clear all local data. Press again to log out.",
			 "Dies löscht alle lokalen Daten. Nochmal drücken zum ausloggen.",
			 "Cette opération efface toutes les données locales. Appuyez à nouveau pour vous déconnecter.",
			 "Questo cancellerà tutti i dati locali. Premi di nuovo per uscire.",
			 "Esto borra todos los datos locales. Presiona otra vey para cerrar sesión");

	xadd("STR_FAP_WARN2",
			 "Are you really sure you want to log out?",
			 "Wirklich vom Serverkonto abmelden?",
			 "Êtes-vous vraiment sûr de vouloir vous déconnecter?",
			 "Sei sicuro di voler uscire?",
			 "Estas seguro que quieres cerrar sesión");

	xadd("STR_FAP_LOGOUT_SUCESS",
			 "Logged out from account",
			 "Lokaler Benutzer wurde abgemeldet.",
			 "Déconnecté du compte",
			 "Uscito dall'account",
			 "Usuario desconectado");

	xadd("STR_CPP_CHANGEPW",
			 "Change Password",
			 "Passwort ändern",
			 "Changer mot de passe",
			 "Cambia Password",
			 "Cambiar clave");

	xadd("STR_CPP_USERNAME",
			 "Username:",
			 "Benutzername:",
			 "Nom d'utilisateur",
			 "Username:",
			 "Usuario");

	xadd("STR_CPP_NEWPW",
			 "New Password",
			 "Neues Passwort",
			 "Noveau mot de passe",
			 "Nuova Password",
			 "Nueva clave");

	xadd("STR_CPP_CHANGE",
			 "Change",
			 "Ändern",
			 "Changer mot de passe",
			 "Cambia",
			 "Cambiar");

	xadd("STR_CPP_CHANGING",
			 "Changing password",
			 "Passwort wird geändert",
			 "Changement du mot de passe ",
			 "Cambiando la password",
			 "Coambiando clave");

	xadd("STR_CPP_CHANGED",
			 "Password changed",
			 "Passwort geändert",
			 "Mot de passe est changé",
			 "Password cambiata",
			 "Clave modificada");

	xadd("STR_CPP_COMERR",
			 "Could not communicate with server",
			 "Kommunikation mit Server ist gestört",
			 "La communication avec le serveur est perturbé",
			 "Impossibile comunicare con il server",
			 "No se puede comunicar con Server");

	xadd("STR_CPP_AUTHERR",
			 "Authentication error",
			 "Authentifizierung fehlgeschlagen",
			 "Erreur d'authentification",
			 "Errore di autenticazione",
			 "Error de auntentificación");

	xadd("STR_CPP_CHANGEERR",
			 "Could not change password",
			 "Passwort konnte nicht geändert werden",
			 "Mot de passe ne peut pas être modifié",
			 "Impossibile cambiare password",
			 "No se pudo cambiar la clave");

	xadd("STR_ATTRIBUTIONS",
			 "Attributions",
			 "Lizenzen",
			 "Licences",
			 "Attribuzioni",
			 "Atribuciones");

	xadd("STR_AAP_HEADER",
			 "Sign up / Log in",
			 "Anmelden / Registrieren",
			 "Se connecter",
			 "Iscriviti / Accedi",
			 "Registrarse / Iniciar sesión");

	xadd("STR_AAP_USERNAME",
			 "Username",
			 "Benutzername",
			 "Nom d'utilisateur",
			 "Username",
			 "Usuario");

	xadd("STR_AAP_PASSWORD",
			 "Password",
			 "Passwort",
			 "Mot de passe",
			 "Password",
			 "Clave");

	xadd("STR_AAP_CREATEACCOUNT",
			 "Create Account",
			 "Registrieren",
			 "Registrer",
			 "Crea Account",
			 "Registrarse");

	xadd("STR_AAP_LOGIN",
			 "Login",
			 "Anmelden",
			 "S'inscrire",
			 "Accedi",
			 "Iniciar sesión");

	xadd("STR_AAP_LOGGINGIN",
			 "Logging in",
			 "Wird angemeldet",
			 "Est enregistré",
			 "Accedendo",
			 "Iniciando sesión");

	xadd("STR_AAP_WRONGPW",
			 "Wrong password",
			 "Falsches Passwort",
			 "Mot de passe incorrect",
			 "Password sbagliata",
			 "Clave incorrecta");

	xadd("STR_AAP_USERNOTFOUND",
			 "User not found",
			 "Benutzer nicht gefunden",
			 "Utilisateur introuvable",
			 "Utente non trovato",
			 "Usuario no encontrado");

	xadd("STR_AAP_NOCOM",
			 "Could not communicate with server",
			 "Konnte nicht mit Server kommunizieren",
			 "La communication avec le serveur est perturbé",
			 "Impossibile comunicare con il server",
			 "No se puede comunicar con Server");

	xadd("STR_AAP_LOGINSUCCESS",
			 "Successfully logged in",
			 "Benutzer erfolgreich angemeldet",
			 "Connecté avec succès",
			 "Accesso completo",
			 "Sesión iniciada correctamente");

	xadd("STR_AAP_NOLOGIN",
			 "Could not login",
			 "Anmeldung fehlgeschlagen",
			 "Échec de la connexion",
			 "Impossibile accedere",
			 "No se pudo iniciar sesión");

	xadd("STR_AAP_ACCCREATING",
			 "Creating account",
			 "Konto wird erstellt",
			 "Création du compte",
			 "Creando l'account",
			 "Registrando cuenta de usuario");

	xadd("STR_AAP_ACCCREATED",
			 "Account created",
			 "Konto erfolgreich erstellt",
			 "Compte créé",
			 "Account creato",
			 "Usuario creado");

	xadd("STR_AAP_USERTAKEN",
			 "Username already taken",
			 "Benutzername bereits vergeben",
			 "Nom d'utilisateur déjà pris",
			 "Username già in uso",
			 "Nombre de usuario no disponible");

	xadd("STR_AAP_ALREADYCREATED",
			 "Account already created",
			 "Konto bereits erstellt",
			 "Compte déjà créé",
			 "Account già creato",
			 "Esta cuenta ya está registrada");

	xadd("STR_AAP_AUTHERROR",
			 "Authentication error",
			 "Authentifizierungsfehler",
			 "Erreur d'authentification",
			 "Errore di autenticazione",
			 "Error de auntentificación");

	xadd("STR_AAP_COULDNOTCREATE",
			 "Could not create account",
			 "Konto konnte nicht erstellt werden",
			 "Impossible de créer compte",
			 "Impossibile creare account",
			 "No se pudo crear cuenta");

	xadd("STR_PAUS_RESUME",
			 "RESUME",
			 "WEITER",
			 "REPRENDRE",
			 "CONTINUA",
			 "Continuar");

	xadd("STR_PAUS_RESTART",
			 "RESTART",
			 "NEU STARTEN",
			 "REDÉMARRER",
			 "RICOMINCIA",
			 "Reiniciar");

	xadd("STR_PAUS_EXIT",
			 "EXIT",
			 "BEENDEN",
			 "TERMINER",
			 "ESCI",
			 "Salir");

	xadd("STR_HSP_LEVEL",
			 "Level",
			 "Level",
			 "Niveau",
			 "Livello",
			 "Nivel");

	xadd("STR_HSP_POINTS",
			 "Points",
			 "Punkte",
			 "Ponts",
			 "Punti",
			 "Puntos");

	xadd("STR_HSP_PROGRESS",
			 "Progress",
			 "Fortschritt",
			 "Progrès",
			 "Progresso",
			 "Progreso");

	xadd("STR_HSP_TIME_NOW",
			 "Time (now)",
			 "Levelzeit",
			 "Temps",
			 "Tempo",
			 "Tiempo actual");

	xadd("STR_HSP_TIME_BEST",
			 "Time (best)",
			 "Bestzeit",
			 "Meilleur temps",
			 "Miglior tempo",
			 "Mejor tiempo");

	xadd("STR_HSP_BACK",
			 "Back",
			 "Zurück",
			 "De retour",
			 "Indietro",
			 "Atrás");

	xadd("STR_HSP_NEXT",
			 "Next",
			 "Weiter",
			 "Prochain",
			 "Avanti",
			 "Siguiente");

	xadd("STR_HSP_AGAIN",
			 "Again",
			 "Wiederholen",
			 "Répéter",
			 "Ripeti",
			 "Repetir");

	xadd("STR_HSP_TUTORIAL",
			 "Tutorial",
			 "Tutorial",
			 "Tutorial",
			 "Tutorial",
			 "Tutorial");

	xadd("STR_HSP_GETSTARTED",
			 "Let's get started",
			 "Los gehts",
			 "C'est parti",
			 "Iniziamo",
			 "A comenzar");

	xadd("STR_HSP_CONERROR",
			 "Could not connect to server",
			 "Kommunikation mit Server fehlgeschlagen",
			 "Impossible de se connecter au serveur",
			 "Impossibile connettersi al server della classifica",
			 "No se puede comunicar con Server");

	xadd("STR_DIFF_0",
			 "Easy",
			 "Leicht",
			 "Facile",
			 "Facile",
			 "Fácil");

	xadd("STR_DIFF_1",
			 "Normal",
			 "Normal",
			 "Normal",
			 "Normale",
			 "Normal");

	xadd("STR_DIFF_2",
			 "Hard",
			 "Schwer",
			 "Difficile",
			 "Difficile",
			 "Difícil");

	xadd("STR_DIFF_3",
			 "Extreme",
			 "Extrem",
			 "Extrême",
			 "Estremo",
			 "Extremo");

	xadd("STR_TUT_INFO1",
			 "Drag to rotate your own cannons",
			 "Drücke und Ziehe um deine Kanonen zu drehen",
			 "Tirer pour tourner tes canons",
			 "Trascina per ruotare i tuoi cannoni",
			 "Para rotar tus cañones presiona y tira");

	xadd("STR_TUT_INFO2",
			 "Shoot it until it becomes your cannon",
			 "Schieße bis die feindliche Kanone dir gehört",
			 "Abattre jusque le canon ennemi est à toi",
			 "Spara fino a che non diventa il tuo cannone",
			 "Dispara hasta que se convierta en tu cañón");

	xadd("STR_TUT_INFO3",
			 "Now capture the next cannon",
			 "Erobere nun die nächste Einheit",
			 "Captiver le prochain canon",
			 "Ora cattura il prossimo cannone",
			 "Ahora captura tu próximo cañón");

	xadd("STR_TUT_INFO4",
			 "Keep shooting at the first cannon to increase its fire rate",
			 "Schieß auf deine eigene Kanone um ihre Feuerrate zu erhöhen",
			 "Gardez le tir au premier canon pour augmenter sa cadence de tir",
			 "Continua a sparare al primo cannone per aumentare la cadenza di fuoco",
			 "Continúa disparando con tu primer cañón para aumentar su nivel de tiro");

	xadd("STR_TUT_INFO5",
			 "The enemy has captured a cannon. Attack him!",
			 "Der Gegner hat eine Einheit erobert, greif ihn an!",
			 "L'enemi a conquis  un canon. Attaque.",
			 "Il nemico ha conquistato un cannone. Attaccalo!",
			 "El enemigo ha conquistado un cañón, atácalo");

	xadd("STR_TUT_INFO6",
			 "Speed up the Game with the bottom left button.",
			 "Mit dem Knopf unten links kannst du die Spielgeschwindigkeit erhöhen",
			 "Accélérez le jeu avec le bouton en bas à gauche.",
			 "Accellera il gioco con i pulsanti in basso a sinistra.",
			 "Aumenta la velocidad de juego con el botón de abajo a la izquierda");

	xadd("STR_TUT_INFO7",
			 "Now capture the next cannon",
			 "Erobere jetzt die nächste Einheit",
			 "Maintenant capturez le prochain canon",
			 "Ora cattura il prossimo cannone",
			 "Ahora captura tu próximo cañón");

	xadd("STR_TUT_INFO8",
			 "Win the game by capturing all enemy cannons",
			 "Gewinne die Schlacht indem du alle Einheiten eroberst",
			 "Gagnez le jeu en capturant tous les canons ennemis",
			 "Vinci il gioco catturando tutti i cannoni nemici",
			 "Ganarás cuando hayas capturado todos los cañones");

	xadd("STR_API_CONERR",
			 "Could not connect to highscore server",
			 "Verbindung mit Highscore Server fehlgeschlagen",
			 "Impossible de se connecter au serveur Highscore",
			 "Impossibile connettersi al server della classifica",
			 "No se puede comunicar con Server");

	xadd("STR_API_COMERR",
			 "Could not communicate with highscore server",
			 "Kommunikation mit Highscore Server fehlgeschlagen",
			 "Impossible de communiquer avec le serveur highscore",
			 "Impossibile comunicare al server della classifica",
			 "No se puede comunicar con Server");

	xadd("STR_GLOB_EXITTOAST",
			 "Click again to exit game",
			 "Drücke nochmal \"Zurück\" um das Spiel zu beenden",
			 "Cliquez de noouveau pour quitter le jeu",
			 "Clicca di nuovo per uscire dal gioco",
			 "Presiona otra vez para finalizar el juego");

	xadd("STR_GLOB_UNLOCKTOAST1",
			 "Click two times more to unlock",
			 "Noch zweimal drücken um die Welt freizuschalten",
			 "Cliquez deux fois plus pour débloquer",
			 "Clicca due volte per sbloccare",
			 "Presiona dos veces para desbloquear");

	xadd("STR_GLOB_UNLOCKTOAST2",
			 "Click again to unlock",
			 "Nochmal drücken um die Welt freizuschalten",
			 "Cliquez de nouvea pour quitter le jeu",
			 "Clicca di nuovo per sbloccare",
			 "Presiona otra vez para desbloquear");

	xadd("STR_GLOB_UNLOCKTOAST3",
			 "World unlocked",
			 "Welt freigeschaltet",
			 "Monde débloqué",
			 "Mondo sbloccato",
			 "Mundo desbloqueado");

	xadd("STR_GLOB_WORLDLOCK",
			 "World locked",
			 "Welt noch nicht freigespielt",
			 "Monde bloqué",
			 "Mondo bloccato",
			 "Mundo bloqueado");

	xadd("STR_GLOB_LEVELLOCK",
			 "Level locked",
			 "Level noch nicht freigespielt",
			 "Niveau bloqué",
			 "Livello bloccato",
			 "Nivel bloqueado");

	xadd("STR_INF_YOU",
			 "You",
			 "Du",
			 "Toi",
			 "Tu",
			 "Tú");

	xadd("STR_INF_GLOBAL",
			 "Stats",
			 "Total",
			 "Global",
			 "Statistiche",
			 "Estadística");

	xadd("STR_INF_HIGHSCORE",
			 "Highscore",
			 "Bestzeit",
			 "Highscore",
			 "Highscore",
			 "Puntaje alto");

	xadd("STR_GLOB_OVERWORLD",
			 "Overworld",
			 "Übersicht",
			 "L'aperçu",
			 "Overworld",
			 "Síntesis");

	xadd("STR_GLOB_WAITFORSERVER",
			 "Contacting server",
			 "Server wird kontaktiert",
			 "Contacter le serveur",
			 "Contattando il server",
			 "Conectando con Server");

	xadd("STR_WORLD_TUTORIAL",
			 "Tutorial",
			 "Tutorial",
			 "Didacticiel",
			 "Tutorial",
			 "Tutorial");

	xadd("STR_SSB_LANGUAGE",
			 "Language",
			 "Sprache",
			 "Longue",
			 "Linguaggio",
			 "Idioma");

	xadd("STR_WORLD_W1",
			 "Basic",
			 "Grundlagen",
			 "Niveau base",
			 "Base",
			 "Principios básicos");

	xadd("STR_WORLD_W2",
			 "Professional",
			 "Fortgeschritten",
			 "Niveau avancé",
			 "Professionale",
			 "Profesional");

	xadd("STR_WORLD_W3",
			 "Futuristic",
			 "Futuristisch",
			 "Futuriste",
			 "Futuristico",
			 "Futurístico");

	xadd("STR_WORLD_W4",
			 "Toy Box",
			 "Spielzeugkiste",
			 "Coffre à jouets",
			 "Svago",
			 "Caja de juegos");

	xadd("STR_WORLD_MULTIPLAYER",
			 "Multiplayer",
			 "Mehrspieler",
			 "Multijoueur",
			 "Multiplayer",
			 "Multijugador");

	xadd("STR_WORLD_SINGLEPLAYER",
			 "Singleplayer",
			 "Einzelspieler",
			 "Seul joueur",
			 "Singleplayer",
			 "Juego individual");

	xadd("STR_IAB_TESTERR",
			 "Error connecting to Google Play services",
			 "Fehler beim Versuch mit Google Play zu verbinden",
			 "Erreurde connexion avec Google Play services",
			 "Impossibile connettersi ai servizi Google Play",
			 "Error en conexión con Google Play");

	xadd("STR_IAB_TESTNOCONN",
			 "No connection to Google Play services",
			 "Keine Verbindung zu Google Play services",
			 "Pas de connexion avec Google Play services",
			 "Nessuna connessione ai servizi Google Play",
			 "Sin conexión a Google Play");

	xadd("STR_IAB_TESTINPROGRESS",
			 "Payment in progress",
			 "Zahlung wird verarbeitet",
			 "Paiement en cours",
			 "Pagamento in lavorazione",
			 "Pago en progreso");

	xadd("STR_UNLOCK",
			 "Promotion Code",
			 "Promo Code",
			 "Code promotionnel",
			 "Codice Promozionale",
			 "Código de promoción");

	xadd("STR_ACKNOWLEDGEMENTS",
			 "Acknowledgements",
			 "Danksagungen",
			 "Remerciements",
			 "Ringraziamenti",
			 "Agradecimientos");

	xadd("STR_GLOB_UNLOCKSUCCESS",
			 "Upgraded game to full version!",
			 "Spiel wurde zur Vollversion aufgewertet",
			 "Mise à niveau du jeu en version complète!",
			 "Congratulazioni, hai acquistato la versione completa!",
			 "Has obtenido la versión completa del juego");

	xadd("STR_PREV_BUYNOW",
			 "Unlock now",
			 "Jetzt freischalten",
			 "Débloquer maintenant",
			 "Sblocca ora",
			 "Desbloquear ahora");

	xadd("STR_IAB_BUYERR",
			 "Error connecting to Google Play services",
			 "Fehler beim Versuch mit Google Play zu verbinden",
			 "Erreurde connexion avec Google Play services",
			 "Impossibile connettersi ai servizi Google Play",
			 "Error en conexión con Google Play");

	xadd("STR_IAB_BUYNOCONN",
			 "No connection to Google Play services",
			 "Keine Verbindung zu Google Play services",
			 "Pas de connexion avec Google Play services",
			 "Nessuna connessione ai servizi Google Play",
			 "Sin conexión a Google Play");

	xadd("STR_IAB_BUYNOTREADY",
			 "Connection to Google Play services not ready",
			 "Verbindung zu Google Play services nicht bereit",
			 "La connexion aux services Google Play n'est pas prête",
			 "Connessione ai servizi Google Play non pronta",
			 "Conexión a Google Play no disponible");

	xadd("STR_IAB_BUYSUCESS",
			 "World successfully purchased",
			 "Levelpack wurde erfolgreich erworben",
			 "Le monde a acheté avec succès",
			 "Mondo acquistato!",
			 "Has comprado el mundo exitosamente");

	xadd("STR_HINT_001",
			 "Tip: Shoot stuff to win!",
			 "Tipp: Versuche auf die andere Kanone zu schiessen",
			 "Allusion: tirez des trucs pour gagner!",
			 "Consiglio: Spara per vincere!",
			 "Consejo: dispara al otro cañón");

	xadd("STR_HINT_002",
			 "Bigger Cannon",
			 "Größere Kanone",
			 "Plus grands canons",
			 "Cannone più grande",
			 "Cañón más grande");

	xadd("STR_HINT_003",
			 "More Power",
			 "Mehr Schaden",
			 "Plus d'énergie",
			 "Più potenza",
			 "Más potencia");

	xadd("STR_HINT_004",
			 "Black holes attract your bullets",
			 "Schwarze Löcher saugen deine Kugeln ein",
			 "Les trous noirs attirent vos balles",
			 "I buchi neri attraggono i tuoi proiettili",
			 "Hoyos negros succionan tus balas ");

	xadd("STR_HINT_005",
			 "Lasers!",
			 "Laser!",
			 "Lasers!",
			 "Laser!",
			 "Lásers!");

	xadd("STR_HINT_006",
			 "Try dragging the map around",
			 "Versuche mal die Karte zu verschieben",
			 "Essayez de faire glisser la carte autour",
			 "Prova a trascinare la mappa",
			 "Intenta desplazar el mapa");

	xadd("STR_HINT_007",
			 "Speedy thing goes in,",
			 "Speedy thing goes in,",
			 "Speedy thing goes in,",
			 "Cosina veloce entra,",
			 "Speedy thing entra");

	xadd("STR_HINT_008",
			 "speedy thing comes out.",
			 "speedy thing comes out.",
			 "speedy thing comes out.",
			 "cosina veloce esce.",
			 "Speedy thing sale");

	xadd("STR_HINT_009",
			 "Some cannons only relay",
			 "Manche Kanonen leiten nur weiter",
			 "Certains canons relèvent",
			 "Alcuni cannoni collegano e basta",
			 "Algunos cañones solo redireccionan");

	xadd("STR_HINT_010",
			 "Shields can",
			 "Schilde können",
			 "Les écus peuvent",
			 "Gli scudi",
			 "Los escudos");

	xadd("STR_HINT_011",
			 "protect you",
			 "dich beschützen",
			 "protégez-vous",
			 "ti proteggono",
			 "pueden protegerte");

	xadd("STR_INFOTOAST_1",
			 "Your best time is {0}",
			 "Deine Bestzeit ist {0}",
			 "Votre meilleur temps est {0}",
			 "Il tuo tempo migliore è {0}",
			 "Tu mejor tiempo");

	xadd("STR_INFOTOAST_2",
			 "The global best time is {0}",
			 "Die globale Bestzeit ist {0}",
			 "Le meilleur temps global est {0}",
			 "Il tempo assoluto migliore è {0}",
			 "El récord de tiempo");

	xadd("STR_INFOTOAST_3",
			 "{0} users have completed this level on {1}",
			 "{0} Spieler haben dieses Level auf {1} geschafft",
			 "{0} utilisateurs ont complété ce niveau sur {1}",
			 "{0} utenti hanno completato questo livello in {1}",
			 "{0} Jugadores han completado este nivel en {1}");

	xadd("STR_INFOTOAST_4",
			 "You have not completed this level on {0}",
			 "Du hast dieses Level auf {0} noch nicht geschafft",
			 "Vous n'avez pas terminé ce niveau sur {0}",
			 "Non hai completato questo livello in {0}",
			 "No has completado este nivel en {0}");

	xadd("STR_PREV_FINISHWORLD",
			 "Finish World {0}",
			 "Welt {0}",
			 "Terminer Monde {0}",
			 "Finisci il mondo {0}",
			 "Finalizar el mundo");

	xadd("STR_PREV_OR",
			 "OR",
			 "ODER",
			 "OU",
			 "oppure",
			 "O");

	xadd("STR_PREV_MISS_TOAST",
			 "You are missing {0} points to unlock world {1}",
			 "Dir fehlen noch {0} Punkte um Welt {1} freizuschalten",
			 "Vous manquez de {0} points pour débloquer le monde {1}",
			 "Ti mancano {0} punti per sbloccare il mondo {1}",
			 "Te faltan {0} puntos para desbloquear el mundo {1}");

	xadd("STR_MP_TIMEOUT",
			 "Timeout - Connection to server lost",
			 "Timeout - Verbindung zu server verloren",
			 "Timeout - Connexion au serveur perdu",
			 "Timeout - Connessione al server persa",
			 "Timeout - sin conexión a Server");

	xadd("STR_MP_TIMEOUT_USER",
			 "Timeout - Connection to user [{0}] lost",
			 "Timeout - Verbindung zu Spieler [{0}] verloren",
			 "Timeout - Connexion à l'utilisateur [{0}] perdu",
			 "Timeout - Connection all'utente [{0}] persa",
			 "Timeout - sin conexión a usuario");

	xadd("STR_MP_NOTINLOBBY",
			 "You a not part of this session",
			 "Du bist kein Teilnehmer dieser Sitzung",
			 "Vous ne faites pas partie de cette session",
			 "Non fai parte di questa sessione",
			 "No eres parte de esta sesión");

	xadd("STR_MP_SESSIONNOTFOUND",
			 "Session on server not found",
			 "Sitzung konnte auf dem Server nicht gefunden werden",
			 "Session sur le serveur pas trouvé",
			 "Nessuna sessione trovata su questo server",
			 "Sesión no pudo ser encontrada");

	xadd("STR_MP_AUTHFAILED",
			 "Authentification on server failed",
			 "Authentifizierung auf Server fehlgeschlagen",
			 "L'authentification sur serveur a échoué",
			 "Autenticazione sul server fallita",
			 "Autentificación en Server ha fallado");

	xadd("STR_MP_LOBBYFULL",
			 "Server lobby is full",
			 "Serverlobby ist voll",
			 "Le lobby du serveur est plein",
			 "La lobby del server è piena",
			 "Server lobby está lleno");

	xadd("STR_MP_VERSIONMISMATCH",
			 "Server has a different game version ({0})",
			 "Serverversion unterscheidet sich von lokaler Version ({0})",
			 "Le serveur a une version de jeu différente({0})",
			 "Il server ha una versione diversa del gioco ({0})",
			 "Server tiene una versión de juego diferente ({0})");

	xadd("STR_MP_LEVELNOTFOUND",
			 "Could not find server level locally",
			 "Level konnte lokal nicht gefunden werden",
			 "Impossible de trouver le niveau de serveur localement",
			 "Impossibile trovare il livello nel server sul telefono",
			 "Nivel no pudo ser encontrado localmente");

	xadd("STR_MP_LEVELMISMATCH",
			 "Server has different version of level",
			 "Level auf dem Server unterscheidet sich von lokaler Version",
			 "Le serveur a une version de niveau différente",
			 "Il server ha una versione diversa del livello",
			 "Server tiene un nivel de versión diferente");

	xadd("STR_MP_USERDISCONNECT",
			 "User {0} has disconnected",
			 "Der Benutzer {0} hat die Verbindung getrennt",
			 "L'utilisateur {0} s'est déconnecté",
			 "L'utente {0} si è disconnesso",
			 "Usuario {0} se ha desconectado");

	xadd("STR_MP_SERVERDISCONNECT",
			 "Server has closed this session",
			 "Spiel wurde vom Server geschlossen",
			 "Le serveur a fermé cette session",
			 "Il server ha chiuso questa sessione",
			 "Server ha cerrado esta sesión");

	xadd("STR_MP_INTERNAL",
			 "Internal multiplayer error",
			 "Interner Fehler im Mehrspielermodul",
			 "Error interal au module multiplayer",
			 "Problema intero di multiplayer",
			 "Error en módulo mas de un jugador");

	xadd("STR_MP_BTADAPTERNULL",
			 "No bluetooth hardware found",
			 "Bluetooth Hardware nicht gefunden",
			 "Bluetooth n'a pas été trouvé",
			 "Nessun apparecchio bluetooth trovato",
			 "Bluetooth no encontrado");

	xadd("STR_MP_BTADAPTERPERMDENIED",
			 "Missing bluetooth permission",
			 "Bluetooth Berechtigung wurde nicht gewährt",
			 "Absence d'autorisation de bluetooth",
			 "Permessi bluetooth disattivati",
			 "Autorización de Bluetooth desactivada");

	xadd("STR_MP_BTDISABLED",
			 "Bluetooth is disabled",
			 "Bluetooth ist deaktiviert",
			 "Connexion Bluetooth deactivé",
			 "Bluetooth disattivatp",
			 "Bluetooth no disponible");

	xadd("STR_MP_DIRECTCONNFAIL",
			 "Bluetooth connection failed",
			 "Bluetooth Verbindungsaufbau fehlgeschlagen",
			 "Connexion Bluetooth échoué",
			 "Connessione Bluetooth fallita",
			 "Conexión a Bluetooth ha fallado");

	xadd("STR_MP_DIRECTCONNLOST",
			 "Bluetooth connection lost",
			 "Bluetooth Verbindung verloren",
			 "Connexion Bluetooth perdu",
			 "Connessione Bluetooth persa",
			 "Conexión a Bluetooth perdida");

	xadd("STR_MP_NOSERVERCONN",
			 "No connection to server",
			 "Keine Verbindung zu Server",
			 "Pas de connexion au serveur",
			 "Nessuna connessione al server",
			 "Sin conexión a Server");

	xadd("STR_MENU_CAP_MULTIPLAYER",
			 "Multiplayer",
			 "Mehrspieler",
			 "Multijoueur",
			 "Multiplayer",
			 "Multijugador");

	xadd("STR_MENU_CAP_LOBBY",
			 "Multiplayer Lobby",
			 "Lobby",
			 "Online Lobby",
			 "Multiplayer Lobby",
			 "Lobby mas de un jugador");

	xadd("STR_MENU_CAP_CGAME_PROX",
			 "Create Online Game",
			 "Onlinespiel erstellen",
			 "Creer un jeu en ligne",
			 "Crea una partita Online",
			 "Crear juego online");

	xadd("STR_MENU_CAP_CGAME_BT",
			 "Create Local Game",
			 "Lokales Spiel erstellen",
			 "Creer un jeu local",
			 "Crea una partita Local",
			 "Crear un juego local");

	xadd("STR_MENU_CAP_SEARCH",
			 "Search for local devices",
			 "Suche nach lokalem Spiel",
			 "Cherchez des périphériques locaux",
			 "Cerca dispostivi locali",
			 "Buscando dispositivos locales");

	xadd("STR_MP_ONLINE",
			 "Online",
			 "Online",
			 "En ligne",
			 "Online",
			 "Online");

	xadd("STR_MP_OFFLINE",
			 "Offline",
			 "Offline",
			 "Hors ligne",
			 "Offline",
			 "Offline");

	xadd("STR_MP_CONNECTING",
			 "Connecting",
			 "Verbinden",
			 "Connecter",
			 "Connessione",
			 "Conectando");

	xadd("STR_MENU_MP_JOIN",
			 "Join",
			 "Beitreten",
			 "Joindre",
			 "Partecipa",
			 "Participar");

	xadd("STR_MENU_MP_HOST",
			 "Host",
			 "Erstellen",
			 "Rédiger",
			 "Ospita",
			 "Crear");

	xadd("STR_MENU_MP_CREATE",
			 "Create",
			 "Start",
			 "Créer",
			 "Crea",
			 "Iniciar");

	xadd("STR_MENU_CANCEL",
			 "Cancel",
			 "Abbrechen",
			 "Abandonner",
			 "Cancella",
			 "Cancelar");

	xadd("STR_MENU_DISCONNECT",
			 "Disconnect",
			 "Verbindung trennen",
			 "Déconnecter",
			 "Disconnetti",
			 "Desconectar");

	xadd("STR_MENU_MP_LOCAL_CLASSIC",
			 "Local (Bluetooth)",
			 "Lokal (Bluetooth)",
			 "Local (Bluetooth)",
			 "Locale (Bluetooth)",
			 "Local (Bluetooth)");

	xadd("STR_MENU_MP_ONLINE",
			 "Online (UDP/IP)",
			 "Internet (UDP/IP)",
			 "En ligne (UDP/IP)",
			 "Online (UDP/IP)",
			 "Online (UDP/IP)");

	xadd("STR_MENU_CAP_AUTH",
			 "Enter lobby code",
			 "Lobby Code eingeben",
			 "Entrer lobby code",
			 "Inserisci codice lobby",
			 "Ingresa código lobby");

	xadd("STR_MENU_MP_GAMESPEED",
			 "Game speed:",
			 "Spielgeschwindigkeit:",
			 "La vitesse du jeux",
			 "Velocità gioco:",
			 "Velocidad de juego");

	xadd("STR_MENU_MP_MUSIC",
			 "Background music:",
			 "Hintergrundmusik:",
			 "Musique de fond",
			 "Musica:",
			 "Música de fondo");

	xadd("STR_MENU_MP_LOBBYINFO",
			 "Enter this code on another phone to join this session.",
			 "Gib diesen Code auf einem anderen Smartphone ein, um diesem Spiel beizutreten",
			 "Entrez ce code sur un autre smartphone pour rejoindre ce jeu",
			 "Inserire questo codice su un altro telefono per unirsi alla sessione.",
			 "Ingresa este código en otro Smartphone para unirte al juego");

	xadd("STR_MENU_MP_LOBBY_USER",
			 "Users:",
			 "Mitspieler:",
			 "Coéquipier:",
			 "Utenti:",
			 "Usuarios:");

	xadd("STR_MENU_MP_LOBBY_USER_FMT",
			 "Users: {0}",
			 "Mitspieler: {0}",
			 "Coéquipier: {0}",
			 "Utenti: {0}",
			 "Usuario: {0}");

	xadd("STR_MENU_MP_LOBBY_LEVEL",
			 "Level:",
			 "Level:",
			 "Level:",
			 "Livello:",
			 "Nivel");

	xadd("STR_MENU_MP_LOBBY_MUSIC",
			 "Background music:",
			 "Hintergrundmusik:",
			 "Musique de fond:",
			 "Musica:",
			 "Música de fondo");

	xadd("STR_MENU_MP_LOBBY_SPEED",
			 "Game speed:",
			 "Spielgeschwindigkeit:",
			 "La vitesse de jeu",
			 "Velocità gioco:",
			 "Velocidad de juego");

	xadd("STR_MENU_MP_LOBBY_PING",
			 "Ping",
			 "Ping",
			 "Ping",
			 "Ping",
			 "Ping");

	xadd("STR_MENU_MP_START",
			 "Start",
			 "Start",
			 "Démarrage",
			 "Inizia",
			 "Iniciar");

	xadd("STR_FRAC_N0",
			 "Gray",
			 "Gray",
			 "Gris",
			 "Grigio",
			 "Gris");

	xadd("STR_FRAC_P1",
			 "Green",
			 "Grün",
			 "Vert",
			 "Verde",
			 "Verde");

	xadd("STR_FRAC_A2",
			 "Red",
			 "Rot",
			 "Rouge",
			 "Rosso",
			 "Rojo");

	xadd("STR_FRAC_A3",
			 "Blue",
			 "Blau",
			 "Bleu",
			 "Blu",
			 "Azul");

	xadd("STR_FRAC_A4",
			 "Purple",
			 "Lila",
			 "Violet",
			 "Viola",
			 "Púrpura");

	xadd("STR_FRAC_A5",
			 "Orange",
			 "Orange",
			 "Orange",
			 "Arancione",
			 "Naranja");

	xadd("STR_FRAC_A6",
			 "Teal",
			 "BlauGrün",
			 "Vert bleu",
			 "Turchese",
			 "Turquesa");

	xadd("STR_MENU_MP_LOBBY_COLOR",
			 "Color",
			 "Farbe",
			 "Couleur",
			 "Colore",
			 "Colores");

	xadd("STR_HSP_NEWGAME",
			 "New Game",
			 "Neues Spiel",
			 "Nouveau jeu",
			 "Nuova partita",
			 "Nuevo juego");

	xadd("STR_HSP_RANDOMGAME",
			 "Random level",
			 "Zufälliges Level",
			 "Niveau aléatoire",
			 "Livello casuale",
			 "Nivel aleatorio");

	xadd("STR_HSP_MPPOINTS",
			 "Multiplayer score",
			 "Mehrspieler Punkte",
			 "Multiplayer score",
			 "Punteggio Multiplayer",
			 "Puntaje Multiplayer");

	xadd("STR_MP_TOAST_CONN_TRY",
			 "Connecting to '{0}'",
			 "Verbinden mit '{0}'",
			 "Connecter à '{0}'",
			 "Connessione a '{0}'",
			 "Conectando a");

	xadd("STR_MP_TOAST_CONN_FAIL",
			 "Connection to '{0}' failed",
			 "Verbindung mit '{0}' fehlgeschlagen",
			 "Connexion avec '{0}' est échoué",
			 "Connessione a '{0}' fallita",
			 "Conección a '{0}' ha fallado");

	xadd("STR_MP_TOAST_CONN_SUCC",
			 "Connected to '{0}'",
			 "Verbunden mit '{0}'",
			 "Connecté avec '{0}'",
			 "Connessione a '{0}'",
			 "Conectado a '{0}'");

	xadd("STR_ENDGAME_1",
			 "THANKS FOR",
			 "THANKS FOR",
			 "THANKS FOR",
			 "GRAZIE PER",
			 "GRACIAS POR");

	xadd("STR_ENDGAME_2",
			 "PLAYING",
			 "PLAYING",
			 "PLAYING",
			 "AVER GIOCATO",
			 "JUGANDO");

	xadd("STR_ACCOUNT_REMINDER",
			 "You can create an account to display your name in the highscore and to backup your score online.\nDo you want to create an account now?",
			 "Du kannst einen Onlineaccount anlegen um deinen Namen im Highscore zu sehen und deine Punkte zu sichern.\n Account jetzt erstellen?",
			 "Vous pouvez créer un compte pour afficher votre nom dans les meilleurs scores et sauvegarder vos points en ligne.\nVoulez - vous créer un compte maintenant?",
			 "Puoi creare un account per far apparire il tuo nome nella classifica e per salvare i tuoi dati online.\nVuoi creare un'account?",
			 "Puedes crear una cuenta online para registrar tu puntuación y guardar tus puntajes altos.\nCrear cuenta ahora?");

	xadd("STR_BTN_OK",
			 "OK",
			 "OK",
			 "OK",
			 "OK",
			 "OK");

	xadd("STR_BTN_NO",
			 "No",
			 "Nein",
			 "Aucun",
			 "No",
			 "No");

	xadd("STR_ERR_SOUNDPLAYBACK",
			 "Sound playback failed. Disabling sounds ...",
			 "Soundwiedergabe fehlgeschlagen. Sounds werden deaktiviert ...",
			 "La lecture du son a échoué. Désactivation des sons ...",
			 "La riproduzione audio non è riuscita. Disattivazione dei suoni ...",
			 "Reproducción de sonido ha fallado. Deactivando sonidos");

	xadd("STR_ERR_MUSICPLAYBACK",
			 "Music playback failed. Disabling music ...",
			 "Musikwiedergabe fehlgeschlagen. Musik wird deaktiviert ...",
			 "La lecture de musique a échoué. Désactivation de la musique ...",
			 "La riproduzione musicale è fallita. Disattivazione della musica ...",
			 "Reproducción de música ha fallado. Deactivando sonidos");

	xadd("STR_ERR_OUTOFMEMORY",
			 "Saving failed: Disk full",
			 "Speichern fehlgeschlagen: Speicher voll",
			 "Échec échoué: Disque complet",
			 "Salvataggio non riuscito: disco pieno",
			 "Memoria llena: almacenamiento ha fallado ");

	xadd("STR_PROFILESYNC_START",
			 "Starting manual sync",
			 "Manuelle Synchronisation gestartet",
			 "La synchronisation manuelle a commencé",
			 "La sincronizzazione manuale è iniziata",
			 "Inicio de sincronización manual");

	xadd("STR_PROFILESYNC_ERROR",
			 "Manual sync failed",
			 "Manuelle Synchronisation fehlgeschlagen",
			 "La synchronisation manuelle a échoué",
			 "La sincronizzazione manuale non è riuscita",
			 "La sincronización manual falló");

	xadd("STR_PROFILESYNC_SUCCESS",
			 "Manual sync finished",
			 "Manuelle Synchronisation erfolgreich",
			 "Synchronisation manuelle réussie",
			 "La sincronizzazione manuale è riuscita",
			 "Sincronización manual finalizada");

	xadd("STR_AUTHERR_HEADER",
			 "You've been logged out",
			 "Sie wurden ausgeloggt",
			 "Vous avez été déconnecté",
			 "Sei stato disconnesso",
			 "Has sido desconectado");

	xadd("STR_WORLD_ONLINE",
			 "Online",
			 "Online",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me


	xadd("STR_LVLED_MOUSE",
			 "Move",
			 "Verschieben",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_CANNON",
			 "Cannon",
			 "Kanone",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_WALL",
			 "Wall",
			 "Wand",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_OBSTACLE",
			 "Obstacle",
			 "Hindernis",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_PORTAL",
			 "Portal",
			 "Portal",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_SETTINGS",
			 "Settings",
			 "Einstellungen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_PLAY",
			 "Test",
			 "Testen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_UPLOAD",
			 "Upload",
			 "Hochladen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_EXIT",
			 "Exit",
			 "Beenden",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me


	xadd("STR_LVLED_BTN_FRAC",
			 "Fraction",
			 "Fraktion",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_SCALE",
			 "Scale",
			 "Größe",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_ROT",
			 "Rotation",
			 "Drehung",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_TYPE",
			 "Type",
			 "Typ",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_DIAMETER",
			 "Diameter",
			 "Durchmesser",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_WIDTH",
			 "Width",
			 "Breite",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_HEIGHT",
			 "Height",
			 "Höhe",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_POWER",
			 "Power",
			 "Stärke",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_DEL",
			 "Delete",
			 "Entfernen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_CHANNEL",
			 "Channel",
			 "Kanal",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_DIR",
			 "Direction",
			 "Richtung",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_LEN",
			 "Length",
			 "Länge",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_CFG_ID",
			 "ID",
			 "ID",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_CFG_NAME",
			 "Name",
			 "Name",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_CFG_SIZE",
			 "Size",
			 "Größe",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_CFG_VIEW",
			 "Initial view",
			 "Startansicht",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_CFG_GEOMETRY",
			 "Geometry",
			 "Geometrie",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_CFG_WRAP_INFINITY",
			 "Infinity",
			 "Endlos",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_CFG_WRAP_DONUT",
			 "Donut",
			 "Donut",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_CFG_WRAP_REFLECT",
			 "Reflect",
			 "Reflektierend",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_ERR_NONAME",
			 "Level has no name set",
			 "Level hat keinen Namen gesetzt",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_ERR_NOENEMY",
			 "You need at least two player",
			 "Ein Level braucht mindestens zwei Spieler",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_ERR_NOPLAYER",
			 "At least one cannon must be owned by the player",
			 "Mindestens eine Kanone muss dem Spieler gehören",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_ERR_TOOMANYENTS",
			 "Level has too many entities",
			 "Zu viele Elemente im Level",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_ERR_COMPILERERR",
			 "Level compilation failed internally",
			 "Level konnte nicht kompiliert werden",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_DELLEVEL",
			 "Delete level",
			 "Level löschen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_SAVE",
			 "Save changes",
			 "Änderungen speichern",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_DISCARD",
			 "Discard changes",
			 "Änderungen verwerfen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_TOAST_DELLEVEL",
			 "Level deleted.",
			 "Level gelöscht.",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_MENU_CAP_SCCM",
			 "User levels",
			 "Benutzerlevel",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_TAB_MYLEVELS",
			 "My levels",
			 "Meine Level",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_TAB_HOT",
			 "Hot",
			 "Beliebt",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_TAB_TOP",
			 "Top",
			 "Top",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_TAB_NEW",
			 "New",
			 "Neu",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_TAB_SEARCH",
			 "Search",
			 "Suche",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_NEWLVL",
			 "Create new level",
			 "Neues Level erstellen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_GENERIC_SERVER_QUERY",
			 "Processing your request",
			 "Anfrage wird bearbeitet",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me


	xadd("STR_LVLED_BTN_EDIT",
			 "Edit",
			 "Edit",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_PLAY",
			 "Play",
			 "Play",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_COMPILING",
			 "Compiling",
			 "Erzeugen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me


	xadd("STR_MUSIC_NONE",
			 "None",
			 "Keine",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_MUSIC_INT",
			 "Song {0}",
			 "Lied {0}",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_CFG_MUSIC",
			 "Song:",
			 "Lied:",
			 "Musique:",
			 "Musica:",
			 "Música:");

	xadd("STR_LVLED_BTN_UPLOAD",
			 "Upload",
			 "Veröffentlichen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_UPLOADING",
			 "Uploading",
			 "Wird hochgeladen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_UPLOAD_FIN",
			 "Level successfully published",
			 "Level wurde veröffentlicht",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me


	xadd("STR_LVLUPLD_ERR_INTERNAL",
			 "Server error while uploading level",
			 "Im Server trat ein Fehler während des Uploads auf",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLUPLD_ERR_FILETOOBIG",
			 "Levelfile too big",
			 "Leveldatei zu groß",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLUPLD_ERR_WRONGUSER",
			 "Level was created by a different account",
			 "Level wurde von einem anderen Benutzer erstellt",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLUPLD_ERR_LIDNOTFOUND",
			 "Level was deleted on the server",
			 "Level wurde auf dem Server gelöscht",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLUPLD_ERR_INVALIDNAME",
			 "Ungültiger Name",
			 "Invalid name for level",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_INF_CLEARS",
			 "Clears",
			 "Gesamt",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_SCCM_DOWNLOADFAILED",
			 "Download failed",
			 "Dowload fehlgeschlagen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_SCCM_DOWNLOADINPROGRESS",
			 "Downloading...",
			 "Downloading...",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_SCCM_VERSIONTOOOLD",
			 "This level needs a newer version of the game",
			 "Level benötigt eine neue Version der App",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_SCCM_NEEDS_ACC",
			 "You need a registered account to create levels",
			 "Ein Account wird benötigt um Level zu erstellen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_SCOREMAN_INFO_SCORE",
			 "You get these tokens by completing normal levels.\nHigher difficulties result in more points.",
			 "Diese Punkte erlangt man durch das Spielen normaler Level.\nHöhere Schwierigkeitsstufen geben mehr Punkte.",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_SCOREMAN_INFO_MPSCORE",
			 "You can get these tokens by winning multiplayer games against your friends.",
			 "Diese Punkte erlangt man durch gewinnen von lokalen Mehrspieler-Partien",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_SCOREMAN_INFO_SCCMSCORE",
			 "You can get these tokens by winning user generated levels from other players",
			 "Dise Punkte erlangt man durch das Spielen von Leveln die von anderen Spielern erstellt wurden",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_SCOREMAN_INFO_STARS",
			 "You get these stars when other players give a level you created a star",
			 "Du bekommst Sterne wenn andere Spieler ein Level, das du erstellt hast, positiv bewerten",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_HSP_STARRANKING",
			 "Stars",
			 "Sterne",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_HSP_SCCMRANKING",
			 "User level points",
			 "Benutzerlevel Punkte",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_TAB_STARS",
			 "Stars",
			 "Sterne",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_PREV_FINISHGAME",
			 "Finish the game",
			 "Gewinne das Spiel",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_ACH_UNLOCK_ONLINE",
			 "User levels unlocked",
			 "Benutzerlevel entsperrt",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_ACH_UNLOCK_MULTIPLAYER",
			 "Local multiplayer unlocked",
			 "Lokaler Mehrspieler entsperrt",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_ACH_UNLOCK_WORLD",
			 "Unlocked world {0}",
			 "Welt {0} entsperrt",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLUPLD_ERR_DUPLNAME",
			 "Name is already used by another level",
			 "Levelname wird schon verwendet",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_ABORT",
			 "Abort",
			 "Abbrechen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_LVLED_BTN_RETRY",
			 "Retry",
			 "Wiederholen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_ACH_FIRSTCLEAR",
			 "First Clear",
			 "Erster",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_ACH_WORLDRECORD",
			 "World record",
			 "Weltrekord",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_HSP_AUTHOR",
			 "Author",
			 "Ersteller",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_HSP_TIME_YOU",
			 "Time (you)",
			 "Deine Bestzeit",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me

	xadd("STR_SCCM_UPLOADINFO",
			 "Beat your own level to upload it.",
			 "Gewinne dein eigenes Level um es hochzuladen",
			 "?",  //TODO translate me
			 "?",  //TODO translate me
			 "?"); //TODO translate me
	#endregion
}

void xadd(string id, string en, string de, string fr, string it, string es)
{
	var frold = texts.Single(t => t.id == id.Substring(4)).fr;

	if (fr != frold) 
	{
		$"{fr}\n{frold}\n\n".Dump();
		var ttt = texts.Single(t => t.id == id.Substring(4));
		ttt.fr = fr;
		this.Update(ttt);
	}
	
}