using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace KVSWebApplication.Permission
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void ShowUserTab(object sender, EventArgs e)
        {

            RadButton clickedButton = (RadButton)sender;

            string selValue = clickedButton.ToggleStates[clickedButton.SelectedToggleStateIndex].Value;
            if (string.Compare(selValue, "0") == 0)
            {
                PermissionsPanel.Visible = true;
                PermissionProfilePanel.Visible = false;

            }
            else
            {

                PermissionsPanel.Visible = false;
                PermissionProfilePanel.Visible = true;

            }
          

        }
        protected void CreateUserTab(object sender, EventArgs e)
        {

            //Permissions.Visible = false;
    



        }
           protected void RadTabStrip1_TabClick(object sender,RadTabStripEventArgs e)
           {
                switch (e.Tab.Value)
                    {
                    case "0": 
                        PermissionsPanel.Visible = true;
                        PermissionProfilePanel.Visible = false;
                        break;
                     case "1": 
                      PermissionsPanel.Visible = false;
                      PermissionProfilePanel.Visible = true;
                        break;
                    }
            }
    }
}