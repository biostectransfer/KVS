<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LieferscheinAbmeldung.ascx.cs" Inherits="KVSWebApplication.Nachbearbeitung_Abmeldung.LieferscheinAbmeldung" %>
<telerik:RadCodeBlock ID="RadCodeBlock21" runat = "server">
     <script type="text/javascript">
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
             function Closewindow() {
                 var Result = confirm("Wir haben nicht geschlossene Auftraege gefunden. Wollen Sie trotzdem fortfahren?");
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
        </script>
</telerik:RadCodeBlock>
<telerik:RadFormDecorator runat = "server" ID = "LieferscheinDekorator" DecoratedControls = "all"/>
<asp:Label ID = "testlabel" Visible = "false" runat = "server"></asp:Label>
<asp:Panel runat = "server" ID = "Panel1">
<asp:HiddenField runat = "server" ID="UserValueConfirm"/>
    <telerik:RadGrid ID="RadGridLieferscheine"  Width = "1450px" DataSourceID="LinqDataSourceLieferscheine" runat="server" PageSize="15"
        AllowSorting="True" AllowMultiRowSelection="True" AllowPaging="True" ShowGroupPanel="True"
        AutoGenerateColumns="False" GridLines="none"  EnableLinqExpressions = "true" OnItemCommand="OnItemCommand_Fired">
        <PagerStyle Mode="NextPrevNumericAndAdvanced"></PagerStyle>
        <MasterTableView CommandItemDisplay = "Top" ShowHeader = "true"  AutoGenerateColumns = "false" AllowFilteringByColumn="True"
        DataSourceID = "LinqDataSourceLieferscheine" GroupLoadMode = "Client" GroupsDefaultExpanded = "false">
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
                 <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerLocation" HeaderText="Standort"
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
   <telerik:RadButton runat = "server" ID = "LieferungButton" Text ="Lieferscheine erstellen" OnClick = "LieferItems_Selected"> </telerik:RadButton>
   <telerik:RadTextBox runat="server" Enabled = "false" Visible = "false" DisabledStyle-ForeColor = "Red" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Bitte wählen Sie die Aufträge" ID = "BitteTextBox" Width = "240px" ></telerik:RadTextBox>
    <asp:Label ID = "AllesIstOkeyLabelLieferschein" runat = "server" Visible = "false" ForeColor = "Green" Text = "Lieferschein erfolgreich erstellt!"></asp:Label>  
    <asp:Label ID = "ErrorLabelLieferschein" runat = "server" Visible = "false" ForeColor = "Red" Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator."></asp:Label>         
           <telerik:RadTextBox runat="server" Enabled = "false" Visible = "false" DisabledStyle-ForeColor = "Red" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Sie haben ja gesagt. Drücke die Lieferscheine." ID = "AuswahlGetroffenBox" Width = "240px" ></telerik:RadTextBox>
            <telerik:RadTextBox runat="server" Enabled = "false" Visible = "false" DisabledStyle-ForeColor = "Red" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Sie haben nein gesagt. " ID = "NichtFortfahrenBox" Width = "240px" ></telerik:RadTextBox>
<asp:LinqDataSource TableName = "Customer" ID="LinqDataSourceLieferscheine" runat="server" OnSelecting="LieferscheineLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
</asp:Panel>
<asp:Panel runat = "server" ID = "OffenePanel" Visible = "false">
<asp:Label ID = "ErrorOffeneLabel" runat = "server" ForeColor = "Red"></asp:Label>
<br />
<asp:Label ID = "OffeneLabel" runat = "server" Visible = "false" Text = "Hier sind alle noch nicht geschlossene Aufträge: " Font-Size = "Larger"></asp:Label>
<br />
 <telerik:RadGrid ID="NochOffenAuftraegeRadGrid" OnItemCommand = "FertigstellenButton_Clicked" Width ="1450px" Enabled = "false" OnEditCommand = "FertigstellenButton_Clicked" DataSourceID="LinqDataSourceLieferscheineOffene" runat="server" PageSize="15"
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
   <asp:Label runat = "server" ForeColor = "Green" ID = "AllesIsOkeyBeiOffene" Visible = "false" Text = "Ausgewählten Auftrag ist erfolgreich abgeschlossen!"></asp:Label>
</asp:Panel>
 <telerik:RadAjaxPanel runat = "server" ID = "WindowAjaxPanel">
 <telerik:RadWindowManager runat = "server" ID = "WindowManager1"  EnableViewState = "false"   DestroyOnClose = "true" VisibleOnPageLoad = "false" ReloadOnShow = "true"></telerik:RadWindowManager>
 <telerik:RadWindow ShowContentDuringLoad = "false" runat="server"  Height = "350" Width = "400" ID="AddAdressRadWindow" Modal="true" DestroyOnClose = "true" ReloadOnShow = "true">
          <ContentTemplate>
          <asp:UpdatePanel ID="Updatepanel1" runat="server" UpdateMode="Conditional">
          <ContentTemplate>
          <asp:Label runat = "server" ID = "LocationLabelWindow"></asp:Label>
               <div class="contButton">
                <asp:Label runat = "server" ID = "StreetLabel" Text = "Straße: " Width = "130"></asp:Label>
                    <asp:TextBox ID="StreetTextBox" runat="server" Width = "230"></asp:TextBox>
                    <br />
                    <asp:Label runat = "server" ID = "Label1" Text = "Nummer: " Width = "130"></asp:Label>
                    <asp:TextBox ID="StreetNumberTextBox" runat="server" Width = "230"></asp:TextBox>
                    <br />
                    <asp:Label runat = "server" ID = "Label2" Text = "ZIP: " Width = "130"></asp:Label>
                    <asp:TextBox ID="ZipcodeTextBox" runat="server" Width = "230"></asp:TextBox>
                    <br />
                    <asp:Label runat = "server" ID = "Label3" Text = "Stadt: " Width = "130"></asp:Label>
                    <asp:TextBox ID="CityTextBox" runat="server" Width = "230"></asp:TextBox>
                    <br />
                    <asp:Label runat = "server" ID = "Label4" Text = "Land: " Width = "130"></asp:Label>
                    <asp:TextBox ID="CountryTextBox" runat="server" Width = "230"></asp:TextBox> 
                    <br />
                    <asp:Label runat = "server" ID = "Label5" Text = "Empfänger: " Width = "130"></asp:Label>
                     <asp:TextBox ID="InvoiceRecipient" runat="server" Width = "230"></asp:TextBox>  
                     <br />    
               </div>
               <br />  
               <br />  
               <div class="contButton">
                    <asp:Button ID="Button1" Text="Speichern" runat="server" OnClick = "OnAddAdressButton_Clicked">
                    </asp:Button>
               </div>
               <asp:Label ID = "AllesIstOkeyLabel" runat = "server" Text = ""></asp:Label>               
          </ContentTemplate>
          </asp:UpdatePanel>
          </ContentTemplate>
     </telerik:RadWindow>
      </telerik:RadAjaxPanel>
<asp:HiddenField runat = "server" ID = "LocationIdHiddenField"/>
<asp:HiddenField runat = "server" ID = "userAuswahl"/>
<asp:LinqDataSource TableName = "Customer" ID="LinqDataSourceLieferscheineOffene" runat="server" OnSelecting="LieferscheineOffeneLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>