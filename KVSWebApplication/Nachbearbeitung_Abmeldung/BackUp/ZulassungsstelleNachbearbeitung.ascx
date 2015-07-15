<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ZulassungsstelleNachbearbeitung.ascx.cs" Inherits="KVSWebApplication.Auftragsbearbeitung_Neuzulassung.Zulassungsstelle" %>

            <body>
             <form id="form1">
  
	            <script type="text/javascript">
	                function RowDblClick(sender, eventArgs) {
	                    sender.get_masterTableView().editItem(eventArgs.get_itemIndexHierarchical());
	                }
              </script>


                <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Ist das Large oder Small Customer: " ID = "LrSmKundeTextBox" Width = "240px" ></telerik:RadTextBox>
                <br />
                <telerik:radcombobox id="RadComboCustomer" runat="server"  OnSelectedIndexChanged = "SmLrCustomerIndex_Changed" OnItemsRequested ="SmLrCustomerIndex_Changed"  AutoPostBack = "true" > 
                <Items>                    
                    <telerik:RadComboBoxItem runat="server" Value = "2" Text="LargeCustomer" />   
                    <telerik:RadComboBoxItem runat="server" Value = "1" Text="SmallCustomer" />   
                </Items>
                 </telerik:radcombobox>
                 <br />
                <telerik:RadTextBox runat="server" Enabled = "false" Visible = "True" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Bitte wählen Sie einen Kunde aus: " ID = "KundeRadTextBox" Width = "240px" ></telerik:RadTextBox>
                <br />
                <telerik:RadDropDownList  Enable = "false"  DataTextField = "Name" DataValueField = "Value" DataSourceID = "CustomerDataSource" AutoPostBack = "true" runat = "server" ID = "CustomerZulDropDownList"  OnSelectedIndexChanged="CustomerZulassungIndex_Changed" ></telerik:RadDropDownList>
                <br />


                <telerik:RadGrid AutoGenerateColumns="false" ID="RadGridAbmeldung" DataSourceId = "LinqDataSourceZulassungCust" 
                AllowFilteringByColumn="True" AllowSorting="True" PageSize="15"
                ShowFooter="True" AllowPaging="True" Enabled = "false" OnEditCommand = "Edit_Command" runat="server" GridLines="None" EnableLinqExpressions="false" AllowMultiRowSelection = "false" >

                <pagerstyle mode="NextPrevAndNumeric"></pagerstyle>
                <groupingsettings casesensitive="false" ></groupingsettings>

             <mastertableview autogeneratecolumns="false"  allowfilteringbycolumn="True" showfooter="True" tablelayout="Auto" EditMode = "PopUp">
            
               <Columns>
                 <telerik:GridEditCommandColumn ButtonType="PushButton" EditText = "Fertigstellen" UniqueName = "FertigstellenColumn"  />  

                 <telerik:GridBoundColumn FilterControlWidth="105px" DataField="OrderId" HeaderText="OrderId"
                    SortExpression="OrderId" UniqueName="OrderId" Visible = "false" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerName" HeaderText="CustomerName"
                    SortExpression="CustomerName" UniqueName="CustomerName" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Ordernumber" HeaderText="OrderNumber"
                    SortExpression="Ordernumber" UniqueName="Ordernumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Status" HeaderText="Status"
                    SortExpression="Status" UniqueName="Status" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

                 <telerik:GridDateTimeColumn DataField="CreateDate" HeaderText="CreateDate" FilterControlWidth="95px"
                    SortExpression="CreateDate" UniqueName="CreateDate" PickerType="DatePicker" EnableTimeIndependentFiltering="true">
                </telerik:GridDateTimeColumn>

                 <telerik:GridBoundColumn FilterControlWidth="105px" DataField="CustomerLocation" HeaderText="Location"
                    SortExpression="Location" Visible = "False" UniqueName="Location" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn FilterControlWidth="105px" DataField="VIN" HeaderText="VIN"
                    SortExpression="VIN" UniqueName="VIN" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Model" HeaderText="Model"
                    SortExpression="Model" UniqueName="Model" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Make" HeaderText="Make"
                    SortExpression="Make" UniqueName="Make" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Amount" HeaderText="Amount"
                    SortExpression="Amount" UniqueName="Amount" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

                  <telerik:GridBoundColumn FilterControlWidth="105px" DataField="Product" HeaderText="Product"
                    SortExpression="Product" UniqueName="Product" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains"
                    ShowFilterIcon="false">
                </telerik:GridBoundColumn>

              </Columns>

            <EditFormSettings InsertCaption="Auftrag fertigstellen"  EditColumn-HeaderText="Dieses Auftrag fertigstellen?" EditFormType="Template" PopUpSettings-Modal="true">
                <FormTemplate>
                  <table id="Table1" cellspacing="1" cellpadding="1" width="250" border="0">
                        <tr>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                         <asp:TextBox ID="orderIdBox" Visible = "false" Text='<%# Bind( "OrderId") %>' runat="server">
                                </asp:TextBox>
                        <tr>
                            <td>
                                Customer Name 
                            </td>
                            <td>
                                <asp:TextBox ID="CustomerNameBox" Text='<%# Bind( "CustomerName") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                OrderNumber
                            </td>
                            <td>
                                <asp:TextBox ID="OrderNumberBox" Text='<%# Bind( "OrderNumber") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                          <tr>
                            <td>
                                Status
                            </td>
                            <td>
                                <asp:TextBox ID="StatusBox" Text='<%# Bind( "Status") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                CreateDate
                            </td>
                            <td>
                                <telerik:RadDateTimePicker ID = "CreateDateBox" runat = "server" SelectedDate = '<%# Bind( "CreateDate") %>'></telerik:RadDateTimePicker>          
                            </td>
                        </tr>
                    <%--    <tr>
                            <td>
                                ExecutionDate
                            </td>
                            <td>
                                <telerik:RadDateTimePicker ID = "ExecutionDate" runat = "server" SelectedDate = '<%# Bind( "ExecutionDate") %>'></telerik:RadDateTimePicker>          
                            </td>
                        </tr>--%>
                         <tr>
                            <td>
                                Customer Location
                            </td>
                            <td>
                                <asp:TextBox ID="LocationBox" Text='<%# Bind( "CustomerLocation") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                          <tr>
                            <td>
                                Kennzeichen
                            </td>
                            <td>
                                <asp:TextBox ID="KennzeichenBox" Text='<%# Bind( "Kennzeichen") %>' runat="server">
                                </asp:TextBox>
                                 <asp:RequiredFieldValidator
                                ID="TextBoxRequiredFieldValidator"
                                Runat="server"
                                ForeColor = "Red"
                                Display="Dynamic"
                                ControlToValidate="KennzeichenBox"
                                ErrorMessage="Kennzeichen kann nicht leer sein!" >
                            </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                           <tr>
                            <td>
                                VIN
                            </td>
                            <td>
                                <asp:TextBox ID="VINBox" Text='<%# Bind( "VIN") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Make
                            </td>
                            <td>
                                <asp:TextBox ID="MakeBox" Text='<%# Bind( "Make") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Model
                            </td>
                            <td>
                                <asp:TextBox ID="ModelBox" Text='<%# Bind( "Model") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                           <tr>
                            <td>
                                Amount
                            </td>
                            <td>
                                <asp:TextBox ID="AmountBox" Text='<%# Bind( "Amount") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                            <td>
                                Product
                            </td>
                            <td>
                                <asp:TextBox ID="ProductBox" Text='<%# Bind( "Product") %>' runat="server">
                                </asp:TextBox>
                            </td>
                        </tr>
                        <%--<tr>
                            <td>
                                Variant
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox13" Text='<%# Bind( "Variant") %>' runat="server">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator
                                ID="TextBoxRequiredFieldValidator"
                                Runat="server"
                                Display="Dynamic"
                                ControlToValidate="TextBox13"
                                ErrorMessage="The textbox can not be empty!" >
                            </asp:RequiredFieldValidator>
                            </td>
                        </tr>--%>

                       
                        <telerik:RadTextBox runat="server" Enabled = "false" Visible = "False" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Das ist Small Customer" ID = "SmallCustomerBox" Width = "240px" ></telerik:RadTextBox>

                        <telerik:RadTextBox runat="server" Enabled = "false" Visible = "False" DisabledStyle-ForeColor = "Black" BorderColor="Transparent" DisabledStyle-BackColor = "Transparent" Text = "Das ist Large Customer " ID = "LargeCustomerBox" Width = "240px" ></telerik:RadTextBox>

                       <table style="width: 100%">
                        <tr>
                            <td align="right" colspan="2">
                                <asp:Button ID="FertigStellenButton" Text="Fertigstellen" runat="server" OnClick = "AuftragFertig_Command">
                                </asp:Button>&nbsp;
                                <asp:Button ID="Button2" Text="Cancel" runat="server" CausesValidation="False" CommandName="Cancel" >
                                </asp:Button>
                            </td>
                        </tr>
                    </table>
                </FormTemplate>
            </EditFormSettings>      
               
          </mastertableview>
           
        <clientsettings>
            <Scrolling AllowScroll="false" ></Scrolling>
             <Selecting AllowRowSelect="false" ></Selecting>
             <ClientEvents OnRowDblClick="RowDblClick"></ClientEvents>
        </clientsettings>
    </telerik:RadGrid>



    <asp:LinqDataSource TableName = "Customer" ID="LinqDataSourceZulassungCust" runat="server" OnSelecting="AbmeldungenLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
    </asp:LinqDataSource>

    <asp:LinqDataSource ID="CustomerZulDataSource" runat="server" OnSelecting="CustomerZulLinq_Selected" ContextTypeName="KVSCommon.Database.DataClasses1DataContext" >                 
    </asp:LinqDataSource>

</form>
</body>

