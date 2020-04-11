﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.Diagnostics;                // for Debug
using System.Drawing;                    // for Color (add reference to  System.Drawing assembly)
using System.Runtime.InteropServices;    // for StructLayout
using System.Threading;

internal class SetScreenColorsDemo
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct COORD
    {
        internal short X;
        internal short Y;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct SMALL_RECT
    {
        internal short Left;
        internal short Top;
        internal short Right;
        internal short Bottom;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct COLORREF
    {
        internal uint ColorDWORD;

        internal COLORREF(Color color)
        {
            ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
        }

        internal COLORREF(uint r, uint g, uint b)
        {
            ColorDWORD = r + (g << 8) + (b << 16);
        }

        internal Color GetColor()
        {
            return Color.FromArgb((int)(0x000000FFU & ColorDWORD),
               (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);
        }

        internal void SetColor(Color color)
        {
            ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct CONSOLE_SCREEN_BUFFER_INFO_EX
    {
        internal int cbSize;
        internal COORD dwSize;
        internal COORD dwCursorPosition;
        internal ushort wAttributes;
        internal SMALL_RECT srWindow;
        internal COORD dwMaximumWindowSize;
        internal ushort wPopupAttributes;
        internal bool bFullscreenSupported;
        internal COLORREF black;
        internal COLORREF darkBlue;
        internal COLORREF darkGreen;
        internal COLORREF darkCyan;
        internal COLORREF darkRed;
        internal COLORREF darkMagenta;
        internal COLORREF darkYellow;
        internal COLORREF gray;
        internal COLORREF darkGray;
        internal COLORREF blue;
        internal COLORREF green;
        internal COLORREF cyan;
        internal COLORREF red;
        internal COLORREF magenta;
        internal COLORREF yellow;
        internal COLORREF white;
    }

    public struct ColorRGB

    {

        public byte R;

        public byte G;

        public byte B;

        public ColorRGB(Color value)

        {

            this.R = value.R;

            this.G = value.G;

            this.B = value.B;

        }

        public static implicit operator Color(ColorRGB rgb)

        {

            Color c = Color.FromArgb(rgb.R, rgb.G, rgb.B);

            return c;

        }

        public static explicit operator ColorRGB(Color c)

        {

            return new ColorRGB(c);

        }

    }

    const int STD_OUTPUT_HANDLE = -11;                                        // per WinBase.h
    internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);    // per WinBase.h

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);
    
    public static int SetColor(ConsoleColor consoleColor, Color targetColor)
    {
        return SetColor(consoleColor, targetColor.R, targetColor.G, targetColor.B);
    }

    public static int SetColor(ConsoleColor color, uint r, uint g, uint b)
    {
        CONSOLE_SCREEN_BUFFER_INFO_EX csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        csbe.cbSize = (int)Marshal.SizeOf(csbe);                    // 96 = 0x60
        IntPtr hConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);    // 7
        if (hConsoleOutput == INVALID_HANDLE_VALUE)
        {
            return Marshal.GetLastWin32Error();
        }
        bool brc = GetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
        if (!brc)
        {
            return Marshal.GetLastWin32Error();
        }

        switch (color)
        {
            case ConsoleColor.Black:
                csbe.black = new COLORREF(r, g, b);
                break;
            case ConsoleColor.DarkBlue:
                csbe.darkBlue = new COLORREF(r, g, b);
                break;
            case ConsoleColor.DarkGreen:
                csbe.darkGreen = new COLORREF(r, g, b);
                break;
            case ConsoleColor.DarkCyan:
                csbe.darkCyan = new COLORREF(r, g, b);
                break;
            case ConsoleColor.DarkRed:
                csbe.darkRed = new COLORREF(r, g, b);
                break;
            case ConsoleColor.DarkMagenta:
                csbe.darkMagenta = new COLORREF(r, g, b);
                break;
            case ConsoleColor.DarkYellow:
                csbe.darkYellow = new COLORREF(r, g, b);
                break;
            case ConsoleColor.Gray:
                csbe.gray = new COLORREF(r, g, b);
                break;
            case ConsoleColor.DarkGray:
                csbe.darkGray = new COLORREF(r, g, b);
                break;
            case ConsoleColor.Blue:
                csbe.blue = new COLORREF(r, g, b);
                break;
            case ConsoleColor.Green:
                csbe.green = new COLORREF(r, g, b);
                break;
            case ConsoleColor.Cyan:
                csbe.cyan = new COLORREF(r, g, b);
                break;
            case ConsoleColor.Red:
                csbe.red = new COLORREF(r, g, b);
                break;
            case ConsoleColor.Magenta:
                csbe.magenta = new COLORREF(r, g, b);
                break;
            case ConsoleColor.Yellow:
                csbe.yellow = new COLORREF(r, g, b);
                break;
            case ConsoleColor.White:
                csbe.white = new COLORREF(r, g, b);
                break;
        }
        ++csbe.srWindow.Bottom;
        ++csbe.srWindow.Right;
        brc = SetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
        if (!brc)
        {
            return Marshal.GetLastWin32Error();
        }
        return 0;
    }

    public static ColorRGB HSL2RGB(double h, double sl, double l)

    {

        double v;

        double r, g, b;



        r = l;   // default to gray

        g = l;

        b = l;

        v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);

        if (v > 0)

        {

            double m;

            double sv;

            int sextant;

            double fract, vsf, mid1, mid2;



            m = l + l - v;

            sv = (v - m) / v;

            h *= 6.0;

            sextant = (int)h;

            fract = h - sextant;

            vsf = v * sv * fract;

            mid1 = m + vsf;

            mid2 = v - vsf;

            switch (sextant)

            {

                case 0:

                    r = v;

                    g = mid1;

                    b = m;

                    break;

                case 1:

                    r = mid2;

                    g = v;

                    b = m;

                    break;

                case 2:

                    r = m;

                    g = v;

                    b = mid1;

                    break;

                case 3:

                    r = m;

                    g = mid2;

                    b = v;

                    break;

                case 4:

                    r = mid1;

                    g = m;

                    b = v;

                    break;

                case 5:

                    r = v;

                    g = m;

                    b = mid2;

                    break;

            }

        }

        ColorRGB rgb;

        rgb.R = Convert.ToByte(r * 255.0f);

        rgb.G = Convert.ToByte(g * 255.0f);

        rgb.B = Convert.ToByte(b * 255.0f);

        return rgb;

    }
}

namespace GunplayMetaEditor
{
    enum FileType
    {
        Pedaccuracy_meta,
        Peddamage_xml,
        Pedhealth_meta,
        Pickups_meta,
        Scaleformpreallocation_xml,
        Taskdata_meta,
        Wantedtuning_ymt, //cant edit this one
        Weapons_meta,
        Unknown

    }
    class Program
    {
        const bool Debug = true; //|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        private static bool usingRGS;

        static void Main(string[] args)
        {
            
            List<string> arguments = args.ToList();

            foreach (string arg in args)
            {
                switch (arg.ToUpper()) //Read known command-line switches
                {
                    case "/RGS":
                        {
                            usingRGS = true;
                            break;
                        }
                }
                
                if (!File.Exists(arg) && !Directory.Exists(arg)) //Strip switches and nonsense
                {
                    arguments.RemoveAll(n => n.Equals(arg, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (usingRGS) { PrintLine("RGS Values enabled.", ConsoleColor.Blue, 0.0f); }
            

            if (arguments.Count == 0) //Complain if no file was dragged onto exe
            {
                PrintLine("Please supply one or more GTA V data files.", ConsoleColor.Red, 0.0f);
                Console.ReadKey();
                return;
            }
            else
            {
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (File.Exists(arguments[i]))
                    {
                        ModifyFile(arguments[i], false);
                    }
                    else if (Directory.Exists(arguments[i]))
                    {
                        BackupDirectory(arguments[i]);
                        ModifyAllInDirectory(arguments[i]);
                        RemoveEmptySubdirectories(arguments[i]);
                    }
                }

                ShowSuccessMessage();
            }
        }

        public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            //PrintLine("from: " + fromPath, ConsoleColor.Cyan, 0.0f);
            //PrintLine("to: " + toPath, ConsoleColor.Cyan, 0.0f);

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        public static void ModifyAllInDirectory(string source)
        {
            var diSource = new DirectoryInfo(source);
            // Copy each file into the new directory.
            foreach (FileInfo fi in diSource.GetFiles())
            {
                ModifyFile(fi.FullName, true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in diSource.GetDirectories())
            {
                ModifyAllInDirectory(diSourceSubDir.FullName);
            }
        }

        public static void RemoveEmptySubdirectories(string source)
        {
            foreach (string subDir in Directory.GetDirectories(source))
            {
                RemoveEmptySubdirectories(subDir);
                if (Directory.GetFiles(subDir).Length == 0 &&
                    Directory.GetDirectories(subDir).Length == 0)
                {
                    Directory.Delete(subDir, false);
                }
            }
        }

        public static void BackupDirectory(string directory)
        {
            DirectoryInfo sourceDir = new DirectoryInfo(directory);
            CopyDirectory(sourceDir.FullName, sourceDir.Parent.FullName + @"\" + sourceDir.Name + @" - Backup");
        }

        public static void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAllInDirectory(diSource, diTarget);
        }

        public static void CopyAllInDirectory(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(@"Backing up {1}\{0}", MakeRelativePath(Directory.GetCurrentDirectory(), target.FullName), MakeRelativePath(Directory.GetCurrentDirectory(), fi.FullName));
                Console.ResetColor();
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }
            
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAllInDirectory(diSourceSubDir, nextTargetSubDir);
            }
        }

        private static void ShowSuccessMessage()
        {
            string msg = ".meta files edited. Press any key to close.";
            float sparkle = 0;
            double c = 0;
            Console.CursorVisible = false;

            while (!Console.KeyAvailable)
            {
                int index = (int)Math.Round(sparkle, 0);
                string frntmsg = msg.Substring(0, index); ;
                string rearmsg = msg.Substring(index + 1, msg.Length - (index + 1));


                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                SetScreenColorsDemo.SetColor(ConsoleColor.DarkMagenta, SetScreenColorsDemo.HSL2RGB(c, 0.40, 0.50));
                Console.SetCursorPosition((int)((Console.WindowWidth - msg.Length) * 0.5), Console.CursorTop);
                Console.Write(frntmsg);
                Console.SetCursorPosition((int)((Console.WindowWidth - msg.Length) * 0.5) + index + 1, Console.CursorTop);
                Console.Write(rearmsg);

                Console.SetCursorPosition((int)((Console.WindowWidth - msg.Length) * 0.5) + index, Console.CursorTop);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(msg[index]);
                sparkle += 0.015f;
                if (sparkle >= msg.Length - 1) { sparkle = 0; }
                c += 0.0001;
                if (c >= 1) { c = 0; }
                Thread.Sleep(0);
            }
            Console.Clear();
            Console.CursorVisible = true;
        }

        private static void ModifyFile(string arg, bool deleteUnused)
        {
            XDocument xmlFile;
            try
            {
                xmlFile = XDocument.Load(arg);
                FileType fType = FileType.Unknown;
                if (xmlFile.Elements("sPedAccuracyModifiers").Count() > 0) { fType = FileType.Pedaccuracy_meta; }
                else if (xmlFile.Elements("CPedDamageData").Count() > 0) { fType = FileType.Peddamage_xml; }
                else if (xmlFile.Elements("CHealthConfigInfoManager").Count() > 0) { fType = FileType.Pedhealth_meta; }
                else if (xmlFile.Elements("CPickupDataManager").Count() > 0) { fType = FileType.Pickups_meta; }
                else if (xmlFile.Elements("ScaleformPreallocation").Count() > 0) { fType = FileType.Scaleformpreallocation_xml; }
                else if (xmlFile.Elements("CTaskDataInfoManager").Count() > 0) { fType = FileType.Taskdata_meta; }
                else if (xmlFile.Elements("CWeaponInfoBlob").Count() > 0) { fType = FileType.Weapons_meta; }
                else if (Path.GetFileNameWithoutExtension(arg).ToLower().Contains("wantedtuning")) { fType = FileType.Wantedtuning_ymt; }

                switch (fType)
                {
                    case FileType.Pedaccuracy_meta:
                        {
                            PedAccuracyMod(xmlFile, arg);
                            Console.WriteLine();
                            break;
                        }
                    case FileType.Peddamage_xml:
                        {
                            PedDamageMod(xmlFile, arg);
                            Console.WriteLine();
                            break;
                        }
                    case FileType.Pedhealth_meta:
                        {
                            PedHealthMod(xmlFile, arg);
                            Console.WriteLine();
                            break;
                        }
                    case FileType.Pickups_meta:
                        {
                            PickupsMod(xmlFile, arg);
                            Console.WriteLine();
                            break;
                        }
                    case FileType.Scaleformpreallocation_xml:
                        {
                            ScaleformPreallocationMod(xmlFile, arg);
                            Console.WriteLine();
                            break;
                        }
                    case FileType.Taskdata_meta:
                        {
                            TaskdataMod(xmlFile, arg);
                            Console.WriteLine();
                            break;
                        }
                    case FileType.Weapons_meta:
                        {
                            WeaponsMod(xmlFile, arg);
                            Console.WriteLine();
                            break;
                        }
                    case FileType.Unknown:
                        {
                            if (deleteUnused) { File.Delete(arg); } //delete unused XML file
                            break;
                        }
                }
            }
            catch (Exception exc)
            {
                if (deleteUnused) { File.Delete(arg); } //delete non-XML file
            }
        }

        private static void PrintLine(string text, ConsoleColor color, float justify)
        {
            if (text.Length > 0)
            {
                Console.SetCursorPosition((int)((Console.WindowWidth - text.Length) * justify), Console.CursorTop);
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }

        private static void PedAccuracyMod(XDocument xmlFile, string arg)
        {
            PrintLine("Ped Accuracy Modifier File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f);

            var query = from c in xmlFile.Elements("sPedAccuracyModifiers")
                        select c;
            foreach (XElement sPedAccuracyModifiers in query)
            {
                XElement globalMod = sPedAccuracyModifiers.Element("AI_GLOBAL_MODIFIER");
                if (globalMod != null)
                {
                    globalMod.Attribute("value").Value = "1.000000";
                    PrintLine("AI_GLOBAL_MODIFIER decreased to 1.000000", ConsoleColor.DarkGray, 0.0f);
                }
                XElement proVSAiMod = sPedAccuracyModifiers.Element("AI_PROFESSIONAL_PISTOL_VS_AI_MODIFIER");
                if (proVSAiMod != null)
                {
                    proVSAiMod.Attribute("value").Value = "1.000000";
                    PrintLine("AI_PROFESSIONAL_PISTOL_VS_AI_MODIFIER decreased to 1.000000", ConsoleColor.DarkGray, 0.0f);
                }
            }
            xmlFile.Save(arg + (Debug ? "test" : ""));
        }

        private static void PedDamageMod(XDocument xmlFile, string arg)
        {
            PrintLine("Ped Damage Data File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f);

            var query = from c in xmlFile.Elements("CPedDamageData")
                        select c;
            foreach (XElement CPedDamageData in query)
            {
                XElement loResTDist = CPedDamageData.Element("LowResTargetDistanceCutoff");
                if (loResTDist != null)
                {
                    loResTDist.Attribute("value").Value = "300.000000";
                    PrintLine("LowResTargetDistanceCutoff increased to 300.000000", ConsoleColor.DarkGray, 0.0f);
                }
                XElement ScarNum = CPedDamageData.Element("NumWoundsToScarsOnDeathSP");
                if (ScarNum != null)
                {
                    ScarNum.Attribute("value").Value = "20";
                    PrintLine("NumWoundsToScarsOnDeathSP increased to 20", ConsoleColor.DarkGray, 0.0f);
                }
                XElement WoundNum = CPedDamageData.Element("MaxPlayerBloodWoundsSP");
                if (WoundNum != null)
                {
                    WoundNum.Attribute("value").Value = "200";
                    PrintLine("MaxPlayerBloodWoundsSP increased to 200", ConsoleColor.DarkGray, 0.0f);
                }
            }
            xmlFile.Save(arg + (Debug ? "test" : ""));
        }

        private static void PedHealthMod(XDocument xmlFile, string arg)
        {
            PrintLine("Health Config File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f);

            var query = from c in xmlFile.Elements("CHealthConfigInfoManager").Elements("aHealthConfig").Elements("Item")
                        select c;
            foreach (XElement CHealthConfigInfoManager in query)
            {
                bool disableMeleeOneshot = false;
                PrintLine(CHealthConfigInfoManager.Element("Name").Value, ConsoleColor.Yellow, 0.0f);
                XElement fHealthThresh = CHealthConfigInfoManager.Element("FatiguedHealthThreshold");
                if (fHealthThresh != null)
                {
                    if (float.Parse(fHealthThresh.Attribute("value").Value, CultureInfo.InvariantCulture.NumberFormat) > 100.000000f)
                    {
                        disableMeleeOneshot = true;
                        fHealthThresh.Attribute("value").Value = "100.000000";
                        PrintLine("   FatiguedHealthThreshold set to 100.000000", ConsoleColor.DarkGray, 0.0f);
                    }
                }
                XElement hHealthThresh = CHealthConfigInfoManager.Element("HurtHealthThreshold");
                if (hHealthThresh != null)
                {
                    if (float.Parse(hHealthThresh.Attribute("value").Value, CultureInfo.InvariantCulture.NumberFormat) > 100.000000f)
                    {
                        disableMeleeOneshot = true;
                        hHealthThresh.Attribute("value").Value = "100.000000";
                        PrintLine("   HurtHealthThreshold set to 100.000000", ConsoleColor.DarkGray, 0.0f);
                    }
                }
                if (disableMeleeOneshot)
                {
                    XElement meleeFatal = CHealthConfigInfoManager.Element("MeleeCardinalFatalAttackCheck");
                    if (meleeFatal != null)
                    {
                        if (meleeFatal.Attribute("value").Value == "true")
                        {
                            meleeFatal.Attribute("value").Value = "false";
                            PrintLine("   MeleeCardinalFatalAttackCheck disabled.", ConsoleColor.DarkGray, 0.0f);
                        }
                    }
                }
                
                Console.WriteLine();
            }
            xmlFile.Save(arg + (Debug ? "test" : ""));
        }

        private static void PickupsMod(XDocument xmlFile, string arg)
        {
            PrintLine("Pickup Data File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f);

            var query = from c in xmlFile.Elements("CPickupDataManager").Elements("pickupData").Elements("Item")
                        select c;
            foreach (XElement CPickupData in query)
            {
                var typ = CPickupData.Attribute("type").Value;
                if (typ == "CPickupData")
                {

                    PrintLine(CPickupData.Element("Name").Value, ConsoleColor.Yellow, 0.0f);
                    XElement ERewards = CPickupData.Element("Rewards");
                    if (ERewards.HasElements)
                    {
                        ERewards.Elements().Remove();
                        PrintLine("   Rewards removed.", ConsoleColor.DarkGray, 0.0f);
                    }

                    XElement EPickupFlags = CPickupData.Element("PickupFlags");
                    if (EPickupFlags.Value.Contains("RequiresButtonPressToPickup"))
                    {
                        EPickupFlags.Value = EPickupFlags.Value.Replace("RequiresButtonPressToPickup", String.Empty);
                        PrintLine("   RequiresButtonPressToPickup removed.", ConsoleColor.DarkGray, 0.0f);
                    }
                }
                Console.WriteLine();
            }
            xmlFile.Save(arg + (Debug ? "test" : ""));
        }

        private static void ScaleformPreallocationMod(XDocument xmlFile, string arg)
        {
            //<movie name="PLAYER_NAME_02" peakSize="288" granularity="16" sfalloc="true">
            //	<SFAlloc>
            //    <Size value="16"/>
            //    <Count value="10"/>
            //  </SFAlloc>
            //	<SFAlloc>
            //    <Size value="32"/>
            //    <Count value="1"/>
            //  </SFAlloc>
            //</movie>
            PrintLine("Scaleform Preallocation File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f);

            XElement ScaleformPreallocation = xmlFile.Element("ScaleformPreallocation");
            if (ScaleformPreallocation != null)
            {
                string fmt = "00";
                for (int i = 1; i < 17; i++)
                {
                    ScaleformPreallocation.Add
                        (
                        new XElement("movie",
                            new XAttribute("name", "PICKUP_DISPLAY_" + i.ToString(fmt)),
                            new XAttribute("peakSize", "288"),
                            new XAttribute("granularity", "16"),
                            new XAttribute("sfalloc", "true"),
                            new XElement("SFAlloc",
                                new XElement("Size", new XAttribute("value", "16")),
                                new XElement("Count", new XAttribute("value", "10"))
                            ),
                            new XElement("SFAlloc",
                                new XElement("Size", new XAttribute("value", "32")),
                                new XElement("Count", new XAttribute("value", "1"))
                            )
                        ));
                    PrintLine("Added Preallocation Data for " + "PICKUP_DISPLAY_" + i.ToString(fmt), ConsoleColor.DarkGray, 0.0f);
                }
                ScaleformPreallocation.Add
                    (
                    new XElement("movie",
                        new XAttribute("name", "ECG_DISPLAY"),
                        new XAttribute("peakSize", "288"),
                        new XAttribute("granularity", "16"),
                        new XAttribute("sfalloc", "true"),
                        new XElement("SFAlloc",
                            new XElement("Size", new XAttribute("value", "16")),
                            new XElement("Count", new XAttribute("value", "10"))
                        ),
                        new XElement("SFAlloc",
                            new XElement("Size", new XAttribute("value", "32")),
                            new XElement("Count", new XAttribute("value", "1"))
                        )
                    ));
                PrintLine("Added Preallocation Data for " + "ECG_DISPLAY", ConsoleColor.DarkGray, 0.0f);
            }

            xmlFile.Save(arg + (Debug ? "test" : ""));
        }

        private static void TaskdataMod(XDocument xmlFile, string arg)
        {
            PrintLine("Task Data Info File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f);

            var query = from c in xmlFile.Elements("CTaskDataInfoManager").Elements("aTaskData").Elements("Item")
                        select c;
            foreach (XElement CTaskDataInfo in query)
            {
                var typ = CTaskDataInfo.Attribute("type").Value;
                if (typ == "CTaskDataInfo")
                {
                    if (CTaskDataInfo.Element("Name").Value == "STANDARD_PED")
                    {
                        PrintLine(CTaskDataInfo.Element("Name").Value, ConsoleColor.Yellow, 0.0f);
                        XElement EFlags = CTaskDataInfo.Element("Flags");
                        if (EFlags.Value.Contains("PreferFleeOnPavements"))
                        {
                            EFlags.Value = EFlags.Value.Replace("PreferFleeOnPavements", String.Empty);
                            PrintLine("   PreferFleeOnPavements removed.", ConsoleColor.DarkGray, 0.0f);
                        }
                    }
                }
            }
            xmlFile.Save(arg + (Debug ? "test" : ""));
        }

        private static void WeaponsMod(XDocument xmlFile, string arg)
        {
            PrintLine("Weapon Info File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f);

            var query = from c in xmlFile.Elements("CWeaponInfoBlob").Elements("Infos")
                        select c;
            foreach (XElement Info in query)
            {
                bool insertAmmoHere = false;
                var que = from a in Info.Elements("Item").Elements("Infos").Elements("Item")
                            select a;
                foreach (XElement CWeaponInfo in que)
                {
                    var typ = CWeaponInfo.Attribute("type").Value;
                    if (typ == "CAmmoInfo" && Path.GetFileName(arg).ToLower() == "weapons.meta")
                    {
                        insertAmmoHere = true;
                    }
                    //if (typ == "CAmmoProjectileInfo") //disabled until can remove auto-trajectory for thrown weapons as well
                    //{
                    //    PrintLine(CWeaponInfo.Element("Name").Value, ConsoleColor.Yellow, 0.0f);

                    //    XElement EFlags = CWeaponInfo.Element("ProjectileFlags");
                    //    if (EFlags.Value.Contains("AlignWithTrajectory"))
                    //    {
                    //        EFlags.Value = EFlags.Value.Replace("AlignWithTrajectory", String.Empty);
                    //        PrintLine("   Automatic Trajectory Adjustment Disabled.", ConsoleColor.DarkGray, 0.0f);
                    //    }
                    //    Console.WriteLine();
                    //}

                    if (typ == "CWeaponInfo")
                    {
                        PrintLine(CWeaponInfo.Element("Name").Value, ConsoleColor.Yellow, 0.0f);

                        if (usingRGS)
                        {
                            XElement EAudio = CWeaponInfo.Element("Audio");
                            if (EAudio != null)
                            {
                                switch (CWeaponInfo.Element("Name").Value)
                                {
                                    case "WEAPON_ASSAULTRIFLE":
                                        {
                                            EAudio.Value = "AUDIO_ITEM_HEAVYRIFLE";
                                            break;
                                        }
                                    case "WEAPON_ADVANCEDRIFLE":
                                        {
                                            EAudio.Value = "AUDIO_ITEM_BULLPUPRIFLE";
                                            break;
                                        }
                                    case "WEAPON_MG":
                                        {
                                            EAudio.Value = "AUDIO_ITEM_ASSAULTMG";
                                            break;
                                        }
                                    case "WEAPON_COMBATMG ":
                                        {
                                            EAudio.Value = "AUDIO_ITEM_COMBATMG_MK2";
                                            break;
                                        }
                                }
                            }
                        }

                        XElement ETracerFx = CWeaponInfo.Element("Fx").Element("TracerFx");
                        if (ETracerFx != null)
                        {
                            if (ETracerFx.Value != "")
                            {
                                ETracerFx.Value = "";
                                PrintLine("   Tracer Effects removed.", ConsoleColor.DarkGray, 0.0f);
                            }
                        }

                        XElement EFlashlightShadows = CWeaponInfo.Element("Fx").Element("FlashFxLightCastsShadows");
                        if (EFlashlightShadows != null)
                        {
                            if (EFlashlightShadows.Attribute("value").Value != "true")
                            {
                                EFlashlightShadows.Attribute("value").Value = "true";
                                PrintLine("   Flash Shadows Enabled.", ConsoleColor.DarkGray, 0.0f);
                            }
                        }

                        var q = from k in CWeaponInfo.Elements()
                                  select k;
                        foreach (XElement EInfo in q)
                        {
                            if (EInfo.Name == "FireType")
                            {
                                if (EInfo.Value == "INSTANT_HIT")
                                {
                                    EInfo.Value = "DELAYED_HIT";
                                    PrintLine("   FireType set to DELAYED_HIT.", ConsoleColor.DarkGray, 0.0f);
                                }
                            }
                            if (EInfo.Name == "RecoilShakeAmplitude")
                            {
                                if (EInfo.Attribute("value").Value != "0.000000")
                                {
                                    EInfo.Attribute("value").Value = "0.000000";
                                    PrintLine("   Stock Recoil Disabled.", ConsoleColor.DarkGray, 0.0f);
                                }
                            }
                            if (EInfo.Name == "HeadShotDamageModifierAI")
                            {
                                if (EInfo.Attribute("value").Value != "1.000000")
                                {
                                    EInfo.Attribute("value").Value = "1.000000";
                                    PrintLine("   Stock AI Headshots Disabled.", ConsoleColor.DarkGray, 0.0f);
                                }
                            }
                            if (EInfo.Name == "HeadShotDamageModifierPlayer")
                            {
                                if (EInfo.Attribute("value").Value != "1.000000")
                                {
                                    EInfo.Attribute("value").Value = "1.000000";
                                    PrintLine("   Stock Player Headshots Disabled.", ConsoleColor.DarkGray, 0.0f);
                                }
                            }
                            if (EInfo.Name == "HitLimbsDamageModifier")
                            {
                                if (EInfo.Attribute("value").Value != "1.000000")
                                {
                                    EInfo.Attribute("value").Value = "1.000000";
                                    PrintLine("   Stock Limb Damage Modifier Disabled.", ConsoleColor.DarkGray, 0.0f);
                                }
                            }
                            if (EInfo.Name == "NetworkHitLimbsDamageModifier")
                            {
                                if (EInfo.Attribute("value").Value != "1.000000")
                                {
                                    EInfo.Attribute("value").Value = "1.000000";
                                    PrintLine("   Stock Net Limb Damage Modifier Disabled.", ConsoleColor.DarkGray, 0.0f);
                                }
                            }
                            if (EInfo.Name == "LightlyArmouredDamageModifier")
                            {
                                if (EInfo.Attribute("value").Value != "1.000000")
                                {
                                    EInfo.Attribute("value").Value = "1.000000";
                                    PrintLine("   Stock Armor Damage Modifier Disabled.", ConsoleColor.DarkGray, 0.0f);
                                }
                            }
                        }
                        Console.WriteLine();
                    }
                }
                
                if (insertAmmoHere)
                {
                    var ammoList = new List<KeyValuePair<string, int>>();               //    mm            -       ci      -       *2.16 for in-mag size
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_57X28", 120));     //7.95 x 40.50      - 0.1226809147 
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_556X45", 120));    //9.60 x 57.40      - 0.2535377905
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_545X39", 120));    //10.00 x 57.00     - 0.2731892669
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_58X42", 120));     //10.40 x 58.00     - 0.3006652076
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_762X39", 120));    //11.35 x 56.00     - 0.3457550419
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_338", 120));       //11.90 x 70.00     - 0.4750954778
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_762X51", 120));    //11.90 x 71.10     - 0.4818825386
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_93X64", 120));     //12.88 x 85.60     - 0.68060331003
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_408", 120));       //16.18 X 115.50    - 1.449197977
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_50BMG", 30));      //20.40 x 138.00    - 2.7525052688
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_127X108", 120));   //21.75 x 147.50    - 3.344254346

                    ammoList.Add(new KeyValuePair<string, int>("AMMO_22LR", 120));      //5.70 x 25.40      - 0.0395525397
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_38SPC", 60));      //9.60 x 39.00      - 0.1722645374
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_9MM", 120));       //9.93 x 29.69      - 0.1403131153
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_40SW", 120));      //10.80 x 28.80     - 0.1610007748
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_10MM", 120));      //10.80 x 32.00     - 0.1788898853
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_45ACP", 120));     //12.20 x 32.40     - 0.2311286512
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_45LC", 120));      //12.20 x 40.60     - 0.2896241816
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_50AE", 120));      //13.90 x 40.90     - 0.3787402063

                    ammoList.Add(new KeyValuePair<string, int>("AMMO_20G", 30));        //15.60 x 63.50     - 0.74064762303
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_20GSLUG", 30));
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_12G", 30));        //16.90 x 63.50     - 0.86923258492
                    ammoList.Add(new KeyValuePair<string, int>("AMMO_12GSLUG", 30));

                    ammoList.Add(new KeyValuePair<string, int>("AMMO_18MM", 20));
                    
                    foreach (KeyValuePair<string, int> ammo in ammoList)
                    {
                        bool addAmmo = true;
                        foreach (XElement element in Info.Element("Item").Element("Infos").Elements("Item"))
                        {
                            if (element.Element("Name").Value == ammo.Key)
                            {
                                addAmmo = false;
                                break;
                            }
                        }
                        if (addAmmo)
                        {
                            Info.Element("Item").Element("Infos").Add
                                (
                                new XElement("Item",
                                    new XAttribute("type", "CAmmoInfo"),
                                    new XElement("Name", ammo.Key),
                                    new XElement("Model"),
                                    new XElement("Audio"),
                                    new XElement("Slot"),
                                    new XElement("AmmoMax", new XAttribute("value", ammo.Value.ToString())),
                                    new XElement("AmmoMax50", new XAttribute("value", ammo.Value.ToString())),
                                    new XElement("AmmoMax100", new XAttribute("value", ammo.Value.ToString())),
                                    new XElement("AmmoMaxMP", new XAttribute("value", ammo.Value.ToString())),
                                    new XElement("AmmoMax50MP", new XAttribute("value", ammo.Value.ToString())),
                                    new XElement("AmmoMax100MP", new XAttribute("value", ammo.Value.ToString()))
                                ));
                            PrintLine("Added Ammo Data for " + ammo.Key, ConsoleColor.DarkGray, 0.0f);
                        }
                    }
                }
            }
            xmlFile.Save(arg + (Debug ? "test" : ""));
        }
    }
}
