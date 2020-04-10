using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

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
                if (!File.Exists(arg)) //Strip switches and nonsense
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
                    XDocument xmlFile = XDocument.Load(arguments[i]);
                    FileType fType = FileType.Unknown;
                    if (xmlFile.Elements("sPedAccuracyModifiers").Count() > 0) { fType = FileType.Pedaccuracy_meta; }
                    else if (xmlFile.Elements("CPedDamageData").Count() > 0) { fType = FileType.Peddamage_xml; }
                    else if (xmlFile.Elements("CHealthConfigInfoManager").Count() > 0) { fType = FileType.Pedhealth_meta; }
                    else if (xmlFile.Elements("CPickupDataManager").Count() > 0) { fType = FileType.Pickups_meta; }
                    else if (xmlFile.Elements("ScaleformPreallocation").Count() > 0) { fType = FileType.Scaleformpreallocation_xml; }
                    else if (xmlFile.Elements("CTaskDataInfoManager").Count() > 0) { fType = FileType.Taskdata_meta; }
                    else if (xmlFile.Elements("CWeaponInfoBlob").Count() > 0) { fType = FileType.Weapons_meta; }
                    else if (Path.GetFileNameWithoutExtension(arguments[i]).ToLower().Contains("wantedtuning")) { fType = FileType.Wantedtuning_ymt; }

                    switch (fType)
                    {
                        case FileType.Pedaccuracy_meta:
                            {
                                PedAccuracyMod(xmlFile, arguments[i]);
                                Console.WriteLine();
                                break;
                            }
                        case FileType.Peddamage_xml:
                            {
                                PedDamageMod(xmlFile, arguments[i]);
                                Console.WriteLine();
                                break;
                            }
                        case FileType.Pedhealth_meta:
                            {
                                PedHealthMod(xmlFile, arguments[i]);
                                Console.WriteLine();
                                break;
                            }
                        case FileType.Pickups_meta:
                            {
                                PickupsMod(xmlFile, arguments[i]);
                                Console.WriteLine();
                                break;
                            }
                        case FileType.Scaleformpreallocation_xml:
                            {
                                ScaleformPreallocationMod(xmlFile, arguments[i]);
                                Console.WriteLine();
                                break;
                            }
                        case FileType.Taskdata_meta:
                            {
                                TaskdataMod(xmlFile, arguments[i]);
                                Console.WriteLine();
                                break;
                            }
                        case FileType.Weapons_meta:
                            {
                                WeaponsMod(xmlFile, arguments[i]);
                                Console.WriteLine();
                                break;
                            }
                    }
                }
                Console.WriteLine(".meta files edited. Press any key to close.");
                Console.ReadKey();
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
                    PrintLine("AI_GLOBAL_MODIFIER set to 1.000000", ConsoleColor.DarkGray, 0.0f);
                }
                XElement proVSAiMod = sPedAccuracyModifiers.Element("AI_PROFESSIONAL_PISTOL_VS_AI_MODIFIER");
                if (proVSAiMod != null)
                {
                    proVSAiMod.Attribute("value").Value = "1.000000";
                    PrintLine("AI_PROFESSIONAL_PISTOL_VS_AI_MODIFIER  set to 1.000000", ConsoleColor.DarkGray, 0.0f);
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
                    PrintLine("LowResTargetDistanceCutoff set to 300.000000", ConsoleColor.DarkGray, 0.0f);
                }
                XElement ScarNum = CPedDamageData.Element("NumWoundsToScarsOnDeathSP");
                if (ScarNum != null)
                {
                    ScarNum.Attribute("value").Value = "20";
                    PrintLine("NumWoundsToScarsOnDeathSP set to 20", ConsoleColor.DarkGray, 0.0f);
                }
                XElement WoundNum = CPedDamageData.Element("MaxPlayerBloodWoundsSP");
                if (WoundNum != null)
                {
                    WoundNum.Attribute("value").Value = "200";
                    PrintLine("MaxPlayerBloodWoundsSP set to 200", ConsoleColor.DarkGray, 0.0f);
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
