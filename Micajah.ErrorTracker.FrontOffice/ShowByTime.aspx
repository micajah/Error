<%@ Page Language="C#" MasterPageFile="ErrorTracker.master" AutoEventWireup="true" CodeFile="ShowByTime.aspx.cs" Inherits="ShowByTime" %>
<%@ MasterType VirtualPath="ErrorTracker.master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
        <asp:Button ID="Delete" runat="server" OnClick="Delete_Click" Text="Delete selected items" />&nbsp;&nbsp;
        <asp:Button ID="InvertButton" runat="server" OnClick="InvertButton_Click" Text="Invert selection" />&nbsp;&nbsp;&nbsp;&nbsp;
        <br />
        <br />
        <asp:GridView ID="GridView2" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AllowSorting="True" OnRowCommand="GridView2_RowCommand" AutoGenerateColumns="False" DataSourceID="errorTypesSqlDataSource" EmptyDataText="No error found"
         AllowPaging="true" PageSize="30">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#EFF3FB" />
            <EditRowStyle BackColor="#2461BF" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
            <EmptyDataRowStyle Font-Bold="true" Font-Size="16pt" />
            <Columns>
                <asp:TemplateField HeaderText="Select">
                    <ItemTemplate>
                        <asp:CheckBox ID="DelCheckBox" runat="server" Text="  " />
                        <asp:HiddenField ID="hfErrorID" runat="server" Value='<%#DataBinder.Eval(Container, "DataItem.ErrorID")%>'/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:HyperLinkField DataNavigateUrlFields="ApplicationID,path,ErrorID" DataNavigateUrlFormatString="ErrorDetails.aspx?AppID={0}&amp;path={1}&amp;ErrorID={2}&amp;rb=3"
                    DataTextField="Path" DataTextFormatString="{0}" HeaderText="Source File" />
                <asp:BoundField DataField="Name" HeaderText="Error Type" SortExpression="Name" />
                <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                <asp:BoundField DataField="ErrorLineNumber" HeaderText="Line Number" SortExpression="ErrorLineNumber" >
                <ItemStyle HorizontalAlign=Center />
                </asp:BoundField>
                <asp:BoundField DataField="Date" HeaderText="Date" SortExpression="Date" />
                <asp:ButtonField ButtonType="Button" HeaderText="Mail to BWD" Text="Send e-mail" CommandName="MailClick" />
            </Columns>
        </asp:GridView>
    
    </div>
        <asp:SqlDataSource ID="errorTypesSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:mits_errortracker_ConnectionString %>" SelectCommand="SELECT ApplicationID, ErrorID, Name, Description, SourceFile, Path, ErrorLineNumber, Date FROM Error WHERE (ApplicationID = @ApplID) AND Date > @Date ORDER BY Date DESC">
            <SelectParameters>
                <asp:QueryStringParameter DefaultValue="1" Name="ApplID" QueryStringField="AppID" />
                <asp:Parameter DbType="DateTime" Name="Date"/>
            </SelectParameters>
        </asp:SqlDataSource>
</asp:Content>
