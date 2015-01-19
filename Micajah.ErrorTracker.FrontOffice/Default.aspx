<%@ Page Language="C#" MasterPageFile="ErrorTracker.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<%@ MasterType VirtualPath="ErrorTracker.master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4"
            DataSourceID="SqlDataSource1" DataKeyNames="ApplicationID" ForeColor="#333333" GridLines="None" OnRowCommand="GridView1_RowCommand" OnRowDataBound="GridView1_RowDataBound">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <Columns>
                <asp:HyperLinkField DataNavigateUrlFields="ApplicationID" DataNavigateUrlFormatString="ApplicationErrors.aspx?AppID={0}"
                    DataTextField="Name" HeaderText="Application Name" />
                <asp:BoundField DataField="ErrorsCount" HeaderText="Errors Count" ReadOnly="True"
                    SortExpression="ErrorsCount" >
                    <ItemStyle HorizontalAlign=Center />
                </asp:BoundField>
                <asp:BoundField DataField="LastExceptionTime" HeaderText="Last" ReadOnly="True" SortExpression="LastExceptionTime" />
                <asp:TemplateField HeaderText="Clear">
                    <ItemTemplate>
                        <asp:ImageButton ID="ImageButton1" runat="server" PostBackUrl="~/Default.aspx.cs" CausesValidation="false" CommandName="clear"
                            ImageUrl="~/Images/delete.gif" />
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
            SelectCommand="SELECT A.ApplicationID, A.Name, MAX(E.Date) AS LastExceptionTime, COUNT(*) AS ErrorsCount FROM dbo.Error AS E INNER JOIN dbo.Application AS A ON A.ApplicationID = E.ApplicationID GROUP BY A.ApplicationID, A.Name ORDER BY A.Name" DataSourceMode="DataReader" DeleteCommand="DELETE Error&#13;&#10;WHERE ApplicationID =0">
        </asp:SqlDataSource>
    
    </div>
</asp:Content>
