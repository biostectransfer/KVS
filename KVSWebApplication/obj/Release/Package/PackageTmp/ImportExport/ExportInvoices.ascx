<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExportInvoices.ascx.cs" Inherits="KVSWebApplication.ImportExport.ExportInvoices" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns='http://www.w3.org/1999/xhtml'>
<link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />
<link href="../Scripts/scripts.js" type="text/javascript" />
 <meta http-equiv="X-UA-Compatible" content="IE=9" />
<div runat="server" class="uebersichtDiv2" id="myUebersicht">
<style>
.halfWidth
{
    width: 50%;
}
</style>
  <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
  <script type="text/javascript">
    function RequestStart(sender, eventArgs) {
        var eventTarget = eventArgs.get_eventTarget();
        if (eventTarget.indexOf("ExportSelected") != -1) {
            eventArgs.set_enableAjax(false);
        }
    }
    function GoToToday1() {
        var datepicker = $find("<%=FromDateSearchBox.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }
    function GoToToday2() {
        var datepicker = $find("<%=ToDateSearchBox.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }
 </script>
</telerik:RadCodeBlock>       
          <telerik:RadWindowManager ID="RadWindowManagerCostCenter" runat="server" EnableShadow="true">
          </telerik:RadWindowManager>
              <telerik:RadAjaxPanel ID="RadAjaxPanelCostCenter" runat="server" Width="1600px" LoadingPanelID="RadAjaxLoadingPanelInvoices">
             <table>
             <tr>
             <td>        
                 <telerik:RadComboBox  Height="300px" Width="250px"  Enabled = "true"  
                Filter="Contains" runat = "server" HighlightTemplatedItems="true" AutoPostBack = "false" 
                 DropDownWidth="515px"    EmptyMessage = "Bitte wählen Sie einen Kunden aus: "
                DataTextField = "Name" DataValueField = "Value" ID = "CustomerNameBox"
              DataSourceID="LinqDataSourceCustomerName">
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
                 </telerik:RadComboBox >
            <td>
             <telerik:RadButton runat = "server" Text = "X" ID = "clearButton" OnClick="clearButton_Click" ToolTip="Auswahl löschen" > </telerik:RadButton>
             </td>
            </td>
            <td>
            <telerik:RadTextBox ID="RechnungsnummerSearchBox" runat="server" EmptyMessage="Rechnungsnummer" ></telerik:RadTextBox>
            </td>
            <td>
            <telerik:RadTextBox ID="DebitorennummerSearchBox" runat="server" EmptyMessage="Debitorennummer" ></telerik:RadTextBox>
             </td>
                <td>
            <telerik:RadTextBox ID="RechnungsempfaengerSearchBox" runat="server" EmptyMessage="Rechnungsempfänger" ></telerik:RadTextBox>
            </td>
            <td>
               <telerik:RadFormDecorator ID="FormDecorator1" runat="server" DecoratedControls="all" />
            <telerik:RadTextBox ID="KontonummerSearchBox" runat="server" EmptyMessage="Kontonummer" ></telerik:RadTextBox></td>
           <td>
            <label class="formRow">
                Von:
            </label>
      </td>
    <td>
       <telerik:RadDatePicker ID="FromDateSearchBox" runat="server" >
         <Calendar ID="Calendar2" runat = "server">
            <FooterTemplate> 
                        <div style="width: 100%; text-align: center; background-color: Gray;"> 
                            <input id="Button1" type="button" value="Heute" onclick="GoToToday1()" /> 
                        </div> 
            </FooterTemplate> 
        </Calendar>
       </telerik:RadDatePicker>
    </td>
      <td>
        <label class="formRow">
                Bis:
        </label>
      </td>
    <td>
       <telerik:RadDatePicker ID="ToDateSearchBox" runat="server">
         <Calendar ID="Calendar1" runat = "server">
            <FooterTemplate> 
                        <div style="width: 100%; text-align: center; background-color: Gray;"> 
                            <input id="Button1" type="button" value="Heute" onclick="GoToToday2()" /> 
                        </div> 
            </FooterTemplate> 
        </Calendar>
       </telerik:RadDatePicker>
    </td>
    <td>
        <telerik:RadButton runat = "server" Text = "Suchen" ID = "searchButton" OnClick="searchButton_Click" > </telerik:RadButton>
    </td>
       <td>
       <telerik:RadButton ID="ExportSelected" runat="server" Text="Markierte Exportieren" OnClick="PrintSelectedInvoices_Click"  />
       </td>
    </tr>
 </table>
        <telerik:RadGrid id="getAllInvoices" runat="server" Culture="de-De"  AllowAutomaticUpdates="true"  ShowGroupPanel="true"
            AllowAutomaticInserts="true" AllowAutomaticDeletes="false" AutoGenerateColumns="false"  ItemStyle-HorizontalAlign="Center"
            AllowPaging="true" DataSourceID="getAllCostCenterDataSource" Width="1640px" OnItemDataBound ="getAllInvoice_RowDataBound">
            <ClientSettings AllowDragToGroup="true" />
        <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"></PagerStyle> 
       <MasterTableView DataKeyNames="CustomerId"  CommandItemDisplay="Top" EditMode="PopUp" ShowFooter="true" AutoGenerateColumns="false" TableLayout="Auto"         
         AllowFilteringByColumn="false" >
           <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" />
            <Columns>
         <telerik:GridTemplateColumn UniqueName="Exporting" AllowFiltering="false" Display="true" >
        <ItemTemplate>
         <asp:CheckBox ID="ChbExporting" runat="server" OnCheckedChanged="CheckMyInvoice" AutoPostBack="true"/>
         <asp:Label id="myInvoiceNumber" Visible="false" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceNumber_").ToString() %>'></asp:Label>
        </ItemTemplate>
      </telerik:GridTemplateColumn>     
                    <telerik:GridBoundColumn DataField="TableId" Visible="false" ReadOnly="true" ForceExtractValue="Always"   />
                    <telerik:GridBoundColumn DataField="CustomerId" ReadOnly="true"  Visible="false" ForceExtractValue="Always"  />
                    <telerik:GridBoundColumn DataField="InvoiceNumber_" HeaderText="Rechnungsnummer"  UniqueName="InvoiceNumber_"
                    AutoPostBackOnFilter="false" CurrentFilterFunction="EqualTo" FilterControlWidth="90px" AllowSorting="true" >                             
                    </telerik:GridBoundColumn>
                     <telerik:GridDateTimeColumn  DataField="CreateDate" HeaderText="Erstellungsdatum"
                       EnableRangeFiltering="true"  PickerType="DatePicker"  ShowFilterIcon="true">
                        <HeaderStyle Width="260px" Height="20px" ></HeaderStyle>
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn DataField="Printed" HeaderText="Gedruckt" AutoPostBackOnFilter="false" CurrentFilterFunction="EqualTo" FilterControlWidth="60px" AllowSorting="true"  >
                    </telerik:GridBoundColumn>
                     <telerik:GridDateTimeColumn FilterControlWidth="95px" DataField="PrintDate" HeaderText="Druckdatum"
                        PickerType="DatePicker" EnableRangeFiltering="true">
                        <HeaderStyle Width="160px"></HeaderStyle>
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn DataField="InvoiceRecipient" HeaderText="Rechnungsempfänger"  AutoPostBackOnFilter="false" CurrentFilterFunction="EqualTo" FilterControlWidth="100px" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="CustomerName" HeaderText="Kundenname"  AutoPostBackOnFilter="false" CurrentFilterFunction="EqualTo" FilterControlWidth="150px" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="CustomerNumber" HeaderText="Kundennummer"  AutoPostBackOnFilter="false" CurrentFilterFunction="EqualTo" FilterControlWidth="100px" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="DebitorNumber" HeaderText="Debitorennummer"  AutoPostBackOnFilter="false" CurrentFilterFunction="EqualTo" FilterControlWidth="100px" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="InternalAccountNumber" HeaderText="MatchCode"  AutoPostBackOnFilter="false" CurrentFilterFunction="EqualTo" FilterControlWidth="100px" AllowSorting="true" />
                        <telerik:GridBoundColumn DataField="Discount" HeaderText="Rabattierung"  AutoPostBackOnFilter="false" CurrentFilterFunction="EqualTo" FilterControlWidth="100px" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="NettoSum" HeaderText="Nettobetrag" UniqueName="NettoSum"
                     DataType="System.Decimal" dataFormatString="{0:C}" 
                    AutoPostBackOnFilter="false"  FilterControlWidth="100px" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="BruttoBetrag" HeaderText="Bruttobetrag" UniqueName="BruttoBetrag"                   
                    AutoPostBackOnFilter="false" FilterControlWidth="100px" AllowSorting="true" />           
             </Columns>                
                     <EditFormSettings EditFormType="AutoGenerated" InsertCaption="Kostenstelle bearbeiten"  >
                    <EditColumn ButtonType="ImageButton" />
                    <PopUpSettings Modal="true"    />               
                </EditFormSettings>
                       <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
            <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" HorizontalAlign="Center"/>
            </MasterTableView>            
            <PagerStyle AlwaysVisible="true" />
        </telerik:RadGrid>
        </telerik:RadAjaxPanel>
              <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelInvoices" BackgroundTransparency="100" runat="server">
        </telerik:RadAjaxLoadingPanel>
      <asp:LinqDataSource runat="server" ID="getAllCostCenterDataSource" OnSelecting="getAllInvoiceDataSource_Selecting" ></asp:LinqDataSource>
        <asp:LinqDataSource runat="server" ID="LinqDataSourceCustomerName" OnSelecting="CustomerName_Selected" ></asp:LinqDataSource>
        </div>