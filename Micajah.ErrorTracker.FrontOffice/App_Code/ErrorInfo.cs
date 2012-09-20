using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for ErrorInfo
/// </summary>
	public class ErrorInfo
	{
		public int ErrorID;
		public int ApplicationID;
		public string Browser;
		public string Method;
		public string Name;
		public string Description;
		public string URL;
		public string URLReferrer;
		public string PhysicalFileName;
		public string SourceFile;
		public string ErrorFile;
		public int ErrorLineNumber;
		public string QueryString;
		public string MachineName;
		public string UserIPAddress;
		public string UserHostName;
		public string ExceptionType;
		public string StackTrace;
		public string ExceptionsDescription;
		public string QueryStringDescription;
		public string Form;
		public string Session;
		public string ApplicationDescription;
		public string Version;
		public string RequestCookies;
		public string ResponseCookies;
		public string RequestHeader;
		public string ServerVariables;
		public decimal CacheSize;

		public ErrorInfo()
		{ }

		public ErrorInfo(int applicationID, Exception incomingException, string machineName, string userIPAddress, string version, string path)
			: this()
		{
			ApplicationID = applicationID;
			MachineName = machineName;
			UserIPAddress = userIPAddress;
			Version = version;
			PhysicalFileName = path;
			Name = incomingException.GetType().FullName;
			Description = incomingException.Message;
			ExceptionType = incomingException.GetType().FullName;
			ExceptionsDescription = incomingException.Message;
			StackTrace = incomingException.StackTrace;
		}
	}
