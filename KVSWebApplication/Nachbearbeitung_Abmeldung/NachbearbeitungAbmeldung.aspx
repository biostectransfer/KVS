<%@ Page Title="" Language="C#" EnableViewState = "true" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="NachbearbeitungAbmeldung.aspx.cs" Inherits="KVSWebApplication.Nachbearbeitung_Abmeldung.NachbearbeitungAbmeldung" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
 <asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
      <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
		<Scripts>			     
            <telerik:RadScriptReference  Path = "~/Scripts/scripts.js" />
            <telerik:RadScriptReference  Path = "../Scripts/lieferscheine_abm.js" />
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
            <telerik:RadAjaxLoadingPanel runat="server" ID="LoadingPanel1" BackgroundTransparency = "100" >
            </telerik:RadAjaxLoadingPanel>
            <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
                <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID = "LieferungButton"></telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID = "RadCodeBlock21"></telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID = "UserValueConfirm"></telerik:AjaxSetting>                     
                    <telerik:AjaxSetting AjaxControlID="RadTabStrip1"></telerik:AjaxSetting>    
                <telerik:AjaxSetting AjaxControlID="RadTabStrip1">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="RadTabStrip1"></telerik:AjaxUpdatedControl>
                        <telerik:AjaxUpdatedControl ControlID="RadMultiPage1" LoadingPanelID="LoadingPanel1"></telerik:AjaxUpdatedControl>
                    <telerik:AjaxUpdatedControl ControlID="LieferungButton"></telerik:AjaxUpdatedControl>                        
                        <telerik:AjaxUpdatedControl ControlID="UserValueConfirm"></telerik:AjaxUpdatedControl>
                        <telerik:AjaxUpdatedControl ControlID="RadCodeBlock21"></telerik:AjaxUpdatedControl>
                    </UpdatedControls>
             </telerik:AjaxSetting>
            </AjaxSettings>
                    <ClientEvents OnRequestStart="RequestStart" />
            </telerik:RadAjaxManager>
            <div style="float: left; width: auto">
                <telerik:RadTabStrip  ID="RadTabStrip1" OnClientTabSelecting="onTabSelecting" OnTabClick="RadTabStrip1_TabClick" SelectedIndex="0" runat="server" MultiPageID="RadMultiPage1"  CssClass="tabStrip">
                </telerik:RadTabStrip>
                <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" OnPageViewCreated="RadMultiPage1_PageViewCreated" CssClass="multiPage">
                </telerik:RadMultiPage>
            </div>
</asp:Content>