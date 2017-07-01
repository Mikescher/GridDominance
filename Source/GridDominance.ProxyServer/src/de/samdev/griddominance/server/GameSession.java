package de.samdev.griddominance.server;

import java.net.InetAddress;

public class GameSession {

    public final int SessionID;
    public final int SessionSecret;
    public final int MaxSize;
    public final GameSessionUser Users[];

    public long LastActivity;

    public GameSession(int sid, int ssc, int ssz) {
        SessionID = sid;
        SessionSecret = ssc;
        MaxSize = ssz;

        Users = new GameSessionUser[ssz];

        LastActivity = System.currentTimeMillis();
    }

    public void SetLastActivity(int uid) {
        long now = System.currentTimeMillis();

        if (uid == 0) {
            LastActivity = now;
            Users[0].LastActivity = now;
        } else {
            if (uid > 0 && uid < MaxSize && Users[uid] != null) Users[uid].LastActivity = now;
        }
    }

    public int CountUsers() {
        int c = 0;
        for (int i = 0; i < MaxSize; i++) if (Users[i] != null) c++;
        return c;
    }

    public GameSessionUser AddHostUser(InetAddress host, int port) {
        Users[0] = new GameSessionUser(0, host, port);
        return Users[0];
    }

    public GameSessionUser AddClientUser(InetAddress host, int port) {
        for (int i = 1; i < MaxSize; i++) {
            if (Users[i] == null) {
                Users[i] = new GameSessionUser(i, host, port);
                return Users[i];
            }
        }
        return null;
    }

    public GameSessionUser FindUser(InetAddress host, int port) {
        for (int i = 0; i < MaxSize; i++) {
            if (Users[i] != null && Users[i].Adress.getHostAddress().equals(host.getHostAddress()) && Users[i].Port == port) {
                return Users[i];
            }
        }
        return null;
    }

    public GameSessionUser FindUser(int uid, InetAddress host, int port) {
        for (int i = 0; i < MaxSize; i++) {
            if (Users[i] != null && Users[i].Adress.getHostAddress().equals(host.getHostAddress()) && Users[i].Port == port && Users[i].UserID == uid) {
                return Users[i];
            }
        }
        return null;
    }
}
