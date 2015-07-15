<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StornierteRechnungen.ascx.cs" Inherits="KVSWebApplication.Abrechnung.StornierteRechnungen" %>
<script>
    function RowSelecting(sender, args) {
        if (args.get_tableView().get_name() != "Invoices") {
            args.set_cancel(true);
        }
    } 
</script>
<telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" 
Text = "Bitte wählen Sie einen Kunde aus: " ID = "RadCustomerTextBox" Width = "240px" ></telerik:RadTextBox>
                 <telerik:RadComboBox  Height="300px" Width="250px"  Enabled = "true"  
                Filter="Contains" runat = "server" HighlightTemplatedItems="true" AutoPostBack = "true" 
                 DropDownWidth="515px"    EmptyMessage = "Bitte wählen Sie einen Kunden aus: "
                DataTextField = "Name" DataValueField = "Value" ID = "CustomerDropDownList"
                 OnSelectedIndexChanged="CustomerIndex_Changed"   DataSourceID="CustomerDataSource">   
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
                   <asp:Button runat = "server" Text = "X" 
                     ID = "clearButton" OnClick="clearButton_Click" ToolTip="Auswahl löschen" > </asp:Button>
                       <asp:Button  runat = "server" ID = "btnRechnungsvorschau" Text="Vorschau" OnClick = "RechnungVorschauButton_Clicked"></asp:Button >    
                       <br />
                                  <br />
<asp:Panel ID = "AllButtonsPanel" Visible = "true" runat = "server">   
     <asp:Label runat = "server" ID = "RechnungVorschauErrorLabel" Text = "Für Rechnungvorschau haben Sie keine Positionen ausgewählt!" ForeColor = "Red" Visible = "false"></asp:Label>
      <asp:Label runat = "server" Visible = "false" ID = "EmailOkeyLabel" ForeColor = "Green" Text = "Email wurde erfolgreich gesendet!"></asp:Label> 
      <asp:Label runat = "server" Visible = "false" ID = "PrintCopyErrorLabel" ForeColor = "Red" Text = "Ausgewählten Auftrag ist noch nicht gedruckt!"></asp:Label>
</asp:Panel>
<telerik:RadFormDecorator runat = "server" ID = "VersandDecorator"/>
   <asp:Panel ID = "panel11" runat = "server">
   </asp:Panel>
                    <asp:HiddenField ID = "AmountField" runat = "server"/>
                    <asp:HiddenField ID = "NameField" runat = "server"/>
           <telerik:RadWindowManager ID="RadWindowManagerAbrechnungErstellen" runat="server" EnableShadow="true">
          </telerik:RadWindowManager>
<telerik:RadGrid AutoGenerateColumns="false" ID="RadGridAbrechnungErstellen"  OnSelectedIndexChanged = "Cell_Selected" 
OnSelectedCellChanged = "Cell_Selected" DataSourceId = "LinqDataSourceAbrechnung" 
                AllowFilteringByColumn="True" AllowSorting="True" PageSize="15"
                ShowFooter="True" AllowPaging="True" Enabled = "false" runat="server"  
                GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "false" >
                <pagerstyle mode="NextPrevAndNumeric"></pagerstyle>
                <groupingsettings casesensitive="false" ></groupingsettings>                
                 <HeaderStyle Width="150px" />
             <mastertableview autogeneratecolumns="false" Width="1000px"  DataKeyNames="InvoiceId"  allowfilteringbycolumn="True" CommandItemDisplay = "Top"  ShowHeader = "true" showfooter="True"  tablelayout="Auto"  Name="Invoices">
            <DetailTables >
              <telerik:GridTableView Name="OrderItems"  AllowFilteringByColumn = "false"  EditMode = "PopUp" DataSourceID="detailGridSource">   
                <ParentTableRelation>
                            <telerik:GridRelationFields DetailKeyField="InvoiceId" MasterKeyField="InvoiceId" />                            
                        </ParentTableRelation>         
                            <Columns>   
                                <telerik:GridBoundColumn ReadOnly = "true"  UniqueName = "ItemId" SortExpression="ItemId" HeaderText="ItemId" HeaderButtonType="TextButton"
                                    DataField="ItemId" Display = "false" Visible = "true" ForceExtractValue = "always">
                                </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn ReadOnly = "true"  UniqueName = "OrderItemId" SortExpression="OrderItemId" HeaderText="OrderItemId" HeaderButtonType="TextButton"
                                    DataField="OrderItemId" Display = "false" Visible = "true" ForceExtractValue = "always">
                                </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn ReadOnly = "true"  UniqueName = "InvoiceId" SortExpression="InvoiceId" HeaderText="InvoiceId" HeaderButtonType="TextButton"
                                    DataField="InvoiceId" Display = "false" Visible = "true" ForceExtractValue = "always">
                                </telerik:GridBoundColumn>                                
                                <telerik:GridBoundColumn SortExpression="Amount" HeaderText="Preis" HeaderButtonType="TextButton"
                                    DataField="Amount">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="Name" HeaderText="Name" HeaderButtonType="TextButton"
                                    DataField="Name">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="Count" HeaderText="Anzahl" HeaderButtonType="TextButton"
                                    DataField="Count">
                                </telerik:GridBoundColumn> 
                                 <telerik:GridTemplateColumn HeaderText="Erlöskonten" HeaderButtonType="TextButton"  DataField="AccountCol"
                                          HeaderStyle-Width="120px" FilterControlWidth="85px" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center" 
                                          AutoPostBackOnFilter="true">
                                          <ItemTemplate>
                                              <asp:Label ID="lblItemId" runat="server" Visible="false"   Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceItemId").ToString() %>'></asp:Label>
                                              <asp:Label ID="lblAccountId" runat="server" Visible="false" Text='<%#  DataBinder.Eval(Container, "DataItem.AccountId").ToString() %>'></asp:Label>
                                              <telerik:RadTextBox ID="AccountText" runat="server"  Enabled='<%#  DataBinder.Eval(Container, "DataItem.Active") %>'
                                              Text='<%#  DataBinder.Eval(Container, "DataItem.AccountNumber").ToString() %>'></telerik:RadTextBox>
                               </ItemTemplate>
                               </telerik:GridTemplateColumn>
                            </Columns>
                        </telerik:GridTableView>                                    
            </DetailTables>
             <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="true"  />
               <Columns>           
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="invoiceId" HeaderText="invoiceId"
                    SortExpression="invoiceId" UniqueName="invoiceId" Visible = "true" Display = "false" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                 <telerik:GridBoundColumn FilterControlWidth="105px" DataField="customerId" HeaderText="customerId"
                    SortExpression="customerId" UniqueName="customerId" Visible = "true" Display = "false" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridDateTimeColumn DataField="createDate" HeaderText="Erstellt" FilterControlWidth="95px"  AutoPostBackOnFilter="true"
                    SortExpression="createDate" UniqueName="createDate" ShowFilterIcon="false" PickerType="DatePicker" EnableTimeIndependentFiltering="true">
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="isPrinted" HeaderText="Gedruckt"
                    SortExpression="isPrinted" UniqueName="isPrinted" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="recipient" HeaderText="Empfänger"
                    SortExpression="recipient" UniqueName="recipient" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn> 
                     <telerik:GridBoundColumn FilterControlWidth="105px" DataField="customerNumber" HeaderText="Kundennummer"
                    SortExpression="customerNumber" UniqueName="customerNumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn> 
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Matchcode" HeaderText="Matchcode"
                    SortExpression="Matchcode" UniqueName="Matchcode" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>  
                    <telerik:GridBoundColumn FilterControlWidth="105px" DataField="customerName" HeaderText="Kundenname"
                    SortExpression="customerName" UniqueName="customerName" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn> 
              </Columns>
            </mastertableview>            
                <ClientSettings ReorderColumnsOnClient="true"  EnablePostBackOnRowClick="false"  AllowDragToGroup="false">
                  <Scrolling AllowScroll="false"  />
               <Resizing  AllowColumnResize="true" ResizeGridOnColumnResize="true"></Resizing> 
                <ClientEvents  OnRowSelecting="RowSelecting" />  
            <Scrolling AllowScroll="false" ></Scrolling>
             <Selecting AllowRowSelect="true"  ></Selecting>           
        </clientsettings>
    </telerik:RadGrid>
     <asp:LinqDataSource ID="CustomerDataSource" runat="server" OnSelecting="CustomerLinq_Selected" >                 
    </asp:LinqDataSource>
<asp:LinqDataSource  ID="LinqDataSourceAbrechnung" runat="server" OnSelecting="AbrechnungLinq_Selected">                 
    </asp:LinqDataSource>
    <asp:LinqDataSource ID="detailGridSource" runat="server" OnSelecting="DetailTable_Selected"
          Where="InvoiceId.ToString() == @InvoiceId">
            <WhereParameters>
                <asp:Parameter Name="InvoiceId"  />
            </WhereParameters>                    
    </asp:LinqDataSource>
    <telerik:RadAjaxLoadingPanel runat = "server" ID = "VersandLoadingPanel"></telerik:RadAjaxLoadingPanel>
