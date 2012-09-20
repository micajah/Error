<%@ Page Language="C#" MasterPageFile="ErrorTracker.master" AutoEventWireup="true" CodeFile="ErrorDetailsByPath.aspx.cs" Inherits="ErrorDetailsByPath" %>
<%@ MasterType VirtualPath="ErrorTracker.master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
        <asp:Button ID="Delete" runat="server" OnClick="Delete_Click" Text="Delete selected items" />&nbsp;&nbsp;
        <asp:Button ID="InvertButton" runat="server" OnClick="InvertButton_Click" Text="Invert selection" />&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnShowAll" runat="server" OnClick="btnShowAll_Click" Text="Show all Application Errors" Title="Show all Application Errors in this Source File"/>
        <br />
        <br />
        <asp:GridView ID="GridView1" runat="server" AllowSorting="True"
            AutoGenerateColumns="False" DataSourceID="ErrorSqlDataSource" CellPadding="4" ForeColor="#333333" GridLines="None" EmptyDataText="No error found" OnRowCommand="GridView1_RowCommand"  AllowPaging="true" PageSize="30">
            <Columns>
                <asp:TemplateField HeaderText="Select">
                    <ItemTemplate>
                        <asp:CheckBox ID="DelCheckBox" runat="server" Text="  " />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:HyperLinkField DataNavigateUrlFields="ApplicationID,path,ErrorID" DataNavigateUrlFormatString="ErrorDetails.aspx?AppID={0}&amp;path={1}&amp;ErrorID={2}&amp;rb=1"
                    DataTextField="Name" DataTextFormatString="{0}" HeaderText="Error Type" />
                <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                <asp:BoundField DataField="ErrorLineNumber" HeaderText="Line Number" SortExpression="ErrorLineNumber">
                <ItemStyle HorizontalAlign=Center />
                </asp:BoundField>
                <asp:BoundField DataField="Date" HeaderText="Date" SortExpression="Date" />
                <asp:BoundField DataField="ErrorID" HeaderText="ErrorID" InsertVisible="False" SortExpression="ErrorID" />
                <asp:ButtonField ButtonType="Button" HeaderText="Mail to BWD" Text="Send e-mail" CommandName="MailClick" />
            </Columns>
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#EFF3FB" />
            <EditRowStyle BackColor="#2461BF" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
    
    </div>
        <asp:SqlDataSource ID="ErrorSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:mits_errortracker_ConnectionString %>"
            SelectCommand="SELECT ApplicationID, ErrorID, Name, Description, SourceFile, Path, ErrorLineNumber, Date FROM Error WHERE (ApplicationID = @ApplID) AND (Path = @Path) ORDER BY Date DESC">
            <SelectParameters>
                <asp:QueryStringParameter DefaultValue="1" Name="ApplID" QueryStringField="AppID" />
                <asp:QueryStringParameter DefaultValue="" Name="Path" QueryStringField="path" />
            </SelectParameters>
        </asp:SqlDataSource>
</asp:Content>
