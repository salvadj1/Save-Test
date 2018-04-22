using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fougerite;
using System.Collections;
using RustProto;
using RustProto.Helpers;
using System.IO;
using System.ComponentModel;

namespace ClassLibrary1
{
    public class Class1: Fougerite.Module
    {
        public override string Name { get { return "Server Save"; } }
        public override string Author { get { return "Salva/Juli"; } }
        public override string Description { get { return "FFFFFFFDDDDDDDDDDDDDD"; } }
        public override Version Version { get { return new Version("1.0"); } }

        public DateTime SaveStartTime = DateTime.Now;
        public DateTime SaveEndTime = DateTime.Now;
        public int ObjectsCount = 0;
        // path = ServerSaveManager.autoSavePath;
        public override void Initialize()
        {
            Hooks.OnCommand += OnCommand;
            Hooks.OnConsoleReceived += OnConsoleReceived;
            Hooks.OnServerInit += OnServerInit;
        }
        public override void DeInitialize()
        {
            Hooks.OnCommand -= OnCommand;
            Hooks.OnConsoleReceived -= OnConsoleReceived;
            Hooks.OnServerInit -= OnServerInit;
        }
        public void OnServerInit()
        {
            //ConsoleSystem.Run("save.autosavetime " + int.MaxValue, false);//dretax way
        }
        public void OnConsoleReceived(ref ConsoleSystem.Arg arg, bool external)
        {
            if (arg.Class == "asave" && arg.Function == "save" && ((arg.argUser != null && arg.argUser.admin) || arg.argUser == null))
            {
                StartSaveBW();
            }
        }

        public void OnCommand(Fougerite.Player player, string cmd, string[] args)
        {
            if (cmd == "save1")
            {
                StartSave();
            }
            if (cmd == "save2")
            {
                StartSaveBW();
            }
        }
        public void StartSave()
        {
            SaveStartTime = DateTime.Now;

            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\save\server_data\BackUpSaves\"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\save\server_data\BackUpSaves\");
            }
            try
            {
                if (File.Exists(Directory.GetCurrentDirectory() + @"\save\server_data\rust_island_2013.sav"))
                {
                    string d = System.DateTime.Now.Day.ToString();
                    string h = System.DateTime.Now.Hour.ToString();
                    string m = System.DateTime.Now.Minute.ToString();
                    string s = System.DateTime.Now.Second.ToString();
                    string date = "Day " + d + " Hour " + h + "-" + m + "-" + s;
                    string name = "rust_island_2013 " + date + ".sav";
                    File.Copy(Directory.GetCurrentDirectory() + @"\save\server_data\rust_island_2013.sav", Directory.GetCurrentDirectory() + @"\save\server_data\BackUpSaves\" + name, true);
                }
            }
            catch (Exception ex)
            {
            }

            WorldSave fsave;
            using (Recycler<WorldSave, WorldSave.Builder> recycler = WorldSave.Recycler())
            {
                WorldSave.Builder builder = recycler.OpenBuilder();
                ServerSaveManager.Get(false).DoSave(ref builder);
                fsave = builder.Build();
            }

            ObjectsCount = fsave.SceneObjectCount + fsave.InstanceObjectCount;

            FileStream stream2 = File.Open(Directory.GetCurrentDirectory() + @"\save\server_data\rust_island_2013.sav", FileMode.Create, FileAccess.Write);
            fsave.WriteTo(stream2);
            stream2.Flush();
            stream2.Dispose(); //??

            SaveEndTime = DateTime.Now;
            AnnounceResults();
        }
        public void StartSaveBW()
        {
            BackgroundWorker BGW = new BackgroundWorker();
            BGW.DoWork += new DoWorkEventHandler(SaveBW);
            BGW.RunWorkerAsync();
        }
        public void SaveBW(object sender, DoWorkEventArgs e)
        {
            SaveStartTime = DateTime.Now;

            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\save\server_data\BackUpSaves\"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\save\server_data\BackUpSaves\");
            }
            try
            {
                if (File.Exists(Directory.GetCurrentDirectory() + @"\save\server_data\rust_island_2013.sav"))
                {
                    string d = System.DateTime.Now.Day.ToString();
                    string h = System.DateTime.Now.Hour.ToString();
                    string m = System.DateTime.Now.Minute.ToString();
                    string s = System.DateTime.Now.Second.ToString();
                    string date = "Day " + d + " Hour " + h + "-" + m + "-" + s;
                    string name = "rust_island_2013 " + date + ".sav";
                    File.Copy(Directory.GetCurrentDirectory() + @"\save\server_data\rust_island_2013.sav", Directory.GetCurrentDirectory() + @"\save\server_data\BackUpSaves\" + name, true);
                }
            }
            catch (Exception ex)
            {
            }

            WorldSave fsave;
            using (Recycler<WorldSave, WorldSave.Builder> recycler = WorldSave.Recycler())
            {
                WorldSave.Builder builder = recycler.OpenBuilder();
                ServerSaveManager.Get(false).DoSave(ref builder);
                fsave = builder.Build();
            }

            ObjectsCount = fsave.SceneObjectCount + fsave.InstanceObjectCount;

            FileStream stream2 = File.Open(Directory.GetCurrentDirectory() + @"\save\server_data\rust_island_2013.sav", FileMode.Create, FileAccess.Write);
            fsave.WriteTo(stream2);
            stream2.Flush();
            stream2.Dispose(); //??

            SaveEndTime = DateTime.Now;
            AnnounceResults();
        }

        public void AnnounceResults()
        {
            Loom.QueueOnMainThread(() =>
               {
                   TimeSpan DifTime = SaveEndTime.Subtract(SaveStartTime);
                   Logger.Log(Name + " " + ObjectsCount + " Object(s). Took " + DifTime.Seconds + "." + DifTime.Milliseconds + " seconds save them in the backgound");
                   Server.GetServer().BroadcastFrom(Name, ObjectsCount + " Object(s). Took " + DifTime.Seconds + "." + DifTime.Milliseconds + " seconds save them in the backgound");

               }); return;
        }
       
    }
}
