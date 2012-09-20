using System;
using System.Data;
using System.Web;
using System.Net.Mail;
using System.Web.UI;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections;

namespace Micajah.ErrorTracker
{
    public class BWDMailClass
    {
        public static bool SendMailToBWD(int ErrorID)
        {
            try
            {   
                dsError.ErrorDataTable ds = new dsError.ErrorDataTable();
                dsErrorTableAdapters.ErrorTableAdapter adapter = new dsErrorTableAdapters.ErrorTableAdapter();
                ds = adapter.GetErrorByID(ErrorID);
                if (ds.Rows.Count == 0)
                {
                    return false;
                }
                DataRow ErrorDBInfo = ds.Rows[0];
                ErrorInfo oErrorInfo = new ErrorInfo();
                oErrorInfo.ErrorID = ErrorID;
                oErrorInfo.ApplicationID = int.Parse(ErrorDBInfo[2].ToString());
                oErrorInfo.Browser = Convert.ToString(ErrorDBInfo[3]);
                oErrorInfo.Description = Convert.ToString(ErrorDBInfo[6]);
                oErrorInfo.ErrorLineNumber = int.Parse(ErrorDBInfo[10].ToString());
                oErrorInfo.ExceptionType = Convert.ToString(ErrorDBInfo[14]);
                oErrorInfo.MachineName = Convert.ToString(ErrorDBInfo[12]);
                oErrorInfo.Method = Convert.ToString(ErrorDBInfo[4]);
                oErrorInfo.Name = Convert.ToString(ErrorDBInfo[5]);
                oErrorInfo.QueryString = Convert.ToString(ErrorDBInfo[11]);
                oErrorInfo.QueryStringDescription = Convert.ToString(ErrorDBInfo[16]);
                oErrorInfo.RequestCookies = Convert.ToString(ErrorDBInfo[18]);
                oErrorInfo.RequestHeader = Convert.ToString(ErrorDBInfo[19]);
                oErrorInfo.SourceFile = Convert.ToString(ErrorDBInfo[9]);
                oErrorInfo.StackTrace = Convert.ToString(ErrorDBInfo[15]);
                oErrorInfo.URL = Convert.ToString(ErrorDBInfo[7]);
                oErrorInfo.URLReferrer = Convert.ToString(ErrorDBInfo[8]);
                oErrorInfo.UserIPAddress = Convert.ToString(ErrorDBInfo[13]);
                oErrorInfo.Version = Convert.ToString(ErrorDBInfo[17]);
                oErrorInfo.ErrorFile = Convert.ToString(ErrorDBInfo[20]);
                oErrorInfo.Session = Convert.ToString(ErrorDBInfo[21]);
                decimal cacheSize = 0;
                decimal.TryParse(Convert.ToString(ErrorDBInfo[22]), out cacheSize);
                if (cacheSize > 0)
                {
                    oErrorInfo.CacheSize = cacheSize;
                }
                Micajah.ErrorTracker.EmailingHelper oHelper = new Micajah.ErrorTracker.EmailingHelper(oErrorInfo);

                dsApplication dsApp = new dsApplication();
                dsApplicationTableAdapters.ApplicationTableAdapter adapterApp = new dsApplicationTableAdapters.ApplicationTableAdapter();
                adapterApp.Fill(dsApp.Application);
                string WebConfigMail = System.Configuration.ConfigurationSettings.AppSettings["BWDFromMail"];
                string RealWebConfigMail = WebConfigMail;
                if (WebConfigMail == null || WebConfigMail == "")
                {
                    RealWebConfigMail = "error.tracker@micajah.com";
                }

                DataRow[] drs = dsApp.Application.Select("ApplicationID = " + oErrorInfo.ApplicationID);
                if (drs.Length > 0)
                {
                    DataRow drApplication = drs[0];
                    oHelper.MailFrom = RealWebConfigMail;
                    oHelper.MailTo = Convert.ToString(drApplication["MailTo"]);
                    oHelper.MailBWD = Convert.ToString(drApplication["MailBWD"]);
                    oHelper.ApplicationName = Convert.ToString(drApplication["Name"]);
                    oHelper.SmtpServer = Convert.ToString(drApplication["SMTPServer"]);
                    oHelper.MailAdmin = Convert.ToString(drApplication["MailAdmin"]);
                    oHelper.FloodCount = 10;
                    oHelper.FloodMins = 5;
                    oHelper.SendEmail(oErrorInfo, (oHelper.MailBWD == "" ? EmailingHelper.MailRecepient.Users: EmailingHelper.MailRecepient.bigWebDesk));
                    //oHelper.SendEmail(oErrorInfo);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
    }
	internal class EmailingHelper
    {
        #region Constructor

        public EmailingHelper(ErrorInfo oInfo)
        {
            ErrorInfo = oInfo;
        }

        #endregion

        #region Private Members

        private bool _SendMail = true;
		private string m_ApplicationName = ("localhost");
		private string _MailFrom = string.Empty;
		private string _MailTo = string.Empty;
		private string _MailAdmin = string.Empty;
		private string strErrorMessage = String.Empty;
		private Exception _CurrentException;
		private bool _DrillDownInCache = false;
		private string FullTrace = String.Empty;
		private int _TheFloodCount = 10;
		private int _FloodMins = 30;
		private string _ContentAfterException = String.Empty;
		private string _SmtpServer = "localhost";
        private int _ApplicationID = 0;
        private ErrorInfo ErrorInfo = null;

		#endregion

		#region Public Members

		public string ApplicationName
		{
			set
			{
				m_ApplicationName = value;
			}
		}

		public int FloodCount
		{
			set
			{
				_TheFloodCount = value;
			}
		}
		
		public int FloodMins
		{
			set
			{
				_FloodMins = value;
			}
		}

		public string MailFrom
		{
			set
			{
				_MailFrom = value;
			}
		}

		public string MailTo
		{
			set
			{
				_MailTo = value;
			}
		}

        private string _MailBWD;

        public string MailBWD
        {
            set { _MailBWD = value; }
            get { return _MailBWD; }
        }
	

		public Exception CurrentException
		{
			set
			{
				_CurrentException = value;
			}
		}

		public bool DrillDownInCache
		{
			set
			{
				_DrillDownInCache = value;
			}
		}

		public string ContentAfterException
		{
			set
			{
				_ContentAfterException = value;
			}
		}

		public string SmtpServer
		{
			set
			{
				_SmtpServer = value;
			}
		}

		public string MailAdmin
		{
			set
			{
				_MailAdmin = value;
			}
		}

        protected StringBuilder _BWDMessagebody = null;

        protected StringBuilder BWDMessagebody
        {
            get
            {
                if (null == _BWDMessagebody)
                {
                    _BWDMessagebody = new StringBuilder();
                    CreateBWDTicketBody(_BWDMessagebody);
                }

                return _BWDMessagebody;
            }
        }

        protected StringBuilder MessageBody
        {
            get 
            {
                StringBuilder _MessageBody = new StringBuilder();
                CreateMessageBody(_MessageBody);

                return _MessageBody;
            }
        }

		#endregion

        #region CreateBWDTicketBody

        protected void CreateBWDTicketBody(StringBuilder oSB)
        {
            try
            {
                GetMailHeader(oSB, false);

                oSB.AppendLine("Exception Type: " + ErrorInfo.ExceptionType);
                oSB.Append(Environment.NewLine);
                oSB.AppendLine("Message: " + ErrorInfo.Description);
                oSB.Append(Environment.NewLine);
                oSB.Append(Environment.NewLine);
                oSB.Append(ErrorInfo.RequestCookies);
                oSB.Append(Environment.NewLine);
                oSB.Append(ErrorInfo.ResponseCookies);
                oSB.Append(Environment.NewLine);
                oSB.AppendLine("QueryString: " + ErrorInfo.QueryString);
                oSB.Append(Environment.NewLine);
                oSB.AppendLine("Server Variables: " + ErrorInfo.ServerVariables);
                oSB.Append(Environment.NewLine);
                oSB.AppendLine("Version: " + ErrorInfo.Version);
            }
            catch (Exception ex)
            {
                //Handle any exceptions by sending them in the e-mail.
                oSB.Append("<h1 style='color:red;'>There was a problem building this message</h1>");
                oSB.Append("<p>" + ex.Message.ToString() + "</p>");
                oSB.Append("<p>" + ex.StackTrace.ToString() + "</p>");
            }
        }

        #endregion

        #region CreateMessageBody

        protected void CreateMessageBody(StringBuilder oSB)
        {
            try
            {
                //Set up the Mail Header
                GetMailHeader(oSB, true);

                //Set up the Exceptions
                oSB.Append(ErrorInfo.ExceptionsDescription);
                
                //Get the Form
                oSB.Append(ErrorInfo.Form);
                //Get the Request Cookies
                oSB.Append(ErrorInfo.RequestCookies);

                //Get the Response Cookies
                oSB.Append(ErrorInfo.ResponseCookies);

                //Get the Content Passed in After the Exceptions
                if (_ContentAfterException != String.Empty)
                {
                    oSB.Append(_ContentAfterException);
                    oSB.Append("<hr />");
                }               

                //Get the QueryString
                oSB.Append(ErrorInfo.QueryString);
                 
                oSB.Append(ErrorInfo.QueryStringDescription);

                if (ErrorInfo.QueryStringDescription != String.Empty)
                {
                    oSB.Append("<hr />");
                }

                //Get the Session
                oSB.Append(ErrorInfo.Session);

                //Get the Application
                oSB.Append(ErrorInfo.ApplicationDescription);

                

                //Get the Request Headers
                oSB.Append(ErrorInfo.RequestHeader);

                //Get the Response Headers
                //oSB.Append(ErrorInfo.ResponeCookies);

                //Get the Cache
                //oSB.Append(ErrorInfo.Cache);

                //Get the ServerVariables
                oSB.Append(ErrorInfo.ServerVariables);

                //Get the Trace
                //oSB.Append(ErrorInfo.StackTrace);

                //Set up the Version Numbers
                //oSB.Append(ErrorInfo.Version);
                if (ErrorInfo.CacheSize > 0)
                {
                    oSB.Append("<b>Cache Size:</b> " + ErrorInfo.CacheSize.ToString("N") + "KB<br><hr>");
                }

                //Get the Mail Footer
                oSB.Append(GetMailFooter());

            }
            catch (Exception ex)
            {
                //Handle any exceptions by sending them in the e-mail.
                oSB.Append("<h1 style='color:red;'>There was a problem building this message</h1>");
                oSB.Append("<p>" + ex.Message.ToString() + "</p>");
                oSB.Append("<p>" + ex.StackTrace.ToString() + "</p>");
            }
        }

        #endregion
		
		public enum MailRecepient
		{
			Users = 1,
			bigWebDesk = 2
		}
        /// <summary>
		/// This method constructs the e-mail and then sends the message.
		/// </summary>
		public void SendEmail(ErrorInfo oErrorInfo, MailRecepient recepient)
		{
			_ApplicationID = oErrorInfo.ApplicationID;
			System.Web.Mail.MailMessage myMail = new System.Web.Mail.MailMessage();
			myMail.BodyFormat = System.Web.Mail.MailFormat.Html;
			myMail.From = this._MailFrom;
			if(recepient == MailRecepient.Users)
			{
				myMail.To = this._MailTo;
				myMail.Subject = this.m_ApplicationName + " - " + oErrorInfo.ExceptionType;
			}
			else
			{
        myMail.To = this._MailBWD;
        myMail.Subject = string.Format("Error Tracker: {0} at {1} : {2}", oErrorInfo.ExceptionType, oErrorInfo.ErrorFile, oErrorInfo.ErrorLineNumber);
			}
			myMail.Body = MessageBody.ToString();
			if  (_SendMail == true)
			{
				//Set the Mail Server and Send the e-mail
				System.Web.Mail.SmtpMail.SmtpServer = _SmtpServer;
				System.Web.Mail.SmtpMail.Send(myMail);
			}
    }

        #region Private Helper Methods

        #region CreateAnchor

        private string CreateAnchor(string Text)
        {
            return ("<a href=\"" + Text + "\">" + Text + "</a>");
        }

        #endregion

        #region AppendTableHeader

        private void AppendTableHeader(StringBuilder oSB)
        {
            oSB.Append("<table cellpadding=2 cellspacing=1>");
        }

        #endregion

        #region AppendTableFooter

        private void AppendTableFooter(StringBuilder oSB)
        {
            oSB.Append("</table>");
        }

        #endregion

        #region AppendHr

        private void AppendHr(StringBuilder oSB)
        {
            oSB.Append("<hr />");
        }

        #endregion

        #region AppendTableRow

        private void AppendTableRow(StringBuilder oSB, string CellName, string CellValue, bool Header)
        {
            #region Add the Table Row To the String Builder
            oSB.Append("<tr>");
            if (Header)
            {
                oSB.Append("<td valign='top'><b>" + CellName + "</b></td>");
            }
            else
            {
                oSB.Append("<td valign='top'>" + CellName + "</td>");
            }
            oSB.Append("<td>" + CellValue + "</td>");
            oSB.Append("</tr>");
            #endregion
        }

        private void AppendTableRow(StringBuilder oSB, string CellName, string CellValue, bool Header, string ValueStyle)
        {
            #region Add the Table Row To the String Builder
            oSB.Append("<tr>");
            if (Header)
            {
                oSB.Append("<td valign='top'><b>" + CellName + "</b></td>");
            }
            else
            {
                oSB.Append("<td valign='top'>" + CellName + "</td>");
            }
            oSB.Append("<td><span style='" + ValueStyle + "'>" + CellValue + "</span></td>");
            oSB.Append("</tr>");
            #endregion
        }

        #endregion

        #endregion

        /// <summary>
		/// This method determines wether the e-mail should be sent.
		/// </summary>
		/// <returns>Wether or not the e-mail should be sent</returns>
		private bool EmailFlooding()
		{
			#region Private Members
			int TheCount = 1;
			bool flag = false;
			#endregion

			//Check to see if the e-mail can be sent, if there is no Cache item, then the e-mail can be sent.

			#region Flood Checking

			//This is acheived by Using the Full Trace of the Exception tree as the Cache Key, enabling the 
			//Exception tree to be used as a Unique entry for the specific exception that is being thrown.
			if (System.Web.HttpContext.Current.Cache[FullTrace + "_Count"] == null)
			{
				System.Web.HttpContext.Current.Cache.Add(FullTrace + "_Count", //The Cache Key
						TheCount, //The Value (This is used to determine when to stop sending a specific Error). 
						null, //There are no dependancies.
						DateTime.Now.AddMinutes(_FloodMins), //The Time for which the Cache Entry is Valid.
						System.TimeSpan.Zero, //Set the Sliding Expiration to 0.
						System.Web.Caching.CacheItemPriority.Normal, //The Level at which the Cache item is removed from the Server.
						null); //Callback (There isn't one)
			}
			else
			{
				//If the Cache item already exists, the server needs to determine that the Error can be sent or not
				//by counting the number of times the error has been encountered and setting the Cache Entry's value
				//to the number of times the Cache item has happened.
				TheCount = Convert.ToInt32(System.Web.HttpContext.Current.Cache[FullTrace + "_Count"]); 
				TheCount++;
				System.Web.HttpContext.Current.Cache[FullTrace + "_Count"] = TheCount;

				//If the cache Count is more than or equal to the Allowed count return True to state that we've encountered a Flood.
				if (TheCount >= _TheFloodCount)
				{
					flag = true;
				}
			}

			#endregion

			//Return the Value.
			return flag;
		}

		private void GetMailHeader(StringBuilder Sb, bool GenerateHTML)
		{
            if (GenerateHTML)
            {
                Sb.Append("<html>");
                Sb.Append("<head>");
                Sb.Append("<title>");
            }
            Sb.AppendLine(this.m_ApplicationName + " - Application Issue");
            int endIndex = HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("Service.asmx");
            if (endIndex < 0)
            {
                endIndex = HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("ErrorDetailsByPath.aspx");
            }
            if (endIndex < 0)
            {
                endIndex = HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("ShowByTime.aspx");
            }
            if (endIndex < 0)
            {
                endIndex = HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("ErrorDetailsByExceptions.aspx");
            }
			if (GenerateHTML)
            {
                Sb.Append("</title>");
                Sb.Append("</head>");

                Sb.Append("<style>");
                Sb.Append("body{	font-family:Verdana;font-size:8pt;background:#FFFF99;}p{font-family:Verdana;font-size:8pt;}td{	font-family:Verdana;font-size:8pt;}span{	font-family:Verdana;font-size:8pt;}");
                Sb.Append("</style>");

                Sb.Append("<body>");
                Sb.Append("<h1>" + this.m_ApplicationName + " - Application Issue</h1>");
                Sb.Append("<h2>" + DateTime.Now.ToString("r") + "</h2>");
				Sb.AppendFormat("<table><tr style=\"background-color:Yellow\"><td><a href=\"{1}ErrorDetails.aspx?ErrorID={0}\">Click here for summary and detailed information</a></td></tr></table>", ErrorInfo.ErrorID, HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, endIndex));

                AppendHr(Sb);

                AppendTableHeader(Sb);
            }
            else
            {
                Sb.Append(Environment.NewLine);
                Sb.AppendFormat("For more details follow the link: {1}ErrorDetails.aspx?ErrorID={0}\n", ErrorInfo.ErrorID, HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, endIndex));
                Sb.Append(Environment.NewLine);
                Sb.AppendFormat("URL: {0}\n", ErrorInfo.URL);
                Sb.Append(Environment.NewLine);
            }
            if (GenerateHTML)
            {
                AppendTableRow(Sb, "Url", CreateAnchor(ErrorInfo.URL), true);
                AppendTableFooter(Sb);

                AppendHr(Sb);

                AppendTableHeader(Sb);
                AppendTableRow(Sb, "User IP Address", ErrorInfo.UserIPAddress, true);

                AppendTableRow(Sb, "User Host", ErrorInfo.UserHostName, true);
                AppendTableFooter(Sb);

                AppendHr(Sb);

                if (ErrorInfo.URLReferrer != null)
                {
                    AppendTableHeader(Sb);
                    AppendTableRow(Sb, "Url Referrer", CreateAnchor(ErrorInfo.URLReferrer), true);
                    AppendTableFooter(Sb);

                    AppendHr(Sb);
                }

                AppendTableHeader(Sb);
                AppendTableRow(Sb, "Machine Name", ErrorInfo.MachineName, true);
                AppendTableFooter(Sb);

                AppendHr(Sb);
            }
		}

		private string GetMailFooter()
		{
			StringBuilder Sb = new StringBuilder();

			Sb.Append("</table>");
			//Sb.Append("<hr />");
			Sb.Append("</body>");
			Sb.Append("</html>");
			
			return Sb.ToString();

		}
	}
}
