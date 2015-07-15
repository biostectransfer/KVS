<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomerDetails.ascx.cs" Inherits="KVSWebApplication.Customer.CustomerDetails" %>

<div runat="server" class="uebersichtDiv" id="myUebersicht">


<telerik:radgrid id="getAllCustomer" runat="server" DataSourceID="GetAllCustomerDataSource">
 <PagerStyle Mode="NextPrevAndNumeric" AlwaysVisible="true"></PagerStyle>
        <MasterTableView  DataKeyNames="Id"  AllowPaging="true" AllowMultiColumnSorting="True"  CommandItemDisplay="Top" ShowGroupFooter="true"
            GroupLoadMode="Server">
        <%--    <Columns>
            <telerik:GridBoundColumn  HeaderButtonType="TextButton"
                    DataField="Id" UniqueName="Id" Visible="false" >
                </telerik:GridBoundColumn>
            
            <telerik:GridBoundColumn  HeaderButtonType="TextButton"
                    DataField="Name" UniqueName="Name" HeaderText="Kundenname >
                </telerik:GridBoundColumn>


             </Columns>--%>
                <ItemStyle BackColor="#DFDFDF" />

            <HeaderStyle BackColor="#FFFFFF" ForeColor="#767676" />

            <AlternatingItemStyle BackColor="#FFFFFF" />
        </MasterTableView>
        <ClientSettings AllowDragToGroup="true">
        </ClientSettings>

</telerik:radgrid>
<asp:LinqDataSource runat=server ID="GetAllCustomerDataSource" OnSelecting="GetAllCustomerDataSource_Selecting" ></asp:LinqDataSource>
</div>