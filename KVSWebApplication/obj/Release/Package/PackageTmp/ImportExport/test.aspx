<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="KVSWebApplication.ImportExport.test" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns='http://www.w3.org/1999/xhtml'>
<head>
    <title>Basic Filtering capabilities of ASP.NET AJAX Grid</title>
</head>
<body>
    <form id="form1" runat="server">
      <asp:LinqDataSource runat="server" ID="getAllCostCenterDataSource" OnSelecting="getAllInvoiceDataSource_Selecting" ></asp:LinqDataSource>
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
    <telerik:RadSkinManager ID="QsfSkinManager" runat="server" ShowChooser="true" Skin="Default" />
    <telerik:RadFormDecorator ID="QsfFromDecorator" runat="server" DecoratedControls="All" EnableRoundedCorners="false" />
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
    </telerik:RadAjaxLoadingPanel>
    <h2>Filtering with Server-Side Binding</h2>
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
        <telerik:RadGrid AutoGenerateColumns="false" ID="RadGrid1" DataSourceID="getAllCostCenterDataSource"
            AllowFilteringByColumn="True" AllowSorting="True"
            ShowFooter="True" AllowPaging="True" runat="server">
            <GroupingSettings CaseSensitive="false"></GroupingSettings>
            <MasterTableView AutoGenerateColumns="false" AllowFilteringByColumn="True" ShowFooter="True">
                <Columns>
                    <telerik:GridMaskedColumn DataField="OrderID" HeaderText="OrderID"
                        FilterControlWidth="50px" AutoPostBackOnFilter="false" CurrentFilterFunction="EqualTo"
                        FilterDelay="2000" ShowFilterIcon="false" Mask="#####">
                    </telerik:GridMaskedColumn>
                    <telerik:GridBoundColumn FilterControlWidth="120px" DataField="ShipName" HeaderText="ShipName"
                        AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                        ShowFilterIcon="false">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn DataField="OrderDate" HeaderText="OrderDate" FilterControlWidth="110px"
                        SortExpression="OrderDate" PickerType="DatePicker" EnableTimeIndependentFiltering="true"
                        DataFormatString="{0:MM/dd/yyyy}">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridDateTimeColumn FilterControlWidth="95px" DataField="ShippedDate" HeaderText="ShippedDate"
                        PickerType="DatePicker" EnableRangeFiltering="true">
                        <HeaderStyle Width="160px"></HeaderStyle>
                    </telerik:GridDateTimeColumn>
                   <telerik:GridBoundColumn FilterControlWidth="50px" DataField="ShipPostalCode" HeaderText="ShipPostalCode">
                        <FooterStyle Font-Bold="true"></FooterStyle>
                    </telerik:GridBoundColumn>         
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </telerik:RadAjaxPanel>
    <br />    </form>
</body>
</html>