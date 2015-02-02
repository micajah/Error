<%@ Page Language="C#" MasterPageFile="ErrorTracker.master" AutoEventWireup="true" CodeFile="ErrorDetails.aspx.cs" Inherits="ErrorDetails" %>
<%@ MasterType VirtualPath="ErrorTracker.master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
        <asp:DataList ID="DataList1" runat="server" DataKeyField="ErrorID" 
            DataSourceID="SqlDataSource1" ItemStyle-BackColor="#ccccff" 
            AlternatingItemStyle-BackColor="Wheat" OnItemCommand="DataList1_ItemCommand" 
            onitemdatabound="DataList1_ItemDataBound">
            <ItemTemplate>
                <table border="1">
                <tr>
                <td>ErrorID:</td>
                <td><%# Eval("ErrorID") %></td>
                </tr>
                <tr>
                <td>Date:</td>
                <td><%# Eval("Date") %></td>
                </tr>
                <tr>
                <td>Browser:</td>
                <td><%# Eval("Browser") %></td>
                </tr>
                <tr>
                <td>Exception Name:</td>
                <td><%# Eval("Name") %></td>
                </tr>
                <tr>
                <td>Description:</td>
                <td><%# Eval("Description") %></td>
                </tr>
                <tr>
                <td>URL:</td>
                <td><%# Eval("URL") %></td>
                </tr>
                <tr>
                <td>URL Referrer:</td>
                <td><%# Eval("URLReferrer") %></td>
                </tr>
                <tr>
                <td>Source File :</td>
                <td><%# Eval("SourceFile") %></td>
                </tr>
                <tr>
                <td>Error Line Number:</td>
                <td><%# Eval("ErrorLineNumber") %></td>
                </tr>
                <tr>
                <td>Query String:</td>
                <td><%# Eval("QueryString") %></td>
                </tr>
                <tr>
                <td>Query String Description:</td>
                <td><%# Eval("QueryStringDescription") %></td>
                </tr>
                <tr>
                <td>Machine Name:</td>
                <td><%# Eval("MachineName") %></td>
                </tr>
                <tr>
                <td>User IP Address:</td>
                <td><%# Eval("UserIPAddress") %></td>
                </tr>
                <tr>
                <td>Exception Type:</td>
                <td><%# Eval("ExceptionType") %></td>
                </tr>
                <tr>
                <td>Stack Trace:</td>
                <td><%# Eval("StackTrace") %></td>
                </tr>
                <tr>
                <td>Version:</td>
                <td id="tdVersion" runat="server"></td>
                </tr>
                <tr>
                <td>Request Cookies:</td>
                <td id="tdRequestCookies" runat="server"></td>
                </tr>
                <tr>
                <td>Request Header:</td>
                <td><%# Eval("RequestHeader") %></td>
                </tr>
                <tr>
                <td>Session:</td>
                <td><%# Eval("Session")%></td>
                </tr>
                <tr>
                <td>Application:</td>
                <td id="tdApplication" runat="server"></td>
                </tr>
                <tr>
                <td>Server Variables:</td>
                <td id="tdServerVariables" runat="server"></td>
                </tr>
                <tr>
                <td>Cache Size:</td>
                <td id="tdCache" runat="server"></td>
                </tr>
                <tr> 
                <td colspan="2"><asp:ImageButton ImageUrl="~/images/delete.gif" runat="server" CommandArgument='<%# Eval("ErrorID") %>' id="ImageBtn" CommandName="clear" />
                </td>
                </tr>
                </table>
                
            </ItemTemplate>
            <AlternatingItemStyle BackColor="Wheat" />
            <ItemStyle BackColor="#CCCCFF" />
        </asp:DataList><asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:mits_errortracker_ConnectionString %>"
            SelectCommand="SELECT * FROM [Error] WHERE (([ApplicationID] = @ApplicationID) AND ([Path] = @Path) AND ([ErrorID] = @ErrorID))" OnSelecting="SqlDataSource1_Selecting">
            <SelectParameters>
                <asp:QueryStringParameter DefaultValue="0" Name="ApplicationID" ConvertEmptyStringToNull="false" QueryStringField="appid" Type="Int32" />
                <asp:QueryStringParameter Name="Path" ConvertEmptyStringToNull="false" QueryStringField="path" Type="String" DefaultValue="" />
                <asp:QueryStringParameter DefaultValue="0" ConvertEmptyStringToNull="false" Name="ErrorID" QueryStringField="ErrorID" />
            </SelectParameters>
        </asp:SqlDataSource>
    
    </div>
</asp:Content>
