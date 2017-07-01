package de.samdev.griddominance.server;

import java.net.InetAddress;
import java.util.ArrayList;
import java.util.List;

public class GameSession {

    public int SessionID;
    public int SessionSecret;
    public long LastActivity;
    public int MaxSize;

    public List<InetAddress> UserAddr = new ArrayList<>();
    public List<Integer> UserPorts = new ArrayList<>();

}
