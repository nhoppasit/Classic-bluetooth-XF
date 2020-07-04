using System;
using System.Collections.Generic;
using System.Text;

namespace PaperX_SCG_forms
{
 public static   class globalVariables
    {
        public static bool BootFirstTime = true;
        public static bool isTest = false;

        public static cls_ws_scg ws;
        public static string ticket = "";
        public static string language;

        public static object Authentication;
        public static object ValidateQRCode;

        public static string filePath_config;
        public static string filePath_log;
        public static string filePath_data;

        public static string[] customer; // id/customername/usertype
        public static string customerQR;
        public static string ProductID;
        public static double SumWeight;
        public static double Weight;
        public static t_config config;
        public static int NumclickSetting;

        public static double percentBinFull;
        public static double weightBinFull;

        public static double PreviousWeight;
        public static string ErrorCode = "00";

        public static bool ShutterHomePosition_OK = false;
        public static bool KikerHomePosition_OK =false;

        public static int idPaperX;
        public static int resultLog;

        public static int AdsVersion = 0;
        public static int PartnerVesion = 0;

        public static string saveTree = "0";
        public static t_MasterAdvertise Master_Ads;
        public static t_MasterPartnerRedeemData Master_Partner;

        public static int timeGotoMainPage = 30;

        public static Android.App.Activity activity;

    }
}
