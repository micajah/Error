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

public partial class ApplicationErrorsByTypes : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            Master.PageTitle = "Application Errors by Exception Types";
            Master.ErrorTracketTitle = "Application Errors by Error Types : ";
        }
        if (null == Request.QueryString["appid"] ||
            0 == Request.QueryString["appid"].Length)
        {
            Response.Redirect("~/Default.aspx");
        }
        dsApplication.ApplicationDataTable dtApplication = new dsApplication.ApplicationDataTable();
        dsApplicationTableAdapters.ApplicationTableAdapter oAdapter = new dsApplicationTableAdapters.ApplicationTableAdapter();
        oAdapter.Fill(dtApplication);
        try
        {
            dsApplication.ApplicationRow dr = dtApplication.FindByApplicationID(Convert.ToInt32(Request.QueryString["AppID"]));
            if (null != dr)
            {
                Master.ErrorTracketTitle += dr.Name;
            }
        }
        catch { }
    }
    protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "clear":
                string strRes = e.CommandArgument.ToString();

                dsErrorTableAdapters.QueriesTableAdapter adapter = new dsErrorTableAdapters.QueriesTableAdapter();
                adapter.DeleteErrorsByCategory(Convert.ToInt32(Request.QueryString["AppID"]), strRes);

                Response.Redirect(Request.Url.ToString());
                break;
        }
    }
    protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ImageButton l = (ImageButton)e.Row.FindControl("ImageButton2");
            l.Attributes.Add("onclick", "javascript:return " +
            "confirm('Are you sure you want to delete " +
            DataBinder.Eval(e.Row.DataItem, "Name") + " errors ')");
            l.CommandArgument = DataBinder.Eval(e.Row.DataItem, "Name").ToString();
        }
    }
}
