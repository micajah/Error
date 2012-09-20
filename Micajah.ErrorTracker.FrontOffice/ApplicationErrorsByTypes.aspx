<%@ Page Language="C#" MasterPageFile="ErrorTracker.master" AutoEventWireup="true" CodeFile="ApplicationErrorsByTypes.aspx.cs" Inherits="ApplicationErrorsByTypes" %>
<%@ MasterType VirtualPath="ErrorTracker.master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
        <asp:GridView ID="GridView2" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="errorTypesSqlDataSource" OnRowCommand="GridView2_RowCommand" OnRowDataBound="GridView2_RowDataBound" EmptyDataText="No error found"
         AllowPaging="true" PageSize="40">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#EFF3FB" />
            <EditRowStyle BackColor="#2461BF" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:HyperLinkField DataNavigateUrlFields="ApplicationID,Name" DataNavigateUrlFormatString="ErrorDetailsByExceptions.aspx?AppID={0}&amp;ErrorName={1}"
                    DataTextField="Name" DataTextFormatString="{0}" HeaderText="Error Type" SortExpression="Name" />
                <asp:BoundField DataField="ErrorsCount" HeaderText="Errors Count" ReadOnly="True"
                    SortExpression="ErrorsCount" >
                    <ItemStyle HorizontalAlign=Center />
                </asp:BoundField>
                <asp:BoundField DataField="LastExceptionTime" HeaderText="Last" ReadOnly="True"
                    SortExpression="LastExceptionTime" />
                <asp:TemplateField HeaderText="Clear">
                    <ItemTemplate>
                    <asp:ImageButton ID="ImageButton2" runat="server" CausesValidation="false" CommandName="clear"
                            CommandArgument="" ImageUrl="~/images/delete.gif" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    
    </div>
        <asp:SqlDataSource ID="errorTypesSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:mits_errortracker_ConnectionString %>" SelectCommand="SELECT ApplicationID, Name, MAX(Date) AS LastExceptionTime, COUNT(ErrorID) AS ErrorsCount FROM dbo.Error WHERE (ApplicationID = @ApplID) GROUP BY ApplicationID, Name&#13;&#10;ORDER BY LastExceptionTime DESC">
            <SelectParameters>
                <asp:QueryStringParameter DefaultValue="1" Name="ApplID" QueryStringField="AppID" />
            </SelectParameters>
        </asp:SqlDataSource>
</asp:Content>
