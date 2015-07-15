<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="ChangeLog.aspx.cs" Inherits="KVSWebApplication.ChangeLog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:ScriptManager ID="SearchManager" runat="server"></asp:ScriptManager>
      <telerik:RadAjaxManager runat="server" ID="RadAjaxManagerRadGridAllChanges">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="RadGridAllChanges">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="RadGridAllChanges" LoadingPanelID="myAllChangesPanel" />
                    </UpdatedControls>
                </telerik:AjaxSetting>              
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel ID="myAllChangesPanel" runat="server"></telerik:RadAjaxLoadingPanel>
         <telerik:RadAjaxPanel ID="RadAjaxPanelLargeCustomerShow" runat="server" Width="1000px"   LoadingPanelID="RadAjaxLoadingPanelLargeCustomerRequired">
  <telerik:RadGrid   AutoGenerateColumns="false" ID="RadGridAllChanges"  
                DataSourceId = "LinqDataSourceAllChanges"  OnDetailTableDataBind="RadGridAllChanges_DetailTableDataBind"
                AllowFilteringByColumn="True" AllowSorting="True"
                ShowFooter="True" AllowPaging="true" Enabled = "true" ShowHeader = "true" runat="server" GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "true"  >
                <pagerstyle mode="NextPrevAndNumeric" AlwaysVisible="true"></pagerstyle>
         <ExportSettings >
                <Excel Format="Biff" ></Excel>
            </ExportSettings>
                <groupingsettings casesensitive="false" ></groupingsettings>
             <mastertableview  CommandItemDisplay = "Top"  autogeneratecolumns="false"  Name="Orders" allowfilteringbycolumn="True" 
              ShowHeader = "true" ShowFooter="True" tablelayout="Auto" >
                 <CommandItemSettings ShowExportToWordButton="false" ShowExportToExcelButton="false" ShowExportToCsvButton="false" ShowAddNewRecordButton="false">
            </CommandItemSettings>    
            <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="true"  />
               <Columns>             
                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Id" HeaderText="Id"
                    SortExpression="Id" Display = "false" UniqueName="Id" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>   
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="LogUserId" HeaderText="LogUserId"
                    SortExpression="LogUserId" Display = "false" UniqueName="LogUserId" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>  
                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="ReferenceId" HeaderText="ReferenceId"
                    SortExpression="ReferenceId" Display = "false" UniqueName="ReferenceId" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>     
                            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Expr1" HeaderText="Datensatz Id"
                    SortExpression="Expr1" UniqueName="Expr1" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>    
                      <telerik:GridBoundColumn FilterControlWidth="105px" DataField="TableName" HeaderText="TableName"
                    SortExpression="TableName" Display = "false" UniqueName="TableName" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>  
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Name" HeaderText="Vorname"
                    SortExpression="Name" UniqueName="Name" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="90px" DataField="FirstName" HeaderText="Nachname"
                    SortExpression="FirstName" UniqueName="FirstName" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                     <telerik:GridBoundColumn FilterControlWidth="90px" DataField="Login" HeaderText="Benutzername"
                    SortExpression="Login" UniqueName="Login" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                   <telerik:GridDateTimeColumn DataField="LastLogin" HeaderText="Letzter Login" AutoPostBackOnFilter="true" FilterControlWidth="105px" 
                    SortExpression="LastLogin" UniqueName="LastLogin" PickerType="DatePicker"  ShowFilterIcon="false"
                     CurrentFilterFunction="Contains" EnableTimeIndependentFiltering="true">
                </telerik:GridDateTimeColumn>
                  <telerik:GridDateTimeColumn DataField="Date" HeaderText="Erstellungsdatum" AutoPostBackOnFilter="true" FilterControlWidth="105px" 
                    SortExpression="Date" UniqueName="Date" PickerType="DatePicker"  ShowFilterIcon="false"
                     CurrentFilterFunction="Contains" EnableTimeIndependentFiltering="true">
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn  DataField="Text" HeaderText="Änderung"
                    SortExpression="Text" UniqueName="Text" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn> 
              </Columns>
                      <DetailTables>
              <telerik:GridTableView  Width="100%" AllowFilteringByColumn = "false" EditFormSettings-EditFormType  = "Template" EditMode = "PopUp">     
              </telerik:GridTableView>
              </DetailTables>
                <NestedViewTemplate>                 
                   <asp:Panel runat="server" ID="InnerContainer" CssClass="nestedTemplate" Visible="true">
                      <asp:Label ID="lblReferenceId" Visible = "false" Text='<%# Bind( "ReferenceId") %>' runat="server"></asp:Label>  
                    <asp:Label ID="lblTableName" Visible = "false" Text='<%# Bind( "TableName") %>' runat="server"></asp:Label>  
                    <telerik:RadGrid ID="radGridDetailInfo" runat="server"  AutoGenerateColumns="true"  OnNeedDataSource="RadGridAllChanges_DetailTableDataBind"     AllowFilteringByColumn="false" AllowSorting="True"
                ShowFooter="True" AllowPaging="true" Enabled = "true" ShowHeader = "true"  GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "true"  >
                <pagerstyle mode="NextPrevAndNumeric" AlwaysVisible="true"></pagerstyle>  
                    <mastertableview   ShowHeader="true" CommandItemDisplay = "Top"  
                    ShowHeadersWhenNoRecords="true" NoMasterRecordsText="Keine Datensätze vorhanden"  AllowFilteringByColumn="false" tablelayout="Auto" >
             <HeaderStyle Width="150px" />
                      <CommandItemSettings ShowExportToWordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="false" ShowExportToCsvButton="false" ShowAddNewRecordButton="false">
            </CommandItemSettings>           
             </mastertableview>
                    </telerik:RadGrid>
                               </asp:Panel>
                   </NestedViewTemplate>
            </mastertableview>
            <ItemStyle BackColor="#DFDFDF" />
              <clientsettings EnableAlternatingItems="false">              
            <Scrolling AllowScroll="false" ></Scrolling>
             <Selecting AllowRowSelect="false" ></Selecting>
        </clientsettings>
    </telerik:RadGrid>
    </telerik:RadAjaxPanel>
    <asp:LinqDataSource TableName = "ChangeLog" ID="LinqDataSourceAllChanges" runat="server" OnSelecting="AllChangesLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
</asp:Content>