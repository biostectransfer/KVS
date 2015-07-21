<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Zulassungsstelle.ascx.cs" Inherits="KVSWebApplication.Auftragsbearbeitung_Neuzulassung.Zulassungsstelle" %>                       
                 <telerik:RadScriptReference  Path = "~/Scripts/scripts.js" />     
<telerik:RadCodeBlock ID="RadCodeBlock1" runat = "server">
	            <script type="text/javascript">
	                function RequestStart1(sender, eventArgs) {
	                    var eventTarget = eventArgs.get_eventTarget();
	                    if (eventTarget.indexOf("Button1") != -1) {
	                        eventArgs.set_enableAjax(false);
	                    }
	                }
	                function MyValueChanging(sender, args) {
	                    args.set_newValue(args.get_newValue().toUpperCase());
	                }
	                function RowSelecting(sender, args) {

	                    if (args.get_tableView().get_name() != "Orders") {
	                        args.set_cancel(true);
	                    }
	                }
	                function openFile(path) {
	                    window.open(path, "_blank", "left=0,top=0,scrollbars=yes,menubar=yes,toolbar=yes,resizable=yes");
	                }
              </script>
</telerik:RadCodeBlock>
<link type="text/css"  rel="stylesheet" href="../Styles/zulassungstelle.css" />
<style type="text/css">
.uppercase
{
    text-transform: uppercase;
}
</style>
      <telerik:RadFormDecorator ID = "MyDecorator" runat = "server" />
   <asp:Panel ID = "panel111" runat = "server">
   </asp:Panel>
                <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Sofort- oder Großkunde: " ID = "RadTextBox2" Width = "240px" ></telerik:RadTextBox>
                <telerik:radcombobox id="RadComboBoxCustomerZulassungsstelle" runat="server"  
                OnSelectedIndexChanged = "SmallLargeCustomerIndex_Changed" Width="250px" OnItemsRequested ="SmallLargeCustomerIndex_Changed"  AutoPostBack = "true" > 
                <Items>                    
                    <telerik:RadComboBoxItem runat="server" Value = "2" Text="Großkunden" />   
                    <telerik:RadComboBoxItem runat="server" Value = "1" Text="Sofortkunden" />   
                </Items>
                 </telerik:radcombobox>                 
                 <asp:ImageButton ID="go" CssClass="infoState" OnClientClick="return false;"  style="cursor:pointer; width:35px; margin-top:15px;"  runat="server" ImageUrl="../Pictures/achtung.gif"  />
                 <telerik:RadToolTip ManualClose="true" ManualCloseButtonText="Schließen"    
                                         ID="ttOpenOrders" runat="server"    ShowEvent="OnClick"
                                          TargetControlID="go" Animation="Slide" 
                                          RelativeTo="Element"  Position="BottomRight">
                        Aktuell sind noch <asp:Label ID="ordersCount" runat="server"></asp:Label> Auftr&auml;ge offen.
                                    </telerik:RadToolTip>
                 <br />
                <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Bitte wählen Sie einen Kunden aus: " ID = "RadTextBox1" Width = "240px" ></telerik:RadTextBox>
               <telerik:RadComboBox  Height="300px" Width="250px"  Enabled = "true" 
                     DataSourceID = "CustomerDataSource" AutoPostBack = "true" Filter="Contains" runat = "server"  
                    DropDownWidth="515px" EmptyMessage="Wählen Sie einen Kunden aus" HighlightTemplatedItems="true"
                DataTextField = "Name" DataValueField = "Value" ID = "CustomerDropDownListZulassungsstelle" 
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
                     <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" 
                BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Zusätzliche Optionen: " ID = "RadTextBox3" Width = "240px" ></telerik:RadTextBox>
                <asp:Button runat = "server" ID = "ShowAllButton" OnClick = "ShowAllButton_Click" Text = "Alles anzeigen"></asp:Button>
                <asp:Button runat = "server" ID = "NewPositionZulButton" Text = "Neue Position hinzufügen" OnClientClick="openRadWindowZulNeuzulassungPos(); return false;" />
                <asp:Button runat="server" ID = "StornierenButton" OnClick = "StornierenButton_Clicked" Text = "Auftrag stornieren"/>
                <br />
                <asp:Label Id = "ZulassungErfolgtLabel" Visible = "false" Text="Auftrag ist erfoglreich zugelassen!" ForeColor = "Green" runat="server" />
                <asp:Label runat = "server" ID = "ZulassungErrLabel" Text = "Sie haben keinen Auftrag ausgewählt!" ForeColor = "Red" Visible = "false"></asp:Label>
                <asp:Label runat = "server" ID = "StornierungErfolgLabel" Text = "Auftrag ist erfolgreich storniert" Visible = "false" ForeColor = "Green"></asp:Label>
                <br />        
                <telerik:RadGrid AutoGenerateColumns="false" ID="RadGridNeuzulassung" DataSourceId = "LinqDataSourceZulassung" 
                AllowFilteringByColumn="True" AllowSorting="True" PageSize="5" OnItemCommand = "OnItemCommand_Fired" OnPreRender="RadGridNeuzulassung_PreRender"
                ShowFooter="True"   AllowPaging="True" Enabled = "true" OnEditCommand = "Edit_Command"    OnDetailTableDataBind="RadGridNeuzulassung_DetailTableDataBind"                
                 runat="server" GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "false" >
                <pagerstyle mode="NextPrevAndNumeric"></pagerstyle>
                <groupingsettings casesensitive="false" ></groupingsettings>
             <mastertableview CommandItemDisplay = "Top" ShowHeader = "true" autogeneratecolumns="false" Name="Orders" allowfilteringbycolumn="True" showfooter="True" tablelayout="Auto">            
            <DetailTables>
              <telerik:GridTableView Name="OrderItems" Width="100%" AllowFilteringByColumn = "false" EditFormSettings-EditFormType  = "Template" EditMode = "PopUp">   
                        </telerik:GridTableView>                                 
            </DetailTables>           
             <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="true"  />
               <Columns>
                 <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderNumber" HeaderText="OrderNumber"
                    SortExpression="OrderNumber" Display = "false" UniqueName="OrderNumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
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
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Ordernumber" HeaderText="Auftragsnummer"
                    SortExpression="Ordernumber" UniqueName="Ordernumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Status" HeaderText="Auftragsstatus"
                    SortExpression="Status" UniqueName="Status" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                 <telerik:GridDateTimeColumn DataField="CreateDate" HeaderText="Erstellungsdatum" FilterControlWidth="95px"
                    SortExpression="CreateDate" UniqueName="CreateDate" PickerType="DatePicker" EnableTimeIndependentFiltering="true">
                </telerik:GridDateTimeColumn>
                 <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerLocation" HeaderText="Standort"
                    SortExpression="CustomerLocation" UniqueName="CustomerLocation" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="VIN" HeaderText="FIN"
                    SortExpression="VIN" UniqueName="VIN" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="HSN" HeaderText="HSN"
                    SortExpression="HSN" UniqueName="HSN" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="TSN" HeaderText="TSN"
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
               </Columns>    
                <NestedViewTemplate>
                   <asp:Panel runat="server" ID="InnerContainer" CssClass="nestedTemplate" Visible="true">
                   <br />
                <asp:Label runat = "server" ID = "WellcomeNeuzulassungLabel" Font-Bold = "true" Font-Size = "Larger" Text = "Hier können Sie die Daten überprüfen und den Auftrag fertigstellen oder als Fehler markieren."></asp:Label>      
                    <asp:TextBox ID="orderIdBox" Visible = "false" Text='<%# Bind( "OrderNumber") %>' runat="server">
                                </asp:TextBox>
                         <asp:TextBox ID="customerIdBox" Visible = "false" Text='<%# Bind( "customerID") %>' runat="server">   </asp:TextBox>                                        
                  <table id="Table1" cellspacing="10" cellpadding="1" width="250" border="0">       
                        <tr>
                            <td>
                                Kundenname 
                            </td>
                            <td>
                                <asp:TextBox ID="CustomerNameBox"   ReadOnly = "true" Enabled = "true" Text='<%# Bind( "CustomerName") %>' runat="server">
                                </asp:TextBox>
                            </td>
                                 <td>
                                Erstellungsdatum
                            </td>
                            <td>
                                <telerik:RadDateTimePicker  Calendar-Enabled = "false" TimePopupButton-Enabled = "false" ReadOnly = "true" Enabled = "false" ID = "CreateDateBox" runat = "server" SelectedDate = '<%# Bind( "CreateDate") %>'></telerik:RadDateTimePicker>          
                            </td>
                               <td>
                                FIN
                            </td>
                            <td>
                                <asp:TextBox ID="VINBox" CssClass="uppercase" Text='<%# Bind( "VIN") %>' runat="server">
                                   <ClientEvents OnValueChanging="MyValueChanging" />
                                </asp:TextBox>                         
                            </td>
                            <td>
                                Fehler
                            </td>
                            <td>
                            <asp:CheckBox runat = "server" ID = "ErrorZulCheckBox"  AutoPostBack = "false"/>
                             </td>
                             <td>
                             Ursache
                            </td>
                            <td>
                               <asp:TextBox runat = "server" ID = "ErrorReasonZulTextBox"></asp:TextBox>
                            </td>                                                   
                        </tr>
                        <tr>
                            <td>
                                Auftragsnummer
                            </td>
                            <td>
                                <asp:TextBox ID="OrderNumberBox"   ReadOnly = "true" Enabled = "true" Text='<%# Bind( "OrderNumber") %>' runat="server">
                                </asp:TextBox>
                            </td>
                             <td>
                                Standort
                            </td>
                            <td>
                                <asp:TextBox ID="LocationBox" ReadOnly = "true" Enabled = "true" Text='<%# Bind( "CustomerLocation") %>' runat="server">
                                </asp:TextBox>
                            </td>
                                 <td>
                                HSN
                            </td>
                           <td>
                           <asp:TextBox Text='<%# Bind( "HSN") %>' AutoPostBack = "true" OnTextChanged = "HSNBox_TextChanged" runat = "server" ID = "HSNAbmBox"></asp:TextBox>                 
                            <asp:Label runat = "server" Enabled = "true" Visible = "false" Text = "" ID = "HSNSearchLabel"></asp:Label>
                            </td>
                                   <td align="right" colspan="4">
                                <asp:Button ID="FertigStellenButton" Text="Fertigstellen" runat="server" OnClick = "AuftragFertigStellen_Command">
                                </asp:Button>&nbsp;                     
                            </td>
                        </tr>
                          <tr>
                            <td>
                                Auftragsstatus
                            </td>
                            <td>
                                <asp:TextBox ID="StatusBox" ReadOnly = "true" Enabled = "true" Text='<%# Bind( "Status") %>' runat="server">
                                </asp:TextBox>
                            </td>
                              <td>
                                Kennzeichen*
                            </td>
                            <td>                     
                                <asp:TextBox CssClass="uppercase"  ID="KennzeichenBox" Text='<%# Bind( "Kennzeichen") %>' runat="server">
                                   <ClientEvents OnValueChanging="MyValueChanging" />
                                </asp:TextBox>                           
                            </asp:RequiredFieldValidator>
                            </td>
                               <td>
                                TSN
                            </td>
                            <td>
                             <asp:TextBox Text='<%# Bind( "TSN") %>' MaxLength = "4" runat = "server" ID = "TSNAbmBox"></asp:TextBox>                 
                            </td>
                        </tr>                                     
                       <br />
                        <tr>                     
                        </tr>
                    </table>
                  <telerik:RadGrid AutoGenerateColumns="false" ID="RadGridNeuzulassungDetails" 
               AllowSorting="false" AllowFilteringByColumn="false" OnItemCommand = "OnItemCommand_Fired" 
              AllowPaging="false" Enabled = "true"                
                 runat="server" GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "false" >                              
                <pagerstyle mode="NextPrevAndNumeric"></pagerstyle>
                <groupingsettings casesensitive="false" ></groupingsettings>
                    <mastertableview CommandItemDisplay = "Top" ShowHeader = "true" autogeneratecolumns="false" Name="Orders" showfooter="false" tablelayout="Auto">
                            <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="false"  />
                      <Columns>
                   <telerik:GridBoundColumn ReadOnly = "true"  UniqueName = "ItemIdColumn" SortExpression="OrderItemId" HeaderText="OrderItemId" HeaderButtonType="TextButton"
                                    DataField="OrderItemId" Display = "false" Visible = "true" ForceExtractValue = "always">
                                </telerik:GridBoundColumn>
                             <telerik:GridBoundColumn HeaderStyle-Width = "100px" ItemStyle-Width="200px"  SortExpression="ProductName" HeaderText="Produktname" HeaderButtonType="TextButton"
                                    DataField="ProductName">
                                </telerik:GridBoundColumn>
                               <telerik:GridTemplateColumn UniqueName="ColumnPrice" SortExpression="Amount" HeaderText="Preis" HeaderStyle-Width="95px">
                                                <itemtemplate>
                                                     <telerik:RadTextBox  Width = "75px" ID="tbEditPrice" Text= '<%# Bind( "Amount") %>'  runat = "server">                                                  
                                                    </telerik:RadTextBox>                                             
                                                </itemtemplate>
                               </telerik:GridTemplateColumn>
                                               <telerik:GridTemplateColumn UniqueName="AuthCharge" SortExpression="AuthCharge" HeaderText="Amtliche Gebühren" HeaderStyle-Width="95px">
                                                <itemtemplate>
                                                     <telerik:RadTextBox  Width = "75px" ID="tbAuthChargePrice" Text= '<%# Bind("AuthCharge") %>'  Visible='<%# Bind("AmtGebuhr") %>'   runat = "server">                                                 
                                                    </telerik:RadTextBox>                                             
                                                </itemtemplate>
                               </telerik:GridTemplateColumn>
                                  <telerik:GridBoundColumn HeaderStyle-Width = "100px" Display="false" Visible="true" ForceExtractValue="Always"
                                    DataField="AuthChargeId"  UniqueName="AuthChargeId"></telerik:GridBoundColumn>
                             <telerik:GridButtonColumn HeaderStyle-Width = "100px" ButtonType = "PushButton" Text = "Preis setzen" UniqueName = "AmtGebSetzenButton" Visible = "true" CommandName = "AmtGebuhrSetzen">
                             </telerik:GridButtonColumn>                               
                                <telerik:GridBoundColumn Display = "false" SortExpression="AmtGebuhr" HeaderText="Amtliche Gebühr" HeaderButtonType="TextButton"
                                    DataField="AmtGebuhr">
                                </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn Display = "false" SortExpression="AmtGebuhr2" HeaderText="Amtliche Gebühr" HeaderButtonType="TextButton"
                                    DataField="AmtGebuhr2">
                                </telerik:GridBoundColumn>
                            </Columns>
            </mastertableview>
            </telerik:RadGrid>
                   </asp:Panel>
                   </NestedViewTemplate>
          </mastertableview>
              <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
            <AlternatingItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
        <clientsettings>
                       <ClientEvents  OnRowSelecting="RowSelecting" />  
            <Scrolling AllowScroll="false" ></Scrolling>
             <Selecting AllowRowSelect="true" ></Selecting>
        </clientsettings>
    </telerik:RadGrid>
    <telerik:RadFormDecorator runat = "server" ID = "OffenNeuzulassungFormDekorator" DecoratedControls="all"/>
<telerik:RadWindow Title = "Neue Position hinzufügen" runat="server" ID="RadWindowZul_Product" Width="500px" Height="300px" Modal="true">
    <ContentTemplate>
        <p class="contText">
            Wählen Sie bitte neues Produkt aus
        </p>
 <telerik:RadComboBox  Height="300px" Width="400px"   Enabled = "true" 
                DataSourceID = "ProductDataSource" AutoPostBack = "false" Filter="Contains" runat = "server"  DropDownWidth="515px" EmptyMessage = "Produkt..." HighlightTemplatedItems="true"
                DataTextField = "Name" DataValueField = "Value" ID = "NewProductDropDownList"  >                 
                     <HeaderTemplate>
                                   <table style="width: 515px" cellspacing="0" cellpadding="0">
                                        <tr align="center">
                                            <td style="width: 90px;">
                                                  Produktnummer
                                             </td>
                                             <td style="width: 175px;">
                                                 Produktname
                                             </td>                                         
                                             <td style="width: 250px">
                                                  Warengruppe
                                             </td>
                                        </tr>
                                   </table>
                              </HeaderTemplate>
                                 <ItemTemplate>
                                   <table style="width: 515px;" cellspacing="0" cellpadding="0">
                                        <tr>
                                             <td style="width: 110px;">
                                                  <%# DataBinder.Eval(Container, "DataItem.ItemNumber")%>
                                             </td>
                                              <td style="width: 175px;">
                                                  <%# DataBinder.Eval(Container, "DataItem.Name")%>                                                
                                             </td>
                                             <td style="width: 250px;">
                                                  <%# DataBinder.Eval(Container, "DataItem.Category")%>
                                             </td>
                                        </tr>
                                   </table>
                              </ItemTemplate>
                 </telerik:RadComboBox >
     <%--   <p class="contText">
            Wählen Sie bitte die Kostenstelle aus
        </p>             
         <asp:DropDownList DataTextField = "Name" DataValueField = "Value"  Width="400px" DataSourceID = "CostCenterDataSource" runat = "server" ID = "CostCenterDropDownList" ></asp:DropDownList>          --%>
        <p class="contText">
            Und bestätigen:
        </p>
        <div class="contButton">
            <asp:Button ID="NewPositionButtonHinzuguegen" Text="Neue Position hinzufügen" runat="server" OnClick = "NewPositionButton_Clicked">
            </asp:Button>
        </div>               
    </ContentTemplate>
</telerik:RadWindow>
    <asp:HiddenField runat = "server" ID = "MakeHiddenField"> </asp:HiddenField>
    <asp:HiddenField runat = "server" ID = "itemIndexHiddenField"> </asp:HiddenField>
    <asp:HiddenField runat = "server" ID = "smallCustomerOrderHiddenField"/>
    <asp:HiddenField  runat = "server" ID = "orderitemidHiddenField"/>
<asp:HiddenField runat = "server" ID = "InvoiceIdHidden"/>
   <telerik:RadWindowManager runat = "server" ID = "WindowManager1" EnableViewState = "false" DestroyOnClose = "true" VisibleOnPageLoad = "false" ReloadOnShow = "true"></telerik:RadWindowManager>
    <telerik:RadWindow ShowContentDuringLoad = "false" runat="server" Height = "400" Width = "350" ID="AddAdressRadWindow" Modal="true" DestroyOnClose = "true" ReloadOnShow = "true">
          <ContentTemplate>   
          <ContentTemplate>
          <asp:Label runat = "server" ID = "LocationLabelWindow"></asp:Label>
               <div class="contButton">
               <asp:HiddenField ID = "CustomerIdHiddenField" runat = "server"/>
                <asp:Label runat = "server" ID = "StreetLabel" Text = "Straße: " Width = "130"></asp:Label>
                    <asp:TextBox ID="StreetTextBox" runat="server"></asp:TextBox>
                    <br />
                    <asp:Label runat = "server" ID = "Label1" Text = "Nummer: " Width = "130"></asp:Label>
                    <asp:TextBox ID="StreetNumberTextBox" runat="server"></asp:TextBox>
                    <br />
                    <asp:Label runat = "server" ID = "Label2" Text = "ZIP: " Width = "130"></asp:Label>
                    <asp:TextBox ID="ZipcodeTextBox" runat="server"></asp:TextBox>
                    <br />
                    <asp:Label runat = "server" ID = "Label3" Text = "Stadt: " Width = "130"></asp:Label>
                    <asp:TextBox ID="CityTextBox" runat="server"></asp:TextBox>
                    <br />
                    <asp:Label runat = "server" ID = "Label4" Text = "Land: " Width = "130"></asp:Label>
                    <asp:TextBox ID="CountryTextBox" runat="server"></asp:TextBox> 
                    <br />
                    <asp:Label runat = "server" ID = "Label5" Text = "Rechnungsempfänger: " Width = "130"></asp:Label>
                     <asp:TextBox ID="InvoiceRecipient" runat="server"></asp:TextBox>  
                     <br />  
                      <asp:Label runat = "server" ID = "Label6" Text = "Rabatt in %: "  Width = "130"></asp:Label>
                    <telerik:RadNumericTextBox  ID="txbDiscount" runat="server"  NumberFormat-DecimalDigits="0" Value="0" MinValue="0" MaxValue="100" ></telerik:RadNumericTextBox>  
               </div>
               <br />  
               <br />  
               <div class="contButton">
                    <telerik:RadButton ID="AddAdressButton" Text="Speichern und neue Rechnungsauftrag erstellen" runat="server" OnClick = "OnAddAdressButton_Clicked">
                    </telerik:RadButton>
               </div>
               <asp:Label ID = "AllesIstOkeyLabel" runat = "server" Text = ""></asp:Label>
               <asp:Label ID = "ZusatzlicheInfoLabel" Visible = "false" runat = "server" Text = "*Die Rechnung wird sofort erstellt!"></asp:Label>               
          </ContentTemplate>
          </ContentTemplate>
     </telerik:RadWindow>
<asp:LinqDataSource ID="ProductDataSource" runat="server" OnSelecting="ProductLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
<%--<asp:LinqDataSource ID="CostCenterDataSource" runat="server" OnSelecting="CostCenterDataSourceLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>--%>
<asp:LinqDataSource TableName = "Customer" ID="LinqDataSourceZulassung" runat="server" OnSelecting="AbmeldungenLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
<asp:LinqDataSource ID="CustomerDataSource" runat="server" OnSelecting="CustomerLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>