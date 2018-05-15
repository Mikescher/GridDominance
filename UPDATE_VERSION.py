#!/usr/bin/env python3


import re
import os
from shutil import move
from os.path import basename


def repl(path, regex, repl):
    with open(path) as f:
        out_fname = path + ".tmp"
        out = open(out_fname, "w")

        content = f.read()

        m = re.search(regex, content)
        if m:
            print(("[" + path + "]").ljust(75, ' ') + " " + str(m.group('repl')).rjust(10, ' ') + " -> " + repl.ljust(10, ' '))

            ss = m.start('repl')
            ee = m.end('repl')

            g2 = content[:ss] + repl + content[ee:]

            out.write(g2)
        else:
            print()
            print('ERROR: Regex not found')
            print(path)
            print(regex)
            print()
            out.write(content)

        out.close()
        move(out_fname, path)


version_dot3 = input("New Version (a.b.c): ")
version_dot4 = version_dot3 + ".0"
version_com4 = version_dot4.replace(".", ",")

version_int = 0

for i in version_dot3.split('.'):
    version_int = version_int*100 + int(i)

version_int = str(version_int)

repl(r"Source\GridDominance.Core\Resources\GDConstants.cs", r"Version = new Version\((?P<repl>[0-9,\s]+)\)", version_com4)
repl(r"Source\GridDominance.Common\Properties\AssemblyInfo.cs", "AssemblyVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)
repl(r"Source\GridDominance.Common\Properties\AssemblyInfo.cs", "AssemblyFileVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)

repl(r"Source\GridDominance.Android.Full\Properties\AndroidManifest.xml", "android:versionCode=\"(?P<repl>[0-9]+)\"", version_int)
repl(r"Source\GridDominance.Android.Full\Properties\AndroidManifest.xml", "android:versionName=\"(?P<repl>[^\"]+)\"", version_dot3)

repl(r"Source\GridDominance.Android.IAB\Properties\AndroidManifest.xml", "android:versionCode=\"(?P<repl>[0-9]+)\"", version_int)
repl(r"Source\GridDominance.Android.IAB\Properties\AndroidManifest.xml", "android:versionName=\"(?P<repl>[^\"]+)\"", version_dot3)

repl(r"Source\GridDominance.Android.Amazon\Properties\AndroidManifest.xml", "android:versionCode=\"(?P<repl>[0-9]+)\"", version_int)
repl(r"Source\GridDominance.Android.Amazon\Properties\AndroidManifest.xml", "android:versionName=\"(?P<repl>[^\"]+)\"", version_dot3)

repl(r"Source\GridDominance.OpenGL\Properties\AssemblyInfo.cs", "AssemblyVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)
repl(r"Source\GridDominance.OpenGL\Properties\AssemblyInfo.cs", "AssemblyFileVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)

repl(r"Source\GridDominance.DirectX\Properties\AssemblyInfo.cs", "AssemblyVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)
repl(r"Source\GridDominance.DirectX\Properties\AssemblyInfo.cs", "AssemblyFileVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)

repl(r"Source\GridDominance.iOS.Full\Info.plist", "<key>CFBundleShortVersionString</key>\s*<string>(?P<repl>[0-9\.]+)</string>", version_dot3)
repl(r"Source\GridDominance.iOS.Full\Info.plist", "<key>CFBundleVersion</key>\s*<string>(?P<repl>[0-9\.]+)</string>", version_dot3)
repl(r"Source\GridDominance.iOS.Full\Properties\AssemblyInfo.cs", "AssemblyVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)
repl(r"Source\GridDominance.iOS.Full\Properties\AssemblyInfo.cs", "AssemblyFileVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)

repl(r"Source\GridDominance.iOS.IAB\Info.plist", "<key>CFBundleShortVersionString</key>\s*<string>(?P<repl>[0-9\.]+)</string>", version_dot3)
repl(r"Source\GridDominance.iOS.IAB\Info.plist", "<key>CFBundleVersion</key>\s*<string>(?P<repl>[0-9\.]+)</string>", version_dot3)
repl(r"Source\GridDominance.iOS.IAB\Properties\AssemblyInfo.cs", "AssemblyVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)
repl(r"Source\GridDominance.iOS.IAB\Properties\AssemblyInfo.cs", "AssemblyFileVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)

repl(r"Source\GridDominance.Core\Properties\AssemblyInfo.cs", "AssemblyVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)
repl(r"Source\GridDominance.Core\Properties\AssemblyInfo.cs", "AssemblyFileVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)

repl(r"Source\GridDominance.Server\internals\config_auto.php", "'latest_version'\s+=>\s+'(?P<repl>[0-9\\.]+)'", version_dot4)

repl(r"Source\GridDominance.WinPhone8.Full\Package.appxmanifest", "<Identity[^\\r\\n>]+Version=\"(?P<repl>[0-9\\.]+)\"", version_dot4)
repl(r"Source\GridDominance.WinPhone8.Full\Properties\AssemblyInfo.cs", "AssemblyVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)
repl(r"Source\GridDominance.WinPhone8.Full\Properties\AssemblyInfo.cs", "AssemblyFileVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)

repl(r"Source\GridDominance.UWP.Full\Package.appxmanifest", "<Identity[^\\r\\n>]+Version=\"(?P<repl>[0-9\\.]+)\"", version_dot4)
repl(r"Source\GridDominance.UWP.Full\Properties\AssemblyInfo.cs", "AssemblyVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)
repl(r"Source\GridDominance.UWP.Full\Properties\AssemblyInfo.cs", "AssemblyFileVersion\\(\"(?P<repl>[0-9\\.]+)\"\\)", version_dot4)

input('Press enter to continue')
