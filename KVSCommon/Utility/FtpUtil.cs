using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using KVSCommon.Database;

namespace KVSCommon.Utility
{
    public class FtpUtil
    {
       public void SendData(String content)
        {
           try
           {

            var server = ConfigurationManager.AppSettings["FtpServer"];
            var username = ConfigurationManager.AppSettings["FtpUsername"];
            var pwd = ConfigurationManager.AppSettings["FtpPassword"];


            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential(username, pwd);
                client.UploadString(server, "STOR",content);
            }

           }
           catch (Exception ex)
           {

               throw ex;
           }
        }
    }
}
