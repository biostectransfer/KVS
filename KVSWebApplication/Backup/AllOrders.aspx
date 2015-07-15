<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="AllOrders.aspx.cs" Inherits="KVSWebApplication.AllOrders" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
        <asp:ScriptManager ID="SearchManager" runat="server"></asp:ScriptManager>
   <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <script type="text/javascript">
                function onRequestStart(sender, args) {
                    if (args.get_eventTarget().indexOf("ExportToExcelButton") >= 0 ||
                    args.get_eventTarget().indexOf("ExportToWordButton") >= 0 ||
                    args.get_eventTarget().indexOf("ExportToCsvButton") >= 0) {
                        args.set_enableAjax(false);
                    }
                }
    </script>
          <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Sofort- oder Großkunde: " ID = "RadTextBox2" Width = "240px" ></telerik:RadTextBox>
                <telerik:radcombobox id="RadComboBoxCustomerOffenNeuzulassung" runat="server" OnSelectedIndexChanged = "SmallLargeCustomerIndex_Changed"
                OnItemsRequested ="SmallLargeCustomerIndex_Changed"  AutoPostBack = "true" Width="250px"> 
                <Items>   
                    <telerik:RadComboBoxItem runat="server" Value = "2" Text="Großkunden" />  
                    <telerik:RadComboBoxItem runat="server" Value = "1" Text="Sofortkunden" />  
                </Items>
                 </telerik:radcombobox>
                 <br />                
                <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" 
                BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Bitte wählen Sie einen Kunden aus: " ID = "RadTextBox1" Width = "240px" ></telerik:RadTextBox>
                <telerik:RadComboBox  Height="300px" Width="250px"  Enabled = "true" 
                DataSourceID = "CustomerDataSource" Filter="Contains" runat = "server"  
                DropDownWidth="515px" EmptyMessage="Wählen Sie einen Kunden aus" AutoPostBack="true"
                DataTextField = "Name"  DataValueField = "Value" ID = "CustomerDropDownListOffenNeuzulassung" 
                 OnSelectedIndexChanged="CustomerIndex_Changed" >                 
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
                                                  <%# DataBinder.Eval(Container, "DataItem.Name")%>
                                             </td>
                                        </tr>
                                   </table>
                              </ItemTemplate>
                 </telerik:RadComboBox >
                <br />
                <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Zusätzliche Optionen: " ID = "RadTextBox3" Width = "240px" ></telerik:RadTextBox>
                <asp:Button runat = "server" ID = "ShowAllButton1" OnClick = "ShowAllButton1_Click" Text = "Alles anzeigen"></asp:Button>
                <br /><br />
               <telerik:RadFormDecorator runat = "server" ID = "Zulassungsdekorator"/>           
                <telerik:RadGrid  OnDetailTableDataBind="RadGridAllOrders_DetailTableDataBind"  AutoGenerateColumns="false" ID="RadGridAllOrders" 
                DataSourceId = "LinqDataSourceAllOrders"  Width="900px"
                AllowFilteringByColumn="True" AllowSorting="True" OnPreRender="RadGridAllOrders_PreRender"
                ShowFooter="True" AllowPaging="true" Enabled = "true" ShowHeader = "true" runat="server" GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "true"  >
                <pagerstyle mode="NextPrevAndNumeric" AlwaysVisible="true"></pagerstyle>
         <ExportSettings >
                <Excel Format="Biff" ></Excel>
            </ExportSettings>
                <groupingsettings casesensitive="false" ></groupingsettings>
             <mastertableview  CommandItemDisplay = "Top"  autogeneratecolumns="false"  Name="Orders" allowfilteringbycolumn="True" 
              ShowHeader = "true" ShowFooter="True" tablelayout="Auto" >
                 <CommandItemSettings ShowExportToWordButton="false" ShowExportToExcelButton="false" ShowExportToCsvButton="false">
            </CommandItemSettings>             
              <DetailTables>
              <telerik:GridTableView Name="OrderItems"  BorderWidth="1px" BorderColor="Black" AllowFilteringByColumn = "false" >            
                    <Columns>                         
                        <telerik:GridBoundColumn ReadOnly = "true"  UniqueName = "ItemIdColumn" SortExpression="OrderItemId" HeaderText="OrderItemId" HeaderButtonType="TextButton"
                            DataField="OrderItemId" Display = "false" Visible = "true" ForceExtractValue = "always">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width = "100px" ItemStyle-Width="200px" SortExpression="ProductName" HeaderText="Produktname" HeaderButtonType="TextButton"
                            DataField="ProductName">
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn UniqueName="ColumnPrice" SortExpression="Amount" HeaderText="Preis" HeaderStyle-Width="95px">
                            <itemtemplate>
                                <telerik:RadTextBox  Width = "75px" ID="tbEditPrice" Enabled="false" Text= '<%# Bind( "Amount") %>'  runat = "server">                                                  
                                </telerik:RadTextBox>                                             
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="AuthCharge" SortExpression="AuthCharge" HeaderText="Amtliche Gebühren" HeaderStyle-Width="95px">
                            <itemtemplate>
                                <telerik:RadTextBox  Width = "75px" ID="tbAuthChargePrice" Enabled="false" Text= '<%# Bind("AuthCharge") %>'   Visible='<%# Bind("AmtGebuhr") %>'    runat = "server">                                                  
                                </telerik:RadTextBox>                                             
                            </itemtemplate>
                        </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn HeaderStyle-Width = "100px" Display="false" Visible="true" ForceExtractValue="Always"
                            DataField="AuthChargeId"  UniqueName="AuthChargeId">
                        </telerik:GridBoundColumn>                             
                        <telerik:GridBoundColumn Display = "false" SortExpression="AmtGebuhr" HeaderText="Amtliche Gebühr" HeaderButtonType="TextButton"
                            DataField="AmtGebuhr">
                        </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn Display = "false" SortExpression="AmtGebuhr2" HeaderText="Amtliche Gebühr" HeaderButtonType="TextButton"
                            DataField="AmtGebuhr2">
                        </telerik:GridBoundColumn>
                    </Columns>                            
                </telerik:GridTableView>            
            </DetailTables>
            <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="true"  />
               <Columns>             
                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderId" HeaderText="OrderId"
                    SortExpression="OrderId" Display = "false" UniqueName="OrderId" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>  
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="locationId" HeaderText="locationId"
                    SortExpression="locationId" Display = "false" UniqueName="locationId" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>  
                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="customerID" HeaderText="customerID"
                    SortExpression="customerID" Display = "false" UniqueName="customerID" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>                           
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerName" HeaderText="Kundenname"
                    SortExpression="CustomerName" UniqueName="CustomerName" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="90px" DataField="Ordernumber" HeaderText="Auftragsnummer"
                    SortExpression="Ordernumber" UniqueName="Ordernumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="90px" DataField="Status" HeaderText="Auftragsstatus"
                    SortExpression="Status" UniqueName="Status" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                 <telerik:GridDateTimeColumn DataField="CreateDate" HeaderText="Erstellungsdatum" AutoPostBackOnFilter="true" FilterControlWidth="105px" 
                    SortExpression="CreateDate" UniqueName="CreateDate" PickerType="DatePicker"  ShowFilterIcon="false"
                     CurrentFilterFunction="Contains" EnableTimeIndependentFiltering="true">
                </telerik:GridDateTimeColumn>
                 <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerLocation" HeaderText="Standort"
                    SortExpression="CustomerLocation" UniqueName="CustomerLocation" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="VIN" HeaderText="FIN"
                    SortExpression="VIN" UniqueName="VIN" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                  <telerik:GridBoundColumn FilterControlWidth="70px" DataField="HSN" HeaderText="HSN"
                    SortExpression="HSN" UniqueName="HSN" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                  <telerik:GridBoundColumn FilterControlWidth="70px" DataField="TSN" HeaderText="TSN"
                    SortExpression="TSN" UniqueName="TSN" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderTyp" HeaderText="Auftragstyp"
                    SortExpression="OrderTyp" UniqueName="OrderTyp" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>                    
                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Freitext" HeaderText="Freitext"
                    SortExpression="Freitext" UniqueName="Freitext" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>                     
                   <telerik:GridBoundColumn Display = "false" FilterControlWidth="105px" DataField="Geprueft" HeaderText="Geprüft"
                    SortExpression="Geprueft" UniqueName="Geprueft" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>                    
                   <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Datum" HeaderText="Zulassungsdatum"
                    SortExpression="Datum" UniqueName="Datum" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>               
              </Columns>
            </mastertableview>
            <ItemStyle BackColor="#DFDFDF" />
              <clientsettings EnableAlternatingItems="false">              
            <Scrolling AllowScroll="false" ></Scrolling>
             <Selecting AllowRowSelect="false" ></Selecting>
        </clientsettings>
    </telerik:RadGrid>
<br />
<asp:LinqDataSource TableName = "Customer" ID="LinqDataSourceAllOrders" runat="server" OnSelecting="AllOrdersLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
<asp:LinqDataSource ID="CustomerDataSource" runat="server" OnSelecting="CustomerLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
<asp:LinqDataSource ID="ProductDataSource" runat="server" OnSelecting="ProductLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>