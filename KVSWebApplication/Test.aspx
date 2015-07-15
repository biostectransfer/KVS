<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Default.Master"  CodeBehind="Test.aspx.cs" Inherits="KVSWebApplication.Test" %>
<%@ MasterType VirtualPath="~/Default.Master" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <title>Test Seite</title>
	<telerik:RadStyleSheetManager id="RadStyleSheetManager1" runat="server"  />
</asp:Content>

 <asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <body>
             <form id="form1">
   
                <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
		        <Scripts>
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
		        </Scripts>
	            </telerik:RadScriptManager>

	            <script type="text/javascript">
	              //Put your JavaScript code here.
                </script>


                <%--Put your stuff here--%>


	        </form>
            </body>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
