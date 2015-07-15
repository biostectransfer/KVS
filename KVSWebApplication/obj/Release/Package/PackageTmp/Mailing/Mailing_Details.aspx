<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Mailing_Details.aspx.cs" Inherits="KVSWebApplication.Mailing.Mailing_Details" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />
     <link href="../Scripts/scripts.js" type="text/javascript" />
    <telerik:RadFormDecorator ID="QsfFromDecorator" runat="server" DecoratedControls="All" EnableRoundedCorners="false" />         
   <asp:ScriptManager ID="MailerManager" runat="server"></asp:ScriptManager>
        <telerik:RadAjaxPanel ID="RadAjaxPanelCustomer" runat="server" LoadingPanelID="RadAjaxLoadingPanelCustomer">   
     <telerik:RadWindowManager ID="RadWindowManagerEditMail" runat="server" EnableShadow="true">
          </telerik:RadWindowManager>
       <asp:Panel ID="MailingPanel" runat="server" >
        <div>                           
            <asp:RadioButton ID="rbtCustomerMail" runat="server" Text="Kunden Email Verteiler" Checked="true" GroupName="CustomerType" AutoPostBack="true"  OnCheckedChanged="rbtCustomerMail_Checked" />
            <asp:RadioButton ID="rbtLocationMail" runat="server" Text="Standort Email Verteiler" GroupName="CustomerType" AutoPostBack="true"  OnCheckedChanged="rbtLocationMail_Checked"/>                            
            <asp:Label ID="lblAllErrors" runat="server" CssClass="Validator" ></asp:Label> 
            <br />                      
        </div>
<div id="CustomerMailingHeader" class="CustomerPriceListHeader" runat="server" >
<table>
<tr>
<td>
<asp:Label ID="textCustomerCMB" runat="server" CssClass="CustomerText" Text="Kunde:" AssociatedControlID="AllCustomer"></asp:Label>
    <telerik:RadComboBox  Height="300px" Width="250px"  Enabled = "true"  
            Filter="Contains" runat = "server" HighlightTemplatedItems="true" AutoPostBack = "true" 
                DropDownWidth="515px"    EmptyMessage = "Bitte wählen Sie einen Kunden aus: "
            DataTextField = "Name" DataValueField = "Value" ID = "AllCustomer"
                OnSelectedIndexChanged="CustomerCombobox_SelectedIndexChanged"   OnInit="CustomerCombobox_Init">    
                    <HeaderTemplate>
                        <table style="width: 515px" cellspacing="0" cellpadding="0">
                            <tr align="center">
                                <td style="width: 90px;">
                                    Kundennummer
                                </td>
                                <td style="width: 175px;">
                                    Matchcode
                                </td>                                         
                                <td style="width: 250px">
                                    Kundenname
                                </td>
                            </tr>
                        </table>
                    </HeaderTemplate>
                        <ItemTemplate>
                        <table style="width: 515px;" cellspacing="0" cellpadding="0">
                            <tr>
                                <td style="width: 110px;">
                                    <%# DataBinder.Eval(Container, "DataItem.Kundennummer")%>
                                </td>
                                <td style="width: 175px;">
                                    <%# DataBinder.Eval(Container, "DataItem.Matchcode") %>                                     
                                </td>
                                <td style="width: 250px;">
                                    <%# DataBinder.Eval(Container, "DataItem.Name").ToString() %>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
</telerik:RadComboBox>
</td>
<td>
<asp:Label ID="lblTextLocation" runat="server" CssClass="LocationText" Text="Standort:"  AssociatedControlID="AllCustomer"></asp:Label>
<telerik:RadComboBox CssClass="LocationText" runat = "server" ID="cmbLocations" ></telerik:RadComboBox>
</td>
<td>
<telerik:RadButton ID="bSchow" runat ="server" Text="Anzeigen" onclick="bSchow_Click"
        CssClass="CustomerPriceShowButton"></telerik:RadButton>
</td>
</tr>
</table>
</div>
</asp:Panel>
<telerik:RadGrid id="RadGridMailEditor" runat="server" Culture="de-De"  AllowAutomaticUpdates="false" 
            AllowAutomaticInserts="true" AllowAutomaticDeletes="false" AutoGenerateColumns="false"   Visible="false"
            Enabled="false" DataSourceID="GetMailAdresses" Width="730px" OnItemCommand="RadGridMailEditor_ItemCommand"
            AllowPaging="true" OnInit="RadGridMailEditor_Init"   EnableViewState ="false" >           
 <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"></PagerStyle>
       <MasterTableView  CommandItemDisplay="Top" EditMode="PopUp" AllowFilteringByColumn="true" TableLayout="Auto" >
       <CommandItemTemplate>  
      <asp:ImageButton ID="add" runat="server" ToolTip="Neue Email Adresse hinzufügen" ImageUrl="~/Pictures/AddRecord.gif" Height="20px" Width="20px"  CommandName="InsertItem"></asp:ImageButton >
       <asp:Label ID="Label5" runat="server" Text="Neues Email Adresse hinzufügen"></asp:Label>
    </CommandItemTemplate>
           <CommandItemSettings ShowAddNewRecordButton="true"  ShowRefreshButton="true"   />
           <Columns>
                    <telerik:GridEditCommandColumn ButtonType="ImageButton"  />
                    <telerik:GridButtonColumn CommandName="Delete"  ButtonType="ImageButton" ImageUrl="~/Pictures/Delete.gif"  UniqueName="DeleteColumn"></telerik:GridButtonColumn>
                    <telerik:GridBoundColumn DataField="Id" HeaderText="Id" UniqueName="Id" Display="false" ForceExtractValue="Always"  />
                    <telerik:GridBoundColumn DataField="CustomerId" HeaderText="Id" ReadOnly="true"  Visible="false" ForceExtractValue="Always"  />
                    <telerik:GridBoundColumn DataField="LocationId" HeaderText="Id" ReadOnly="true"  Visible="false" ForceExtractValue="Always"  />
                    <telerik:GridBoundColumn DataField="EmailAdresse"  HeaderText="Email Adresse" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false" FilterControlWidth="95%"/>
                    <telerik:GridBoundColumn DataField="EmailType" HeaderText="Email Typ" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false" FilterControlWidth="95%"/>
           </Columns>
  <EditFormSettings  EditFormType="Template" CaptionFormatString="Emailadresse bearbeiten" PopUpSettings-Modal="true">    
  <FormTemplate > 
            <asp:Label ID="SchowErrorMessages" runat="server"  BackColor="Red"></asp:Label>             
        <table id="tableEditCustomerMail" cellspacing="1" cellpadding="1" width="450" border="0">
            <tr>
                <td>
                </td>
                <td>
                </td>
            </tr>
                <asp:TextBox ID="EditMailId" Visible = "false"  runat="server"  Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>'>
                    </asp:TextBox>
            <asp:TextBox ID="EditCustomerId" Visible = "false"  runat="server"  Text='<%#  DataBinder.Eval(Container, "DataItem.CustomerId") %>'>
                    </asp:TextBox>
            <asp:TextBox ID="EditLocationId" Visible = "false"  runat="server"  Text='<%#  DataBinder.Eval(Container, "DataItem.LocationId")%>'>
                    </asp:TextBox>                          
            <tr>
                <td>
                    E-Mail Adresse:
                </td>
                <td>
                    <asp:TextBox ID="EmailAdress"  runat="server"  Text='<%#  DataBinder.Eval(Container, "DataItem.EmailAdresse").ToString() %>'>
                    </asp:TextBox>                             
                </td>
            </tr>
            <tr>
                <td>
                    E-Mail Typ:
                </td>
                <td>
                    <telerik:RadComboBox CssClass="CustomerCombobox" runat = "server" ID="cmbMailTyp" enableloadondemand="true" Text='<%#  DataBinder.Eval(Container, "DataItem.EmailType").ToString() %>'
                        DataValueField="id" DataTextField="typeName"  ShowMoreResultsBox="True" Height="140px" 
                            OnItemsRequested="MailType_ItemsRequested" ></telerik:RadComboBox>
                </td>
            </tr>      
                <td align="center" colspan="2">
                    <br>
                </br>
                    <asp:Button ID="btnSaveCustomerProduct" Text="Ok" runat="server" OnClick="btnSaveMail_Click" >
                    </asp:Button>&nbsp;
                    <asp:Button ID="btnAbortCustomerProduct" Text="Abbrechen" runat="server" CausesValidation="False" OnClick="btnAbortMail_Click" >
                    </asp:Button>
                </td>
        </tr>
        </table>
    </FormTemplate>
</EditFormSettings>     
            <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
<HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
<AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />
</MasterTableView>            
<PagerStyle AlwaysVisible="true" />
</telerik:RadGrid>
</telerik:RadAjaxPanel>
   <asp:LinqDataSource runat=server ID="GetMailAdresses"  OnSelecting="GetMailAdresses_Selecting" >
   </asp:LinqDataSource>
</asp:Content>