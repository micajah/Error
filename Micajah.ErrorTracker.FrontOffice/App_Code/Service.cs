using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Data;

[WebService(Namespace = "http://www.micajah.com/ErrorTracker/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Service : System.Web.Services.WebService
{
    public Service () 
		{
    }

    [WebMethod(MessageName="AddErrorInfoError")]
    public int AddError(ErrorInfo oErrorInfo) 
    {
			return ProcessErrorInfo(oErrorInfo);
    }

		private static int ProcessErrorInfo(ErrorInfo oErrorInfo)
		{
			try
			{
				try
				{
					SaveException(oErrorInfo);
				}
				catch (Exception)
				{ }
				Micajah.ErrorTracker.EmailingHelper oHelper = new Micajah.ErrorTracker.EmailingHelper(oErrorInfo);

				#region Email exception

				dsApplication dsApplications = GetApplications();

				DataRow[] drs = dsApplications.Application.Select("ApplicationID = " + oErrorInfo.ApplicationID.ToString());
				if (drs.Length > 0 && Convert.ToBoolean(drs[0]["SendEmail"]))
				{
					DataRow drApplication = drs[0];
					oHelper.MailFrom = Convert.ToString(drApplication["MailFrom"]);
					oHelper.MailTo = Convert.ToString(drApplication["MailTo"]);
					oHelper.MailBWD = Convert.ToString(drApplication["MailBWD"]);
					oHelper.ApplicationName = Convert.ToString(drApplication["Name"]);
					oHelper.SmtpServer = Convert.ToString(drApplication["SMTPServer"]);
					oHelper.MailAdmin = Convert.ToString(drApplication["MailAdmin"]);
					oHelper.FloodCount = 10;
					oHelper.FloodMins = 5;
                    if (!drApplication.IsNull("CacheItemsSize"))
                    {
                        oHelper.IncludeCacheItemsSizeInEmail = Convert.ToBoolean(drApplication["CacheItemsSize"]);
                    }
					oHelper.SendEmail(oErrorInfo, Micajah.ErrorTracker.EmailingHelper.MailRecepient.Users);
				}

				#endregion

				#region BWD ticket automatic email creating

				dsErrorTableAdapters.QueriesTableAdapter oErrorsAdapter = new dsErrorTableAdapters.QueriesTableAdapter();
				object oCount = oErrorsAdapter.SelectSimilarExceptionCount(oErrorInfo.ApplicationID, oErrorInfo.ErrorFile, oErrorInfo.ErrorLineNumber);

				if (1 == (int)oCount &&
						null != oHelper.MailBWD &&
						oHelper.MailBWD.Trim().Length > 0)
				{
					oHelper.SendEmail(oErrorInfo, Micajah.ErrorTracker.EmailingHelper.MailRecepient.bigWebDesk);
				}

				#endregion

				return 0;
			}
			catch (Exception ex)
			{
				return -1;
			}
		}

		private static dsApplication GetApplications()
		{
			dsApplication dsApp = new dsApplication();
			dsApplicationTableAdapters.ApplicationTableAdapter adapterApp = new dsApplicationTableAdapters.ApplicationTableAdapter();
			adapterApp.Fill(dsApp.Application);
			return dsApp;
		}

		private static void SaveException(ErrorInfo oErrorInfo)
		{
			dsError ds = new dsError();
			dsError.ErrorRow row = ds.Error.NewErrorRow();
			row.ApplicationID = oErrorInfo.ApplicationID;
			row.Date = DateTime.Now;
			row.Browser = oErrorInfo.Browser;
			row.Description = oErrorInfo.Description;
			row.ErrorLineNumber = oErrorInfo.ErrorLineNumber;
			row.ExceptionType = oErrorInfo.ExceptionType;
			row.MachineName = oErrorInfo.MachineName;
			row.Method = oErrorInfo.Method;
			row.Name = oErrorInfo.Name;
			row.QueryString = oErrorInfo.QueryString;
			row.QueryStringDescription = oErrorInfo.QueryStringDescription;
			row.RequestCookies = oErrorInfo.RequestCookies;
			row.RequestHeader = oErrorInfo.RequestHeader;
			row.SourceFile = oErrorInfo.SourceFile;
			row.StackTrace = oErrorInfo.StackTrace;
			row.URL = oErrorInfo.URL;
			row.URLReferrer = oErrorInfo.URLReferrer;
			row.UserIPAddress = oErrorInfo.UserIPAddress;
			row.Version = oErrorInfo.Version;
			row.Path = oErrorInfo.ErrorFile;
		    row.Session = oErrorInfo.Session;
            row.CacheSize = oErrorInfo.CacheSize;
			ds.Error.Rows.Add(row);

			dsErrorTableAdapters.ErrorTableAdapter adapter = new dsErrorTableAdapters.ErrorTableAdapter();
			adapter.Update(ds.Error);
			oErrorInfo.ErrorID = ConvertHelper.GetIntValue(row["ErrorID"].ToString());
		}  
}
