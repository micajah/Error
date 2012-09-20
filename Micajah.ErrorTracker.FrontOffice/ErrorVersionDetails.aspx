<%@ Page Title="" Language="C#" MasterPageFile="~/ErrorTracker.master" AutoEventWireup="true" CodeFile="ErrorVersionDetails.aspx.cs" Inherits="ErrorVersionDetails" %>
<%@ MasterType VirtualPath="ErrorTracker.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
        <table class="ET_CacheTable" border="1">
            <tr>
                <td class="ET_CacheTableHeader">Version:</td>
            </tr>
            <tr>
                <td id="tdVersion" runat="server">
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

