using System;
using System.Data;
using System.Web.Security;

public partial class ErrorTracker : System.Web.UI.MasterPage
{
    private string m_PageTitle = "Micajah Error Tracker";
    private string m_ErrorTracketTitle = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadMenu();
    }

    public string PageTitle
    {
        get { return m_PageTitle; }
        set { m_PageTitle = value; }
    }

    public string ErrorTracketTitle
    {
        get { return m_ErrorTracketTitle; }
        set { m_ErrorTracketTitle = value; }
    }

    private void LoadMenu()
    {
        if (Request.Url.AbsolutePath.ToLower().IndexOf("default.aspx") == -1)
        {
            AddBackMenuItem();
            if (AppID > 0)
            {
                liAppErrors.Visible = true;
                hlBySourceFiles.NavigateUrl = "ApplicationErrors.aspx?AppID=" + AppID;
                hlByErrorTypes.NavigateUrl = "ApplicationErrorsByTypes.aspx?AppID=" + AppID;
                hlByTime.NavigateUrl = "ShowByTime.aspx?AppID=" + AppID;
            }
            else
            {
                liSeparator2.Visible = true;
            }
        }
    }

    private void AddBackMenuItem()
    {
        liBack.Visible = true;
        liSeparator.Visible = true;
        liApplications.Visible = true;
        string navigateURL = "Default.aspx";
        if (Request.Url.ToString().IndexOf("ErrorDetailsByPath.aspx") != -1)
        {
            navigateURL = "ApplicationErrors.aspx?AppID=" + AppID;
        }
        if (Request.Url.ToString().IndexOf("ErrorDetailsByExceptions.aspx") != -1)
        {
            navigateURL = "ApplicationErrorsByTypes.aspx?AppID=" + AppID;
        }
        if (Request.Url.ToString().IndexOf("ErrorDetails.aspx") != -1)
        {
            navigateURL = "ApplicationErrors.aspx?AppID=" + AppID;
            if(Request.QueryString["rb"] != null)
            {
                switch(Request.QueryString["rb"].ToString())
                {
                    case "1"://by Source Files
                        navigateURL = "ErrorDetailsByPath.aspx?AppID=" + AppID + "&path=" + Request.QueryString["path"];
                        break;
                    case "2"://by Error Types 
                        string errorName = "";
                        if(Request.QueryString["ErrorName"] != null)
                        {
                            errorName = Request.QueryString["ErrorName"];
                        }
                        else
                        {
                            dsError.ErrorDataTable ds = new dsError.ErrorDataTable();
                            dsErrorTableAdapters.ErrorTableAdapter adapter = new dsErrorTableAdapters.ErrorTableAdapter();
                            ds = adapter.GetErrorByID(ErrorID);
                            if (ds.Rows.Count > 0)
                            {
                                DataRow ErrorDBInfo = ds.Rows[0];
                                if (!ErrorDBInfo.IsNull("Name"))
                                {
                                    errorName = ErrorDBInfo["Name"].ToString();
                                }
                            }
                
                        }
                        navigateURL = "ErrorDetailsByExceptions.aspx?AppID=" + AppID + "&ErrorName=" + errorName;
                        break;
                    case "3"://by Time
                        navigateURL = "ShowByTime.aspx?AppID=" + AppID;
                        break;
                }
            }
        }
        if (Request.Url.ToString().IndexOf("ErrorCacheDetails.aspx") != -1 ||
            Request.Url.ToString().IndexOf("ErrorVersionDetails.aspx") != -1)
        {
            if (null != Request.UrlReferrer)
                navigateURL = Request.UrlReferrer.ToString();
            else
                navigateURL = "ApplicationErrors.aspx?AppID=" + AppID;
        }
        hlBack.NavigateUrl = navigateURL;
    }

    private int AppID
    {
        get
        {
            if(Request.QueryString["AppID"] != null)
            {
                return int.Parse(Request.QueryString["AppID"]);
            }
            return 0;
        }
    }

    public string  CurrYear
    {
        get { return DateTime.Now.Year.ToString(); }
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

    protected void lbSignOut_Click(object sender, EventArgs e)
    {
        FormsAuthentication.SignOut();
        Response.Redirect("~/Login.aspx", true);
    }
}
