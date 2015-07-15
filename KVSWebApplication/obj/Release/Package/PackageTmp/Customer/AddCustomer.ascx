<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddCustomer.ascx.cs" Inherits="KVSWebApplication.Customer.CustomerInformation" %> 
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns='http://www.w3.org/1999/xhtml'>
<body>
<style>
 .rbPredefined16, .rbPredefined24 {
     background-image: url(images/rbPredefinedIcons.png);
     background-repeat: no-repeat;
} 
.rbPredefined16 {
     width: 16px !important;
     height: 16px;
} 
.rbPredefined24 {
     width: 24px;
     height: 24px;
}
.rbPredefinedIcons {
     display: block;
     float: left;
     width: 16px;
     height: 16px;
     background-image: url(images/rbPredefinedIcons.png);
}
.rbAdd16 {
     background-position: 0 0 !important;
}
.rbRemove16 {
     background-position: -24px 0 !important;
}
</style>
   <link href="../Styles/CustomerStyle.css" rel="stylesheet" type="text/css" />
     <link href="../Scripts/scripts.js" type="text/javascript" />
      <telerik:RadAjaxPanel ID="RadAjaxPanelCustomer" runat="server"  LoadingPanelID="RadAjaxLoadingPanelCustomer">
    <telerik:RadFormDecorator ID="QsfFromDecorator" runat="server" DecoratedControls="All" EnableRoundedCorners="false" />
    <asp:RadioButton ID="rbtLargeCustomer" runat="server" Text="Großkunde" Checked="true" GroupName="CustomerType"
                           
        autopostback="true"  OnCheckedChanged="LargeCustomerChecked" />
    <asp:RadioButton ID="rbtSmallCustomer" runat="server" Text="Laufkundschaft" GroupName="CustomerType" AutoPostBack="true"  
    OnCheckedChanged="SmallCustomerChecked"/>                
    <br />   
   <br />
<asp:Label ID="resultMessage" runat="server"></asp:Label>
   <asp:Panel id="CreateLargeCustomerTable"  Visible="true" runat="server" >
            <table  border="0" align="left"  cellpadding="5" >
                <colgroup>                   
                </colgroup>
                <thead>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="TitleLabel1" runat="server" AssociatedControlID="PhoneNumer" Font-Bold="true"
                                Text="Stammdaten"></asp:Label>
                        </td>
                    </tr>
                </thead>
                <tbody>
                    <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblCustomerName" runat="server" AssociatedControlID="txbCustomerName" Text="Firma:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbCustomerName" runat="server" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblCustomerNameError" runat="server" CssClass="Validator" ></asp:Label>                          
                          </td>                    
                    </tr>
                      <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblMatchcode" runat="server" AssociatedControlID="txbCustomerName" Text="Matchcode:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLargeCustomerMatchcode" runat="server" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblLargeCustomerMatchcode" runat="server" CssClass="Validator" ></asp:Label>                          
                          </td>                    
                    </tr>
                     <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblLargeCustomerDebitorenNummer" runat="server" AssociatedControlID="txbCustomerName" Text="Debitorennummer:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLargeCustomerDebitorenNummer" runat="server" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblLargeCustomerDebitorenNummerError" runat="server" CssClass="Validator" ></asp:Label>                          
                          </td>                    
                    </tr>
                          <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblAnrede" runat="server" AssociatedControlID="txbCustomerName" Text="Anrede:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLargeCustomerSalutation" runat="server" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblAnredeError" runat="server" CssClass="Validator" ></asp:Label>                          
                          </td>                    
                    </tr>                   
                      <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblLargeCustomerFirstname" runat="server" AssociatedControlID="txbCustomerName" Text="Vorname:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLargeCustomerFirstname" runat="server" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblLargeCustomerFirstnameError" runat="server" CssClass="Validator" ></asp:Label> 
                          </td> 
                    </tr>
                         <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblLargeCustomerName" runat="server" AssociatedControlID="txbCustomerName" Text="Name:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLargeCustomerName" runat="server" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblLargeCustomerNameError" runat="server" CssClass="Validator" ></asp:Label> 
                          </td> 
                    </tr>
                             <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblLargeCustomerZusatz" runat="server" AssociatedControlID="txbCustomerName" Text="Zusatz:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLargeCustomerZusatz" runat="server" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblLargeCustomerZusatzError" runat="server" CssClass="Validator" ></asp:Label> 
                          </td> 
                    </tr>
                                  <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblLargeCustomerFirstContactPerson" runat="server" AssociatedControlID="txbLargeCustomerFirstContactPerson" Text="Ansprechspartner:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLargeCustomerFirstContactPerson" runat="server" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                             <br />
                       <asp:Label ID="lblLargeCustomerFirstContactPersonError" runat="server" CssClass="Validator" ></asp:Label> 
                          </td> 
                    </tr>
                    <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblStreet" runat="server" AssociatedControlID="txbStreet" Text="Strasse /Nr.:"></asp:Label>
                        </td>
                        <td>
                          <telerik:RadTextBox ID="txbStreet" runat="server" ValidationGroup="Group1" Width="120px">
                            </telerik:RadTextBox>     <telerik:RadTextBox ID="txbStreetNumber" runat="server" Width="35px" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                         <br />
                       <asp:Label ID="lblStreetError" runat="server" CssClass="Validator" ></asp:Label>                       
                        </td>
                    </tr>
                        <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblZipCode" runat="server" AssociatedControlID="ZipCode" Text="PLZ:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="ZipCode" runat="server" ShowMoreResultsBox="True" Height="140px" 
                            OnInit="ZipCodes_ItemsRequested" 
                            AllowCustomText="true" MarkFirstMatch="true" 
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                        <br />
                       <asp:Label ID="lblZipCodeError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>    
                    </tr>
                       <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblCity" runat="server" AssociatedControlID="cmbCity" Text="Ort:"></asp:Label>
                        </td>
                        <td>
                              <telerik:RadComboBox runat="server" ID="cmbCity"  ShowMoreResultsBox="True" Height="140px" 
                              OnInit="Citys_ItemsRequested"  
                              AllowCustomText="true" MarkFirstMatch="true" ValidationGroup="Group1"></telerik:RadComboBox>
                        <br />
                       <asp:Label ID="lblCityError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                    </tr>
                       <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblCountry" runat="server" AssociatedControlID="cmbCountry" Text="Land:"></asp:Label>
                        </td>
                        <td>
                        <telerik:RadComboBox runat="server" ID="cmbCountry"  ShowMoreResultsBox="True" Height="140px" 
                        OnInit="Countrys_ItemsRequested"  
                        AllowCustomText="true" MarkFirstMatch="true" ValidationGroup="Group1"  ></telerik:RadComboBox>
                       <br />
                       <asp:Label ID="lblErrorCountry" runat="server" CssClass="Validator" ></asp:Label>                         
                        </td>
                    </tr>
                    <tr class="FormContainer">
                          <td valign="top">
                            <asp:Label ID="lblLargeCustomerNumber" runat="server" AssociatedControlID="txbLargeCustomerNumber" Text="Kundennummer:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLargeCustomerNumber" runat="server" 
                                ValidationGroup="Group1">
                            </telerik:RadTextBox>
                               <br />
                       <asp:Label ID="lblLargeCustomerNumberError" runat="server" CssClass="Validator" ></asp:Label>
                         </td>
                    </tr>
                        <tr class="FormContainer">
                          <td valign="top">
                            <asp:Label ID="lblEVBNUMBERINFO" runat="server" AssociatedControlID="txbLargeCustomerNumber" Text="eVB-Nummer:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbEvbNumber" runat="server"  MaxLength="7" 
                                ValidationGroup="Group1">
                            </telerik:RadTextBox>
                               <br />
                       <asp:Label ID="lblEvbNumberError" runat="server" CssClass="Validator" ></asp:Label>
                         </td>
                    </tr>
                      <tr class="FormContainer">
                       <td valign="top">
                            <asp:Label ID="lblPhoneNumber" runat="server" AssociatedControlID="PhoneNumer" Text="Telefonnummer:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="PhoneNumer" runat="server" 
                                ValidationGroup="Group1">
                            </telerik:RadTextBox></td>
                    </tr>
                       <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblFax" runat="server" AssociatedControlID="txbFax" Text="Fax:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbFax" runat="server" 
                                ValidationGroup="Group1">
                            </telerik:RadTextBox>
                            <br/>
                        </td>
                    </tr>
                       <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblMobil" runat="server" AssociatedControlID="txbMobil" Text="Mobil:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbMobil" runat="server" 
                                ValidationGroup="Group1">
                            </telerik:RadTextBox>
                            <br/>
                        </td>                         
                    <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txbEmailAdress" Text="Email:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbEmailAdress" runat="server" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                     </td>
                    </tr>                
                        <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblLargeCustomerZahlungsziel" runat="server"  Text="Zahlungsziel:" 
                            AssociatedControlID="txbLargeCustomerZahlungsziel"></asp:Label>
                        </td>
                        <td>
                        <telerik:RadComboBox ID="txbLargeCustomerZahlungsziel" runat="server" ValidationGroup="Group1" >
                        <Items>
                        <telerik:RadComboBoxItem Value="0" Text="Sofort" />
                         <telerik:RadComboBoxItem Value="5" Text="5" />
                          <telerik:RadComboBoxItem Value="10" Text="10" />
                        </Items>
                        </telerik:RadComboBox>
                        </td>
                        <tr class="FormContainer">
                            <td valign="top">
                                <asp:Label ID="Label7" runat="server" 
                                    AssociatedControlID="txbSmallCustomerZahlungsziel" Text="Rechnungstyp:"></asp:Label>
                            </td>
                            <td>
                            <telerik:RadComboBox ID="rcbInvoiceType" OnInit="rcbInvoiceType_ItemsRequested" DataTextField="Value" DataValueField="Key"  runat="server" 
                                    ValidationGroup="Group1">
                                    </telerik:RadComboBox>
                            </td>
                        </tr>
                    </tr>
                 <tr class="FormContainer">
                      <td colspan="2">
                      <asp:CheckBox runat="server" ID="chbSendToMainLocation" />
                    <asp:Label ID="lblSendToMainLocation" runat="server" AssociatedControlID="chbSendToMainLocation" Text="Rechnung senden an Hauptstandort"></asp:Label>
                  </td>
                    </tr>
                       <tr class="FormContainer">
                <td colspan="2">
                      <asp:CheckBox runat="server" ID="chbSendViaMail" />
                
                    <asp:Label ID="lblchbSendViaMail" runat="server" AssociatedControlID="chbSendToMainLocation" Text="Rechnung senden per Email"></asp:Label>
                      </td>
                    </tr>
                </tbody>
            </table>
                       <table  align="left" border="0" cellpadding="5" >
                <colgroup>
                    <col width="100"/>
                    <col width="100"/>
                </colgroup>
                <thead>
                    <tr> 
                        <td  colspan="2">                      
                            <asp:Label ID="TitleLabel2" runat="server" AssociatedControlID="PhoneNumer" Font-Bold="true"
                                Text="Kostenstelle"></asp:Label>
                        </td>                         
                    </tr>
                </thead>
                <tbody>
                    <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblCostCenterName" runat="server" AssociatedControlID="CostCenterName" Text="Name:"></asp:Label>
                        </td>
                        <td>                    
                            <asp:TextBox ID="CostCenterName" runat="server" ValidationGroup="Group2">
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            </asp:TextBox>
                             <br />
                       <asp:Label ID="lblCostCenterNameError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                    </tr>                      
                    <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblBankName" runat="server" AssociatedControlID="txbBankName" Text="Kreditinstitut:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txbBankName" runat="server"  ValidationGroup="Group2">
                            </asp:TextBox>
                                 <br />
                       <asp:Label ID="lblBankNameError" runat="server" CssClass="Validator" ></asp:Label> 
                  </td>
                    </tr>
                        <tr class="FormContainer">
                         <td>
                            <asp:Label ID="lblAccountNumber" runat="server" AssociatedControlID="txbAccount" Text="Kontonummer:"  AutoCompleteType="Disabled"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox  ID="txbAccount" runat="server"  ValidationGroup="Group2" >
                            </telerik:RadTextBox >                            
                    <br />
                       <asp:Label ID="lblAccountError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                    </tr>
                       <tr class="FormContainer">
                          <td>
                            <asp:Label ID="lblBankCode" runat="server" AssociatedControlID="txbBankCode" Text="BLZ:" ></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox  ID="txbBankCode" runat="server"  ValidationGroup="Group2" >
                            </telerik:RadTextBox >
                          <br />
                       <asp:Label ID="lblBankCodeError" runat="server" CssClass="Validator" ></asp:Label>                   
                        </td>
                        </tr>
                        <tr class="FormContainer">       
                         <td>
                            <asp:Label ID="Label2" runat="server" Text="IBAN"  AssociatedControlID="txbSmallCustomerBankCode"
                                 AutoCompleteType="Disabled"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox  ID="txbLargeCustomerIBAN" runat="server"  
                                ValidationGroup="Group2" >
                            </telerik:RadTextBox >                
                    <br />                
                        </td>
                       </tr>
                       <tr class="FormContainer">
                        <td>
                               <asp:Label ID="Label8" runat="server" Text="BIC"  AssociatedControlID="txbSmallCustomerBankCode"
                                 AutoCompleteType="Disabled"></asp:Label>
                        </td>
                        <td>
                             <telerik:RadTextBox  ID="txbLargeCustomerBIC" runat="server"  
                                ValidationGroup="Group2" >
                            </telerik:RadTextBox >
                          <telerik:RadButton ID="btnLargeCustomerGenerateIBAN" runat="server"  
                            Text="IBAN/BIC" ToolTip="IBAN Nummer generieren" onclick="genIbanLargeCustomer_Click"></telerik:RadButton>
                            <asp:Label ID="Label5" runat="server" ForeColor="Red" Font-Bold="true" AssociatedControlID="PhoneNumer" ></asp:Label> 
                            <br />
                        </td>
                    </tr>
                       <tr class="FormContainer">                   
                        <td >
                            <asp:Label ID="lblLargeCustomerVatInfo" runat="server" Text="MwSt:" AssociatedControlID="txbLargeCustomerVat" ></asp:Label>
                        </td>
                             <td >
                     <telerik:RadTextBox ID="txbLargeCustomerVat" runat="server" Text="19" ValidationGroup="Group1" >
                            </telerik:RadTextBox> 
                     <br />
                       <asp:Label ID="lblLargeCustomerErrorVat" runat="server" CssClass="Validator" ></asp:Label>
                        </td>
                    </tr>
                    <tr class="FormContainer">
              
                     <td valign="top">
                            <asp:Label ID="lblLargeCustomerCostCenterNummber" runat="server" AssociatedControlID="txbLargeCustomerNumber" Text="Kostenstellennummer:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLargeCustomerCostCenterNumber" runat="server" 
                                ValidationGroup="Group1">
                            </telerik:RadTextBox>
                                <br />
                       <asp:Label ID="lblLargeCustomerCostCenterNummberError" runat="server" CssClass="Validator" ></asp:Label>                            
                            </td>
                    </tr>
                      <tr >                               
                        <td colspan="2">
                         <asp:Label ID="lblLargeCustomerSendArt" runat="server" Font-Bold="true"  AssociatedControlID="txbLargeCustomerNumber" 
                         Text="Versand bei Auftragserledigung"></asp:Label>
                        </td>
                    </tr>     
					   <tr>
                         <td>
                    <asp:Label ID="lblAuftragserledigungWhere" runat="server" Font-Bold="true"  
                    AssociatedControlID="txbLargeCustomerNumber" Text="Ziel:  "></asp:Label>             
             </td>
             <td>             
                    <asp:Label ID="lblAuftragserledigungTime" runat="server" Font-Bold="true"  AssociatedControlID="txbLargeCustomerNumber" 
                    Text="Zeitpunkt:"></asp:Label>     
                   </td>  
              </tr>
                       <tr class="FormContainer">
                          <td>
                         <div style="margin-top:-45px; ">
                             <asp:CheckBox ID="chbLCustomerAuftragKunde" runat="server" Text="Kunde" /><br />
                             <asp:CheckBox ID="chbLCustomerAuftragStandort" runat="server" Text="Standort" />
                          </div>
                        </td>
                       <td>                  
                             <asp:RadioButton ID="LCustomerAuftragHourly" runat="server" Text="stündlich" GroupName="LCZeitpunkt" />
                        <br />
                             <asp:RadioButton ID="LCustomerAuftragDayly" runat="server" Text="täglich" GroupName="LCZeitpunkt" />
                             <br />
                              <asp:RadioButton ID="LCustomerAuftragNow" runat="server" Text="sofort" Checked="true" GroupName="LCZeitpunkt" />
                                <br />
                              <asp:RadioButton ID="LCustomerAuftragNoInfo" runat="server" Text="kein" Checked="true" GroupName="LCZeitpunkt" />                             
                        </td>
                    </tr>
                    <tr class="FormContainer">                         
                       <td colspan="2">                          
                         <asp:Label ID="lblLargeCustomerSendLieferschein" runat="server" Font-Bold="true"  AssociatedControlID="txbLargeCustomerNumber" 
                         Text="Versand bei Lieferscheinerstellung"></asp:Label>               
                           <br />
                        </td>
                    </tr>                
                        <tr >
                           <td>
                    <asp:Label ID="lbllargeCustomerLieferscheinZiel" runat="server" Font-Bold="true"  AssociatedControlID="txbLargeCustomerNumber" Text="Ziel: ">
                    </asp:Label>    
                   </td>
                    </tr>
                 <tr >
                      <td>                        
                             <asp:CheckBox ID="chbLCustomerLiefescheinCustomer" runat="server" Text="Kunde" /><br />
                             <asp:CheckBox ID="chbLCustomerLieferscheinStandort" runat="server" Text="Standort" />                             
                        </td>
                    </tr>
                </tbody>
            </table>
        <table border="0" cellpadding="5"  align="left">
            <colgroup>
            </colgroup>
            <thead>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblRechnungsadresse" runat="server" 
                            AssociatedControlID="PhoneNumer" Font-Bold="true" Text="Rechnungsadresse"></asp:Label>
                    </td>
                </tr>
            </thead>
            <tr class="FormContainer">
                <td colspan="2">
                    <asp:CheckBox ID="chbInvoiceSameAsAdress" runat="server" AutoPostBack="true" 
                        Checked="true" GroupName="CustomerType" 
                        OnCheckedChanged="chbInvoiceSameAsAdress_Checked" 
                        Text="Rechnungsadresse wie Standartadresse" />
                </td>
            </tr>
                    <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblLargeCustomerIAStreet" runat="server" 
                                AssociatedControlID="txbLargeCustomerIAStreet" Text="Strasse /Nr.:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbLargeCustomerIAStreet" runat="server" 
                                ValidationGroup="Group1" Width="120px">
                            </telerik:RadTextBox>
                            <telerik:RadTextBox ID="txbLargeCustomerIAStreetNr" runat="server" 
                                ValidationGroup="Group1" Width="35px">
                            </telerik:RadTextBox>
                            <br />
                            <asp:Label ID="lblLargeCustomerIAStreetError" runat="server" 
                                CssClass="Validator"></asp:Label>
                        </td>
            </tr>
            <tr class="FormContainer">
                <td valign="top">
                    <asp:Label ID="lblLargeCustomerIAZipCode" runat="server" 
                        AssociatedControlID="ZipCode" Text="PLZ:"></asp:Label>
                </td>
                <td>
                    <telerik:RadComboBox ID="cmbLargeCustomerIAZipCode" runat="server" 
                        AllowCustomText="true"  Height="140px" 
                        MarkFirstMatch="true" OnInit="ZipCodes_ItemsRequested" 
                        ShowMoreResultsBox="True" ValidationGroup="Group1">
                    </telerik:RadComboBox>
                    <br />
                    <asp:Label ID="lblLargeCustomerIAZipCodeError" runat="server" 
                        CssClass="Validator"></asp:Label>
                </td>
            </tr>
            <tr class="FormContainer">
                <td valign="top">
                    <asp:Label ID="lblIAOrt" runat="server" AssociatedControlID="cmbCountry" 
                        Text="Ort:"></asp:Label>
                </td>
                <td>
                    <telerik:RadComboBox ID="cmbIAOrt" runat="server" AllowCustomText="true" 
                        Height="140px" MarkFirstMatch="true" 
                        OnInit="Citys_ItemsRequested" ShowMoreResultsBox="True" 
                        ValidationGroup="Group1">
                    </telerik:RadComboBox>
                    <br />
                    <asp:Label ID="lblcmbIAOrtError" runat="server" CssClass="Validator"></asp:Label>
                </td>
            </tr>
            <tr class="FormContainer">
                <td valign="top">
                    <asp:Label ID="lblCountyIAInfo" runat="server" AssociatedControlID="cmbCountry" 
                        Text="Land:"></asp:Label>
                </td>
                <td>
                    <telerik:RadComboBox ID="cmbCountryIA" runat="server" AllowCustomText="true" 
                      Height="140px" MarkFirstMatch="true" 
                        OnInit="Countrys_ItemsRequested" ShowMoreResultsBox="True" 
                        ValidationGroup="Group1">
                    </telerik:RadComboBox>
                    <br />
                    <asp:Label ID="lblCountyIAInfoError" runat="server" CssClass="Validator"></asp:Label>
                </td>
            </tr>
            <tr class="FormContainer">
                <td>
                </td>
                <td>
                </td>
            </tr>
    </table> 
    <table border="0"  align="left" cellpadding="5">
        <colgroup>
        </colgroup>
        <thead>
            <tr>
                <td colspan="2">
                            <asp:Label ID="lblVersandadresseSmallCustomer" runat="server" 
                        AssociatedControlID="PhoneNumer" Font-Bold="true" Text="Versandadresse"></asp:Label>
                </td>
            </tr>
        </thead>
        <tr class="FormContainer">
            <td colspan="2">
                <asp:CheckBox ID="chbSameAsDispatch" runat="server" AutoPostBack="true" 
                    Checked="true" GroupName="CustomerType" 
                    OnCheckedChanged="chbSameAsDispatch_Checked" 
                    Text="Versandadresse wie Rechnungsadresse" />                   
            </td>
     </tr>
        <tr class="FormContainer">
            <td>
                <asp:Label ID="Label3" runat="server" 
                    AssociatedControlID="txbLargeCustomerSendStreet" Text="Strasse /Nr.:"></asp:Label>
            </td>
            <td>
                <telerik:RadTextBox ID="txbLargeCustomerSendStreet" runat="server" 
                    ValidationGroup="Group1" Width="120px">
                </telerik:RadTextBox>
                <telerik:RadTextBox ID="txbLargeCustomerSendStreetNr" runat="server" 
                    ValidationGroup="Group1" Width="35px">
                </telerik:RadTextBox>
                <br />
                <asp:Label ID="lblLargeCustomerSendStreetError" runat="server" 
                    CssClass="Validator"></asp:Label>
            </td>
        </tr>
        <tr class="FormContainer">
            <td valign="top">
                <asp:Label ID="lblLargeCustomerSendZipCode" runat="server" 
                    AssociatedControlID="ZipCode" Text="PLZ:"></asp:Label>
            </td>
            <td>
                <telerik:RadComboBox ID="cmbLargeCustomerSendZipCode" runat="server" 
                    AllowCustomText="true"  Height="140px" 
                    MarkFirstMatch="true" OnInit="ZipCodes_ItemsRequested" 
                    ShowMoreResultsBox="True" ValidationGroup="Group1">
                </telerik:RadComboBox>
                <br />
                <asp:Label ID="lblLargeCustomerSendZipCodeError" runat="server" 
                    CssClass="Validator"></asp:Label>
            </td>
        </tr>
        <tr class="FormContainer">
            <td valign="top">
                <asp:Label ID="lblSendOrtInfo" runat="server" AssociatedControlID="cmbSendOrt" 
                    Text="Ort:"></asp:Label>
            </td>
            <td>
                <telerik:RadComboBox ID="cmbSendOrt" runat="server" AllowCustomText="true" 
                   Height="140px" MarkFirstMatch="true" 
                    OnInit="Citys_ItemsRequested" ShowMoreResultsBox="True" 
                    ValidationGroup="Group1">
                </telerik:RadComboBox>
                <br />
                <asp:Label ID="lblSendOrtInfoError" runat="server" CssClass="Validator"></asp:Label>
            </td>
        </tr>
        <tr class="FormContainer">
            <td valign="top">
                <asp:Label ID="lblCountrySend" runat="server" AssociatedControlID="cmbCountry" 
                    Text="Land:"></asp:Label>
            </td>
            <td>
                <telerik:RadComboBox ID="cmbCountrySend" runat="server" AllowCustomText="true" 
                   Height="140px" MarkFirstMatch="true" 
                    OnInit="Countrys_ItemsRequested" ShowMoreResultsBox="True" 
                    ValidationGroup="Group1">
                </telerik:RadComboBox>
                <br />
                <asp:Label ID="lblCountrySendError" runat="server" CssClass="Validator"></asp:Label>
            </td>
        </tr>
        <tr class="FormContainer">
            <td>
            </td>
            <td>
                <telerik:RadButton ID="RadButton4" runat="server" CssClass="bSaveButton" 
                    OnClick="bSaveClick" Text="Speichern" AutoPostBack="true">
                </telerik:RadButton>
            </td>
        </tr>
    </table>
    </asp:Panel>
          <asp:Panel id="CreateSmallCustomerTable"   Visible="false"  runat="server"  >
            <table  border="0" cellpadding="5" align="left">
                <colgroup>
                </colgroup>
                <thead>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="Label1" runat="server"  Font-Bold="true" AssociatedControlID="PhoneNumer" 
                                Text="Stammdaten"></asp:Label>
                        </td>
                    </tr>
                </thead>
                <tbody>
                    <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblSmallCustomerVorname" runat="server" Text="Vorname:" AssociatedControlID="txbSmallCustomerVorname"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbSmallCustomerVorname" runat="server" 
                                ValidationGroup="Group1">
                            </telerik:RadTextBox>
                             <br />
                            <asp:Label ID="lblSmallCustomerFirstNameError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                    </tr>
                    <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblSmallCustomerNachname" runat="server" Text="Nachname:" AssociatedControlID="txbSmallCustomerNachname"></asp:Label>
                        </td>
                        <td>
                          <telerik:RadTextBox ID="txbSmallCustomerNachname" runat="server" ValidationGroup="Group1" 
                             >
                            </telerik:RadTextBox>     
                         <br />
                       <asp:Label ID="lblSmallCustomerNachnameError" runat="server" CssClass="Validator" ></asp:Label>                       
                        </td>
                    </tr>
                        <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblSmallCustomerTitle" runat="server" AssociatedControlID="txbSmallCustomerTitle"
                                Text="Titel:"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbSmallCustomerTitle" runat="server" 
                                ValidationGroup="Group1">
                            </telerik:RadTextBox>
                        <br />
                        </td>
                    </tr>
                       <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblGender" runat="server" Text="Geschlecht:"  AssociatedControlID="cmbSmallCustomerGender"></asp:Label>
                        </td>
                        <td>
                              <telerik:RadComboBox runat="server" ID="cmbSmallCustomerGender"  AllowCustomText="True" 
                                  MarkFirstMatch="True" ValidationGroup="Group1" Culture="de-DE" 
                                  Width="60px">
                                  <Items>
                                      <telerik:RadComboBoxItem runat="server" Text="M" Value="M" 
                                          Owner="cmbSmallCustomerGender" />
                                      <telerik:RadComboBoxItem runat="server" Text="W" Value="W" 
                                          Owner="cmbSmallCustomerGender" />
                                  </Items>
                              </telerik:RadComboBox>
                        <br />
                        </td>
                    </tr>
                       <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblSmallCustomerStreetNr" runat="server" Text="Strasse /Nr:" AssociatedControlID="txbSmallCustomerStreet"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbSmallCustomerStreet" runat="server" ValidationGroup="Group1" Width="120px">
                            </telerik:RadTextBox>     <telerik:RadTextBox ID="txbSmallCustomerNr" runat="server" Width="35px" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                       <br />
                            <asp:Label ID="lblSmallCustomerStreetError" runat="server"  CssClass="Validator"  ></asp:Label>
                        </td>
                    </tr>
                    <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblSmallCustomerZipCode" runat="server" Text="PLZ:" AssociatedControlID="txbSmallCustomerZipCode"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="txbSmallCustomerZipCode" ShowMoreResultsBox="True" Height="140px" 
                            OnInit="ZipCodes_ItemsRequested" 
                             runat="server" AllowCustomText="true" MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                        <br />
                       <asp:Label ID="lblSmallCustomerZipCodeError" runat="server" CssClass="Validator" ></asp:Label> 
                            </tr>
                       <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblSmallCustomerCity" runat="server"  Text="Stadt:" AssociatedControlID="cmbSmallCustomerCity"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbSmallCustomerCity" runat="server"  ShowMoreResultsBox="True" Height="140px" 
                            OnInit="Citys_ItemsRequested"  
                            AllowCustomText="true" MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                            <br/>
                           <asp:Label ID="lblSmallCustomerCityError" runat="server" CssClass="Validator" ></asp:Label>                      
                        </td>                     
                    </tr>
                       <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblSmallCustomerCountry" runat="server"  Text="Land:" AssociatedControlID="cmbSmallCustomerCountry"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbSmallCustomerCountry" ShowMoreResultsBox="True" Height="140px" 
                            OnInit="Countrys_ItemsRequested"
                            runat="server" AllowCustomText="true" MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                            <br/>
                           <asp:Label ID="lblSmallCustomerCountryError" runat="server" CssClass="Validator" ></asp:Label>                      
                        </td>
                    </tr>
                        <tr class="FormContainer">
                        <td valign="top">
                            <asp:Label ID="lblSmallCustomerNumber" runat="server"  Text="Kundennummer:" AssociatedControlID="txbSmallCustomerNumber"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbSmallCustomerNumber" runat="server" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                       <br/>
                           <asp:Label ID="lblSmallCustomerNumberError" runat="server" CssClass="Validator" ></asp:Label>                                            
                        </td>
                    </tr
                    <tr class="FormContainer">
                    </tr>
                          <tr>
                        <td valign="top">
                            <asp:Label ID="lblSmallCustomerPhone" runat="server"  Text="Telefon:" 
                                AssociatedControlID="txbSmallCustomerPhone"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox ID="txbSmallCustomerPhone" runat="server" 
                                ValidationGroup="Group1">
                            </telerik:RadTextBox>
                        </td></tr>
                              <tr class="FormContainer">
                                  <td valign="top">
                                      <asp:Label ID="lblSmallCustomerFax" runat="server" 
                                          AssociatedControlID="txbSmallCustomerFax" Text="Fax:"></asp:Label>
                                  </td>
                                  <td>
                                      <telerik:RadTextBox ID="txbSmallCustomerFax" runat="server" 
                                          ValidationGroup="Group1">
                                      </telerik:RadTextBox>
                                  </td>
                              </tr>
                              <tr class="FormContainer">
                                  <td valign="top">
                                      <asp:Label ID="lblSmallCustomerMobil" runat="server" 
                                          AssociatedControlID="txbSmallCustomerMobil" Text="Mobil:"></asp:Label>
                                  </td>
                                  <td>
                                      <telerik:RadTextBox ID="txbSmallCustomerMobil" runat="server" 
                                          ValidationGroup="Group1">
                                      </telerik:RadTextBox>
                                  </td>
                              </tr>
                              <tr class="FormContainer">
                                  <td valign="top">
                                      <asp:Label ID="lblSmallCustomerEmail" runat="server" 
                                          AssociatedControlID="txbSmallCustomerEmail" Text="Email:"></asp:Label>
                                  </td>
                                  <td>
                                      <telerik:RadTextBox ID="txbSmallCustomerEmail" runat="server" 
                                          ValidationGroup="Group1">
                                      </telerik:RadTextBox>
                                  </td>
                              </tr>
                              <tr class="FormContainer">
                                  <td valign="top">
                                      <asp:Label ID="lblSmallCustomerZahlungsziel" runat="server" 
                                          AssociatedControlID="txbSmallCustomerZahlungsziel" Text="Zahlungsziel:"></asp:Label>
                                  </td>
                                  <td>
                                  <telerik:RadComboBox ID="txbSmallCustomerZahlungsziel" runat="server" 
                                          ValidationGroup="Group1">
                                            <Items>
                                        <telerik:RadComboBoxItem Value="0" Text="Sofort" />
                                        <telerik:RadComboBoxItem Value="5" Text="5" />
                                        <telerik:RadComboBoxItem Value="10" Text="10" />
                                            </Items>
                                          </telerik:RadComboBox>
                                  </td>
                              </tr>
                </tbody>
            </table>
              <table  border="0" cellpadding="5" align="left">
                <colgroup>                  
                </colgroup>
                <thead>
                    <tr>
                        <td colspan="2"> 
                            <asp:Label ID="Label4" runat="server" Font-Bold="true" AssociatedControlID="PhoneNumer"
                                Text="Bankverbindung"></asp:Label>
                        </td>
                    </tr>
                </thead>
                <tbody>
                    <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblSmallCustomerCostCenter" runat="server" AssociatedControlID="txbSmallCustomerKreditinstitut"
                                Text="Kreditinstitut:"></asp:Label>
                        </td>
                        <td>                    
                            <asp:TextBox ID="txbSmallCustomerKreditinstitut" runat="server" 
                                ValidationGroup="Group2"></asp:TextBox>
                             <br />
                        </td>
                    </tr>
                    <tr class="FormContainer">
                        <td>
                            <asp:Label ID="lblSmallCustomerAccountNumber" runat="server"  Text="Kontonummer"  AssociatedControlID="txbSmallCustomerAccountNumber"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txbSmallCustomerAccountNumber" runat="server"    
                                ValidationGroup="Group2"></asp:TextBox>
                                 <br />
                        </td>
                    </tr>
                        <tr class="FormContainer">
                         <td>
                            <asp:Label ID="lblSmallCustomerBankCode" runat="server" Text="Bankleitzahl"  AssociatedControlID="txbSmallCustomerBankCode"
                                 AutoCompleteType="Disabled"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox  ID="txbSmallCustomerBankCode"   runat="server"  
                                ValidationGroup="Group2" >
                            </telerik:RadTextBox >                            
                    <br />                
                        </td>           
                    </tr>
                     <tr class="FormContainer">       
                         <td>
                            <asp:Label ID="lblSmallCustomerIbanInfo" runat="server" Text="IBAN"  AssociatedControlID="txbSmallCustomerBankCode"
                                 AutoCompleteType="Disabled"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadTextBox  ID="txbSmallCustomerIBANinfo" runat="server"  
                                ValidationGroup="Group2" >
                            </telerik:RadTextBox >                            
                    <br />                
                        </td>
                    </tr>
                     <tr class="FormContainer">
                        <td>
                             <asp:Label ID="Label9" runat="server" Text="BIC"  AssociatedControlID="txbSmallCustomerBankCode"
                              AutoCompleteType="Disabled"></asp:Label>
                        </td>
                        <td>
                             <telerik:RadTextBox  ID="txbSmallCustomerBIC" runat="server"  
                                ValidationGroup="Group2" >
                            </telerik:RadTextBox >
                            <telerik:RadButton ID="genIban" runat="server"  
                            Text="IBAN" ToolTip="IBAN Nummer generieren" onclick="genIban_Click"></telerik:RadButton>
                            <asp:Label ID="lblSmallCustomerIBAN_Error" runat="server" ForeColor="Red" Font-Bold="true" AssociatedControlID="PhoneNumer" ></asp:Label> 
                            <br />
                        </td>
                     </tr>
                       <tr class="FormContainer">
                          <td>
                             <asp:Label ID="lblSmallCustomerVat" runat="server" Text="MwSt:"  AssociatedControlID="cmbSmallCustomerGender"></asp:Label>
                            </td>
                        <td>
                         <telerik:RadTextBox ID="txbSmallCustomerVat" runat="server" ValidationGroup="Group1" Text="19">
                            </telerik:RadTextBox> <br />
                        </td>
                    </tr>
                       <tr class="FormContainer">
                         <td></td>
                           <td></td>
                    </tr>
                    <tr class="FormContainer">
                        <td>
                        </td>
                        <td>
                            </tr>
                </tbody>
            </table>      
          <table  border="0" cellpadding="5" align="left" >
                <colgroup>
                </colgroup>
                <thead>
                    <tr>
                             <td colspan="2">
                            <asp:Label ID="Label6" runat="server" Font-Bold="true" AssociatedControlID="PhoneNumer"
                                Text="Rechnungsadresse"></asp:Label> 
                        </td>
                    </tr>
                </thead>
                <tbody>
                    <tr class="FormContainer">
                        <td colspan="2">
                            <asp:CheckBox ID="chbSmallCustomerRechnungsaderesse" runat="server" Checked="true" Text="Versandadresse wie Standartadersse"  GroupName="CustomerType"
                            AutoPostBack="true"  OnCheckedChanged="chbSmallCustomerStandartadresse_Checked" />                       
                      </td>
                    </tr>
                    <tr class="FormContainer">
                                    <td>
                            <asp:Label ID="lblSmallCustomerRechnungsStrasse" runat="server" AssociatedControlID="txbsmallCustomerInvoiceStreet" Text="Strasse /Nr.:"></asp:Label>
                        </td>
                        <td>
                              <telerik:RadTextBox ID="txbsmallCustomerInvoiceStreet" runat="server" ValidationGroup="Group1" Width="120px">
                            </telerik:RadTextBox>     <telerik:RadTextBox ID="txbsmallCustomerInvoiceStreetNr" runat="server" Width="35px" ValidationGroup="Group1">
                            </telerik:RadTextBox>
                                 <br />
                       <asp:Label ID="lblSmallCustomerRechnungsStrasseError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                    </tr>
                        <tr class="FormContainer">
                         <td>
                            <asp:Label ID="lblSmallCustomerInvoicePLZ" runat="server" Text="PLZ:" AssociatedControlID="cmbInvoiceZC"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbInvoiceZC" runat="server" ShowMoreResultsBox="True" Height="140px" 
                            OnInit="ZipCodes_ItemsRequested" 
                            AllowCustomText="true" MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                                <br />
                       <asp:Label ID="lblSmallCustomerInvoicePLZError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                    </tr>
                       <tr class="FormContainer">
                           <td>
                            <asp:Label ID="lblSmallCustomerInvoiceCity" runat="server" Text="Stadt:" AssociatedControlID="cmbSmallCustomerInvoiceCity"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbSmallCustomerInvoiceCity" runat="server" AllowCustomText="true"
                            OnInit="Citys_ItemsRequested" ShowMoreResultsBox="True" Height="140px" 
                             MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>                    
                                <br />
                       <asp:Label ID="cmbSmallCustomerInvoiceCityError" runat="server" CssClass="Validator" ></asp:Label>                      
                        </td>
                    </tr>
                       <tr class="FormContainer">
                             <td>
                            <asp:Label ID="lblSmallCustomerInvoiceCountry" runat="server" Text="Land:" AssociatedControlID="cmbSmallCustomerInvoiceCountry"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbSmallCustomerInvoiceCountry" runat="server"               
                            OnInit="Countrys_ItemsRequested"  ShowMoreResultsBox="True" Height="140px" 
                            AllowCustomText="true" MarkFirstMatch="true"
                                ValidationGroup="Group1">
                            </telerik:RadComboBox>
                             <br />
                       <asp:Label ID="lblSmallCustomerInvoiceCountryError" runat="server" CssClass="Validator" ></asp:Label> 
                        </td>
                    </tr>
                </tbody>
            </table>
              <table border="0" cellpadding="5" align="left">
                  <colgroup>
                  </colgroup>
                  <thead>
                      <tr>
                          <td colspan="2">
                              <asp:Label ID="lblVersandadresse" runat="server" 
                                  AssociatedControlID="PhoneNumer" Font-Bold="true" Text="Versandadresse:"></asp:Label>
                          </td>
                      </tr>
                  </thead>
                  <tr class="FormContainer">
                      <td colspan="2">
                          <asp:CheckBox ID="chbSmallCustomerVersandadresse" runat="server" 
                              AutoPostBack="true" Checked="true" GroupName="CustomerType" 
                              OnCheckedChanged="chbSmallCustomerVersandadresse_Checked" 
                              Text="Versandadresse wie Rechnungsadresse" />
                      </td>
                  </tr>
                  <tr class="FormContainer">
                      <td>
                          <asp:Label ID="lblSmallCustomerSendStreet" runat="server" 
                              AssociatedControlID="txbsmallCustomerInvoiceStreet" Text="Strasse /Nr.:"></asp:Label>
                      </td>
                      <td>
                          <telerik:RadTextBox ID="txbSmallCustomerSendStreet" runat="server" 
                              ValidationGroup="Group1" Width="120px">
                          </telerik:RadTextBox>
                          <telerik:RadTextBox ID="txbSmallCustomerSendStreetNr" runat="server" 
                              ValidationGroup="Group1" Width="35px">
                          </telerik:RadTextBox>
                          <br />
                          <asp:Label ID="lblSmallCustomerSendStreetError" runat="server" 
                              CssClass="Validator"></asp:Label>
                      </td>
                  </tr>
                  <tr class="FormContainer">
                      <td>
                          <asp:Label ID="lblSmallCustomerSendZip" runat="server" 
                              AssociatedControlID="cmbSmallCustomerSendZip" Text="PLZ:"></asp:Label>
                      </td>
                      <td>
                          <telerik:RadComboBox ID="cmbSmallCustomerSendZip" runat="server" 
                              AllowCustomText="true"  Height="140px" 
                              MarkFirstMatch="true" OnInit="ZipCodes_ItemsRequested" 
                              ShowMoreResultsBox="True" ValidationGroup="Group1">
                          </telerik:RadComboBox>
                          <br />
                          <asp:Label ID="lblSmallCustomerSendZipError" runat="server" 
                              CssClass="Validator"></asp:Label>
                      </td>
                  </tr>
                  <tr class="FormContainer">
                      <td>
                          <asp:Label ID="lblSmallCustomerSendCity" runat="server" 
                              AssociatedControlID="cmbSmallCustomerSendCity" Text="Stadt:"></asp:Label>
                      </td>
                      <td>
                          <telerik:RadComboBox ID="cmbSmallCustomerSendCity" runat="server" 
                              AllowCustomText="true" Height="140px" 
                              MarkFirstMatch="true" OnInit="Citys_ItemsRequested" 
                              ShowMoreResultsBox="True" ValidationGroup="Group1">
                          </telerik:RadComboBox>
                          <br />
                          <asp:Label ID="lblSmallCustomerSendCityError" runat="server" 
                              CssClass="Validator"></asp:Label>
                      </td>
                  </tr>
                  <tr class="FormContainer">
                      <td>
                          <asp:Label ID="lblSmallCustomerSendCountry" runat="server" 
                              AssociatedControlID="cmbSmallCustomerSendCountry" Text="Land:"></asp:Label>
                      </td>
                      <td>
                          <telerik:RadComboBox ID="cmbSmallCustomerSendCountry" runat="server" 
                              AllowCustomText="true"  Height="140px" 
                              MarkFirstMatch="true" OnInit="Countrys_ItemsRequested" 
                              ShowMoreResultsBox="True" ValidationGroup="Group1">
                          </telerik:RadComboBox>
                          <br />
                          <asp:Label ID="lblSmallCustomerSendCountryError" runat="server" 
                              CssClass="Validator"></asp:Label>
                      </td>
                  </tr>
                  <tr class="FormContainer">
                      <td>
                      </td>
                      <td>
                          <telerik:RadButton ID="RadButton3" runat="server" CssClass="bSaveButton" 
                              OnClick="bSaveClick" Text="Speichern"  AutoPostBack="true">
                          </telerik:RadButton>
                      </td>
                  </tr>
              </table>
      </asp:Panel>        
      </telerik:RadAjaxPanel>
        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelCustomer" runat="server">
        </telerik:RadAjaxLoadingPanel>
</body>
</html>