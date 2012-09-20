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
			try
			{
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
				//oErrorInfo.StackTrace = WebException.GetTrace();
				oErrorInfo.Form = WebException.GetForm();
				oErrorInfo.Session = WebException.GetSession();
				oErrorInfo.QueryString = System.Web.HttpContext.Current.Request.QueryString.ToString();
				oErrorInfo.QueryStringDescription = WebException.GetQueryString();
				oErrorInfo.ApplicationDescription = WebException.GetApplication();
				oErrorInfo.RequestCookies = WebException.GetRequestCookies().ToString();
				oErrorInfo.ResponseCookies = WebException.GetResponseCookies().ToString();
				oErrorInfo.ServerVariables = WebException.GetServerVariables().ToString();
				oErrorInfo.CacheSize = WebException.GetCacheSize();

				#endregion

				string errorInfoText = SerializeErrorInfo(oErrorInfo);
				oErrorInfo.QueryStringDescription += string.Format("<br/><hr/>Error Info length: {0}<hr/>", errorInfoText.Length);
				//File.WriteAllText(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "Temp\\Errors\\" + Guid.NewGuid().ToString(), errorInfoText);

				ErrorTracker.Service service = new Micajah.ErrorTrackerHelper2.ErrorTracker.Service();
				//service.Url = "http://error.micajah.com/Service.asmx";
				service.AddError(oErrorInfo);
			}
			catch (Exception inner_ex)
			{
				SendEmail(string.Format("Initial exception of type {8} in {0}:{3} {1} ({2}). {3}{3}Error Tracker internal exception in {4} of type {7}: {5} ({6}).",
						System.Web.HttpContext.Current.Request.Url.LocalPath,
						ex.Message,
						ex.StackTrace,
						"\n\n\n",
						inner_ex.TargetSite,
						inner_ex.Message,
						inner_ex.StackTrace,
						inner_ex.GetType().FullName,
						ex.GetType().FullName
						),
					"Error Tracker exception",
					"igor.vladyka@micajah.com"
					);
			}
			return oErrorInfo;
		}

		private static void SendEmail(string body, string subject, string to)
		{
			MailMessage message = new MailMessage();
			message.Body = body;
			message.IsBodyHtml = false;
			message.Subject = subject;
			message.To.Add(to);
			message.From = new MailAddress("igor.vladyka@micajah.com");
			message.IsBodyHtml = true;

			SmtpClient smtp = new SmtpClient("127.0.0.1");
			smtp.Send(message);
		}

		private static string SerializeErrorInfo(ErrorInfo value)
		{
			string result = string.Empty;
			try
			{
				XmlSerializer serealizer = new XmlSerializer(typeof(ErrorInfo));
				MemoryStream stream = new MemoryStream();
				serealizer.Serialize(stream, value);
				byte[] buffer = stream.GetBuffer();
				result = Encoding.Default.GetString(buffer, 0, (int)stream.Length);
				stream.Close();
				stream.Dispose();
			}
			catch (Exception)
			{
				return string.Empty;
			}
			return result;
		}

		public static void ReportApplicationException(Exception exception)
		{
			try
			{
				ErrorTracker.Service oService = new Micajah.ErrorTrackerHelper2.ErrorTracker.Service();
				int applicationID = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationID"]);
				ErrorInfo errorInfo = ConvertToErrorInfo(exception, applicationID);

				oService.AddError(errorInfo);
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
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
