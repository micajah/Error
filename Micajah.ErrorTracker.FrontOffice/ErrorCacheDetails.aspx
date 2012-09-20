<%@ Page Title="" Language="C#" MasterPageFile="~/ErrorTracker.master" AutoEventWireup="true" CodeFile="ErrorCacheDetails.aspx.cs" Inherits="ErrorCacheDetails" %>
<%@ MasterType VirtualPath="ErrorTracker.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
        <table class="ET_CacheTable" border="1">
            <tr>
                <td class="ET_CacheTableHeader">Cache:</td>
            </tr>
            <tr>
                <td id="tdCache" runat="server">
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

