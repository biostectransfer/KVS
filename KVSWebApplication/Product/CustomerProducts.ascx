<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomerProducts.ascx.cs" Inherits="KVSWebApplication.Product.CustomerProducts" %>
</style>
   <link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />
<div runat="server" class="uebersichtDiv" id="CustomerPriceDiv">
    <telerik:RadAjaxPanel ID="RadAjaxPanelCustomerPrice" runat="server"
        LoadingPanelID="RadAjaxLoadingPanelCustomerPrice">
        <telerik:RadWindowManager ID="RadWindowManagerCustomerPrice" runat="server" EnableShadow="true">
        </telerik:RadWindowManager>
        <telerik:RadFormDecorator runat="server" ID="CustomerPrice" DecoratedControls="All" />
        <div id="CustomerProductHeader" class="CustomerPriceListHeader" runat="server">
            <table>
                <tr>
                    <td>
                        <asp:Label ID="textCustomerCMB" runat="server" CssClass="CustomerText" Text="Kunde:" AssociatedControlID="AllCustomer"></asp:Label>
                        <telerik:RadComboBox Height="300px" Width="250px" Enabled="true"
                            Filter="Contains" runat="server" HighlightTemplatedItems="true" AutoPostBack="true"
                            DropDownWidth="515px" EmptyMessage="Bitte wählen Sie einen Kunden aus: "
                            DataTextField="Name" DataValueField="Value" ID="AllCustomer"
                            OnSelectedIndexChanged="CustomerCombobox_SelectedIndexChanged" OnInit="CustomerCombobox_Init">
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
                    <td>
                        <asp:Label ID="lblTextLocation" runat="server" CssClass="LocationText" Text="Standort:" AssociatedControlID="AllCustomer"></asp:Label>
                        <telerik:RadComboBox CssClass="LocationText" runat="server" ID="cmbLocations"></telerik:RadComboBox>
                    </td>
                    <td>
                        <telerik:RadButton ID="bSchow" runat="server" Text="Anzeigen"
                            CssClass="CustomerPriceShowButton" OnClick="bSchow_Click">
                        </telerik:RadButton>
                    </td>
                </tr>
            </table>
        </div>
        <div style="height: 5px">
        </div>
        <telerik:RadGrid ID="getCustomerPrice" runat="server" DataSourceID="GetCustomerProductsDataSource" Culture="de-De" AllowAutomaticUpdates="false"
            AllowAutomaticInserts="true" AllowAutomaticDeletes="false" AutoGenerateColumns="false" Visible="true"
            Enabled="false" OnItemCommand="getAllCustomerProducts_ItemCommand" OnInit="getCustomerPrice_Init"
            AllowPaging="true" OnItemDataBound="getAllCustomerProducts_ItemDataBound">
            <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"></PagerStyle>
            <MasterTableView CommandItemDisplay="Top" EditMode="PopUp" AllowFilteringByColumn="true">
                <CommandItemTemplate>
                    <asp:ImageButton ID="add" runat="server" ToolTip="Neuen Preis hinzufügen" ImageUrl="~/Pictures/AddRecord.gif" Height="20px" Width="20px" CommandName="InsertItem"></asp:ImageButton>
                    <asp:Label ID="Label5" runat="server" Text="Neues Produkt hinzufügen"></asp:Label>
                </CommandItemTemplate>
                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true" />
                <Columns>
                    <telerik:GridEditCommandColumn ButtonType="ImageButton" />
                    <telerik:GridBoundColumn DataField="Id" ReadOnly="true" Visible="false" ForceExtractValue="InEditMode" />
                    <telerik:GridBoundColumn DataField="PriceId" HeaderText="Id" ReadOnly="true" Display="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="LocationIdCustomer" ReadOnly="true" Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="OrderTypeId" ReadOnly="true" Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="ProductCategoryId" ReadOnly="true" Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="RegistrationOrderTypeId" ReadOnly="true" Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="ProductName" HeaderText="Produktname"
                        CurrentFilterFunction="Contains" ShowFilterIcon="false" AutoPostBackOnFilter="true" />
                    <telerik:GridBoundColumn DataField="ItemNumber" HeaderText="Produktnummer"
                        CurrentFilterFunction="Contains" FilterControlWidth="95%" ShowFilterIcon="false" AutoPostBackOnFilter="true" />
                    <telerik:GridBoundColumn DataField="ProductCategorieName" HeaderText="Kategorie" ShowFilterIcon="false" AutoPostBackOnFilter="true"
                        CurrentFilterFunction="Contains" />
                    <telerik:GridBoundColumn DataField="OrderTypeName" HeaderText="Auftragstyp" ShowFilterIcon="false" AutoPostBackOnFilter="true"
                        CurrentFilterFunction="Contains" FilterControlWidth="95%" />
                    <telerik:GridBoundColumn DataField="RegistrationOrderTypeName" FilterControlWidth="95%"
                        HeaderText="Zulassungstyp" ShowFilterIcon="false" AutoPostBackOnFilter="true"
                        CurrentFilterFunction="Contains" />
                    <telerik:GridBoundColumn DataField="AccountNumber" HeaderText="Erlöskonten"
                        ShowFilterIcon="false" AutoPostBackOnFilter="true"
                        CurrentFilterFunction="Contains" FilterControlWidth="95%" />
                    <telerik:GridBoundColumn DataField="Amount" HeaderText="Preis" CurrentFilterFunction="Contains"
                        FilterControlWidth="95%" DataType="System.Decimal" ShowFilterIcon="false" AutoPostBackOnFilter="true"
                        ItemStyle-HorizontalAlign="Center" />
                    <telerik:GridBoundColumn DataField="AutoCharge" HeaderText="Behördliche Gebühr" CurrentFilterFunction="Contains"
                        DataType="System.Decimal" ItemStyle-HorizontalAlign="Center" ShowFilterIcon="false" AutoPostBackOnFilter="true" />
                    <telerik:GridTemplateColumn HeaderText="Löschen" HeaderButtonType="TextButton" AllowFiltering="false"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                        AutoPostBackOnFilter="false">
                        <ItemTemplate>
                            <asp:Label ID="lblPriceId" Text='<%#  DataBinder.Eval(Container, "DataItem.PriceId") %>' Visible="false" runat="server"></asp:Label>
                            <telerik:RadButton ID="btnRemovePrice" runat="server" Text="Löschen" ToolTip="Preis löschen" OnClick="RemovePrice_Click">
                                <Icon PrimaryIconCssClass="rbRemove" PrimaryIconLeft="4" PrimaryIconTop="3"></Icon>
                            </telerik:RadButton>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
                <EditFormSettings EditFormType="Template" PopUpSettings-Modal="true" InsertCaption="Kundenprodukt bearbeiten">
                    <FormTemplate>
                        <asp:Label ID="SchowErrorMessages" runat="server" BackColor="Red"></asp:Label>
                        <table id="tableEditCustomerProducts" cellspacing="1" cellpadding="1" width="450" border="0">
                            <tr>
                                <td></td>
                                <td></td>
                            </tr>
                            <asp:TextBox ID="ProductIdCustomer" Visible="false" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>'>
                            </asp:TextBox>
                            <asp:TextBox ID="LocationIdCustomer" Visible="false" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.LocationIdCustomer").ToString() %>'>
                            </asp:TextBox>
                            <asp:TextBox ID="txbPriceId" Visible="false" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.PriceId") %>'>
                            </asp:TextBox>
                            <tr>
                                <td>Produktname:
                                </td>
                                <td>
                                    <asp:TextBox ID="ProductNameBoxCustomer" ReadOnly="true" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.ProductName").ToString() %>'>
                                    </asp:TextBox>
                                    <telerik:RadComboBox Height="300px" Enabled="true"
                                        OnInit="cmdProductNames_OnInit" AutoPostBack="false" Filter="Contains" runat="server" DropDownWidth="515px" EmptyMessage="Produkt..." HighlightTemplatedItems="true"
                                        DataTextField="Name" DataValueField="Value" ID="cmdProductNames">
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
                                </td>
                            </tr>
                            <tr>
                                <td>Produktnummer:
                                </td>
                                <td>
                                    <asp:TextBox ID="ProductNumberBoxCustomer" ReadOnly="true" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.ItemNumber").ToString() %>'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <td>Preis:
                            </td>
                            <td>
                                <asp:TextBox ID="txbAmountCustomer" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.Amount").ToString() %>'>
                                </asp:TextBox>
                            </td>
                            </tr>
                            <tr>
                                <td>Behördliche Gebühr:
                                </td>
                                <td>
                                    <asp:TextBox ID="txbAuthorativeChargeCustomer" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.AutoCharge").ToString() %>'> 
                                    </asp:TextBox>
                                </td>
                            </tr>
                            </tr>
                            <tr>
                                <td>Erlöskonto:
                                </td>
                                <td>
                                    <telerik:RadComboBox ID="cmbErloeskonten" runat="server"
                                        AllowCustomText="false" Text="" DataValueField="AccountId" OnInit="cmbErloeskonten_OnInit"
                                        DataTextField="AccountNumber">
                                    </telerik:RadComboBox>
                                </td>
                            </tr>


                            <tr>
                                <td align="center" colspan="2"></br>
                                <asp:Button ID="btnSaveCustomerProduct" Text="Ok" runat="server" OnClick="btnSaveProduct_Click"></asp:Button>&nbsp;
                                <asp:Button ID="btnAbortCustomerProduct" Text="Abbrechen" runat="server" CausesValidation="False" OnClick="btnAbortProduct_Click"></asp:Button>
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
    <asp:LinqDataSource runat="server" ID="GetCustomerProductsDataSource"
        OnSelecting="GetCustomerProductsDataSource_Selecting">
    </asp:LinqDataSource>
</div>
<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelCustomerPrice" BackgroundTransparency="100" runat="server">
</telerik:RadAjaxLoadingPanel>
