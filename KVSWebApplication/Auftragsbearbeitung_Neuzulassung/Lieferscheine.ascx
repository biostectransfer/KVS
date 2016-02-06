<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Lieferscheine.ascx.cs" Inherits="KVSWebApplication.Auftragsbearbeitung_Neuzulassung.Lieferscheine" %>
 <telerik:RadCodeBlock ID="RadCodeBlock134"  runat = "server">
       <script type="text/javascript">
           function Closewindow() {
               var Result = confirm("Für ausgewählten Standort haben wir nicht geschlossene Auftraege gefunden. Wollen Sie trotzdem fortfahren?");
               if (Result == true) {
                   var UserValueConfirm = document.getElementById("<%=UserValueConfirm.ClientID %>");
                   UserValueConfirm.value = Result;
                   __doPostBack('UserValueConfirmLieferscheine', "CloseWindow");
                   return true;
               }
               else {
                   document.getElementById("<%=UserValueConfirm.ClientID %>").value = Result;
                   __doPostBack('UserValueDontConfirmLieferscheine', "CloseWindow");
                   return false;
               }
           }         
           function selectRows(sender) {
               var header = sender.parentNode.parentNode;
               var currentNode = header.nextSibling;
               var masterTable = $find('<%=RadGridLieferscheine.ClientID%>').get_masterTableView();
               if (sender.checked) {
                   while (currentNode.className != 'rgGroupHeader' && typeof currentNode.className != 'undefined') {
                       masterTable.selectItem(currentNode);
                       currentNode = currentNode.nextSibling;
                   }
               }
               else {
                   while (currentNode.className != 'rgGroupHeader' && typeof currentNode.className != 'undefined') {
                       masterTable.deselectItem(currentNode);
                       currentNode = currentNode.nextSibling;
                   }
               }
           }
           function CreatePacking() {
               __doPostBack('UserValueConfirmLieferscheine', "CreatePacking");
               return true;
           }
        </script>
</telerik:RadCodeBlock>
<asp:Panel runat = "server" ID = "Panel5">         
   <asp:HiddenField runat = "server" ID="UserValueConfirm" />
    <telerik:RadGrid ID="RadGridLieferscheine" Width = "1450px" DataSourceID="LinqDataSourceLieferscheine" runat="server" PageSize="15"
        AllowSorting="True" AllowMultiRowSelection="True" AllowPaging="True" ShowGroupPanel="True" ShowFooter="true"
        AutoGenerateColumns="False" GridLines="none"  EnableLinqExpressions = "true" OnItemCommand="OnItemCommand_Fired">
        <PagerStyle Mode="NextPrevNumericAndAdvanced"></PagerStyle>
        <MasterTableView  CommandItemDisplay = "Top" ShowHeader = "true" AutoGenerateColumns = "false" DataSourceID = "LinqDataSourceLieferscheine" GroupLoadMode = "Client" 
            GroupsDefaultExpanded = "false" AllowFilteringByColumn="True">
            <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="true"  />
            <Columns>    
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderNumber" HeaderText="Auftragsnummer"
                    SortExpression="OrderNumber"   UniqueName="OrderNumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn> 
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="locationId" HeaderText="locationId"
                    SortExpression="locationId" Display = "false" UniqueName="locationId" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
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
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Status" HeaderText="Auftragsstatus"
                    SortExpression="Status" UniqueName="Status" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                 <telerik:GridDateTimeColumn DataField="CreateDate" HeaderText="Erstellungsdatum" FilterControlWidth="95px"
                    SortExpression="CreateDate" UniqueName="CreateDate" PickerType="DatePicker" EnableTimeIndependentFiltering="true">
                </telerik:GridDateTimeColumn>
                 <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Standort" HeaderText="CustomerLocation"
                    SortExpression="CustomerLocation" UniqueName="CustomerLocation" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="VIN" HeaderText="FIN"
                    SortExpression="VIN" UniqueName="VIN" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Kennzeichen" HeaderText="Kennzeichen"
                    SortExpression="Kennzeichen" UniqueName="Kennzeichen" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderTyp" HeaderText="Auftragstyp"
                    SortExpression="OrderTyp" UniqueName="OrderTyp" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>  
                <telerik:GridButtonColumn HeaderStyle-Width="180px" ButtonType="PushButton" Text="zurück zur Zulassungstelle" UniqueName="ZurueckZullasungstelleButton" 
                    Visible="true" CommandName="ZurueckZullasungstelle" ItemStyle-HorizontalAlign="Right">
                </telerik:GridButtonColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings  ReorderColumnsOnClient="True" AllowDragToGroup="True" AllowColumnsReorder="True">
            <Selecting  AllowRowSelect="True"></Selecting>
            <Resizing AllowRowResize="True" AllowColumnResize="True" EnableRealTimeResize="True"
                ResizeGridOnColumnResize="False"></Resizing>
        </ClientSettings>
        <GroupingSettings ShowUnGroupButton="true"></GroupingSettings>
    </telerik:RadGrid>    
    <br />
   <telerik:RadButton runat = "server" ID = "LieferungButton" Text = "Lieferscheine erstellen" OnClick = "LieferItems_Selected"> </telerik:RadButton>
   <telerik:RadTextBox runat="server" Enabled = "false" Visible = "false" DisabledStyle-ForeColor = "Red" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Bitte wählen Sie die Aufträge" ID = "BitteTextBox" Width = "240px" ></telerik:RadTextBox>
   <asp:Label ID = "ErrorLabelLieferschein" runat = "server" Visible = "false" ForeColor = "Red" Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator."></asp:Label>         
         <telerik:RadTextBox runat="server" Enabled = "false" Visible = "false" DisabledStyle-ForeColor = "Red" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Sie haben bestätigt" ID = "AuswahlGetroffenBox" Width = "240px" ></telerik:RadTextBox>
            <telerik:RadTextBox runat="server" Enabled = "false" Visible = "false" DisabledStyle-ForeColor = "Red" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Sie haben abgelehnt " ID = "NichtFortfahrenBox" Width = "240px" ></telerik:RadTextBox>
<asp:LinqDataSource TableName = "Customer" ID="LinqDataSourceLieferscheine" runat="server" OnSelecting="LieferscheineLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
</asp:Panel>
<asp:Label Visible = "false" runat = "server" ID = "AllesOkLieferscheine" ForeColor = "Green" Text = "Lieferschein erfolgreich erstellt!"></asp:Label>
<asp:Panel runat = "server" ID = "OffenePanel" Visible = "false">
<asp:Label ID = "ErrorOffeneLabel" runat = "server" ForeColor = "Red"></asp:Label>
<br />
<asp:Label ID = "OffeneLabel" runat = "server" Visible = "false" Text = "Hier sind alle noch nicht geschlossene Aufträge: " Font-Size = "Larger"></asp:Label>
<br />
 <telerik:RadGrid ID="NochOffenAuftraegeRadGrid" OnItemCommand = "FertigstellenButton_Clicked" Enabled = "false" 
 OnEditCommand = "FertigstellenButton_Clicked" Width ="1450px" DataSourceID="LinqDataSourceLieferscheineOffene" runat="server" PageSize="15"
        AllowSorting="True" AllowMultiRowSelection="True" AllowPaging="True" ShowGroupPanel="True"
        AutoGenerateColumns="False" GridLines="none"  EnableLinqExpressions = "true">
        <PagerStyle Mode="NextPrevNumericAndAdvanced"></PagerStyle>
        <MasterTableView  AutoGenerateColumns = "false" DataSourceID = "LinqDataSourceLieferscheineOffene" GroupLoadMode = "Client" GroupsDefaultExpanded = "false">
            <Columns>                
            <telerik:GridButtonColumn ButtonType = "PushButton" Text = "Fertigstellen"> </telerik:GridButtonColumn>        
            <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderNumber" HeaderText="OrderNumber"
                    SortExpression="OrderNumber" Display = "false" UniqueName="OrderNumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn> 
                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="customerID" HeaderText="customerID"
                    SortExpression="customerID" Display = "false" UniqueName="customerID" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>  
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerName" HeaderText="Kundenname"
                    SortExpression="CustomerName" UniqueName="CustomerName" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Ordernumber" HeaderText="Auftragsnummer"
                    SortExpression="Ordernumber" UniqueName="Ordernumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
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
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="HSN" HeaderText="HSN"
                    SortExpression="HSN" UniqueName="HSN" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="TSN" HeaderText="TSN"
                    SortExpression="TSN" UniqueName="TSN" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderTyp" HeaderText="Auftragstyp"
                    SortExpression="OrderTyp" UniqueName="OrderTyp" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn> 
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="HasError" HeaderText="Fehler"
                    SortExpression="HasError" UniqueName="HasError" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn> 
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="ErrorReason" HeaderText="Ursache"
                    SortExpression="ErrorReason" UniqueName="ErrorReason" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn> 
            </Columns>
        </MasterTableView>
        <ClientSettings  ReorderColumnsOnClient="True" AllowDragToGroup="True" AllowColumnsReorder="True">
            <Selecting  AllowRowSelect="True"></Selecting>
            <Resizing AllowRowResize="True" AllowColumnResize="True" EnableRealTimeResize="True"
                ResizeGridOnColumnResize="False"></Resizing>
        </ClientSettings>
    </telerik:RadGrid>
    <asp:Label ForeColor = "Green"  runat = "server" ID = "AllesIsOkeyBeiOffene" Visible = "false" Text = "Ausgewählten Auftrag ist erfolgreich abgeschlossen!"></asp:Label>
</asp:Panel>
<asp:HiddenField runat = "server" ID = "LocationIdHiddenField"/>
<asp:HiddenField runat = "server" ID = "userAuswahl"/>
<asp:LinqDataSource TableName = "Customer" ID="LinqDataSourceLieferscheineOffene" runat="server" OnSelecting="LieferscheineOffeneLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>