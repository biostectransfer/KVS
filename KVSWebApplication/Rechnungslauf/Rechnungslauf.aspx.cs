using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Threading;

namespace KVSWebApplication.Rechnungslauf
{
    public partial class Rechnungslauf : System.Web.UI.Page
    {
        System.Net.WebRequest myRequest;

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void Start_Clicked(object sender, EventArgs e)
        {
            ProgressLabel.Text = "Page_Load: thread #" + System.Threading.Thread.CurrentThread.GetHashCode();

            BeginEventHandler bh = new BeginEventHandler(this.BeginGetAsyncData);
            EndEventHandler eh = new EndEventHandler(this.EndGetAsyncData);

            AddOnPreRenderCompleteAsync(bh, eh);

            // Initialize the WebRequest.
            string address = "http://localhost/";

            myRequest = System.Net.WebRequest.Create(address);
        }

        IAsyncResult BeginGetAsyncData(Object src, EventArgs args, AsyncCallback cb, Object state)
        {
            Label2.Text = "BeginGetAsyncData: thread #" + System.Threading.Thread.CurrentThread.GetHashCode();
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);
            }
            return myRequest.BeginGetResponse(cb, state);
        }

        void EndGetAsyncData(IAsyncResult ar)
        {
            Label3.Text = "EndGetAsyncData: thread #" + System.Threading.Thread.CurrentThread.GetHashCode();

            System.Net.WebResponse myResponse = myRequest.EndGetResponse(ar);

          //  result.Text = new System.IO.StreamReader(myResponse.GetResponseStream()).ReadToEnd();
            myResponse.Close();
        }

    }
}