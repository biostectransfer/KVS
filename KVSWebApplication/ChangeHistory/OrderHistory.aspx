<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="OrderHistory.aspx.cs" Inherits="KVSWebApplication.ChangeHistory.OrderHistory" %>
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
         <telerik:RadAjaxPanel ID="RadAjaxPanelLargeCustomerShow" runat="server" Width="1400px"   LoadingPanelID="RadAjaxLoadingPanelLargeCustomerRequired">
  <telerik:RadGrid   AutoGenerateColumns="false" ID="RadGridAllChanges"  
                DataSourceId = "LinqDataSourceAllChanges" 
                AllowFilteringByColumn="True" AllowSorting="True"
                ShowFooter="True" AllowPaging="true" Enabled = "true" ShowHeader = "true" 
                runat="server" GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "true"  >
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
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="LogId" HeaderText="Datensatz Id"
                    SortExpression="LogId" UniqueName="LogId" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
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
                  <telerik:GridDateTimeColumn DataField="Date" HeaderText="Erstellungsdatum" AutoPostBackOnFilter="true" FilterControlWidth="105px" 
                    SortExpression="Date" UniqueName="Date" PickerType="DatePicker"  ShowFilterIcon="false"
                     CurrentFilterFunction="Contains" EnableTimeIndependentFiltering="true">
                </telerik:GridDateTimeColumn>
                  <telerik:GridBoundColumn  DataField="OrderNumber" HeaderText="Auftragsnummer"
                    SortExpression="OrderNumber" UniqueName="Text" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn> 
                     <telerik:GridBoundColumn  DataField="Type" HeaderText="Typ"
                    SortExpression="Type" UniqueName="Type" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn> 
                <telerik:GridBoundColumn  DataField="TranslatedText" HeaderText="Änderung"
                    SortExpression="TranslatedText" UniqueName="TranslatedText"   FilterControlWidth="190px"
                    AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn> 
              </Columns>   
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