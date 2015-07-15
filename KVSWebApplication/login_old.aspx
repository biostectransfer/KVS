<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="KVSWebApplication.login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="Styles/styles.css" type="text/css"  rel="Stylesheet"/>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1" Height="100%"  />
       
                 <!--[if IE]>
       <style>
        #tempGroup
        {
            margin-top:10%; width:1250px; text-align:center;  margin-left:600px; margin-right:auto;
        }
        </style>
         
       <![endif]-->
       <!--[if !IE]>
       <style>
        #tempGroup
        {
             margin-top:10%; width:1250px; text-align:center;  margin-left:670px; margin-right:auto;
        }
        </style>
        <![endif]-->
      
     <div id="login-zone">
          <div class="vlaidationSummary">
           <telerik:RadFormDecorator ID="RadFormDecorator1" runat="server" DecoratedControls="All"
                 DecorationZoneID="login-zone"></telerik:RadFormDecorator>
             <%--    <h2 id="flyin" style="position:relative;left:-800px;font-style:italic" style=&{ns4def};>--%>
                    <div class="groupWrapper" 
                    style="<!--[if lte IE 8 ]>
                       width:1250px; margin-left:400px; margin-right:auto;
                   <![endif]-->
                    <!--[if !IE]>
                       width:1250px; margin-left:670px; margin-right:auto;
                  <!-- <![endif]-->
                    >
                         <asp:Login ID="Login2" runat="server"   EnableViewState="false" 
                         OnAuthenticate="OnAuthenticate" OnLoggedIn="OnLoggedIn" >                        
                              <LayoutTemplate>
                                   <table cellpadding="1" border="1" >
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
</asp:Content>