package de.samdev.griddominance.server;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.PrintWriter;
import java.net.*;
import java.util.*;

public class Main {

    public static final int PORT_SERVER             = 28023;
    public static final int PACKAGE_SIZE            = 1500;
    public static final int PACKAGE_SIZE_SMALL      = 32;

    public static final int GAME_SESSION_TIMEOUT    = 16 * 1000;
    public static final int GAME_USER_TIMEOUT       =  8 * 1000;
    public static final int STATE_OUTPUT_FREQ       = 45 * 1000;

    public static final byte CMD_CREATESESSION      = 100;
    public static final byte CMD_JOINSESSION        = 101;
    public static final byte CMD_QUITSESSION        = 102;
    public static final byte CMD_QUERYSESSION       = 103;
    public static final byte CMD_PING               = 104;
    public static final byte CMD_FORWARD            = 125;
    public static final byte CMD_FORWARDLOBBYSYNC   = 126;
    public static final byte CMD_FORWARDHOSTINFO    = 127;
    public static final byte CMD_AUTOJOIN           =  23;

    public static final byte ACK_SESSIONCREATED      = 50;
    public static final byte ACK_SESSIONJOINED       = 51;
    public static final byte ACK_QUERYANSWER         = 52;
    public static final byte ACK_SESSIONQUITSUCCESS  = 53;
    public static final byte ACK_PINGRESULT          = 54;
    public static final byte ACK_SESSIONNOTFOUND     = 61;
    public static final byte ACK_SESSIONFULL         = 62;
    public static final byte ACK_SESSIONSECRETWRONG  = 63;

    public static final byte MSG_SESSIONTERMINATED   = 71;

    public static final byte ANS_FORWARDLOBBYSYNC    = 81;

    private static final SimpleLog _log = new SimpleLog();
    private static final Random _random = new Random(System.currentTimeMillis());

    public static HashMap<Short, GameSession> Sessions;

    public static byte[] receiveData   = new byte[PACKAGE_SIZE];
    public static byte[] sendDataSmall = new byte[PACKAGE_SIZE_SMALL];
    public static DatagramSocket Socket;

    public static Short sid = 0;
    public static byte msgIdx = 0;

    public static long lastStateOuput;

    public static String logfileError = "";
    public static String stateFile = "";

    public static void main(String[] args) throws Exception {

        try  {
            run();
        } catch (Exception e) {
            _log.FatalError("Fatal Error in run:\n" + e.toString());
        } finally {
            _log.PersistantInfo("Server stopped");
        }

    }

    private static void run() throws Exception {

        Socket = new DatagramSocket(PORT_SERVER);
        Sessions = new HashMap<>();

        Socket.setSoTimeout(40 * 1000);

        try {
            Properties properties = new Properties();
            properties.load(new FileInputStream("gdproxy.properties"));

            logfileError = properties.getProperty("logfile_error");
            stateFile    = properties.getProperty("statefile");
        } catch (FileNotFoundException e) {
            _log.Error(e.toString());
        }

        _log.PersistantInfo("Server started");
        _log.PersistantInfo("Logfile:  " + logfileError);
        _log.PersistantInfo("StateFile: " + stateFile);

        OutputState();

        while(true)
        {
            try {

                // FORWARD =   [8: CMD] [8:seq] [16: SessionID] [4: UserID] [12: SessionSecret] [440: Payload]
                // CREATE  =   [8: CMD] [8:seq] [4: SessionSize]
                // JOIN    =   [8: CMD] [8:seq] [16: SessionID] [12: SessionSecret]
                // QUIT    =   [8: CMD] [8:seq] [16: SessionID] [12: SessionSecret]
                // QUERY   =   [8: CMD] [8:seq] [16: SessionID] [4: UserID] [12: SessionSecret]
                DatagramPacket receivePacket = new DatagramPacket(receiveData, PACKAGE_SIZE);

                try {
                    Socket.receive(receivePacket);
                } catch (SocketTimeoutException e) {
                    _log.Debug("Server idle (recieve timeout)");
                    CleanUpSessions();
                    if (System.currentTimeMillis() - lastStateOuput > STATE_OUTPUT_FREQ) OutputState();
                    continue;
                }

                switch (receiveData[0]) {
                    case CMD_CREATESESSION:
                        byte create_seq = receiveData[1];
                        int create_sessionsize = (receiveData[2] & 0x0F);
                        CreateNewSession(receivePacket.getAddress(), receivePacket.getPort(), create_seq, create_sessionsize);
                        break;
                    case CMD_JOINSESSION:
                        byte join_seq = receiveData[1];
                        int join_sessionid = (((receiveData[2]&0xFF)<<8) | (receiveData[3]&0xFF));
                        int join_sessionsecret = ((((receiveData[4]&0xFF)<<8) | (receiveData[5]&0xFF)) & 0x0FFF);
                        JoinExistingSession(receivePacket.getAddress(), receivePacket.getPort(), join_seq, join_sessionid, join_sessionsecret);
                        break;
                    case CMD_AUTOJOIN:
                        byte ajoin_seq = receiveData[1];
                        AutoJoinSession(receivePacket.getAddress(), receivePacket.getPort(), ajoin_seq);
                        break;
                    case CMD_QUITSESSION:
                        byte quit_seq = receiveData[1];
                        int quit_sessionid = (((receiveData[2]&0xFF)<<8) | (receiveData[3]&0xFF));
                        int quit_sessionsecret = ((((receiveData[4]&0xFF)<<8) | (receiveData[5]&0xFF)) & 0x0FFF);
                        int quit_userid = (receiveData[6] & 0xFF);
                        QuitSession(receivePacket.getAddress(), receivePacket.getPort(), quit_seq, quit_sessionid, quit_sessionsecret, quit_userid);
                        break;
                    case CMD_FORWARD:
                        byte fwd1_seq = receiveData[1];
                        int fwd1_sessionid = (((receiveData[2]&0xFF)<<8) | (receiveData[3]&0xFF));
                        int fwd1_sessionsecret = ((((receiveData[4]&0xFF)<<8) | (receiveData[5]&0xFF)) & 0x0FFF);
                        int fwd1_userid = ((receiveData[4] & 0xF0) >> 4);
                        if (fwd1_userid == 0)
                            ForwardToClients(receivePacket.getAddress(), receivePacket.getPort(), receivePacket.getLength(), fwd1_seq, fwd1_sessionid, fwd1_sessionsecret, true);
                        else
                            ForwardToServer(receivePacket.getAddress(), receivePacket.getPort(), receivePacket.getLength(), fwd1_seq, fwd1_userid, fwd1_sessionid, fwd1_sessionsecret);

                        //_log.Debug("Forward from "+fwd1_userid+" (cmd="+receiveData[0]+")");
                        break;
                    case CMD_FORWARDLOBBYSYNC:
                        byte fwd2_seq = receiveData[1];
                        int fwd2_sessionid = (((receiveData[2]&0xFF)<<8) | (receiveData[3]&0xFF));
                        int fwd2_sessionsecret = ((((receiveData[4]&0xFF)<<8) | (receiveData[5]&0xFF)) & 0x0FFF);
                        int fwd2_userid = ((receiveData[4] & 0xF0) >> 4);
                        ForwardToClients(receivePacket.getAddress(), receivePacket.getPort(), receivePacket.getLength(), fwd2_seq, fwd2_sessionid, fwd2_sessionsecret, false);

                        _log.Debug("ForwardLobbySync from "+fwd2_userid+" (cmd="+receiveData[0]+")");
                        break;
                    case ANS_FORWARDLOBBYSYNC:
                        byte fwd3_seq = receiveData[1];
                        int fwd3_sessionid = (((receiveData[2]&0xFF)<<8) | (receiveData[3]&0xFF));
                        int fwd3_sessionsecret = ((((receiveData[4]&0xFF)<<8) | (receiveData[5]&0xFF)) & 0x0FFF);
                        int fwd3_userid = ((receiveData[4] & 0xF0) >> 4);
                        ForwardToServer(receivePacket.getAddress(), receivePacket.getPort(), receivePacket.getLength(), fwd3_seq, fwd3_userid, fwd3_sessionid, fwd3_sessionsecret);

                        _log.Debug("AnswerLobbySync from "+fwd3_userid+" (cmd="+receiveData[0]+")");
                        break;
                    case CMD_FORWARDHOSTINFO:
                        byte fwd4_seq = receiveData[1];
                        int fwd4_sessionid = (((receiveData[2]&0xFF)<<8) | (receiveData[3]&0xFF));
                        int fwd4_sessionsecret = ((((receiveData[4]&0xFF)<<8) | (receiveData[5]&0xFF)) & 0x0FFF);
                        int fwd4_userid = ((receiveData[4] & 0xF0) >> 4);
                        ForwardToClients(receivePacket.getAddress(), receivePacket.getPort(), receivePacket.getLength(), fwd4_seq, fwd4_sessionid, fwd4_sessionsecret, false);

                        _log.Debug("ForwardHostInfo from "+fwd4_userid+" (cmd="+receiveData[0]+")");
                        break;
                    case CMD_QUERYSESSION:
                        byte query_seq = receiveData[1];
                        int query_sessionid = (((receiveData[2]&0xFF)<<8) | (receiveData[3]&0xFF));
                        int query_sessionsecret = ((((receiveData[4]&0xFF)<<8) | (receiveData[5]&0xFF)) & 0x0FFF);
                        int query_userid = ((receiveData[4] & 0xF0) >> 4);
                        QuerySession(receivePacket.getAddress(), receivePacket.getPort(), query_seq, query_sessionid, query_sessionsecret, query_userid);
                        break;
                    case CMD_PING:
                        byte ping_seq = receiveData[1];
                        Ping(receivePacket.getAddress(), receivePacket.getPort(), ping_seq);
                        break;
                    default:
                        _log.Error("Unknown Command: " + receiveData[0]);
                }

                CleanUpSessions();

                if (System.currentTimeMillis() - lastStateOuput > STATE_OUTPUT_FREQ) OutputState();

            } catch (Exception e) {
                _log.Error("Error in mainloop:\n" + e.toString());
            }
        }
    }

    private static void CreateNewSession(InetAddress host, int port, byte seq, int size) throws IOException {
        sid++;

        GameSession session = new GameSession(sid & 0xFFFF, _random.nextInt() & 0x0FFF, size & 0xFF);

        session.AddHostUser(host, port);

        Sessions.put(sid, session);

        _log.Info("Session " + session.SessionID + ":["  + session.SessionSecret + "] created for " + host.getHostAddress() + " : " + port + "  (Capacity="+session.MaxSize + ")");

        // [8: RET] [8:seq] [16: SessionID] [4: _ ] [12: SessionSecret] [8: Size]
        sendDataSmall[0] = ACK_SESSIONCREATED;
        sendDataSmall[1] = seq;
        sendDataSmall[2] = (byte)((session.SessionID >> 8) & 0xFF);
        sendDataSmall[3] = (byte)((session.SessionID) & 0xFF);
        sendDataSmall[4] = (byte)((session.SessionSecret >> 8) & 0xFF);
        sendDataSmall[5] = (byte)((session.SessionSecret) & 0xFF);
        sendDataSmall[6] = (byte)session.MaxSize;


        DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
        Socket.send(sendPacket);

        OutputState();
    }

    private static void JoinExistingSession(InetAddress host, int port, byte seq, int sessionid, int sessionsecret) throws IOException {
        GameSession session = Sessions.getOrDefault((short)sessionid, null);

        if (session == null) {
            sendDataSmall[0] = ACK_SESSIONNOTFOUND;
            sendDataSmall[1] = seq;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried joining non-existant session " + sessionid);

            return;
        }

        if ((session.SessionSecret & 0x0FFF) != (sessionsecret & 0x0FFF)) {
            sendDataSmall[0] = ACK_SESSIONSECRETWRONG;
            sendDataSmall[1] = seq;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried joining session " + sessionid + " with wrong secret (real{" + session.SessionSecret + "} != message{" + sessionsecret + "})");

            return;
        }

        if (session.CountUsers() >= session.MaxSize) {
            sendDataSmall[0] = ACK_SESSIONFULL;
            sendDataSmall[1] = seq;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried joining full session " + sessionid + "(capacity=" + session.MaxSize + ")");

            return;
        }

        GameSessionUser user = session.FindUser(host, port);
        if (user != null) {
            // ======== already joined ========

            session.SetLastActivity(user.UserID);

            // [8: RET] [8:seq] [16: SessionID]  [12: SessionSecret] [4: _ ] [4: UserID] [8: Size]
            sendDataSmall[0] = ACK_SESSIONJOINED;
            sendDataSmall[1] = seq;
            sendDataSmall[2] = (byte)((session.SessionID >> 8) & 0xFF);
            sendDataSmall[3] = (byte)((session.SessionID) & 0xFF);
            sendDataSmall[4] = (byte)((session.SessionSecret >> 8) & 0xFF);
            sendDataSmall[5] = (byte)((session.SessionSecret) & 0xFF);
            sendDataSmall[6] = (byte)user.UserID;
            sendDataSmall[7] = (byte)session.MaxSize;

            _log.Info("User " + host.getHostAddress() + " : " + port + " double-joined session " + sessionid + "(uid="+user.UserID+";  capacity=" + session.MaxSize + ")");

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);
        }

        user = session.AddClientUser(host, port);

        session.SetLastActivity(user.UserID);

        // [8: RET] [8:seq] [16: SessionID]  [12: SessionSecret] [4: _ ] [4: UserID] [8: Size]
        sendDataSmall[0] = ACK_SESSIONJOINED;
        sendDataSmall[1] = seq;
        sendDataSmall[2] = (byte)((session.SessionID >> 8) & 0xFF);
        sendDataSmall[3] = (byte)((session.SessionID) & 0xFF);
        sendDataSmall[4] = (byte)((session.SessionSecret >> 8) & 0xFF);
        sendDataSmall[5] = (byte)((session.SessionSecret) & 0xFF);
        sendDataSmall[6] = (byte)user.UserID;
        sendDataSmall[7] = (byte)session.MaxSize;

        _log.Info("User " + host.getHostAddress() + " : " + port + " joined session " + sessionid + "(uid="+user.UserID+";  capacity=" + session.MaxSize + ")");

        DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
        Socket.send(sendPacket);
    }

    private static void AutoJoinSession(InetAddress host, int port, byte seq) throws IOException {

        GameSession session = null;

        for (Map.Entry<Short, GameSession> mentry : Sessions.entrySet()) {
            GameSession entry = mentry.getValue();
            if (entry.isFull() || entry.GameActive) continue;

            if (System.currentTimeMillis() - entry.LastActivity < 1500) session = entry;
        }

        if (session == null) {
            sendDataSmall[0] = ACK_SESSIONNOTFOUND;
            sendDataSmall[1] = seq;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            return;
        }

        GameSessionUser user = session.FindUser(host, port);
        if (user != null) {
            // ======== already joined ========

            session.SetLastActivity(user.UserID);

            // [8: RET] [8:seq] [16: SessionID]  [12: SessionSecret] [4: _ ] [4: UserID] [8: Size]
            sendDataSmall[0] = ACK_SESSIONJOINED;
            sendDataSmall[1] = seq;
            sendDataSmall[2] = (byte)((session.SessionID >> 8) & 0xFF);
            sendDataSmall[3] = (byte)((session.SessionID) & 0xFF);
            sendDataSmall[4] = (byte)((session.SessionSecret >> 8) & 0xFF);
            sendDataSmall[5] = (byte)((session.SessionSecret) & 0xFF);
            sendDataSmall[6] = (byte)user.UserID;
            sendDataSmall[7] = (byte)session.MaxSize;

            _log.Info("User " + host.getHostAddress() + " : " + port + " double-joined session " + session.SessionID + "(uid="+user.UserID+";  capacity=" + session.MaxSize + ")");

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);
        }

        user = session.AddClientUser(host, port);

        session.SetLastActivity(user.UserID);

        // [8: RET] [8:seq] [16: SessionID]  [12: SessionSecret] [4: _ ] [4: UserID] [8: Size]
        sendDataSmall[0] = ACK_SESSIONJOINED;
        sendDataSmall[1] = seq;
        sendDataSmall[2] = (byte)((session.SessionID >> 8) & 0xFF);
        sendDataSmall[3] = (byte)((session.SessionID) & 0xFF);
        sendDataSmall[4] = (byte)((session.SessionSecret >> 8) & 0xFF);
        sendDataSmall[5] = (byte)((session.SessionSecret) & 0xFF);
        sendDataSmall[6] = (byte)user.UserID;
        sendDataSmall[7] = (byte)session.MaxSize;

        _log.Info("User " + host.getHostAddress() + " : " + port + " joined session " + session.SessionID + "(uid="+user.UserID+";  capacity=" + session.MaxSize + ")");

        DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
        Socket.send(sendPacket);
    }

    private static void QuerySession(InetAddress host, int port, byte seq, int sessionid, int sessionsecret, int uid) throws IOException {
        GameSession session = Sessions.getOrDefault((short)sessionid, null);

        if (session == null) {
            sendDataSmall[0] = ACK_SESSIONNOTFOUND;
            sendDataSmall[1] = seq;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried querying non-existant session " + sessionid);

            return;
        }

        if ((session.SessionSecret & 0x0FFF) != (sessionsecret & 0x0FFF)) {
            sendDataSmall[0] = ACK_SESSIONSECRETWRONG;
            sendDataSmall[1] = seq;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried querying session " + sessionid + " with wrong secret (real{" + session.SessionSecret + "} != message{" + sessionsecret + "})");

            return;
        }

        session.SetLastActivity(uid);

        boolean userInSession = (session.FindUser(uid, host, port) != null);

        // [8: RET] [16: SessionID] [8: SessionContainsUser ] [8: UserID] [8: SessionSize] [8: SessionCapacity]
        sendDataSmall[0] = ACK_QUERYANSWER;
        sendDataSmall[1] = seq;
        sendDataSmall[2] = (byte)((session.SessionID >> 8) & 0xFF);
        sendDataSmall[3] = (byte)((session.SessionID) & 0xFF);
        sendDataSmall[4] = (byte)(userInSession?1:0);
        sendDataSmall[5] = (byte)uid;
        sendDataSmall[6] = (byte)session.CountUsers();
        sendDataSmall[7] = (byte)session.MaxSize;

        _log.Debug("User " + host.getHostAddress() + " : " + port + " queried session " + sessionid);

        DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
        Socket.send(sendPacket);
    }

    private static void QuitSession(InetAddress host, int port, byte seq, int sessionid, int sessionsecret, int uid) throws IOException {
        GameSession session = Sessions.getOrDefault((short)sessionid, null);

        if (session == null) {

            // ======= Already killed =========

            // [8: RET] [16: SessionID]
            sendDataSmall[0] = ACK_SESSIONQUITSUCCESS;
            sendDataSmall[1] = seq;
            sendDataSmall[2] = (byte)((sessionid >> 8) & 0xFF);
            sendDataSmall[3] = (byte)((sessionid) & 0xFF);

            _log.Info("Session " + sessionid + ":["  + sessionsecret + "] double-terminating by " + host.getHostAddress() + " : " + port);

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            return;
        }

        if ((session.SessionSecret & 0x0FFF) != (sessionsecret & 0x0FFF)) {
            sendDataSmall[0] = ACK_SESSIONSECRETWRONG;
            sendDataSmall[1] = seq;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried terminating session " + sessionid + " with wrong secret (real{" + session.SessionSecret + "} != message{" + sessionsecret + "})");

            return;
        }

        Sessions.remove((short)session.SessionID);

        _log.Info("Session " + session.SessionID + ":["  + session.SessionSecret + "] terminating by " + host.getHostAddress() + " : " + port);

        for (int i = 0; i < session.MaxSize; i++) {
            if (session.Users[i] == null) continue;

            // [8: RET] [8:seq] [16: SessionID] [8: UserID]
            sendDataSmall[0] = MSG_SESSIONTERMINATED;
            sendDataSmall[1] = msgIdx++;
            sendDataSmall[2] = (byte)((session.SessionID >> 8) & 0xFF);
            sendDataSmall[3] = (byte)((session.SessionID) & 0xFF);
            sendDataSmall[4] = (byte)uid;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, session.Users[i].Adress, session.Users[i].Port);
            Socket.send(sendPacket);
        }

        // [8: RET] [8:seq] [16: SessionID]
        sendDataSmall[0] = ACK_SESSIONQUITSUCCESS;
        sendDataSmall[1] = seq;
        sendDataSmall[2] = (byte)((session.SessionID >> 8) & 0xFF);
        sendDataSmall[3] = (byte)((session.SessionID) & 0xFF);

        DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
        Socket.send(sendPacket);
    }

    private static void ForwardToServer(InetAddress host, int port, int len, byte seq, int userid, int sessionid, int sessionsecret) throws IOException {
        GameSession session = Sessions.getOrDefault((short)sessionid, null);

        if (session == null) {
            sendDataSmall[0] = ACK_SESSIONNOTFOUND;
            sendDataSmall[1] = seq;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried joining non-existant session " + sessionid);

            return;
        }

        if ((session.SessionSecret & 0x0FFF) != (sessionsecret & 0x0FFF)) {
            sendDataSmall[0] = ACK_SESSIONSECRETWRONG;
            sendDataSmall[1] = seq;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried querying session " + sessionid + " with wrong secret (real{" + session.SessionSecret + "} != message{" + sessionsecret + "})");

            return;
        }

        session.SetLastActivity(userid);

        DatagramPacket sendPacket = new DatagramPacket(receiveData, len, session.Users[0].Adress, session.Users[0].Port);
        Socket.send(sendPacket);
    }

    private static void ForwardToClients(InetAddress host, int port, int len, byte seq, int sessionid, int sessionsecret, boolean setActive) throws IOException {
        GameSession session = Sessions.getOrDefault((short)sessionid, null);

        if (session == null) {
            sendDataSmall[0] = ACK_SESSIONNOTFOUND;
            sendDataSmall[1] = seq;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried joining non-existant session " + sessionid);

            return;
        }

        if ((session.SessionSecret & 0x0FFF) != (sessionsecret & 0x0FFF)) {
            sendDataSmall[0] = ACK_SESSIONSECRETWRONG;
            sendDataSmall[1] = seq;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried querying session " + sessionid + " with wrong secret (real{" + session.SessionSecret + "} != message{" + sessionsecret + "})");

            return;
        }

        if (setActive) session.GameActive = true;
        session.SetLastActivity(0);

        for (int i = 1; i < session.MaxSize; i++) {
            if (session.Users[i] == null) continue;
            DatagramPacket sendPacket = new DatagramPacket(receiveData, len, session.Users[i].Adress, session.Users[i].Port);
            Socket.send(sendPacket);
        }

    }

    private static void Ping(InetAddress host, int port, byte seq) throws IOException {

        // [8: RET]
        sendDataSmall[0] = ACK_PINGRESULT;
        sendDataSmall[1] = seq;

        _log.Debug("Ping " + host.getHostAddress() + " : " + port);

        DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
        Socket.send(sendPacket);
    }

    private static void CleanUpSessions() {
        for(Iterator<Map.Entry<Short, GameSession>> it = Sessions.entrySet().iterator(); it.hasNext(); ) {
            Map.Entry<Short, GameSession> entry = it.next();

            GameSession session = entry.getValue();
            long delta = System.currentTimeMillis() - session.LastActivity;

            if(delta > GAME_SESSION_TIMEOUT) {
                _log.Warn("Session terminated by timeout: " + session.SessionID + "  (Usercount=" + session.CountUsers() + ")");
                it.remove();
            } else {
                for (int i = 1; i < session.MaxSize; i++) {
                    if (session.Users[i] == null) continue;
                    long delta2 = System.currentTimeMillis() - session.Users[i].LastActivity;
                    if(delta2 > GAME_USER_TIMEOUT) {
                        _log.Warn("User terminated by timeout: (session=" + session.SessionID + " user=" + i + ")");
                        session.Users[i] = null;
                    }
                }
            }
        }
    }

    private static void OutputState() throws FileNotFoundException {
        lastStateOuput = System.currentTimeMillis();

        if (stateFile.length() == 0) return;

        StringBuilder builder = new StringBuilder();
        builder.append("{\"sessions\":[");
        boolean f = true;
        for (Map.Entry<Short, GameSession> mentry : Sessions.entrySet()) {
            GameSession e = mentry.getValue();

            if (!f) builder.append(",");
            builder.append("{\"sid\":"+e.SessionID+",\"sec\":"+e.SessionSecret+",\"usr\":"+e.CountUsers()+",\"cap\":"+e.MaxSize+"}");
            f = false;
        }
        builder.append("]}");

        try (PrintWriter out = new PrintWriter(stateFile)) { out.write(builder.toString()); }
    }
}
