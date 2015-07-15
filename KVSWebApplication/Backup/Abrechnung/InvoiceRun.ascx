<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InvoiceRun.ascx.cs" Inherits="KVSWebApplication.Abrechnung.InvoiceRun" %>
<asp:Panel ID="invoiceRunPanel" runat="server">
 <script src="https://code.jquery.com/jquery-1.9.1.js"></script>
<script src="https://code.jquery.com/ui/1.10.4/jquery-ui.js"></script>
 <link rel="stylesheet" href="https://code.jquery.com/ui/1.10.4/themes/smoothness/jquery-ui.css">
 <style type="text/css">
.ui-progressbar {
position: relative;
}
.progress-label {
position: absolute;
text-align:center;
top: 4px;
font-weight: bold;
text-shadow: 1px 1px 0 #fff;
}
</style>
   <script type="text/javascript">
       function onInit(sender, args) {
           var label = document.getElementById(arguments[0]._clientID.replace('txbRequest', 'invRunProp'));
           var progresslabel = document.getElementById(arguments[0]._clientID.replace('txbRequest', 'progress_label'));
           var progressDiv = document.getElementById(arguments[0]._clientID.replace('txbRequest', 'progressbar'));
           if (typeof (arguments[0]) != 'undefined' && label != null && progressDiv != null && progresslabel != null) {
               var currIRId = document.getElementById(arguments[0]._clientID.replace('txbRequest', 'invRunProp')).textContent;
               $(function () {
                   var progressbar = $("#" + progressDiv.id),
                progressLabel = $("#" + progresslabel.id);
                   progressbar.progressbar({
                       value: false,
                       change: function () {
                           progressLabel.text(progressbar.progressbar("value") + "%");
                       },
                       complete: function () {
                           progressLabel.text("Abgeschlossen!");
                       }
                   });
                   function progress() {
                       getInfomation(label.textContent);
                       var val = progressbar.progressbar("value");
                       if (typeof (returnValue) != 'undefined' && returnValue != 'null' && returnValue != '') {
                           progressbar.progressbar("value", parseInt(returnValue));
                       }                 
                       if (typeof (returnValue) == 'undefined' || returnValue < 100) {
                           setTimeout(progress, 4000);
                       }
                   }
                   setTimeout(progress, 4000);
                   var returnValue;
                   function getInfomation(sender) {
                       $.ajax({
                           type: 'POST',
                           url: 'stateRequest.aspx',
                           async: true,
                           data: { invoiceRunId: sender },
                           success: function (data) {
                               returnValue = data;                               
                           }
                       });
                   }
               });
           }
       }
       function CreateInvoiceRunConfirm() {
           if (confirmHelper()) {
               __doPostBack('GenerateInvoiceRunButton_Click', "GenerateRun");
           }
       }
       function confirmHelper() {
           return confirm("Sind Sie sicher, dass Sie einen Rechnunglauf erstellen möchten?");
       }  
     </script>
<table>
<tr>
<td>
 <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" 
Text = "Bitte wählen Sie einen Kunde aus: " ID = "RadCustomerTextBox" Width = "240px" ></telerik:RadTextBox>
    <telerik:RadComboBox  Height="300px" Width="250px"  Enabled = "true"  
Filter="Contains" runat = "server" HighlightTemplatedItems="true" AutoPostBack = "true"  
    DropDownWidth="515px"    EmptyMessage = "Alle Kunden: " OnSelectedIndexChanged = "CustomerDropDownList_OnSelectedIndexChanged"
DataTextField = "Name" DataValueField = "Value" ID = "CustomerDropDownList"
    DataSourceID="CustomerDataSource">     
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
        <asp:Button runat = "server" Text = "X" 
        ID = "clearButton" OnClick="clearButton_Click" ToolTip="Auswahl löschen" > </asp:Button>
</td>
</tr>
<tr>
<td>
<telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" 
Text = "Bitte wählen Sie den Rechnungstyp aus: " ID = "RadTextBox1" Width = "240px" ></telerik:RadTextBox>
 <telerik:radcombobox id="RechnungsTypComboBox" runat="server"
    OnInit="RechnungsTypComboBox_init" DataTextField="InvoiceTypeName" DataValueField="ID" AutoPostBack = "true" Width = "250px" > 
        </telerik:radcombobox>
</td>
</tr>
   <tr>      
</tr>
        <tr>
        <td>
        <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" 
Text = "Rechnungslauf erstellen: " ID = "rtxbCreateInvoiceRun" Width = "240px" ></telerik:RadTextBox>
    <asp:Button  runat = "server"  ID = "btnGenerateInvoiceRun" 
            Text = "Rechnungslauf erstellen" Width = "245px" 
            OnClick="GenerateInvoiceRunButton_Click"> </asp:Button>               
         </td>
            </tr>
            <tr >
            <td colspan="4"><%--Für die amtlichen Gebühren wird das Erlöskonto  verwedet!--%>
             <asp:Label ID="accInfoMessage" CssClass="riTextBox riDisabled" runat="server" Text=" Achtung! Für die amtlichen Gebühren wird das Erlöskonto "></asp:Label>
 <asp:Label ID="accountInfo" CssClass="riTextBox riDisabled" runat="server" Text="<%$AppSettings: DefaultAccountNumber %>"></asp:Label>
 <asp:Label ID="Label1" CssClass="riTextBox riDisabled" runat="server" Text=" verwedet!"></asp:Label>
 </td>
            <td></td>
            </tr>
            </table>       
     <asp:LinqDataSource ID="CustomerDataSource"  runat="server" OnSelecting="CustomerLinq_Selected" >                 
    </asp:LinqDataSource>
          <telerik:RadWindowManager ID="RadWindowManagerInvoiceRun" runat="server" EnableShadow="true">
          </telerik:RadWindowManager>
               <asp:Label ForeColor = "Red" runat = "server" ID = "InvoiceRunError" Visible = "false" ></asp:Label>
<telerik:RadGrid AutoGenerateColumns="false" ID="RadGridInvoiceRun"  
 DataSourceId = "LinqDataSourceInvoiceRun" 
                AllowFilteringByColumn="True" AllowSorting="True" 
               AllowPaging="true" Enabled = "false" runat="server"  
                GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "false" >
                <pagerstyle mode="NextPrevAndNumeric"></pagerstyle>
                <groupingsettings casesensitive="false" ></groupingsettings>
                 <HeaderStyle Width="250px" />
             <mastertableview autogeneratecolumns="false"  AllowPaging="true" PageSize="14" 
             allowfilteringbycolumn="True" CommandItemDisplay = "Top"  
             ShowHeader = "true" showfooter="True"  tablelayout="Fixed"  Name="InvoiceProgresses">        
             <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="true"  />
               <Columns>   
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="InvoiceRunId" 
                    SortExpression="InvoiceRunId" UniqueName="InvoiceRunId" Visible = "true" Display = "false" ForceExtractValue="Always" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerId" HeaderText="CustomerId" 
                    SortExpression="CustomerId" UniqueName="CustomerId" Visible = "true" Display = "false" ForceExtractValue="Always" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerName" HeaderText="Kundenname" 
                    SortExpression="CustomerName" UniqueName="CustomerName"  AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="InvoiceTypeId" ForceExtractValue="Always"
                    SortExpression="InvoiceTypeName" UniqueName="InvoiceTypeId" Visible = "true" Display = "false" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="InvoiceTypeName" HeaderText="Rechnungstyp" 
                    SortExpression="InvoiceTypeName" UniqueName="InvoiceTypeName" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                 <telerik:GridDateTimeColumn FilterControlWidth="105px" DataField="CreateDate" HeaderText="Startdatum" 
                    SortExpression="InvoicCreateDateeTypeName" UniqueName="CreateDate"  AutoPostBackOnFilter="true"   CurrentFilterFunction="Contains"
                     PickerType="DatePicker" EnableRangeFiltering="false" ShowFilterIcon="false"  EnableTimeIndependentFiltering="true">
                        <HeaderStyle Width="160px"></HeaderStyle>
                </telerik:GridDateTimeColumn>
                <telerik:GridDateTimeColumn FilterControlWidth="105px"   DataField="FinishedDate" HeaderText="Enddatum" 
                    SortExpression="FinishedDate" UniqueName="FinishedDate" AutoPostBackOnFilter="true"   CurrentFilterFunction="Contains"
                     PickerType="DatePicker" EnableRangeFiltering="false" ShowFilterIcon="false"  EnableTimeIndependentFiltering="true">
                        <HeaderStyle Width="160px"></HeaderStyle>
                </telerik:GridDateTimeColumn>
               <telerik:GridTemplateColumn UniqueName="progressbarColumn" ShowFilterIcon="false" AllowFiltering="false"  HeaderText="Status" >
                <ItemTemplate>
                <asp:Label ID="invRunProp" runat="server" Display="false" style="visibility:hidden; position:absolute;  height:0px !important;"  Text='<%# DataBinder.Eval(Container.DataItem, "InvoiceSection")%>'/> 
                <telerik:RadTextBox ID="txbRequest" runat="server"  Display="false">
                <ClientEvents OnLoad="onInit"  />
                    </telerik:RadTextBox>
                    <div id="progressbar" class="ui-progressbar" runat="server">
                    <div id="progress_label" class="progress-label"  runat="server">Warten...</div></div>
                </ItemTemplate>
                </telerik:GridTemplateColumn>
              </Columns>
            </mastertableview>            
                <ClientSettings ReorderColumnsOnClient="true"  EnablePostBackOnRowClick="true"  AllowDragToGroup="false">
                  <Scrolling AllowScroll="false"  />                  
               <Resizing  AllowColumnResize="true" ResizeGridOnColumnResize="true"></Resizing> 
            <Scrolling AllowScroll="false" ></Scrolling>
        </ClientSettings>
    </telerik:RadGrid>
    <asp:LinqDataSource  ID="LinqDataSourceInvoiceRun" runat="server" OnSelecting="InvoiceRunLinq_Selected">                 
    </asp:LinqDataSource>
    </asp:Panel>
