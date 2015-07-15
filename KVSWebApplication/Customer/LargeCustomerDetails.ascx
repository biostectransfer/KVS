<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LargeCustomerDetails.ascx.cs" Inherits="KVSWebApplication.Customer.LargeCustomerDetails" %>
   <link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />
<div runat="server" class="uebersichtDiv" id="myUebersicht">
         <telerik:RadAjaxPanel ID="RadAjaxPanelLargeCustomerShow" runat="server" 
         LoadingPanelID="RadAjaxLoadingPanelLargeCustomer">
          <telerik:RadWindowManager ID="RadWindowManagerLargeCustomer" runat="server" EnableShadow="true">
          </telerik:RadWindowManager>
<telerik:RadGrid id="getAllCustomer" runat="server" DataSourceID="GetAllCustomerDataSource" Culture="de-De"  
AllowAutomaticUpdates="true" 
            AllowAutomaticInserts="true" AllowAutomaticDeletes="false" AutoGenerateColumns="false"    
            OnInit="LargeCustomer_Init"  
            AllowPaging="true"  OnUpdateCommand="getAllCustomer_EditCommand" >           
 <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"></PagerStyle>
       <MasterTableView DataKeyNames="ContactId, PersonId, Id" CommandItemDisplay="Top" EditMode="PopUp" 
       AllowFilteringByColumn="true" >     
           <CommandItemSettings ShowAddNewRecordButton="false"   />
            <Columns>

             <telerik:GridEditCommandColumn ButtonType="ImageButton" />
                      <telerik:GridTemplateColumn HeaderText="Löschen" HeaderButtonType="TextButton" AllowFiltering="false"
                                         HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center" 
                                          AutoPostBackOnFilter="false">  
                                          <ItemTemplate>
                                               <asp:Label ID="lblLargeCustomerId" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>' Visible="false"    runat="server"  ></asp:Label>
                                           <telerik:RadButton ID="btnRemoveLargeCustomer" runat="server" Text="Löschen" ToolTip="Laufkunden löschen" OnClick="RemoveLargeCustomer_Click">
                        <Icon PrimaryIconCssClass="rbRemove" PrimaryIconLeft="4" PrimaryIconTop="3"  ></Icon>
                    </telerik:RadButton>
                                          </ItemTemplate> 
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="TableId" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                        <telerik:GridTemplateColumn HeaderText="Rechnungsadressen" HeaderButtonType="TextButton" ShowFilterIcon="false" 
                                        HeaderStyle-Width="120px" FilterControlWidth="85px" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center" 
                                        AutoPostBackOnFilter="true" AllowFiltering="false">                            
                    <ItemTemplate>
                    <asp:Panel ID="myInvoice" runat="server">     
                                <asp:Label ID="lblRechnungsadressen"  runat="server" Text='Bearbeiten' >
                                </asp:Label>
                                    <asp:Label ID="lblId" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>' Visible="false"    runat="server"  >
                                </asp:Label>    
                                        <telerik:RadToolTip ManualClose="true" ManualCloseButtonText="Schließen"  
                                        ID="ttAngebotskopf1" runat="server"   
                                        TargetControlID="lblRechnungsadressen" Animation="Slide" 
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
                                <asp:CheckBox ID="chbInvoiceSameAsAdressLC" runat="server" Text="Rechnungsadresse wie Standard-Adresse" Checked='<%#  DataBinder.Eval(Container, "DataItem.SameAsAdress").ToString()  == "true" ? true :  false  %>'
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
                            <asp:Label ID="lblLargeEditCustomerInvoiceStreet" runat="server" AssociatedControlID="txbLargeEditCustomerInvoiceStreet" Text="Strasse /Nr.:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLargeEditCustomerInvoiceStreet" runat="server" 
                              Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceStreet").ToString() %>' 
                              ValidationGroup="Group1" Width="120px">
                            </telerik:RadTextBox>   
                            
                            <telerik:RadTextBox ID="txbLargeCustomerInvoiceStreetNr" runat="server"
                            Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceStreeetNumber").ToString() %>'
                             Width="35px" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                                 <br />
                       <asp:Label ID="lblLargeEditCustomerInvoiceStreetError" runat="server" CssClass="Validator" ></asp:Label>                   
                        </td>                  
                    <td>
                            <asp:Label ID="lblLargeEditCustomerSendStreet" runat="server" AssociatedControlID="txbLargeEditCustomerSendStreet" Text="Strasse /Nr.:"></asp:Label>
                        </td>
                        <td>
                              <telerik:RadTextBox ID="txbLargeEditCustomerSendStreet" runat="server" 
                              Text='<%#  DataBinder.Eval(Container, "DataItem.SendStreet").ToString() %>'
                              
                               ValidationGroup="Group1" Width="120px">
                            </telerik:RadTextBox>     <telerik:RadTextBox ID="txbLargeEditCustomerSendStreetNr"  
                               Text='<%#  DataBinder.Eval(Container, "DataItem.SendStreeetNumber").ToString() %>'
                            runat="server" Width="35px" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                                 <br />
                       <asp:Label ID="lblLargeEditCustomerSendStreetError" runat="server" CssClass="Validator" ></asp:Label>                   
                        </td>
                 </tr>
                  <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblLargeCustomerEditInvoiceZipCode" runat="server" AssociatedControlID="cmbLargeCustomerEditInvoiceZipCode" Text="PLZ:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLargeCustomerEditInvoiceZipCode" enableloadondemand="true" OnItemsRequested="ZipCodes_ItemsRequested"
                             Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceZipCode").ToString() %>'
                            ZIndex="9999" runat="server" AllowCustomText="true" MarkFirstMatch="true"  ShowMoreResultsBox="True" Height="140px" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                          
                        <br />
                       <asp:Label ID="lblLargeCustomerEditInvoiceZipCodeError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                           <td valign="top">
                            <asp:Label ID="lblLargeCustomerEditSendZipCode" runat="server" AssociatedControlID="cmbLargeCustomerEditSendZipCode" Text="PLZ:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLargeCustomerEditSendZipCode" enableloadondemand="true" OnItemsRequested="ZipCodes_ItemsRequested"
                            Text='<%#  DataBinder.Eval(Container, "DataItem.SendZipCode").ToString() %>' ShowMoreResultsBox="True" Height="140px" 
                            ZIndex="9999" runat="server" AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                          
                        <br />
                       <asp:Label ID="lblLargeCustomerEditSendZipCodeError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                  </tr>
                       <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblLargeCustomerEditInvoiceCity" runat="server" AssociatedControlID="cmbLargeCustomerEditInvoiceCity" Text="Ort:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLargeCustomerEditInvoiceCity" ZIndex="9999" enableloadondemand="true" OnItemsRequested="City_ItemsRequested"
                             Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceCity").ToString() %>' ShowMoreResultsBox="True" Height="140px" 
                            runat="server" AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                          
                        <br />
                       <asp:Label ID="lblLargeCustomerEditInvoiceCityError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                           <td valign="top">
                            <asp:Label ID="lblLargeCustomerEditSendCity" runat="server" AssociatedControlID="LargeCustomerEditSendCity" Text="Ort:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="LargeCustomerEditSendCity" enableloadondemand="true" OnItemsRequested="City_ItemsRequested" 
                            Text='<%#  DataBinder.Eval(Container, "DataItem.SendCity").ToString() %>'  ShowMoreResultsBox="True" Height="140px" 
                            ZIndex="9999" runat="server" AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                          
                        <br />
                       <asp:Label ID="lblLargeCustomerEditSendCityError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                  </tr>

                    <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblLargeCustomerEditInvoiceCountry" runat="server" AssociatedControlID="cmbLargeCustomerEditInvoiceCountry" Text="Land:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLargeCustomerEditInvoiceCountry" ZIndex="9999" runat="server"  enableloadondemand="true" OnItemsRequested="Country_ItemsRequested" 
                            Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceCountry").ToString() %>' AllowCustomText="true" MarkFirstMatch="true" ShowMoreResultsBox="True" Height="140px" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                          
                        <br />
                       <asp:Label ID="lblLargeCustomerEditInvoiceCountryError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                           <td valign="top">
                            <asp:Label ID="lblLargeCustomerSendCountry" runat="server" AssociatedControlID="cmbLargeCustomerSendCountry" Text="Land:"></asp:Label>                             
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbLargeCustomerSendCountry"    enableloadondemand="true" OnItemsRequested="Country_ItemsRequested" ShowMoreResultsBox="True" Height="140px" 
                            Text='<%#  DataBinder.Eval(Container, "DataItem.SendCountry").ToString() %>'  ZIndex="9999" runat="server" AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                          
                        <br />
                       <asp:Label ID="lblLargeCustomerSendCountryError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                  </tr>

                         <tr class="FormContainer" >                    
                     <td colspan="4">       
                     <telerik:RadButton id="bSaveAdressData" OnClick="bSaveAdressData_Click" 
                       runat="server" Text="Speichern"  ></telerik:RadButton>
                     </td>
                     </tr>
                </tbody>
                 </table>
                </div>
                    </telerik:RadToolTip>    
                </asp:Panel>                                      
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Erlöskonten" HeaderButtonType="TextButton"  ShowFilterIcon="false"
                                          HeaderStyle-Width="120px" FilterControlWidth="85px" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center" 
                                          AutoPostBackOnFilter="true" AllowFiltering="false">                            
                      <ItemTemplate>
                      <asp:Panel ID="PanelErloeskonten" runat="server">
                        <asp:Label ID="lblKofigErloeskonto"  runat="server" Text='Bearbeiten' >
                                    </asp:Label>
                                                 <telerik:RadToolTip ManualClose="true" ManualCloseButtonText="Schließen"    
                                         ID="ttKonfigErloeskonto" runat="server"   
                                          TargetControlID="lblKofigErloeskonto" Animation="Slide" 
                                          RelativeTo="Element"  Position="BottomRight">
                      <table  border="0" cellpadding="2" style="float:left; width:120px; z-index:999;">
                <colgroup>
                    <col width="100"/>
                    <col width="200"/>                
                </colgroup>
                <thead>
                    <td colspan="2">
                        <asp:Label ID="lblErlöskontenText" runat="server" Font-Bold="true"
                            Text="Elöskonten"></asp:Label> 
                    </td>          
                </thead>
                <tbody>
                <tr>
                <td>
                <telerik:RadComboBox ID="cmbErloeskonten" runat="server" style="z-index:9999;" AllowCustomText="true"  enableloadondemand="true" OnItemsRequested="cmbErloeskonten_ItemsRequested">               
                </telerik:RadComboBox>
                </td>
                <td>
                <telerik:RadButton ID="AddKonto" runat="server" Text="Hinzufügen" ToolTip="Neues Erlöskonto hinzufügen" OnClick="AddKonto_Click"> 
                    <Icon PrimaryIconCssClass="rbAdd" PrimaryIconLeft="4" PrimaryIconTop="3"  ></Icon>
                </telerik:RadButton>              
                </td>
                 <td>
                    <telerik:RadButton ID="RemoveKonto" runat="server" Text="Löschen" ToolTip="Erlöskonto löschen" OnClick="RemoveKonto_Click">
                        <Icon PrimaryIconCssClass="rbRemove" PrimaryIconLeft="4" PrimaryIconTop="3"  ></Icon>
                    </telerik:RadButton>
                </td>
                </tr>                
                </tbody>
                </table>     
                    </div>
                    </telerik:RadToolTip>
                      </asp:Panel>
                      </ItemTemplate>
                      </telerik:GridTemplateColumn>
                             <telerik:GridTemplateColumn HeaderText="Versandkonfig" HeaderButtonType="TextButton" ShowFilterIcon="false"
                                          HeaderStyle-Width="120px" FilterControlWidth="85px" HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center" 
                                          AutoPostBackOnFilter="true" AllowFiltering="false">                            
                      <ItemTemplate>
                      <asp:Panel ID="myKonfig" runat="server">
                                    <asp:Label ID="lblKofigSmallCustomer"  runat="server" Text='Bearbeiten' >
                                    </asp:Label>
                                      <asp:Label ID="lblIdkonfig" Text='<%#  DataBinder.Eval(Container, "DataItem.Id").ToString() %>' Visible="false"    runat="server"  >
                                    </asp:Label>
                                         <telerik:RadToolTip ManualClose="true" ManualCloseButtonText="Schließen"   
                                         ID="ttKonfig" runat="server"   
                                          TargetControlID="lblKofigSmallCustomer" Animation="Slide" 
                                          RelativeTo="Element"  Position="BottomRight">
                                        <div style="width:350px;  white-space:normal;" >
                                  <table >
                                  <tr>
                                <td colspan="2">
                        <div >
                         <asp:Label ID="lblLargeCustomerSendArt" runat="server" Font-Bold="true"  AssociatedControlID="lblKofigSmallCustomer" 
                         Text="Versand bei Auftragserledigung"></asp:Label>
                         </div>
                        </td>
                          <td colspan="2">
                           <div>
                         <asp:Label ID="lblLargeCustomerSendLieferschein" runat="server" Font-Bold="true"  AssociatedControlID="lblKofigSmallCustomer" 
                         Text="Versand bei Lieferscheinerstellung"></asp:Label>
                         </div>
                        </td>
                              </tr> 
                              <tr class="FormContainer">
                     <td>
                   <div  style="margin-left:30%;">
                    <asp:Label ID="lblAuftragserledigungWhere" runat="server" Font-Bold="true"  
                    AssociatedControlID="lblKofigSmallCustomer" Text="Ziel"></asp:Label>
             </div>                                
             </td>
             <td>
                <div style="margin-left:10%;">
                    <asp:Label ID="lblAuftragserledigungTime" runat="server" Font-Bold="true"  AssociatedControlID="lblKofigSmallCustomer" 
                    Text="Zeitpunkt"></asp:Label>      
                </div>
                   </td>  
                         <td>
                   <div style="margin-left:30%;">
                    <asp:Label ID="lblLieferscheinText" runat="server" Font-Bold="true"  
                    AssociatedControlID="lblKofigSmallCustomer" Text="Ziel"></asp:Label>
             </div>                                
             </td>
                <td></td>
                    </tr>   
                             <tr>
                                 <td>
                        <div style="margin-top:-50px;">
                             <asp:CheckBox ID="chbLCustomerAuftragKunde" runat="server" Text="Kunde" 
                             Checked='<%#  DataBinder.Eval(Container, "DataItem.VersandadresseKunde").ToString()  == "true" ? true :  false  %>'   /></br>
                             <asp:CheckBox ID="chbLCustomerAuftragStandort" runat="server" Text="Standort" 
                             Checked='<%#  DataBinder.Eval(Container, "DataItem.VersandadresseStandort").ToString()  == "true" ? true :  false  %>'   />
                             </div>
                        </td>
                               <td>
                        <div style="max-width:80px;">
                              <asp:RadioButton ID="LCustomerAuftragHourly" runat="server" Text="stündlich" GroupName="LCZeitpunkt" 
                              Checked='<%#  DataBinder.Eval(Container, "DataItem.VersandadresseTimeHourly").ToString()  == "true" ? true :  false  %>'/>
                              <asp:RadioButton ID="LCustomerAuftragDaily" runat="server" Text="täglich" GroupName="LCZeitpunkt" 
                              Checked='<%#  DataBinder.Eval(Container, "DataItem.VersandadresseTimeDayly").ToString()  == "true" ? true :  false  %>'/>
                              <asp:RadioButton ID="LCustomerAuftragNow" runat="server" Text="sofort"  GroupName="LCZeitpunkt"
                              Checked='<%#  DataBinder.Eval(Container, "DataItem.VersandadresseTimeNow").ToString()  == "true" ? true :  false  %>' />
                              <asp:RadioButton ID="LCustomerAuftragNoInfo" runat="server" Text="kein" GroupName="LCZeitpunkt"
                               Checked='<%#  DataBinder.Eval(Container, "DataItem.VersandadresseTimeNothing").ToString()  == "true" ? true :  false  %>' />                         
                             </div>
                        </td>                             
                             <td width="50">                             
                                 <div style="margin-top:-50px; max-width:80px; ">
                             <asp:CheckBox ID="chbLieferscheinKunde" runat="server" Text="Kunde" 
                             Checked='<%#  DataBinder.Eval(Container, "DataItem.LieferscheinKunde").ToString()  == "true" ? true :  false  %>'  />
                             <asp:CheckBox ID="chbLieferscheinStandort" runat="server" Text="Standort"  
                             Checked='<%#  DataBinder.Eval(Container, "DataItem.LieferscheinStandort").ToString()  == "true" ? true :  false  %>'  />
                             </div>                             
                             </td>
                           <td>                         
                           </td>                             
                             </tr>                         
                         <tr>
                         <td colspan="4">
                           <div>
                             <asp:Button id="SaveSendOrder" runat="server" Text="Speichern" OnClick="SaveSendOrder_Click" />
                             </div>
                         </td>
                         </tr>
                            </table>
                            </div>
                                </telerik:RadToolTip>    
                            </asp:Panel>                                      
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                              <telerik:GridTemplateColumn HeaderText="Rechnungstyp" HeaderButtonType="TextButton"  ShowFilterIcon="false" CurrentFilterFunction="Contains" 
                                          HeaderStyle-Width="120px" FilterControlWidth="85px" HeaderStyle-HorizontalAlign="Center"  DataField="InvoiceType"  ItemStyle-HorizontalAlign="Center" 
                                          AutoPostBackOnFilter="true">                            
                      <ItemTemplate>
                      <asp:Panel ID="PanelInvoiceType" runat="server">
                        <asp:Label ID="InvoiceTypeName"   runat="server" Text='<%#  DataBinder.Eval(Container, "DataItem.InvoiceType").ToString() %>' >
                                    </asp:Label>
                                                 <telerik:RadToolTip ManualClose="true" ManualCloseButtonText="Schließen"    
                                         ID="tplInvoiceType" runat="server"   
                                          TargetControlID="InvoiceTypeName" Animation="Slide" 
                                          RelativeTo="Element"  Position="BottomRight">
                      <table  border="0" cellpadding="2" style="float:left; width:120px; z-index:999;">
                <colgroup>
                    <col width="100"/>
                    <col width="200"/>
                </colgroup>
                <thead>
                               <td colspan="2">
                            <asp:Label ID="lblInvoiceTypeText" runat="server" Font-Bold="true"
                                Text="Rechnungstyp:"></asp:Label> 
                        </td> 
                </thead>
                <tbody>
                <tr>
                <td>
                <telerik:RadComboBox ID="cmbInvoiceTypes" runat="server" style="z-index:9999;" AllowCustomText="false"   DataTextField="Value" DataValueField="Key"
                 EnableLoadOnDemand="true" OnItemsRequested="GetAllInvoiceTypes_Selecting" >
                </telerik:RadComboBox>
                </td>
                <td>
                <telerik:RadButton ID="AddInvoiceType" runat="server" Text="Speichern" ToolTip="Als Standard speichern" OnClick="AddInvoiceType_Click"> 
                        <Icon PrimaryIconCssClass="rbAdd" PrimaryIconLeft="4" PrimaryIconTop="3"  ></Icon>
                    </telerik:RadButton>              
                </td>
                </tr>
                </tbody>
                </table>
                        </div>
                        </telerik:RadToolTip>
                      </asp:Panel>
                      </ItemTemplate>
                      </telerik:GridTemplateColumn>
                     <telerik:GridBoundColumn DataField="Id"  ReadOnly="true"  Visible="true" Display="false" ForceExtractValue="Always"  />
                      <telerik:GridBoundColumn DataField="Kundennummer" HeaderText="Kundennummer" AutoPostBackOnFilter="true" 
                      CurrentFilterFunction="Contains" FilterControlWidth="80px"  ShowFilterIcon="false" AllowSorting="true" SortExpression="Name"/>
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Kundenname"  AutoPostBackOnFilter="true" 
                    CurrentFilterFunction="Contains" FilterControlWidth="80px" ShowFilterIcon="false"  AllowSorting="true" SortExpression="Name"/>                                
                    <telerik:GridBoundColumn DataField="ContactId"  ReadOnly="true"  Visible="false" ForceExtractValue="Always" />
                     <telerik:GridBoundColumn DataField="PersonId"  ReadOnly="true"  Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="AdressId"  ReadOnly="true"  Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="Street" HeaderText="Strasse"  CurrentFilterFunction="Contains" AutoPostBackOnFilter="true"
                    FilterControlWidth="80px" ShowFilterIcon="false"  AllowSorting="true" SortExpression="Street"/>
                    <telerik:GridBoundColumn DataField="StreetNumber" HeaderText="Nummer"  CurrentFilterFunction="Contains" FilterControlWidth="80px" 
                    AllowSorting="true" ShowFilterIcon="false"  SortExpression="StreetNumber"/>
                    <telerik:GridBoundColumn DataField="Zipcode" HeaderText="PLZ" CurrentFilterFunction="Contains" FilterControlWidth="80px" AutoPostBackOnFilter="true"
                    AllowSorting="true" ShowFilterIcon="false"  SortExpression="Zipcode"/>
                    <telerik:GridBoundColumn DataField="City" HeaderText="Ort" CurrentFilterFunction="Contains" AutoPostBackOnFilter="true"
                    FilterControlWidth="80px" AllowSorting="true" ShowFilterIcon="false"  SortExpression="City"/>
                    <telerik:GridBoundColumn DataField="Country" HeaderText="Land" CurrentFilterFunction="Contains" FilterControlWidth="80px" AutoPostBackOnFilter="true"
                    AllowSorting="true" ShowFilterIcon="false"  SortExpression="Country"/>
                    <telerik:GridBoundColumn DataField="Vat" HeaderText="MwSt" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" FilterControlWidth="80px" ShowFilterIcon="false"  AllowSorting="false"/>
                    <telerik:GridBoundColumn DataField="Zahlungsziel" HeaderText="Zahlungsziel" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="false"  FilterControlWidth="80px" AllowSorting="false"/>
                     <telerik:GridBoundColumn DataField="MatchCode" HeaderText="Matchcode" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" FilterControlWidth="80px" ShowFilterIcon="false"  AllowSorting="false"/>
                      <telerik:GridBoundColumn DataField="Debitornumber" HeaderText="Debitornumber" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" FilterControlWidth="80px" ShowFilterIcon="false"  AllowSorting="false"/>
                       <telerik:GridBoundColumn DataField="evbnumber" HeaderText="eVB-Nummer" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" FilterControlWidth="80px" ShowFilterIcon="false"  AllowSorting="false"/>
                    <telerik:GridCheckBoxColumn DataField="SendInvoiceToMainLocation"  ItemStyle-HorizontalAlign="Center"  ItemStyle-VerticalAlign="Middle"  ToolTip="Rechnung an den Hauptstandort senden" HeaderText="Standort"  FilterControlWidth="40px" AllowFiltering="false" AllowSorting="false"/>
                    <telerik:GridCheckBoxColumn DataField="SendInvoiceByEmail" ItemStyle-HorizontalAlign="Center"  ItemStyle-VerticalAlign="Middle" ToolTip="Rechnung per Email sender"  HeaderText="Email"  FilterControlWidth="40px" AllowFiltering="false" AllowSorting="false"/>                    
                      <telerik:GridBoundColumn FilterControlWidth="120px"  HeaderText = "InternalId"  DataField="InternalId"  ReadOnly="true"  Visible="true" />
             </Columns>
             <DetailTables>
                 <telerik:GridTableView DataSourceID="GetAllCustomerContactData"  Width="800px" BorderWidth="2px"
                  HeaderStyle-HorizontalAlign="Center" 
                       EditMode="PopUp">
                          <CommandItemSettings ShowAddNewRecordButton="false"  ShowRefreshButton="false"  />
                        <ParentTableRelation>
                            <telerik:GridRelationFields DetailKeyField="PersonId" MasterKeyField="PersonId" />
                             <telerik:GridRelationFields DetailKeyField="ContactId" MasterKeyField="ContactId" />
                             <telerik:GridRelationFields DetailKeyField="CustomerId" MasterKeyField="Id" />
                        </ParentTableRelation>                          
                        <Columns>
                              <telerik:GridEditCommandColumn ButtonType="ImageButton" />
                            <telerik:GridBoundColumn DataField="Title" HeaderText="Anrede" />
                            <telerik:GridBoundColumn DataField="Name" HeaderText="Name" />
                            <telerik:GridBoundColumn DataField="Vorname" HeaderText="Vorname" />
                             <telerik:GridBoundColumn DataField="Extension" HeaderText="Zusatz" />
                            <telerik:GridBoundColumn  DataField="TableId" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                            <telerik:GridBoundColumn DataField="PersonId" HeaderText="PersonId" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                            <telerik:GridBoundColumn DataField="CustomerId" HeaderText="CustomerId" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                            <telerik:GridBoundColumn DataField="ContactId" HeaderText="ContactId" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                            <telerik:GridBoundColumn DataField="Phone" HeaderText="Telefonnummer" />
                            <telerik:GridBoundColumn DataField="Fax" HeaderText="Faxnummer" />
                            <telerik:GridBoundColumn DataField="MobilePhone" HeaderText="Mobil" />
                            <telerik:GridBoundColumn DataField="Email" HeaderText="Email Adresse" />
                        </Columns>
                        <EditFormSettings EditFormType="AutoGenerated">
                    <EditColumn ButtonType="ImageButton" />
                    <PopUpSettings Modal="true"   />               
                </EditFormSettings>
                     <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
            <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />
                    </telerik:GridTableView>    
                </DetailTables>
                <EditFormSettings EditFormType="AutoGenerated"   InsertCaption="Kunden bearbeiten" >
                    <EditColumn ButtonType="ImageButton" />
                    <PopUpSettings Modal="true"    />
                </EditFormSettings>
                     <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
            <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />
            </MasterTableView>
            <ClientSettings>
               <ClientEvents  OnPopUpShowing="onPopUpShowing"/>
            </ClientSettings>
            <PagerStyle AlwaysVisible="true" />
        </telerik:RadGrid>
 </telerik:RadAjaxPanel>
      <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelLargeCustomer" runat="server"  BackgroundTransparency="100" >
        </telerik:RadAjaxLoadingPanel>
<asp:LinqDataSource runat=server ID="GetAllCustomerDataSource" OnSelecting="GetAllCustomerDataSource_Selecting" ></asp:LinqDataSource>
 <asp:LinqDataSource runat=server ID="GetAllCustomerContactData" OnSelecting="GetAllCustomerContactData_Selecting"
         Where="ContactId.ToString() == @ContactId">
            <WhereParameters>
                <asp:Parameter Name="ContactId"  />
                 <asp:Parameter Name="PersonId"  />
                       <asp:Parameter Name="CustomerId"  />
            </WhereParameters>
        </asp:LinqDataSource>        
     </div>