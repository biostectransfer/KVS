<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Auftragseingang.aspx.cs" Inherits="KVSWebApplication.Auftragseingang.Auftragseingang" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
 <asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanelAuftragseingang" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <body>
             <form id="form1">
             <asp:Label runat = "server" ID = "AbmeldungRechtLabel" Text = "Sie haben keine Berechtigung für Außerbetriebsetzung" Visible = "false" ForeColor = "Red"> </asp:Label>
             <asp:Label runat = "server" ID = "ZulassungRechtLabel" Text = "Sie haben keine Berechtigung für Zulassung" Visible = "false" ForeColor = "Red"> </asp:Label>
                <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
		        <Scripts>
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
		        </Scripts>
	            </telerik:RadScriptManager>
	            <script type="text/javascript">
                </script>
            <div class="exampleWrapper">
                   <telerik:RadAjaxLoadingPanel runat="server" ID="LoadingPanelAuftragseingang" BackgroundTransparency = "100">
                   </telerik:RadAjaxLoadingPanel>
                   <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1" LoadingPanelID="LoadingPanelAuftragseingang" Height="100%" Width = "1400px" >
                    <div style="float: left; ">
                        <telerik:RadTabStrip  ID="RadTabStripAuftragseingang"   SelectedIndex="0" runat="server" MultiPageID="RadMultiPageAuftragseingang"  CssClass="tabStrip">
                        </telerik:RadTabStrip>
                        <telerik:RadMultiPage ID="RadMultiPageAuftragseingang"  runat="server" SelectedIndex="0" OnPageViewCreated="RadMultiPageAuftragseingang_PageViewCreated" CssClass="multiPage">
                        </telerik:RadMultiPage>
                    </div>
                </telerik:RadAjaxPanel>
            </div>
	        </form>
            </body>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>