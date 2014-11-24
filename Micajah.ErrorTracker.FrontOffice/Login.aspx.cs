using System;
using System.Web.Security;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblError.Visible = false;
    }
    protected void btnLogIn_Click(object sender, EventArgs e)
    {
        if (rfvUserName.IsValid && rfvPassword.IsValid
            && tbUserName.Text == System.Configuration.ConfigurationManager.AppSettings["UserName"].ToString()
            && tbPassword.Text == System.Configuration.ConfigurationManager.AppSettings["Password"].ToString())
        {
            FormsAuthentication.RedirectFromLoginPage(tbUserName.Text, true);
        }
        else
        {
            lblError.Visible = true;
        }
    }
}