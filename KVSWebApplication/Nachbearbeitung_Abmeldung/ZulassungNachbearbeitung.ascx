<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ZulassungNachbearbeitung.ascx.cs" Inherits="KVSWebApplication.Nachbearbeitung_Abmeldung.ZulassungNachbearbeitung" %>
<telerik:RadScriptReference  Path = "~/Scripts/scripts.js" />
<telerik:RadCodeBlock ID="RadCodeBlock1" runat = "server">
<script type="text/javascript">
    function openRadWindowZulPos() {
        $find("<%=RadWindowZul_Product.ClientID %>").show();
    }	       
	function RowDblClick(sender, eventArgs) {
	    sender.get_masterTableView().editItem(eventArgs.get_itemIndexHierarchical());
	}
    function MyValueChanging(sender, args) {
        args.set_newValue(args.get_newValue().toUpperCase());
    }
    function RowSelecting(sender, args) {

        if (args.get_tableView().get_name() != "MasterTableViewAbmeldung") {
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
<asp:Panel runat = "server" ID = "Panel5">
                <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Sofort- oder Großkunden: " ID = "LrSmKundeTextBox" Width = "240px" ></telerik:RadTextBox>
                <telerik:radcombobox id="RadComboCustomerAbmeldungZulassunsstelle" runat="server"   Width="250px" 
                OnSelectedIndexChanged = "SmLrCustomerIndex_Changed" OnItemsRequested ="SmLrCustomerIndex_Changed"  AutoPostBack = "true" > 
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
                <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Bitte wählen Sie einen Kunden aus: " ID = "KundeRadTextBox" Width = "240px" ></telerik:RadTextBox>
                <telerik:RadComboBox  Height="300px" Width="250px"  Enabled = "true" 
                Filter="Contains" runat = "server"  
                 DropDownWidth="515px"  HighlightTemplatedItems="true"  EmptyMessage = "Bitte wählen Sie einen Kunden aus: "
                DataTextField = "Name" DataValueField = "Value" ID = "CustomerDropDownListAbmeldungZulassunsstelle"
                 OnSelectedIndexChanged="CustomerZulassungIndex_Changed"  DataSourceID="CustomerZulDataSource">
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
                <asp:Button runat = "server" ID = "NewPositionZulButton" Text = "Neue Position hinzufügen" OnClientClick="openRadWindowZulPos(); return false;" />
                <asp:Button runat="server" ID = "StornierenButton" OnClick = "StornierenButton_Clicked" Text = "Auftrag stornieren"/>
                <br />
<asp:Label runat = "server" ID = "StornierungErfolgLabel" Text = "Auftrag ist erfolgreich storniert" Visible = "false" ForeColor = "Green"></asp:Label>
<asp:Label Visible = "false" ForeColor = "Red" ID = "ErrorZulLabel" Text = "Fahrzeugdaten oder amtliche Gebühr oder die Dienstleistung sind nicht vorhanden!" runat = "server"></asp:Label>
<asp:Label Id = "AbmeldungErfolgtLabel" Visible = "false" Text="Auftrag ist erfoglreich abgeschlossen!" ForeColor = "Green" runat="server" />
<asp:Label runat = "server" ID = "AbmeldungErrLabel" Text = "Sie haben keinen Auftrag ausgewählt!" ForeColor = "Red" Visible = "false"></asp:Label>
<br />
</asp:Panel>
    <telerik:RadGrid AutoGenerateColumns="false" ID="RadGridAbmeldung" DataSourceId = "LinqDataSourceZulassungCust" 
    AllowFilteringByColumn="True" AllowSorting="True" PageSize="5" EnableViewState ="true" OnItemCommand = "OnItemCommand_Fired" OnPreRender="RadGridAbmeldung_PreRender"
    ShowFooter="True" OnDetailTableDataBind="RadGridAbmeldung_DetailTableDataBind" AllowPaging="True" Enabled = "true" OnEditCommand = "Edit_Command" runat="server" 
    GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "false" >
    <pagerstyle mode="NextPrevAndNumeric"></pagerstyle>
    <groupingsettings casesensitive="false" ></groupingsettings>
    <mastertableview CommandItemDisplay = "Top" ShowHeader = "true"  autogeneratecolumns="false"  Name="MasterTableViewAbmeldung" allowfilteringbycolumn="True" showfooter="True" tablelayout="Auto" >
<DetailTables>
    <telerik:GridTableView Name="OrderItems" Width="100%" AllowFilteringByColumn = "false" EditMode = "PopUp">                                     
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
    <br />   
        <table id="Table1" cellspacing="10" cellpadding="1" width="250" border="0">                     
                <asp:TextBox ID="orderIdBox" Visible = "false" Text='<%# Bind( "OrderNumber") %>' runat="server">
                    </asp:TextBox>
                <asp:TextBox ID="customerIdBox" Visible = "false" Text='<%# Bind( "customerID") %>' runat="server"></asp:TextBox>
            <tr>
                <td>
                    Kundenname 
                </td>
                <td>
                    <asp:TextBox ID="CustomerNameBox"  ReadOnly = "true" Enabled = "true" Text='<%# Bind( "CustomerName") %>' runat="server">
                    </asp:TextBox>
                </td>
                    <td>
                    Erstellungsdatum
                </td>
                <td>
                    <telerik:RadDateTimePicker  Calendar-Enabled = "false" TimePopupButton-Enabled = "false" ReadOnly = "true" Enabled = "true" ID = "CreateDateBox" runat = "server" SelectedDate = '<%# Bind( "CreateDate") %>'></telerik:RadDateTimePicker>          
                </td>
                    <td>
                    Standort
                </td>
                <td>
                    <asp:TextBox ID="LocationBox" ReadOnly = "true" Enabled = "true" Text='<%# Bind( "CustomerLocation") %>' runat="server">
                    </asp:TextBox>
                </td>    
            <td >
                <asp:Label ID="lblErrorMessage" runat="server" Text="Fehler"></asp:Label> 
                </td>
                <td>
                <asp:CheckBox runat = "server" ID = "ErrorZulCheckBox" OnCheckedChanged = "ErrorCheckBox_Clicked" AutoPostBack = "true"/>
            </td>
            </tr>
            <tr>
                <td>
                    Auftragsnummer
                </td>
                <td>
                    <asp:TextBox ID="OrderNumberBox"  ReadOnly = "true" Enabled = "true" Text='<%# Bind( "OrderNumber") %>' runat="server">
                    </asp:TextBox>
                </td>                         
                    <td>
                    Status
                </td>
                <td>
                    <asp:TextBox ID="StatusBox"   ReadOnly = "true" Enabled = "true" Text='<%# Bind( "Status") %>' runat="server">
                    </asp:TextBox>
                </td>
                    <td>
                    FIN
                </td>
                <td>
                    <asp:TextBox CssClass="uppercase"  ID="VINBox" Text='<%# Bind( "VIN") %>' runat="server">
                    <ClientEvents OnValueChanging="MyValueChanging" />
                    </asp:TextBox>
                </td>
                    <td>
                    <asp:Label ID="lblreason" runat="server" Text="Ursache:"></asp:Label> 
                    </td>
                <td>
                <asp:TextBox runat = "server" ID = "ErrorReasonZulTextBox"></asp:TextBox>
                </td>
            </tr>
        <tr>
            <td>
                    HSN
                </td>
                <td>
                <asp:TextBox runat = "server" Text='<%# Bind( "HSN") %>' AutoPostBack = "true" OnTextChanged = "HSNBox_TextChanged" ID = "HSNAbmBox"></asp:TextBox>
                <asp:Label runat = "server" Enabled = "true" Visible = "false" Text = "" ID = "HSNSearchLabel"></asp:Label>
                </td>
                    <td>
                    TSN
                </td>
                <td>
                <asp:TextBox runat = "server" Text='<%# Bind( "TSN") %>' ID = "TSNAbmBox"></asp:TextBox>                         
                </td>
                    <td>
                    Kennzeichen*
                </td>
                <td>
                    <asp:TextBox CssClass="uppercase"  ID="KennzeichenBox" Text='<%# Bind( "Kennzeichen") %>' runat="server">
                    <ClientEvents OnValueChanging="MyValueChanging" />
                    </asp:TextBox>                         
                </td> 
                <td></td> 
                <td align="right" colspan="6">
                    <asp:Button ID="FertigStellenButton" Text="Fertigstellen" runat="server" OnClick = "AbmeldungFertigStellen_Command">
                    </asp:Button>&nbsp;
                </td>
                </tr>
                <tr>                   
                </tr>                    
        </table>
            <telerik:RadGrid AutoGenerateColumns="false" ID="RadGridNeuzulassungDetails" 
    AllowSorting="false" AllowFilteringByColumn="false"  OnItemCommand = "OnItemCommand_Fired" 
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
            <Scrolling AllowScroll="false" ></Scrolling>
            <Selecting AllowRowSelect="true" ></Selecting>            
              <ClientEvents  OnRowSelecting="RowSelecting" />  
        </clientsettings>
    </telerik:RadGrid>
<telerik:RadWindow Title = "Neue Position hinzufügen" runat="server" ID="RadWindowZul_Product" Width="500px" Height="300px"  Modal="true">
        <ContentTemplate>
            <p class="contText">
                Wählen Sie bitte neues Produkt aus
            </p>
 <telerik:RadComboBox  Height="300px" Width="400px"   Enabled = "true" 
                DataSourceID = "ProductDataSource" AutoPostBack = "false" Filter="Contains" runat = "server"  DropDownWidth="515px" EmptyMessage = "Produkt..." HighlightTemplatedItems="true"
                DataTextField = "Name" DataValueField = "Value" ID = "NewProductDropDownList" >                 
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
           <%-- <p class="contText">
                Wählen Sie bitte die Kostenstelle aus
            </p>             
            <asp:DropDownList DataTextField = "Name" DataValueField = "Value" Width="400px" DataSourceID = "CostCenterDataSource" runat = "server" ID = "CostCenterDropDownList" ></asp:DropDownList>--%>
            <p class="contText">
                Und bestätigen:
            </p>
            <div class="contButton">
                <asp:Button ID="NewPositionButtonHinzuguegen" Text="Neue Position hinzufügen" runat="server" OnClick = "NewPositionButton_Clicked">
                </asp:Button>
                <asp:Button ID="Button2" Text="Schließen" runat="server" CausesValidation="False" CommandName="Cancel" >
            </asp:Button>
            </div>               
        </ContentTemplate>
    </telerik:RadWindow>
<asp:HiddenField runat = "server" ID = "MakeHiddenField"> </asp:HiddenField>
<asp:HiddenField runat = "server" ID = "itemIndexHiddenField"> </asp:HiddenField>
<asp:HiddenField runat = "server" ID = "smallCustomerOrderHiddenField"/>
<asp:HiddenField runat = "server" ID = "InvoiceIdHidden"/>             
 <telerik:RadWindowManager runat = "server" ID = "WindowManager1" EnableViewState = "false" DestroyOnClose = "true" VisibleOnPageLoad = "false" ReloadOnShow = "true"></telerik:RadWindowManager>
    <telerik:RadWindow ShowContentDuringLoad = "false" runat="server" Height = "400" Width = "350" ID="AddAdressRadWindow" Modal="true" DestroyOnClose = "true" ReloadOnShow = "true">   
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
                    <asp:Label runat = "server" ID = "Label5" Text = "Rechnungsempfänger*: " Width = "130"></asp:Label>
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
     </telerik:RadWindow>
<asp:LinqDataSource ID="ProductDataSource" runat="server" OnSelecting="ProductLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
<%--<asp:LinqDataSource ID="CostCenterDataSource" runat="server" OnSelecting="CostCenterDataSourceLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>--%>
<asp:LinqDataSource TableName = "Customer" ID="LinqDataSourceZulassungCust" runat="server" OnSelecting="AbmeldungenLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
<asp:LinqDataSource ID="CustomerZulDataSource" runat="server" OnSelecting="CustomerZulLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>