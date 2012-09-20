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

public partial class ApplicationErrors : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            Master.PageTitle = "Application Errors";
            Master.ErrorTracketTitle = "Application Errors by Source Files";
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
            if(null != dr)
            {
                if (Master != null)
                {
                    Master.ErrorTracketTitle += " : " + dr.Name;
                }
            }
        }
        catch { }
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ImageButton l = (ImageButton)e.Row.FindControl("ImageButton1");
            l.Attributes.Add("onclick", "javascript:return " +
            "confirm('Are you sure you want to delete " +
            DataBinder.Eval(e.Row.DataItem, "Name") + " errors at line " + DataBinder.Eval(e.Row.DataItem, "ErrorLineNumber") + " ')");
            l.CommandArgument = string.Format("{0},{1}", DataBinder.Eval(e.Row.DataItem, "Name").ToString(), DataBinder.Eval(e.Row.DataItem, "ErrorLineNumber").ToString());
        }
    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "clear":
                string[] strRes = e.CommandArgument.ToString().Split(new char[1]{','});
                string sExceptionName = strRes[0];
                int iErrorLine = Convert.ToInt32(strRes[1]);

                dsErrorTableAdapters.QueriesTableAdapter adapter = new dsErrorTableAdapters.QueriesTableAdapter();
                adapter.DeleteSpecErrors(sExceptionName, iErrorLine, Convert.ToInt32(Request.QueryString["AppID"]));

                Response.Redirect(Request.Url.ToString());
                break;
        }

    }    
}
