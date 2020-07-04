using System;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using PaperX_SCG_forms.ViewModels;
using PaperX_SCG_forms.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using Newtonsoft.Json;
using PaperX_SCG_forms.Classes;
using Android.Util;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PaperX_SCG_forms
{
    public partial class App : PrismApplication
    {
        static PaperXItemDatabase database;
        static LogDatabase databaseLog;
        public App(IPlatformInitializer initializer = null) : base(initializer){}

        #region Implement to service page
        public static HardwareClass Hardware;
        public static bool IsWsAuthenticated = false;
        public static t_Authentication LastAuthentication;

        public static void GenerateConfiguration()
        {
            try
            {
                globalVariables.language = "1";

                var filePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                globalVariables.filePath_config = System.IO.Path.Combine(filePath, "config");

                if (!System.IO.Directory.Exists(globalVariables.filePath_config))
                {
                    System.IO.Directory.CreateDirectory(globalVariables.filePath_config);
                }

                globalVariables.filePath_data = System.IO.Path.Combine(filePath, "data");

                if (!System.IO.Directory.Exists(globalVariables.filePath_data))
                {
                    System.IO.Directory.CreateDirectory(globalVariables.filePath_data);
                }

                //This creates the full file path to your "testfile.txt" file.
                globalVariables.filePath_config = System.IO.Path.Combine(globalVariables.filePath_config, "config.json");
                globalVariables.filePath_data = System.IO.Path.Combine(globalVariables.filePath_config, "data.json");


                if (!File.Exists(globalVariables.filePath_config))
                {
                    // server
                    //      dev :    https://paperx.septiem-dev.com
                    //      PROD :   https://www.paperx.net
                    // QR Transpoter
                    //      dev :     "zQcyLNdRCbov23VHTo89dPa/1yltJIgm24NjsW3X3Pislt/fusZSXk1qZs7BWanK";
                    //      prod :       "UvONAxLaeQDjFXiLJgOy22nodA3VhTvLWrlWbpXP2oM=";
                    File.WriteAllText(globalVariables.filePath_config, @"{""data"":{
                          ""APIServer"": ""https://paperx.septiem-dev.com"",
                          ""MachineName"": ""machine01"",
                          ""MachinePassword"": ""Qq123456"",
                          ""TimeCloseDoor"": ""60"",
                            ""TimePageBack"" :  ""180"",
                        ""TimeSendToAPI"" : ""180"",
                        ""QRTransporter"" : ""zQcyLNdRCbov23VHTo89dPa/1yltJIgm24NjsW3X3Pislt/fusZSXk1qZs7BWanK""
                        }}");
                }

                string text = File.ReadAllText(globalVariables.filePath_config);
                globalVariables.config = JsonConvert.DeserializeObject<t_config>(text);

                globalVariables.ws = new cls_ws_scg(globalVariables.config.data.APIServer);

            }
            catch (Exception ex)
            {
                Log.Debug("PPX", ex.Message);
            }
        }

        public static t_Authentication WsAuthentication()
        {
            string SW_Server ;
            string machine_name ;
            string machine_password ;
            string hash  ;
            string timestamp = "";
            string checksum ;
            try
            {
                string result = "";
                // เคลียร์ค่า ticket
           //     globalVariables.ticket = "";
                 SW_Server = globalVariables.config.data.APIServer;
                 machine_name = globalVariables.config.data.MachineName;
                 machine_password = globalVariables.config.data.MachinePassword;
                 hash = new cls_MD5().GetMd5Hash(machine_password).ToString().ToUpper();
                 timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                 checksum = new cls_MD5().GetMd5Hash(machine_name + "|" + hash + "|" + timestamp);

                globalVariables.ws = new cls_ws_scg(SW_Server);
                t_Authentication Authentication;
                int i = 0;
                do
                {
                    App.SaveLog(DateTime.Now, "ppx", timestamp + " : ws-Authentication");
                    result = globalVariables.ws.ws_Authentication(machine_name, hash, timestamp, checksum, "1.0", "", globalVariables.language);
                    Authentication = JsonConvert.DeserializeObject<t_Authentication>(result);
                    if (Authentication.data.success)
                    {
                        globalVariables.ticket = Authentication.data.ticket;
                    }
                    i++;
                }
                 while (globalVariables.ticket == "" && i < 3);           
                IsWsAuthenticated = Authentication.data.success == true;
                LastAuthentication = Authentication;
                return Authentication;
            }
            catch (Exception ex)
            {
                IsWsAuthenticated = false;              
                Log.Debug("PPX", ex.Message);
                 App.SaveLog(DateTime.Now, "ppx",ex.Message);
                LastAuthentication = null;
                return null;
            }
        }
        #endregion

        protected override void OnInitialized()
        {
            InitializeComponent();

           

            GenerateConfiguration();
          //  NavigationService.NavigateAsync("MainPage");
            NavigationService.NavigateAsync("ServicePage");
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
          // containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
           containerRegistry.RegisterForNavigation<ServicePage, ServicePageViewModel>();
        }

        public static PaperXItemDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new PaperXItemDatabase();
                }
                return database;
            }
        }

        public static LogDatabase DatabaseLog
        {
            get
            {
                if (databaseLog == null)
                {
                    databaseLog = new LogDatabase();
                }
                return databaseLog;
            }
        }

        public static async void SaveLog(DateTime dt, string tag, string message)
        {
            LogItem a = new LogItem();
            a.ID = 0;
            a.DateTimeLog = dt;
            a.tagLog = tag;
            a.messageLog = message;
            globalVariables.resultLog = await App.DatabaseLog.SaveItemAsync(a);
        }

        public static async void SaveDBandSendSW()
        {
            var data = await App.Database.GetWeightLastDate();
            //double weight = 0;

            if (data.Count > 0)
            {
                PaperXItem item = new PaperXItem();
                item.ID = data[0].ID;
                item.transactionDate = data[0].transactionDate;
                item.ticket = data[0].ticket;
                item.customerQR = data[0].customerQR;
                item.cusID = data[0].cusID;
                item.cusName = data[0].cusName;
                item.cusType = data[0].cusType;
                item.productId = data[0].productId;
                item.Weight = data[0].Weight;
                item.TotalWeightInBin = data[0].TotalWeightInBin;
                item.statusSaved = true;
                if (data[0].Weight <= 0)
                {
                    item.statusSend = true;
                    int saved = await App.Database.SaveItemAsync(item);
                    //App.Current.MainPage = new MainPage();
                    return;
                }
                string result = globalVariables.ws.ws_SaveBuying(item.ticket, item.customerQR, item.productId, item.Weight, item.transactionDate.ToString("dd/MM/yyyy HH:mm:ss"));
                var SaveBuying = JsonConvert.DeserializeObject<t_SaveBuying>(result);
                // === supot : 2020-02-12 เพิ่มเงื่อนไข Duplicate ให้บันทึกส่งแล้ว
                //  if (SaveBuying.data.success == true)
                if (SaveBuying.data.success == true || SaveBuying.data.message.IndexOf("Duplicate") > -1)

                {
                    item.statusSend = true;
                    int saved = await App.Database.SaveItemAsync(item);
                    globalVariables.SumWeight = 0;
                    App.SaveLog(DateTime.Now, "Save Data", "save data to server finished.");
                    //result = globalVariables.ws.ws_Logout(globalVariables.ticket);
                    //var Logout = JsonConvert.DeserializeObject<t_Logout>(result);

                }
                else
                {
                    Log.Debug("Save Data", "Error save data finished.");
                    App.SaveLog(DateTime.Now, "Save Data", "Error save data to server finished.");
                    int saved = await App.Database.SaveItemAsync(item);
                }
            }
            else
            {
                Log.Debug("PXX", "no data");
                App.SaveLog(DateTime.Now, "PPX", "no data");
            }
            //App.Current.MainPage = new MainPage();
        }

    }
}
