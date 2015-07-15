<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="KVSWebApplication.User.CreateUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
   <link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />
     <link href="../Scripts/scripts.js" type="text/javascript" />
     <style>
      .rgEditForm {
    height: 280px !important;
}  
  </style>
   <script type="text/javascript">
       function onPopUpShowing(sender, args) {
           args.get_popUp().className += " popUpEditForm";
       }
        </script>
        <telerik:RadAjaxManager runat="server" ID="RadAjaxManagerCreateUser">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="getAllUser">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="getAllUser" />
                        <telerik:AjaxUpdatedControl ControlID="getAllUser" LoadingPanelID="RadAjaxLoadingPanelCreateUser" />
                    </UpdatedControls>
                </telerik:AjaxSetting>              
            </AjaxSettings>
        </telerik:RadAjaxManager>
             <asp:ScriptManager ID="ScriptManagerCreateUser" runat="server">
    </asp:ScriptManager>
        <telerik:RadAjaxPanel ID="RadAjaxPanelCreateUser" runat="server" Width="1050px" LoadingPanelID="RadAjaxLoadingPanelCreateUser">
     <telerik:RadFormDecorator ID="QsfFromDecoratorCreateUser" runat="server" DecoratedControls="All" EnableRoundedCorners="false" />        
        <asp:Panel ID="UserPanel" runat="server" >
                            <asp:RadioButton ID="rbtShowUser" runat="server" Text="Benutzerübersicht" Checked="true" GroupName="CustomerType" AutoPostBack="true"  OnCheckedChanged="ShowUserTab" />
                            <asp:RadioButton ID="rbtcreateUser" runat="server" Text="Benutzer anlegen" GroupName="CustomerType" AutoPostBack="true"  OnCheckedChanged="CreateUserTab"/>
                            </asp:Panel>
           <telerik:RadWindowManager ID="RadWindowManagerCreateUser" 
          Behaviors="Default" 
           runat="server"  Style="z-index: 31000" >
          </telerik:RadWindowManager>
       <telerik:RadWindow Title = "Passwort zurücksetzen" runat="server" ID="RadWindow_ChangePWD" VisibleOnPageLoad="false" Modal="true">
       <ContentTemplate>       
   <table cellpadding="1"   >
        <tr>
            <td >
                    <asp:Label ID="lblNewPWD" runat="server" AssociatedControlID="txbNewPassword">Neues Passwort:</asp:Label>
            </td>
            <td colspan="2">
                    <asp:TextBox ID="txbNewPassword" runat="server" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator CssClass="validatorStyle" ID="PasswordRequired" runat="server" ControlToValidate="txbNewPassword"
                        ErrorMessage="Das Passwort darf nicht leer sein!" ToolTip="Neues Passwort eingeben" ValidationGroup="Login1"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>                                                       
            <td >
                    <asp:Label ID="lblRepeatPWD" runat="server" AssociatedControlID="txbRepeatPWD">Passwort wiederholen:</asp:Label>
            </td>
            <td colspan="2">
                    <asp:TextBox ID="txbRepeatPWD" runat="server" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator CssClass="validatorStyle" ID="RequiredFieldValidator1" runat="server" ControlToValidate="txbRepeatPWD"
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
                    ValidationGroup="Login1"  OnClick="ChangeSaveBtn_Click" />
                </td>
        </tr>
    </table>
</td>
</tr>
</table>               
</ContentTemplate>
</telerik:RadWindow>      
    <asp:Panel ID="ShowUserPanel" runat="server">
        <telerik:RadGrid id="getAllUser" DataSourceID="getAllUserDataSource"  OnInit="getAllUser_Init" 
            OnUpdateCommand="getAllUser_EditCommand" OnItemCommand="getAllUser_ItemCommand"
            runat="server"  Culture="de-De"  AllowAutomaticUpdates="false"   EnableViewState ="false"
            AllowAutomaticInserts="true" AllowAutomaticDeletes="false" AutoGenerateColumns="false"  
            AllowPaging="true"   >  
 <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"></PagerStyle>
       <MasterTableView DataKeyNames="Id" CommandItemDisplay="Top" AllowSorting="true" EditMode="PopUp" AllowFilteringByColumn="true"  >
           <CommandItemSettings ShowAddNewRecordButton="false"  />
            <Columns>                       
                     <telerik:GridEditCommandColumn ButtonType="ImageButton" />
                    <telerik:GridButtonColumn CommandName="reversePWD" ButtonType="ImageButton" ImageUrl="../Pictures/back.gif" Text="Passwort zurücksetzen"  UniqueName="DeleteColumn"></telerik:GridButtonColumn>
                    <telerik:GridBoundColumn DataField="Id" HeaderText="Id" ReadOnly="true"  Display="false" ForceExtractValue="Always"  />
                    <telerik:GridBoundColumn DataField="PersonId"  ReadOnly="true"  Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="AdressId"  ReadOnly="true"  Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Nachname" HeaderButtonType="TextButton" 
                     CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true"  />
                    <telerik:GridBoundColumn DataField="FirstName" HeaderText="Vorname"   HeaderButtonType="TextButton" 
                     CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" />        
                    <telerik:GridBoundColumn DataField="Title" HeaderText="Titel"  HeaderButtonType="TextButton"
                      CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="Phone" HeaderText="Telefonnummer" HeaderButtonType="TextButton"
                       CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%"   AllowSorting="true"  />
                    <telerik:GridBoundColumn DataField="Fax" HeaderText="Fax" HeaderButtonType="TextButton"
                      CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="MobilePhone" HeaderText="Mobil"  HeaderButtonType="TextButton" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="Email" HeaderText="Email"  HeaderButtonType="TextButton"
                      CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="Login" HeaderText="Benutzername"  HeaderButtonType="TextButton"
                    CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" />
                    <telerik:GridCheckBoxColumn DataField="Locked" HeaderText="Gesperrt" HeaderStyle-HorizontalAlign="Center" HeaderButtonType="TextButton"
                    ItemStyle-HorizontalAlign="Center" ShowFilterIcon="false" ></telerik:GridCheckBoxColumn>             
             </Columns>
            <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
            <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />             
                <EditFormSettings EditFormType="AutoGenerated"   >
                    <EditColumn ButtonType="ImageButton"   />
                    <PopUpSettings Modal="true"    />               
                </EditFormSettings>               
            </MasterTableView>
               <ClientSettings>
            <ClientEvents  OnPopUpShowing="onPopUpShowing"/>             
        </ClientSettings>
            <PagerStyle AlwaysVisible="true" />
        </telerik:RadGrid>     
        <asp:LinqDataSource runat=server ID="getAllUserDataSource" OnSelecting="getAllUserDataSource_Selecting" ></asp:LinqDataSource>
        </asp:Panel>
        <asp:Panel id="CreateUserPanel" Visible="false" runat="server" CssClass="CreateNewUser" Width="900" >
            <table  border="0" cellpadding="5">
                <colgroup>
                    <col width="100"/>
                    <col width="200"/>
                    <col width="100"/>
                    <col width="200"/>
                </colgroup>
                <thead>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="Label1" runat="server"  AssociatedControlID="txbUserVorname" 
                                Text="Stammdaten"></asp:Label>
                        </td>                    
                    </tr>
                </thead>
                <tbody>
                    <tr class="FormContainer">
                        <td >
                            <asp:Label ID="lblUserVorname" runat="server" Text="Vorname:" AssociatedControlID="txbUserVorname"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbUserVorname" runat="server" 
                                ValidationGroup="Group1">
                            </telerik:RadTextBox>
                             <br />
                            <asp:Label ID="lblUserVornameError" runat="server" CssClass="Validator" ></asp:Label> 
                             </td>
                    </tr>
                    <tr class="FormContainer">
                        <td  >
                            <asp:Label ID="lblUserNachname" runat="server" Text="Nachname:" AssociatedControlID="txbUserNachname"></asp:Label>
                        </td>
                        <td>
                          <telerik:RadTextBox ID="txbUserNachname" runat="server" ValidationGroup="Group1" 
                             >
                            </telerik:RadTextBox>     
                         <br />
                       <asp:Label ID="lblUserNachnameError" runat="server" CssClass="Validator" ></asp:Label>                       
                        </td>                       
                    </tr>
                        <tr class="FormContainer">
                        <td valign="top" >
                            <asp:Label ID="lbUserTitle" runat="server" AssociatedControlID="txbUserTitle"
                                Text="Titel:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbUserTitle" runat="server" 
                                ValidationGroup="Group1">
                            </telerik:RadTextBox>                  
                        <br />
                        </td>                     
                    </tr>
                       <tr class="FormContainer">
                        <td valign="top" >
                            <asp:Label ID="lblUserLogin" runat="server" Text="Benutzername"  AssociatedControlID="txbUserLogin"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbUserLogin" runat="server" 
                                ValidationGroup="Group1">
                            </telerik:RadTextBox>
                        <br />
                           <asp:Label ID="lblUserLoginError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>                         
                    </tr>
                       <tr class="FormContainer">
                        <td valign="top" >
                            <asp:Label ID="lblUserPassword1" runat="server" Text="Passwort:" AssociatedControlID="txbUserPassword1"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbUserPassword1" runat="server" TextMode="Password" >
                            </telerik:RadTextBox>     
                       <br />
                          <asp:Label ID="lblUserPassword1Error" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                    </tr>
                       <tr class="FormContainer">
                        <td valign="top" style="width:152px" >
                               <asp:Label ID="UserNameLabel" Width="152" runat="server" AssociatedControlID="txbUserPassword2">Passwort wiederholen:</asp:Label>
                        </td>
                        <td >
                            <telerik:RadTextBox ID="txbUserPassword2" runat="server" TextMode="Password" >
                            </telerik:RadTextBox>     
                       <br />
                          <asp:Label ID="lblUserPassword2Error" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>     
                    </tr>
                     <tr class="FormContainer">
                        <td valign="top" >
                            <asp:Label ID="lblUserPhone" runat="server" Text="Festnetz:" AssociatedControlID="txbUserPhone"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbUserPhone" runat="server"  >
                            </telerik:RadTextBox>     
                       <br />
                          <asp:Label ID="lblUserPhoneError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td> 
                    </tr>
                     <tr class="FormContainer">
                        <td valign="top" >
                            <asp:Label ID="lblUserFax" runat="server" Text="Fax:" AssociatedControlID="txbUserFax"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbUserFax" runat="server"  >
                            </telerik:RadTextBox>     
                       <br />
                          <asp:Label ID="lblUserFaxError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td> 
                    </tr>    
                     <tr class="FormContainer">
                        <td valign="top" >
                            <asp:Label ID="lblUserMobile" runat="server" Text="Mobil:" AssociatedControlID="txbUserMobile"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbUserMobile" runat="server"  >
                            </telerik:RadTextBox>     
                       <br />
                          <asp:Label ID="lblUserMobileError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td> 
                    </tr>                    
                     <tr class="FormContainer">
                        <td valign="top" >
                            <asp:Label ID="lblUserEmail" runat="server" Text="E-Mail:" AssociatedControlID="txbUserEmail"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbUserEmail" runat="server"  >
                            </telerik:RadTextBox>     
                       <br />
                          <asp:Label ID="lblUserEmailError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td> 
                    </tr>
                      <tr class="FormContainer">
                        <td valign="top" >              
                        </td>
                        <td>
                             <telerik:RadButton id="rbtSaveUser" runat="server" Text="Speichern" CssClass="bSaveButton" OnClick="bSaveClick" ></telerik:RadButton>
                       <br />                      
                        </td> 
                    </tr>     
                </tbody>
            </table>      
      </asp:Panel>
        </telerik:RadAjaxPanel>        
        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelCreateUser" BackgroundTransparency="100" runat="server">
        </telerik:RadAjaxLoadingPanel>
</asp:Content>