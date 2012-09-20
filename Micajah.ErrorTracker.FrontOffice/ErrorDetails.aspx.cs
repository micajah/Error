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

public partial class ErrorDetails : System.Web.UI.Page
{
    protected bool IsSignificant(string sQueryPart)
    {
        if(null == sQueryPart ||
            0 == sQueryPart.Length)
            return false;

        return true;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            Master.PageTitle = "Error Details";
        }
        string etTitle = "Error Details";

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

        if ((Request.QueryString["path"] == "all") && (Request.QueryString["ErrorID"] == "-1"))
        {
            etTitle = "All Errors in Application";
        }
        if ((Request.QueryString["path"] == "all") && (Request.QueryString["ErrorID"] == "-2"))
        {
            etTitle = "All Errors with Type : " + Request.QueryString["ErrorName"];
        }
        if (Request.QueryString["ErrorID"] == "-3")
        {
            etTitle = "All Errors in Source File : " + Request.QueryString["path"];
        }

        if (Master != null)
        {
            Master.ErrorTracketTitle = etTitle;
        }
    }

    protected void DataList1_ItemCommand(object source, DataListCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "clear":
                int iErrorID = Convert.ToInt32((e.Item.FindControl("ImageBtn") as ImageButton).CommandArgument);
                dsApplicationTableAdapters.QueriesTableAdapter adapter = new dsApplicationTableAdapters.QueriesTableAdapter();
                adapter.DeleteError(iErrorID);
                Response.Redirect("~/Default.aspx");
                break;
        }
    }
    protected void SqlDataSource1_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
    {
        if ((Request.QueryString["path"] == "all") && (ConvertHelper.GetIntValue(Request.QueryString["ErrorID"]) == -1))
        {
            e.Command.CommandText = "SELECT * FROM [Error] WHERE ([ApplicationID] = @ApplicationID)";             
        }
        if ((Request.QueryString["path"] == "all") && (ConvertHelper.GetIntValue(Request.QueryString["ErrorID"]) == -2))
        {
            e.Command.CommandText = "SELECT * FROM [Error] WHERE ([ApplicationID] = @ApplicationID) AND ([Error].[Name] = '" + Request.QueryString["ErrorName"] + "')"; 
        }
        if (ConvertHelper.GetIntValue(Request.QueryString["ErrorID"]) == -3)
        {
            e.Command.CommandText = "SELECT * FROM [Error] WHERE (([ApplicationID] = @ApplicationID) AND ([Path] = @Path))";
        }
        if (ConvertHelper.GetIntValue(Request.QueryString["ErrorID"]) > 0)
        {
            e.Command.CommandText = string.Format("SELECT * FROM [Error] WHERE [ErrorID] = {0}", Request.QueryString["ErrorID"]);
        }
    }

    protected void DataList1_ItemDataBound(object sender, DataListItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView _r = (DataRowView)e.Item.DataItem;
            if (_r != null)
            {
                decimal cacheSize = 0;
                decimal.TryParse(_r["CacheSize"].ToString(), out cacheSize);
                if (cacheSize > 0)
                {
                    HtmlTableCell td = (HtmlTableCell)e.Item.FindControl("tdCache");
                    if (td != null)
                    {
                        td.InnerHtml = cacheSize.ToString() + "KB";
                    }
                }
            }
            SetLink(e.Item, "Version");
        }
    }

    private void SetLink(DataListItem item, string name)
    {
        if (DataBinder.Eval(item.DataItem, name).ToString().Length > 0)
        {
            HtmlTableCell td = (HtmlTableCell)item.FindControl("td" + name);
            if (td != null)
            {
                td.InnerHtml = " &nbsp;&nbsp;<a href='Error" + name + "Details.aspx?AppID=" + AppID + "&ErrorID=" + ErrorID + "'>See Details</a>";
            }
        }
    }

    private int AppID
    {
        get
        {
            if (Request.QueryString["AppID"] != null)
            {
                return int.Parse(Request.QueryString["AppID"]);
            }
            return 0;
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
