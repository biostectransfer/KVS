﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Product_Details.aspx.cs" Inherits="KVSWebApplication.Product.Product_Details" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <script type="text/javascript">
      function onTabSelecting(sender, args) {
          if (args.get_tab().get_pageViewID()) {
              args.get_tab().set_postBack(false);
          }
      }             
    </script>
     <asp:ScriptManager ID="ScriptManagerPermissionDetails" runat="server">
    </asp:ScriptManager>
    <link href="../Styles/styles.css" rel="stylesheet" type="text/css" />
      <div class="exampleWrapper">
        <telerik:RadAjaxLoadingPanel runat="server" ID="LoadingPanelPermissionDetails" BackgroundTransparency="100">
        </telerik:RadAjaxLoadingPanel>
        <telerik:RadAjaxPanel runat="server" ID="RadAjaxPanelPermissionDetails" LoadingPanelID="LoadingPanelProductDetails"
            Height="100%">
            <div style="float: left; ">
                <telerik:RadTabStrip ID="RadTabStrip1" SelectedIndex="0" runat="server" MultiPageID="RadMultiPage1" OnTabClick="RadTabStrip1_TabClick" AutoPostBack="true"   OnClientTabSelecting="onTabSelecting" 
                    CssClass="tabStrip">
                </telerik:RadTabStrip>           
                 <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" OnPageViewCreated="RadMultiPage1_PageViewCreated"
                CssClass="multiPage">
            </telerik:RadMultiPage>        
            </div>          
        </telerik:RadAjaxPanel>
    </div>
</asp:Content>