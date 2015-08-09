<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AbrechnungSave.ascx.cs" Inherits="KVSWebApplication.Abrechnung.AbrechnungSave" %>
<script>
    function openFile(path) {
        window.open(path, "_blank", "left=0,top=0,scrollbars=yes,menubar=yes,toolbar=yes,resizable=yes");
    }
</script>
<asp:Panel ID="AllControlsPanel" runat="server" Visible="true">
    <table border="0" width="200px">
        <tr>
            <td>
                <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Sofort- oder Großkunde: " ID="RadTextBox2" Width="240px"></telerik:RadTextBox>
            </td>
            <td>
                <telerik:RadComboBox ID="RadComboBoxCustomer" runat="server" Width="250px"
                    OnSelectedIndexChanged="Customer_Selected" OnItemsRequested="Customer_Selected" AutoPostBack="true">
                    <Items>
                        <telerik:RadComboBoxItem runat="server" Value="2" Text="Großkunden" />
                        <telerik:RadComboBoxItem runat="server" Value="1" Text="Sofortkunden" />
                    </Items>
                </telerik:RadComboBox>
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Bitte wählen Sie bestimmten Kunde aus: " ID="RadCustomerTextBox" Width="240px"></telerik:RadTextBox>
            </td>
            <td>
                <telerik:RadComboBox Height="300px" Width="250px" Enabled="true"
                    Filter="Contains" runat="server" HighlightTemplatedItems="true" AutoPostBack="true"
                    DropDownWidth="515px" EmptyMessage="Bitte wählen Sie einen Kunden aus: "
                    DataTextField="Name" DataValueField="Value" ID="CustomerDropDownList"
                    OnSelectedIndexChanged="CustomerChanged_Selected" DataSourceID="CustomerDataSource">
                    <HeaderTemplate>
                        <table style="width: 515px" cellspacing="0" cellpadding="0">
                            <tr align="center">
                                <td style="width: 90px;">Kundennummer
                                </td>
                                <td style="width: 175px;">Matchcode
                                </td>

                                <td style="width: 250px">Kundenname
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
        </tr>
        <tr>
            <td>
                <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Wählen Sie ein Rechnungstyp aus: " ID="RechnungsTypTextBox" Width="240px"></telerik:RadTextBox>
            </td>
            <td>
                <telerik:RadComboBox ID="RechnungsTypComboBox" runat="server" OnSelectedIndexChanged="RechnungsTyp_Selected"
                    OnItemsRequested="RechnungsTyp_Selected" Enabled="false" AutoPostBack="true" Width="250px">
                    <Items>
                        <telerik:RadComboBoxItem runat="server" Value="Einzel" Text="Einzelrechnung" />
                        <telerik:RadComboBoxItem runat="server" Value="Sammel" Text="Sammelrechnung" />
                        <telerik:RadComboBoxItem runat="server" Value="Woche" Text="Wochenrechnung" />
                        <telerik:RadComboBoxItem runat="server" Value="Monat" Text="Monatsrechnung" />
                    </Items>
                </telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Bitte wählen Sie Standort aus: " ID="StandortLabel" Width="240px"></telerik:RadTextBox>
            </td>
            <td>
                <telerik:RadDropDownList Enable="true" DataTextField="Name" DataValueField="Value" DataSourceID="StandortDataSource"
                    Width="250px"
                    AutoPostBack="true" runat="server" ID="StandortDropDown" OnSelectedIndexChanged="Standort_Changed">
                </telerik:RadDropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Oder wählen Sie alle Standorte: " ID="AllStandOrteLabel" Width="240px"></telerik:RadTextBox>
            </td>
            <td>
                <asp:CheckBox runat="server" Enabled="false" ID="AllLocationsCheckBox" AutoPostBack="true" OnCheckedChanged="AllLocationsCheckBox_Changed" />
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Weitere Optionen: " ID="RadTextBox1" Width="240px"></telerik:RadTextBox>
            </td>
            <td>
                <asp:Button runat="server" ClientIDMode="static" OnClick="AddAdressButton_Click" ID="AddAdressButton"
                    Text="Adresse hinzufügen" Width="140"></asp:Button>
                &nbsp;
                <asp:Button runat="server" ClientIDMode="static" OnClick="CreateInvoiceButton_Click" ID="CreateInvoiceButton"
                    Text="Rechnung" Width="90"></asp:Button>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ForeColor="Red" runat="server" ID="AbrechnungSaveErrorLabel" Visible="false"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>
<telerik:RadGrid ID="RadGridAbrechnung" DataSourceID="LinqDataSourceAbrechnung" OnSelectedIndexChanged="Cell_Selected"
    AllowFilteringByColumn="True" AllowSorting="True" PageSize="15" OnSelectedCellChanged="Cell_Selected"
    ShowFooter="True" AllowPaging="false" Enabled="true" runat="server" GridLines="None" AllowMultiRowSelection="true">
    <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
    <MasterTableView AutoGenerateColumns="false" AllowFilteringByColumn="True" CommandItemDisplay="Top" ShowHeader="true" ShowFooter="True" TableLayout="Auto">
        <HeaderStyle Width="150px" />
        <NoRecordsTemplate>Keine Datensätze vorhanden</NoRecordsTemplate>
        <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" />
        <Columns>
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderNumber" HeaderText="Auftragsnummer"
                SortExpression="OrderNumber"   UniqueName="OrderNumber" Visible="true" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderItemId" HeaderText="OrderItemId"
                SortExpression="OrderItemId" Display="false" UniqueName="OrderItemId" Visible="true" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderLocation" HeaderText="OrderLocation"
                SortExpression="OrderLocation" Display="false" UniqueName="OrderLocation" Visible="true" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CostCenterId" HeaderText="CostCenterId"
                SortExpression="CostCenterId" Display="false" UniqueName="CostCenterId" Visible="true" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CostCenterName" HeaderText="Kostenstelle"
                SortExpression="CostCenterName" UniqueName="CostCenterName" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Amount" HeaderText="Preis" DataFormatString="{0:C2}"
                SortExpression="Amount" UniqueName="Amount" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Location" HeaderText="Standort"
                SortExpression="Location" UniqueName="Location" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="ItemCount" HeaderText="Rechnungspositionen"
                SortExpression="ItemCount" UniqueName="ItemCount" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false">
            </telerik:GridBoundColumn>
            <%--<telerik:GridBoundColumn FilterControlWidth="105px" DataField="Ordernumber" HeaderText="Auftragsnummer"
                    SortExpression="Ordernumber" UniqueName="Ordernumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>--%>
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="ProductName" HeaderText="Produktname"
                SortExpression="ProductName" UniqueName="ProductName" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false">
            </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn DataField="ExecutionDate" HeaderText="Vollzugszeitpunkt" AutoPostBackOnFilter="true"
                SortExpression="ExecutionDate" UniqueName="ExecutionDate" PickerType="DatePicker" EnableTimeIndependentFiltering="true">
            </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="ItemStatus" HeaderText="Positionsstatus"
                SortExpression="ItemStatus" UniqueName="ItemStatus" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderDate" HeaderText="Auftragsdatum"
                SortExpression="OrderDate" UniqueName="OrderDate" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                ShowFilterIcon="false">
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
    <ClientSettings ReorderColumnsOnClient="true" AllowDragToGroup="false" EnablePostBackOnRowClick="true">
        <Resizing AllowColumnResize="true" ResizeGridOnColumnResize="true"></Resizing>
        <Scrolling AllowScroll="false"></Scrolling>
        <Selecting AllowRowSelect="true"></Selecting>
    </ClientSettings>
</telerik:RadGrid>
<asp:ListBox Visible="false" runat="server" ID="LocationIdsHiddenListBox"></asp:ListBox>
<telerik:RadAjaxPanel runat="server" ID="WindowAjaxPanel">
    <telerik:RadWindowManager runat="server" ID="WindowManager1" EnableViewState="false" DestroyOnClose="true" VisibleOnPageLoad="false" ReloadOnShow="true"></telerik:RadWindowManager>
    <telerik:RadWindow ShowContentDuringLoad="false" runat="server" Height="400" Title='<%# Bind( "Location") %>' Width="350" ID="AddAdressRadWindow" Modal="true" DestroyOnClose="true" ReloadOnShow="true">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label runat="server" ID="LocationLabelWindow"></asp:Label>
                    <div class="contButton">
                        <asp:Label runat="server" ID="StreetLabel" Text="Straße: " Width="130"></asp:Label>
                        <asp:TextBox ID="StreetTextBox" runat="server"></asp:TextBox>
                        <br />
                        <asp:Label runat="server" ID="Label1" Text="Nummer: " Width="130"></asp:Label>
                        <asp:TextBox ID="StreetNumberTextBox" runat="server"></asp:TextBox>
                        <br />
                        <asp:Label runat="server" ID="Label2" Text="ZIP: " Width="130"></asp:Label>
                        <asp:TextBox ID="ZipcodeTextBox" runat="server"></asp:TextBox>
                        <br />
                        <asp:Label runat="server" ID="Label3" Text="Stadt: " Width="130"></asp:Label>
                        <asp:TextBox ID="CityTextBox" runat="server"></asp:TextBox>
                        <br />
                        <asp:Label runat="server" ID="Label4" Text="Land: " Width="130"></asp:Label>
                        <asp:TextBox ID="CountryTextBox" runat="server"></asp:TextBox>
                        <br />
                        <asp:Label runat="server" ID="Label5" Text="Rechnungsempfänger: " Width="130"></asp:Label>
                        <asp:TextBox ID="InvoiceRecipient" runat="server"></asp:TextBox>
                        <br />
                        <br />
                        <asp:Label runat="server" ID="Label6" Text="Text: " Width="130"></asp:Label>
                        <asp:TextBox ID="FooterTextBox" Wrap="true" TextMode="MultiLine" runat="server"></asp:TextBox>
                        <br />
                    </div>
                    <br />
                    <br />
                    <div class="contButton">
                        <asp:Button ID="Button1" Text="Rechnung speichern" runat="server" OnClick="OnAddAdressButton_Clicked"></asp:Button>
                        <asp:Button ID="btnPreviewInvoice" Text="Rechnungsvorschau" runat="server" OnClick="btnPreviewInvoice_Clicked"></asp:Button>
                    </div>
                    <asp:Label ID="AllesIstOkeyLabel" runat="server" Text=""></asp:Label>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
    <br />
</telerik:RadAjaxPanel>
<telerik:RadFormDecorator ID="MyDecorator" runat="server" DecoratedControls="all" />
<asp:LinqDataSource ID="LinqDataSourceAbrechnung" runat="server"
    OnSelecting="AbrechnungLinq_Selected">
</asp:LinqDataSource>
<asp:LinqDataSource ID="CustomerDataSource" runat="server" OnSelecting="CustomerLinq_Selected">
</asp:LinqDataSource>
<asp:LinqDataSource ID="StandortDataSource" runat="server" OnSelecting="StandortLinq_Selected">
</asp:LinqDataSource>
