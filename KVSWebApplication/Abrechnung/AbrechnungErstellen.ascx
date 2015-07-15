<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AbrechnungErstellen.ascx.cs" Inherits="KVSWebApplication.Abrechnung.AbrechnungErstellen" %>

<telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Bitte wählen Sie einen Kunde aus: " ID = "RadCustomerTextBox" Width = "240px" ></telerik:RadTextBox>
 
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
<br /><br />
<asp:Panel ID = "AllButtonsPanel" Visible = "true" runat = "server">
     <table border="0">
     <tr>
     <td>
     <asp:Button runat = "server" ID = "RechnungErstellenButton" OnClick = "RechnungErstellen_Click" Text = "Rechnung Erstellen"></asp:Button>
    <asp:LinkButton ID="LinkButton1" runat="server" />
      </td>
          <td>
      <asp:Button  runat = "server" ID = "StornierenButton" Text="Stornieren" Enabled="false" OnClick = "Stornieren_Clicked"></asp:Button >    
      </td>
      <td>
    <asp:Button ClientIDMode = "static" Enabled = "false" ID = "AddButton" runat = "server" Text = "Hinzufügen"  OnClick = "AddButton_Clicked" 
    OnClientClick = "openWinContentTemplate(); return false;"> </asp:Button> 
     </td>  
      <td>
      <asp:Button  runat = "server" Enabled = "false" ID = "EmailSendButton" OnClick = "EmailSendButton_Clicked" Text = "Rechnung per Email senden"></asp:Button>    
      </td>
      <td>
      <asp:Button  ID = "PrintCopyButton" Enabled = "false" runat = "server" Text = "Kopie drucken" OnClick = "PrintCopyButton_Clicked"></asp:Button >   
      </td>
     <td>
      <asp:Button  ID = "btnShowInvoice" Enabled = "false" runat = "server" Text = "Rechnung anzeigen" OnClick = "ShowInvoiceButton_Clicked"></asp:Button >   
      </td>
      <td>
      <asp:CheckBox ID="defaultAccountNumber" runat="server" Checked="true" Text="Standard Erlös-Konto" ToolTip="<%$AppSettings: DefaultAccountNumber %>"  />
      </td>
     </tr>
     </table>
     <asp:Label runat = "server" ID = "RechnungVorschauErrorLabel" Text = "Für Rechnungvorschau haben Sie keine Positionen ausgewählt!" ForeColor = "Red" Visible = "false"></asp:Label>
      <asp:Label runat = "server" Visible = "false" ID = "EmailOkeyLabel" ForeColor = "Green" Text = "Email wurde erfolgreich gesendet!"></asp:Label> 
      <asp:Label runat = "server" Visible = "false" ID = "PrintCopyErrorLabel" ForeColor = "Red" Text = "Ausgewählten Auftrag ist noch nicht gedruckt!"></asp:Label>
</asp:Panel>
  <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
  <script type="text/javascript">
      function populateValue() {
          var selecteditem = $find("<%= RadGridAbrechnungErstellen.MasterTableView.ClientID %>").get_selectedItems();
          if (selecteditem.length > 0 && selecteditem.length < 2) {
                  document.getElementById("<%=AmountField.ClientID %>").value = $get("<%= AmountTextBox.ClientID %>").value;
                  document.getElementById("<%=NameField.ClientID %>").value = $get("<%= InvoiceNameTextBox.ClientID %>").value;
                  __doPostBack('New_Param', '');
          }
          else { 
          alert('Kein Auftrag oder mehr als 1 ausgewählt ist! Bitte nur 1 auswählen!')
          }
          $find("<%=RadWindow_ContentTemplate.ClientID%>").close();
      }

      function openWinContentTemplate() {
          $find("<%=RadWindow_ContentTemplate.ClientID %>").show();
      }
                    
      function RequestStart(sender, eventArgs) {
            var eventTarget = eventArgs.get_eventTarget();
            if (eventTarget.indexOf("RechnungErstellenButton") != -1) {
                eventArgs.set_enableAjax(false);
            }
        }
        function RowSelecting(sender, args) {

            if (args.get_tableView().get_name() != "Invoices") {
                args.set_cancel(true);
            }

        } 
 </script>
</telerik:RadCodeBlock>
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
             <mastertableview autogeneratecolumns="false" Width="1000px"  DataKeyNames="InvoiceId,isPrinted"  allowfilteringbycolumn="True" CommandItemDisplay = "Top"  
             ShowHeader = "true" showfooter="True"  tablelayout="Auto"  Name="Invoices">
            <DetailTables >
              <telerik:GridTableView Name="OrderItems"   AllowFilteringByColumn = "false"  EditMode = "PopUp" DataSourceID="detailGridSource">   
                <ParentTableRelation>
                            <telerik:GridRelationFields DetailKeyField="InvoiceId" MasterKeyField="InvoiceId" />
                             <telerik:GridRelationFields DetailKeyField="isPrinted" MasterKeyField="isPrinted" />
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
                                     <telerik:GridBoundColumn Visible = "true" Display = "false"  ForceExtractValue = "always" 
                                     FilterControlWidth="105px" DataField="isPrinted" HeaderText="Gedruckt"
                                SortExpression="isPrinted" UniqueName="isPrinted" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                                ShowFilterIcon="false">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="Amount" HeaderText="Preis" DataFormatString="{0:C2}"  HeaderButtonType="TextButton"
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
                <telerik:GridDateTimeColumn DataField="createDate" HeaderText="Erstellt" FilterControlWidth="95px" AutoPostBackOnFilter="true"
                    SortExpression="createDate" UniqueName="createDate" ShowFilterIcon="false" CurrentFilterFunction="Contains" PickerType="DatePicker" EnableTimeIndependentFiltering="true">
                </telerik:GridDateTimeColumn>
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
                 <telerik:GridBoundColumn FilterControlWidth="105px" DataField="invoiceNumber" HeaderText="Rechnungsnummer"
                    SortExpression="invoiceNumber"  Visible = "true" ForceExtractValue="Always" UniqueName="invoiceNumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>   
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="isPrintedMEssage" HeaderText=""
                    SortExpression="isPrintedMEssage" UniqueName="isPrintedMEssage" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn Visible = "true" Display = "false" FilterControlWidth="105px" DataField="isPrinted" HeaderText="Gedruckt"
                    SortExpression="isPrinted" UniqueName="isPrinted" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
              </Columns>
            </mastertableview>            
                <ClientSettings ReorderColumnsOnClient="true"  EnablePostBackOnRowClick="true"  AllowDragToGroup="false">
                  <Scrolling AllowScroll="false"  />
               <Resizing  AllowColumnResize="true" ResizeGridOnColumnResize="true"></Resizing> 
                <ClientEvents  OnRowSelecting="RowSelecting" />  
            <Scrolling AllowScroll="false" ></Scrolling>
             <Selecting AllowRowSelect="true"  ></Selecting>
        </ClientSettings>
    </telerik:RadGrid>  
    <br />
    <asp:Label ID="Label1" Text="" runat="server"></asp:Label>
    <telerik:RadWindowManager runat = "server" ID = "WindowManager"></telerik:RadWindowManager>
    <telerik:RadWindow Title = "Neue Position hinzufügen" runat="server" ID="RadWindow_ContentTemplate" Modal="true">
          <ContentTemplate>
               <p class="contText">
                    Geben Sie bitte den Betrag und den Name für neue Position
               </p>
               <div class="contButton">
               <asp:Label runat = "server" ID = "AmountLabel" Text = "Betrag " Width = "50"></asp:Label>
                    <asp:TextBox ID="AmountTextBox" CausesValidation = "true"  runat="server"></asp:TextBox>
                    <br />
                    <asp:Label runat = "server" ID = "InvoiceItemNameLabel" Text = "Name: " Width = "50"></asp:Label>
                    <asp:TextBox ID="InvoiceNameTextBox" runat="server"></asp:TextBox>
               </div>
               <p class="contText">
                    Und bestätigen:
               </p>
               <div class="contButton">
                    <asp:Button ID="Button1" Text="Neue Position speichern" runat="server" OnClientClick="populateValue(); return false;">
                    </asp:Button>
               </div>               
          </ContentTemplate>
     </telerik:RadWindow>
       <telerik:RadFormDecorator ID = "MyDecorator" runat = "server" DecoratedControls = "Buttons" />     
     <asp:LinqDataSource ID="CustomerDataSource" runat="server" OnSelecting="CustomerLinq_Selected" >                 
    </asp:LinqDataSource>
<asp:LinqDataSource  ID="LinqDataSourceAbrechnung" runat="server" OnSelecting="AbrechnungLinq_Selected">                 
    </asp:LinqDataSource>
    <asp:LinqDataSource ID="detailGridSource" runat="server" OnSelecting="DetailTable_Selected"
          Where="InvoiceId.ToString() == @InvoiceId">
            <WhereParameters>
                <asp:Parameter Name="InvoiceId"  />
                <asp:Parameter Name="isPrinted"  />
            </WhereParameters>                    
    </asp:LinqDataSource>
    <telerik:RadAjaxLoadingPanel runat = "server" ID = "VersandLoadingPanel"></telerik:RadAjaxLoadingPanel>
