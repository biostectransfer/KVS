<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VersandZulassung.ascx.cs" Inherits="KVSWebApplication.Auftragsbearbeitung_Neuzulassung.VersandZulassung" %> 
 <telerik:RadCodeBlock ID="RadCodeBlock441" runat="server">
    <script type="text/javascript">
        function RequestStart(sender, eventArgs) {
            var eventTarget = eventArgs.get_eventTarget();
            if (eventTarget.indexOf("LieferscheinDruckenButton") != -1) {
                eventArgs.set_enableAjax(false);            
            }
        }     
    </script>
</telerik:RadCodeBlock>
<link type="text/css"  rel="stylesheet" href="../Styles/zulassungstelle.css" />
 <telerik:RadFormDecorator runat = "server" ID = "VersandDecorator"/>
   <asp:Panel ID = "panel11" runat = "server">
   </asp:Panel>
 <telerik:RadGrid  ID="RadGridVersand" OnEditCommand = "PruefenButton_Clicked" OnDetailTableDataBind="OrdersDetailedTabel_DetailTable" 
 Width = "1050" DataSourceID="LinqDataSourceVersand" runat="server" PageSize="5" ShowGroupPanel="false"
        AllowSorting="True" AllowMultiRowSelection="True" AllowPaging="True"  OnPreRender="RadGridVersand_PreRender"
        AutoGenerateColumns="False" GridLines="none" EnableLinqExpressions = "true"  >
        <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
        <MasterTableView CommandItemDisplay = "Top" ShowHeader = "true"  PageSize="5" 
        AutoGenerateColumns = "false" DataSourceID = "LinqDataSourceVersand" GroupLoadMode = "Client" GroupsDefaultExpanded = "false">        
         <DetailTables>
              <telerik:GridTableView Name="Orders" Width="100%" AllowFilteringByColumn = "false"   EditMode = "PopUp">            
                            <Columns>                                                                            
                                <telerik:GridBoundColumn ReadOnly = "true"  UniqueName = "OrderIdColumn" SortExpression="OrderNumber" HeaderText="OrderNumber" HeaderButtonType="TextButton"
                                    DataField="OrderNumber" Display = "false" Visible = "true" ForceExtractValue = "always">
                                </telerik:GridBoundColumn>  
                                 <telerik:GridBoundColumn SortExpression="CustomerName" ItemStyle-Width="200px"  HeaderText="Kundenname" HeaderButtonType="TextButton"
                                    DataField="CustomerName">
                                </telerik:GridBoundColumn>                             
                                <telerik:GridBoundColumn SortExpression="OrderLocation" HeaderText="Standort" HeaderButtonType="TextButton"
                                    DataField="OrderLocation">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="OrderNumber" HeaderText="Auftragsnummer" HeaderButtonType="TextButton"
                                    DataField="OrderNumber">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="Status" HeaderText="Auftragsstatus" HeaderButtonType="TextButton"
                                    DataField="Status">
                                </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn SortExpression="OrderType" HeaderText="Auftragstyp" HeaderButtonType="TextButton"
                                    DataField="OrderType">
                                </telerik:GridBoundColumn>
                                   <telerik:GridBoundColumn SortExpression="OrderError" HeaderText="Fehler" HeaderButtonType="TextButton"
                                    DataField="OrderError">
                                </telerik:GridBoundColumn>
                            </Columns>
                        </telerik:GridTableView>            
             </DetailTables>
             <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="true"  />
             <Columns>
              <telerik:GridBoundColumn FilterControlWidth="105px" DataField="listId" HeaderText="listId"
                    SortExpression="listId" Display = "false" UniqueName="listId" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>                     
                 <telerik:GridBoundColumn FilterControlWidth="105px" DataField="listNumber" HeaderText="Packingnummer"
                    SortExpression="listNumber" UniqueName="listNumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>                       
                 <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerName" HeaderText="Kundenname"
                    SortExpression="CustomerName" UniqueName="CustomerName" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="isPrinted" HeaderText="Gedruckt"
                    SortExpression="isPrinted" UniqueName="isPrinted" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>           
                <telerik:GridTemplateColumn>
                 <HeaderTemplate>   
                Lieferschein          
        </HeaderTemplate>
        <ItemTemplate>        
               <asp:Label ForeColor = "Green" Font-Bold = "true" runat = "server" ID = "TestPathLabel" Text = '<%# Bind( "PostBackUrl") %>'></asp:Label> 
        </ItemTemplate>               
      </telerik:GridTemplateColumn>
               <telerik:GridTemplateColumn>
                 <HeaderTemplate>   
                        Löschen          
                </HeaderTemplate>
                <ItemTemplate> 
                   <asp:Label  Visible = "false" runat = "server" ID = "lbllistId" Text = '<%# Bind( "listId") %>'></asp:Label> 
                    <asp:Button Text = "Lieferschein löschen" runat = "server" ToolTip="Der Lieferschein wird archiviert und die Aufträge in die Zulassungsstelle verschoben" ID = "btnRemovePackingList" OnClick = "btnRemovePackingList_Click"></asp:Button>
                </ItemTemplate>               
             </telerik:GridTemplateColumn>
            </Columns>
                   <NestedViewTemplate>
                   <asp:Panel runat="server" ID="InnerContainer" CssClass="nestedTemplate" Visible="true">   
                 <asp:Label runat = "server" ID = "ErrorVersandGedrucktLabel" ForeColor = "Red" Text = "Der Auftrag ist bereit gedruckt. Bitte aktualisieren Sie die Tabelle." Visible = "false"></asp:Label>
                <br />
                     <asp:Label runat = "server" ID = "AuswahlLabel" Font-Size = "Larger" Text = "Wählen Sie zwischen einer Eigenverbringung oder tragen Sie die Paketnummer des Versanddienstleister ein."></asp:Label>                                      
                 <asp:TextBox ID="listIdBox" Visible = "false" Text='<%# Bind( "listId") %>' runat="server"></asp:TextBox>             
                 <br /><br />
                  <table id="Table1" cellspacing="1" cellpadding="1" border="0">
                       <tr>
                       <td>
                         <asp:Label ID="DispatchOrderNumberLabel" runat = "server" Text = "Paketnummer: "></asp:Label>
                             </td>
                             <td>
                         <telerik:RadTextBox AutoPostBack = "false"  runat = "server" Text='<%# DataBinder.Eval(Container, "DataItem.DispatchOrderNumber")%>'  ID = "DispatchOrderNumberBox"></telerik:RadTextBox>    <%--OnTextChanged = "DispatchOrderNumberBoxText_Changed"  --%>  
                       </td>
                       <td>
                        &nbsp; <asp:Label ID="isSelfDispathLabel" runat = "server" Text = "Eigenverbringung: "></asp:Label>
                        </td>
                        <td>
                       <asp:CheckBox AutoPostBack = "false"  runat = "server" Checked='<%# DataBinder.Eval(Container, "DataItem.IsSelf")%>' ID = "isSelfDispathCheckBox"/><%--OnCheckedChanged = "isSelfDispathCheckBox_Checked"--%>
                       </td>
                       <td>    <asp:Button Text = "Lieferscheine erstellen" runat = "server" ID = "LieferscheinDruckenButton" OnClick = "DrueckenButton_Clicked"></asp:Button>
                        </td>
                    <td></td>
                       </tr>                    
                    </table>
                      <br />
                <telerik:RadGrid AutoGenerateColumns="false" ID="RadGridVersandDetails" 
               AllowSorting="true" AllowFilteringByColumn="false" PageSize="5"  OnNeedDataSource="OrdersDetailedTabel_DetailTableDataBind" 
                 AllowPaging="true" Enabled = "true"  runat="server" GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "false" >                               
                <pagerstyle mode="NextPrevAndNumeric" AlwaysVisible="true"></pagerstyle>
                <groupingsettings casesensitive="false" ></groupingsettings>
                    <mastertableview CommandItemDisplay = "Top" ShowHeader = "true" autogeneratecolumns="false" Name="Orders" showfooter="true" tablelayout="Auto">                    
                     <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="false"  />
                      <Columns>
                          <telerik:GridBoundColumn ReadOnly = "true"  UniqueName = "OrderIdColumn" SortExpression="OrderNumber" HeaderText="OrderNumber" HeaderButtonType="TextButton"
                                    DataField="OrderNumber" Display = "false" Visible = "false" ForceExtractValue = "always">
                                </telerik:GridBoundColumn>  
                                 <telerik:GridBoundColumn SortExpression="CustomerName" HeaderText="Kundenname" HeaderButtonType="TextButton"
                                    DataField="CustomerName">
                                </telerik:GridBoundColumn>                             
                                <telerik:GridBoundColumn SortExpression="OrderLocation" HeaderText="Standort" HeaderButtonType="TextButton"
                                    DataField="OrderLocation">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="OrderNumber" HeaderText="Auftragsnummer" HeaderButtonType="TextButton"
                                    DataField="OrderNumber">
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn SortExpression="Status" HeaderText="Auftragsstatus" HeaderButtonType="TextButton"
                                    DataField="Status">
                                </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn SortExpression="OrderType" HeaderText="Auftragstyp" HeaderButtonType="TextButton"
                                    DataField="OrderType">
                                </telerik:GridBoundColumn>
                                   <telerik:GridBoundColumn SortExpression="OrderError" HeaderText="Fehler" HeaderButtonType="TextButton"
                                    DataField="OrderError">
                                </telerik:GridBoundColumn>
                    </Columns>
                    </mastertableview>
                    </telerik:RadGrid>                   
                         </asp:Panel>
                 </NestedViewTemplate>
          </MasterTableView>
              <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
            <AlternatingItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
        <ClientSettings ReorderColumnsOnClient="True" AllowDragToGroup="True"  AllowColumnsReorder="True">
            <Selecting AllowRowSelect="True"></Selecting>
            <Resizing AllowRowResize="True" AllowColumnResize="True" EnableRealTimeResize="True"
                ResizeGridOnColumnResize="False"></Resizing>
        </ClientSettings>
    </telerik:RadGrid>
    <asp:HiddenField runat = "server" ID = "itemIndexHiddenField"> </asp:HiddenField>
    <asp:HiddenField runat = "server" ID = "myDispathHiddenField"/>
<br />
<asp:Label runat = "server" ID = "ErrorVersandLabel" ForeColor = "Red" Text = "Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut. Wenn das Problem weiterhin auftritt, wenden Sie sich an den Systemadministrator." Visible = "false"></asp:Label>
<asp:LinqDataSource ID="LinqDataSourceVersand" runat="server" OnSelecting="VersandLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>
<telerik:RadAjaxLoadingPanel runat = "server" ID = "VersandLoadingPanel"></telerik:RadAjaxLoadingPanel>

