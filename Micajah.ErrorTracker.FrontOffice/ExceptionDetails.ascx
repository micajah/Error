<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ExceptionDetails.ascx.cs" Inherits="ExceptionDetails" %>
&nbsp;<table style="width: 379px">
    <tr>
        <td>
            Date</td>
        <td><% =ExceptionDate %>
        </td>
    </tr>
    <tr>
        <td>
            Browser</td>
        <td><% =ExceptionBrowser %>
        </td>
    </tr>
    <tr>
        <td>
            Name</td>
        <td> <% =ExceptionName %>
        </td>
    </tr>
</table>
<br />
