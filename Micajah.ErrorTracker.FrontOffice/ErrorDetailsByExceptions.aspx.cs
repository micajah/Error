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

public partial class ErrorDetailsByExceptions : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Master != null)
        {
            Master.PageTitle = "Errors, group by Error Types<";
            Master.ErrorTracketTitle = "Error Type : " + Request.QueryString["ErrorName"];
        }
    }
    protected void Delete_Click(object sender, EventArgs e)
    {        
        int errorID;
        dsErrorTableAdapters.ErrorTableAdapter adapter = new dsErrorTableAdapters.ErrorTableAdapter();        
        foreach (GridViewRow CurRow in GridView1.Rows)
        {            
            if (((CheckBox)CurRow.Cells[0].Controls[1]).Checked)
            {
                errorID = int.Parse(CurRow.Cells[4].Text);
                adapter.Delete(errorID);
            }
        }        
        Response.Redirect(Request.Url.ToString());
    }

    protected void InvertButton_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow CurRow in GridView1.Rows)
        {
            ((CheckBox)CurRow.Cells[0].Controls[1]).Checked = !(((CheckBox)CurRow.Cells[0].Controls[1]).Checked);
        }     
    }

    protected void btnShowErrorList_Click(object sender, EventArgs e)
    {
        Response.Redirect("ErrorDetails.aspx?appID=" + Request.QueryString["appid"]
            + "&path=all&ErrorID=-2&ErrorName=" + Request.QueryString["ErrorName"] + "&rb=2");
    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "MailClick")
        {
            int ErrorID = -1;
            if (int.TryParse(GridView1.Rows[int.Parse(e.CommandArgument.ToString())].Cells[4].Text, out ErrorID))
            {
                bool SendResult = Micajah.ErrorTracker.BWDMailClass.SendMailToBWD(ErrorID);
                if (SendResult)
                {
                    Page.ClientScript.RegisterClientScriptBlock(typeof(ErrorDetailsByExceptions), "__BWDMessageFinished",
                                                                "<script language='javascript' type='text/javascript'>alert('E-mail sent succesfully.');</script>");
                }
                else
                {
                    Page.ClientScript.RegisterClientScriptBlock(typeof(ErrorDetailsByExceptions), "__BWDMessageFinished",
                                                                "<script language='javascript' type='text/javascript'>alert('Warn, e-mail sent unsuccesfully!');</script>");
                }
            }
        }
    }
}
