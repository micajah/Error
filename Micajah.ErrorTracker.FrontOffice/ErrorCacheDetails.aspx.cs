using System;
using System.Data;

public partial class ErrorCacheDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            Master.PageTitle = "Error Cache Details";
        }
        string etTitle = "Error Cache Details";

        if (ErrorID > 0)
        {
            dsErrorTableAdapters.ErrorTableAdapter adapter = new dsErrorTableAdapters.ErrorTableAdapter();
            dsError.ErrorDataTable table = adapter.GetErrorByID(ErrorID);
            if (table.Rows.Count > 0)
            {
                etTitle = string.Format("Source File : {0}", table.Rows[0]["Name"]);
            }
        }
        else
        {
            etTitle = string.Format("Source File : {0}", Request.QueryString["path"]);
        }

        if (Master != null)
        {
            Master.ErrorTracketTitle = etTitle;
        }
        if(!Page.IsPostBack)
        {
            dsError.ErrorDataTable ds = new dsError.ErrorDataTable();
            dsErrorTableAdapters.ErrorTableAdapter adapter = new dsErrorTableAdapters.ErrorTableAdapter();
            ds = adapter.GetErrorByID(ErrorID);
            if (ds.Rows.Count > 0)
            {
                DataRow ErrorDBInfo = ds.Rows[0];
                if (!ErrorDBInfo.IsNull("Cache"))
                {
                    tdCache.InnerHtml = ErrorDBInfo["Cache"].ToString();
                }
            }
        }
    }

    private int ErrorID
    {
        get
        {
            if (Request.QueryString["ErrorID"] != null)
            {
                return int.Parse(Request.QueryString["ErrorID"]);
            }
            return 0;
        }
    }
}