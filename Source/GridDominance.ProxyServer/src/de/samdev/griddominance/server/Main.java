package de.samdev.griddominance.server;

import java.io.IOException;
import java.net.*;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;
import java.util.Random;

public class Main {

    public static final int PORT_SERVER = 28023;
    public static final int PACKAGE_SIZE = 448;
    public static final int PACKAGE_SIZE_SMALL = 32;

    public static final int GAME_SESSION_TIMEOUT = 90 * 1000;

    public static final byte CMD_CREATESESSION = 100;
    public static final byte CMD_JOINSESSION   = 101;
    public static final byte CMD_QUITSESSION   = 102;
    public static final byte CMD_QUERYSESSION  = 103;
    public static final byte CMD_PING          = 104;
    public static final byte CMD_FORWARD       = 125;

    public static final byte RET_SESSIONCREATED     = 50;
    public static final byte RET_SESSIONJOINED      = 51;
    public static final byte RET_QUERYANSWER        = 52;
    public static final byte RET_SESSIONQUITSUCCESS = 53;
    public static final byte RET_PINGRESULT         = 54;
    public static final byte RET_SESSIONNOTFOUND    = 61;
    public static final byte RET_SESSIONFULL        = 62;
    public static final byte RET_SESSIONSECRETWRONG = 63;
    public static final byte RET_SESSIONTERMINATED  = 71;

    private static final SimpleLog _log = new SimpleLog("proxyserver.log");
    private static final Random _random = new Random(System.currentTimeMillis());

    public static HashMap<Short, GameSession> Sessions;

    public static byte[] receiveData   = new byte[PACKAGE_SIZE];
    public static byte[] sendDataSmall = new byte[PACKAGE_SIZE_SMALL];
    public static DatagramSocket Socket;

    public static Short sid = 0;

    public static void main(String[] args) {

        try  {

            run();
        } catch (Exception e) {
            _log.FatalError("Fatal Error in run:\n" + e.toString());
        }

    }

    private static void run() throws SocketException {

        Socket = new DatagramSocket(PORT_SERVER);
        Sessions = new HashMap<>();

        Socket.setSoTimeout(40 * 1000);

        _log.Debug("Server started");

        while(true)
        {
            try {

                // FORWARD =   [8: CMD] [16: SessionID] [4: UserID] [12: SessionSecret] [24: _] [440: Payload]
                // CREATE  =   [8: CMD] [4: SessionSize]
                // JOIN    =   [8: CMD] [16: SessionID] [12: SessionSecret]
                // QUIT    =   [8: CMD] [16: SessionID] [12: SessionSecret]
                // QUERY   =   [8: CMD] [16: SessionID] [4: UserID] [12: SessionSecret]
                DatagramPacket receivePacket = new DatagramPacket(receiveData, PACKAGE_SIZE);

                try {
                    Socket.receive(receivePacket);
                } catch (SocketTimeoutException e) {
                    _log.Debug("Server idle (recieve timeout)");
                    CleanUpSessions();
                    continue;
                }

                switch (receiveData[0]) {
                    case CMD_CREATESESSION:
                        int create_sessionsize = (receiveData[1] & 0x0F);
                        CreateNewSession(receivePacket.getAddress(), receivePacket.getPort(), create_sessionsize);
                        break;
                    case CMD_JOINSESSION:
                        int join_sessionid = (((receiveData[1]&0xFF)<<8) | (receiveData[2]&0xFF));
                        int join_sessionsecret = ((((receiveData[3]&0xFF)<<8) | (receiveData[4]&0xFF)) & 0x0FFF);
                        JoinExistingSession(receivePacket.getAddress(), receivePacket.getPort(), join_sessionid, join_sessionsecret);
                        break;
                    case CMD_QUITSESSION:
                        int quit_sessionid = (((receiveData[1]&0xFF)<<8) | (receiveData[2]&0xFF));
                        int quit_sessionsecret = ((((receiveData[3]&0xFF)<<8) | (receiveData[4]&0xFF)) & 0x0FFF);
                        QuitSession(receivePacket.getAddress(), receivePacket.getPort(), quit_sessionid, quit_sessionsecret);
                        break;
                    case CMD_FORWARD:
                        int fwd_sessionid = (((receiveData[1]&0xFF)<<8) | (receiveData[2]&0xFF));
                        int fwd_sessionsecret = ((((receiveData[3]&0xFF)<<8) | (receiveData[4]&0xFF)) & 0x0FFF);
                        int fwd_userid = ((receiveData[3] & 0xF0) >> 4);
                        if (fwd_userid == 0)
                            ForwardToClients(receivePacket.getAddress(), receivePacket.getPort(), fwd_sessionid, fwd_sessionsecret);
                        else
                            ForwardToServer(receivePacket.getAddress(), receivePacket.getPort(), fwd_sessionid, fwd_sessionsecret);
                        break;
                    case CMD_QUERYSESSION:
                        int query_sessionid = (((receiveData[1]&0xFF)<<8) | (receiveData[2]&0xFF));
                        int query_sessionsecret = ((((receiveData[3]&0xFF)<<8) | (receiveData[4]&0xFF)) & 0x0FFF);
                        int query_userid = ((receiveData[3] & 0xF0) >> 4);
                        QuerySession(receivePacket.getAddress(), receivePacket.getPort(), query_sessionid, query_sessionsecret, query_userid);
                        break;
                    case CMD_PING:
                        Ping(receivePacket.getAddress(), receivePacket.getPort());
                        break;
                    default:
                        _log.Error("Unknown Command: " + receiveData[0]);
                }
            } catch (Exception e) {
                _log.Error("Error in mainloop:\n" + e.toString());
            }
        }
    }

    private static void CreateNewSession(InetAddress host, int port, int size) throws IOException {
        sid++;

        GameSession session = new GameSession();
        session.LastActivity = System.currentTimeMillis();
        session.MaxSize = size & 0xFF;
        session.SessionID = sid & 0xFFFF;
        session.SessionSecret = (short)(_random.nextInt() & 0x0FFF);
        session.UserAddr.add(host);
        session.UserPorts.add(port);

        Sessions.put(sid, session);

        _log.Info("Session " + session.SessionID + ":["  + session.SessionSecret + "] created for " + host.getHostAddress() + " : " + port + "  (Capacity="+session.MaxSize + ")");

        // [8: RET] [16: SessionID] [4: _ ] [12: SessionSecret] [8: Size]
        sendDataSmall[0] = RET_SESSIONCREATED;
        sendDataSmall[1] = (byte)((session.SessionID >> 8) & 0xFF);
        sendDataSmall[2] = (byte)((session.SessionID) & 0xFF);
        sendDataSmall[3] = (byte)((session.SessionSecret >> 8) & 0xFF);
        sendDataSmall[4] = (byte)((session.SessionSecret) & 0xFF);
        sendDataSmall[5] = (byte)session.MaxSize;


        DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
        Socket.send(sendPacket);
    }

    private static void JoinExistingSession(InetAddress host, int port, int sessionid, int sessionsecret) throws IOException {

        GameSession session = Sessions.getOrDefault((short)sessionid, null);

        if (session == null) {
            sendDataSmall[0] = RET_SESSIONNOTFOUND;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried joining non-existant session " + sessionid);

            return;
        }

        if ((session.SessionSecret & 0x0FFF) != (sessionsecret & 0x0FFF)) {
            sendDataSmall[0] = RET_SESSIONSECRETWRONG;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried joining session " + sessionid + " with wrong secret (real{" + session.SessionSecret + "} != message{" + sessionsecret + "})");

            return;
        }

        if (session.UserAddr.size() >= session.MaxSize) {
            sendDataSmall[0] = RET_SESSIONFULL;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried joining full session " + sessionid + "(capacity=" + session.MaxSize + ")");

            return;
        }

        session.LastActivity = System.currentTimeMillis();


        for (int i=0; i < session.UserAddr.size(); i++) {
            if (session.UserAddr.get(i).getHostAddress().equals(host.getHostAddress()) && session.UserPorts.get(i) == port) {

                // ======== already joined ========

                // [8: RET] [16: SessionID] [4: _ ] [4: UserID] [8: SessionSize]
                sendDataSmall[0] = RET_SESSIONJOINED;
                sendDataSmall[1] = (byte)((session.SessionID >> 8) & 0xFF);
                sendDataSmall[2] = (byte)((session.SessionID) & 0xFF);
                sendDataSmall[3] = (byte)i;
                sendDataSmall[4] = (byte)session.UserAddr.size();

                _log.Info("User " + host.getHostAddress() + " : " + port + " double-joined session " + sessionid + "(uid="+i+";  capacity=" + session.MaxSize + ")");

                DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
                Socket.send(sendPacket);
            }
        }

        byte uid = (byte)session.UserAddr.size();
        session.UserAddr.add(host);
        session.UserPorts.add(port);

        // [8: RET] [16: SessionID] [4: _ ] [4: UserID] [8: SessionSize]
        sendDataSmall[0] = RET_SESSIONJOINED;
        sendDataSmall[1] = (byte)((session.SessionID) & 0xFF);
        sendDataSmall[2] = (byte)((session.SessionID >> 8) & 0xFF);
        sendDataSmall[3] = uid;
        sendDataSmall[4] = (byte)session.UserAddr.size();

        _log.Info("User " + host.getHostAddress() + " : " + port + " joined session " + sessionid + "(uid="+uid+";  capacity=" + session.MaxSize + ")");

        DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
        Socket.send(sendPacket);
    }

    private static void QuerySession(InetAddress host, int port, int sessionid, int sessionsecret, int uid) throws IOException {
        GameSession session = Sessions.getOrDefault((short)sessionid, null);

        if (session == null) {
            sendDataSmall[0] = RET_SESSIONNOTFOUND;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried querying non-existant session " + sessionid);

            return;
        }

        if ((session.SessionSecret & 0x0FFF) != (sessionsecret & 0x0FFF)) {
            sendDataSmall[0] = RET_SESSIONSECRETWRONG;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried querying session " + sessionid + " with wrong secret (real{" + session.SessionSecret + "} != message{" + sessionsecret + "})");

            return;
        }

        session.LastActivity = System.currentTimeMillis();

        boolean userInSession = true;
        if (session.UserAddr.size() <= uid)
            userInSession = false;
        if (userInSession && !session.UserAddr.get(uid).getHostAddress().equals(host.getHostAddress()))
            userInSession = false;
        if (userInSession && session.UserPorts.get(uid) != port)
            userInSession = false;


        // [8: RET] [16: SessionID] [8: SessionContainsUser ] [8: UserID] [8: SessionSize] [8: SessionCapacity]
        sendDataSmall[0] = RET_QUERYANSWER;
        sendDataSmall[1] = (byte)((session.SessionID >> 8) & 0xFF);
        sendDataSmall[2] = (byte)((session.SessionID) & 0xFF);
        sendDataSmall[3] = (byte)(userInSession?1:0);
        sendDataSmall[4] = (byte)uid;
        sendDataSmall[5] = (byte)session.UserAddr.size();
        sendDataSmall[6] = (byte)session.MaxSize;

        _log.Debug("User " + host.getHostAddress() + " : " + port + " queried session " + sessionid);

        DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
        Socket.send(sendPacket);
    }

    private static void QuitSession(InetAddress host, int port, int sessionid, int sessionsecret) throws IOException {
        GameSession session = Sessions.getOrDefault((short)sessionid, null);

        if (session == null) {

            // ======= Already killed =========

            // [8: RET] [16: SessionID]
            sendDataSmall[0] = RET_SESSIONQUITSUCCESS;
            sendDataSmall[1] = (byte)((sessionid >> 8) & 0xFF);
            sendDataSmall[2] = (byte)((sessionid) & 0xFF);

            _log.Info("Session " + sessionid + ":["  + sessionsecret + "] double-terminating by " + host.getHostAddress() + " : " + port);

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            return;
        }

        if ((session.SessionSecret & 0x0FFF) != (sessionsecret & 0x0FFF)) {
            sendDataSmall[0] = RET_SESSIONSECRETWRONG;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried terminating session " + sessionid + " with wrong secret (real{" + session.SessionSecret + "} != message{" + sessionsecret + "})");

            return;
        }

        Sessions.remove((short)session.SessionID);

        _log.Info("Session " + session.SessionID + ":["  + session.SessionSecret + "] terminating by " + host.getHostAddress() + " : " + port);

        for (int i = 0; i < session.UserAddr.size(); i++) {
            // [8: RET] [16: SessionID] [8: UserID]
            sendDataSmall[0] = RET_SESSIONTERMINATED;
            sendDataSmall[1] = (byte)((session.SessionID >> 8) & 0xFF);
            sendDataSmall[2] = (byte)((session.SessionID) & 0xFF);
            sendDataSmall[3] = (byte)i;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, session.UserAddr.get(i), session.UserPorts.get(i));
            Socket.send(sendPacket);
        }

        // [8: RET] [16: SessionID]
        sendDataSmall[0] = RET_SESSIONQUITSUCCESS;
        sendDataSmall[1] = (byte)((session.SessionID >> 8) & 0xFF);
        sendDataSmall[2] = (byte)((session.SessionID) & 0xFF);

        DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
        Socket.send(sendPacket);
    }

    private static void ForwardToServer(InetAddress host, int port, int sessionid, int sessionsecret) throws IOException {
        GameSession session = Sessions.getOrDefault((short)sessionid, null);

        if (session == null) {
            sendDataSmall[0] = RET_SESSIONNOTFOUND;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried joining non-existant session " + sessionid);

            return;
        }

        if ((session.SessionSecret & 0x0FFF) != (sessionsecret & 0x0FFF)) {
            sendDataSmall[0] = RET_SESSIONSECRETWRONG;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried querying session " + sessionid + " with wrong secret (real{" + session.SessionSecret + "} != message{" + sessionsecret + "})");

            return;
        }

        session.LastActivity = System.currentTimeMillis();

        DatagramPacket sendPacket = new DatagramPacket(receiveData, PACKAGE_SIZE, session.UserAddr.get(0), session.UserPorts.get(0));
        Socket.send(sendPacket);
    }

    private static void ForwardToClients(InetAddress host, int port, int sessionid, int sessionsecret) throws IOException {
        GameSession session = Sessions.getOrDefault((short)sessionid, null);

        if (session == null) {
            sendDataSmall[0] = RET_SESSIONNOTFOUND;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried joining non-existant session " + sessionid);

            return;
        }

        if ((session.SessionSecret & 0x0FFF) != (sessionsecret & 0x0FFF)) {
            sendDataSmall[0] = RET_SESSIONSECRETWRONG;

            DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
            Socket.send(sendPacket);

            _log.Info("User " + host.getHostAddress() + " : " + port + " tried querying session " + sessionid + " with wrong secret (real{" + session.SessionSecret + "} != message{" + sessionsecret + "})");

            return;
        }

        session.LastActivity = System.currentTimeMillis();

        for (int i = 1; i < session.UserAddr.size(); i++) {
            DatagramPacket sendPacket = new DatagramPacket(receiveData, PACKAGE_SIZE, session.UserAddr.get(i), session.UserPorts.get(i));
            Socket.send(sendPacket);
        }

    }

    private static void Ping(InetAddress host, int port) throws IOException {

        // [8: RET]
        sendDataSmall[0] = RET_PINGRESULT;

        _log.Debug("Ping " + host.getHostAddress() + " : " + port);

        DatagramPacket sendPacket = new DatagramPacket(sendDataSmall, PACKAGE_SIZE_SMALL, host, port);
        Socket.send(sendPacket);
    }

    private static void CleanUpSessions() {
        for(Iterator<Map.Entry<Short, GameSession>> it = Sessions.entrySet().iterator(); it.hasNext(); ) {
            Map.Entry<Short, GameSession> entry = it.next();

            long delta = System.currentTimeMillis() - entry.getValue().LastActivity;

            if(delta > GAME_SESSION_TIMEOUT) {
                _log.Warn("Session terminated by timeout: " + entry.getValue().SessionID + "  (Usercount="+entry.getValue().UserAddr.size()+")");
                it.remove();
            }
        }
    }
}
