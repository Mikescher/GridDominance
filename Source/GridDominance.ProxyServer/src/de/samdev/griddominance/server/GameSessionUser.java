package de.samdev.griddominance.server;

import java.net.InetAddress;

public class GameSessionUser {

    public final int UserID;
    public final InetAddress Adress;
    public final int Port;
    public long LastActivity;

    public GameSessionUser(int i, InetAddress a, int p) {
        UserID = i;
        Adress = a;
        Port = p;

        LastActivity = System.currentTimeMillis();
    }

}
