<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="AbmeldungGrosskunde.aspx.cs" Inherits="KVSWebApplication.Auftragseingang.AbmeldungGrosskunde" %>
<%@ Register TagPrefix="smc" TagName="LargeCustomer" Src="../Auftragseingang/AbmeldungGrosskunde.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
 <asp:UpdatePanel ID="UpdatePanelAuftragseingang" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <body>
             <form id="form1">
           <asp:Label runat = "server" ID = "Label1" Text = "Sie haben keine Berechtigung für Außerbetriebsetzung" Visible = "false" ForeColor = "Red"> </asp:Label>
             <asp:Label runat = "server" ID = "Label2" Text = "Sie haben keine Berechtigung für Zulassung" Visible = "false" ForeColor = "Red"> </asp:Label>        
                <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
		        <Scripts>
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
		        </Scripts>
	            </telerik:RadScriptManager>
                    <div class="exampleWrapper">
                   <telerik:RadAjaxLoadingPanel runat="server" ID="LoadingPanelAuftragseingang" BackgroundTransparency = "100">
                   </telerik:RadAjaxLoadingPanel>
                   <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1" LoadingPanelID="LoadingPanelAuftragseingang" Height="100%" Width = "1400px" >
                    <div style="float: left; ">
                          <smc:LargeCustomer  ID="LargeCustomer" runat="server" />
                    </div>
                </telerik:RadAjaxPanel>
            </div>
	        </form>
            </body>              
  </ContentTemplate>  
</asp:UpdatePanel>
</asp:Content>
