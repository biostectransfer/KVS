<%@ Page Title="" EnableViewState = "true" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="AuftragsbearbeitungNeuzulassung.aspx.cs" Inherits="KVSWebApplication.Auftragsbearbeitung_Neuzulassung.AuftragsbearbeitungNeuzulassung" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

 <asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    
             <asp:HiddenField  ID = "CustomerValue" runat = "server" />
  
                <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
		        <Scripts>

                    <telerik:RadScriptReference  Path = "~/Scripts/lieferscheine_zul.js" />
                    <telerik:RadScriptReference  Path = "~/Scripts/scripts.js" />
                      <telerik:RadScriptReference  Path = "../Scripts/jquery-ui.js"/>
                    <telerik:RadScriptReference  Path = "../Scripts/jQUery.js"/>



		        </Scripts>
	            </telerik:RadScriptManager>

	            <script type="text/javascript">
	                function onTabSelecting(sender, args) {
	                    if (args.get_tab().get_pageViewID()) {
	                        args.get_tab().set_postBack(false);
	                    }
	                }

	          
	        
                </script>
                  <link href="/Styles/nachbearbeitungInfo.css" rel="stylesheet" >
             
                <asp:HiddenField runat = "server" ID = "SelectedCustomerId"/>

             <telerik:RadAjaxLoadingPanel runat="server" ID="LoadingPanel1" BackgroundTransparency = "100" >
                   </telerik:RadAjaxLoadingPanel>

                    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
                     <AjaxSettings>
                     <telerik:AjaxSetting AjaxControlID = "LieferungButton"></telerik:AjaxSetting>
                        <telerik:AjaxSetting AjaxControlID = "RadCodeBlock134"></telerik:AjaxSetting>
                           <telerik:AjaxSetting AjaxControlID = "UserValueConfirm"></telerik:AjaxSetting>
                           <telerik:AjaxSetting AjaxControlID = "RadCodeBlock441"></telerik:AjaxSetting>
            
                        

                         <telerik:AjaxSetting AjaxControlID="RadTabStripNeuzulassung">
                             <UpdatedControls>
                             <telerik:AjaxUpdatedControl ControlID="RadTabStripNeuzulassung"></telerik:AjaxUpdatedControl>
                             <telerik:AjaxUpdatedControl ControlID="RadMultiPageNeuzulassung" LoadingPanelID="LoadingPanel1"></telerik:AjaxUpdatedControl>
                             <telerik:AjaxUpdatedControl ControlID="LieferungButton"></telerik:AjaxUpdatedControl>
                             <telerik:AjaxUpdatedControl ControlID="RadCodeBlock134"></telerik:AjaxUpdatedControl>
                             <telerik:AjaxUpdatedControl ControlID="UserValueConfirm"></telerik:AjaxUpdatedControl>
                              <telerik:AjaxUpdatedControl ControlID="RadCodeBlock441"></telerik:AjaxUpdatedControl>

                             </UpdatedControls>
                      </telerik:AjaxSetting>
                    </AjaxSettings>
                     <ClientEvents OnRequestStart="RequestStart" />
                   

                    </telerik:RadAjaxManager>

                  <%-- <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1" LoadingPanelID="LoadingPanel1" Height="100%">--%>
                    <div style="float: left; width: auto">
                        <telerik:RadTabStrip  OnClientTabSelecting="onTabSelecting" ID="RadTabStripNeuzulassung" OnTabClick="RadTabStrip1_TabClick" SelectedIndex="0" runat="server" MultiPageID="RadMultiPageNeuzulassung"  CssClass="tabStrip">
                        </telerik:RadTabStrip>

                        <telerik:RadMultiPage ID="RadMultiPageNeuzulassung" runat="server" SelectedIndex="0" OnPageViewCreated="RadMultiPageNeuzulassung_PageViewCreated"  CssClass="multiPage">
                        </telerik:RadMultiPage>
                    </div>
               <%-- </telerik:RadAjaxPanel>--%>
                                

</asp:Content>