using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Micajah.ErrorTrackerHelper2.ErrorTracker;
using System.Net.Mail;
using System.Xml.Serialization;
using System.IO;

namespace Micajah.ErrorTrackerHelper2
{
	public class Helper
	{
		private static object x = 1;

		public static ErrorTracker.ErrorInfo SendError(Exception ex)
		{
			if (ex == null)
				return null;

			ErrorTracker.ErrorInfo oErrorInfo = null;

            if (null == System.Web.HttpContext.Current)
					return null;
				oErrorInfo = new ErrorTracker.ErrorInfo();


				#region Collecting ErrorInfo Data

				oErrorInfo.ApplicationID = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationID"]);

				try
				{
					WebException.GetExceptions(ex, oErrorInfo);
				}
				catch (TelerikWebResourceException)
				{
					return null;
				}
				catch (ApplicationStartException)
				{
					System.Web.HttpContext.Current.Response.Clear();
					System.Web.HttpContext.Current.Response.Buffer = false;
					System.Web.HttpContext.Current.Response.Write("Error, Site is restarting. Try again later.");
					System.Web.HttpContext.Current.Response.Flush();
					System.Web.HttpContext.Current.Response.End();
					return null;
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
				oErrorInfo.Form = WebException.GetForm();
				oErrorInfo.Session = WebException.GetSession();
				oErrorInfo.QueryString = System.Web.HttpContext.Current.Request.QueryString.ToString();
				oErrorInfo.QueryStringDescription = WebException.GetQueryString();
				oErrorInfo.ApplicationDescription = WebException.GetApplication();
				oErrorInfo.RequestCookies = WebException.GetRequestCookies().ToString();
				oErrorInfo.ResponseCookies = WebException.GetResponseCookies().ToString();
				oErrorInfo.ServerVariables = WebException.GetServerVariables().ToString();
                string cacheItemsInfo = "";
				oErrorInfo.CacheSize = WebException.GetCacheSize(ref cacheItemsInfo);
                oErrorInfo.CacheItemsInfo = cacheItemsInfo;

				#endregion

				string errorInfoText = SerializeErrorInfo(oErrorInfo);
				oErrorInfo.QueryStringDescription += string.Format("<br/><hr/>Error Info length: {0}<hr/>", errorInfoText.Length);

				ErrorTracker.Service service = new Micajah.ErrorTrackerHelper2.ErrorTracker.Service();
				service.AddError(oErrorInfo);

            return oErrorInfo;
		}


		private static string SerializeErrorInfo(ErrorInfo value)
		{
			string result = string.Empty;

            XmlSerializer serealizer = new XmlSerializer(typeof(ErrorInfo));
            MemoryStream stream = new MemoryStream();
            serealizer.Serialize(stream, value);
            byte[] buffer = stream.GetBuffer();
            result = Encoding.Default.GetString(buffer, 0, (int)stream.Length);
            stream.Close();
            stream.Dispose();

            return result;
		}

		public static void ReportApplicationException(Exception exception)
		{
            ErrorTracker.Service oService = new Micajah.ErrorTrackerHelper2.ErrorTracker.Service();
            int applicationID = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationID"]);
            ErrorInfo errorInfo = ConvertToErrorInfo(exception, applicationID);

            oService.AddError(errorInfo);
		}

		private static ErrorInfo ConvertToErrorInfo(Exception exception, int applicationID)
		{
			ErrorInfo errorInfo = new ErrorInfo();
			errorInfo.ApplicationID = applicationID;
			errorInfo.ExceptionType = exception.GetType().FullName;
			errorInfo.MachineName = Environment.MachineName;
			errorInfo.UserIPAddress = "localhost";
			errorInfo.Version = Environment.Version.ToString();
			errorInfo.PhysicalFileName = Environment.CurrentDirectory;
			errorInfo.StackTrace = exception.StackTrace;
			errorInfo.Method = exception.TargetSite.Name;
			errorInfo.Name = exception.Message;
			errorInfo.SourceFile = exception.Source;

			errorInfo.URL = string.Format("{0} - {1}", Environment.MachineName, GetSourceFileFromStackTrace(exception.StackTrace));
			return errorInfo;
		}

		private static string GetSourceFileFromStackTrace(string trace)
		{
			int start = trace.IndexOf(" in ");
			int end = trace.IndexOf(":line ");
			if (start > 0 && end > 0 && end > start)
			{
				return trace.Substring(start + 4, end - start - 4);
			}
			return string.Empty;
		}
	}
}
