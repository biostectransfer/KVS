<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="RequiredField_Details.aspx.cs" Inherits="KVSWebApplication.RequiredField.RequiredField_Details" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />
 <link href="../Styles/styles.css" rel="stylesheet" type="text/css" />
<div runat="server" class="uebersichtDiv" id="myUebersicht">
   <asp:ScriptManager ID="ScriptManagerPermissionDetails" runat="server">
    </asp:ScriptManager>   
      <telerik:RadAjaxManager runat="server" ID="RadAjaxManagerAllCustomerRequired">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="getAllCustomerRequired">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="getAllCustomerRequired" />
                    </UpdatedControls>
                </telerik:AjaxSetting>              
            </AjaxSettings>
        </telerik:RadAjaxManager>
         <telerik:RadAjaxPanel ID="RadAjaxPanelLargeCustomerShow" runat="server" Width="1000px"  LoadingPanelID="RadAjaxLoadingPanelLargeCustomerRequired">
          <telerik:RadWindowManager ID="RadWindowManagerLargeCustomerRequired" runat="server" EnableShadow="true">
          </telerik:RadWindowManager>
<telerik:RadGrid id="getAllCustomerRequired" runat="server" DataSourceID="getAllCustomerRequiredDataSource" Culture="de-De"  AllowAutomaticUpdates="false" 
            AllowAutomaticInserts="true" AllowAutomaticDeletes="false" AutoGenerateColumns="false"   OnInit="getAllCustomerRequired_Init" 
            AllowPaging="true"  EnableViewState ="false">           
 <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"></PagerStyle>
       <MasterTableView DataKeyNames="ContactId, Id" CommandItemDisplay="Top" EditMode="PopUp" AllowSorting="true" AllowFilteringByColumn="true" >     
           <CommandItemSettings ShowAddNewRecordButton="false"   />
            <Columns>        
                 <telerik:GridBoundColumn DataField="TableId" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                     <telerik:GridBoundColumn DataField="Id" HeaderText="Id" ReadOnly="true"  Visible="false" ForceExtractValue="Always"  />
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Kundenname"  
                     CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" SortExpression="Name"  HeaderButtonType="TextButton"/>
                        <telerik:GridBoundColumn DataField="CustomerNumber" HeaderText="Kundennummer"  HeaderButtonType="TextButton"
                       CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" SortExpression="Kundennummer"/>
                    <telerik:GridBoundColumn DataField="ContactId"  ReadOnly="true"  Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="AdressId"  ReadOnly="true"  Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="Street" HeaderText="Strasse"   HeaderButtonType="TextButton"
                    CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" SortExpression="Street"/>
                    <telerik:GridBoundColumn DataField="StreetNumber" HeaderText="Nummer"   HeaderButtonType="TextButton"
                    CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" SortExpression="StreetNumber"/>
                    <telerik:GridBoundColumn DataField="Zipcode" HeaderText="PLZ"  HeaderButtonType="TextButton"
                     CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" SortExpression="Zipcode"/>
                    <telerik:GridBoundColumn DataField="City" HeaderText="Ort"  HeaderButtonType="TextButton"
                    CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" SortExpression="City"/>
                    <telerik:GridBoundColumn DataField="Country" HeaderText="Land" HeaderButtonType="TextButton"
                    CurrentFilterFunction="Contains" ShowFilterIcon="false"   AutoPostBackOnFilter="true" FilterControlWidth="95%" AllowSorting="true" SortExpression="Country"/>                
                    <telerik:GridTemplateColumn HeaderText="Pflichtfelder" HeaderButtonType="TextButton" ShowFilterIcon="false"  AllowFiltering="false"
                    HeaderStyle-Width="120px" FilterControlWidth="85px" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center" 
                    AutoPostBackOnFilter="true" >                            
                      <ItemTemplate>
                        <asp:Label ID="lblPflichtfelderText" runat="server" Text='Zuweisen/Anzeigen' >
                        </asp:Label>
                            <asp:Label ID="lblId" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>' Visible="false"    runat="server"  >
                        </asp:Label>   
                                <telerik:RadToolTip ManualClose="true" ManualCloseButtonText="Schließen"   ID="ttAngebotskopf" runat="server"   
                                TargetControlID="lblPflichtfelderText" Animation="Slide" 
                                RelativeTo="Element"  Position="BottomLeft">
                        <div style="width:640px;  white-space:normal;" >
                            <telerik:RadListBox runat="server" ID="AllRequired" CssClass="InsuranceListBox" SelectionMode="Multiple" TabIndex="1"
            AllowReorder="true" AccessKey="y" Height="230px" AllowDelete="true" Width="270px"  TransferMode="Move"
            AllowTransfer="true" TransferToID="CustomerRequired"  EnableDragAndDrop="true" DataSourceID="RequiredListBoxDataSource" DataValueField="Id" DataTextField="Name">
            <ButtonSettings ShowDelete="false"></ButtonSettings>
            <HeaderTemplate>
          <asp:Label ID="HeaderCustomerRequired" runat="server" Text="Bekannte Pflichtfelder:"></asp:Label>
            </HeaderTemplate>         
        </telerik:RadListBox>
        <telerik:RadListBox runat="server" ID="CustomerRequired"  EnableDragAndDrop="true" Height="230px" TabIndex="2" Width="260px" DataValueField="Id" DataTextField="Name" 
            SelectionMode="Multiple" DataSourceID="CustomerRequiredListbox" >
            <HeaderTemplate>
             <asp:Label ID="HeaderCustomerRequired"  runat="server" Text="Zugewiesene Pflichtfelder:"></asp:Label>
            </HeaderTemplate>
        </telerik:RadListBox>
     <telerik:RadButton ID="bSave" runat="server" CssClass="CustomerInsuranceSaveButton"  OnClick="btnSaveRequired_Click" CommandArgument='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>'
      Text="Speichern"></telerik:RadButton>                            
                     <asp:LinqDataSource runat=server ID="RequiredListBoxDataSource" OnSelecting="RequiredListBox_Selecting" >
                    <WhereParameters>
                        <asp:ControlParameter ControlID="lblId" Name="LargeCustomerId" />
                     </WhereParameters>
                      </asp:LinqDataSource>                     
                   <asp:LinqDataSource runat=server ID="CustomerRequiredListbox" OnSelecting="CustomerRequiredListbox_Selecting">                       
                        <WhereParameters>
                        <asp:ControlParameter ControlID="lblId" Name="LargeCustomerId" />
                        </WhereParameters>
                    </asp:LinqDataSource>
                    </div>
                        </div>
                    </telerik:RadToolTip>   
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
             </Columns>
                <EditFormSettings EditFormType="AutoGenerated" >
                    <EditColumn ButtonType="ImageButton" />
                    <PopUpSettings Modal="true"    />               
                </EditFormSettings>
                     <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
            <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />
            </MasterTableView>            
            <PagerStyle AlwaysVisible="true" />
        </telerik:RadGrid> 
 </telerik:RadAjaxPanel>
      <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelLargeCustomerRequired" BackgroundTransparency="100" runat="server">
        </telerik:RadAjaxLoadingPanel>
<asp:LinqDataSource runat=server ID="getAllCustomerRequiredDataSource" OnSelecting="getAllCustomerRequiredDataSource_Selecting" ></asp:LinqDataSource>
     </div>
</asp:Content>