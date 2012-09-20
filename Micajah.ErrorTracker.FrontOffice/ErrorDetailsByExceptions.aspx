<%@ Page Language="C#" MasterPageFile="ErrorTracker.master" AutoEventWireup="true" CodeFile="ErrorDetailsByExceptions.aspx.cs" Inherits="ErrorDetailsByExceptions" %>
<%@ MasterType VirtualPath="ErrorTracker.master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>        
        <asp:Button ID="Delete" runat="server" Text="Delete selected items" OnClick="Delete_Click" />&nbsp;&nbsp;
        <asp:Button ID="InvertButton" runat="server" OnClick="InvertButton_Click" Text="Invert selection" />&nbsp;&nbsp;
        <asp:Button ID="btnShowErrorList" runat="server" OnClick="btnShowErrorList_Click" Text="Show Error List" ToolTip="Show error list with this exception type" />
        <br /><br />
        <asp:GridView ID="GridView1" runat="server" CellPadding="4" DataSourceID="SqlDataSource1"
            ForeColor="#333333" GridLines="None" AllowSorting="True" AutoGenerateColumns="False" EmptyDataText="No error found"
             AllowPaging="true" PageSize="40" OnRowCommand="GridView1_RowCommand">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#EFF3FB" />
            <EditRowStyle BackColor="#2461BF" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:TemplateField HeaderText="Select">
                    <ItemTemplate>
                        &nbsp;<asp:CheckBox ID="DelCheckBox" runat="server" Text="  " />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:HyperLinkField DataNavigateUrlFields="ApplicationID,path,ErrorID" DataNavigateUrlFormatString="ErrorDetails.aspx?AppID={0}&amp;path={1}&amp;ErrorID={2}&amp;rb=2"
                    DataTextField="Path" DataTextFormatString="{0}" HeaderText="Source File" SortExpression="Path" />
                <asp:BoundField DataField="ErrorLineNumber" HeaderText="Line Number" SortExpression="ErrorLineNumber" >
                    <ItemStyle HorizontalAlign=Center />
                </asp:BoundField>
                <asp:BoundField DataField="Date" HeaderText="Date" SortExpression="Date" />
                <asp:BoundField DataField="ErrorID" HeaderText="ErrorID" InsertVisible="False" SortExpression="ErrorID" />
                <asp:ButtonField ButtonType="Button" HeaderText="Mail to BWD" Text="Send e-mail" CommandName="MailClick" />
            </Columns>
        </asp:GridView>
    
    </div>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:mits_errortracker_ConnectionString %>"
            SelectCommand="SELECT ApplicationID, ErrorID, Name, SourceFile, Path, ErrorLineNumber, Date FROM Error WHERE (ApplicationID = @ApplID) AND (Name = @ErrorName) ORDER BY Date DESC">
            <SelectParameters>
                <asp:QueryStringParameter DefaultValue="1" Name="ApplID" QueryStringField="AppID" />
                <asp:QueryStringParameter DefaultValue="Uknown" Name="ErrorName" QueryStringField="ErrorName" />
            </SelectParameters>
        </asp:SqlDataSource>
</asp:Content>
