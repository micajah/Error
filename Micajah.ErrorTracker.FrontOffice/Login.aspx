<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 95%;">
<head runat="server">
    <title>Log in</title>
    <link href="Style.css" type="text/css" rel="stylesheet" />
</head>
<body style="height: 100%;">
    <form id="form1" runat="server" style="width: 100%; height: 100%;">
    <table style="width: 100%; height: 100%;">
        <tr>
            <td>
    <table align="center" cellpadding="0" cellspacing="0" class="ET_LogIn">
        <tr>
            <td colspan="2" class="ET_LogInHeader">
                Log In
            </td>
            <td></td>
        </tr>
        <tr>
            <td class="ET_LogInTitle">
                User Name:
            </td>
            <td>
                <asp:TextBox ID="tbUserName" runat="server" MaxLength="50"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator id="rfvUserName" runat="server" controltovalidate="tbUserName"
                        display="Dynamic" enableclientscript="True" errormessage="Required Field."></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="ET_LogInTitle">
                Password:
            </td>
            <td>
                <asp:TextBox ID="tbPassword" runat="server" TextMode="Password" MaxLength="50"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator id="rfvPassword" runat="server" controltovalidate="tbPassword"
                        display="Dynamic" enableclientscript="True" errormessage="Required Field."></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td style="border-top: 1px solid #507CD1;">
            </td>
            <td align="right" style="border-top: 1px solid #507CD1;">
                <asp:Button ID="btnLogIn" runat="server" Text="Log In" OnClick="btnLogIn_Click" />
            </td>
            <td></td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Label ID="lblError" runat="server" Text="Your log in attempt was not successful. Please try again." CssClass="ET_LogInError" Visible="false"></asp:Label>
            </td>
        </tr>
    </table>   
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
