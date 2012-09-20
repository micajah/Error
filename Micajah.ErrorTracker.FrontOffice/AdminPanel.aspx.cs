using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class AdminPanel : System.Web.UI.Page
{   

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            Master.PageTitle = "Admin Panel";
            Master.ErrorTracketTitle = "Micajah Error Tracker :: Admin Panel";
        }
        ApllicationDropDownList.BackColor = System.Drawing.Color.White;        
    }
    protected void ApllicationDropDownList_DataBound(object sender, EventArgs e)
    {        
        ApllicationDropDownList.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select", "0"));        
    }
    protected void OneAppSqlDataSource_Inserted(object sender, SqlDataSourceStatusEventArgs e)
    {        
        ApllicationDropDownList.DataBind();
        ApllicationDropDownList.SelectedValue = e.Command.Parameters["@AppID"].Value.ToString();
    }
    protected void AppDetailsView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
    {
        WarnLabel.Text = "";
        if (e.NewValues[0] == null)
        {
            e.Cancel = true;
            WarnLabel.Text = "Field Name can't be empty!";
        }
        if (((bool)e.NewValues[2] == true) && (e.NewValues[7] == null))
        {
            e.Cancel = true;
            WarnLabel.Text = WarnLabel.Text + " " + "Field SMTP Server can't be empty, if Field Send Mail is checked!";
        }
    }
    protected void AppDetailsView_ItemInserting(object sender, DetailsViewInsertEventArgs e)
    {
        WarnLabel.Text = "";
        if (e.Values[0] == null)
        {
            e.Cancel = true;
            WarnLabel.Text = "Field Name can't be empty";
        }
        if (((bool)e.Values[2] == true) && (e.Values[7] == null))
        {
            e.Cancel = true;
            WarnLabel.Text = WarnLabel.Text + " " + "Field SMTP Server can't be empty, if Field Send Mail is checked!";
        }
    }
    protected void AppDetailsView_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
    {
        if (e.AffectedRows == 0)
        {
            e.KeepInEditMode = true;
            AppDetailsView.DataBind();
            WarnLabel.Text = "Fields have not been updated, because another user just updated data of this application before your query!";
        }
    }
    protected void AddButton_Click(object sender, EventArgs e)
    {
        AppDetailsView.ChangeMode(DetailsViewMode.Insert);
    }   
}
