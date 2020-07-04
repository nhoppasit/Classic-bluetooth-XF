using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;

using System;
using System.IO;
using Newtonsoft.Json;
using Android.Content.Res;
using System.Net;
using System.Data;
using System.Security.Cryptography;
using Android.Media;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

public class cls_ws_scg
{
    string SW_Server;
    int ws_timeout = 10000;

    public cls_ws_scg(string sw_server)
    {
        SW_Server = sw_server;
    }
  
    
    public string ws_Authentication(string username, string password, string timestamp, string checkSum, string softwareVersion, string notificationToken, string languege)
    {
        try
        {
            HttpClient client = new HttpClient();
            string apiUrl = SW_Server + "/MachineService/Authentication";
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            p_Authentication p = new p_Authentication();
            p.checkSum = checkSum;
            p.languege = languege;
            p.notificationToken = notificationToken;
            p.password = password;
            p.softwareVersion = softwareVersion;
            p.timestamp = timestamp;
            p.username = username;

            var serializedProduct = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedProduct, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {

                System.IO.Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string contents = readStream.ReadToEnd();
                return @"{""data"":" + contents.ToString() + "}";
            }
            else
            {
                return @"{""data"":{""success"":false,""message"":""" + response.RequestMessage + @"""}}";
            }
        }
        catch(Exception ex)
        {
            return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        }
    }

    public string ws_ValidateQRCode(string ticket, string qrCode)
    {
        try
        {
            HttpClient client = new HttpClient();
            string apiUrl = SW_Server + "/MachineService/ValidateQRCode";
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            p_ValidateQRCode p = new p_ValidateQRCode();
            p.qrCode = qrCode;

            var serializedProduct = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedProduct, System.Text.Encoding.UTF8, "application/json");
            content.Headers.Add("ticket", ticket);
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                System.IO.Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string contents = readStream.ReadToEnd();
                return @"{""data"":" + contents.ToString() + "}";
            }
            else
            {
                return @"{""data"":{""success"":false,""message"":""" + response.RequestMessage + @"""}}";
            }
        }
        catch(Exception ex)
        {
           return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        }   
    }

    public string ws_SaveBuying(string ticket, string customerQR, int productId, double weight, string transactionDate)
    {
        try
        {
            HttpClient client = new HttpClient();
            string apiUrl = SW_Server + "/MachineService/SaveBuying";
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            p_SaveBuying p = new p_SaveBuying();
            p.customerQR = customerQR;
            p.productId = productId;
            p.transactionDate = transactionDate;
            p.weight = weight;

            var serializedProduct = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedProduct, System.Text.Encoding.UTF8, "application/json");
            content.Headers.Add("ticket", ticket);
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                System.IO.Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string contents = readStream.ReadToEnd();
                return @"{""data"":" + contents.ToString() + "}";
            }
            else
            {
                return @"{""data"":{""success"":false,""message"":""" + response.RequestMessage + @"""}}";
            }
        }
        catch (Exception ex)
        {
            return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        }
    }

    public string ws_carryOut(string ticket)
    {
        try
        {
            HttpClient client = new HttpClient();
            string apiUrl = SW_Server + "/MachineService/carryOut";
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var serializedProduct = JsonConvert.SerializeObject("");
            var content = new StringContent(serializedProduct, System.Text.Encoding.UTF8, "application/json");
            content.Headers.Add("ticket", ticket);
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                System.IO.Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string contents = readStream.ReadToEnd();
                return @"{""data"":" + contents.ToString() + "}";
            }
            else
            {
                return @"{""data"":{""success"":false,""message"":""" + response.RequestMessage + @"""}}";
            }
        }
        catch (Exception ex)
        {
            return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        }
    }

    public string ws_SendMachineStatus(string ticket, int machineStatusId, string moreDetail, int productid, double percentBinFull, double weightBinFull)
    {
        try
        {
            HttpClient client = new HttpClient();
            string apiUrl = SW_Server + "/MachineService/SendMachineStatus";
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            p_productCapacity t = new p_productCapacity();
            t.percentBinFull = percentBinFull;
            t.productid = productid;
            t.weightBinFull = weightBinFull;

            p_SendMachineStatus p = new p_SendMachineStatus();
            p.machineStatusId = machineStatusId;
            p.moreDetail = moreDetail;
            p.productCapacity = JsonConvert.SerializeObject(t);



            var serializedProduct = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedProduct, System.Text.Encoding.UTF8, "application/json");
            content.Headers.Add("ticket", ticket);
        //    content.Headers.Add("Content-Type", "application/json");
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                System.IO.Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string contents = readStream.ReadToEnd();
                return @"{""data"":" + contents.ToString() + "}";
            }
            else
            {
                return @"{""data"":{""success"":false,""message"":""" + response.RequestMessage + @"""}}";
            }
        }
        catch(Exception ex)
        {
           return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        }   

        //    // create a request
        //    HttpWebRequest request = (HttpWebRequest)
        //WebRequest.Create(string.Format(@"" + SW_Server + "/MachineService/SendMachineStatus"));
        //request.KeepAlive = false;
        //request.ProtocolVersion = HttpVersion.Version10;
        //request.Method = "POST";
        //request.Headers.Add("ticket", ticket);


        //// turn our request string into a byte stream
        //string inputData = "{\"machineStatusId\":" + machineStatusId + ",\"moreDetail\":\"" + WebUtility.UrlEncode(moreDetail)  + "\"," +
        //                        "\"productCapacity\" : [{\"id\":" + productid + ",\"percent\":" + percentBinFull + ",\"weight\":" + weightBinFull + "}]}";
        //byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(inputData);

        //// this is important - make sure you specify type this way
        //request.ContentType = "application/json; charset=UTF-8";
        //request.Accept = "application/json";
        //request.ContentLength = postBytes.Length;
        //request.Timeout = ws_timeout;
        ////   request.CookieContainer  = Cookies;
        ////  request.UserAgent = currentUserAgent;
        //System.IO.Stream requestStream = request.GetRequestStream();

        //// now send it
        //requestStream.Write(postBytes, 0, postBytes.Length);
        //requestStream.Close();

        //// grab te response and print it out to the console along with the status code
        //try
        //{
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    string result;
        //    using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
        //    {
        //        result = rdr.ReadToEnd();
        //    }
        //    return @"{""data"":" + result.ToString() + "}";
        //}
        //catch (Exception ex)
        //{
        //    return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        //}
    }

    public string ws_Logout(string ticket)
    {
        try
        {
            HttpClient client = new HttpClient();
            string apiUrl = SW_Server + "/MachineService/Logout";
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var serializedProduct = JsonConvert.SerializeObject("");
            var content = new StringContent(serializedProduct, System.Text.Encoding.UTF8, "application/json");
            content.Headers.Add("ticket", ticket);
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                System.IO.Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string contents = readStream.ReadToEnd();
                return @"{""data"":" + contents.ToString() + "}";
            }
            else
            {
                return @"{""data"":{""success"":false,""message"":""" + response.RequestMessage + @"""}}";
            }
        }
        catch (Exception ex)
        {
            return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        }
    }


    public string ws_GetMasterProduct(string ticket)
    {
        try
        {
            HttpClient client = new HttpClient();
            string apiUrl = SW_Server + "/MachineService/MasterProduct";
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var serializedProduct = JsonConvert.SerializeObject("");
            var content = new StringContent(serializedProduct, System.Text.Encoding.UTF8, "application/json");
            content.Headers.Add("ticket", ticket);
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                System.IO.Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string contents = readStream.ReadToEnd();
                return @"{""data"":" + contents.ToString() + "}";
            }
            else
            {
                return @"{""data"":{""success"":false,""message"":""" + response.RequestMessage + @"""}}";
            }
        }
        catch (Exception ex)
        {
            return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        }
    }


    public string ws_GetMasterAdvertise(string ticket)
    {
        try
        {
            HttpClient client = new HttpClient();
            string apiUrl = SW_Server + "/MachineService/GetMasterAdvertise";
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var serializedProduct = JsonConvert.SerializeObject("");
            var content = new StringContent(serializedProduct, System.Text.Encoding.UTF8, "application/json");
            content.Headers.Add("ticket", ticket);
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                System.IO.Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string contents = readStream.ReadToEnd();
                return @"{""data"":" + contents.ToString() + "}";
            }
            else
            {
                return @"{""data"":{""success"":false,""message"":""" + response.RequestMessage + @"""}}";
            }
        }
        catch (Exception ex)
        {
            return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        }

    }


    public string ws_MasterGeneralData(string token)
    {
        try
        {
            HttpClient client = new HttpClient();
            string apiUrl = SW_Server + "/MachineService/getMasterGeneralData";
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var serializedProduct = JsonConvert.SerializeObject("");
            var content = new StringContent(serializedProduct, System.Text.Encoding.UTF8, "application/json");
            content.Headers.Add("token", token);
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                System.IO.Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string contents = readStream.ReadToEnd();
                return @"{""data"":" + contents.ToString() + "}";
            }
            else
            {
                return @"{""data"":{""success"":false,""message"":""" + response.RequestMessage + @"""}}";
            }
        }
        catch (Exception ex)
        {
            return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        }
    }

    public string ws_MasterPartnerRedeemData(string ticket)
    {
        try
        {
            HttpClient client = new HttpClient();
            string apiUrl = SW_Server + "/MachineService/MasterPartnerRedeemData";
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var serializedProduct = JsonConvert.SerializeObject("");
            var content = new StringContent(serializedProduct, System.Text.Encoding.UTF8, "application/json");
            content.Headers.Add("ticket", ticket);
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                System.IO.Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string contents = readStream.ReadToEnd();
                return @"{""data"":" + contents.ToString() + "}";
            }
            else
            {
                return @"{""data"":{""success"":false,""message"":""" + response.RequestMessage + @"""}}";
            }
        }
        catch (Exception ex)
        {
            return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        }
    }



    public string ws_Decrypt(string value)
    {
        try
        {
            HttpClient client = new HttpClient();
            string apiUrl = SW_Server + "/MachineService/Dx";
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            p_Dx p = new p_Dx();
            p.id = value;

            var serializedProduct = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedProduct, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                System.IO.Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string contents = readStream.ReadToEnd();
                return @"{""data"":" + contents.ToString() + "}";
            }
            else
            {
                return @"{""data"":{""success"":false,""message"":""" + response.RequestMessage + @"""}}";
            }
        }
        catch (Exception ex)
        {
            return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        }


    }

    public string ws_Encrypt(string value)
    {
        try
        {
            HttpClient client = new HttpClient();
            string apiUrl = SW_Server + "/MachineService/Ex";
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            p_Ex p = new p_Ex();
            p.id = value;

            var serializedProduct = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedProduct, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {
                System.IO.Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string contents = readStream.ReadToEnd();
                return @"{""data"":" + contents.ToString() + "}";
            }
            else
            {
                return @"{""data"":{""success"":false,""message"":""" + response.RequestMessage + @"""}}";
            }
        }
        catch (Exception ex)
        {
            return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        }
    }

    public string ws_Checksum(string username, string password, string timestamp)
    {
        try
        {
            HttpClient client = new HttpClient();
            string apiUrl = SW_Server + "/MachineService/Checksum";
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            p_Checksum p = new p_Checksum();
            p.password = password;
            p.timestamp = timestamp;
            p.username = username;

            var serializedProduct = JsonConvert.SerializeObject(p);
            var content = new StringContent(serializedProduct, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
            if (response.IsSuccessStatusCode)
            {

                System.IO.Stream receiveStream = response.Content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);
                string contents = readStream.ReadToEnd();
                return @"{""data"":" + contents.ToString() + "}";
            }
            else
            {
                return @"{""data"":{""success"":false,""message"":""" + response.RequestMessage + @"""}}";
            }
        }
        catch (Exception ex)
        {
            return @"{""data"":{""success"":false,""message"":""" + ex.Message + @"""}}";
        }


    }


}
