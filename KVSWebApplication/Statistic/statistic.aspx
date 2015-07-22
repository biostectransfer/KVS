<%@ Page Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="statistic.aspx.cs" Inherits="KVSWebApplication.Statistic.statistic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
 <asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<link href = "../Styles/statisticStyle.css"  rel="stylesheet" type="text/css" />
<script type="text/javascript">
    function GoToToday() {
        var datepicker = $find("<%=BisPicker.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }
    function GoToToday2() {
        var datepicker = $find("<%=VonPicker.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }  
</script>
  <table border="0">
             <tr>
             <td>
              <telerik:RadChart Skin = "Sunset" ChartTitle-TextBlock-Text = "Anzahl der Aufträge pro Kunde" AlternateText = "hallo" runat = "server" ID = "anzahlChart" Width = "1200px"  Height="700px"></telerik:RadChart>
             </td>
              </tr>
              <tr>
             <td>
              <telerik:RadChart Skin = "Sunset" ChartTitle-TextBlock-Text = "Umsatz pro Kunde" AlternateText = "hallo" runat = "server" ID = "umsatzProKundeChart" Width = "1200px" Height="700px"></telerik:RadChart>
             </td>
             </tr>
  </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
      <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
		        <Scripts>
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
			        <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
		        </Scripts>
	            </telerik:RadScriptManager>  
<telerik:RadAjaxLoadingPanel runat="server" ID="LoadingPanel1" BackgroundTransparency = "100" ></telerik:RadAjaxLoadingPanel>
<telerik:RadAjaxPanel runat="server" ID="RadAjaxPanel1" LoadingPanelID="LoadingPanel1" Height="100%">
<br />
<br />
  <table  cellspacing="10"   cellpadding="20" class="table1">
     <thead >
 <tr >
  <th colspan="4">
  <asp:Label runat = "server" ID = "ForTableLabel" Font-Bold = "true"   Text = "Für die ausgewählten Suchparameter gelten folgende Ergebnisse"></asp:Label>
  </th>
  </tr>
  </thead>
             <tr>
             <td>
             Anzahl der Aufträge
             </td>
             <td>
             Amtliche Gebühr(Gesamt)
             </td>
             <td>
             Umsatz(Gesamt)
             </td>
             <td align = "center">
             Anzahl der Aufträge pro Auftragstyp
             </td>
             </tr>
             <tr>
             <td align = "center">
             <asp:Label runat = "server" ID = "AuftragsCounterLabel"></asp:Label>
             </td>
             <td>
             <asp:Label runat = "server" ID = "SummeAmtGebuhrLabel"></asp:Label>
             </td>
             <td>
             <asp:Label runat = "server" ID = "UmsatzLabel"></asp:Label>
             </td>  
             <td>
                    <telerik:RadHtmlChart runat="server" ID="PieChart1" Width = "560px"  Height="450" Transitions="true"
                    Skin="Forest">
                    <Appearance>
                         <FillStyle BackgroundColor="White"></FillStyle>
                    </Appearance>
                    <ChartTitle Text="">
                         <Appearance Align="Center" BackgroundColor="White" Position="Top"></Appearance>
                    </ChartTitle>
                    <Legend>
                         <Appearance BackgroundColor="White" Position="Right" Visible="true"></Appearance>
                    </Legend>
                    <PlotArea>
                         <Appearance>
                              <FillStyle BackgroundColor="White"></FillStyle>
                         </Appearance>
                         <Series>
                              <telerik:PieSeries StartAngle="90">
                                   <LabelsAppearance Position="Circle" DataFormatString="{0} Auftrag(e)">
                                   </LabelsAppearance>
                                   <Items>
                                        <telerik:SeriesItem BackgroundColor="#ff9900" Exploded="true" Name="Zulassung"></telerik:SeriesItem>
                                        <telerik:SeriesItem BackgroundColor="#cccccc" Exploded="false" Name="Abmeldung">
                                        </telerik:SeriesItem>                              
                                   </Items>
                              </telerik:PieSeries>
                         </Series>
                    </PlotArea>
               </telerik:RadHtmlChart>
             </td>
             </tr>              
  </table>
<br />
<br />
<asp:Label runat = "server" ID = "Label1" Font-Bold = "true" CssClass="labelFont"  Text = "Suche: "></asp:Label>  
<br />
<table class="table1" width="1200px">
<thead>
<tr>
<th>
    <telerik:RadComboBox  Height="300px" Width="250px"  Enabled = "true"  
Filter="Contains" runat = "server" HighlightTemplatedItems="true" AutoPostBack = "false" 
    DropDownWidth="515px"    EmptyMessage = "Bitte wählen Sie einen Kunden aus: "
DataTextField = "Name" DataValueField = "Value" ID = "CustomerNameBox"
DataSourceID="LinqDataSourceCustomerName">  
        <HeaderTemplate>
                    <table style="width: 515px" cellspacing="0" cellpadding="0">
                        <tr align="center">
                            <td style="width: 90px;">
                                Kundennummer
                            </td>
                            <td style="width: 175px;">
                                Matchcode
                            </td>                                         
                            <td style="width: 250px">
                                Kundenname
                            </td>
                        </tr>
                    </table>
                </HeaderTemplate>
                    <ItemTemplate>
                    <table style="width: 515px;" cellspacing="0" cellpadding="0">
                        <tr>
                            <td style="width: 110px;">
                                <%# DataBinder.Eval(Container, "DataItem.Kundennummer")%>
                            </td>
                            <td style="width: 175px;">
                                <%# DataBinder.Eval(Container, "DataItem.Matchcode") %>                                     
                            </td>
                            <td style="width: 250px;">
                                <%# DataBinder.Eval(Container, "DataItem.Name").ToString() %>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
    </telerik:RadComboBox >
</th>
<th>
<telerik:RadComboBox ID="AuftragstypBox" runat="server" Width="200" DataTextField = "Name" DataValueField = "Value" EmptyMessage="Auftragstyp" DataSourceID="LinqDataSourceAuftragstyp"></telerik:RadComboBox>
</th>
<th>
Von:
<telerik:RadDatePicker runat = "server" ID = "VonPicker">
   <Calendar ID="Calendar2" runat = "server">
        <FooterTemplate> 
            <div style="width: 100%; text-align: center; background-color: Gray;"> 
                <input id="Button1" type="button" value="Heute" onclick="GoToToday2()" /> 
            </div> 
        </FooterTemplate> 
    </Calendar>
</telerik:RadDatePicker></td>
<th>
Bis:
<telerik:RadDatePicker runat = "server" ID = "BisPicker">
   <Calendar ID="Calendar1" runat = "server">
        <FooterTemplate> 
            <div style="width: 100%; text-align: center; background-color: Gray;"> 
                <input id="Button1" type="button" value="Heute" onclick="GoToToday()" /> 
            </div> 
        </FooterTemplate> 
    </Calendar>
</telerik:RadDatePicker></td>
<th>
<telerik:RadButton runat = "server" Text = "Anzeigen" ID = "searchButton" OnClick = "searchButton_Clicked"> </telerik:RadButton>
<telerik:RadButton runat = "server" Text = "Alles" ID = "AllgemeinButton" OnClick = "AllgemeinButton_Clicked"> </telerik:RadButton>
</th>
</tr>
  </thead>
  </table>
        <telerik:RadGrid AutoGenerateColumns="false" ID="RadGridStatistic" DataSourceID="RadGridStatistic_NeedDataSource" 
        AllowFilteringByColumn="True" AllowSorting="True" PageSize="15" PagerStyle-AlwaysVisible="true"
        ShowFooter="True" Width = "1200"  AllowPaging="True" Enabled = "true" runat="server" GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "false" >
        <pagerstyle mode="NextPrevAndNumeric"></pagerstyle>
        <groupingsettings casesensitive="false" ></groupingsettings>
        <mastertableview CommandItemDisplay = "Top" ShowHeader = "true" autogeneratecolumns="false"  allowfilteringbycolumn="True" showfooter="True" tablelayout="Auto" >       
    <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="true"  />
        <Columns>
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderNumber" HeaderText="OrderNumber"
            SortExpression="OrderNumber"   UniqueName="OrderNumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
            ShowFilterIcon="false">
        </telerik:GridBoundColumn>   
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerId" HeaderText="CustomerId"
            SortExpression="CustomerId" Display = "false" UniqueName="CustomerId" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
            ShowFilterIcon="false">
        </telerik:GridBoundColumn>                             
        <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerName" HeaderText="Kundenname"
            SortExpression="CustomerName" UniqueName="CustomerName" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
            ShowFilterIcon="false">
        </telerik:GridBoundColumn>
        <%--<telerik:GridBoundColumn FilterControlWidth="105px" DataField="Ordernumber" HeaderText="Auftragsnummer"
            SortExpression="Ordernumber" UniqueName="Ordernumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
            ShowFilterIcon="false">
        </telerik:GridBoundColumn>--%>
        <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderTyp" HeaderText="Auftragstyp"
            SortExpression="OrderTyp" UniqueName="OrderTyp" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
            ShowFilterIcon="false">
        </telerik:GridBoundColumn>                 
        <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Kennzeichen" HeaderText="Kennzeichen"
            SortExpression="Kennzeichen" UniqueName="Kennzeichen" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
            ShowFilterIcon="false">
        </telerik:GridBoundColumn>                  
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Haltername" HeaderText="Haltername"
            SortExpression="Haltername" UniqueName="Haltername" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
            ShowFilterIcon="false">
        </telerik:GridBoundColumn>   
        <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Status" HeaderText="Auftragsstatus"
            SortExpression="Status" UniqueName="Status" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
            ShowFilterIcon="false">
        </telerik:GridBoundColumn>
            <telerik:GridDateTimeColumn DataField="CreateDate" HeaderText="Erstellungsdatum" FilterControlWidth="95px"
            SortExpression="CreateDate" UniqueName="CreateDate" PickerType="DatePicker" EnableTimeIndependentFiltering="true">
        </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerLocation" HeaderText="Standort"
            SortExpression="CustomerLocation" UniqueName="CustomerLocation" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
            ShowFilterIcon="false">
        </telerik:GridBoundColumn>
        <telerik:GridBoundColumn FilterControlWidth="105px" DataField="VIN" HeaderText="VIN"
            SortExpression="VIN" UniqueName="VIN" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
            ShowFilterIcon="false">
        </telerik:GridBoundColumn>       
        </Columns>
    </mastertableview>           
<clientsettings>
    <Scrolling AllowScroll="false" ></Scrolling>
    <Selecting AllowRowSelect="true" ></Selecting>
</clientsettings>
    </telerik:RadGrid>
   </telerik:RadAjaxPanel>     
<asp:LinqDataSource ID="LinqDataSourceCustomerName" runat="server" OnSelecting="CustomerName_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
<asp:LinqDataSource ID="RadGridStatistic_NeedDataSource" runat="server" OnSelecting="RadGridStatistic_NeedDataSource_Linq" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
<asp:LinqDataSource ID="LinqDataSourceAuftragstyp" runat="server" OnSelecting="LinqDataSourceAuftragstyp_Linq" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>