<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShowOrder.ascx.cs" Inherits="KVSWebApplication.Auftragsbearbeitung_Neuzulassung.ShowOrder" %>

<link href="../Styles/styles.css" type="text/css" />
<div runat="server" class="uebersichtDiv" id="myUebersicht">


<telerik:RadGrid runat="server" ID="AuftragsUebersicht" DataSourceID="AuftragsDataSource">

</telerik:RadGrid>
<asp:LinqDataSource runat=server ID="AuftragsDataSource" OnSelecting="AuftragsDataSource_Selecting" ></asp:LinqDataSource>
</div>

