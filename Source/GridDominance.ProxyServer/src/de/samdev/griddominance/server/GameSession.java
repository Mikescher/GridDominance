package de.samdev.griddominance.server;

import java.net.InetAddress;
import java.util.ArrayList;
import java.util.List;

public class GameSession {

    public short SessionID;
    public short SessionSecret;
    public long LastActivity;
    public byte MaxSize;

    public List<InetAddress> UserAddr = new ArrayList<>();
    public List<Integer> UserPorts = new ArrayList<>();

}
