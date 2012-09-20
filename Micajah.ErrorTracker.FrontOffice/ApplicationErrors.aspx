<%@ Page Language="C#" MasterPageFile="ErrorTracker.master" AutoEventWireup="true" CodeFile="ApplicationErrors.aspx.cs" Inherits="ApplicationErrors" %>
<%@ MasterType VirtualPath="ErrorTracker.master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4"
            DataSourceID="SqlDataSource1" ForeColor="#333333" GridLines="None" AllowSorting="True" OnRowCommand="GridView1_RowCommand" OnRowDataBound="GridView1_RowDataBound"
            EmptyDataText="No error found" AllowPaging="true" PageSize="30">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <Columns>
                <asp:TemplateField HeaderText="Source File" SortExpression="Path">
                    <ItemTemplate>
                        <div class="ET_Wrapped" style="width:300px;"><asp:HyperLink ID="hlErrorDetails" runat="server" NavigateUrl='<%# "ErrorDetailsByPath.aspx?appID=" + DataBinder.Eval(Container, "DataItem.ApplicationID") + "&path=" + DataBinder.Eval(Container, "DataItem.Path")%>'
                            Text='<%#DataBinder.Eval(Container, "DataItem.Path")%>'/></div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Name" HeaderText="Error Type" SortExpression="Name" />
                <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" >
                    <ItemStyle Width="300px"/>
                </asp:BoundField>
                <asp:BoundField DataField="ErrorLineNumber" HeaderText="Line Number" SortExpression="ErrorLineNumber">
                <ItemStyle HorizontalAlign=Center />
                </asp:BoundField>
                <asp:BoundField DataField="ErrorsCount" DataFormatString="{0}" HeaderText="Errors Count">
                <ItemStyle HorizontalAlign=Center />
                </asp:BoundField>
                <asp:BoundField DataField="LastExceptionTime"
                    HeaderText="Last" ReadOnly="True" SortExpression="LastExceptionTime" />
                <asp:TemplateField HeaderText="Clear" ShowHeader="False">
                    <ItemTemplate>
                        <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="false" CommandName="clear"
                            CommandArgument="" ImageUrl="~/images/delete.gif" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" />
            <EditRowStyle BackColor="#2461BF" />
        </asp:GridView>
        
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:mits_errortracker_ConnectionString %>"
            SelectCommand="SELECT ApplicationID, SourceFile, Path, Name, Description, ErrorLineNumber, MAX(Date) AS LastExceptionTime, COUNT(ErrorID) AS ErrorsCount FROM dbo.Error WHERE (ApplicationID = @ApplID) GROUP BY ApplicationID, SourceFile, Path, Name, Description, ErrorLineNumber&#13;&#10;ORDER BY LastExceptionTime DESC">
            <SelectParameters>
                <asp:QueryStringParameter DefaultValue="1" Name="ApplID" QueryStringField="AppID" />
            </SelectParameters>
        </asp:SqlDataSource>       
    </div>    
</asp:Content>
