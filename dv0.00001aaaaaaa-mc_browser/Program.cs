using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace dv0._00001aaaaaaa_mc_browser
{
    class Program
    {
        static void Main(string[] args)
        {
            // Init Console color to dark red
            ConsoleColor initColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = initColor;
            int ExitCode = 0;

            for (; ; )
            {
                displayObject();
                string iCommand = Console.ReadLine();

                Console.WriteLine("");
                if (iCommand == "help")
                {
                    help();
                }

                else if (iCommand.Contains("getuserinfo"))
                {
                    try
                    {
                        string username = iCommand.Split(" ")[1];

                        if (username == "")
                        {
                            Console.WriteLine("error excepted: <2>");
                            Console.WriteLine("Reason: is their name fucking seriously SPACE?");

                            ConsoleKeyInfo ErrorKey2;
                            ErrorKey2 = Console.ReadKey(true);

                            continue;
                        }

                        getUserInfo(username);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("error excepted: <1>");
                        Console.WriteLine("Reason: is their name fucking seriously SPACE?");

                        ConsoleKeyInfo ErrorKey1;
                        ErrorKey1 = Console.ReadKey(true);

                        continue;
                    }
                }

                else if (iCommand == "havingusers")
                {
                    havingusers();
                }

                else if (iCommand.Contains("displayuserinfo"))
                {
                    try
                    {
                        string uuid = iCommand.Split(" ")[1];

                        if (uuid == "")
                        {
                            Console.WriteLine("error excepted: <8>");
                            Console.WriteLine("Reason: is their name fucking seriously SPACE?");

                            ConsoleKeyInfo ErrorKey8;
                            ErrorKey8 = Console.ReadKey(true);
                            continue;
                        }

                        displayUserInfo(uuid);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("error excepted: <9>");
                        Console.WriteLine("Reason: is their name fucking seriously SPACE?");

                        ConsoleKeyInfo ErrorKey9;
                        ErrorKey9 = Console.ReadKey(true);
                        continue;
                    }
                }

                else if (iCommand == "exit")
                {
                    ExitCode = 1;
                }

                if (ExitCode == 1) break;
            }

            Console.WriteLine("[>] goodbye. (press any key)");
            ConsoleKeyInfo exitKey;
            exitKey = Console.ReadKey(true);

            return;
        }

        static void displayObject()
        {

            for (int i = 0; i < 100; i++) Console.WriteLine("");

            Console.WriteLine("__________.__                    .___");
            Console.WriteLine("\\______   \\  |   ____   ____   __| _/");
            Console.WriteLine(" |    |  _/  | _/ __ \\_/ __ \\ / __ | ");
            Console.WriteLine(" |    |   \\  |_\\  ___/\\  ___// /_/ | ");
            Console.WriteLine(" |______  /____/\\___  >\\___  >____ | ");
            Console.WriteLine("        \\/          \\/     \\/     \\/ ");
            Console.WriteLine("[>] hi. welcome again. \"help\" to see commands.");
            return;
        }

        static void help()
        {
            for (int i = 0; i < 100; i++) Console.WriteLine("");

            Console.WriteLine("\"help\":");
            Console.WriteLine("    usage: help");
            Console.WriteLine("    help: displays this. nothing anymore.\n");

            Console.WriteLine("\"getuserinfo\":");
            Console.WriteLine("    usage: getuserinfo <username>");
            Console.WriteLine("    help: saves data to ./userdata/<uuid>.txt");
            Console.WriteLine("    if minecraft server status is not ok, this command will not executed.\n");

            Console.WriteLine("\"displayuserinfo\":");
            Console.WriteLine("    usage: displayuserinfo <uuid>");
            Console.WriteLine("    help: displays data of <uuid>.");
            Console.WriteLine("     Depends on their txt file.\n");

            Console.WriteLine("\"havingusers\":");
            Console.WriteLine("    usage: havingusers");
            Console.WriteLine("    help: displays all of users you have.\n");

            Console.WriteLine("\"exit\":");
            Console.WriteLine("    usage: exit");
            Console.WriteLine("    help: ...who asked twice times?\n");

            Console.WriteLine("[>] press any key when you read this.");
            ConsoleKeyInfo exitKey;
            exitKey = Console.ReadKey(true);
            return;
        }

        static void getUserInfo(string name)
        {
            // Get Timestamp
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            // Checking if minecraft api server is OK
            WebRequest statusRequest = WebRequest.Create("https://api.mojang.com");
            statusRequest.Credentials = CredentialCache.DefaultCredentials;
            WebResponse statusResponse = statusRequest.GetResponse();
            if (((HttpWebResponse)statusResponse).StatusDescription != "OK")
            {
                Console.WriteLine("error excepted: <4>");
                Console.WriteLine("Reason: minecraft api server is fucked now.");

                ConsoleKeyInfo ErrorKey4;
                ErrorKey4 = Console.ReadKey();

                return;
            }
            statusResponse.Close();

            string jsonUUID;
            WebRequest uidRequest = WebRequest.Create("https://api.mojang.com/users/profiles/minecraft/" + name + "?at=" + unixTimestamp);
            uidRequest.Credentials = CredentialCache.DefaultCredentials;
            WebResponse uidResponse = uidRequest.GetResponse();
            using (Stream uidDataStream = uidResponse.GetResponseStream())
            {
                StreamReader uidReader = new StreamReader(uidDataStream);
                jsonUUID = uidReader.ReadToEnd();
            }

            // if that name not exist
            if (jsonUUID == "")
            {
                Console.WriteLine("error excepted: <5>");
                Console.WriteLine("Reason: uuid with that name doesn\'t exist. is that player\'s name changed?");

                ConsoleKeyInfo ErrorKey5;
                ErrorKey5 = Console.ReadKey();

                return;
            }

            JObject uidObj = JObject.Parse(jsonUUID);
            string uuid = uidObj["id"].ToString();

            Console.WriteLine("Got UUID... 1/3");
            uidResponse.Close();

            string jsonNH;
            WebRequest nhRequest = WebRequest.Create("https://api.mojang.com/user/profiles/" + uuid + "/names");
            nhRequest.Credentials = CredentialCache.DefaultCredentials;
            WebResponse nhResponse = nhRequest.GetResponse();
            using (Stream nhDataStream = nhResponse.GetResponseStream())
            {
                StreamReader nhReader = new StreamReader(nhDataStream);
                jsonNH = nhReader.ReadToEnd();
            }

            string[] lists = String_Cutby(jsonNH);
            List<string> NameHistory_Name = new List<string>();
            List<DateTime> NameHistory_Time = new List<DateTime>();
            bool isPlayerFirstName = false;

            foreach (string l in lists)
            {
                string[] dummyL = l.Split(":");
                string[] dummyR = l.Split(",");

                if (dummyL.Length == 2) // if it is first name
                {
                    dummyL[1] = dummyL[1].Replace("\"", "");
                    NameHistory_Name.Add(dummyL[1]);
                }
                else
                {
                    string[] dummyLL1; string[] dummyLL2;
                    dummyLL1 = dummyR[0].Split(":"); dummyLL2 = dummyR[1].Split(":");
                    dummyLL1[1] = dummyLL1[1].Replace("\"", ""); dummyLL2[1] = dummyLL2[1].Replace("\"", "");
                    NameHistory_Name.Add(dummyLL1[1]);

                    int newUnixTimeStamp = Int32.Parse((dummyLL2[1].ToString()).Substring(0, 10));
                    NameHistory_Time.Add(UnixTimeStampToDateTime(newUnixTimeStamp));
                }
            }

            if (NameHistory_Time.Count == 0) isPlayerFirstName = true;
            Console.WriteLine("Got name history... 2/3");
            nhResponse.Close();

            string jsonData;
            WebRequest userReq = WebRequest.Create("https://sessionserver.mojang.com/session/minecraft/profile/" + uuid);
            userReq.Credentials = CredentialCache.DefaultCredentials;
            WebResponse userResponse = userReq.GetResponse();
            using (Stream userDataStream = userResponse.GetResponseStream())
            {
                StreamReader userReader = new StreamReader(userDataStream);
                jsonData = userReader.ReadToEnd();
            }

            JObject userValue = JObject.Parse(jsonData);
            string EncodedBase64SkinAndCapeData = userValue["properties"][0]["value"].ToString();

            string DecodedBase64JsonData = Base64Decode(EncodedBase64SkinAndCapeData);

            /*
            DecodedBase64JsonData seems like this v
            {
                "timestamp" : 1606694075947,
                "profileId" : "250df05ffdf247d4915720bbab8fa1fe",
                "profileName" : "kwerts",
                "textures" : {
                "SKIN" : {
                    "url" : "http://textures.minecraft.net/texture/a4b214fb32ad6847a32aac7cc9ec1fdc7097c55a440a6c43b483cbd48f462dca",
                    "metadata" : {
                        "model" : "slim"
                        }
                    }
                }
            }
             */

            JObject DecodedBase64Value = JObject.Parse(DecodedBase64JsonData);
            string SKINVALUE = DecodedBase64Value["textures"]["SKIN"]["url"].ToString();
            string CAPEVALUE;
            try
            {
                CAPEVALUE = DecodedBase64Value["textures"]["CAPE"]["url"].ToString();
            }
            catch (Exception)
            {
                CAPEVALUE = "this player does not have cape(s) ";
            }

            Console.WriteLine("Got skin and cape url... 3/3");
            userResponse.Close();

            string dir = "./userdata";
            string path = "./userdata/" + uuid + ".txt";
            List<string> writingLines = new List<string>();

            // 1. Add UUID and nowname
            writingLines.Add("UUID: " + uuid);
            writingLines.Add("Nowname: " + name);
            writingLines.Add("");

            // 2. Add Name history
            if (isPlayerFirstName)
            {
                foreach (string listIndex in NameHistory_Name)
                {
                    writingLines.Add("1. Name: " + listIndex);
                }
            }
            else
            {
                for (int i = 0; i < NameHistory_Name.Count; i++)
                {
                    try
                    {
                        writingLines.Add((i + 1).ToString() + ". Name: " + NameHistory_Name[i] + ", ChangedAt: " + NameHistory_Time[i - 1].ToString("yyyy/MM/dd hh:mm:ss"));
                    }
                    catch (Exception) // catch if it is first name
                    {
                        writingLines.Add("1. Name: " + NameHistory_Name[i]);
                    }
                }
            }
            writingLines.Add("");

            // 3. Add skin and Cape url
            writingLines.Add("Skin URL: " + SKINVALUE);
            writingLines.Add("Cape URL: " + CAPEVALUE);

            string[] dWriting = writingLines.ToArray();
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (dirInfo.Exists == false)
            {
                dirInfo.Create();
            }

            using (var stream = File.Create(path))
            {
                using (TextWriter tw = new StreamWriter(stream))
                {
                    foreach (string d in dWriting)
                    {
                        tw.WriteLine(d);
                    }
                }
            }

            Console.WriteLine("All done! It saved at ./userdata/" + uuid + ".txt");
            Console.WriteLine("if you wanna see info in this, copy this: [displayuserinfo " + uuid + "]");
            Console.WriteLine("[>] press any key to pay respect to mojang server");

            ConsoleKeyInfo ExitKey;
            ExitKey = Console.ReadKey();

            return;
        }
        
        static void displayUserInfo(string uuid)
        {
            string path = "./userdata/" + uuid + ".txt";
            string Texts;

            try
            {
                Texts = File.ReadAllText(path);
                Console.WriteLine("\n=====================================\n");
                Console.WriteLine(Texts);
                Console.WriteLine("\n=====================================\n");
            }
            catch (Exception)
            {
                Console.WriteLine("error excepted: <7>");
                Console.WriteLine("Reason: you\'re trying with wrong UUID.");

                ConsoleKeyInfo ErrorKey7;
                ErrorKey7 = Console.ReadKey();

                return;
            }

            Console.WriteLine("[>] Press any key.");
            ConsoleKeyInfo ExitKey;
            ExitKey = Console.ReadKey();

            return;
        }

        static void havingusers()
        {
            DirectoryInfo UUIDInfo = new DirectoryInfo("./userdata");
            if (!UUIDInfo.Exists)
            {
                Console.WriteLine("error excepted: <6>");
                Console.WriteLine("Reason: why are you trying this lol");

                ConsoleKeyInfo ErrorKey6;
                ErrorKey6 = Console.ReadKey(true);

                return;
            }

            int cnt = 0;
            FileInfo[] UUIDFiles = UUIDInfo.GetFiles();
            foreach (FileInfo uidFile in UUIDFiles)
            {
                string[] fileLines = File.ReadAllLines(uidFile.FullName);
                string username = fileLines[1].Split(" ")[1];
                Console.WriteLine((cnt + 1).ToString() + ". " + (uidFile.Name).Split(".")[0] + " , MinecraftName: " + username);
                cnt++;
            }

            Console.WriteLine("\n[>] That\'s it! Press any key!");
            ConsoleKeyInfo ExitKey;
            ExitKey = Console.ReadKey();

            return;
        }

        static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        static string[] String_Cutby(string cuttingString)
        {
            List<string> toReturn = new List<string>();
            int first = 0; int second = 0;

            for (int i = 0; i < cuttingString.Length; i++)
            {
                if (cuttingString[i] == '{')
                {
                    first = i + 1;
                }
                if (cuttingString[i] == '}')
                {
                    second = i;
                    toReturn.Add(cuttingString.Substring(first, second - first));
                    first = 0; second = 0;
                }
            }

            string[] nowReturn = toReturn.ToArray();
            return nowReturn;
        }

        static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
