<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AllProducts.ascx.cs" Inherits="KVSWebApplication.Product.AllProducts" %>
<link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />
<div runat="server" class="uebersichtDiv" id="myUebersicht">
    <style>
        .rgEditForm {
            height: 300px !important;
        }
    </style>
    <script type="text/javascript">
        function onPopUpShowing(sender, args) {
            args.get_popUp().className += " popUpEditForm";
        }
    </script>
    <telerik:RadAjaxPanel ID="RadAjaxPanelAllProducts" runat="server" LoadingPanelID="RadAjaxLoadingPanelAllProducst">
        <telerik:RadWindowManager ID="RadWindowManagerAllProducts" runat="server" EnableShadow="true">
        </telerik:RadWindowManager>
        <telerik:RadGrid ID="getAllProducts" runat="server" DataSourceID="GetAllProductsDataSource" Culture="de-De"
            AllowAutomaticUpdates="false"
            AllowAutomaticInserts="true" AllowAutomaticDeletes="false" AutoGenerateColumns="false" OnInit="getAllProducts_Init" OnUpdateCommand="Grid_UpdateCommand" OnItemCommand="getAllProducts_ItemCommand"
            OnItemDataBound="getAllProducts_ItemDataBound" PagerStyle-AlwaysVisible="true"
            AllowPaging="true" AllowFilteringByColumn="True"
            AllowMultiRowSelection="false">
            <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"></PagerStyle>
            <MasterTableView CommandItemDisplay="Top" AllowCustomSorting="true" ShowHeader="true" EditMode="PopUp" AutoGenerateColumns="false" AllowFilteringByColumn="True" ShowFooter="True" TableLayout="Auto">
                <CommandItemTemplate>
                    <asp:ImageButton ID="add" runat="server" ToolTip="Neues Produkt hinzufügen" ImageUrl="~/Pictures/AddRecord.gif" Height="20px" Width="20px" CommandName="InsertItem"></asp:ImageButton>
                    <asp:Label ID="Label5" runat="server" Text="Neues Produkt hinzufügen"></asp:Label>
                </CommandItemTemplate>
                <CommandItemSettings ShowAddNewRecordButton="true" ShowRefreshButton="true" />
                <Columns>
                    <telerik:GridEditCommandColumn ButtonType="ImageButton" />


                    <telerik:GridBoundColumn DataField="Id" ReadOnly="true" UniqueName="Id"
                        Display="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="PriceId" ReadOnly="true" UniqueName="PriceId" Display="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="OrderTypeId" ReadOnly="true" UniqueName="OrderTypeId" Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="ProductCategoryId" UniqueName="ProductCategoryId" ReadOnly="true" Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="RegistrationOrderTypeId" ReadOnly="true" UniqueName="RegistrationOrderTypeId" Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="ProductName" HeaderText="Produktname" UniqueName="ProductName"
                        CurrentFilterFunction="Contains" ShowFilterIcon="false" AutoPostBackOnFilter="true" HeaderButtonType="TextButton" />
                    <telerik:GridBoundColumn DataField="ItemNumber" HeaderText="Produktnummer" UniqueName="ItemNumber"
                        CurrentFilterFunction="Contains" FilterControlWidth="95%" ShowFilterIcon="false" AutoPostBackOnFilter="true" HeaderButtonType="TextButton" />
                    <telerik:GridBoundColumn DataField="ProductCategorieName" HeaderText="Kategorie"
                        UniqueName="ProductCategorieName"
                        CurrentFilterFunction="Contains" ShowFilterIcon="false" AutoPostBackOnFilter="true" HeaderButtonType="TextButton" />
                    <telerik:GridBoundColumn DataField="OrderTypeName" HeaderText="Auftragstyp"
                        UniqueName="OrderTypeName" FilterControlWidth="95%" CurrentFilterFunction="Contains"
                        ShowFilterIcon="false" AutoPostBackOnFilter="true" HeaderButtonType="TextButton" />
                    <telerik:GridBoundColumn DataField="RegistrationOrderTypeName" HeaderText="Zulassungstyp" UniqueName="RegistrationOrderTypeName"
                        CurrentFilterFunction="Contains" ShowFilterIcon="false" AutoPostBackOnFilter="true" HeaderButtonType="TextButton" />
                    <telerik:GridBoundColumn DataField="AccountNumber" FilterControlWidth="95%" HeaderText="Erlöskonten"
                        CurrentFilterFunction="Contains" UniqueName="AccountNumber" HeaderButtonType="TextButton"
                        ShowFilterIcon="false" AutoPostBackOnFilter="true" />
                    <telerik:GridBoundColumn DataField="Amount" HeaderText="Preis" HeaderButtonType="TextButton"
                        FilterControlWidth="95%" ShowFilterIcon="false" AllowFiltering="false" UniqueName="Amount"
                        AutoPostBackOnFilter="true" DataFormatString="{0:c}" DataType="System.String"
                        ItemStyle-HorizontalAlign="Center" />
                    <telerik:GridBoundColumn DataField="AutoCharge" HeaderText="BehördlicheGebühr"
                        HeaderStyle-Width="140px" UniqueName="AutoCharge" HeaderButtonType="TextButton"
                        DataType="System.String" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:c}"
                        ShowFilterIcon="false" AutoPostBackOnFilter="true" AllowFiltering="false" />
                    <telerik:GridCheckBoxColumn DataField="Vat" ReadOnly="true" HeaderText="MwSt" UniqueName="Vat" HeaderButtonType="TextButton"
                        ShowFilterIcon="false" AutoPostBackOnFilter="true"
                        AllowFiltering="false">
                    </telerik:GridCheckBoxColumn>
                    <telerik:GridTemplateColumn HeaderText="Löschen" HeaderButtonType="TextButton" AllowFiltering="false"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                        AutoPostBackOnFilter="false">
                        <ItemTemplate>
                            <asp:Label ID="lblProductId" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>' Visible="false" runat="server"></asp:Label>
                            <asp:Label ID="lblPriceId" Text='<%#  DataBinder.Eval(Container, "DataItem.PriceId") %>' Visible="false" runat="server"></asp:Label>
                            <telerik:RadButton ID="btnRemoveProduct" runat="server" Text="Löschen" ToolTip="Produkt löschen" OnClick="RemoveProduct_Click">
                                <Icon PrimaryIconCssClass="rbRemove" PrimaryIconLeft="4" PrimaryIconTop="3"></Icon>
                            </telerik:RadButton>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
                <EditFormSettings EditFormType="Template" PopUpSettings-Modal="true" CaptionFormatString="Produktinformationen bearbeiten">
                    <FormTemplate>
                        <asp:Label ID="SchowErrorMessages" runat="server" BackColor="Red"></asp:Label>
                        <table id="tableEditAllProducts" cellspacing="1" cellpadding="1" width="450" style="height: 90% !important;" border="0">
                            <tr>
                                <td></td>
                                <td></td>
                            </tr>
                            <asp:TextBox ID="ProductId" Visible="false" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>'>
                            </asp:TextBox>
                            <tr>
                                <td>Produktname:
                                </td>
                                <td>
                                    <asp:TextBox ID="ProductNameBox" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.ProductName").ToString() %>'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Produktnummer:
                                </td>
                                <td>
                                    <asp:TextBox ID="ProductNumberBox" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.ItemNumber").ToString() %>'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Kategorie:
                                </td>
                                <td>
                                    <telerik:RadComboBox ID="cmbCategory" runat="server" OnInit="cmbCategory_OnLoad" Height="300px" AllowCustomText="True"
                                        Text='<%#  DataBinder.Eval(Container, "DataItem.ProductCategorieName").ToString() %>'>
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Auftragstyp:
                                </td>
                                <td>
                                    <telerik:RadComboBox ID="cmbOrderTypeName" runat="server" OnInit="cmbOrderTypeName_OnLoad" AllowCustomText="True"
                                        Text='<%#  DataBinder.Eval(Container, "DataItem.OrderTypeName").ToString() %>' AutoPostBack="true"
                                        OnSelectedIndexChanged="cmbOrderTypeName_SelectedIndexChanged">
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Zulassungstyp:
                                </td>
                                <td>
                                    <telerik:RadComboBox ID="cmbRegistrationOrderTypeName" OnInit="cmbRegistrationOrderTypeName_OnLoad"
                                        Enabled='<%#  DataBinder.Eval(Container, "DataItem.EnableDropDown").Equals("true") ? true : false %>' ToolTip="Gilt nur in Verbindung mit dem Auftragstyp: Zulassung!"
                                        runat="server" AllowCustomText="True"
                                        Text='<%#  DataBinder.Eval(Container, "DataItem.RegistrationOrderTypeName").ToString() %>'>
                                    </telerik:RadComboBox>
                                </td>
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
                                <td>Preis:
                                </td>
                                <td>
                                    <asp:TextBox ID="txbAmount" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.Amount").ToString() %>'>
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>Behördliche Gebühr:
                                </td>
                                <td>
                                    <asp:TextBox ID="txbAuthorativeCharge" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.AutoCharge").ToString() %>'> 
                                    </asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>MwSt pflichtig:
                                </td>
                                <td>
                                    <asp:CheckBox ID="chbVAT"
                                        runat="server"
                                        Checked='<%#  DataBinder.Eval(Container, "DataItem.Vat").Equals("true") ? true : false %>'></asp:CheckBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lbKundenProd" Text="Kunden:"></asp:Label>
                                </td>
                                <td>
                                    <telerik:RadComboBox runat="server" Filter="Contains" DataTextField="CustomerName" DropDownAutoWidth="Enabled" DataValueField="CustomerId" ID="cmbCustomerProducts" DataCheckedField="IsChecked" CheckBoxes="true" EnableCheckAllItemsCheckBox="true" EmptyMessage="Wählen Sie Kunden aus"></telerik:RadComboBox>
                                    <asp:HiddenField runat="server" ID="productIdHiddenField" />
                                </td>
                            </tr>
                            </ br>  
                        <tr>
                            <td align="center" colspan="2">
                                <asp:Button ID="btnSave" Text="Ok" runat="server" OnClick="btnSaveProduct_Click"></asp:Button>&nbsp;
                                <asp:Button ID="btnAbort" Text="Abbrechen" runat="server" CausesValidation="False" OnClick="btnAbort_Click"></asp:Button>
                            </td>
                        </tr>
                            </ br>
                        </table>
                    </FormTemplate>
                </EditFormSettings>
                <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
                <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" Width="120px" Height="45px" />
                <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />
            </MasterTableView>
            <ClientSettings>
                <ClientEvents OnPopUpShowing="onPopUpShowing" />
            </ClientSettings>
            <PagerStyle AlwaysVisible="true" />
        </telerik:RadGrid>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelAllProducst" BackgroundTransparency="100" runat="server">
    </telerik:RadAjaxLoadingPanel>
    <asp:LinqDataSource runat="server" ID="GetAllProductsDataSource" OnSelecting="GetAllProductsDataSource_Selecting" ContextTypeName="KVSCommon.Database.DataClasses1DataContext"></asp:LinqDataSource>
</div>
