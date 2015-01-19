<%@ Page Language="C#" MasterPageFile="ErrorTracker.master" AutoEventWireup="true" CodeFile="AdminPanel.aspx.cs" Inherits="AdminPanel" %>
<%@ MasterType VirtualPath="ErrorTracker.master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div>
        <h4><asp:Button ID="AddButton" runat="server" Text="Add new Application" OnClick="AddButton_Click" /></h4><br />
        <asp:DropDownList ID="ApllicationDropDownList" runat="server" DataSourceID="AllApplicationDataSqlDataSource" DataTextField="Name" DataValueField="ApplicationID" OnDataBound="ApllicationDropDownList_DataBound" AutoPostBack="True" Width="404px">
        </asp:DropDownList>&nbsp;<br /><br />
        <asp:DetailsView ID="AppDetailsView" runat="server" AutoGenerateEditButton="True" AutoGenerateInsertButton="True"
            AutoGenerateRows="False" CellPadding="4" DataKeyNames="ApplicationID" DataSourceID="OneAppSqlDataSource"
            ForeColor="#333333" GridLines="None" EmptyDataText="Please, choose Application from dropdown list for Edit or Add new Application." HeaderText="Application Details" OnItemInserting="AppDetailsView_ItemInserting" OnItemUpdating="AppDetailsView_ItemUpdating" OnItemUpdated="AppDetailsView_ItemUpdated">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <CommandRowStyle BackColor="#D1DDF1" Font-Bold="True" />
            <EditRowStyle BackColor="#2461BF" Wrap="False" />
            <RowStyle BackColor="#EFF3FB" Wrap="True" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <Fields>
                <asp:BoundField DataField="ApplicationID" HeaderText="Application ID" InsertVisible="False"
                    ReadOnly="True" SortExpression="ApplicationID" />
                <asp:BoundField DataField="Name" HeaderText="Application Name" SortExpression="Name" >
                    <ControlStyle Width="750px" />
                </asp:BoundField>
                <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" >
                    <ControlStyle Width="750px" />
                </asp:BoundField>
                <asp:CheckBoxField DataField="SendEmail" HeaderText="Send Email?" SortExpression="SendEmail" />
                <asp:BoundField DataField="MailFrom" HeaderText="MailFrom" SortExpression="MailFrom" >
                    <ControlStyle Width="750px" />
                </asp:BoundField>
                <asp:BoundField DataField="MailTo" HeaderText="MailTo" SortExpression="MailTo" >
                    <ControlStyle Width="750px" />
                </asp:BoundField>
                <asp:BoundField DataField="MailBWD" HeaderText="MailBWD" SortExpression="MailBWD" >
                    <ControlStyle Width="750px" />
                </asp:BoundField>
                <asp:BoundField DataField="MailAdmin" HeaderText="MailAdmin" SortExpression="MailAdmin" >
                    <ControlStyle Width="750px" />
                </asp:BoundField>
                <asp:BoundField DataField="SMTPServer" HeaderText="SMTP Server" SortExpression="SMTPServer" Visible="false">
                    <ControlStyle Width="750px" />
                </asp:BoundField>
                <asp:CheckBoxField DataField="CacheItemsSize" HeaderText="Include Cache Details in E-mail" SortExpression="CacheItemsSize" />
            </Fields>
            <FieldHeaderStyle BackColor="#DEE8F5" Font-Bold="True" HorizontalAlign="Right" Width="200px" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" Wrap="False" />
            <AlternatingRowStyle BackColor="White" />
            <EmptyDataRowStyle Wrap="False" />
            <InsertRowStyle Wrap="False" />
        </asp:DetailsView>
            &nbsp;<asp:Label ID="WarnLabel" runat="server" EnableViewState="False" ForeColor="Red"></asp:Label>
        <asp:SqlDataSource ID="AllApplicationDataSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:mits_errortracker_ConnectionString %>"
            SelectCommand="SELECT * FROM Application Order By Name"></asp:SqlDataSource>
        <asp:SqlDataSource ID="OneAppSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:mits_errortracker_ConnectionString %>"
            SelectCommand="SELECT * FROM Application WHERE Application.ApplicationID=@ApplicationID" 
            InsertCommand="INSERT INTO Application (Name, Description, SendEmail, MailFrom, MailTo, MailBWD, MailAdmin, SMTPServer, CacheItemsSize) VALUES (@Name, @Description, @SendEmail, @MailFrom, @MailTo, @MailBWD, @MailAdmin, @SMTPServer, @CacheItemsSize); SELECT @AppID = SCOPE_IDENTITY();" 
            UpdateCommand="UPDATE Application SET Name = @Name, Description = @Description, SendEmail = @SendEmail, MailFrom = @MailFrom, MailTo = @MailTo, MailBWD = @MailBWD, MailAdmin = @MailAdmin, SMTPServer = @SMTPServer, CacheItemsSize = @CacheItemsSize WHERE (ApplicationID = @ApplicationID)" 
            OnInserted="OneAppSqlDataSource_Inserted">
            <SelectParameters>
                <asp:ControlParameter ControlID="ApllicationDropDownList" DefaultValue="1" Name="ApplicationID"
                    PropertyName="SelectedValue" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="Name" />
                <asp:Parameter Name="Description" />
                <asp:Parameter Name="SendEmail" />
                <asp:Parameter Name="MailFrom" />
                <asp:Parameter Name="MailTo" />
                <asp:Parameter Name="MailBWD" />
                <asp:Parameter Name="MailAdmin" />
                <asp:Parameter Name="SMTPServer" />
                <asp:Parameter Name="CacheItemsSize" />
                <asp:Parameter Name="ApplicationID" />
            </UpdateParameters>
            <InsertParameters>
                <asp:Parameter Name="Name" />
                <asp:Parameter Name="Description" />
                <asp:Parameter Name="SendEmail" />
                <asp:Parameter Name="MailFrom" />
                <asp:Parameter Name="MailTo" />
                <asp:Parameter Name="MailBWD" />
                <asp:Parameter Name="MailAdmin" />
                <asp:Parameter Name="SMTPServer" />
                <asp:Parameter Name="CacheItemsSize" />
                <asp:Parameter Name="AppID" Direction="Output" DefaultValue="0" Type="Int32" />
            </InsertParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
