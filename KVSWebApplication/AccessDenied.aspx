<%@ Page Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="KVSWebApplication.AccessDenied" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:ScriptManager ID="ManagerDenied" runat="server"></asp:ScriptManager>
<table width="100%">
<tr style="text-align:center; color:Red; font-size:x-large; font-style:italic; font-weight:bold;">
<td>
Zugriff verweigert!
</td>
</tr>
<tr>
<td style="text-align:center; color:Black; font-style:italic; font-weight:bold;">
Sie verfügen nicht über die notwendigen Rechte
</td>
</tr>
<tr>
<td style="text-align:center; color:Black; font-style:italic; font-weight:bold;">
Bitte informieren Sie Ihren Systemadministrator
</td>
</tr>
</table>
</asp:Content>