<%@ Page Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="search.aspx.cs" Inherits="KVSWebApplication.Search.search" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <body>
                <form id="form1">
                    <asp:ScriptManager ID="SearchManager" runat="server"></asp:ScriptManager>

                    <telerik:RadAjaxLoadingPanel runat="server" ID="LoadingPanel1" BackgroundTransparency="100"></telerik:RadAjaxLoadingPanel>

                    <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1" LoadingPanelID="LoadingPanel1" Height="100%">
                        <asp:Label ID="SucheLabel" runat="server" ForeColor="Blue" Font-Bold="true" Text="Suche: "></asp:Label>
                        <br />
                        <%--<telerik:RadComboBox ID="CustomerNameBox" EnableTextSelection = "true" MarkFirstMatch = "true" runat="server" Width="300" 
                                DataTextField = "Name" DataValueField = "Value" EmptyMessage="Kundenname" DataSourceID="LinqDataSourceCustomerName"></telerik:RadComboBox>
                        --%>

                        <telerik:RadComboBox Height="300px" Width="250px" Enabled="true"
                            Filter="Contains" runat="server" HighlightTemplatedItems="true" AutoPostBack="false"
                            DropDownWidth="515px" EmptyMessage="Bitte wählen Sie einen Kunden aus: "
                            DataTextField="Name" DataValueField="Value" ID="CustomerNameBox"
                            DataSourceID="LinqDataSourceCustomerName">


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
                        

                        <telerik:RadTextBox ID="KennzeichenSearchBox" runat="server" EmptyMessage="Kennzeichen"></telerik:RadTextBox>
                        <telerik:RadTextBox ID="HalterNameBox" runat="server" EmptyMessage="Haltername"></telerik:RadTextBox>
                        <telerik:RadTextBox ID="FINBox" runat="server" EmptyMessage="FIN"></telerik:RadTextBox>
                        <telerik:RadButton runat="server" Text="Suchen" ID="searchButton" OnClick="searchButton_Clicked"></telerik:RadButton>
                        <telerik:RadButton runat="server" Text="Suche leeren" ID="NeueSucheButton" OnClick="NeueSucheButton_Clicked"></telerik:RadButton>
                        <br />
                        <asp:Label runat="server" ID="SearchErrorLabel" ForeColor="Red" Visible="false"></asp:Label>
                        <br />


                        <telerik:RadGrid AutoGenerateColumns="false" ID="RadGridSearch" DataSourceID="RadGridSearch_NeedDataSource" EnableViewState="false"
                            AllowFilteringByColumn="True" AllowSorting="True" PageSize="10" PagerStyle-AlwaysVisible="true" OnItemCommand="RadGridSearch_ItemCommand"
                            ShowFooter="True" AllowPaging="True" Enabled="true" runat="server" GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection="false">

                            <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
                            <GroupingSettings CaseSensitive="false"></GroupingSettings>

                            <MasterTableView CommandItemDisplay="Top" ShowHeader="true" AllowSorting="true" EditMode="PopUp" AutoGenerateColumns="false" AllowFilteringByColumn="True" ShowFooter="True" TableLayout="Auto">

                                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" />
                                <Columns>

                                    <telerik:GridTemplateColumn UniqueName="ZumAuftragText" ShowFilterIcon="false" AllowFiltering="false">
                                        <ItemTemplate>
                                            <asp:Button ID="btnZumAuftrag" Text='<%# Bind( "ZumAuftragText") %>' runat="server" CommandName="Zum Auftrag" Enabled='<%# Bind( "VisibleWeiterleitung") %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <%-- <telerik:GridButtonColumn ButtonType = "PushButton" Text = "Zum Auftrag" UniqueName = "VisibleWeiterleitung"   >
                                         </telerik:GridButtonColumn>--%>

                                    <telerik:GridEditCommandColumn ButtonType="PushButton" EditText="Details" UniqueName="EditOffenColumn" />
                                    <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderNumber" HeaderText="OrderNumber"
                                        SortExpression="OrderNumber" UniqueName="OrderNumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                                        ShowFilterIcon="false">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn AllowFiltering="false">
                                        <HeaderTemplate>
                                            Laufzettel          
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ForeColor="Green" Font-Bold="true" runat="server" ID="Laufzettel" Text='<%# Bind( "PostBackUrl") %>'></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerId" HeaderText="CustomerId"
                                        SortExpression="CustomerId" Display="false" UniqueName="CustomerId" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                                        ShowFilterIcon="false">
                                    </telerik:GridBoundColumn>

                                    <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerName" HeaderText="Kundenname"
                                        SortExpression="CustomerName" UniqueName="CustomerName" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                                        ShowFilterIcon="false">
                                    </telerik:GridBoundColumn>

                                    <%--<telerik:GridBoundColumn FilterControlWidth="105px" DataField="Ordernumber" HeaderText="Auftragsnummer"
                                        SortExpression="Ordernumber" UniqueName="Ordernumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                                        ShowFilterIcon="false">
                                    </telerik:GridBoundColumn>--%>

                                    <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderTyp" HeaderText="Auftragstyp"
                                        SortExpression="OrderTyp" UniqueName="OrderTyp" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                                        ShowFilterIcon="false">
                                    </telerik:GridBoundColumn>

                                    <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Kennzeichen" HeaderText="Kennzeichen"
                                        SortExpression="Kennzeichen" UniqueName="Kennzeichen" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                                        ShowFilterIcon="false">
                                    </telerik:GridBoundColumn>

                                    <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Haltername" HeaderText="Haltername"
                                        SortExpression="Haltername" UniqueName="Haltername" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                                        ShowFilterIcon="false">
                                    </telerik:GridBoundColumn>

                                    <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Status" HeaderText="Auftragsstatus"
                                        SortExpression="Status" UniqueName="Status" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                                        ShowFilterIcon="false">
                                    </telerik:GridBoundColumn>

                                    <telerik:GridDateTimeColumn DataField="CreateDate" HeaderText="Erstellungsdatum" FilterControlWidth="95px" AutoPostBackOnFilter="true" ShowFilterIcon="false"
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

                                    <telerik:GridBoundColumn FilterControlWidth="105px" DataField="HasErrorAsString" HeaderText="Fehler"
                                        SortExpression="HasErrorAsString" UniqueName="HasErrorAsString" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                                        ShowFilterIcon="false">
                                    </telerik:GridBoundColumn>

                                </Columns>
                                <EditFormSettings PopUpSettings-Width="1000" CaptionFormatString="Here können Sie alle Daten von dem Auftrag {0} anschauen" CaptionDataField="Ordernumber" InsertCaption="Status ändern" FormMainTableStyle-Width="1000" FormCaptionStyle-Width="500" EditColumn-ItemStyle-Width="500" EditColumn-HeaderText="Status ändern: " EditFormType="Template" PopUpSettings-Modal="true">
                                    <FormTemplate>
                                        <%-- <table id="Table1" cellspacing="1" cellpadding="1" width= "auto" border="0">--%>

                                        <table width="1000">
                                            <tr>

                                                <td>
                                                    <asp:Panel runat="server" ID="FahrzeugPanel">
                                                        <asp:Label Text="Fahrzeug" ID="FahrzeugLabel" runat="server" />
                                                        <br />

                                                        <telerik:RadTextBox Text='<%# Bind( "OrderNumber") %>' Visible="false" ID="OrderIdBox" runat="server">
                                                        </telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "locationId") %>' Visible="false" ID="LocationIdBox" runat="server">
                                                        </telerik:RadTextBox>

                                                        <telerik:RadTextBox ID="VINLabel" runat="server" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black" Enabled="false" Text="FIN: " Visible="True" Width="140">
                                                        </telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "VIN") %>' AutoPostBack="false" ID="VINBox" runat="server">
                                                        </telerik:RadTextBox>

                                                        <telerik:RadTextBox ID="VariantLabel" runat="server" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black" Enabled="false" Text="Variant: " Visible="True" Width="140px">
                                                        </telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "Variant") %>' ID="VariantBox" runat="server">
                                                        </telerik:RadTextBox>

                                                        <telerik:RadTextBox ID="LicenceNumLabel" runat="server" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black" Enabled="false" Text="Kennzeichen: " Visible="True" Width="140px">
                                                        </telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "Kennzeichen") %>' ID="LicenceBox" runat="server">
                                                        </telerik:RadTextBox>
                                                        <br />

                                                        <telerik:RadTextBox ID="PreviousLicNumLabel" runat="server" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black" Enabled="false" Text="Vorherige Kennzeichen: " Visible="True" Width="140px">
                                                        </telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "Prevkennzeichen") %>' ID="PreviousLicenceBox" runat="server">
                                                        </telerik:RadTextBox>

                                                        <telerik:RadTextBox ID="InspectionLabel" runat="server" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black" Enabled="false" Text="Inspektionsdatum: " Visible="True" Width="140px">
                                                        </telerik:RadTextBox>
                                                        <telerik:RadDatePicker SelectedDate='<%# Bind( "Inspection") %>' ID="InspectionDatePicker" runat="server">
                                                        </telerik:RadDatePicker>
                                                        <br />
                                                        <telerik:RadTextBox ID="TSNLabel" runat="server" BorderColor="Transparent"
                                                            DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black"
                                                            Enabled="false" Text="TSN: " Width="140px">
                                                        </telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "TSN") %>' runat="server" ID="TSNBox"></telerik:RadTextBox>
                                                        <asp:RegularExpressionValidator ID="RegExVal" runat="server"
                                                            ControlToValidate="TSNBox"
                                                            ValidationExpression=".{3}.*"
                                                            Display="Static"
                                                            ForeColor="Red"
                                                            ErrorMessage="TSN ist weniger als 3 Zeichen">
                                                        </asp:RegularExpressionValidator>

                                                        <br />
                                                        <telerik:RadTextBox ID="HSNLabel" runat="server" BorderColor="Transparent"
                                                            DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black"
                                                            Text="HSN: " Enabled="false" Width="140px">
                                                        </telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "HSN") %>' AutoPostBack="false" runat="server" ID="HSNBox"></telerik:RadTextBox>
                                                        <br />
                                                        <asp:Label runat="server" Enabled="true" Visible="false" Text="" ID="HSNSearchLabel"></asp:Label>
                                                        <br />

                                                        <telerik:RadTextBox ID="InsuranceLabel" runat="server"
                                                            BorderColor="Transparent" DisabledStyle-BackColor="Transparent"
                                                            DisabledStyle-ForeColor="Black" Enabled="false" Text="Versicherung: " Visible="True"
                                                            Width="140px">
                                                        </telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "eVBNum") %>' runat="server" ID="InsuranceBox"></telerik:RadTextBox>

                                                        <br />
                                                        <telerik:RadTextBox ID="FreiTextLabel" runat="server" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black" Enabled="false" Text="Freitext: " Visible="True" Width="140px">
                                                        </telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "Freitext") %>' ID="Freitext" runat="server">
                                                        </telerik:RadTextBox>
                                                    </asp:Panel>
                                                </td>

                                                <td>
                                                    <asp:Panel runat="server" Visible="true" ID="Halter">
                                                        <asp:Label Text="Halter" ID="HalterLabel" runat="server" />
                                                        <br />
                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Name: " ID="OwnerNameLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "Name") %>' runat="server" ID="OwnerNameBox"></telerik:RadTextBox>

                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Vorname: " ID="OwnerFirstNameLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "FirstName") %>' runat="server" ID="OwnerFirstNameBox"></telerik:RadTextBox>

                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Bankname: " ID="BankNameLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "BankName") %>' runat="server" ID="BankNameBox"></telerik:RadTextBox>

                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Accountnummer: " ID="AccountNumberLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "AccountNum") %>' runat="server" ID="AccountNumberBox"></telerik:RadTextBox>

                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="BLZ: " ID="BankCodeLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "BankCode") %>' runat="server" ID="BankCodeBox"></telerik:RadTextBox>
                                                    </asp:Panel>
                                                </td>



                                                <td>
                                                    <asp:Panel runat="server" Visible="true" ID="Halterdaten">
                                                        <asp:Label Text="Halterdaten" ID="HalterdatenLabel" runat="server" />
                                                        <br />
                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Straße: " ID="OwnerStreetLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "Street") %>' runat="server" ID="OwnerStreetBox"></telerik:RadTextBox>

                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Nummer: " ID="OwnerStreetNubmerLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "StreetNr") %>' runat="server" ID="OwnerStreetNubmerBox"></telerik:RadTextBox>

                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Zip: " ID="OwnerZipCodeLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "Zip") %>' runat="server" ID="OwnerZipCodeBox"></telerik:RadTextBox>

                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Stadt: " ID="OwnerCityLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "City") %>' runat="server" ID="OwnerCityBox"></telerik:RadTextBox>

                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Land: " ID="OwnerCountryLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "Country") %>' runat="server" ID="OwnerCountryBox"></telerik:RadTextBox>
                                                    </asp:Panel>
                                                </td>

                                                <td>
                                                    <asp:Panel Visible="true" runat="server" ID="Kontaktdaten">
                                                        <asp:Label Text="Kontaktdaten" ID="KontaktdatenLabel" runat="server" />
                                                        <br />
                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Telefonnummer: " ID="OwnerPhoneLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "Phone") %>' runat="server" ID="OwnerPhoneBox"></telerik:RadTextBox>

                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Faxnummer: " ID="OwnerFaxLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "Fax") %>' runat="server" ID="OwnerFaxBox"></telerik:RadTextBox>

                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Handynummer: " ID="OwnerMobilePhoneLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "Mobile") %>' runat="server" ID="OwnerMobilePhoneBox"></telerik:RadTextBox>

                                                        <telerik:RadTextBox runat="server" Enabled="false" Visible="True" DisabledStyle-ForeColor="Black" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" Text="Email: " ID="OwnerEmailLabel" Width="140px"></telerik:RadTextBox>
                                                        <telerik:RadTextBox Text='<%# Bind( "Email") %>' runat="server" ID="OwnerEmailBox"></telerik:RadTextBox>
                                                    </asp:Panel>
                                                </td>
                                            </tr>

                                        </table>
                                        <br />
                                        <br />
                                        <%--        <asp:Button ID="StatusButton" Text="Auftrag speichern" runat="server" ><%--OnClick = "OrderUpdateButton_Clicked"
                        </asp:Button>--%>

                                        <asp:Button ID="Button2" Text="Schließen" runat="server" CausesValidation="False" CommandName="Cancel"></asp:Button>


                                    </FormTemplate>
                                </EditFormSettings>


                            </MasterTableView>

                            <ClientSettings>
                                <Scrolling AllowScroll="false"></Scrolling>
                                <Selecting AllowRowSelect="true"></Selecting>
                            </ClientSettings>
                        </telerik:RadGrid>
                    </telerik:RadAjaxPanel>

                </form>
            </body>

            <asp:LinqDataSource ID="LinqDataSourceCustomerName" runat="server" OnSelecting="CustomerName_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext">
            </asp:LinqDataSource>
            <asp:LinqDataSource ID="RadGridSearch_NeedDataSource" runat="server" OnSelecting="RadGridSearch_NeedDataSource_Linq" ContextTypeName="KVSCommon.Database.DataClasses1DataContext">
            </asp:LinqDataSource>

        </ContentTemplate>

    </asp:UpdatePanel>

</asp:Content>
