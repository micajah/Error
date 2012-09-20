using System;
using System.Web;
using System.Text;
using System.Configuration;

namespace Micajah.ErrorTrackerHelper
{
    public class Helper
    {
        public static void SendError(Exception ex)
        {
            try
            {
                if (null == HttpContext.Current)
                    return;

                ErrorTracker.ErrorInfo oErrorInfo = new ErrorTracker.ErrorInfo();
                ErrorTracker.Service oService = new Micajah.ErrorTrackerHelper.ErrorTracker.Service();

                #region Collecting ErrorInfo Data

                oErrorInfo.ApplicationID = Convert.ToInt32(ConfigurationSettings.AppSettings["ApplicationID"]);

                try
                {
                    WebException.GetExceptions(ex, oErrorInfo);
                }
                catch ( ApplicationStartException )
                {
				          HttpContext.Current.Response.Clear();
				          HttpContext.Current.Response.Buffer = false;
				          HttpContext.Current.Response.Write("Error, Site is restarting. Try again later.");
				          HttpContext.Current.Response.Flush();
				          HttpContext.Current.Response.End();
                  return;
                }

                oErrorInfo.Browser = System.Web.HttpContext.Current.Request.Browser.Browser;
                oErrorInfo.PhysicalFileName = System.Web.HttpContext.Current.Request.PhysicalPath;
                oErrorInfo.UserIPAddress = System.Web.HttpContext.Current.Request.UserHostAddress.ToString();
                oErrorInfo.UserHostName = System.Web.HttpContext.Current.Request.UserHostName.ToString();
                oErrorInfo.ErrorFile = System.Web.HttpContext.Current.Request.Url.LocalPath;
                oErrorInfo.URL = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                oErrorInfo.URLReferrer = null != System.Web.HttpContext.Current.Request.UrlReferrer ? System.Web.HttpContext.Current.Request.UrlReferrer.AbsoluteUri : "None";
                oErrorInfo.MachineName = System.Web.HttpContext.Current.Server.MachineName.ToString();
                oErrorInfo.Version = WebException.GetVersionNumbers();
                oErrorInfo.StackTrace = WebException.GetTrace();
                oErrorInfo.Form = WebException.GetForm();
                oErrorInfo.Session = WebException.GetSession();
                oErrorInfo.QueryString = System.Web.HttpContext.Current.Request.QueryString.ToString();
                oErrorInfo.QueryStringDescription = WebException.GetQueryString();
                oErrorInfo.ApplicationDescription = WebException.GetApplication();
                oErrorInfo.RequestCookies = WebException.GetRequestCookies();
                oErrorInfo.ResponseCookies = WebException.GetResponseCookies();
                oErrorInfo.ServerVariables = WebException.GetServerVariables();
                oErrorInfo.Cache = WebException.GetCache();

                #endregion

                oService.AddError(oErrorInfo);
            }
            catch (Exception exc)
            { 
                throw exc;
            }
        }
    }
}
