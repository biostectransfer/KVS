<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Location_Details.ascx.cs" Inherits="KVSWebApplication.Customer.Location_Details" %>
   <link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />
<div runat="server" class="uebersichtDiv2" id="myUebersicht">
          <telerik:RadWindowManager ID="RadWindowManagerLocationDetails" runat="server" EnableShadow="true">
          </telerik:RadWindowManager>
            <telerik:RadFormDecorator ID="QsfFromDecoratorLoction" runat="server" DecoratedControls="All" EnableRoundedCorners="false" />
              <telerik:RadAjaxPanel ID="RadAjaxPanelLocationDeails" runat="server" Width="900px" LoadingPanelID="RadAjaxLoadingPanelLocations">
                 <asp:Panel ID="LocationDetailsPanel" runat="server" >
                            <asp:RadioButton ID="rbtAllLocation" runat="server" Text="Alle Standorte" Checked="true" GroupName="CustomerType" AutoPostBack="true"  OnCheckedChanged="AllLocationChecked" />
                            <asp:RadioButton ID="rbtCreateLocation" runat="server" Text="Neuer Standort" GroupName="CustomerType" AutoPostBack="true"  OnCheckedChanged="CreateLocationChecked"/>
                             <asp:Label ID="lblAllLOcationErrors" runat="server" CssClass="Validator" ></asp:Label> 
                            </asp:Panel>
<br />
<asp:Label ID="resultMessage" runat="server"></asp:Label>
   <asp:Panel id="AllLocationTable" Visible="false" runat="server" CssClass="CreateNewInsurance" >
            <table  border="0" cellpadding="5" >
                <colgroup>
                    <col width="100"/>
                    <col width="200"/>
                    <col width="100"/>
                    <col width="200"/>
                </colgroup>
                <thead>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="TitleLabel1" runat="server" AssociatedControlID="CustomerLocation"
                                Text="Stammdaten"></asp:Label>
                        </td>
                     <td colspan="2">
                            <asp:Label ID="lblLabel2" runat="server" AssociatedControlID="CustomerLocation"
                                Text="Anschrift"></asp:Label>
                        </td>
                          <td colspan="2">
                            <asp:Label ID="lblRechnungsadresse" runat="server" AssociatedControlID="CustomerLocation"
                                Text="Rechnungsadresse"></asp:Label>
                        </td>
                          <td colspan="2">
                            <asp:Label ID="lblLieferadresse" runat="server" AssociatedControlID="CustomerLocation"
                                Text="Lieferadresse"></asp:Label>
                        </td>                        
                    </tr>
                </thead>
                <tbody>
                    <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblCustomerNameLocation" runat="server" AssociatedControlID="CustomerLocation" Text="Kundenname:"></asp:Label>
                        </td>
                        <td>
            <telerik:RadComboBox  Height="300px"  Enabled = "true" 
             AutoPostBack = "true" Filter="Contains" runat = "server"   TabIndex="0"
                    DropDownWidth="515px" EmptyMessage="Wählen Sie einen Kunden aus..." HighlightTemplatedItems="true"
                  OnSelectedIndexChanged="CustomerCombobox_SelectedIndexChanged"  OnInit="CustomerCombobox_OnLoad"
                DataTextField = "Name" DataValueField = "Id" ID = "CustomerLocation"  >                 
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
                       <asp:Label ID="lblCustomerNameLocationError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                         <td>
                            <asp:Label ID="lblLocationStreetNr" runat="server" Text="Strasse /Nr:" AssociatedControlID="txbLocationStreet"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLocationStreet" runat="server" ValidationGroup="Group1" Width="120px" TabIndex="7">
                            </telerik:RadTextBox>     <telerik:RadTextBox ID="txbLocationNr" runat="server" Width="35px" ValidationGroup="Group1" TabIndex="8">
                            </telerik:RadTextBox>
                       <br />
                               <asp:Label ID="lblLocationStreetNrError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                                    <td colspan="2">
                                <asp:CheckBox ID="chbLocationRechnungsaderesse" runat="server" AutoPostBack="true" OnCheckedChanged="chbLocationRechnungsadresse_Checked" 
                                Checked="true" Text="Rechnungsadresse wie die Anschrift"  GroupName="CustomerType"  />                       
                      </td>
                        <td colspan="2">
                                <asp:CheckBox ID="chbLocationVersandadresse" runat="server" Checked="true" 
                                OnCheckedChanged="chbLocationVersandadresse_Checked" AutoPostBack="true"
                                Text="Versandadresse wie Rechnungsadresse"  GroupName="CustomerType"  />                       
                      </td>
                         </tr>
                         <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblLocationName" runat="server" AssociatedControlID="txbLocationName" Text="Standort Name:"></asp:Label>
                        </td>      
                        <td>                    
                            <telerik:RadTextBox ID="txbLocationName" runat="server" ValidationGroup="Group2" TabIndex="2">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblLocationNameError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                           <td>
                            <asp:Label ID="lblLocationZipCode" runat="server" Text="PLZ:" AssociatedControlID="txbLocationZipCode"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="txbLocationZipCode" runat="server" AllowCustomText="true" ShowMoreResultsBox="True" Height="140px" TabIndex="9"
                            MarkFirstMatch="true" OnInit="ZipCodes_ItemsRequested"  
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                    
                               <br />
                               <asp:Label ID="lblLocationZipCodeError" runat="server" CssClass="Validator" ></asp:Label>                      
                        </td>
                            <td>
                            <asp:Label ID="lblLocationInvoiceAdressInfo" runat="server" Text="Strasse /Nr:" AssociatedControlID="txbLocationInvoiceAdressStreet"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLocationInvoiceAdressStreet" runat="server" ValidationGroup="Group1" Width="120px"  TabIndex="14">
                            </telerik:RadTextBox>     <telerik:RadTextBox ID="txbLocationInvoiceAdressStreetNr" runat="server" Width="35px" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                       <br />
                               <asp:Label ID="lblLocationInvoiceAdressError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                            <td>
                            <asp:Label ID="lblLocationSendAdressInfo" runat="server" Text="Strasse /Nr:" AssociatedControlID="txbLocationSendAdressStreet"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLocationSendAdressStreet" runat="server" ValidationGroup="Group1" Width="120px"  TabIndex="17">
                            </telerik:RadTextBox>     <telerik:RadTextBox ID="txbLocationSendAdressStreetNr" runat="server" Width="35px" ValidationGroup="Group1"  TabIndex="18">
                            </telerik:RadTextBox>
                       <br />
                               <asp:Label ID="lblLocationSendAdressError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                 </tr>
                  <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblLocationPhone" runat="server" AssociatedControlID="txbLocationPhone" Text="Telefonnummer:"></asp:Label>
                        </td>       
                        <td>                    
                            <telerik:RadTextBox ID="txbLocationPhone" runat="server" ValidationGroup="Group2" TabIndex="3">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblLocationPhoneError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                           <td>
                            <asp:Label ID="lblLocationCity" runat="server"  Text="Stadt:" AssociatedControlID="cmbLocationCity"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLocationCity" runat="server"  ShowMoreResultsBox="True" Height="140px" 
                            OnInit="Citys_ItemsRequested"   TabIndex="10"
                            AllowCustomText="true" MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                            <br/>
                        <asp:Label ID="lblLocationCityError" runat="server" CssClass="Validator" ></asp:Label>                      
                        </td>                        
                                   <td>
                            <asp:Label ID="lblLocationInvoiceZipInfo" runat="server"  Text="PLZ:" AssociatedControlID="cmbLocationInvoiceZip"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLocationInvoiceZip" runat="server" ShowMoreResultsBox="True" Height="140px"   TabIndex="14"
                            OnInit="ZipCodes_ItemsRequested"
                            AllowCustomText="true" MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                            <br/>
                        <asp:Label ID="lblLocationInvoiceZipError" runat="server" CssClass="Validator" ></asp:Label>                      
                        </td>
                                   <td>
                            <asp:Label ID="lblLocationSendZipInfo" runat="server"  Text="PLZ:" AssociatedControlID="cmbLocationSendZip"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLocationSendZip" runat="server"  ShowMoreResultsBox="True" Height="140px" 
                            OnInit="ZipCodes_ItemsRequested"  TabIndex="19"
                            AllowCustomText="true" MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                            <br/>
                        <asp:Label ID="lblLocationSendZipInfoError" runat="server" CssClass="Validator" ></asp:Label>                      
                        </td>                 
                 </tr>
                    <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblLocationFax" runat="server" AssociatedControlID="txbLocationFax" Text="Faxnummer:"></asp:Label>
                        </td>         
                        <td>                    
                            <telerik:RadTextBox ID="txbLocationFax" runat="server" ValidationGroup="Group2" TabIndex="4">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblLocationFaxError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                           <td >
                            <asp:Label ID="lblLocationCountry" runat="server"  Text="Land:" AssociatedControlID="cmbLocationCountry"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLocationCountry" runat="server" ShowMoreResultsBox="True" Height="140px" 
                           OnInit="Countrys_ItemsRequested"   TabIndex="11"
                            AllowCustomText="true" MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                            <br/>                     
                        <asp:Label ID="lblLocationCountryError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                            <td>
                            <asp:Label ID="lblLocationCityInvoiceInfo" runat="server"  Text="Stadt:" AssociatedControlID="cmbLocationCityInvoice"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLocationCityInvoice" runat="server" ShowMoreResultsBox="True" Height="140px"   TabIndex="15"
                            OnInit="Citys_ItemsRequested"  
                            AllowCustomText="true" MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                            <br/>
                        <asp:Label ID="lblLocationCityInvoiceInfoError" runat="server" CssClass="Validator" ></asp:Label>                      
                        </td>
                        <td>
                            <asp:Label ID="lblLocationCitySendInfo" runat="server"  Text="Stadt:" AssociatedControlID="cmbLocationCitySend"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLocationCitySend" runat="server"  ShowMoreResultsBox="True" Height="140px" 
                            OnInit="Citys_ItemsRequested"  TabIndex="20"
                            AllowCustomText="true" MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                            <br/>
                        <asp:Label ID="lblLocationCitySendInfoError" runat="server" CssClass="Validator" ></asp:Label>                      
                        </td>
                 </tr>
               <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblLocationMobil" runat="server" AssociatedControlID="txbLocationMobilePhoneNummer" Text="Mobilnummer:"></asp:Label>
                        </td>
                        <td>                    
                            <telerik:RadTextBox ID="txbLocationMobilePhoneNummer" runat="server" ValidationGroup="Group2" TabIndex="5">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblLocationMobilePhoneNummerError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                        <td >                        
                            <asp:Label ID="lblOverLocation" runat="server" AssociatedControlID="txbLocationMobilePhoneNummer" Text="Hauptstandort:"></asp:Label>                     
                        </td>
                        <td >
                            <telerik:RadComboBox ID="cmbOverLocation" runat="server" MarkFirstMatch="false" AllowCustomText="True"  
                                ValidationGroup="Group1"  TabIndex="12">
                            </telerik:RadComboBox>
                            <br/>                     
                        <asp:Label ID="lblOverLocationError" runat="server" CssClass="Validator" ></asp:Label>                             
                        </td>
                               <td>
                            <asp:Label ID="lblLocationInvoiceCountryInfo" runat="server"  Text="Land:" AssociatedControlID="cmbLocationInvoiceCountry"></asp:Label>
                        </td>
                         <td>
                            <telerik:RadComboBox ID="cmbLocationInvoiceCountry" runat="server"  ShowMoreResultsBox="True" Height="140px" 
                            OnInit="Countrys_ItemsRequested"  TabIndex="16"
                            AllowCustomText="true" MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                            <br/>
                        <asp:Label ID="lblLocationInvoiceCountryInfoError" runat="server" CssClass="Validator" ></asp:Label>                      
                        </td>
                        <td>
                            <asp:Label ID="lblLocationSendCountryInfo" runat="server"  Text="Land:" AssociatedControlID="cmbLocationSendCountry"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLocationSendCountry" runat="server"  ShowMoreResultsBox="True" Height="140px" 
                            OnInit="Countrys_ItemsRequested"   TabIndex="21"
                            AllowCustomText="true" MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                            <br/>
                        <asp:Label ID="lblLocationSendCountryInfoError" runat="server" CssClass="Validator" ></asp:Label>                      
                        </td>
                        </tr>
                       <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblLocationEmail" runat="server" AssociatedControlID="txbLocationEmail" Text="Email:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLocationEmail" runat="server" ValidationGroup="Group2" TabIndex="6">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblLocationEmailError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                         <td >                     
                             <asp:Label ID="lblVATInfo" runat="server" AssociatedControlID="txbVat" 
                                 Text="Mehrwertsteuersatz:"></asp:Label>
                        </td>
                        <td>                    
                            <telerik:RadTextBox ID="txbVat" runat="server" ValidationGroup="Group2"  TabIndex="13">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblVatError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>                   
                     <td>
                           </td>                      
                          <td >
                        </td>                         
                          <td >
                        </td>
                        <td>
                           <telerik:RadButton ID="rbtSave" runat="server" CssClass="bSaveButton" TabIndex="22"
                                  onclick="rbtSaveLocation_Click" Text="Speichern">
                              </telerik:RadButton>
                        </td>                 
                 </tr>
                </tbody>
            </table>      
      </asp:Panel>
      <asp:Panel ID="AllCustomerLocations" runat="server">
      <telerik:RadGrid id="getAllCustomerLocations" runat="server" DataSourceID="getAllCustomerLocationsDataSource" Culture="de-De"  AllowAutomaticUpdates="true" 
            AllowAutomaticInserts="true" AllowAutomaticDeletes="false" AutoGenerateColumns="false"   EnableViewState ="true"
            AllowPaging="true" Width="900px" >           
 <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"></PagerStyle>
       <MasterTableView DataKeyNames="Id" CommandItemDisplay="Top" EditMode="PopUp" 
        AllowFilteringByColumn="true"  >
           <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true"   />
            <Columns>           
                    <telerik:GridBoundColumn DataField="TableId" Visible="false" ReadOnly="true" 
                    ForceExtractValue="Always"    />
                    <telerik:GridBoundColumn DataField="Id" HeaderText="Id" ReadOnly="true"  Visible="false" ForceExtractValue="Always"  />
               
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Kundenname"  
                   AutoPostBackOnFilter="true" ShowFilterIcon="false" CurrentFilterFunction="Contains" 
                    FilterControlWidth="95%" AllowSorting="true" SortExpression="Name"/>
                   <telerik:GridBoundColumn DataField="CustomerNumber"
                   ShowFilterIcon="false" HeaderText="Kundennummer"  CurrentFilterFunction="Contains"  
                     AutoPostBackOnFilter="true" 
                   FilterControlWidth="100px" AllowSorting="true" SortExpression="CustomerNumber"/>
                    <telerik:GridBoundColumn DataField="ContactId"  ReadOnly="true"  Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="AdressId"  ReadOnly="true"  Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="Street" HeaderText="Strasse"  
                       AutoPostBackOnFilter="true" ShowFilterIcon="false"
                    CurrentFilterFunction="Contains" FilterControlWidth="100px" AllowSorting="true" SortExpression="Street"/>
                    <telerik:GridBoundColumn DataField="StreetNumber" 
                       AutoPostBackOnFilter="true" ShowFilterIcon="false"
                    HeaderText="Nummer"  CurrentFilterFunction="Contains" FilterControlWidth="80px" AllowSorting="true" SortExpression="StreetNumber"/>
                    <telerik:GridBoundColumn DataField="Zipcode" 
                       AutoPostBackOnFilter="true" ShowFilterIcon="false"
                    HeaderText="PLZ" CurrentFilterFunction="Contains" FilterControlWidth="95%" AllowSorting="true" SortExpression="Zipcode"/>
                    <telerik:GridBoundColumn DataField="City" 
                       AutoPostBackOnFilter="true" ShowFilterIcon="false"
                    HeaderText="Ort" CurrentFilterFunction="Contains" FilterControlWidth="95%" AllowSorting="true" SortExpression="City"/>
                    <telerik:GridBoundColumn DataField="Country" 
                       AutoPostBackOnFilter="true" ShowFilterIcon="false"
                    HeaderText="Land" CurrentFilterFunction="Contains" FilterControlWidth="95%" AllowSorting="true" SortExpression="Country"/>
             </Columns>             
             <DetailTables>
                 <telerik:GridTableView TableLayout="Auto" DataSourceID="GetCustomerLocations" Width="1400px" DataKeyNames="Id" 
                         EditMode="PopUp" BorderWidth="2px"
                  HeaderStyle-HorizontalAlign="Center" >
                        <ParentTableRelation>
                            <telerik:GridRelationFields MasterKeyField="Id" DetailKeyField="Id" />
                        </ParentTableRelation>
                         <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" />
                        <Columns>
                          <telerik:GridEditCommandColumn ButtonType="ImageButton"  />
                             <telerik:GridTemplateColumn HeaderText="Rechnungsadressen" HeaderButtonType="TextButton"
                                          HeaderStyle-Width="120px" FilterControlWidth="85px" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center" 
                                          AutoPostBackOnFilter="true">                            
                      <ItemTemplate>
                      <asp:Panel ID="myInvoice" runat="server">
                                    <asp:Label ID="lblRechnungsadressenLocation"  runat="server" Text='Bearbeiten' >
                                    </asp:Label>
                                      <asp:Label ID="lblId" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>' Visible="false"    runat="server"  >
                                    </asp:Label>
                                       <asp:Label ID="txbLocationInvoiceId" Visible = "false"  runat="server"  Text='<%#  DataBinder.Eval(Container, "DataItem.LocationId") %>'>
                                </asp:Label>
                                         <telerik:RadToolTip ManualClose="true" ManualCloseButtonText="Schließen"   
                                         ID="ttAngebotskopf" runat="server"   
                                          TargetControlID="lblRechnungsadressenLocation" Animation="Slide" 
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
                        </td>
                        <td colspan="2">
                            <asp:Label ID="TitleLabel2" runat="server"
                                Text="Rechnungsversandadresse"></asp:Label>
                        </td>             
                    </tr>
                </thead>
                <tbody>
                 <tr class="FormContainer">
                      <td>
                            <asp:Label ID="lblLocationEditCustomerInvoiceStreet" runat="server" AssociatedControlID="txbLocationEditCustomerInvoiceStreet" Text="Strasse /Nr.:"></asp:Label>
                        </td>
                        <td>
                              <telerik:RadTextBox ID="txbLocationEditCustomerInvoiceStreet" runat="server" 
                              Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceStreet")  != null ? DataBinder.Eval(Container, "DataItem.InvoiceStreet") : ""  %>'
                              ValidationGroup="Group1" Width="120px">
                            </telerik:RadTextBox>     <telerik:RadTextBox ID="txbLocationInvoiceStreetNr" runat="server"
                            Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceStreeetNumber") != null ? DataBinder.Eval(Container, "DataItem.InvoiceStreeetNumber") : "" %>'
                             Width="35px" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                                 <br />
                       <asp:Label ID="lblLocationInvoiceStreetError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>                  
                    <td>
                            <asp:Label ID="lblLocationEditCustomerSendStreet" runat="server" AssociatedControlID="txbLocationEditCustomerSendStreet" Text="Strasse /Nr.:"></asp:Label>
                        </td>
                        <td>
                              <telerik:RadTextBox ID="txbLocationEditCustomerSendStreet" runat="server"
                              Text='<%#  DataBinder.Eval(Container, "DataItem.SendStreet") != null ? DataBinder.Eval(Container, "DataItem.SendStreet") : "" %>'
                              
                               ValidationGroup="Group1" Width="120px">
                            </telerik:RadTextBox>     <telerik:RadTextBox ID="txbLocationEditCustomerSendStreetNr" 
                               Text='<%#  DataBinder.Eval(Container, "DataItem.SendStreeetNumber") != null ? DataBinder.Eval(Container, "DataItem.SendStreeetNumber") : ""%>'
                            runat="server" Width="35px" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                                 <br />
                       <asp:Label ID="lblLocationEditSendStreetError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                 </tr>
                  <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblLocationrEditInvoiceZipCode" runat="server" AssociatedControlID="cmbLocationEditInvoiceZipCode" Text="PLZ:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLocationEditInvoiceZipCode"   OnInit="ZipCodes_ItemsRequested"
                            DataTextField="Zipcode" DataValueField="Zipcode"  ShowMoreResultsBox="True" Height="140px" 
                             Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceZipCode") != null ? DataBinder.Eval(Container, "DataItem.InvoiceZipCode") : ""%>'
                            ZIndex="9999" runat="server" AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                          
                        <br />
                       <asp:Label ID="lblLocationEditInvoiceZipCodeError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                           <td valign="top">
                            <asp:Label ID="lblLocationEditSendZipCode" runat="server" AssociatedControlID="cmbLocationEditSendZipCode" Text="PLZ:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLocationEditSendZipCode"  ShowMoreResultsBox="True" Height="140px" 
                            Text='<%#  DataBinder.Eval(Container, "DataItem.SendZipCode") != null ? DataBinder.Eval(Container, "DataItem.SendZipCode") : ""%>' 
                            DataTextField="Zipcode" DataValueField="Zipcode" OnInit="ZipCodes_ItemsRequested" 
                            ZIndex="9999" runat="server" AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                        <br />
                       <asp:Label ID="lblLocationEditSendZipCodeError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                  </tr>
                       <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblLocationEditInvoiceCity"  runat="server" AssociatedControlID="cmbLocationEditInvoiceCity" Text="Ort:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLocationEditInvoiceCity" ZIndex="9999" OnInit="Citys_ItemsRequested" 
                             Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceCity") != null ? DataBinder.Eval(Container, "DataItem.InvoiceCity") : ""%>'
                            runat="server" AllowCustomText="true" MarkFirstMatch="true" ShowMoreResultsBox="True" Height="140px" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                          
                        <br />
                       <asp:Label ID="lblLocationEditInvoiceCityError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                           <td valign="top">
                            <asp:Label ID="lblLocationEditSendCity" runat="server" AssociatedControlID="LocationEditSendCity" Text="Ort:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="LocationEditSendCity"  OnInit="Citys_ItemsRequested"  ShowMoreResultsBox="True" Height="140px" 
                            Text='<%#  DataBinder.Eval(Container, "DataItem.SendCity")!= null ? DataBinder.Eval(Container, "DataItem.SendCity") : ""%>'
                            ZIndex="9999" runat="server" AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                          
                        <br />
                       <asp:Label ID="lblLocationEditSendCityError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                  </tr>
                    <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblLocationInvoiceCountry" runat="server" AssociatedControlID="cmbLocationEditInvoiceCountry" Text="Land:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLocationEditInvoiceCountry" ZIndex="9999" OnInit="Countrys_ItemsRequested"  ShowMoreResultsBox="True" Height="140px" 
                            runat="server"   Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceCountry") != null ? DataBinder.Eval(Container, "DataItem.InvoiceCountry") : ""%>' 
                            AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                          
                        <br />
                       <asp:Label ID="lblLocationInvoiceCountryError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                           <td valign="top">
                            <asp:Label ID="lblLocationSendCountry" runat="server" AssociatedControlID="cmbLocationSendCountry" Text="Land:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLocationSendCountry"    Text='<%#  DataBinder.Eval(Container, "DataItem.SendCountry") != null ? DataBinder.Eval(Container, "DataItem.SendCountry") : ""%>'  
                            ZIndex="9999" runat="server" AllowCustomText="true" MarkFirstMatch="true" OnInit="Countrys_ItemsRequested"  howMoreResultsBox="True" Height="140px" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                          
                        <br />
                       <asp:Label ID="lblLocationSendCountryError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                        </tr>
                         <tr class="FormContainer" >                    
                     <td colspan="4">
                     <telerik:RadButton id="bSaveAdressData" OnClick="SaveLocationAdress"  runat="server" Text="Speichern" TabIndex="21"  ></telerik:RadButton>
                     </td>
                    </tr>
                </tbody>
                </table>
                </div>
                    </telerik:RadToolTip>     
                </asp:Panel>                                      
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                          <telerik:GridBoundColumn DataField="TableId" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                          <telerik:GridBoundColumn DataField="Id" HeaderText="Id" ReadOnly="true"  Visible="false" ForceExtractValue="Always"  />
                            <telerik:GridBoundColumn DataField="LocationId" HeaderText="Id" ReadOnly="true"  Visible="false" ForceExtractValue="Always"  />

                          <telerik:GridBoundColumn DataField="ContactId"  Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                          <telerik:GridBoundColumn DataField="AdressId"  Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                          <telerik:GridBoundColumn DataField="SuperLocationId"  Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                                   
                          <telerik:GridBoundColumn DataField="Name" HeaderText="Name"  />                         
                          <telerik:GridBoundColumn DataField="Phone" HeaderText="Telefonnummer"/>
                          <telerik:GridBoundColumn DataField="Fax" HeaderText="Faxnummer"/>
                          <telerik:GridBoundColumn DataField="MobilePhone" HeaderText="Mobilnummer"/>
                          <telerik:GridBoundColumn DataField="Email" HeaderText="Email"/>
                          <telerik:GridBoundColumn DataField="Street" HeaderText="Strasse/Nr."/>
                          <telerik:GridBoundColumn DataField="HouseNumber" HeaderText="Nr."/>
                          <telerik:GridBoundColumn DataField="Zipcode" HeaderText="Plz"/>
                          <telerik:GridBoundColumn DataField="City" HeaderText="Stadt"/>
                          <telerik:GridBoundColumn DataField="Country" HeaderText="Land"/>
                          <telerik:GridBoundColumn DataField="SuperLocation" HeaderText="Haupstandort"/>
                          <telerik:GridBoundColumn DataField="Vat" HeaderText="MwSt" />
                           <telerik:GridTemplateColumn HeaderText="Löschen" HeaderButtonType="TextButton" AllowFiltering="false"
                                         HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center" 
                                          AutoPostBackOnFilter="false">  
                                          <ItemTemplate>
                                               <asp:Label ID="lblLocationId" Text='<%#  DataBinder.Eval(Container, "DataItem.LocationId").ToString() %>' Visible="false"    runat="server"  ></asp:Label>
                                                   <asp:Label ID="lblCustomerId" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>' Visible="false"    runat="server"  ></asp:Label>
                                           <telerik:RadButton ID="btnRemoveLocation" runat="server" Text="Löschen" ToolTip="Standort löschen" OnClick="RemoveLocation_Click">
                        <Icon PrimaryIconCssClass="rbRemove" PrimaryIconLeft="4" PrimaryIconTop="3"  ></Icon>
                    </telerik:RadButton>
                                          </ItemTemplate> 
                </telerik:GridTemplateColumn>
                        </Columns>
                             <EditFormSettings  InsertCaption="Stammdaten bearbeiten" CaptionFormatString="Stammdaten bearbeiten" EditFormType="Template"   PopUpSettings-Modal="true" >
                <FormTemplate>          
                  <table id="tableEditAllLocations" cellspacing="1" cellpadding="1" width="350" border="0">
                        <tr>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                         <asp:TextBox ID="txbContactId" Visible = "false"  runat="server"  Text='<%#  DataBinder.Eval(Container, "DataItem.ContactId") %>'>
                                </asp:TextBox>
                                     <asp:TextBox ID="txbAdressId" Visible = "false"  runat="server"  Text='<%#  DataBinder.Eval(Container, "DataItem.AdressId") %>'>
                                </asp:TextBox>
                                     <asp:TextBox ID="txbCustomerId" Visible = "false"  runat="server"  Text='<%#  DataBinder.Eval(Container, "DataItem.Id") %>'>
                                </asp:TextBox>
                                   <asp:TextBox ID="txbSuperLocationId" Visible = "false"  runat="server"  Text='<%#  DataBinder.Eval(Container, "DataItem.SuperLocationId") %>'>
                                </asp:TextBox>
                                   <asp:TextBox ID="txbLocationEditId" Visible = "false"  runat="server"  Text='<%#  DataBinder.Eval(Container, "DataItem.LocationId") %>'>
                                </asp:TextBox>
                        <tr>
                            <td>
                                Name:
                            </td>
                            <td>
                                <asp:TextBox ID="Name" runat="server"  Text='<%#  DataBinder.Eval(Container, "DataItem.Name") != null ? DataBinder.Eval(Container, "DataItem.Name") : " "  %>'>
                                </asp:TextBox>
                            </td>
                        </tr>
                       <tr>
                            <td>
                                Telefonnummer:
                            </td>
                            <td>
                                <asp:TextBox ID="txbPhone" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.Phone") != null ? DataBinder.Eval(Container, "DataItem.Phone") : " " %>'>
                                </asp:TextBox>
                            </td>
                        </tr>
                           <tr>
                            <td>
                                Mobilnummer:
                            </td>
                            <td>
                            <asp:TextBox ID="txbMobile" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.MobilePhone") != null ? DataBinder.Eval(Container, "DataItem.MobilePhone") : " " %>'>
                                </asp:TextBox>

                            </td>
                        </tr>
                         <tr>
                              <tr>
                            <td>
                                Fax:
                            </td>
                            <td>
                            <asp:TextBox ID="txbEditLocationFax" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.Fax") != null ? DataBinder.Eval(Container, "DataItem.Fax") : " " %>'>
                                </asp:TextBox>
                            </td>
                        </tr>
                           <tr>
                            <td>
                                Email:
                            </td>
                            <td>
                            <asp:TextBox ID="txbEditEmail" runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.Email") != null ? DataBinder.Eval(Container, "DataItem.Email") : " " %>'>
                                </asp:TextBox>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                Strasse/Nr:
                      </td>
                            <td>    
                     <asp:TextBox ID="txbStreet" runat="server" Width="127px" Text='<%#  DataBinder.Eval(Container, "DataItem.Street") != null ? DataBinder.Eval(Container, "DataItem.Street") : " "  %>'>
                                </asp:TextBox>
                                    <asp:TextBox ID="txbNumber" runat="server" Width="30px"
                                    Text='<%#  DataBinder.Eval(Container, "DataItem.HouseNumber") != null ? DataBinder.Eval(Container, "DataItem.HouseNumber") : " " %>'>
                                </asp:TextBox>                          
                            </td>
                        </tr>
                       <tr>
                         <td>
                                Plz:
                            </td>
                            <td>
                             <telerik:RadComboBox ID="cmbZipCode"  OnInit="ZipCodes_ItemsRequested" 
                             runat="server"  AllowCustomText="True"  ShowMoreResultsBox="True" Height="140px" 
                             Text='<%#  DataBinder.Eval(Container, "DataItem.Zipcode") != null ? DataBinder.Eval(Container, "DataItem.Zipcode") : " " %>'>
                             </telerik:RadComboBox>
                            </td>
                        </tr>
                      <tr>
                            <td>
                                Stadt:
                            </td>
                            <td>
                            <telerik:RadComboBox ID="cmbCity" OnInit="Citys_ItemsRequested"  
                             runat="server"  AllowCustomText="True" ShowMoreResultsBox="True" Height="140px" 
                             Text='<%#  DataBinder.Eval(Container, "DataItem.City") != null ? DataBinder.Eval(Container, "DataItem.City") : " " %>'>
                             </telerik:RadComboBox>
                            </td>                            
                        </tr>
                              <tr>
                            <td>
                                Land:
                            </td>
                            <td>
                                 <telerik:RadComboBox ID="cmbCountry" OnInit="Countrys_ItemsRequested"  
                             runat="server"  AllowCustomText="True" ShowMoreResultsBox="True" Height="140px" 
                             Text='<%#  DataBinder.Eval(Container, "DataItem.Country") != null ? DataBinder.Eval(Container, "DataItem.Country") : " " %>'>
                             </telerik:RadComboBox>
                            </td>
                        </tr>
                          <tr>
                            <td>
                                Region:
                            </td><%--OnInit="SuperLocation_ItemsRequested"  --%>
                            <td>
                             <telerik:RadComboBox ID="cmbSuperLocation" OnItemsRequested="SuperLocation_ItemsRequested"  EnableLoadOnDemand="true"
                             runat="server" AllowCustomText="false" MarkFirstMatch="false" DataValueField="Id" DataTextField="Name" ShowMoreResultsBox="True" Height="140px" 
                             Text='<%#  DataBinder.Eval(Container, "DataItem.SuperLocation") != null ? DataBinder.Eval(Container, "DataItem.SuperLocation") : " " %>'>
                             </telerik:RadComboBox>
                            </td>
                        </tr>
                          <tr>
                            <td>
                                MwSt:
                            </td>
                            <td>
                            <asp:TextBox ID="txbVatEdit" runat="server"
                                    Text='<%#  DataBinder.Eval(Container, "DataItem.Vat") != null ? DataBinder.Eval(Container, "DataItem.Vat") : " " %>'>
                                </asp:TextBox>                           
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Hauptstandort:
                            </td>
                            <td>
                             <asp:CheckBox ID="chbDefaultLocation" 
                             runat="server" 
                             Checked='<%#  DataBinder.Eval(Container, "DataItem.DefaulLocation").Equals("true") ? true : false %>'  >
                             </asp:CheckBox>                           
                            </td>
                        </tr>
                          <tr>
                            <td align="center" colspan="2">
                                <asp:Button ID="btnSave" Text="Ok" runat="server" OnClick="btnSaveLocation_Click" >
                                </asp:Button>&nbsp;
                                <asp:Button ID="btnAbort" Text="Abbrechen" runat="server" CausesValidation="False" OnClick="btnAbort_Click" >
                                </asp:Button>
                                &nbsp;
                                <br />
                                <br />
                            </td>
                        </tr>
                    </table>
                </FormTemplate>                
                    <EditColumn ButtonType="ImageButton" />
                    <PopUpSettings Modal="true"    />
            </EditFormSettings>
             <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
            <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />
                    </telerik:GridTableView>
                </DetailTables>              
            <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
            <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />             
            </MasterTableView>            
            <PagerStyle AlwaysVisible="true" />
        </telerik:RadGrid>      
      <asp:LinqDataSource runat=server ID="getAllCustomerLocationsDataSource" OnSelecting="getAllCustomerLocationsDataSource_Selecting" ></asp:LinqDataSource>
            <asp:LinqDataSource runat=server ID="GetCustomerLocations" OnSelecting="GetCustomerLocations_Selecting">
            <WhereParameters>
                <asp:Parameter Name="Id"  />          
            </WhereParameters>        
        </asp:LinqDataSource>   
      </asp:Panel>
        </telerik:RadAjaxPanel>
              <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelLocations" runat="server"  BackgroundTransparency="100" >
        </telerik:RadAjaxLoadingPanel>
        </div>