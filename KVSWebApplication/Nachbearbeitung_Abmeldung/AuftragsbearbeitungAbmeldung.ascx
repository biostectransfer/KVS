<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuftragsbearbeitungAbmeldung.ascx.cs" Inherits="KVSWebApplication.Nachbearbeitung_Abmeldung.AuftragsbearbeitungAbmeldung" %>
<telerik:RadCodeBlock runat="server">
    <script type="text/javascript">

        function openRadWindowPos() {
            $find("<%=RadWindow_Product.ClientID %>").show();
        }
        function MyValueChanging(sender, args) {
            args.set_newValue(args.get_newValue().toUpperCase());
        }

        function RowSelecting(sender, args) {
            if (args.get_tableView().get_name() != "MasterTableViewAbmeldung") {
                args.set_cancel(true);
            }
        }

        function GoToToday() {
            var datepicker = $find("<%=ZulassungsDatumPicker.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }

    function OnClientFilesUploaded(sender, args) {

        $find("<%=UploadRadAjaxManager.ClientID%>").ajaxRequest();

    }
    </script>
</telerik:RadCodeBlock>
<style type="text/css">
    .uppercase {
        text-transform: uppercase;
    }

    .RadPicker {
        display: inline-block !important;
    }

    * + html .RadPicker {
        display: inline !important;
    }

    * html .RadPicker {
        display: inline !important;
    }
</style>

<asp:Panel ID="PanelOffAbm1" runat="server">
    <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Sofort- oder Großkunde: " ID="RadTextBox2" Width="240px"></telerik:RadTextBox>
    <telerik:RadComboBox ID="RadComboBoxCustomerAbmeldungOffen" runat="server" OnSelectedIndexChanged="SmallLargeCustomerIndex_Changed"
        OnItemsRequested="SmallLargeCustomerIndex_Changed" AutoPostBack="true" Width="250px" EnableViewState="false">
        <Items>
            <telerik:RadComboBoxItem runat="server" Value="0" Text="Alle" />
            <telerik:RadComboBoxItem runat="server" Value="2" Text="Großkunden" />
            <telerik:RadComboBoxItem runat="server" Value="1" Text="Sofortkunden" />
        </Items>
    </telerik:RadComboBox>
    <asp:ImageButton ID="go" CssClass="infoState" OnClientClick="return false;" Style="cursor: pointer; width: 35px; margin-top: 15px;" runat="server" ImageUrl="../Pictures/achtung.gif" />
    <telerik:RadToolTip ManualClose="true" ManualCloseButtonText="Schließen"
        ID="ttOpenOrders" runat="server" ShowEvent="OnClick"
        TargetControlID="go" Animation="Slide"
        RelativeTo="Element" Position="BottomRight">
        Aktuell sind noch
        <asp:Label ID="ordersCount" runat="server"></asp:Label>
        Auftr&auml;ge offen.
    </telerik:RadToolTip>
    <br />
    <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent"
        DisabledStyle-BackColor="Transparent" Text="Bitte wählen Sie einen Kunden aus: " ID="RadTextBox1" Width="240px">
    </telerik:RadTextBox>
    <telerik:RadComboBox Height="300px" Width="250px" Enabled="true"
        Filter="Contains" runat="server" HighlightTemplatedItems="true" AutoPostBack="true"
        DropDownWidth="515px" EmptyMessage="Bitte wählen Sie einen Kunden aus: "
        DataTextField="Name" DataValueField="Value" ID="CustomerDropDownListAbmeldungOffen"
        OnSelectedIndexChanged="CustomerIndex_Changed" DataSourceID="CustomerDataSource">
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
                        <%# DataBinder.Eval(Container, "DataItem.Name")%>
                    </td>
                </tr>
            </table>
        </ItemTemplate>
    </telerik:RadComboBox>
    <br />
    <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black"
        BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Zusätzliche Optionen: " ID="RadTextBox3" Width="240px">
    </telerik:RadTextBox>
    <asp:Button runat="server" ID="ShowAllButton" OnClick="ShowAllButton_Click" Text="Alles anzeigen"></asp:Button>
    <asp:Button runat="server" ID="NewPositionButton" Text="Neue Position hinzufügen" OnClientClick="openRadWindowPos(); return false;" />
    <asp:Button runat="server" ID="StornierenButton" OnClick="StornierenButton_Clicked" Text="Auftrag stornieren" />
    <br />
    <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black"
        BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Abmeldedatum: " ID="RadTextBox4" Width="240px">
    </telerik:RadTextBox>
    <telerik:RadDatePicker DateInput-ButtonsPosition="Right" runat="server" ID="ZulassungsDatumPicker">
        <Calendar ID="Calendar1" runat="server">
            <FooterTemplate>
                <div style="width: 100%; text-align: center; background-color: Gray;">
                    <input id="Button1" type="button" value="Heute" onclick="GoToToday()" />
                </div>
            </FooterTemplate>
        </Calendar>
    </telerik:RadDatePicker>
    <asp:Button runat="server" ID="ZulassungsstelleLieferscheineButton" Text="Laufzettel erstellen" OnClick="ZulassungsstelleLieferscheineButton_Clicked" />
    <asp:Label runat="server" ID="LieferscheinePath" Visible="false"></asp:Label>
    <br />
    <asp:Label runat="server" ID="AbmeldungErrLabel" Text="Sie haben keinen Auftrag ausgewählt!" ForeColor="Red" Visible="false"></asp:Label>
    <asp:Label Visible="false" ID="AbmeldungOkLabel" Text="Ausgewählten Auftrag ist erfolgreich bearbeitet!" ForeColor="Green" runat="server" />
    <br />
    <asp:Label runat="server" ID="StornierungErfolgLabel" Text="Auftrag ist erfolgreich storniert" Visible="false" ForeColor="Green"></asp:Label>
    <telerik:RadFormDecorator runat="server" ID="AbmeldungDekorator" />



    <telerik:RadAjaxManager ID="UploadRadAjaxManager" runat="server" EnablePageHeadUpdate="false">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="UploadRadAjaxManager">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FPUpload" />
                    <telerik:AjaxUpdatedControl ControlID="MMUpload" />
                    <telerik:AjaxUpdatedControl ControlID="RentUpload" />
                    <telerik:AjaxUpdatedControl ControlID="MergeUpload" />
                    <telerik:AjaxUpdatedControl ControlID="RadGridAbmeldung" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>

    <table style="padding-left: 5px;">
        <tr>
            <td><asp:Label ID="FPLabel" Text="FP Datei: " runat="server"></asp:Label></td>
            <td>
                <telerik:RadAsyncUpload ID="FPUpload" runat="server"
                    OnClientFilesUploaded="OnClientFilesUploaded" OnFileUploaded="FPUpload_FileUploaded"
                    MaxFileSize="2097152" AllowedFileExtensions="csv"
                    AutoAddFileInputs="false" Localization-Select="Import" />
            </td>
        </tr>
        <tr>
            <td><asp:Label ID="MMLabel" Text="MM Datei: " runat="server"></asp:Label></td>
            <td>
                <telerik:RadAsyncUpload ID="MMUpload" runat="server"
                    OnClientFilesUploaded="OnClientFilesUploaded" OnFileUploaded="MMUpload_FileUploaded"
                    MaxFileSize="2097152" AllowedFileExtensions="csv"
                    AutoAddFileInputs="false" Localization-Select="Import" />
            </td>
        </tr>
        <tr>
            <td><asp:Label ID="RentLabel" Text="Rent Datei: " runat="server"></asp:Label></td>
            <td>
                <telerik:RadAsyncUpload ID="RentUpload" runat="server"
                    OnClientFilesUploaded="OnClientFilesUploaded" OnFileUploaded="RentUpload_FileUploaded"
                    MaxFileSize="2097152" AllowedFileExtensions="xls,xlsx"
                    AutoAddFileInputs="false" Localization-Select="Import" />
            </td>
        </tr>
        <tr>
            <td><asp:Label ID="MergeLabel" Text="Briefnummer Datei: " runat="server"></asp:Label></td>
            <td>
                <telerik:RadAsyncUpload ID="MergeUpload" runat="server"
                    OnClientFilesUploaded="OnClientFilesUploaded" OnFileUploaded="MergeUpload_FileUploaded"
                    MaxFileSize="2097152" AllowedFileExtensions="xls,xlsx"
                    AutoAddFileInputs="false" Localization-Select="Import" />
            </td>
        </tr>
    </table>
    
    <%--<telerik:RadAjaxPanel ID="RadAjaxPanelCostCenter" runat="server" Width="1600px" LoadingPanelID="RadAjaxLoadingPanelExports">--%>
    <%--<asp:Button runat="server" ID="btnExport" Text="Export" OnClick="Export_Button_Clicked" />--%>
    <%--</telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelExports" BackgroundTransparency="100" runat="server">
    </telerik:RadAjaxLoadingPanel>--%>
    <br />
    <br />

    <telerik:RadGrid AutoGenerateColumns="false" ID="RadGridAbmeldung" OnDetailTableDataBind="RadGridOffen_DetailTableDataBind" DataSourceID="LinqDataSourceAbmeldung"
        AllowFilteringByColumn="True" AllowSorting="True" PageSize="10" EnableHeaderContextMenu="true" OnItemCommand="OnItemCommand_Fired"
        ShowFooter="True" AllowPaging="True" Enabled="true" runat="server" OnEditCommand="EditButton_Clicked" GridLines="None"
        OnPreRender="RadGridAbmeldung_PreRender"
        EnableLinqExpressions="true" AllowMultiRowSelection="true">
        <GroupingSettings CaseSensitive="false"></GroupingSettings>
        <MasterTableView CommandItemDisplay="Top" ShowHeader="true" Name="MasterTableViewAbmeldung" AutoGenerateColumns="false" AllowFilteringByColumn="True" ShowFooter="True" TableLayout="Auto">
            <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
            <DetailTables>
                <telerik:GridTableView Name="OrderItems" Width="100%" AllowFilteringByColumn="false" EditMode="PopUp">
                    <Columns>
                        <telerik:GridBoundColumn ReadOnly="true" UniqueName="ItemIdColumn" SortExpression="OrderItemId" HeaderText="OrderItemId" HeaderButtonType="TextButton"
                            DataField="OrderItemId" Display="false" Visible="true" ForceExtractValue="always">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="100px" ItemStyle-Width="200px" SortExpression="ProductName" HeaderText="Produktname" HeaderButtonType="TextButton"
                            DataField="ProductName">
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn UniqueName="ColumnPrice" SortExpression="Amount" HeaderText="Preis" HeaderStyle-Width="95px">
                            <ItemTemplate>
                                <telerik:RadTextBox Width="75px" ID="tbEditPrice" Text='<%# Bind( "Amount") %>' runat="server">
                                </telerik:RadTextBox>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="AuthCharge" SortExpression="AuthCharge" HeaderText="Amtliche Gebühren" HeaderStyle-Width="95px">
                            <ItemTemplate>
                                <telerik:RadTextBox Width="75px" ID="tbAuthChargePrice" Text='<%# Bind("AuthCharge") %>'
                                    Visible='<%# Bind("AmtGebuhr") %>'
                                    runat="server">
                                </telerik:RadTextBox>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="100px" Display="false" Visible="true" ForceExtractValue="Always"
                            DataField="AuthChargeId" UniqueName="AuthChargeId">
                        </telerik:GridBoundColumn>
                        <telerik:GridButtonColumn HeaderStyle-Width="100px" ButtonType="PushButton" Text="Preis setzen" UniqueName="AmtGebSetzenButton" Visible="true" CommandName="AmtGebuhrSetzen">
                        </telerik:GridButtonColumn>
                        <telerik:GridButtonColumn HeaderStyle-Width="100px" ButtonType="PushButton" Text="Auftragsposition löschen"
                            UniqueName="RemoveOrderItem" Visible="true" CommandName="RemoveOrderItem">
                        </telerik:GridButtonColumn>

                        <telerik:GridBoundColumn Display="false" SortExpression="AmtGebuhr" HeaderText="Amtliche Gebühr" HeaderButtonType="TextButton"
                            DataField="AmtGebuhr">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn Display="false" SortExpression="AmtGebuhr2" HeaderText="Amtliche Gebühr" HeaderButtonType="TextButton"
                            DataField="AmtGebuhr2">
                        </telerik:GridBoundColumn>
                    </Columns>
                </telerik:GridTableView>
            </DetailTables>
            <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" />
            <Columns>
                <telerik:GridEditCommandColumn ButtonType="PushButton" EditText="Ändern" UniqueName="EditOffenColumn" />
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderNumber" HeaderText="Auftragsnummer"
                    SortExpression="OrderNumber" UniqueName="OrderNumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="locationId" HeaderText="locationId"
                    SortExpression="locationId" Display="false" UniqueName="locationId" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="customerID" HeaderText="customerID"
                    SortExpression="customerID" Display="false" UniqueName="customerID" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerName" HeaderText="Kundenname"
                    SortExpression="CustomerName" UniqueName="CustomerName" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <%--<telerik:GridBoundColumn FilterControlWidth="90px" DataField="Ordernumber" HeaderText="Auftragsnummer"
                    SortExpression="Ordernumber" UniqueName="Ordernumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>--%>
                <telerik:GridBoundColumn FilterControlWidth="90px" DataField="Status" HeaderText="Auftragsstatus"
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
                <telerik:GridBoundColumn Display="false" FilterControlWidth="105px" DataField="Geprueft" HeaderText="Geprüft"
                    SortExpression="Geprueft" UniqueName="Geprueft" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Datum" HeaderText="Abmeldedatum"
                    SortExpression="Datum" UniqueName="Datum" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
            </Columns>
            <EditFormSettings InsertCaption="Auftrag abmelden" EditColumn-HeaderText="Dieses Auftrag abmelden?" EditFormType="Template" PopUpSettings-Modal="true">
                <FormTemplate>
                    <br />
                    <asp:Label runat="server" ID="WellcomeNeuzulassungLabel" Font-Bold="true" Font-Size="Larger" Text="Hier können Sie die Daten überprüfen und den Auftrag für die Zulassungsstelle bereitstellen oder als Fehler markieren."></asp:Label>
                    <br />
                    <table id="Table1" cellspacing="1" cellpadding="1" width="250" border="0">
                        <tr>
                            <td></td>
                            <td></td>
                        </tr>
                        <asp:TextBox ID="orderIdBox" Visible="false" Text='<%# Bind( "OrderNumber") %>' runat="server">
                        </asp:TextBox>
                        <asp:TextBox ID="customerIdBox" Visible="false" Text='<%# Bind( "customerID") %>' runat="server">   </asp:TextBox>
                        <tr>
                            <td>Kundenname 
                            </td>
                            <td>
                                <asp:TextBox ID="CustomerNameBox" ReadOnly="true" Enabled="true" Text='<%# Bind( "CustomerName") %>' runat="server">
                                </asp:TextBox>
                            </td>
                            <td></td>
                            <td>Fehler
                            </td>
                            <td>
                                <asp:CheckBox runat="server" ID="ErrorCheckBox" OnCheckedChanged="ErrorCheckBox_Clicked" AutoPostBack="true" />
                            </td>
                            <td>Ursache
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="ErrorReasonTextBox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>Auftragsnummer
                            </td>
                            <td>
                                <asp:TextBox ID="OrderNumberBox" ReadOnly="true" Enabled="true" Text='<%# Bind( "OrderNumber") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>Status
                            </td>
                            <td>
                                <asp:TextBox ID="StatusBox" ReadOnly="true" Enabled="true" Text='<%# Bind( "Status") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>Erstellungsdatum
                            </td>
                            <td>
                                <telerik:RadDateTimePicker Calendar-Enabled="false" TimePopupButton-Enabled="false" ReadOnly="true" Enabled="true" ID="CreateDateBox" runat="server" SelectedDate='<%# Bind( "CreateDate") %>'></telerik:RadDateTimePicker>
                            </td>
                        </tr>
                        <tr>
                            <td>Standort
                            </td>
                            <td>
                                <asp:TextBox ID="LocationBox" ReadOnly="true" Enabled="true" Text='<%# Bind( "CustomerLocation") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>Kennzeichen
                            </td>
                            <td>
                                <asp:TextBox CssClass="uppercase" ID="KennzeichenBox" Text='<%# Bind( "Kennzeichen") %>' runat="server">
                                 <ClientEvents OnValueChanging="MyValueChanging" />
                                </asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>FIN
                            </td>
                            <td>
                                <asp:TextBox CssClass="uppercase" ID="VINBox" Text='<%# Bind( "VIN") %>' runat="server">
                                 <ClientEvents OnValueChanging="MyValueChanging" />
                                </asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>HSN
                            </td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="false" Text='<%# Bind( "HSN") %>' ID="HSNAbmFormBox"></asp:TextBox>
                                <asp:Label runat="server" Enabled="true" Visible="false" Text="" ID="HSNSearchLabel"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>TSN
                            </td>
                            <td>
                                <asp:TextBox runat="server" Text='<%# Bind( "TSN") %>' ID="TSNAbmFormBox"></asp:TextBox>
                            </td>
                        </tr>
                        <br />
                        <tr>
                            <td align="right" colspan="2">
                                <asp:Button ID="ZulassenButton" Text="Als geprüft speichern" runat="server" OnClick="AbmeldungZulassen_Command"></asp:Button>&nbsp;
                                <asp:Button ID="Button2" Text="Schließen" runat="server" CausesValidation="False" CommandName="Cancel"></asp:Button>
                            </td>
                        </tr>
                    </table>
                </FormTemplate>
            </EditFormSettings>
        </MasterTableView>
        <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
        <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
        <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />
        <ClientSettings AllowDragToGroup="false">
            <Scrolling AllowScroll="false" />
            <ClientEvents OnRowSelecting="RowSelecting" />
            <Selecting AllowRowSelect="True"></Selecting>
        </ClientSettings>
        <PagerStyle Mode="NextPrevNumericAndAdvanced"></PagerStyle>
    </telerik:RadGrid>
    <asp:HiddenField runat="server" ID="MakeHiddenField"></asp:HiddenField>
    <asp:HiddenField runat="server" ID="itemIndexHiddenField"></asp:HiddenField>
    <telerik:RadWindow Title="Neue Position hinzufügen" runat="server" ID="RadWindow_Product" Width="500px" Height="300px" Modal="true">
        <ContentTemplate>
            <p class="contText">
                Wählen Sie bitte neues Produkt aus
            </p>
            <telerik:RadComboBox Height="300px" Width="400px" Enabled="true"
                DataSourceID="ProductDataSource" AutoPostBack="false" Filter="Contains" runat="server" DropDownWidth="515px" EmptyMessage="Produkt..." HighlightTemplatedItems="true"
                DataTextField="Name" DataValueField="Value" ID="NewProductDropDownList">
                <HeaderTemplate>
                    <table style="width: 515px" cellspacing="0" cellpadding="0">
                        <tr align="center">
                            <td style="width: 90px;">Produktnummer
                            </td>
                            <td style="width: 175px;">Produktname
                            </td>
                            <td style="width: 250px">Warengruppe
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
            </telerik:RadComboBox>
            <%--      <p class="contText">
                   Wählen Sie bitte die Kostenstelle aus
             </p>             
             <asp:DropDownList DataTextField = "Name" DataValueField = "Value" Width="400px" DataSourceID = "CostCenterDataSource" runat = "server" ID = "CostCenterDropDownList" ></asp:DropDownList>--%>
            <p class="contText">
                Und bestätigen:
            </p>
            <div class="contButton">
                <asp:Button ID="NewPositionButtonHinzuguegen" Text="Neue Position hinzufügen" runat="server" OnClick="NewPositionButton_Clicked"></asp:Button>
                <asp:Button ID="Button2" Text="Schließen" runat="server" CausesValidation="False" CommandName="Cancel"></asp:Button>
            </div>
        </ContentTemplate>
    </telerik:RadWindow>
    <asp:LinqDataSource TableName="Customer" ID="LinqDataSourceAbmeldung" runat="server" OnSelecting="OrderLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext">
    </asp:LinqDataSource>
    <asp:LinqDataSource ID="CustomerDataSource" runat="server" OnSelecting="CustomerLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext">
    </asp:LinqDataSource>
    <asp:LinqDataSource ID="ProductDataSource" runat="server" OnSelecting="ProductLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext">
    </asp:LinqDataSource>
    <%--<asp:LinqDataSource ID="CostCenterDataSource" runat="server" OnSelecting="CostCenterDataSourceLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource> --%>
</asp:Panel>
