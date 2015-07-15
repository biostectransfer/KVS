<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OffenNeuzulassung.ascx.cs" Inherits="KVSWebApplication.Auftragsbearbeitung_Neuzulassung.OffenNeuzulassung" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

 <telerik:RadCodeBlock ID="RadCodeBlock1" runat = "server">
<script type="text/javascript">


    function MyValueChanging(sender, args) {
        args.set_newValue(args.get_newValue().toUpperCase());
    }


    function openRadWindowPos() {
        $find("<%=RadWindow_Product.ClientID %>").show();
    }

    function keyPress(sender, args) {
        var text = sender.get_value() + args.get_keyCharacter();
        if (!text.match('^[0-9]+$'))
            args.set_cancel(true);
    }
    function RowSelecting(sender, args) {

        if (args.get_tableView().get_name() != "Orders") {
            args.set_cancel(true);
        }

    }


    function GoToToday() {
        var datepicker = $find("<%=ZulassungsDatumPicker.ClientID%>");
        var dt = new Date();
        datepicker.set_selectedDate(dt);
        datepicker.hidePopup();
    }

</script>
</telerik:RadCodeBlock>

<style type="text/css">
.uppercase
{
    text-transform: uppercase;
}


    .RadPicker{display: inline-block !important;}
    * + html .RadPicker{display: inline !important;}
    * html .RadPicker{display: inline !important;}

</style>

             



<asp:Panel runat = "server" ID = "Panel5">

  <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Sofort- oder Großkunde: " ID = "RadTextBox2" Width = "240px" ></telerik:RadTextBox>
                <telerik:radcombobox id="RadComboBoxCustomerOffenNeuzulassung" runat="server" OnSelectedIndexChanged = "SmallLargeCustomerIndex_Changed"
                OnItemsRequested ="SmallLargeCustomerIndex_Changed"  AutoPostBack = "true" Width="250px"> 
                <Items>   
                    <telerik:RadComboBoxItem runat="server" Value = "2" Text="Großkunden" />  
                    <telerik:RadComboBoxItem runat="server" Value = "1" Text="Sofortkunden" />   
 
                </Items>
                 </telerik:radcombobox>

        
                 <asp:ImageButton ID="go" CssClass="infoState" OnClientClick="return false;"  style="cursor:pointer; width:35px; margin-top:15px;"  runat="server" ImageUrl="../Pictures/achtung.gif"  />
                 <telerik:RadToolTip ManualClose="true" ManualCloseButtonText="Schließen"    
                                         ID="ttOpenOrders" runat="server"    ShowEvent="OnClick"
                                          TargetControlID="go" Animation="Slide" 
                                          RelativeTo="Element"  Position="BottomRight">
                        Aktuell sind noch <asp:Label ID="ordersCount" runat="server"></asp:Label> Auftr&auml;ge offen.
                                    </telerik:RadToolTip>


                 <br />
                
                <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" 
                BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Bitte wählen Sie einen Kunden aus: " ID = "RadTextBox1" Width = "240px" ></telerik:RadTextBox>
                <telerik:RadComboBox  Height="300px" Width="250px"  Enabled = "true" 
                DataSourceID = "CustomerDataSource" Filter="Contains" runat = "server"  
                DropDownWidth="515px" EmptyMessage="Wählen Sie einen Kunden aus" AutoPostBack="true"
                DataTextField = "Name"  DataValueField = "Value" ID = "CustomerDropDownListOffenNeuzulassung" 
                 OnSelectedIndexChanged="CustomerIndex_Changed" >
                 
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
                                                  <%# DataBinder.Eval(Container, "DataItem.Name")%>
                                             </td>
                                        </tr>
                                   </table>
                              </ItemTemplate>
                 </telerik:RadComboBox >
                <br />
                   <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" 
                BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Zusätzliche Optionen: " ID = "RadTextBox3" Width = "240px" ></telerik:RadTextBox>
                <asp:Button runat = "server" ID = "ShowAllButton1" OnClick = "ShowAllButton1_Click" Text = "Alles anzeigen"></asp:Button>
                <asp:Button runat = "server" ID = "NewPositionButton" Text = "Neue Position hinzufügen" OnClientClick="openRadWindowPos(); return false;" />
                <asp:Button runat="server" ID = "StornierenButton" OnClick = "StornierenButton_Clicked" Text = "Auftrag stornieren"/>
                 
                <br />
                 <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" 
                BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Zulassungsdatum: " ID = "RadTextBox4" Width = "240px" ></telerik:RadTextBox>
                <telerik:RadDatePicker  DateInput-ButtonsPosition = "Right" runat = "server" ID = "ZulassungsDatumPicker" >
                  <Calendar ID="Calendar1" runat = "server">
                       <FooterTemplate> 
                                    <div style="width: 100%; text-align: center; background-color: Gray;"> 
                                        <input id="Button1" type="button" value="Heute" onclick="GoToToday()" /> 
                                    </div> 
                       </FooterTemplate> 
                    </Calendar>
                 </telerik:RadDatePicker>
                
                <asp:Button runat = "server" AutoPostBack = "true"  ID = "ZulassungsstelleLieferscheineButton"  Text = "Laufzettel erstellen" OnClick = "ZulassungsstelleLieferscheineButton_Clicked"></asp:Button>
            
            
              <asp:Label runat = "server" ID = "LieferscheinePath" Visible = "false"></asp:Label>

                <br />
                <asp:Label runat = "server" ID = "ZulassungErrLabel" Text = "Sie haben keinen Auftrag ausgewählt!" ForeColor = "Red" Visible = "false"></asp:Label>
<asp:Label Visible = "false" Id = "ZulassungOkLabel" Text="Ausgewählter Auftrag ist erfolgreich bearbeitet!" ForeColor = "Green" runat="server" />
                     <br />     
               <telerik:RadFormDecorator runat = "server" ID = "Zulassungsdekorator"/>
                                
                <telerik:RadGrid OnEditCommand = "EditButton_Clicked" OnDetailTableDataBind="RadGridZulOffen_DetailTableDataBind"  AutoGenerateColumns="false" ID="RadGridOffNeuzulassung" 
                DataSourceId = "LinqDataSourceAbmeldung"  
                AllowFilteringByColumn="True" AllowSorting="True" PageSize="15" OnItemCommand = "OnItemCommand_Fired" OnPreRender="RadGridRadGridOffNeuzulassung_PreRender"
                ShowFooter="True" AllowPaging="True" Enabled = "true" ShowHeader = "true" runat="server" GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "true" 
             
                >

                <pagerstyle mode="NextPrevAndNumeric"></pagerstyle>
                <groupingsettings casesensitive="false" ></groupingsettings>

             <mastertableview  CommandItemDisplay = "Top" autogeneratecolumns="false" Name="Orders" allowfilteringbycolumn="True"  ShowHeader = "true" showfooter="True" tablelayout="Auto" >
              <DetailTables>
              <telerik:GridTableView Name="OrderItems"  AllowFilteringByColumn = "false"   EditMode = "PopUp">            
                            <Columns>                         
                                <telerik:GridBoundColumn ReadOnly = "true"  UniqueName = "ItemIdColumn" SortExpression="OrderItemId" HeaderText="OrderItemId" HeaderButtonType="TextButton"
                                    DataField="OrderItemId" Display = "false" Visible = "true" ForceExtractValue = "always">
                                </telerik:GridBoundColumn>
                                   
                             <telerik:GridBoundColumn  HeaderStyle-Width = "100px" ItemStyle-Width="200px" SortExpression="ProductName" HeaderText="Produktname" HeaderButtonType="TextButton"
                                    DataField="ProductName">
                                </telerik:GridBoundColumn>

                               <telerik:GridTemplateColumn UniqueName="ColumnPrice" SortExpression="Amount" HeaderText="Preis" HeaderStyle-Width="95px">
                                                <itemtemplate>
                                                     <telerik:RadTextBox  Width = "75px" ID="tbEditPrice" Text= '<%# Bind( "Amount") %>'  runat = "server">                                                  
                                                      <%--  <ClientEvents OnKeyPress="keyPress" /> --%>
                                                    </telerik:RadTextBox>                                             
                                                </itemtemplate>
                               </telerik:GridTemplateColumn>
                                  <telerik:GridTemplateColumn UniqueName="AuthCharge" SortExpression="AuthCharge" HeaderText="Amtliche Gebühren" HeaderStyle-Width="95px">
                                                <itemtemplate>
                                                     <telerik:RadTextBox  Width = "75px" ID="tbAuthChargePrice" Text= '<%# Bind("AuthCharge") %>'   Visible='<%# Bind("AmtGebuhr") %>'    runat = "server">                                                  
                                                      <%--  <ClientEvents OnKeyPress="keyPress" /> --%>
                                                    </telerik:RadTextBox>                                             
                                                </itemtemplate>
                               </telerik:GridTemplateColumn>
                                  <telerik:GridBoundColumn HeaderStyle-Width = "100px" Display="false" Visible="true" ForceExtractValue="Always"
                                    DataField="AuthChargeId"  UniqueName="AuthChargeId">
                                </telerik:GridBoundColumn>

                             <telerik:GridButtonColumn HeaderStyle-Width = "100px" ButtonType = "PushButton" Text = "Preis setzen" UniqueName = "AmtGebSetzenButton" Visible = "true" CommandName = "AmtGebuhrSetzen">
                             </telerik:GridButtonColumn>
                             
                                 <telerik:GridButtonColumn HeaderStyle-Width = "100px" ButtonType = "PushButton" Text = "Auftragsposition löschen" 
                                 UniqueName = "RemoveOrderItem"  CommandName = "RemoveOrderItem"    >
                             </telerik:GridButtonColumn>
                           
                            <%--<telerik:GridEditCommandColumn UniqueName = "EditDetailsColumn"  ButtonType = "ImageButton" > </telerik:GridEditCommandColumn>--%>

                               <%-- <telerik:GridBoundColumn  SortExpression="Amount" HeaderText="Betrag" HeaderButtonType="TextButton"
                                    DataField="Amount">
                                </telerik:GridBoundColumn>--%>
                               
                                <telerik:GridBoundColumn Display = "false" SortExpression="AmtGebuhr" HeaderText="Amtliche Gebühr" HeaderButtonType="TextButton"
                                    DataField="AmtGebuhr">
                                </telerik:GridBoundColumn>
                                 <telerik:GridBoundColumn Display = "false" SortExpression="AmtGebuhr2" HeaderText="Amtliche Gebühr" HeaderButtonType="TextButton"
                                    DataField="AmtGebuhr2">
                                </telerik:GridBoundColumn>
                            </Columns>
                             <%-- <EditFormSettings EditFormType="Template" CaptionFormatString = "Position bearbeiten" >
                                <FormTemplate>
                                <table id="Table1" cellspacing="1" cellpadding="1" width="250" border="0">
                                    <asp:TextBox ID="OrderItemId" Visible = "false" Text='<%# Bind( "OrderItemId") %>' runat="server"></asp:TextBox>
                                    <tr>
                                        <td>
                                         Betrag:
                                        </td>
                                        <td>
                                        <asp:TextBox ID="amountBox" Visible = "true" Text='<%# Bind( "Amount") %>' runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                       <tr>
                                        <td>
                                         Produktname:
                                        </td>
                                        <td>
                                        <asp:TextBox ID="productNameBox" Visible = "true" Text='<%# Bind( "ProductName") %>' runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td>
                                          Amtliche Gebühr:
                                        </td>
                                        <td>
                                         <asp:CheckBox runat="server" ID = "amtGebCheckBox" Enabled = "false" Visible = "true" Checked = '<%# Bind( "AmtGebuhr") %>' />
                                        </td>
                                    </tr> 
                                    <tr>
                                    <td>
                                      <asp:Button ID="PositionUpdaten" Text="Aktualisieren" runat="server" OnClick = "UpdatePosition_Command">
                                      </asp:Button>&nbsp;
                                    </td>
                                     <td>
                                     <asp:Button ID="Button2" Text="Schließen" runat="server" CausesValidation="False" CommandName="Cancel" >
                                      </asp:Button> 
                                    </td>
                                    </tr>                              
                                </table>                             
                                </FormTemplate>
                              </EditFormSettings> --%>
                        </telerik:GridTableView>            
            </DetailTables>
            <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="true"  />
               <Columns>   
                 <telerik:GridEditCommandColumn ButtonType="PushButton" EditText = "Ändern" UniqueName = "EditOffenColumn"  />                        
                                      
                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderId" HeaderText="OrderId"
                    SortExpression="OrderId" Display = "false" UniqueName="OrderId" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>   

                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="locationId" HeaderText="locationId"
                    SortExpression="locationId" Display = "false" UniqueName="locationId" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
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

                <telerik:GridBoundColumn FilterControlWidth="90px" DataField="Ordernumber" HeaderText="Auftragsnummer"
                    SortExpression="Ordernumber" UniqueName="Ordernumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn FilterControlWidth="90px" DataField="Status" HeaderText="Auftragsstatus"
                    SortExpression="Status" UniqueName="Status" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

                 <telerik:GridDateTimeColumn DataField="CreateDate" HeaderText="Erstellungsdatum" FilterControlWidth="105px" 
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

                  <telerik:GridBoundColumn FilterControlWidth="70px" DataField="HSN" HeaderText="HSN"
                    SortExpression="HSN" UniqueName="HSN" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

                  <telerik:GridBoundColumn FilterControlWidth="70px" DataField="TSN" HeaderText="TSN"
                    SortExpression="TSN" UniqueName="TSN" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderTyp" HeaderText="Auftragstyp"
                    SortExpression="OrderTyp" UniqueName="OrderTyp" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>     
                
                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Freitext" HeaderText="Freitext"
                    SortExpression="Freitext" UniqueName="Freitext" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>     
                
                   <telerik:GridBoundColumn Display = "false" FilterControlWidth="105px" DataField="Geprueft" HeaderText="Geprüft"
                    SortExpression="Geprueft" UniqueName="Geprueft" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>    
                
                   <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Datum" HeaderText="Zulassungsdatum"
                    SortExpression="Datum" UniqueName="Datum" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>               
              </Columns>

              
               <EditFormSettings InsertCaption="Auftrag abmelden"  EditColumn-HeaderText="Dieses Auftrag abmelden?" EditFormType="Template" PopUpSettings-Modal="true">
                <FormTemplate>
                <br />
                 <asp:Label runat = "server" ID = "WellcomeNeuzulassungLabel" Font-Bold = "true" Font-Size = "Larger" Text = "Hier können Sie die Daten überprüfen und den Auftrag für die Zulassungsstelle bereitstellen oder als Fehler markieren."></asp:Label>                                              
                  <table id="Table1" cellspacing="1" cellpadding="1" width="250" border="0">
                        <tr>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <asp:TextBox ID="orderIdBox" Visible = "false" Text='<%# Bind( "OrderId") %>' runat="server"> </asp:TextBox>
                         <asp:TextBox ID="customerIdBox" Visible = "false" Text='<%# Bind( "customerID") %>' runat="server">   </asp:TextBox>
                             
                        <tr>
                            <td>
                                Kundenname &nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="CustomerNameBox"  ReadOnly = "true" Enabled = "true" Text='<%# Bind( "CustomerName") %>' runat="server">
                                </asp:TextBox>
                            </td>
                            <td>
                            </td>
                             
                              <td>
                                Fehler 
                            </td>
                            <td>
                            <asp:CheckBox runat = "server" ID = "ErrorCheckBox"  AutoPostBack = "false"/> <%--OnCheckedChanged = "ErrorCheckBox_Clicked"--%>
                            </td>
                              <td>
                              Ursache
                            </td>
                            <td>
                             <asp:TextBox runat = "server" ID = "ErrorReasonTextBox"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Auftragsnummer &nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="OrderNumberBox"  ReadOnly = "true" Enabled = "true" Text='<%# Bind( "OrderNumber") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                          <tr>
                            <td>
                                Status &nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="StatusBox"  ReadOnly = "true" Enabled = "true" Text='<%# Bind( "Status") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Erstellungsdatum &nbsp;
                            </td>
                            <td>
                                <telerik:RadDateTimePicker Calendar-Enabled = "false" TimePopupButton-Enabled = "false" ReadOnly = "true" Enabled = "true" ID = "CreateDateBox" runat = "server" SelectedDate = '<%# Bind( "CreateDate") %>'></telerik:RadDateTimePicker>          
                            </td>
                        </tr>
                         <tr>
                            <td>
                                Standort &nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="LocationBox" ReadOnly = "true" Enabled = "true" Text='<%# Bind( "CustomerLocation") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                          <tr>
                            <td>
                                Kennzeichen &nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="KennzeichenBox" CssClass="uppercase"   Text='<%# Bind( "Kennzeichen") %>' runat="server">
                                <%--<ClientEvents OnValueChanging="MyValueChanging" />--%>
                                </asp:TextBox>
                               <%--  <asp:RequiredFieldValidator
                                ID="TextBoxRequiredFieldValidator"
                                Runat="server"
                                ForeColor = "Red"
                                Display="Dynamic"
                                ControlToValidate="KennzeichenBox"
                                ErrorMessage="Kennzeichen kann nicht leer sein!" >
                            </asp:RequiredFieldValidator>--%>
                            </td>
                        </tr>
                           <tr>
                            <td>
                                FIN &nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="VINBox" CssClass="uppercase"  Text='<%# Bind( "VIN") %>' runat="server">
                                <ClientEvents OnValueChanging="MyValueChanging" />
                                </asp:TextBox>
                             <%--   <asp:RequiredFieldValidator
                                ID="RequiredFieldValidator1"
                                Runat="server"
                                ForeColor = "Red"
                                Display="Dynamic"
                                ControlToValidate="VINBox"
                                ErrorMessage="FIN kann nicht leer sein!" >
                            </asp:RequiredFieldValidator>--%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                HSN &nbsp;
                            </td>
                           <td>
                           <asp:TextBox  AutoPostBack = "false" runat = "server" Text='<%# Bind( "HSN") %>' ID = "HSNAbmBox">
                           <ClientEvents OnValueChanging="MyValueChanging" />
                           </asp:TextBox>
                            <%--<asp:RequiredFieldValidator
                                ID="RequiredFieldValidator2"
                                Runat="server"
                                ForeColor = "Red"
                                Display="Dynamic"
                                ControlToValidate="HSNAbmBox"
                                ErrorMessage="HSN kann nicht leer sein!" >
                            </asp:RequiredFieldValidator>--%>
                             <asp:Label runat = "server" Enabled = "true" Visible = "false" Text = "" ID = "HSNSearchLabel"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                TSN &nbsp;
                            </td>
                            <td>
                            <asp:TextBox runat = "server" Text='<%# Bind( "TSN") %>' ID = "TSNAbmBox">
                            <ClientEvents OnValueChanging="MyValueChanging" />
                            </asp:TextBox>
                            <%--<asp:RequiredFieldValidator
                                ID="RequiredFieldValidator3"
                                Runat="server"
                                ForeColor = "Red"
                                Display="Dynamic"
                                ControlToValidate="TSNAbmBox"
                                ErrorMessage="TSN kann nicht leer sein!" >
                            </asp:RequiredFieldValidator>--%>
                            </td>
                        </tr>                     

                        <tr>

                       <br />
                        <br />
                            <td align="right" colspan="2">
                            <p>
                                <asp:Button ID="ZulassenButton" Text="Änderung speichern" runat="server" OnClick = "ZulassungZulassen_Command" >
                                </asp:Button>&nbsp;
                                <asp:Button ID="Button12" Text="Schließen" runat="server" CausesValidation="False" CommandName="Cancel" >
                                </asp:Button>
                                </p>
                            </td>
                        </tr>
                    </table>
                </FormTemplate>
            </EditFormSettings>      
            

            </mastertableview>
            <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
            <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />
             
              <clientsettings>
               <ClientEvents  OnRowSelecting="RowSelecting" />  
            <Scrolling AllowScroll="false" ></Scrolling>
             <Selecting AllowRowSelect="true" ></Selecting>
        </clientsettings>
    </telerik:RadGrid>

    <asp:HiddenField runat = "server" ID = "MakeHiddenField"> </asp:HiddenField>
    <asp:HiddenField runat = "server" ID = "itemIndexHiddenField"> </asp:HiddenField>

    <telerik:RadFormDecorator runat = "server" ID = "OffenNeuzulassungFormDekorator" DecoratedControls="all"/>
<telerik:RadWindow Title = "Neue Position hinzufügen" runat="server" Width="500px" Height="300px" ID="RadWindow_Product" Modal="true">
       
        <ContentTemplate>

                Wählen Sie bitte neues Produkt aus
                    <br />
           <%-- <asp:DropDownList DataTextField = "Name" DataValueField = "Value" DataSourceID = "ProductDataSource" runat = "server" ID = "NewProductDropDownList" > </asp:DropDownList>--%>


            
 <telerik:RadComboBox  Height="300px" Width="400px"   Enabled = "true" 
                DataSourceID = "ProductDataSource" AutoPostBack = "false" Filter="Contains" runat = "server"  DropDownWidth="515px" EmptyMessage = "Produkt..." HighlightTemplatedItems="true"
                DataTextField = "Name" DataValueField = "Value" ID = "NewProductDropDownList" 
 >
                 
                     <HeaderTemplate>
                                   <table style="width: 515px" cellspacing="0" cellpadding="0">
                                        <tr align="center">
                                            <td style="width: 90px;">
                                                  Produktnummer
                                             </td>
                                             <td style="width: 175px;">
                                                 Produktname
                                             </td>
                                         
                                             <td style="width: 250px">
                                                  Warengruppe
                                             </td>
                                        </tr>
                                   </table>
                              </HeaderTemplate>
                                 <ItemTemplate>
                                   <table style="width: 515px;" cellspacing="0" cellpadding="0">
                                        <tr>
                                             <td style="width: 110px;">
                                                  <%# DataBinder.Eval(Container, "DataItem.ItemNumber")%>
                                             </td>
                                              <td style="width: 175px;">
                                                  <%# DataBinder.Eval(Container, "DataItem.Name")%>
                                                <%--   DataBinder.Eval(Container, "DataItem.AccountNumber")--%>
                                             </td>
                                             <td style="width: 250px;">
                                                  <%# DataBinder.Eval(Container, "DataItem.Category")%>
                                             </td>
                                        </tr>
                                   </table>
                              </ItemTemplate>
                 </telerik:RadComboBox >

            <%--    <br />

                Wählen Sie bitte die Kostenstelle aus
                 <br />
            <asp:DropDownList DataTextField = "Name" DataValueField = "Value" Width="400px" DataSourceID = "CostCenterDataSource" runat = "server" ID = "CostCenterDropDownList" >
             </asp:DropDownList>--%>
            <br />
                Und bestätigen:
            <div class="contButton">
                <asp:Button ID="NewPositionButtonHinzuguegen" Text="Neue Position hinzufügen" runat="server" OnClick = "NewPositionButton_Clicked">
                </asp:Button>
                <asp:Button ID="Button2" Text="Schließen" runat="server" CausesValidation="False" CommandName="Cancel" >
                                </asp:Button>
            </div>
               
        </ContentTemplate>
    </telerik:RadWindow>

    

<br />

<asp:Label runat = "server" ID = "StornierungErfolgLabel" Text = "Auftrag ist erfolgreich storniert" Visible = "false" ForeColor = "Green"></asp:Label>
<br />

<asp:LinqDataSource TableName = "Customer" ID="LinqDataSourceAbmeldung" runat="server" OnSelecting="AbmeldungenLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>

<asp:LinqDataSource ID="CustomerDataSource" runat="server" OnSelecting="CustomerLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>

<asp:LinqDataSource ID="ProductDataSource" runat="server" OnSelecting="ProductLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>

<%--<asp:LinqDataSource ID="CostCenterDataSource" runat="server" OnSelecting="CostCenterDataSourceLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
</asp:LinqDataSource>--%>
</asp:Panel>