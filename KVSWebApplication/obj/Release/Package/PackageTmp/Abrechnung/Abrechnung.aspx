<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Abrechnung.aspx.cs" Inherits="KVSWebApplication.Abrechnung.Abrechnung" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
 <asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">           
        <ContentTemplate>
            <body>
             <form id="form1">
             <asp:HiddenField  ID = "CustomerValue" runat = "server" />       
                <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
		        <Scripts>
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />           
		        </Scripts>
	            </telerik:RadScriptManager>
            <div class="exampleWrapper">
                   <telerik:RadAjaxLoadingPanel runat="server" ID="LoadingPanel1" BackgroundTransparency = "100" >
                   </telerik:RadAjaxLoadingPanel>
                   <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1" LoadingPanelID = "LoadingPanel1" Height="100%">
                    <div style="float: left; width: 700px">
                        <telerik:RadTabStrip ID="RadTabStripAbrechnung" SelectedIndex="0" runat="server" MultiPageID="RadMultiPageAbrechnung"  CssClass="tabStrip">
                        </telerik:RadTabStrip>
                        <telerik:RadMultiPage ID="RadMultiPageAbrechnung" runat="server" SelectedIndex="0" OnPageViewCreated="RadMultiPageAbrechnung_PageViewCreated"  CssClass="multiPage">
                        </telerik:RadMultiPage>
                    </div>
                </telerik:RadAjaxPanel>
            </div>
	        </form>
            </body>
        </ContentTemplate>
</asp:Content>