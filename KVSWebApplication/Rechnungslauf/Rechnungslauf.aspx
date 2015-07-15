<%@ Page  Async = "true"  Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Rechnungslauf.aspx.cs" Inherits="KVSWebApplication.Rechnungslauf.Rechnungslauf" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>



<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <body>
             <form id="form1">

              <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
		        <Scripts>
		        </Scripts>
	            </telerik:RadScriptManager>

             <asp:Label runat= "server" ID = "StatusLabel"></asp:Label>

<asp:Label runat= "server" ID = "ProgressLabel"></asp:Label>
<asp:Label runat= "server" ID = "Label2"></asp:Label>
<asp:Label runat= "server" ID = "Label3"></asp:Label>


<asp:Button ID = "StartAsyncOperation" OnClick = "Start_Clicked" Text = "Run Thread"  runat = "server"> </asp:Button >
<asp:Button ID = "CancelAsyncOperation" Text = "Stop Thread" runat = "server"> </asp:Button >
	        </form>
            </body>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>