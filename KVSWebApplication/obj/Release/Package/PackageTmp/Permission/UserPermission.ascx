<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserPermission.ascx.cs" Inherits="KVSWebApplication.Permission.UserPermission" %>
   <link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />
<div runat="server" class="uebersichtDiv3" id="allPermissionDiv">
          <telerik:RadWindowManager ID="RadWindowManagerAllUserPermission" runat="server" EnableShadow="true">
          </telerik:RadWindowManager>
              <telerik:RadAjaxPanel ID="RadAjaxPanelAllUserPermission" runat="server" Width="900px" LoadingPanelID="RadAjaxLoadingPanelUserPermission">
        <telerik:RadGrid id="getAllUserPermission" runat="server" Culture="de-De" Width="740px"  AllowAutomaticUpdates="true" 
            AllowAutomaticInserts="true" DataSourceID="getAllUserPermissionDataSource" AllowAutomaticDeletes="false" AutoGenerateColumns="false"  
            AllowPaging="true"   EnableViewState ="false" >           
 <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"></PagerStyle>
       <MasterTableView DataKeyNames="Id"  CommandItemDisplay="Top" EditMode="PopUp"  AllowFilteringByColumn="true" >
           <CommandItemSettings ShowAddNewRecordButton="false"   />
            <Columns>          
            <telerik:GridBoundColumn DataField="Id" HeaderText="Id" ReadOnly="true"  Visible="false" ForceExtractValue="Always"  />
                <telerik:GridBoundColumn DataField="Name" HeaderText="Nachname"   
            CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%"   AllowSorting="true"  />
            <telerik:GridBoundColumn DataField="FirstName" HeaderText="Vorname" 
            CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%"  AllowSorting="true" />
            <telerik:GridBoundColumn DataField="Email" HeaderText="Email"  
                CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%"  ItemStyle-Width="90px"  AllowSorting="true" />
            <telerik:GridBoundColumn DataField="Login" HeaderText="Benutzername"  
            CurrentFilterFunction="Contains" ShowFilterIcon="false" 
                AutoPostBackOnFilter="true" FilterControlWidth="95%" ItemStyle-Width="70px"  AllowSorting="true" />
                                <telerik:GridTemplateColumn HeaderText="Rechte"
                                    HeaderStyle-Width="65px" ShowFilterIcon="false" AllowFiltering="false" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center" >
                <ItemTemplate>
                <asp:Label ID="lblIdPermisssion" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>' Visible="false"  runat="server"  >
                            </asp:Label>
                <asp:Label ID="lblUserPermissionShow" runat="server" Text='Zuweisen/Anzeigen' >
                            </asp:Label>  
                                    <telerik:RadToolTip ManualClose="true" 
                    ManualCloseButtonText="Schließen"   ID="ttUserPermission" runat="server"   
                                    TargetControlID="lblUserPermissionShow" Animation="Slide"
                                    RelativeTo="Element"  Position="BottomLeft" Width="180px">
                                <div style="width:600px;  white-space:normal;" >
                            <asp:Panel ID="PermissionsPanel" runat="server">
                                <telerik:RadListBox runat="server" ID="lbsUserPermissions" CssClass="InsuranceListBox" SelectionMode="Multiple" TabIndex="1" DataSourceID="AllPermissionDataSource"
                            AllowReorder="true" AccessKey="y" Height="230px" AllowDelete="true" Width="250px"  TransferMode="Move" DataKeyField="Description"  OnItemDataBound="lbsUserPermissions_ItemDataBound"
                            AllowTransfer="true" TransferToID="AddedUserPermission"  EnableDragAndDrop="true" DataValueField="Id" DataTextField="Name">
                            <ButtonSettings ShowDelete="false"></ButtonSettings>
                            <HeaderTemplate>
                            <asp:Label ID="HeaderCustomerInsurance" runat="server" Text="Vorhandene Rechte:"></asp:Label>
                            </HeaderTemplate>
                        </telerik:RadListBox>
                        <telerik:RadListBox runat="server" ID="AddedUserPermission"  EnableDragAndDrop="true" Height="230px" TabIndex="2" Width="250px" DataValueField="Id" DataTextField="Name"   DataKeyField="Description" 
                            SelectionMode="Multiple" DataSourceID="UserPermissionDataSource"  OnItemDataBound="AddedUserPermission_ItemDataBound"> 
                            <HeaderTemplate>
                                <asp:Label ID="HeaderCustomerInsurance"  runat="server" Text="Zugewiesene Rechte:"></asp:Label>
                            </HeaderTemplate>
                        </telerik:RadListBox>
                                    <telerik:RadButton ID="rbtSavePermissions" runat="server" CssClass="CustomerInsuranceSaveButton" OnClick="rbtSavePermissions_Click"   CommandArgument='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>'
                        Text="Speichern"></telerik:RadButton>                  
                                    </asp:Panel>    
                                        </div>
                            <asp:LinqDataSource runat=server ID="AllPermissionDataSource" OnSelecting="GetAllPermissionDataSource_Selecting">
                                <WhereParameters>
                    <asp:ControlParameter ControlID="lblIdPermisssion" Name="Id" />
                    </WhereParameters>                              
                            </asp:LinqDataSource>                     
                <asp:LinqDataSource runat=server ID="UserPermissionDataSource" OnSelecting="UserPermissionDataSource_Selecting">
                    <WhereParameters>
                    <asp:ControlParameter ControlID="lblIdPermisssion" Name="Id" />
                    </WhereParameters>
                </asp:LinqDataSource>
                </telerik:RadToolTip>  
                </ItemTemplate>
            </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Rechteprofile" HeaderButtonType="TextButton"
                                        HeaderStyle-Width="65px" ShowFilterIcon="false" AllowFiltering="false" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center" >                            
                    <ItemTemplate>
                    <asp:Label ID="lblIdPermisssionProfile" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>' Visible="false"    runat="server"  >
                            </asp:Label>
                <asp:Label ID="lblUserPermissionProfileShow" runat="server" Text='Zuweisen/Anzeigen' >
                            </asp:Label>  
                                    <telerik:RadToolTip ManualClose="true" 
                    ManualCloseButtonText="Schließen"   ID="ttUserPermissionProfile" runat="server"   
                                    TargetControlID="lblUserPermissionProfileShow" Animation="Slide"
                                    RelativeTo="Element"  Position="BottomLeft" Width="180px">
                                <div style="width:600px;  white-space:normal;" >   
                                        <asp:Panel ID="PermissionProfilePanel" runat="server" >
                                    <telerik:RadListBox runat="server" ID="lsbAllPermissionProfile" CssClass="InsuranceListBox" SelectionMode="Multiple" TabIndex="1"
                        AllowReorder="true" AccessKey="y" Height="230px" AllowDelete="true" Width="250px"  TransferMode="Move" DataKeyField="Description" OnItemDataBound="lbsUserPermissionsProfile_ItemDataBound"
                        AllowTransfer="true" TransferToID="lsbPermissionProfilePermission"  EnableDragAndDrop="true" DataValueField="Id" DataTextField="Name" DataSourceID="AllPermissionProfileDataSource">
                        <ButtonSettings ShowDelete="false"></ButtonSettings>
                        <HeaderTemplate>
                        <asp:Label ID="HeaderAllPermissionProfileInsurance" runat="server" Text="Vorhandene Rechteprofile:"></asp:Label>
                        </HeaderTemplate>
                    </telerik:RadListBox>
                    <telerik:RadListBox runat="server" ID="lsbPermissionProfilePermission"  EnableDragAndDrop="true" Height="230px" TabIndex="2" Width="250px" DataValueField="Id" DataTextField="Name"   DataKeyField="Description" 
                        SelectionMode="Multiple" DataSourceID="UserPermissionProfileDataSource"  OnItemDataBound="AddedUserPermissionsProfile_ItemDataBound"> 
                        <HeaderTemplate>
                            <asp:Label ID="HeaderUserPermissionProfileInsurance"  runat="server" Text="Zugewiesene Rechteprofile:"></asp:Label>
                        </HeaderTemplate>
                    </telerik:RadListBox>
                                <telerik:RadButton ID="rbtSavePermissionProfile" runat="server" CssClass="CustomerInsuranceSaveButton" OnClick="rbtSavePermissionProfile_Click" CommandArgument='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>'
                    Text="Speichern"></telerik:RadButton>
                            </asp:Panel>
                                    </div>                              
                            <asp:LinqDataSource runat=server ID="AllPermissionProfileDataSource" OnSelecting="GetAllPermissionProfileDataSource_Selecting">
                            <WhereParameters>
                <asp:ControlParameter ControlID="lblIdPermisssionProfile" Name="Id" />
                </WhereParameters>                              
                        </asp:LinqDataSource>                     
            <asp:LinqDataSource runat=server ID="UserPermissionProfileDataSource" OnSelecting="UserPermissionProfileDataSource_Selecting">
                <WhereParameters>
                <asp:ControlParameter ControlID="lblIdPermisssionProfile" Name="Id" />
                </WhereParameters>
            </asp:LinqDataSource>
                </telerik:RadToolTip>
            </ItemTemplate>
        </telerik:GridTemplateColumn>
        </Columns>
                <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
    <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
    <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />
        <EditFormSettings EditFormType="AutoGenerated"   >
            <EditColumn ButtonType="ImageButton" />
            <PopUpSettings Modal="true"   />
            </EditFormSettings>             
    </MasterTableView>               
    <PagerStyle AlwaysVisible="true" />
</telerik:RadGrid>
</telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelUserPermission" BackgroundTransparency="100" runat="server">
</telerik:RadAjaxLoadingPanel>
<asp:LinqDataSource runat=server ID="getAllUserPermissionDataSource" OnSelecting="getAllUserPermissionDataSource_Selecting" ></asp:LinqDataSource>
</div>