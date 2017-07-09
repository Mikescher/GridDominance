package de.samdev.griddominance.server;

import java.io.*;
import java.text.SimpleDateFormat;
import java.util.Date;

public class SimpleLog {

    private static SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS");

    public SimpleLog() { }

    public void Debug(String msg) {
        String s = "[DEBUG] " + sdf.format(new Date()) + ": " + msg;
        System.out.println(s);
    }

    public void Info(String msg) {
        String s = "[INFO] " + sdf.format(new Date()) + ": " + msg;
        System.out.println(s);
    }

    public void Warn(String msg) {
        String s = "[WARN] " + sdf.format(new Date()) + ": " + msg;
        System.out.println(s);
    }

    public void PersistantInfo(String msg) {
        String s = "[INFO] " + sdf.format(new Date()) + ": " + msg;
        System.out.println(s);
    }

    public void Error(String msg) throws Exception {
        String s = "[ERROR] " + sdf.format(new Date()) + ": " + msg;
        System.out.println(s);
        FileAppend(s, Main.logfileError);
    }

    public void FatalError(String msg) throws Exception {
        String s = "[FATAL_ERROR] " + sdf.format(new Date()) + ": " + msg;
        System.out.println(s);
        FileAppend(s, Main.logfileError);
    }

    public static void FileAppend(String line, String filename) throws Exception {
        if (filename.length()>0) {
            try(PrintWriter out = new PrintWriter(new BufferedWriter(new FileWriter(filename, true)))) {
                out.println(line);
            }
        }
    }
}
