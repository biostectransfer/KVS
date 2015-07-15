<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" 
CodeBehind="login.aspx.cs" Inherits="KVSWebApplication.login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="Styles/styles.css" type="text/css"  rel="Stylesheet"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<table cellpadding="0" cellspacing="0" style="width: 100%;text-align: center; margin-left: -100px;">
    <tr>
        <td style="width: 50%;"></td>
        <td style="text-align: center; width: 300px;">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1" Height="100%" />
     <div id="login-zone">
          <div class="vlaidationSummary">
           <telerik:RadFormDecorator ID="RadFormDecorator1" runat="server" DecoratedControls="All"
                 DecorationZoneID="login-zone"></telerik:RadFormDecorator>
                 <h2 id="flyin" style="position:relative;left:-800px;font-style:italic" style=&{ns4def};>
                    <div class="groupWrapper" style="margin-top:10%; margin-left:0px;">
                         <asp:Login ID="Login2" runat="server" Width="70%" EnableViewState="false" 
                         OnAuthenticate="OnAuthenticate" OnLoggedIn="OnLoggedIn" >                        
                              <LayoutTemplate>
                                   <table cellpadding="1">
                                        <tr>
                                             <td>
                                                  <table cellpadding="0">
                                                       <tr>
                                                            <td>
                                                                 <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">Benutzername:</asp:Label>
                                                            </td>
                                                            <td colspan="2">
                                                                 <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                                                 <asp:RequiredFieldValidator CssClass="validatorStyle" ID="UserNameRequired" runat="server"  ControlToValidate="UserName"
                                                                      ErrorMessage="Der Benutzername darf nicht leer sein!" ToolTip="Benutzername eingeben" ValidationGroup="Login1"></asp:RequiredFieldValidator>
                                                            </td>
                                                       </tr>
                                                       <tr>
                                                            <td >
                                                                 <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Passwort:</asp:Label>
                                                            </td>
                                                            <td colspan="2">
                                                                 <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                                                                 <asp:RequiredFieldValidator CssClass="validatorStyle" ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                                                      ErrorMessage="Das Passwort darf nicht leer sein!" ToolTip="Passwort eingeben" ValidationGroup="Login1"></asp:RequiredFieldValidator>
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
                                                                <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Anmelden" 
                                                                    ValidationGroup="Login1" />
                                                             </td>
                                                       </tr>
                                                  </table>
                                             </td>
                                        </tr>
                                   </table>                           
                              </LayoutTemplate>
                         </asp:Login>
                         <asp:Label ID="LabelLogin" Text="" Font-Size="Large" ForeColor="Green" runat="server"></asp:Label>     
                    
                    </div>
                 </h2>         
            </telerik:RadAjaxPanel>
<script language="JavaScript1.2">
if (document.getElementById||document.all)
var crossheader=document.getElementById? document.getElementById("flyin").style : document.all.flyin.style
function animatein(){
if (parseInt(crossheader.left)<0)
crossheader.left=parseInt(crossheader.left)+20+"px"
else{
crossheader.left=0
crossheader.fontStyle="normal"
clearInterval(start)
}
}
if (document.getElementById||document.all)
start=setInterval("animatein()",25)
</script>
               <br />    
          </div>
     </div>  

        </td>
        <td style="width: 50%;"></td>
    </tr>
</table>
</asp:Content>