using System;
using System.Web.UI.WebControls;

public partial class ShowByTime : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            Master.PageTitle = "Application Errors by Time";
            Master.ErrorTracketTitle = "Application Errors by Time : ";
        }
        if (null == Request.QueryString["appid"] ||
            0 == Request.QueryString["appid"].Length)
        {
            Response.Redirect("~/Default.aspx");
        }
        errorTypesSqlDataSource.SelectParameters[1].DefaultValue = DateTime.Now.AddDays(-6).ToLongDateString();
        dsApplication.ApplicationDataTable dtApplication = new dsApplication.ApplicationDataTable();
        dsApplicationTableAdapters.ApplicationTableAdapter oAdapter = new dsApplicationTableAdapters.ApplicationTableAdapter();
        oAdapter.Fill(dtApplication);
        try
        {
            dsApplication.ApplicationRow dr = dtApplication.FindByApplicationID(Convert.ToInt32(Request.QueryString["AppID"]));
            if (null != dr)
            {
                if (Master != null)
                {
                    Master.ErrorTracketTitle += dr.Name;
                }
            }
        }
        catch { }
    }

    protected void Delete_Click(object sender, EventArgs e)
    {
        int errorID;
        CheckBox CurCheckBox;
        dsErrorTableAdapters.ErrorTableAdapter adapter = new dsErrorTableAdapters.ErrorTableAdapter();
        foreach (GridViewRow CurRow in GridView2.Rows)
        {
            CurCheckBox = ((CheckBox)CurRow.Cells[0].Controls[1]);
            if (CurCheckBox.Checked)
            {
                HiddenField hfErrorID = (HiddenField)CurRow.Cells[0].FindControl("hfErrorID");
                if (hfErrorID != null)
                {
                    errorID = int.Parse(hfErrorID.Value);
                    adapter.Delete(errorID);
                }
            }
        }
        Response.Redirect(Request.Url.ToString());
    }

    protected void InvertButton_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow CurRow in GridView2.Rows)
        {
            ((CheckBox)CurRow.Cells[0].Controls[1]).Checked = !(((CheckBox)CurRow.Cells[0].Controls[1]).Checked);
        }
    }

    protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "MailClick")
        {
            int ErrorID = -1;
            HiddenField hfErrorID = (HiddenField)GridView2.Rows[int.Parse(e.CommandArgument.ToString())].Cells[0].FindControl("hfErrorID");
            if (hfErrorID != null)
            {
                if (int.TryParse(hfErrorID.Value, out ErrorID))
                {
                    bool SendResult = Micajah.ErrorTracker.BWDMailClass.SendMailToBWD(ErrorID);
                    if (SendResult)
                    {
                        Page.ClientScript.RegisterClientScriptBlock(typeof(ShowByTime), "__BWDMessageFinished",
                                                                    "<script language='javascript' type='text/javascript'>alert('E-mail sent succesfully.');</script>");
                    }
                    else
                    {
                        Page.ClientScript.RegisterClientScriptBlock(typeof(ShowByTime), "__BWDMessageFinished",
                                                                    "<script language='javascript' type='text/javascript'>alert('Warn, e-mail sent unsuccesfully!');</script>");
                    }
                }
            }
        }
    }
}