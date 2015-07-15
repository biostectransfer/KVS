<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SmallCustomerDetails.ascx.cs" Inherits="KVSWebApplication.Customer.SmallCustomerDetails" %>
   <link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/scripts.js"  type="text/javascript"/>    
<div runat="server" class="uebersichtDiv4" id="myUebersicht4">
      <telerik:RadAjaxPanel ID="RadAjaxPanelSmallCustomer" runat="server" Width="1500px" 
      LoadingPanelID="RadAjaxLoadingPanelSmallCustomer">
         <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true">
          </telerik:RadWindowManager>
<telerik:RadGrid id="getSmallCustomer" runat="server" DataSourceID="GetSmallCustomerDataSource" Culture="de-De"  AllowAutomaticUpdates="false" 
            AllowAutomaticInserts="true" AllowAutomaticDeletes="false" AutoGenerateColumns="false"  AllowFilteringByColumn="true" 
            AllowPaging="true" Width="1500px"  OnUpdateCommand="getSmallCustomer_EditCommand" OnInit="SmallCustomer_Init"   >           
 <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"></PagerStyle>
       <MasterTableView DataKeyNames="Id" CommandItemDisplay="Top" EditMode="PopUp" Width="1560px"  ItemStyle-BorderWidth="2px" ItemStyle-BorderColor="Black"  >
           <CommandItemSettings ShowAddNewRecordButton="false"   />
            <Columns>
             <telerik:GridEditCommandColumn ButtonType="ImageButton" />
                 <telerik:GridTemplateColumn HeaderText="Löschen" HeaderButtonType="TextButton" AllowFiltering="false"
                                         HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center" 
                                          AutoPostBackOnFilter="false">  
                                          <ItemTemplate>
                                               <asp:Label ID="lblCustomerId" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>' Visible="false"    runat="server"  ></asp:Label>
                                           <telerik:RadButton ID="btnRemoveCustomer" runat="server" Text="Löschen" ToolTip="Laufkunden löschen" OnClick="RemoveSmallCustomer_Click">
                        <Icon PrimaryIconCssClass="rbRemove" PrimaryIconLeft="4" PrimaryIconTop="3"  ></Icon>
                    </telerik:RadButton>
                                          </ItemTemplate> 
                </telerik:GridTemplateColumn>

                 <telerik:GridBoundColumn DataField="TableId" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                   <telerik:GridTemplateColumn HeaderText="Rechnungsadressen" HeaderButtonType="TextButton" AllowFiltering="false"
                                         HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center" 
                                          AutoPostBackOnFilter="false">                            
                      <ItemTemplate>
                      <asp:Panel ID="myInvoice" runat="server">
                                    <asp:Label ID="lblRechnungsadressenSmallCustomer"  runat="server" Text='Bearbeiten' >
                                    </asp:Label>
                                      <asp:Label ID="lblId" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>' Visible="false"    runat="server"  >
                                    </asp:Label>        
                                         <telerik:RadToolTip ManualClose="true" ManualCloseButtonText="Schließen"   
                                         ID="ttAngebotskopf" runat="server"   
                                          TargetControlID="lblRechnungsadressenSmallCustomer" Animation="Slide" 
                                          RelativeTo="Element"  Position="BottomLeft">
                                    <div style="width:600px;  white-space:normal;" >                              
                                        <table  border="0" cellpadding="5" style="border-top:3px;">
                <colgroup class="FormContainer">
                    <col width="100"/>
                    <col width="200"/>
                    <col width="100"/>
                    <col width="200"/>
                </colgroup>
                <thead>
                    <tr class="FormContainer" >
                        <td colspan="2">                                
                            <asp:Label ID="TitleLabel1" runat="server" 
                                Text="Rechnungsadresse"></asp:Label>
                                <asp:CheckBox ID="chbInvoiceSameAsAdressLC" runat="server" Text="Rechnungsadresse wie Standartadresse" Checked='<%#  DataBinder.Eval(Container, "DataItem.SameAsAdress").ToString()  == "true" ? true :  false  %>'
                                GroupName="CustomerType" 
                                AutoPostBack="false" />
                        </td>
                        <td colspan="2">
                            <asp:Label ID="TitleLabel2" runat="server"
                                Text="Rechnungsversandadresse"></asp:Label>
                                  <asp:CheckBox ID="chbSameASInvoiceAdress" runat="server" Text="Versandadresse wie Rechnungsadresse"   Checked='<%#  DataBinder.Eval(Container, "DataItem.SameAsInvoice").ToString()  == "true" ? true :  false  %>'
                                GroupName="CustomerType"  AutoPostBack="false" />
                        </td>                
                    </tr>
                </thead>
                <tbody>
                 <tr class="FormContainer">
                      <td>
                            <asp:Label ID="lblSmallEditCustomerInvoiceStreet" runat="server" AssociatedControlID="txbSmallEditCustomerInvoiceStreet" Text="Strasse /Nr.:"></asp:Label>
                        </td>
                        <td>
                              <telerik:RadTextBox ID="txbSmallEditCustomerInvoiceStreet" runat="server"
                              Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceStreet").ToString() %>'
                              ValidationGroup="Group1" Width="120px">
                            </telerik:RadTextBox>     <telerik:RadTextBox ID="txbSmallCustomerInvoiceStreetNr" runat="server" 
                            Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceStreeetNumber").ToString() %>'
                             Width="35px" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                                 <br />
                       <asp:Label ID="lblSmallEditCustomerInvoiceStreetError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>                  
                    <td>
                            <asp:Label ID="lblSmallEditCustomerSendStreet" runat="server" AssociatedControlID="txbSmallEditCustomerSendStreet" Text="Strasse /Nr.:"></asp:Label>
                        </td>
                        <td>
                              <telerik:RadTextBox ID="txbSmallEditCustomerSendStreet" runat="server" 
                              Text='<%#  DataBinder.Eval(Container, "DataItem.SendStreet").ToString() %>'                              
                               ValidationGroup="Group1" Width="120px">
                            </telerik:RadTextBox>     <telerik:RadTextBox ID="txbSmallEditCustomerSendStreetNr"   
                               Text='<%#  DataBinder.Eval(Container, "DataItem.SendStreeetNumber").ToString() %>'
                            runat="server" Width="35px" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                                 <br />
                       <asp:Label ID="lblSmallEditCustomerSendStreetError" runat="server" CssClass="Validator" ></asp:Label>                   
                        </td>
                 </tr>
                  <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblSmallCustomerEditInvoiceZipCode" runat="server" AssociatedControlID="cmbSmallCustomerEditInvoiceZipCode" Text="PLZ:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbSmallCustomerEditInvoiceZipCode"  enableloadondemand="true" OnItemsRequested="ZipCodes_ItemsRequested"
                            DataTextField="Zipcode" DataValueField="Zipcode"  ShowMoreResultsBox="True" Height="140px" 
                             Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceZipCode").ToString() %>'
                            ZIndex="9999" runat="server" AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                          
                        <br />
                       <asp:Label ID="lblSMallCustomerEditInvoiceZipCodeError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                           <td valign="top">
                            <asp:Label ID="lblSmallCustomerEditSendZipCode" runat="server" AssociatedControlID="cmbSmallCustomerEditSendZipCode" Text="PLZ:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbSmallCustomerEditSendZipCode"   
                            Text='<%#  DataBinder.Eval(Container, "DataItem.SendZipCode").ToString() %>' ShowMoreResultsBox="True" Height="140px" 
                            DataTextField="Zipcode" DataValueField="Zipcode" OnItemsRequested="ZipCodes_ItemsRequested"  enableloadondemand="true" 
                            ZIndex="9999" runat="server" AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                        <br />
                       <asp:Label ID="lblSmallCustomerEditSendZipCodeError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                  </tr>
                       <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblSmallCustomerEditInvoiceCity" runat="server" AssociatedControlID="cmbSmallCustomerEditInvoiceCity" Text="Ort:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbSmallCustomerEditInvoiceCity" ZIndex="9999" OnItemsRequested="City_ItemsRequested"  enableloadondemand="true" 
                             Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceCity").ToString() %>' ShowMoreResultsBox="True" Height="140px" 
                            runat="server" AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                        <br />
                       <asp:Label ID="lblSmallCustomerEditInvoiceCityError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                           <td valign="top">
                            <asp:Label ID="lblSmallCustomerEditSendCity" runat="server" AssociatedControlID="SmallCustomerEditSendCity" Text="Ort:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="SmallCustomerEditSendCity"  OnItemsRequested="City_ItemsRequested"  enableloadondemand="true" 
                            Text='<%#  DataBinder.Eval(Container, "DataItem.SendCity").ToString() %>'  ShowMoreResultsBox="True" Height="140px" 
                            ZIndex="9999" runat="server" AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                        <br />
                       <asp:Label ID="lblSmallCustomerEditSendCityError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                  </tr>
                    <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblSmallCustomerEditInvoiceCountry" runat="server" AssociatedControlID="cmbSmallCustomerEditInvoiceCountry" Text="Land:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbSmallCustomerEditInvoiceCountry" ZIndex="9999" OnItemsRequested="Country_ItemsRequested"  enableloadondemand="true" 
                            runat="server"   Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceCountry").ToString() %>' ShowMoreResultsBox="True" Height="140px" 
                            AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                        <br />
                       <asp:Label ID="lblSmallCustomerEditInvoiceCountryError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                           <td valign="top">
                            <asp:Label ID="lblSmallCustomerSendCountry" runat="server" AssociatedControlID="cmbSmallCustomerSendCountry" Text="Land:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbSmallCustomerSendCountry"    Text='<%#  DataBinder.Eval(Container, "DataItem.SendCountry").ToString() %>'  ShowMoreResultsBox="True" Height="140px" 
                            ZIndex="9999" runat="server" AllowCustomText="true" MarkFirstMatch="true" OnItemsRequested="Country_ItemsRequested"  enableloadondemand="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                        <br />
                       <asp:Label ID="lblSmallCustomerSendCountryError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                  </tr>
                         <tr class="FormContainer" >
                     <td colspan="4">
                     <telerik:RadButton id="bSaveAdressData" OnClick="bSaveAdressData_Click" runat="server" Text="Speichern"  ></telerik:RadButton>
                     </td>
                    </tr>
                </tbody>
                </table>
                    </div>
                        </telerik:RadToolTip>  
                    </asp:Panel> 
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                     <telerik:GridBoundColumn DataField="Id" HeaderText="Id" ReadOnly="true"  Visible="false" ForceExtractValue="Always"  />
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Kundenname"   AutoPostBackOnFilter="true" 
                    ShowFilterIcon="false" CurrentFilterFunction="Contains" 
               AllowSorting="true" SortExpression="Name"/>
                      <telerik:GridBoundColumn DataField="CustomerNumber" HeaderText="Kundennummer"  AutoPostBackOnFilter="true" ShowFilterIcon="false"
                       CurrentFilterFunction="Contains"  AllowSorting="true" SortExpression="Name"/>
                    <telerik:GridBoundColumn DataField="ContactId"  ReadOnly="true"  Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="AdressId"  ReadOnly="true"  Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="Street" HeaderText="Strasse" AutoPostBackOnFilter="true" ShowFilterIcon="false"
                       CurrentFilterFunction="Contains"
                     AllowSorting="true" SortExpression="Street"/>
                    <telerik:GridBoundColumn DataField="StreetNumber" HeaderText="Nummer"   AutoPostBackOnFilter="true" ShowFilterIcon="false"
                    CurrentFilterFunction="Contains"  AllowSorting="true" SortExpression="StreetNumber"  FilterControlWidth="40px"/>
                    <telerik:GridBoundColumn DataField="Zipcode" HeaderText="PLZ"  FilterControlWidth="60px"  AutoPostBackOnFilter="true" ShowFilterIcon="false" CurrentFilterFunction="Contains" 
                     AllowSorting="true" SortExpression="Zipcode"/>
                    <telerik:GridBoundColumn DataField="City" HeaderText="Ort"  FilterControlWidth="120px" AutoPostBackOnFilter="true" ShowFilterIcon="false" CurrentFilterFunction="Contains"
                      AllowSorting="true" SortExpression="City"/>
                    <telerik:GridBoundColumn DataField="Country" HeaderText="Land"  AutoPostBackOnFilter="true"  FilterControlWidth="80px"
                    ShowFilterIcon="false" CurrentFilterFunction="Contains"  AllowSorting="true" SortExpression="Country"/>
                    <telerik:GridBoundColumn DataField="Phone" HeaderText="Telefonnummer" FilterControlWidth="100px"  
                    AutoPostBackOnFilter="true" ShowFilterIcon="false" CurrentFilterFunction="Contains"  AllowSorting="true" SortExpression="Phone"/>
                    <telerik:GridBoundColumn DataField="Fax" HeaderText="Faxnummer"  AutoPostBackOnFilter="true"  FilterControlWidth="80px"
                    ShowFilterIcon="false" CurrentFilterFunction="Contains"  AllowSorting="true" SortExpression="Fax"/>
                    <telerik:GridBoundColumn DataField="MobilePhone" HeaderText="Mobil"   FilterControlWidth="80px"
                    AutoPostBackOnFilter="true" ShowFilterIcon="false" CurrentFilterFunction="Contains" AllowSorting="true" SortExpression="MobilePhone"/>
                    <telerik:GridBoundColumn DataField="Email" HeaderText="Email" FilterControlWidth="120px"  AutoPostBackOnFilter="true" ShowFilterIcon="false"
                     CurrentFilterFunction="Contains"  AllowSorting="true" SortExpression="Email"/>
                    <telerik:GridBoundColumn DataField="Vat" HeaderText="MwSt" FilterControlWidth="40px"  AutoPostBackOnFilter="true" 
                    ShowFilterIcon="false" CurrentFilterFunction="Contains" AllowSorting="false"/>
                   <telerik:GridBoundColumn DataField="Zahlungsziel" FilterControlWidth="40px" HeaderText="Zahlungsziel"  AutoPostBackOnFilter="true" 
                   ShowFilterIcon="false" CurrentFilterFunction="Contains"  AllowSorting="false"/>
             </Columns>
             <DetailTables> 
                 <telerik:GridTableView DataSourceID="GetSmallCustomerBankData"  DataKeyNames="CustomerId"  
                          EditMode="PopUp" AllowFilteringByColumn="false"  Width="700px" BorderWidth="2px" HeaderStyle-HorizontalAlign="Center">
                         <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="false"   />
                        <ParentTableRelation>
                            <telerik:GridRelationFields DetailKeyField="CustomerId" MasterKeyField="Id" />
                        </ParentTableRelation>
                        <Columns>
                              <telerik:GridEditCommandColumn ButtonType="ImageButton"  />
                            <telerik:GridBoundColumn  DataField="TableId" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                            <telerik:GridBoundColumn DataField="CustomerId" HeaderText="Id" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                            <telerik:GridBoundColumn DataField="BankName" HeaderText="Kreditinstitut" />
                            <telerik:GridBoundColumn DataField="Accountnumber" HeaderText="Kontonummer" />
                            <telerik:GridBoundColumn DataField="BankCode" HeaderText="BLZ" />
                            <telerik:GridBoundColumn DataField="IBAN" HeaderText="IBAN" />
                            <telerik:GridBoundColumn DataField="BIC" HeaderText="BIC" />
                        </Columns>
                        <EditFormSettings EditFormType="AutoGenerated"   >
                    <EditColumn ButtonType="ImageButton" />
                    <PopUpSettings Modal="true"   />
                </EditFormSettings>
                     <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
            <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />
                    </telerik:GridTableView>
                </DetailTables>
                <EditFormSettings EditFormType="AutoGenerated" >
                    <EditColumn ButtonType="ImageButton" />
                    <PopUpSettings Modal="true"    />
                </EditFormSettings>
                     <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
            <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />
            </MasterTableView>
            <PagerStyle AlwaysVisible="true" />
        </telerik:RadGrid>
            </telerik:RadAjaxPanel>
             <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelSmallCustomer"  BackgroundTransparency="100"  runat="server">
        </telerik:RadAjaxLoadingPanel>
<asp:LinqDataSource runat=server ID="GetSmallCustomerDataSource" OnSelecting="GetSmallCustomerDataSource_Selecting" ></asp:LinqDataSource>
 <asp:LinqDataSource runat=server ID="GetSmallCustomerBankData" OnSelecting="GetSmallCustomerBankData_Selecting"
 Where="CustomerId.ToString() == @CustomerId">
            <WhereParameters>
                <asp:Parameter Name="CustomerId"  />
            </WhereParameters>
        </asp:LinqDataSource>
     </div>