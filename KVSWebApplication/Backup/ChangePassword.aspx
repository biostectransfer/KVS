<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="KVSWebApplication.ChangePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<link href="Styles/styles.css" type="text/css"  rel="Stylesheet"/>
<telerik:RadFormDecorator runat="server" id="changePasswordDecorator"  DecorationZoneID="login-zone" DecoratedControls="All" />
  <telerik:RadWindowManager ID="RadWindowManagerChangePassword" runat="server" EnableShadow="true">
          </telerik:RadWindowManager>
 <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" 
                   LoadingPanelID="RadAjaxLoadingPanel1" Width="100%" Height="100%">
                   </telerik:RadAjaxPanel>   
                    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
                    </telerik:RadAjaxLoadingPanel>
     <div id="login-zone">    
          <div class="vlaidationSummary">
                    <div class="groupWrapper" style="margin-top:10%; margin-left:30%;" >
   <table cellpadding="1" border="1"   >
        <tr>
            <td>
                <table cellpadding="0">
                    <tr>
                        <td >
                            <asp:Label ID="lblOldPWD" runat="server" AssociatedControlID="txbOldPWD">Altes Passwort:</asp:Label>
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txbOldPWD" runat="server" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator CssClass="validatorStyle" ID="RequiredFieldValidator2" runat="server" ControlToValidate="txbOldPWD"
                                ErrorMessage="Das Passwort darf nicht leer sein!" ToolTip="Altes Passwort" ValidationGroup="Login1"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td >
                            <asp:Label ID="lblNewPWD" runat="server" AssociatedControlID="txbOldPWD">Neues Passwort:</asp:Label>
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txbNewPassword" runat="server" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator CssClass="validatorStyle" ID="PasswordRequired" runat="server" ControlToValidate="txbOldPWD"
                                ErrorMessage="Das Passwort darf nicht leer sein!" ToolTip="Neues Passwort eingeben" ValidationGroup="Login1"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>                                                       
                        <td >
                            <asp:Label ID="lblRepeatPWD" runat="server" AssociatedControlID="txbOldPWD">Passwort wiederholen:</asp:Label>
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txbRepeatPWD" runat="server" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator CssClass="validatorStyle" ID="RequiredFieldValidator1" runat="server" ControlToValidate="txbOldPWD"
                                ErrorMessage="Das Passwort darf nicht leer sein!" ToolTip="Passwort wiederholen" ValidationGroup="Login1"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="3" style="color: Red;">
                            <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td >
                            </td>
                        <td style="text-align: right; width:40x;" >
                            &nbsp;</td>
                        <td style="text-align: right;" >
                            <asp:Button ID="ChangeSaveBtn" runat="server" Text="Speichern" 
                                ValidationGroup="Login1" onclick="ChangeSaveBtn_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>
</div>
</div>
</asp:Content>