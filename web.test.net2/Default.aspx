<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:CheckBox ID="MainCheckBox" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <br />
        <br />
        <asp:LinkButton ID="PostLinkButton" runat="server" OnClick="LinkButton1_Click">DoQueryString</asp:LinkButton>
        &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;&nbsp;<asp:HyperLink ID="LinkButton1" NavigateUrl="Goori.aspx" runat="server">Goori</asp:HyperLink>
        &nbsp;&nbsp;
        <br /><br />
        <asp:Button ID="SubmitButton" runat="server" OnClick="Button1_Click" Text="DivideByZeroException" /><br />
        <br />
        &nbsp;<asp:Button ID="Button1" runat="server" Text="VersionNotFoundException" OnClick="Button1_Click1" />
        <asp:HyperLink ID="HyperLink1" NavigateUrl="VeryLongNamePart1/VeryLongNamePart2/VeryLongNamePart3/VeryLongNamePart4/VeryLongNamePart5/VeryLongURL.aspx" runat="server">HyperLink</asp:HyperLink></div>
    </form>
</body>
</html>
