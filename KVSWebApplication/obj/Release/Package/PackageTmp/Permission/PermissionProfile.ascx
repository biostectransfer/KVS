<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PermissionProfile.ascx.cs"
    Inherits="KVSWebApplication.Permission.PermissionProfile" %>
<link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />
<div runat="server" class="uebersichtDiv3" id="allPermissionDiv">
    <telerik:RadWindowManager ID="RadWindowManagerAllPermissionProfile" runat="server"
        EnableShadow="true">
    </telerik:RadWindowManager>
    <telerik:RadAjaxPanel ID="RadAjaxPanelAllPermission" runat="server" Width="900px"
        LoadingPanelID="RadAjaxLoadingPanelPermissionProfile">
        <telerik:RadGrid ID="getAllPermissionProfile" runat="server" DataSourceID="getAllPermissionpRrofileDataSource"
            Culture="de-De" AllowAutomaticUpdates="true" OnInit="getAllPermissionProfile_Init"
            AllowAutomaticInserts="true" OnItemCommand = "getAllPermissionProfile_ItemCommand" AllowAutomaticDeletes="false" AutoGenerateColumns="false"
            OnUpdateCommand="getAllPermissionProfile_EditCommand" OnInsertCommand="Grid_InsertCommand"
            AllowPaging="true" EnableViewState="false">
            <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"></PagerStyle>
            <MasterTableView DataKeyNames="Id" CommandItemDisplay="Top" EditMode="PopUp" AllowFilteringByColumn="true">
                <CommandItemSettings ShowAddNewRecordButton="true" />
                <Columns>
                    <telerik:GridEditCommandColumn ButtonType="ImageButton" />
                    <telerik:GridBoundColumn DataField="Id" HeaderText="Id" ReadOnly="true" Visible="false"
                        ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Name" Visible="true" ForceExtractValue="None"
                        CurrentFilterFunction="Contains" ShowFilterIcon="false" FilterControlWidth="95%"
                        AllowSorting="true" AutoPostBackOnFilter="true" />
                    <telerik:GridBoundColumn DataField="Description" HeaderText="Beschreibung" CurrentFilterFunction="Contains"
                        ShowFilterIcon="false" AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" />
                    <telerik:GridTemplateColumn HeaderText="Rechtepackete" HeaderButtonType="TextButton"
                        HeaderStyle-Width="120px" AllowFiltering="false" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                        ShowFilterIcon="false" FilterControlWidth="95%">
                        <ItemTemplate>
                            <asp:Panel ID="myPermissionPanel" runat="server">
                                <asp:Label ID="lblShow" runat="server" Text='Zuweisen/Anzeigen'>
                                </asp:Label>
                                <asp:Label ID="lblId" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>'
                                    Visible="false" runat="server">
                                </asp:Label>
                                <telerik:RadToolTip ManualClose="true" ManualCloseButtonText="Schließen" ID="ttPermission"
                                    runat="server" TargetControlID="lblShow" Animation="Slide" RelativeTo="Element"
                                    Position="BottomLeft">
                                    <div style="width: 600px; white-space: normal;">
                                        <telerik:RadListBox runat="server" ID="Permissions" CssClass="InsuranceListBox" SelectionMode="Multiple"
                                            TabIndex="1" AllowReorder="true" AccessKey="y" Height="230px" AllowDelete="true"
                                            Width="250px" TransferMode="Move" DataKeyField="Description" OnItemDataBound="PermissionsListBox_ItemDataBound"
                                            AllowTransfer="true" TransferToID="AddedPermission" EnableDragAndDrop="true"
                                            DataSourceID="PermissionsListBox" DataValueField="Id" DataTextField="Name">
                                            <ButtonSettings ShowDelete="false"></ButtonSettings>
                                            <HeaderTemplate>
                                                <asp:Label ID="HeaderCustomerInsurance" runat="server" Text="Vorhandene Rechte:"></asp:Label>
                                            </HeaderTemplate>
                                        </telerik:RadListBox>
                                        <telerik:RadListBox runat="server" ID="AddedPermission" EnableDragAndDrop="true"
                                            Height="230px" TabIndex="2" Width="250px" DataValueField="Id" DataTextField="Name"
                                            DataKeyField="Description" SelectionMode="Multiple" DataSourceID="AddedPermissionListbox"
                                            OnItemDataBound="PermissionsAddedListBox_ItemDataBound">
                                            <HeaderTemplate>
                                                <asp:Label ID="HeaderCustomerInsurance" runat="server" Text="Zugewiesene Rechte:"></asp:Label>
                                            </HeaderTemplate>
                                        </telerik:RadListBox>
                                        <telerik:RadButton ID="bSavePermissionPackage" runat="server" CssClass="CustomerInsuranceSaveButton"
                                            OnClick="savePermissionPackageClick" CommandArgument='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>'
                                            Text="Speichern">
                                        </telerik:RadButton>
                                    </div>
                                    </div>
                                </telerik:RadToolTip>
                                <asp:LinqDataSource runat="server" ID="AddedPermissionListbox" OnSelecting="AddedPermissionListboxDataSource_Selecting">
                                    <WhereParameters>
                                        <asp:ControlParameter ControlID="lblId" Name="Id" />
                                    </WhereParameters>
                                </asp:LinqDataSource>
                                <asp:LinqDataSource runat="server" ID="PermissionsListBox" OnSelecting="PermissionsListBoxDataSource_Selecting">
                                    <WhereParameters>
                                        <asp:ControlParameter ControlID="lblId" Name="Id" />
                                    </WhereParameters>
                                </asp:LinqDataSource>
                            </asp:Panel>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridButtonColumn Text="Delete" CommandName="Delete" ButtonType="ImageButton" />
                </Columns>
                <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
                <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
                <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />
                <EditFormSettings EditFormType="AutoGenerated">
                    <EditColumn ButtonType="ImageButton" />
                    <PopUpSettings Modal="true" />
                </EditFormSettings>
            </MasterTableView>
            <PagerStyle AlwaysVisible="true" />
        </telerik:RadGrid>
    </telerik:RadAjaxPanel>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelPermissionProfile" BackgroundTransparency="100"
        runat="server">
    </telerik:RadAjaxLoadingPanel>
    <asp:LinqDataSource runat="server" ID="getAllPermissionpRrofileDataSource" OnSelecting="getAllPermissionProfileDataSource_Selecting">
    </asp:LinqDataSource>
</div>
