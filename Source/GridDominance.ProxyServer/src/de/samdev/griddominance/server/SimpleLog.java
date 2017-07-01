package de.samdev.griddominance.server;

public class SimpleLog {

    public final String Path;

    public SimpleLog(String path) {
        Path = path;
    }

    public void Debug(String msg) {
        System.out.println("[DEBUG] " + msg);
    }

    public void Info(String msg) {
        System.out.println("[INFO] " + msg);
    }

    public void Warn(String msg) {
        System.out.println("[WARN] " + msg);
    }

    public void Error(String msg) {
        System.out.println("[ERROR] " + msg);
    }

    public void FatalError(String msg) {
        System.out.println("[FATAL_ERROR] " + msg);
    }
}
