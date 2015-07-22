<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Fehlerhaft.ascx.cs" Inherits="KVSWebApplication.Nachbearbeitung_Abmeldung.Fehlerhaft" %>

<telerik:RadCodeBlock ID="RadCodeBlock1" runat = "server">
<script type="text/javascript">
    function RowDblClick(sender, eventArgs) {
        sender.get_masterTableView().editItem(eventArgs.get_itemIndexHierarchical());
    } 
</script>
</telerik:RadCodeBlock>
<asp:Panel runat = "server" ID = "Panel1">
    <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Sofort- oder Großkunde: " ID = "RadTextBox2" Width = "240px" ></telerik:RadTextBox>
    <telerik:radcombobox id="RadComboBoxCustomer" runat="server" 
     OnSelectedIndexChanged = "SmallLargeCustomerIndex_Changed"  Width="250px"   AutoPostBack = "true" > 
    <Items>   
        <telerik:RadComboBoxItem runat="server" Value = "2" Text="Großkunden" /> 
        <telerik:RadComboBoxItem runat="server" Value = "1" Text="Sofortkunden" />    
    </Items>
        </telerik:radcombobox>
        <br />
    <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Bitte wählen Sie einen Kunden aus: " ID = "RadTextBox1" Width = "240px" ></telerik:RadTextBox>
       <telerik:RadComboBox  Height="300px" Width="250px"  Enabled = "true"  AutoPostBack="true"
    Filter="Contains" runat = "server"
        DropDownWidth="515px"  HighlightTemplatedItems="true"  EmptyMessage = "Bitte wählen Sie einen Kunden aus: "
    DataTextField = "Name" DataValueField = "Value" ID = "CustomerDropDownList"
        OnSelectedIndexChanged="CustomerIndex_Changed"  DataSourceID="CustomerDataSource">                 
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
        <asp:Button runat = "server" Text = "X" 
            ID = "clearButton" OnClick="clearButton_Click" ToolTip="Auswahl löschen" > </asp:Button>
    <br />
    <br />
     <asp:Label ID="FehlerhaftErrorMessage" ForeColor="Red" runat="server"></asp:Label>
     <telerik:RadGrid AutoGenerateColumns="false" ID="RadGridFehlerhaft" DataSourceId = "LinqDataSourceFehlerhaft" 
                AllowFilteringByColumn="True" AllowSorting="True" PageSize="15" EnableViewState ="false"
                ShowFooter="True" AllowPaging="True"  runat="server" GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "false" >
                <pagerstyle mode="NextPrevAndNumeric"></pagerstyle>
                <groupingsettings casesensitive="false" ></groupingsettings>
             <mastertableview CommandItemDisplay = "Top" ShowHeader = "true" autogeneratecolumns="false" allowfilteringbycolumn="True" showfooter="True" tablelayout="Auto" EditMode = "PopUp">
             <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="true"  />
               <Columns>
                <telerik:GridEditCommandColumn ButtonType="PushButton" EditText = "Auftrag bearbeiten" UniqueName = "EditOffenColumn"  />                        
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
                 <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Freitext" HeaderText="Freitext"
                    SortExpression="Freitext" UniqueName="Freitext" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>          
                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="ErrorReason" HeaderText="Fehler"
                    SortExpression="ErrorReason" UniqueName="ErrorReason" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>
              </Columns>
            <EditFormSettings PopUpSettings-Width = "1000" CaptionFormatString = "Here können Sie alle Daten von dem Auftrag {0} bearbeiten und speichern" CaptionDataField="Ordernumber" InsertCaption="Status ändern" FormMainTableStyle-Width = "1000" FormCaptionStyle-Width = "500" EditColumn-ItemStyle-Width = "500"  EditColumn-HeaderText="Status ändern: " EditFormType="Template" PopUpSettings-Modal="true">
                <FormTemplate>
                 <table width = "1000">                            
                 <tr>
<td>   
<asp:Panel runat = "server" ID = "FahrzeugPanel" >   
    <asp:Label runat = "server" Text = "Auftragsfehler: "></asp:Label>
    <asp:Label ID="EditFormErrorLabel" Text='<%# Bind( "ErrorReason") %>' runat="server" > </asp:Label>
    <br />
    <br />
    <asp:Label Text="Fahrzeug" ID = "FahrzeugLabel" runat="server" />
    <br />
   <telerik:RadTextBox Text='<%# Bind( "OrderNumber") %>' Visible = "false" ID="OrderIdBox" runat="server">
   </telerik:RadTextBox>  
   <telerik:RadTextBox Text='<%# Bind( "locationId") %>'  Visible = "false" ID="LocationIdBox" runat="server">
   </telerik:RadTextBox>  
   <telerik:RadTextBox ID="VINLabel" runat="server" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black"  Enabled="false" Text="FIN: " Visible="True" Width = "140">
   </telerik:RadTextBox>
   <telerik:RadTextBox Text='<%# Bind( "VIN") %>'  AutoPostBack = "true" ID="VINBox" runat="server">
   </telerik:RadTextBox>   
   <telerik:RadTextBox ID="VariantLabel" runat="server" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black"  Enabled="false" Text="Variant: " Visible="True" Width="140px">
   </telerik:RadTextBox>
   <telerik:RadTextBox Text='<%# Bind( "Variant") %>'  ID="VariantBox" runat="server">
   </telerik:RadTextBox> 
    <telerik:RadTextBox ID="LicenceNumLabel" runat="server" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black" Enabled="false" Text="Licence Number: " Visible="True" Width="140px">
    </telerik:RadTextBox>
    <telerik:RadTextBox  Text='<%# Bind( "Kennzeichen") %>' ID="LicenceBox" runat="server">
    </telerik:RadTextBox>
    <br />
    <telerik:RadTextBox ID="PreviousLicNumLabel" runat="server" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black" Enabled="false" Text="Previuos Licence Number: " Visible="True" Width="140px">
    </telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "Prevkennzeichen") %>' ID="PreviousLicenceBox" runat="server">
    </telerik:RadTextBox>
<telerik:RadTextBox ID="InspectionLabel" runat="server" BorderColor="Transparent" DisabledStyle-BackColor="Transparent"   DisabledStyle-ForeColor="Black" Enabled="false" Text="General Inspection Date: " Visible="True" Width="140px">
</telerik:RadTextBox>
<telerik:RadDatePicker SelectedDate='<%# Bind( "Inspection") %>' ID="InspectionDatePicker" runat="server">
 </telerik:RadDatePicker>
 <br />
    <telerik:RadTextBox ID="TSNLabel" runat="server" BorderColor="Transparent" 
        DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black" 
        Enabled="false" Text="TSN" Width="140px">
    </telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "TSN") %>' runat = "server" ID = "TSNBox"></telerik:RadTextBox>
     <asp:RegularExpressionValidator id="RegExVal" runat="server"
            ControlToValidate="TSNBox"
            ValidationExpression=".{3}.*"
            Display="Static"
            ForeColor = "Red"
            ErrorMessage="TSN ist weniger als 3 Zeichen">
    </asp:RegularExpressionValidator>   
<br />     
    <telerik:RadTextBox ID="HSNLabel" runat="server" BorderColor="Transparent" 
    DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black" 
    Text="HSN"  Enabled="false" Width="140px">
    </telerik:RadTextBox>
   <telerik:RadTextBox Text='<%# Bind( "HSN") %>' OnTextChanged = "HSNBox_TextChanged" AutoPostBack = "true" runat = "server" ID = "HSNBox"></telerik:RadTextBox>
   <br />
   <asp:Label runat = "server" Enabled = "true" Visible = "false" Text = "" ID = "HSNSearchLabel"></asp:Label>
   <br />
    <telerik:RadTextBox ID="InsuranceLabel" runat="server" 
    BorderColor="Transparent" DisabledStyle-BackColor="Transparent" 
    DisabledStyle-ForeColor="Black"  Enabled="false" Text="Insurance " Visible="True" 
    Width="140px">
    </telerik:RadTextBox>
   <telerik:RadTextBox Text='<%# Bind( "eVBNum") %>' runat = "server" ID = "InsuranceBox"></telerik:RadTextBox>    
      <br />
   <telerik:RadTextBox ID="FreiTextLabel" runat="server" BorderColor="Transparent" DisabledStyle-BackColor="Transparent" DisabledStyle-ForeColor="Black"  Enabled="false" Text="Freitext: " Visible="True" Width="140px">
   </telerik:RadTextBox>
   <telerik:RadTextBox Text='<%# Bind( "Freitext") %>'  ID="Freitext" runat="server">
   </telerik:RadTextBox> 
 </asp:Panel>
 </td>
<td>  
<asp:Panel runat = "server" Visible = "true" ID = "Halter">
 <asp:Label Text="Halter" ID = "HalterLabel" runat="server" />
  <br />
    <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Name: " ID = "OwnerNameLabel" Width = "140px" ></telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "Name") %>' runat = "server" ID = "OwnerNameBox" ></telerik:RadTextBox>                           
    <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Vorname: " ID = "OwnerFirstNameLabel" Width = "140px" ></telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "FirstName") %>' runat = "server" ID = "OwnerFirstNameBox" ></telerik:RadTextBox>
    <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Bankname: " ID = "BankNameLabel" Width = "140px" ></telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "BankName") %>' runat = "server" ID = "BankNameBox" ></telerik:RadTextBox>
    <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Accountnummer: " ID = "AccountNumberLabel" Width = "140px" ></telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "AccountNum") %>' runat = "server" ID = "AccountNumberBox" ></telerik:RadTextBox>
    <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Bank Code: " ID = "BankCodeLabel" Width = "140px" ></telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "BankCode") %>' runat = "server" ID = "BankCodeBox" ></telerik:RadTextBox>
</asp:Panel>
</td>
 <td>
<asp:Panel runat = "server" Visible = "true" ID = "Halterdaten">
    <asp:Label Text="Halterdaten" ID = "HalterdatenLabel" runat="server" />
    <br />
    <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Straße: " ID = "OwnerStreetLabel" Width = "140px" ></telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "Street") %>' runat = "server" ID = "OwnerStreetBox" ></telerik:RadTextBox>
    <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Nummer: " ID = "OwnerStreetNubmerLabel" Width = "140px" ></telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "StreetNr") %>' runat = "server" ID = "OwnerStreetNubmerBox" ></telerik:RadTextBox>
    <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Zip: " ID = "OwnerZipCodeLabel" Width = "140px" ></telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "Zip") %>' runat = "server" ID = "OwnerZipCodeBox" ></telerik:RadTextBox>
     <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Stadt: " ID = "OwnerCityLabel" Width = "140px" ></telerik:RadTextBox>
     <telerik:RadTextBox Text='<%# Bind( "City") %>' runat = "server" ID = "OwnerCityBox" ></telerik:RadTextBox>
     <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Land: " ID = "OwnerCountryLabel" Width = "140px" ></telerik:RadTextBox>
     <telerik:RadTextBox Text='<%# Bind( "Country") %>' runat = "server" ID = "OwnerCountryBox" ></telerik:RadTextBox>
</asp:Panel>
</td>
<td> 
<asp:Panel Visible = "true" runat = "server" ID = "Kontaktdaten">
    <asp:Label Text="Kontaktdaten" ID = "KontaktdatenLabel" runat="server" />
    <br />
    <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Telefonnummer: " ID = "OwnerPhoneLabel" Width = "140px" ></telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "Phone") %>' runat = "server" ID = "OwnerPhoneBox" ></telerik:RadTextBox>
    <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Faxnummer: " ID = "OwnerFaxLabel" Width = "140px" ></telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "Fax") %>' runat = "server" ID = "OwnerFaxBox" ></telerik:RadTextBox>
    <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Handynummer: " ID = "OwnerMobilePhoneLabel" Width = "140px" ></telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "Mobile") %>' runat = "server" ID = "OwnerMobilePhoneBox" ></telerik:RadTextBox>
    <telerik:RadTextBox runat="server"  Enabled="false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Email: " ID = "OwnerEmailLabel" Width = "140px" ></telerik:RadTextBox>
    <telerik:RadTextBox Text='<%# Bind( "Email") %>' runat = "server" ID = "OwnerEmailBox" ></telerik:RadTextBox>
</asp:Panel>
 </td>
  </tr>
</table>
<br />
<br />
<asp:Button ID="StatusButton" Text="Auftrag speichern" runat="server" OnClick = "OrderUpdateButton_Clicked">
</asp:Button>
<asp:Button ID="Button2" Text="Cancel" runat="server" CausesValidation="False" CommandName="Cancel" >
</asp:Button>
                </FormTemplate>
            </EditFormSettings>                     
          </mastertableview>           
        <clientsettings>
            <Scrolling AllowScroll="false" ></Scrolling>
             <Selecting AllowRowSelect="false" ></Selecting>      
        </clientsettings>
    </telerik:RadGrid>
    <asp:LinqDataSource TableName = "Customer" ID="LinqDataSourceFehlerHaft" runat="server" OnSelecting="FehlerhaftLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
    </asp:LinqDataSource>
    <asp:LinqDataSource ID="CustomerDataSource" runat="server" OnSelecting="CustomerLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
    </asp:LinqDataSource>
</asp:Panel>
<br />
<br />
<br />