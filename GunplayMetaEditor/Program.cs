using System;
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
using CodeWalker.GameFiles;
using GunplayMetaEditor.Properties;
using Microsoft.Win32;

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
        RAGEPackageFile,
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
    

    struct GameFileInfo
    {
        FileType type;
        public string name;
        public string path;
        public string dlcName;
        public bool inMods;
        public bool inUpdate;
        public bool inDLC;
        public bool inDLCPatch;

        public GameFileInfo(string pth) : this()
        {
            name = Path.GetFileNameWithoutExtension(pth);
            path = pth;
            dlcName = "";
            inMods = false;
            inUpdate = false;
            inDLC = false;
            inDLCPatch = false;

            if (pth.ToLower().Contains(@"\mods\"))
            {
                inMods = true;
            }

            if (pth.ToLower().Contains(@"\update\"))
            {
                inUpdate = true;
            }

            if (pth.ToLower().Contains(@"\dlcpacks\"))
            {
                string searchstr = @"\dlcpacks\";
                dlcName = (pth.ToLower().Substring(pth.ToLower().IndexOf(searchstr) + searchstr.Length)).Split('\\')[0];
                inDLC = true;
            }
            else if (pth.ToLower().Contains(@"\dlc_patch\"))
            {
                string searchstr = @"\dlc_patch\";
                dlcName = (pth.ToLower().Substring(pth.ToLower().IndexOf(searchstr) + searchstr.Length)).Split('\\')[0];
                inDLC = true;
                inDLCPatch = true;
            }
        }

        /*
        File importance

        mods+dlcpatch
        mods+dlc
        mods
        update+dlcpatch
        update+dlc
        update
        dlc
        base
        */
    }
    

    

    class Program
    {
        const bool Debug = false; //|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        private static bool usingRGS;

        static void Main(string[] args)
        {
            if (!GTAFolder.UpdateGTAFolder(Properties.Settings.Default.RememberGTADirectory))
            {
                PrintLine("Could not start because no valid GTA 5 folder was selected.", ConsoleColor.Red, 0.0f);
                PrintLine("Exiting...", ConsoleColor.Red, 0.0f);
                Console.ReadKey();
                return;
            }

            PrintLine("Loading Keys...", ConsoleColor.Cyan, 0.0f);
            GTA5Keys.LoadFromPath(Properties.Settings.Default.GTADirectory, Settings.Default.Key);
            Settings.Default.Key = Convert.ToBase64String(GTA5Keys.PC_AES_KEY);
            Settings.Default.Save();
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
                        FileType fType = GetFileType(arguments[i]);
                        PrintLine(fType.ToString(), ConsoleColor.Yellow, 0.5f);
                        switch (fType)
                        {
                            case FileType.RAGEPackageFile:
                                {
                                    string fPath = Path.GetFullPath(arguments[i]);
                                    string relPath = MakeRelativePath(Directory.GetCurrentDirectory(), fPath);
                                    RpfFile file = new RpfFile(fPath, relPath);

                                    //string found = FindNamedFileInRpf("xm_base_cia_data_desks.yft", file);
                                    //if (found != "") { PrintLine(Directory.GetCurrentDirectory() + found, ConsoleColor.Green, 0.0f); } else { PrintLine("Not Found.", ConsoleColor.Red, 0.0f); }
                                    List<GameFileInfo> fileList = FindTypedFilesInRpf(FileType.Weapons_meta, file);
                                    if (fileList.Count > 0)
                                    {
                                        foreach (GameFileInfo a in fileList)
                                        {
                                            Print(a.name, ConsoleColor.Green, 0.0f);
                                            Print(a.dlcName, ConsoleColor.Green, 0.25f);
                                            PrintLine(a.path, ConsoleColor.Green, 1.0f);
                                        }
                                    }
                                    else { PrintLine("Not Found.", ConsoleColor.Red, 0.0f); }

                                    break;
                                }
                            default:
                                {
                                    ModifyFile(arguments[i], false);
                                    break;
                                }
                        }
                    }
                    else if (Directory.Exists(arguments[i]))
                    {
                        BackupDirectory(arguments[i]);
                        ModifyAllInDirectory(arguments[i]);
                        RemoveEmptySubdirectories(arguments[i]);
                    }
                }

                ShowSuccessMessage(".meta files edited. Press any key to close.");
            }
        }

        static void ReadRpfFileHeader(RpfFile file, BinaryReader br)
        {
            file.CurrentFileReader = br;

            file.StartPos = br.BaseStream.Position;

            file.Version = br.ReadUInt32(); //RPF Version - GTAV should be 0x52504637 (1380992567)
            file.EntryCount = br.ReadUInt32(); //Number of Entries
            file.NamesLength = br.ReadUInt32();
            file.Encryption = (RpfEncryption)br.ReadUInt32(); //0x04E45504F (1313165391): none;  0x0ffffff9 (268435449): AES

            if (file.Version != 0x52504637)
            {
                throw new Exception("Invalid Resource - not GTAV!");
            }

            byte[] entriesdata = br.ReadBytes((int)file.EntryCount * 16); //4x uints each
            byte[] namesdata = br.ReadBytes((int)file.NamesLength);

            switch (file.Encryption)
            {
                case RpfEncryption.NONE: //no encryption
                case RpfEncryption.OPEN: //OpenIV style RPF with unencrypted TOC
                    break;
                case RpfEncryption.AES:
                    entriesdata = GTACrypto.DecryptAES(entriesdata);
                    namesdata = GTACrypto.DecryptAES(namesdata);
                    file.IsAESEncrypted = true;
                    break;
                case RpfEncryption.NG:
                    entriesdata = GTACrypto.DecryptNG(entriesdata, file.Name, (uint)file.FileSize);
                    namesdata = GTACrypto.DecryptNG(namesdata, file.Name, (uint)file.FileSize);
                    file.IsNGEncrypted = true;
                    break;
                default: //unknown encryption type? assume NG.. never seems to get here
                    entriesdata = GTACrypto.DecryptNG(entriesdata, file.Name, (uint)file.FileSize);
                    namesdata = GTACrypto.DecryptNG(namesdata, file.Name, (uint)file.FileSize);
                    break;
            }


            var entriesrdr = new DataReader(new MemoryStream(entriesdata));
            var namesrdr = new DataReader(new MemoryStream(namesdata));
            file.AllEntries = new List<RpfEntry>();
            file.TotalFileCount = 0;
            file.TotalFolderCount = 0;
            file.TotalResourceCount = 0;
            file.TotalBinaryFileCount = 0;

            for (uint i = 0; i < file.EntryCount; i++)
            {
                //entriesrdr.Position += 4;
                uint y = entriesrdr.ReadUInt32();
                uint x = entriesrdr.ReadUInt32();
                entriesrdr.Position -= 8;

                RpfEntry e;

                if (x == 0x7fffff00) //directory entry
                {
                    e = new RpfDirectoryEntry();
                    file.TotalFolderCount++;
                }
                else if ((x & 0x80000000) == 0) //binary file entry
                {
                    e = new RpfBinaryFileEntry();
                    file.TotalBinaryFileCount++;
                    file.TotalFileCount++;
                }
                else //assume resource file entry
                {
                    e = new RpfResourceFileEntry();
                    file.TotalResourceCount++;
                    file.TotalFileCount++;
                }

                e.File = file;
                e.H1 = y;
                e.H2 = x;

                e.Read(entriesrdr);

                namesrdr.Position = e.NameOffset;
                e.Name = namesrdr.ReadString();
                e.NameLower = e.Name.ToLowerInvariant();

                if ((e is RpfFileEntry) && string.IsNullOrEmpty(e.Name))
                {
                }
                if ((e is RpfResourceFileEntry))// && string.IsNullOrEmpty(e.Name))
                {
                    var rfe = e as RpfResourceFileEntry;
                    rfe.IsEncrypted = rfe.NameLower.EndsWith(".ysc");//any other way to know..?
                }

                file.AllEntries.Add(e);
            }



            file.Root = (RpfDirectoryEntry)file.AllEntries[0];
            file.Root.Path = file.Path.ToLowerInvariant();// + "\\" + Root.Name;
            var stack = new Stack<RpfDirectoryEntry>();
            stack.Push(file.Root);
            while (stack.Count > 0)
            {
                var item = stack.Pop();

                int starti = (int)item.EntriesIndex;
                int endi = (int)(item.EntriesIndex + item.EntriesCount);

                for (int i = starti; i < endi; i++)
                {
                    RpfEntry e = file.AllEntries[i];
                    e.Parent = item;
                    if (e is RpfDirectoryEntry)
                    {
                        RpfDirectoryEntry rde = e as RpfDirectoryEntry;
                        rde.Path = item.Path + "\\" + rde.NameLower;
                        item.Directories.Add(rde);
                        stack.Push(rde);
                    }
                    else if (e is RpfFileEntry)
                    {
                        RpfFileEntry rfe = e as RpfFileEntry;
                        rfe.Path = item.Path + "\\" + rfe.NameLower;
                        item.Files.Add(rfe);
                    }
                }
            }

            br.BaseStream.Position = file.StartPos;

            file.CurrentFileReader = null;
        }

        public static void ScanRpfFileStructure(RpfFile file)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(file.FilePath)))
            {
                try
                {
                    ScanRpfFileStructure(file, br);
                }
                catch (Exception ex)
                {
                    file.LastError = ex.ToString();
                    file.LastException = ex;
                    //errorLog(FilePath + ": " + LastError);
                }
            }
        }

        public static void ScanRpfFileStructure(RpfFile file, BinaryReader br)
        {
            ReadRpfFileHeader(file, br);

            file.GrandTotalRpfCount = 1; //count this file..
            file.GrandTotalFileCount = 1; //start with this one.
            file.GrandTotalFolderCount = 0;
            file.GrandTotalResourceCount = 0;
            file.GrandTotalBinaryFileCount = 0;

            file.Children = new List<RpfFile>();

            //updateStatus?.Invoke("Scanning " + Path + "...");

            foreach (RpfEntry entry in file.AllEntries)
            {
                try
                {
                    if (entry is RpfBinaryFileEntry)
                    {
                        RpfBinaryFileEntry binentry = entry as RpfBinaryFileEntry;

                        //search all the sub resources for YSC files. (recurse!)
                        string lname = binentry.NameLower;
                        if (lname.EndsWith(".rpf"))
                        {
                            br.BaseStream.Position = file.StartPos + ((long)binentry.FileOffset * 512);

                            long l = binentry.GetFileSize();

                            RpfFile subfile = new RpfFile(binentry.Name, binentry.Path, l);
                            subfile.Parent = file;
                            subfile.ParentFileEntry = binentry;

                            ScanRpfFileStructure(subfile, br);

                            file.GrandTotalRpfCount += subfile.GrandTotalRpfCount;
                            file.GrandTotalFileCount += subfile.GrandTotalFileCount;
                            file.GrandTotalFolderCount += subfile.GrandTotalFolderCount;
                            file.GrandTotalResourceCount += subfile.GrandTotalResourceCount;
                            file.GrandTotalBinaryFileCount += subfile.GrandTotalBinaryFileCount;

                            file.Children.Add(subfile);
                        }
                        else
                        {
                            //binary file that's not an rpf...
                            file.GrandTotalBinaryFileCount++;
                            file.GrandTotalFileCount++;
                        }
                    }
                    else if (entry is RpfResourceFileEntry)
                    {
                        file.GrandTotalResourceCount++;
                        file.GrandTotalFileCount++;
                    }
                    else if (entry is RpfDirectoryEntry)
                    {
                        file.GrandTotalFolderCount++;
                    }


                }
                catch (Exception ex)
                {
                    //errorLog?.Invoke(entry.Path + ": " + ex.ToString());
                }
            }
        }

        //public static string FindNewestFile(string filename)
        //{
        //    bool inMods = false;
        //    bool inUpdate = false;
        //    bool inBase = false;
        //    bool overwritesFile = false; //backup overwritten file

        //    if (inMods)
        //    {
        //        bool inDlcPatch = false;
        //        bool inDlcPack = false;
        //        bool inCore = false;

        //        if (inDlcPack || inDlcPatch)
        //        {
        //            destination = DlcPatch;
        //            if (inDlcPatch) { overwritesFile = true; }
        //        }
        //        if (inCore)
        //        {
        //            destination = Core;
        //            overwritesFile = true;
        //        }
        //    }

        //    if (inUpdate)
        //    {
        //        bool inDlcPatch = false;
        //        bool inDlcPack = false;
        //        bool inCore = false;

        //        if (inDlcPack || inDlcPatch)
        //        {
        //            destination = DlcPatch;
        //            if (inDlcPatch) { overwritesFile = true; }
        //        }
        //        if (inCore)
        //        {
        //            destination = Core;
        //            overwritesFile = true;
        //        }
        //    }

        //    if (inBase)
        //    {
        //        bool inDlcPack = false;
        //        bool inCore = false;

        //        if (inDlcPack)
        //        {
        //            destination = DlcPatch;
        //        }
        //        if (inCore)
        //        {
        //            destination = Core;
        //        }
        //    }
        //    return "";
        //}

        public static string FindNamedFileInRpf(string filename, RpfFile file)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(file.FilePath)))
            {
                try
                {
                    string retval = FindNamedFileInRpf(filename, file, br);
                    if (retval != "") { return retval.Substring(retval.IndexOf('\\')); }
                }
                catch (Exception ex)
                {
                    file.LastError = ex.ToString();
                    file.LastException = ex;
                    //errorLog(FilePath + ": " + LastError);
                }
            }
            return "";
        }

        public static string FindNamedFileInRpf(string filename, RpfFile file, BinaryReader br)
        {
            ReadRpfFileHeader(file, br);

            foreach (RpfEntry entry in file.AllEntries)
            {
                try
                {
                    if (entry is RpfBinaryFileEntry)
                    {
                        RpfBinaryFileEntry binentry = entry as RpfBinaryFileEntry;

                        //search all the sub resources for YSC files. (recurse!)
                        string lname = binentry.NameLower;
                        if (lname.EndsWith(".rpf"))
                        {
                            br.BaseStream.Position = file.StartPos + ((long)binentry.FileOffset * 512);

                            long l = binentry.GetFileSize();

                            RpfFile subfile = new RpfFile(binentry.Name, binentry.Path, l);
                            subfile.Parent = file;
                            subfile.ParentFileEntry = binentry;

                            string recval = FindNamedFileInRpf(filename, subfile, br);
                            if (recval != "") { return recval; }
                        }
                    }
                    if (entry.NameLower == filename) { return entry.Path; }

                }
                catch (Exception ex)
                {
                    //errorLog?.Invoke(entry.Path + ": " + ex.ToString());
                }
            }
            return "";
        }

        public static List<GameFileInfo> FindModifiableFilesInRpf(RpfFile file)
        {
            List<GameFileInfo> files = new List<GameFileInfo>();

            using (BinaryReader br = new BinaryReader(File.OpenRead(file.FilePath)))
            {
                try
                {
                    List<GameFileInfo> recval = FindModifiableFilesInRpf(file, br);
                    files.AddRange(recval);
                }
                catch (Exception ex)
                {
                    file.LastError = ex.ToString();
                    file.LastException = ex;
                    //errorLog(FilePath + ": " + LastError);
                }
            }
            return files;
        }

        public static List<GameFileInfo> FindModifiableFilesInRpf(RpfFile file, BinaryReader br)
        {
            ReadRpfFileHeader(file, br);
            List<GameFileInfo> fileList = new List<GameFileInfo>();

            foreach (RpfEntry entry in file.AllEntries)
            {
                try
                {
                    if (entry is RpfBinaryFileEntry)
                    {
                        RpfBinaryFileEntry binentry = entry as RpfBinaryFileEntry;

                        //search all the sub resources for YSC files. (recurse!)
                        string lname = binentry.NameLower;
                        if (lname.EndsWith(".rpf"))
                        {
                            br.BaseStream.Position = file.StartPos + ((long)binentry.FileOffset * 512);

                            long l = binentry.GetFileSize();

                            RpfFile subfile = new RpfFile(binentry.Name, binentry.Path, l);
                            subfile.Parent = file;
                            subfile.ParentFileEntry = binentry;

                            List<GameFileInfo> recval = FindModifiableFilesInRpf(subfile, br);
                            fileList.AddRange(recval);
                        }
                    }

                    FileType fType = GetFileType(file, entry);
                    if (ModifyFile())
                    {
                        GameFileInfo newFileInfo = new GameFileInfo(entry.Path);
                        fileList.Add(newFileInfo);
                    }

                }
                catch (Exception ex)
                {
                    //errorLog?.Invoke(entry.Path + ": " + ex.ToString());
                }
            }
            return fileList;
        }

        public static List<GameFileInfo> FindTypedFilesInRpf(FileType type, RpfFile file)
        {
            List<GameFileInfo> files = new List<GameFileInfo>();

            using (BinaryReader br = new BinaryReader(File.OpenRead(file.FilePath)))
            {
                try
                {
                    List<GameFileInfo> recval = FindTypedFilesInRpf(type, file, br);
                    files.AddRange(recval);
                }
                catch (Exception ex)
                {
                    file.LastError = ex.ToString();
                    file.LastException = ex;
                    //errorLog(FilePath + ": " + LastError);
                }
            }
            return files;
        }

        public static List<GameFileInfo> FindTypedFilesInRpf(FileType type, RpfFile file, BinaryReader br)
        {
            ReadRpfFileHeader(file, br);
            List<GameFileInfo> fileList = new List<GameFileInfo>();

            foreach (RpfEntry entry in file.AllEntries)
            {
                try
                {
                    if (entry is RpfBinaryFileEntry)
                    {
                        RpfBinaryFileEntry binentry = entry as RpfBinaryFileEntry;

                        //search all the sub resources for YSC files. (recurse!)
                        string lname = binentry.NameLower;
                        if (lname.EndsWith(".rpf"))
                        {
                            br.BaseStream.Position = file.StartPos + ((long)binentry.FileOffset * 512);

                            long l = binentry.GetFileSize();

                            RpfFile subfile = new RpfFile(binentry.Name, binentry.Path, l);
                            subfile.Parent = file;
                            subfile.ParentFileEntry = binentry;

                            List<GameFileInfo> recval = FindTypedFilesInRpf(type, subfile, br);
                            fileList.AddRange(recval);
                        }
                    }
                    
                    FileType fType = GetFileType(file, entry);
                    if (fType == type)
                    {
                        GameFileInfo newFileInfo = new GameFileInfo(entry.Path);
                        fileList.Add(newFileInfo);
                    }

                }
                catch (Exception ex)
                {
                    //errorLog?.Invoke(entry.Path + ": " + ex.ToString());
                }
            }
            return fileList;
        }

        public static List<GameFileInfo> FindTypedFilesInDirectory(FileType type, string directory, bool recurse)
        {
            List<GameFileInfo> fileList = new List<GameFileInfo>();
            foreach (string subDir in Directory.GetDirectories(directory))
            {
                if (recurse)
                {
                    List<GameFileInfo> recval = FindTypedFilesInDirectory(type, subDir, recurse);
                    fileList.AddRange(recval);
                }

                if (Directory.GetFiles(subDir).Length == 0 &&
                    Directory.GetDirectories(subDir).Length == 0)
                {
                    Directory.Delete(subDir, false);
                }
            }
            return fileList;
        }

        public static void UnpackRPF(string fPath)
        {
            Action<string> status = null;//(x) => PrintLine(x, ConsoleColor.DarkCyan, 0.0f);
            Action<string> error = null;// (x) => PrintLine(x, ConsoleColor.Red, 0.0f);
            string relPath = MakeRelativePath(Directory.GetCurrentDirectory(), fPath);
            RpfFile file = new RpfFile(fPath, relPath);
            file.ScanStructure(status, error);
            foreach (RpfFileEntry entry in file.GetFiles("", true))
            {
                string path = entry.Path;
                if (path.EndsWith(".rpf")) { continue; }

                path = path.Replace(".rpf", "_rpf");
                path = Path.GetDirectoryName(fPath) + path.Substring(path.IndexOf('\\'));
                string directory = Path.GetDirectoryName(path);
                Directory.CreateDirectory(directory);
                try
                {
                    byte[] data = file.ExtractFile(entry);
                    if(data == null)
                    {
                        continue;
                    }
                    File.WriteAllBytes(path, data);
                    PrintLine(path, ConsoleColor.Magenta, 1.0f);
                }
                catch (Exception ex)
                {
                    PrintLine("Error saving file " + path + ": " + ex.ToString(), ConsoleColor.Red, 1.0f);
                    Console.ReadKey();
                    return;
                }
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

        public static void UnpackAllInDirectory(string source)
        {
            var diSource = new DirectoryInfo(source);
            // Copy each file into the new directory.
            foreach (FileInfo fi in diSource.GetFiles())
            {
                UnpackRPF(fi.FullName);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in diSource.GetDirectories())
            {
                UnpackAllInDirectory(diSourceSubDir.FullName);
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
                Console.WriteLine(@"Backing up {1} -> {0}", MakeRelativePath(Directory.GetCurrentDirectory(), target.FullName), MakeRelativePath(Directory.GetCurrentDirectory(), fi.FullName));
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

        private static void ShowSuccessMessage(string msg)
        {
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
            //Console.Clear();
            Console.CursorVisible = true;
        }

        private static FileType GetFileType(string path)
        {
            FileType fType = FileType.Unknown;
            if (Path.GetExtension(path).ToLower() == ".rpf") { fType = FileType.RAGEPackageFile; }
            else if (Path.GetExtension(path).ToLower() == ".meta")
            {
                XDocument xmlFile;
                try
                {
                    xmlFile = XDocument.Load(path);
                    if (xmlFile.Elements("sPedAccuracyModifiers").Count() > 0) { fType = FileType.Pedaccuracy_meta; }
                    else if (xmlFile.Elements("CHealthConfigInfoManager").Count() > 0) { fType = FileType.Pedhealth_meta; }
                    else if (xmlFile.Elements("CPickupDataManager").Count() > 0) { fType = FileType.Pickups_meta; }
                    else if (xmlFile.Elements("CTaskDataInfoManager").Count() > 0) { fType = FileType.Taskdata_meta; }
                    else if (xmlFile.Elements("CWeaponInfoBlob").Count() > 0) { fType = FileType.Weapons_meta; }
                }
                catch { }
            }
            else if (Path.GetExtension(path).ToLower() == ".xml")
            {
                XDocument xmlFile;
                try
                {
                    xmlFile = XDocument.Load(path);
                    if (xmlFile.Elements("CPedDamageData").Count() > 0) { fType = FileType.Peddamage_xml; }
                    else if (Path.GetFileNameWithoutExtension(path).ToLower() == "scaleformpreallocation" && xmlFile.Elements("ScaleformPreallocation").Count() > 0) { fType = FileType.Scaleformpreallocation_xml; }
                }
                catch { }
            }
            else if (Path.GetExtension(path).ToLower() == ".ymt")
            {
                XDocument xmlFile;
                try
                {
                    xmlFile = XDocument.Load(path);
                    if (Path.GetFileNameWithoutExtension(path).ToLower().Contains("wantedtuning")) { fType = FileType.Wantedtuning_ymt; }
                }
                catch { }
            }

            return fType;
        }

        private static FileType GetFileType(RpfFile file, RpfEntry entry)
        {
            FileType fType = FileType.Unknown;
            if (Path.GetExtension(entry.Name).ToLower() == ".rpf") { fType = FileType.RAGEPackageFile; }
            else if (Path.GetExtension(entry.Name).ToLower() == ".meta")
            {
                XDocument xmlFile;
                try
                {
                    var byteArray = file.ExtractFile((RpfFileEntry)entry);
                    Stream stream = new MemoryStream(byteArray);
                    xmlFile = XDocument.Load(stream);
                    if (xmlFile.Elements("sPedAccuracyModifiers").Count() > 0) { fType = FileType.Pedaccuracy_meta; }
                    else if (xmlFile.Elements("CHealthConfigInfoManager").Count() > 0) { fType = FileType.Pedhealth_meta; }
                    else if (xmlFile.Elements("CPickupDataManager").Count() > 0) { fType = FileType.Pickups_meta; }
                    else if (xmlFile.Elements("CTaskDataInfoManager").Count() > 0) { fType = FileType.Taskdata_meta; }
                    else if (xmlFile.Elements("CWeaponInfoBlob").Count() > 0) { fType = FileType.Weapons_meta; }
                }
                catch (Exception ex)
                {
                }
            }
            else if (Path.GetExtension(entry.Name).ToLower() == ".xml")
            {
                XDocument xmlFile;
                try
                {
                    var byteArray = file.ExtractFile((RpfFileEntry)entry);
                    Stream stream = new MemoryStream(byteArray);
                    xmlFile = XDocument.Load(stream);
                    if (xmlFile.Elements("CPedDamageData").Count() > 0) { fType = FileType.Peddamage_xml; }
                    else if (Path.GetFileNameWithoutExtension(entry.Name).ToLower() == "scaleformpreallocation" && xmlFile.Elements("ScaleformPreallocation").Count() > 0) { fType = FileType.Scaleformpreallocation_xml; }
                }
                catch (Exception ex)
                {
                }
            }
            else if (Path.GetExtension(entry.Name).ToLower() == ".ymt")
            {
                if (Path.GetFileNameWithoutExtension(entry.Name).ToLower().Contains("wantedtuning")) { fType = FileType.Wantedtuning_ymt; }
            }

            return fType;
        }

        private static FileType GetFileType(string path, byte[] byteArray)
        {
            FileType fType = FileType.Unknown;
            if (Path.GetExtension(path).ToLower() == ".rpf") { fType = FileType.RAGEPackageFile; }
            else if (Path.GetExtension(path).ToLower() == ".meta")
            {
                XDocument xmlFile;
                try
                {
                    Stream stream = new MemoryStream(byteArray);
                    xmlFile = XDocument.Load(stream);
                    if (xmlFile.Elements("sPedAccuracyModifiers").Count() > 0) { fType = FileType.Pedaccuracy_meta; }
                    else if (xmlFile.Elements("CHealthConfigInfoManager").Count() > 0) { fType = FileType.Pedhealth_meta; }
                    else if (xmlFile.Elements("CPickupDataManager").Count() > 0) { fType = FileType.Pickups_meta; }
                    else if (xmlFile.Elements("CTaskDataInfoManager").Count() > 0) { fType = FileType.Taskdata_meta; }
                    else if (xmlFile.Elements("CWeaponInfoBlob").Count() > 0) { fType = FileType.Weapons_meta; }
                }
                catch (Exception ex)
                {
                }
            }
            else if (Path.GetExtension(path).ToLower() == ".xml")
            {
                XDocument xmlFile;
                try
                {
                    Stream stream = new MemoryStream(byteArray);
                    xmlFile = XDocument.Load(stream);
                    if (xmlFile.Elements("CPedDamageData").Count() > 0) { fType = FileType.Peddamage_xml; }
                    else if (Path.GetFileNameWithoutExtension(path).ToLower() == "scaleformpreallocation" && xmlFile.Elements("ScaleformPreallocation").Count() > 0) { fType = FileType.Scaleformpreallocation_xml; }
                }
                catch (Exception ex)
                {
                }
            }
            else if (Path.GetExtension(path).ToLower() == ".ymt")
            {
                if (Path.GetFileNameWithoutExtension(path).ToLower().Contains("wantedtuning")) { fType = FileType.Wantedtuning_ymt; }
            }

            return fType;
        }

        private static bool ModifyFile(string arg, bool test)
        {
            FileType fType = GetFileType(arg);

            switch (fType)
            {
                case FileType.Pedaccuracy_meta:
                    {
                        bool retval = PedAccuracyMod(arg, test);
                        if (!test) Console.WriteLine();
                        return retval;
                        break;
                    }
                case FileType.Peddamage_xml:
                    {
                        bool retval = PedDamageMod(arg, test);
                        if (!test) Console.WriteLine();
                        return retval;
                        break;
                    }
                case FileType.Pedhealth_meta:
                    {
                        bool retval = PedHealthMod(arg, test);
                        if (!test) Console.WriteLine();
                        return retval;
                        break;
                    }
                case FileType.Pickups_meta:
                    {
                        bool retval = PickupsMod(arg, test);
                        if (!test) Console.WriteLine();
                        return retval;
                        break;
                    }
                case FileType.Scaleformpreallocation_xml:
                    {
                        bool retval = ScaleformPreallocationMod(arg, test);
                        if (!test) Console.WriteLine();
                        return retval;
                        break;
                    }
                case FileType.Taskdata_meta:
                    {
                        bool retval = TaskdataMod(arg, test);
                        if (!test) Console.WriteLine();
                        return retval;
                        break;
                    }
                case FileType.Weapons_meta:
                    {
                        bool retval = WeaponsMod(arg, test);
                        if (!test) Console.WriteLine();
                        return retval;
                        break;
                    }
                case FileType.Unknown:
                    {
                        return false;
                        break;
                    }
            }
            return false;
        }

        public static void PrintLine(string text, ConsoleColor color, float justify)
        {
            if (text.Length > 0)
            {
                int curPos = (int)((Console.WindowWidth - text.Length) * justify);
                if (curPos < 0) { curPos = 0; }
                if (curPos > Console.BufferWidth) { curPos = Console.BufferWidth; }
                Console.SetCursorPosition(curPos, Console.CursorTop);
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }

        public static void Print(string text, ConsoleColor color, float justify)
        {
            if (text.Length > 0)
            {
                Console.SetCursorPosition((int)((Console.WindowWidth - text.Length) * justify), Console.CursorTop);
                Console.ForegroundColor = color;
                Console.Write(text);
                Console.ResetColor();
            }
        }

        public static bool SubelementsCheckRemove(XElement Xele, bool test, string message = "")
        {
            bool retval = false;
            if (Xele.HasElements)
            {
                retval = true;
                if (!test)
                {
                    Xele.Elements().Remove();
                    if (message != "") { PrintLine(message, ConsoleColor.DarkGray, 0.0f); }
                }
            }
            return retval;
        }

        public static bool ValueCheckRemove(XElement Xele, string check, bool test, string message = "")
        {
            bool retval = false;
            if (Xele.Value.Contains(check))
            {
                retval = true;
                if (!test)
                {
                    Xele.Value = Xele.Value.Replace(check, String.Empty);
                    if (message != "") { PrintLine(message, ConsoleColor.DarkGray, 0.0f); }
                }
            }
            return retval;
        }

        public static bool ValueCheckAdd(XElement Xele, string check, bool test, string message = "")
        {
            bool retval = false;
            if (!Xele.Value.Contains(check))
            {
                retval = true;
                if (!test)
                {
                    Xele.Value = Xele.Value + check;
                    if (message != "") { PrintLine(message, ConsoleColor.DarkGray, 0.0f); }
                }
            }
            return retval;
        }

        public static bool ValueCheckSet(XElement Xele, string check, bool test, string message = "")
        {
            bool retval = false;
            if (Xele.Value != check)
            {
                retval = true;
                if (!test)
                {
                    Xele.Value = check;
                    if (message != "") { PrintLine(message, ConsoleColor.DarkGray, 0.0f); }
                }
            }
            return retval;
        }

        public static bool ValueCheckSet(XElement Xele, string attrib, string check, bool test, string message = "")
        {
            bool retval = false;
            if (Xele.Attribute(attrib).Value != check)
            {
                retval = true;
                if (!test)
                {
                    Xele.Attribute(attrib).Value = check;
                    if (message != "") { PrintLine(message, ConsoleColor.DarkGray, 0.0f); }
                }
            }
            return retval;
        }

        private static bool PedAccuracyMod(string arg, bool test)
        {
            if (!test) { PrintLine("Ped Accuracy Modifier File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f); }

            XDocument xmlFile;
            try
            {
                int changesMade = 0;
                xmlFile = XDocument.Load(arg);
                var query = from c in xmlFile.Elements("sPedAccuracyModifiers")
                            select c;
                foreach (XElement sPedAccuracyModifiers in query)
                {
                    XElement globalMod = sPedAccuracyModifiers.Element("AI_GLOBAL_MODIFIER");
                    if (globalMod != null)
                    {
                        changesMade += ValueCheckSet(globalMod, "value", "1.000000", test, "AI_GLOBAL_MODIFIER decreased to 1.000000") ? 1 : 0;
                    }
                    XElement proVSAiMod = sPedAccuracyModifiers.Element("AI_PROFESSIONAL_PISTOL_VS_AI_MODIFIER");
                    if (proVSAiMod != null)
                    {
                        changesMade += ValueCheckSet(proVSAiMod, "value", "1.000000", test, "AI_PROFESSIONAL_PISTOL_VS_AI_MODIFIER decreased to 1.000000") ? 1 : 0;
                    }
                }
                if (changesMade > 0)
                {
                    if (!test) { xmlFile.Save(arg + (Debug ? "test" : "")); }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                PrintLine("Error loading " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Red, 0.5f);
                return false;
            }
        }

        private static bool PedDamageMod(string arg, bool test)
        {
            if (!test) { PrintLine("Ped Damage Data File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f); }

            XDocument xmlFile;
            try
            {
                int changesMade = 0;
                xmlFile = XDocument.Load(arg);
                var query = from c in xmlFile.Elements("CPedDamageData")
                            select c;
                foreach (XElement CPedDamageData in query)
                {
                    XElement loResTDist = CPedDamageData.Element("LowResTargetDistanceCutoff");
                    if (loResTDist != null)
                    {
                        changesMade += ValueCheckSet(loResTDist, "value", "300.000000", test, "LowResTargetDistanceCutoff increased to 300.000000") ? 1 : 0;
                    }
                    XElement ScarNum = CPedDamageData.Element("NumWoundsToScarsOnDeathSP");
                    if (ScarNum != null)
                    {
                        changesMade += ValueCheckSet(ScarNum, "value", "20", test, "NumWoundsToScarsOnDeathSP increased to 20") ? 1 : 0;
                    }
                    XElement WoundNum = CPedDamageData.Element("MaxPlayerBloodWoundsSP");
                    if (WoundNum != null)
                    {
                        changesMade += ValueCheckSet(WoundNum, "value", "200", test, "MaxPlayerBloodWoundsSP increased to 200") ? 1 : 0;
                    }
                }
                if (changesMade > 0)
                {
                    if (!test) { xmlFile.Save(arg + (Debug ? "test" : "")); }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                PrintLine("Error loading " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Red, 0.5f);
                return false;
            }
        }

        private static bool PedHealthMod(string arg, bool test)
        {
            if (!test) { PrintLine("Health Config File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f); }

            XDocument xmlFile;
            try
            {
                int changesMade = 0;
                xmlFile = XDocument.Load(arg);
                var query = from c in xmlFile.Elements("CHealthConfigInfoManager").Elements("aHealthConfig").Elements("Item")
                            select c;
                foreach (XElement CHealthConfigInfoManager in query)
                {
                    bool disableMeleeOneshot = false;
                    if (!test) { PrintLine(CHealthConfigInfoManager.Element("Name").Value, ConsoleColor.Yellow, 0.0f); }

                    XElement fHealthThresh = CHealthConfigInfoManager.Element("FatiguedHealthThreshold");
                    if (fHealthThresh != null)
                    {
                        if (float.Parse(fHealthThresh.Attribute("value").Value, CultureInfo.InvariantCulture.NumberFormat) > 100.000000f)
                        {
                            changesMade++;
                            if (!test)
                            {
                                disableMeleeOneshot = true;
                                fHealthThresh.Attribute("value").Value = "100.000000";
                                PrintLine("   FatiguedHealthThreshold set to 100.000000", ConsoleColor.DarkGray, 0.0f);
                            }
                        }
                    }
                    XElement hHealthThresh = CHealthConfigInfoManager.Element("HurtHealthThreshold");
                    if (hHealthThresh != null)
                    {
                        if (float.Parse(hHealthThresh.Attribute("value").Value, CultureInfo.InvariantCulture.NumberFormat) > 100.000000f)
                        {
                            changesMade++;
                            if (!test)
                            {
                                disableMeleeOneshot = true;
                                hHealthThresh.Attribute("value").Value = "100.000000";
                                PrintLine("   HurtHealthThreshold set to 100.000000", ConsoleColor.DarkGray, 0.0f);
                            }
                        }
                    }
                    if (disableMeleeOneshot)
                    {
                        XElement meleeFatal = CHealthConfigInfoManager.Element("MeleeCardinalFatalAttackCheck");
                        if (meleeFatal != null)
                        {
                            changesMade += ValueCheckSet(meleeFatal, "value", "false", test, "   MeleeCardinalFatalAttackCheck disabled.") ? 1 : 0;
                        }
                    }

                    Console.WriteLine();
                }
                if (changesMade > 0)
                {
                    if (!test) { xmlFile.Save(arg + (Debug ? "test" : "")); }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                PrintLine("Error loading " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Red, 0.5f);
                return false;
            }
        }

        private static bool PickupsMod(string arg, bool test)
        {
            if (!test) { PrintLine("Pickup Data File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f); }

            XDocument xmlFile;
            try
            {
                int changesMade = 0;
                xmlFile = XDocument.Load(arg);
                var query = from c in xmlFile.Elements("CPickupDataManager").Elements("pickupData").Elements("Item")
                            select c;
                foreach (XElement CPickupData in query)
                {
                    if (CPickupData.Attribute("type").Value != "CPickupData") { continue; }
                    if (!CPickupData.Element("PickupFlags").Value.Contains("CollectableOnFoot")) { continue; }

                    if (!test) { PrintLine(CPickupData.Element("Name").Value, ConsoleColor.Yellow, 0.0f); }
                    XElement ERewards = CPickupData.Element("Rewards");
                    changesMade += SubelementsCheckRemove(ERewards, test, "   Rewards removed.") ? 1 : 0; ;

                    XElement EPickupFlags = CPickupData.Element("PickupFlags");
                    changesMade += ValueCheckRemove(EPickupFlags, "RequiresButtonPressToPickup", test, "   RequiresButtonPressToPickup removed.") ? 1 : 0;
                    changesMade += ValueCheckAdd(EPickupFlags, " CanBeDamaged", test, "   Pickup damage enabled.") ? 1 : 0;

                    if (!test) { Console.WriteLine(); }
                }
                if (changesMade > 0)
                {
                    if (!test) { xmlFile.Save(arg + (Debug ? "test" : "")); }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                PrintLine("Error loading " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Red, 0.5f);
                return false;
            }
        }

        private static bool ScaleformPreallocationMod(string arg, bool test)
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
            if (!test) { PrintLine("Scaleform Preallocation File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f); }

            XDocument xmlFile;
            try
            {
                int changesMade = 0;
                xmlFile = XDocument.Load(arg);
                XElement ScaleformPreallocation = xmlFile.Element("ScaleformPreallocation");
                if (ScaleformPreallocation != null)
                {
                    string fmt = "00";
                    for (int i = 1; i < 17; i++)
                    {
                        bool addInfos = true;
                        foreach (XElement element in ScaleformPreallocation.Elements("movie"))
                        {
                            if (element.Attribute("name").Value == "PICKUP_DISPLAY_" + i.ToString(fmt))
                            {
                                addInfos = false;
                                break;
                            }
                        }
                        if (addInfos)
                        {
                            changesMade++;
                            if (!test)
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
                        }
                    }
                    bool addInfo = true;
                    foreach (XElement element in ScaleformPreallocation.Elements("movie"))
                    {
                        if (element.Attribute("name").Value == "ECG_DISPLAY")
                        {
                            addInfo = false;
                            break;
                        }
                    }
                    if (addInfo)
                    {
                        changesMade++;
                        if (!test)
                        {
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
                    }
                }

                if (changesMade > 0)
                {
                    if (!test) { xmlFile.Save(arg + (Debug ? "test" : "")); }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                PrintLine("Error loading " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Red, 0.5f);
                return false;
            }
        }

        private static bool TaskdataMod(string arg, bool test)
        {
            if (!test) { PrintLine("Task Data Info File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f); }

            XDocument xmlFile;
            try
            {
                int changesMade = 0;
                xmlFile = XDocument.Load(arg);
                var query = from c in xmlFile.Elements("CTaskDataInfoManager").Elements("aTaskData").Elements("Item")
                            select c;
                foreach (XElement CTaskDataInfo in query)
                {
                    var typ = CTaskDataInfo.Attribute("type").Value;
                    if (typ == "CTaskDataInfo")
                    {
                        if (CTaskDataInfo.Element("Name").Value == "STANDARD_PED")
                        {
                            if (!test) { PrintLine(CTaskDataInfo.Element("Name").Value, ConsoleColor.Yellow, 0.0f); }
                            XElement EFlags = CTaskDataInfo.Element("Flags");
                            changesMade += ValueCheckRemove(EFlags, "PreferFleeOnPavements", test, "   PreferFleeOnPavements removed.") ? 1 : 0;
                        }
                    }
                }
                if (changesMade > 0)
                {
                    if (!test) { xmlFile.Save(arg + (Debug ? "test" : "")); }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                PrintLine("Error loading " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Red, 0.5f);
                return false;
            }
        }

        private static bool WeaponsMod(string arg, bool test)
        {
            if (!test) { PrintLine("Weapon Info File - " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Green, 0.5f); }

            XDocument xmlFile;
            try
            {
                int changesMade = 0;
                xmlFile = XDocument.Load(arg);
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
                            if (!insertAmmoHere)
                            {
                                insertAmmoHere = true;
                            }
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
                            if (!CWeaponInfo.Elements("DamageType").Any() || (CWeaponInfo.Element("DamageType").Value != "BULLET" && CWeaponInfo.Element("DamageType").Value != "MELEE"))
                            {
                                continue;
                            }

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
                                                changesMade += ValueCheckSet(EAudio, "AUDIO_ITEM_HEAVYRIFLE", test) ? 1 : 0;
                                                break;
                                            }
                                        case "WEAPON_ADVANCEDRIFLE":
                                            {
                                                changesMade += ValueCheckSet(EAudio, "AUDIO_ITEM_BULLPUPRIFLE", test) ? 1 : 0;
                                                break;
                                            }
                                        case "WEAPON_MG":
                                            {
                                                changesMade += ValueCheckSet(EAudio, "AUDIO_ITEM_ASSAULTMG", test) ? 1 : 0;
                                                break;
                                            }
                                        case "WEAPON_COMBATMG ":
                                            {
                                                changesMade += ValueCheckSet(EAudio, "AUDIO_ITEM_COMBATMG_MK2", test) ? 1 : 0;
                                                break;
                                            }
                                    }
                                }
                            }

                            XElement ETracerFx = CWeaponInfo.Element("Fx").Element("TracerFx");
                            if (ETracerFx != null)
                            {
                                changesMade += ValueCheckSet(ETracerFx, "", test, "   Tracer Effects removed.") ? 1 : 0;
                            }

                            XElement EFlashlightShadows = CWeaponInfo.Element("Fx").Element("FlashFxLightCastsShadows");
                            if (EFlashlightShadows != null)
                            {
                                changesMade += ValueCheckSet(EFlashlightShadows, "value", "true", test, "   Flash Shadows Enabled.") ? 1 : 0;
                            }

                            var q = from k in CWeaponInfo.Elements()
                                    select k;
                            foreach (XElement EInfo in q)
                            {
                                if (EInfo == null) { continue; }
                                if (EInfo.Name == "FireType")
                                {
                                    if (EInfo.Value == "INSTANT_HIT")
                                    {
                                        changesMade++;
                                        if (!test)
                                        {
                                            EInfo.Value = "DELAYED_HIT";
                                            PrintLine("   FireType set to DELAYED_HIT.", ConsoleColor.DarkGray, 0.0f);
                                        }
                                    }
                                }
                                if (EInfo.Name == "RecoilShakeAmplitude")
                                {
                                    changesMade += ValueCheckSet(EInfo, "value", "0.000000", test, "   Stock Recoil Disabled.") ? 1 : 0;
                                }
                                if (EInfo.Name == "HeadShotDamageModifierAI")
                                {
                                    changesMade += ValueCheckSet(EInfo, "value", "1.000000", test, "   Stock AI Headshots Disabled.") ? 1 : 0;
                                }
                                if (EInfo.Name == "HeadShotDamageModifierPlayer")
                                {
                                    changesMade += ValueCheckSet(EInfo, "value", "1.000000", test, "   Stock Player Headshots Disabled.") ? 1 : 0;
                                }
                                if (EInfo.Name == "HitLimbsDamageModifier")
                                {
                                    changesMade += ValueCheckSet(EInfo, "value", "1.000000", test, "   Stock Limb Damage Modifier Disabled.") ? 1 : 0;
                                }
                                if (EInfo.Name == "NetworkHitLimbsDamageModifier")
                                {
                                    changesMade += ValueCheckSet(EInfo, "value", "1.000000", test, "   Stock Net Limb Damage Modifier Disabled.") ? 1 : 0;
                                }
                                if (EInfo.Name == "LightlyArmouredDamageModifier")
                                {
                                    changesMade += ValueCheckSet(EInfo, "value", "1.000000", test, "   Stock Armor Damage Modifier Disabled.") ? 1 : 0;
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
                                if (element == null) { continue; }
                                if (element.Element("Name").Value == ammo.Key)
                                {
                                    addAmmo = false;
                                    break;
                                }
                            }
                            if (addAmmo)
                            {
                                changesMade++;
                                if (!test)
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
                }
                if (changesMade > 0)
                {
                    if (!test) { xmlFile.Save(arg + (Debug ? "test" : "")); }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                PrintLine("Error loading " + Path.GetFileName(arg).ToUpper(), ConsoleColor.Red, 0.5f);
                return false;
            }
        }

        public static bool Question(string q)
        {
            bool answered = false;
            bool answer = false;
            int startTick = Environment.TickCount;
            bool started = false;
            Console.CursorVisible = false;
            while (!started)
            {
                if ((Environment.TickCount - startTick) < 1000)
                {
                    Print(q + " [y/n]", Environment.TickCount % 200 < 100 ? ConsoleColor.Red : ConsoleColor.White, 0.0f);
                    Thread.Sleep(0);
                }
                else
                {
                    PrintLine(q + " [y/n]", ConsoleColor.Yellow, 0.0f);
                    Console.CursorVisible = true;
                    started = true;
                }
            }
            while (!answered)
            {


                ConsoleKeyInfo Key = Console.ReadKey(false);
                switch (Key.Key)
                {
                    case ConsoleKey.Y:
                        {
                            PrintLine("Yes.", ConsoleColor.Green, 0.0f);
                            Console.WriteLine();
                            answer = true;
                            answered = true;
                            break;
                        }
                    case ConsoleKey.N:
                        {
                            PrintLine("No.", ConsoleColor.Red, 0.0f);
                            Console.WriteLine();
                            answer = false;
                            answered = true;
                            break;
                        }
                }

            }
            return answer;
        }
    }

    

    public static class GTAFolder
    {
        public static string CurrentGTAFolder { get; private set; } = Settings.Default.GTADirectory;

        public static bool ValidateGTAFolder(string folder, out string failReason)
        {
            failReason = "";

            if (string.IsNullOrWhiteSpace(folder))
            {
                failReason = "No directory specified";
                return false;
            }

            if (!Directory.Exists(folder))
            {
                failReason = $"directory \"{folder}\" does not exist";
                return false;
            }

            if (!File.Exists(folder + @"\gta5.exe"))
            {
                failReason = $"GTA5.exe not found in directory \"{folder}\"";
                return false;
            }

            return true;
        }

        public static bool ValidateGTAFolder(string folder)
        {
            string reason;
            return ValidateGTAFolder(folder, out reason);
        }

        public static bool IsCurrentGTAFolderValid() => ValidateGTAFolder(CurrentGTAFolder);

        public static bool UpdateGTAFolder(bool UseCurrentIfValid = false)
        {
            if (UseCurrentIfValid && IsCurrentGTAFolderValid())
            {
                return true;
            }

            string origFolder = CurrentGTAFolder;
            string folder = CurrentGTAFolder;
            string SelectedFolder = CurrentGTAFolder;
            
            string source;
            string autoFolder = AutoDetectFolder(out source);
            Program.PrintLine($"Auto-detected game directory \"{autoFolder}\" from {source}.", ConsoleColor.White, 0.0f);
            
            if (autoFolder != null && Program.Question("Continue with auto-detected directory?"))
            {
                SelectedFolder = autoFolder;
            }

            if (Directory.Exists(SelectedFolder))
            {
                folder = SelectedFolder;
            }

            string failReason;
            if (ValidateGTAFolder(folder, out failReason))
            {
                SetGTAFolder(folder);
                if (folder != origFolder)
                {
                    Program.PrintLine($"Successfully changed GTA Directory to \"{folder}\"", ConsoleColor.Green, 0.0f); 
                }
                return true;
            }
            else
            {
                Program.PrintLine($"Directory \"{folder}\" is not a valid GTA directory: \"{failReason}\"", ConsoleColor.Red, 0.0f);
                if (Program.Question($"Do you want to try choosing a different directory?"))
                {
                    return UpdateGTAFolder(false);
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool SetGTAFolder(string folder)
        {
            if (ValidateGTAFolder(folder))
            {
                CurrentGTAFolder = folder;
                Settings.Default.GTADirectory = folder;
                Settings.Default.Save();
                return true;
            }

            return false;
        }

        public static string GetCurrentGTAFolderWithTrailingSlash() => CurrentGTAFolder.EndsWith(@"\") ? CurrentGTAFolder : CurrentGTAFolder + @"\";

        public static bool AutoDetectFolder(out Dictionary<string, string> matches)
        {
            matches = new Dictionary<string, string>();

            if (ValidateGTAFolder(CurrentGTAFolder))
            {
                matches.Add("Saved Settings", CurrentGTAFolder);
            }

            RegistryKey baseKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            string steamPathValue = baseKey32.OpenSubKey(@"Software\Rockstar Games\GTAV")?.GetValue("InstallFolderSteam") as string;
            string retailPathValue = baseKey32.OpenSubKey(@"Software\Rockstar Games\Grand Theft Auto V")?.GetValue("InstallFolder") as string;
            string oivPathValue = Registry.CurrentUser.OpenSubKey(@"Software\NewTechnologyStudio\OpenIV.exe\BrowseForFolder")?.GetValue("game_path_Five_pc") as string;

            if (steamPathValue?.EndsWith("\\GTAV") == true)
            {
                steamPathValue = steamPathValue.Substring(0, steamPathValue.LastIndexOf("\\GTAV"));
            }

            if (ValidateGTAFolder(steamPathValue))
            {
                matches.Add("Steam", steamPathValue);
            }

            if (ValidateGTAFolder(retailPathValue))
            {
                matches.Add("Retail", retailPathValue);
            }

            if (ValidateGTAFolder(oivPathValue))
            {
                matches.Add("OpenIV", oivPathValue);
            }

            return matches.Count > 0;
        }

        public static string AutoDetectFolder(out string source)
        {
            source = null;
            Dictionary<string, string> matches;
            if (AutoDetectFolder(out matches))
            {
                var match = matches.First();
                source = match.Key;
                return match.Value;
            }

            return null;
        }

        public static string AutoDetectFolder()
        {
            string _;
            return AutoDetectFolder(out _);
        }

        public static void UpdateSettings()
        {
            if (string.IsNullOrEmpty(Settings.Default.Key) && (GTA5Keys.PC_AES_KEY != null))
            {
                Settings.Default.Key = Convert.ToBase64String(GTA5Keys.PC_AES_KEY);
                Settings.Default.Save();
                
            }
        }
    }
}
