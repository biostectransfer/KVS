<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="CustomerInformation.aspx.cs" Inherits="KVSWebApplication.Customer.CreateCustomer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
   <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
    <telerik:RadFormDecorator ID="QsfFromDecorator" runat="server" DecoratedControls="All" EnableRoundedCorners="false" />
  <telerik:RadAjaxLoadingPanel runat="server" BackgroundTransparency="100" ID="LoadingPanel1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadTabStrip1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadTabStrip1"></telerik:AjaxUpdatedControl>
                    <telerik:AjaxUpdatedControl ControlID="RadMultiPage1" LoadingPanelID="LoadingPanel1">
                    </telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting> 
        </AjaxSettings>
    </telerik:RadAjaxManager>
 <style>
  .rgEditForm {
    height: 400px !important;
}
  </style>
  <script type="text/javascript">
      function onPopUpShowing(sender, args) {
          args.get_popUp().className += " popUpEditForm";
      }
        </script>
    <script type="text/javascript">

        function onTabSelecting(sender, args) {
            if (args.get_tab().get_pageViewID()) {
                args.get_tab().set_postBack(false);
            }
        }
    </script>
    <link href="../Styles/styles.css" rel="stylesheet" type="text/css" />
     <div class="spreadSheet">
        <div class="bottomSheetFrame">
         <telerik:RadTabStrip OnClientTabSelecting="onTabSelecting" ID="RadTabStrip1" SelectedIndex="0"
                CssClass="tabStrip" runat="server" MultiPageID="RadMultiPage1"
                OnTabClick="RadTabStrip1_TabClick" Orientation="HorizontalTop">
            </telerik:RadTabStrip>
            <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" OnPageViewCreated="RadMultiPage1_PageViewCreated"
                CssClass="multiPage">
            </telerik:RadMultiPage>   
        </div>
    </div>   
</asp:Content>
