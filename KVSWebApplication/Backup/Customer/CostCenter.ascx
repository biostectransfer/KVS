<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CostCenter.ascx.cs" Inherits="KVSWebApplication.Customer.CostCenter" %>
   <link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />
     <link href="../Scripts/scripts.js" type="text/javascript" />
<div runat="server" class="uebersichtDiv2" id="myUebersicht">
            <telerik:RadFormDecorator ID="QsfFromDecoratorCostCenter" runat="server" DecoratedControls="All" EnableRoundedCorners="false" />
          <telerik:RadWindowManager ID="RadWindowManagerCostCenter" runat="server" EnableShadow="true">
          </telerik:RadWindowManager>
              <telerik:RadAjaxPanel ID="RadAjaxPanelCostCenter" runat="server" Width="900px" LoadingPanelID="RadAjaxLoadingPanelCostCenter">              
       <asp:Panel ID="CostCenterPanel" runat="server"  >
                            <asp:RadioButton ID="rbtAllCostCenter"  runat="server" Text="Kostenstellen" Checked="true" GroupName="CustomerType2" AutoPostBack="true"  OnCheckedChanged="AllCostCenterChecked" />
                            <asp:RadioButton ID="rbtCreateCostCenter" runat="server" Text="Neue Kostenstelle" GroupName="CustomerType2" AutoPostBack="true"  OnCheckedChanged="CreateCostcenterChecked"/>                          
                             <asp:Label ID="lblAllErrors" runat="server" CssClass="Validator" ></asp:Label> 
                            </asp:Panel>
   <asp:Panel id="AllCostCenterTable" Visible="false" runat="server" CssClass="CreateNewInsurance" >
            <table  border="0" cellpadding="5">
                <colgroup>
                    <col width="100"/>
                    <col width="350"/>
                    <col width="300"/>
                    <col width="200"/>
                </colgroup>
                <thead>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="TitleLabel1" runat="server" AssociatedControlID="CustomerCostCenter"
                                Text="Stammdaten"></asp:Label>
                        </td>                  
                    </tr>
                </thead>
                <tbody>
                    <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblCustomerNameCostCenter" runat="server" AssociatedControlID="CustomerCostCenter" Text="Kundenname:"></asp:Label>
                        </td>
                        <td>
 <telerik:RadComboBox  Height="300px" Width="250px"  Enabled = "true" 
             AutoPostBack = "true" Filter="Contains" runat = "server"  
                    DropDownWidth="515px" EmptyMessage="Wählen Sie einen Kunden aus..." HighlightTemplatedItems="true"
                  OnInit="CustomerCombobox_ItemsRequested"  
                DataTextField = "Name" DataValueField = "Value" ID = "CustomerCostCenter" >                 
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
                       <asp:Label ID="lblCustomerNameCostCenterError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                         </tr>
                         <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblCostCenterName" runat="server" AssociatedControlID="CostCenterName" Text="Name:"></asp:Label>
                        </td>
                        <td>                    
                            <telerik:RadTextBox ID="CostCenterName" runat="server"  Width="250px"  ValidationGroup="Group2">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblCostCenterNameError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>                 
                 </tr>
                    <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblCostCenterNumber" runat="server" AssociatedControlID="txbCostCenterNumber" Text="Kostenstellennummer:"></asp:Label>
                        </td>
                        <td>                    
                            <telerik:RadTextBox ID="txbCostCenterNumber" Width="250px"  runat="server" ValidationGroup="Group2">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblCostCenterNumberError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>                 
                 </tr>
                  <tr class="FormContainer">                    
                        <td>
                            <asp:Label ID="lblBankNameCostCenter" runat="server" AssociatedControlID="cmbBankNameCostCenter" Text="Kreditinstitut:"></asp:Label>
                        </td>
                        <td>
                         <telerik:RadTextBox ID="cmbBankNameCostCenter" runat="server"   Width="250px"  ></telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblBankNameCostCenterError" runat="server" CssClass="Validator" ></asp:Label>                                                  
                        </td>
                        </tr>
                  <tr class="FormContainer">
                       <td>
                            <asp:Label ID="lbCostCenterAccountNumber" runat="server"    Text="Kontonummer:"  AssociatedControlID="cmbCostCenterAccountNumber"
                                 AutoCompleteType="Disabled"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox  ID="cmbCostCenterAccountNumber"   Width="250px" runat="server"  
                                ValidationGroup="Group2" >
                            </telerik:RadTextBox >                            
                    <br />
                       <asp:Label ID="lblErrorBankAccount" runat="server" CssClass="Validator" ></asp:Label>                    
                        </td>
                               </tr>
                  <tr class="FormContainer">
                        <td>
                            <asp:Label ID="Label1" runat="server" AssociatedControlID="rcbCostCenterBankCode" Text="BLZ:"></asp:Label>
                        </td>
                        <td>
                         <telerik:RadTextBox ID="rcbCostCenterBankCode"  Width="250px"  runat="server" ></telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblCostCenterBankCodeError" runat="server" CssClass="Validator" ></asp:Label>                          
                     </tr>
            <tr class="FormContainer">       
                         <td>
                            <asp:Label ID="Label2" runat="server" Text="IBAN"  AssociatedControlID="rcbCostCenterBankCode"
                                 AutoCompleteType="Disabled"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox  ID="txbLargeCustomerIBAN"  Width="250px"  runat="server"  
                                ValidationGroup="Group2" >
                            </telerik:RadTextBox >               
                    <br />                
                      </td>
                    </tr>
                      <tr class="FormContainer">
                        <td>
                         <asp:Label ID="Label3" runat="server" Text="BIC"  AssociatedControlID="rcbCostCenterBankCode"
                              AutoCompleteType="Disabled"></asp:Label>
                        </td>
                        <td>
                          <telerik:RadTextBox  ID="txbLargeCustomerBIC"  Width="250px"  runat="server"  
                                ValidationGroup="Group2" >
                            </telerik:RadTextBox >
                           <telerik:RadButton ID="btnLargeCustomerGenerateIBAN" runat="server"  
                            Text="IBAN/BIC" ToolTip="IBAN Nummer generieren" AutoPostBack="true"  onclick="genIban_Click"></telerik:RadButton>
                            <asp:Label ID="lblIbanCostcenterError" runat="server" ForeColor="Red" Font-Bold="true" AssociatedControlID="rcbCostCenterBankCode" ></asp:Label> 
                         <br />
                        </td>
                   </tr>
                    <tr class="FormContainer">
                        <td>                           
                        </td>
                        <td>
                         <asp:Button ID="rbtSaveCostCenter" runat="server" Text="Speichern" 
                                onclick="rbtSaveCostCenter_Click" ></asp:Button>
                             <br />
                     </tr>
                </tbody>
            </table>
      </asp:Panel>
        <telerik:RadGrid id="getAllCostCenter" runat="server" DataSourceID="getAllCostCenterDataSource" Culture="de-De" 
         AllowAutomaticUpdates="true"  OnInit="getAllCostCenter_Init" 
            AllowAutomaticInserts="true" AllowAutomaticDeletes="false" AutoGenerateColumns="false" 
            OnUpdateCommand="getAllCustomer_EditCommand"    
            AllowPaging="true" >           
        <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"></PagerStyle>
       <MasterTableView DataKeyNames="CustomerId,CostCenterId" CommandItemDisplay="Top" EditMode="PopUp"  AllowFilteringByColumn="true" >
           <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="true"   />
            <Columns>           
                    <telerik:GridBoundColumn DataField="TableId" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                    <telerik:GridBoundColumn DataField="CustomerId" HeaderText="Id" ReadOnly="true"  
                    Visible="false" ForceExtractValue="Always"  />
                     <telerik:GridBoundColumn DataField="CostCenterId"  ReadOnly="true"  Visible="false" 
                     ForceExtractValue="Always"  />

                     <telerik:GridBoundColumn DataField="CostCenterName" HeaderText="Kostenstelle"  
                     AutoPostBackOnFilter="true" ShowFilterIcon="false" CurrentFilterFunction="Contains" 
                     FilterControlWidth="140px" AllowSorting="true" />
                     <telerik:GridBoundColumn DataField="CostCenterNumber" HeaderText="Nummer"
                      AutoPostBackOnFilter="true" ShowFilterIcon="false" CurrentFilterFunction="Contains" FilterControlWidth="100px" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="Name" HeaderText="Kundenname"  
                      AutoPostBackOnFilter="true" ShowFilterIcon="false" CurrentFilterFunction="Contains" FilterControlWidth="140px" AllowSorting="true"  />
                <telerik:GridBoundColumn DataField="CustomerNumber" HeaderText="Kundennummer"  
                  AutoPostBackOnFilter="true" ShowFilterIcon="false" CurrentFilterFunction="Contains" FilterControlWidth="110px" AllowSorting="true"  />               
                    <telerik:GridBoundColumn DataField="ContactId"  ReadOnly="true"  Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="AdressId"  ReadOnly="true"  Visible="false" ForceExtractValue="Always" />
                    <telerik:GridBoundColumn DataField="Street" HeaderText="Strasse"  
                      AutoPostBackOnFilter="true" ShowFilterIcon="false" CurrentFilterFunction="Contains" FilterControlWidth="130px" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="StreetNumber" HeaderText="Nummer"  
                      AutoPostBackOnFilter="true" ShowFilterIcon="false" CurrentFilterFunction="Contains" FilterControlWidth="60px" AllowSorting="true"  />
                    <telerik:GridBoundColumn DataField="Zipcode" HeaderText="PLZ" 
                     AutoPostBackOnFilter="true" ShowFilterIcon="false" CurrentFilterFunction="Contains" FilterControlWidth="60px" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="City" HeaderText="Ort"    AutoPostBackOnFilter="true" ShowFilterIcon="false"
                     CurrentFilterFunction="Contains" FilterControlWidth="70px" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="Country" HeaderText="Land"    AutoPostBackOnFilter="true" ShowFilterIcon="false" CurrentFilterFunction="Contains" FilterControlWidth="70px" AllowSorting="true" />
              <telerik:GridTemplateColumn HeaderText="Löschen" HeaderButtonType="TextButton" AllowFiltering="false"
                                         HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center" 
                                          AutoPostBackOnFilter="false">  
                                          <ItemTemplate>
                                               <asp:Label ID="lblCostCenterId" Text='<%#  DataBinder.Eval(Container, "DataItem.CostCenterId").ToString() %>' Visible="false"    runat="server"  ></asp:Label>
                                           <telerik:RadButton ID="btnRemoveCostCenter" runat="server" Text="Löschen" ToolTip="Kostenstelle löschen" OnClick="RemoveCostCenter_Click">
                                             <Icon PrimaryIconCssClass="rbRemove" PrimaryIconLeft="4" PrimaryIconTop="3"  ></Icon>
                                           </telerik:RadButton>
             </ItemTemplate></telerik:GridTemplateColumn>
             </Columns>             
             <DetailTables>
                 <telerik:GridTableView DataSourceID="GetCustomerCostCenter"  DataKeyNames="CustomerId,CostCenterId"
                        Width="800px"  EditMode="PopUp" BorderWidth="2px"
                  HeaderStyle-HorizontalAlign="Center" >
                        <ParentTableRelation>
                            <telerik:GridRelationFields DetailKeyField="CustomerId"  MasterKeyField="CustomerId" />
                            <telerik:GridRelationFields DetailKeyField="CostCenterId"  MasterKeyField="CostCenterId" />
                        </ParentTableRelation>
                         <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" />
                        <Columns>
                            <telerik:GridEditCommandColumn ButtonType="ImageButton" />
                            <telerik:GridBoundColumn DataField="TableId" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                             <telerik:GridBoundColumn DataField="CustomerId" HeaderText="Id" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                              <telerik:GridBoundColumn DataField="CostCenterId" HeaderText="Id" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                            <telerik:GridBoundColumn DataField="BankId" HeaderText="BankId" Visible="false" ReadOnly="true" ForceExtractValue="Always"    />
                              <telerik:GridBoundColumn DataField="CostCenterName" HeaderText="Name" />
                            <telerik:GridBoundColumn DataField="BankName" HeaderText="Kreditinstitut" />
                            <telerik:GridBoundColumn DataField="Accountnumber" HeaderText="Kontonummer" />
                            <telerik:GridBoundColumn DataField="BankCode" HeaderText="BLZ" />
                           <telerik:GridBoundColumn DataField="IBAN" HeaderText="IBAN" />
                            <telerik:GridBoundColumn DataField="BIC" HeaderText="BIC" />                           
                        </Columns>
                        <EditFormSettings EditFormType="AutoGenerated" InsertCaption="Bankdaten bearbeiten"   >
                    <EditColumn ButtonType="ImageButton" />
                    <PopUpSettings Modal="true"   />               
                </EditFormSettings>
                       <ItemStyle BackColor="#DFDFDF" BorderWidth="3px" BorderColor="Black" />
            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />
            <AlternatingItemStyle BackColor="#FFFFFF" BorderWidth="3px" BorderColor="Black" />
                    </telerik:GridTableView>
                </DetailTables>
                     <EditFormSettings EditFormType="AutoGenerated" InsertCaption="Kostenstelle bearbeiten"  >
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
              <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelCostCenter" runat="server"  BackgroundTransparency="100" >
        </telerik:RadAjaxLoadingPanel>
        <asp:LinqDataSource runat=server ID="getAllCostCenterDataSource" OnSelecting="getAllCostCenterDataSource_Selecting" ></asp:LinqDataSource>
            <asp:LinqDataSource runat=server ID="GetCustomerCostCenter" OnSelecting="GetCustomerCostCenter_Selecting">
            <WhereParameters>
            <asp:Parameter Name="CustomerId"  />
           <asp:Parameter Name="CostCenterId"  />
            </WhereParameters>        
        </asp:LinqDataSource>
        </div>