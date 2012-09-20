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

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            Master.PageTitle = "Error Applications";
            Master.ErrorTracketTitle = "Micajah Error Tracker :: Tracking Applications";
        }
        if (0 == GridView1.Rows.Count)
        {
            Response.Output.WriteLine("NO NEW ERRORS");
        }
    }

    protected void LineBinding()
    { 
        
    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        { 
            case "clear":
                int iAppID = Convert.ToInt32(e.CommandArgument);
                dsApplicationTableAdapters.QueriesTableAdapter adapter = new dsApplicationTableAdapters.QueriesTableAdapter();
                adapter.DeleteApplicationErrors(iAppID);

                Response.Redirect("~/Default.aspx");
                break;
        }
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ImageButton l = (ImageButton)e.Row.FindControl("ImageButton1");
            l.Attributes.Add("onclick", "javascript:return " +
            "confirm('Are you sure you want to delete " +
            DataBinder.Eval(e.Row.DataItem, "Name") + " errors')");
            l.CommandArgument = DataBinder.Eval(e.Row.DataItem, "ApplicationID").ToString();
        }
    }
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}
