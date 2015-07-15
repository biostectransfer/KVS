<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="KVSWebApplication.Permission.test" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
     <asp:ScriptManager ID="Script" runat="server">

    </asp:ScriptManager>
    <link href="../Styles/styles.css" rel="stylesheet" type="text/css" />
 <link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />

 
<telerik:RadAjaxManager runat="server" ID="RadAjaxManagerCreateUser">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="rbtShowUser">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="selectPermission" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
               <telerik:AjaxSetting AjaxControlID="rbtcreateUser">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="selectPermission" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
<telerik:RadFormDecorator ID="QsfFromDecoratorCreateUser" runat="server" DecoratedControls="All"  />
        <telerik:RadAjaxPanel ID="RadAjaxPanelCreateUser" runat="server" Width="970px" LoadingPanelID="RadAjaxLoadingPanelCreateUser">





                        <asp:Label ID="lblUserPermissionShow" runat="server" Text='Zuweisen/Anzeigen' >
                                    </asp:Label>
                                 
                                  

                                         <telerik:RadToolTip ManualClose="true" 
                            ManualCloseButtonText="Schließen"   ID="ttUserPermission" runat="server"   
                                          TargetControlID="lblUserPermissionShow" Animation="Slide"
                                          RelativeTo="Element"  Position="BottomLeft" Width="180px">


                                      <div style="width:600px;  white-space:normal;" >

                                        <asp:UpdatePanel ID="selectPermissionUpdatePanel" runat="server" UpdateMode="Conditional">
              <Triggers>
             
              <asp:AsyncPostBackTrigger ControlID="RadTabStripUserPermission" />
              </Triggers>
              <ContentTemplate>
        
                  
      <%--           <telerik:RadButton ID="btnToggle" ToggleType="CheckBox" runat="server" ButtonType="ToggleButton"  AutoPostBack="true" OnClick="ShowUserTab" OnCheckedChanged="ShowUserTab" CssClass="userpermissionCheckBox">
 <ToggleStates>
   <telerik:RadButtonToggleState Text="Rechte"  PrimaryIconCssClass="rbToggleCheckboxChecked" Selected="true" Value="0"  />
   <telerik:RadButtonToggleState Text="Rechteprofile" PrimaryIconCssClass="rbToggleCheckbox" Value="1" />
 </ToggleStates>


 
</telerik:RadButton>--%>

  <telerik:RadTabStrip  ID="RadTabStripUserPermission" Skin="Web20" BackColor="Transparent" Width="183px"  runat="server" OnTabClick="RadTabStrip1_TabClick" CssClass="userpermissionCheckBox">
  <Tabs>
    <telerik:RadTab runat="server"
       Text="Rechte" Selected="true"
       Value="0">
    </telerik:RadTab>
    <telerik:RadTab runat="server"
       Text="Rechteprofile"
       Value="1">
    </telerik:RadTab>
  </Tabs>
</telerik:RadTabStrip>       
                              
                                 <asp:Panel ID="PermissionsPanel" runat="server">
                                        <telerik:RadListBox runat="server" ID="lbsUserPermissions" CssClass="InsuranceListBox" SelectionMode="Multiple" TabIndex="1"
            AllowReorder="true" AccessKey="y" Height="230px" AllowDelete="true" Width="250px"  TransferMode="Move" DataKeyField="Description" 
            AllowTransfer="true" TransferToID="AddedUserPermission"  EnableDragAndDrop="true" DataValueField="Id" DataTextField="Name">
            <ButtonSettings ShowDelete="false"></ButtonSettings>
            <HeaderTemplate>
          <asp:Label ID="HeaderCustomerInsurance" runat="server" Text="Vorhandene Rechte:"></asp:Label>
            </HeaderTemplate>

        </telerik:RadListBox>
        <telerik:RadListBox runat="server" ID="AddedUserPermission"  EnableDragAndDrop="true" Height="230px" TabIndex="2" Width="250px" DataValueField="Id" DataTextField="Name"   DataKeyField="Description" 
            SelectionMode="Multiple"  > 
            <HeaderTemplate>
             <asp:Label ID="HeaderCustomerInsurance"  runat="server" Text="Zugewiesene Rechte:"></asp:Label>
            </HeaderTemplate>
        </telerik:RadListBox>

                   <telerik:RadButton ID="RadButton1" runat="server" CssClass="CustomerInsuranceSaveButton"
      Text="Speichern"></telerik:RadButton>
                  
                  </asp:Panel>    
                     

                   <asp:Panel ID="PermissionProfilePanel" runat="server" Visible="false">
                                        <telerik:RadListBox runat="server" ID="lsbAllPermissionProfile" CssClass="InsuranceListBox" SelectionMode="Multiple" TabIndex="1"
            AllowReorder="true" AccessKey="y" Height="230px" AllowDelete="true" Width="250px"  TransferMode="Move" DataKeyField="Description" 
            AllowTransfer="true" TransferToID="lsbPermissionProfilePermission"  EnableDragAndDrop="true" DataValueField="Id" DataTextField="Name">
            <ButtonSettings ShowDelete="false"></ButtonSettings>
            <HeaderTemplate>
          <asp:Label ID="HeaderCustomerInsurance" runat="server" Text="Vorhandene Rechteprofile:"></asp:Label>
            </HeaderTemplate>

        </telerik:RadListBox>
        <telerik:RadListBox runat="server" ID="lsbPermissionProfilePermission"  EnableDragAndDrop="true" Height="230px" TabIndex="2" Width="250px" DataValueField="Id" DataTextField="Name"   DataKeyField="Description" 
            SelectionMode="Multiple"  > 
            <HeaderTemplate>
             <asp:Label ID="HeaderCustomerInsurance"  runat="server" Text="Zugewiesene Rechteprofile:"></asp:Label>
            </HeaderTemplate>
        </telerik:RadListBox>


        
                   <telerik:RadButton ID="RadButton2" runat="server" CssClass="CustomerInsuranceSaveButton"
      Text="Speichern"></telerik:RadButton>
                  

               </asp:Panel>




                                          
                </ContentTemplate>
    
                     </asp:UpdatePanel>
                        </div>
                                    </telerik:RadToolTip>

                                    </telerik:RadAjaxPanel>




                              


</asp:Content>
