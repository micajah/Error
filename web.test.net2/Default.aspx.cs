using System;
using System.Data;
using System.Configuration;
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
        HttpCookie myC = new HttpCookie("MainK", "Level1");                       
        Response.Cookies.Add(myC);        
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
			try
			{
				//HttpContext.Current.ToString();
				throw new DivideByZeroException("Divide by zero exception");
			}
			catch (Exception ex)
			{
				Micajah.ErrorTrackerHelper2.Helper.ReportApplicationException(ex);
			}
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {                
        Response.Redirect("Default.aspx?recID=45");
    }
    protected void Button1_Click1(object sender, EventArgs e)
    {
        throw new VersionNotFoundException("This is TEST exception from Error Tracker test web application");                
    }
}
