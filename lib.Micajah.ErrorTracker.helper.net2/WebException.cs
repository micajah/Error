using System;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Net.Mail;
using System.Web.UI;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Configuration;

namespace Micajah.ErrorTrackerHelper2
{
	internal class ApplicationStartException : Exception
	{
		public ApplicationStartException()//vbc
			: base()
		{
		}
	}

	internal class TelerikWebResourceException : Exception
	{
		public TelerikWebResourceException()
			: base()
		{
		}
	}

	internal class WebException
	{
		#region Constructor

		public WebException()
		{

		}

		#endregion

		#region Private Members

		private static bool DisableTelerikWebResourceException = Convert.ToInt32(ConfigurationSettings.AppSettings["DisableTelerikWebResourceException"]) > 0;
		private string strErrorMessage = String.Empty;
		private Exception oCurrentException;
		private bool _DrillDownInCache = false;
		private bool _ReturnCache = false;
		private string FullTrace = String.Empty;
		private int _TheFloodCount = 10;
		private int _FloodMins = 30;
		private string _ContentAfterException = String.Empty;
		private string _SmtpServer = "localhost";
		StringBuilder Sb = new StringBuilder();

		#endregion

		#region Public Members

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

		public Exception CurrentException
		{
			set
			{
				oCurrentException = value;
			}
		}

		public bool DrillDownInCache
		{
			set
			{
				_DrillDownInCache = value;
			}
		}

		public bool ReturnCache
		{
			set
			{
				_ReturnCache = value;
			}
		}

		public string ContentAfterException
		{
			set
			{
				_ContentAfterException = value;
			}
		}

		#endregion

		#region AppendTableRow

		private static void AppendTableRow(StringBuilder oSB, string CellName, string CellValue, bool Header)
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

		private static void AppendTableRow(StringBuilder oSB, string CellName, string CellValue, bool Header, string ValueStyle)
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

        private static void AppendTableRowRC(StringBuilder oSB, string CellName, string CellValue, bool Header)
        {
            #region Add the Table Row To the String Builder for Request Cookies
            oSB.Append("<tr>");
            if (Header)
            {
                oSB.Append(GetStartTDRC(CellName, 20, 200, " valign='top'") + "<b>" + CellName + "</b>"
                    + GetEndTDRC(CellName, 20));
            }
            else
            {
                oSB.Append(GetStartTDRC(CellName, 20, 200, " valign='top'") + CellName + GetEndTDRC(CellName, 20));
            }
            oSB.Append(GetStartTDRC(CellValue, 60, 500, "") + CellValue + GetEndTDRC(CellValue, 60));
            oSB.Append(GetStartTDRC(Microsoft.JScript.GlobalObject.unescape(CellValue), 60, 500, "") 
                + Microsoft.JScript.GlobalObject.unescape(CellValue)
                + GetEndTDRC(Microsoft.JScript.GlobalObject.unescape(CellValue), 60));
            oSB.Append("</tr>");
            #endregion
        }

        private static string GetStartTDRC(string innerHTML, int maxLength, int tdWidth, string additionaltags)
        {
            string td = "<td" + (additionaltags.Length > 0 ? additionaltags : "");
            if (innerHTML.Length > maxLength)
            {
                td += " style='width:" + tdWidth.ToString() + "px; word-wrap: break-word;'><div style='width:" 
                    + tdWidth.ToString() + "px;overflow:auto'>";
            }
            else{
                td += ">";
            }
            return td;
        }

        private static string GetEndTDRC(string innerHTML, int maxLength)
        {
            string td = "";
            if (innerHTML.Length > maxLength)
            {
                td = "</div>";
            }
            td += "</td>";
            return td;
        }

		#endregion

		#region CreateAnchor

		private static string CreateAnchor(string Text)
		{
			return ("<a href=\"" + Text + "\">" + Text + "</a>");
		}

		#endregion

		#region AppendTableHeader

		private static void AppendTableHeader(StringBuilder oSB)
		{
			oSB.Append("<table cellpadding=2 cellspacing=1>");
		}

		#endregion

		#region AppendTableFooter

		private static void AppendTableFooter(StringBuilder oSB)
		{
			oSB.Append("</table>");
		}

		#endregion

		#region AppendHr

		private static void AppendHr(StringBuilder oSB)
		{
			oSB.Append("<hr />");
		}

		#endregion

		#region AppendHashTable

		private static void AppendHashTable(StringBuilder oSB, System.Collections.Hashtable ht)
		{
			#region Add the Table Rows To the String Builder
			oSB.Append("<tr>");
			oSB.Append("<td valign='top'></td>");
			oSB.Append("<td valign='top'>");
			oSB.Append("<table>");

			foreach (object hkey in ht.Keys)
			{
				AppendTableRow(oSB, ToStringOrEmpty(hkey), ToStringOrEmpty(ht[hkey]), false);
			}

			oSB.Append("</table>");
			oSB.Append("</td>");
			oSB.Append("</tr>");
			#endregion
		}

		#endregion

		#region AppendArrayList

		private static void AppendArrayList(StringBuilder oSB, string name, System.Collections.ArrayList al)
		{
			#region Add the Table Rows To the String Builder

		    int curItem = 0;
			foreach (object item in al)
			{
				oSB.Append("<tr>");
                if (curItem == 0)
                {
                    oSB.Append("<td valign='top'><b>" + name + "</b></td>");
                }
				else
                {
                    oSB.Append("<td valign='top'></td>");
                }
				oSB.Append("<td>" + ToStringOrEmpty(item) + "</i></td>");
				oSB.Append("</tr>");
			    curItem++;
			}
			#endregion
		}

		#endregion

		#region AppendIErrorInterface

		private static void AppendIErrorInterface(StringBuilder oSB, IError e)
		{
			#region Add the IError Information to the String Builder
			string thedebuginformation = e.GetDebugInformation();
			oSB.Append("<tr>");
			oSB.Append("<td valign='top'>&nbsp;</td>");
			oSB.Append("<td>" + thedebuginformation + "</td>");
			oSB.Append("</tr>");
			#endregion
		}

		#endregion

		#region AppendStateBagEntry

		private static void AppendStateBagEntry(StringBuilder oSB, string Name, object Value, Type ObjectType)
		{
			#region Render Specific Types as HTML
			if (ObjectType == typeof(System.String))
			{
				AppendTableRow(oSB, Name, (string)Value, false);
			}
			else if (ObjectType == typeof(System.Collections.Hashtable))
			{
				AppendHashTable(oSB, (System.Collections.Hashtable)Value);
			}
			else if (ObjectType == typeof(System.Collections.ArrayList))
			{
				AppendArrayList(oSB, Name, (System.Collections.ArrayList)Value);
			}
			else if (Value is IError)
			{
				AppendIErrorInterface(oSB, (IError)Value);
			}
			else if (ObjectType == typeof(System.Xml.XmlDocument))
			{
				AppendTableRow(oSB, String.Empty, System.Web.HttpContext.Current.Server.HtmlEncode(((System.Xml.XmlDocument)Value).OuterXml), false);
			}
			else if (ObjectType == typeof(System.Data.DataTable))
			{
				AppendDataTable(oSB, (System.Data.DataTable)Value);
			}
			else
			{
				AppendTableRow(oSB, "'" + Name + "'", Convert.ToString(Value), false);
			}
			#endregion
		}

		#endregion

		#region AppendDataTable

		private static void AppendDataTable(StringBuilder oSB, System.Data.DataTable Value)
		{
			System.Data.DataTable Dt = (System.Data.DataTable)Value;

			AppendTableRow(oSB, "Row Count", "RowCount : " + Dt.Rows.Count.ToString(), true);

			oSB.Append("<tr>");
			oSB.Append("<td valign='top'></td>");
			oSB.Append("<td>");

			if (Dt.Rows.Count > 0)
			{
				oSB.Append("<table cellspacing=2 cellpadding=2 border=1>");
				oSB.Append("<tr>");
				foreach (System.Data.DataColumn Dc in Dt.Columns)
				{
					oSB.Append("<th>" + Dc.ColumnName + "</th>");
				}
				oSB.Append("</tr>");

				foreach (DataRow Dr in Dt.Rows)
				{
					oSB.Append("<tr>");
					foreach (System.Data.DataColumn Dc in Dt.Columns)
					{
						oSB.Append("<td>" + Dr[Dc.ColumnName].ToString() + "</td>");
					}
					oSB.Append("</tr>");
				}
				oSB.Append("</table>");
			}
		}

		#endregion

		#region GetVersionNumbers

		public static string GetVersionNumbers()
		{
			StringBuilder oSB = new StringBuilder();

			AppendTableHeader(oSB);
			AppendTableRow(oSB, ".NET Framework Version", System.Environment.Version.ToString(), true);
			AppendTableRow(oSB, "&#160;", "&#160;", false);

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (Assembly a in assemblies)
			{
				if ((a.GetName().Name.IndexOf("System") < 0) &&
										(a.GetName().Version.ToString() != "0.0.0.0") &&
										(a.GetName().Name.IndexOf("mscorlib") < 0))
				{
					string Version = a.GetName().Version.ToString();
					AssemblyInformationalVersionAttribute[] infoversion = (AssemblyInformationalVersionAttribute[])a.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
					if (infoversion.Length == 1)
					{
						Version += (" (" + infoversion[0].InformationalVersion.ToString() + ")");
					}

					AppendTableRow(oSB, "<font size=-1>" + a.GetName().Name.ToString() + "</font>", "<font size=-1>" + Version + "</font>", false);
					AppendTableRow(oSB, "<font size=-1>" + "Codebase" + "</font>", "<font size=-1>" + CreateAnchor(a.CodeBase.ToString()) + "</font>", false);
					try
					{
						FileInfo F = new FileInfo(a.Location);
						AppendTableRow(oSB, "<font size=-1>" + "Last Write Time" + "</font>", "<font size=-1>" + File.GetLastWriteTime(F.FullName).ToLongDateString() + " " + File.GetLastWriteTime(F.FullName).ToLongTimeString() + "</font>", false);
					}
					catch { int i = 0; }
					finally
					{
						AppendTableRow(oSB, "&#160;", "&#160;", false);
					}
				}
			}

			AppendTableFooter(oSB);
			AppendHr(oSB);
			return oSB.ToString();
		}

		#endregion

		#region GetExceptions

		public static void GetExceptions(Exception oCurrentException, ErrorTracker.ErrorInfo oErrorInfo)
		{
			string sFullTrace = string.Empty;
			string strInnerErrorType = string.Empty;
			string strErrorTrace = string.Empty;
			string strErrorLine = string.Empty;
			string strErrorFile = string.Empty;
			string strErrorMessage = string.Empty;
			string strErrorPage = System.Web.HttpContext.Current.Request.PhysicalPath;
			StringBuilder oSB = new StringBuilder();

			AppendTableHeader(oSB);

			oSB.Append("<tr>");
			oSB.Append("<td colspan=2><h3>Exceptions</h3></td>");
			oSB.Append("</tr>");

			bool bFirstStepDown = true;

			while (oCurrentException != null)
			{
				Exception exInnerError = oCurrentException;
				if (exInnerError != null)
				{
					strInnerErrorType = exInnerError.GetType().ToString();
					switch (strInnerErrorType)
					{
						// ascx/aspx compile error 
						case "System.Web.HttpCompileException":
							System.CodeDom.Compiler.CompilerErrorCollection colErrors = ((System.Web.HttpCompileException)exInnerError).Results.Errors;
							if (colErrors.Count > 0)
							{
								strErrorLine = colErrors[0].Line.ToString();
								strErrorFile = colErrors[0].FileName;
								strErrorMessage = colErrors[0].ErrorNumber + ": " + colErrors[0].ErrorText;
							}
							break;

						// any other error like XML parsing or bad string manipulations 
						default:
							System.Diagnostics.StackTrace stError = new System.Diagnostics.StackTrace(exInnerError, true);
							for (int i = 0; i < stError.FrameCount; i++)
							{
								if (stError.GetFrame(i).GetFileName() != null)
								{
									strErrorLine = stError.GetFrame(i).GetFileLineNumber().ToString();
									strErrorFile = stError.GetFrame(i).GetFileName();
									strErrorMessage = exInnerError.Message;
									break;
								}
							}
							if (strErrorFile == "")
							{
								strErrorMessage = exInnerError.Message;
								strErrorFile = "Unknown";
							}
							break;
					}
					strErrorTrace = exInnerError.StackTrace;
					sFullTrace += strErrorTrace;

				}
				else
				{
					strErrorMessage = oCurrentException.Message;
					strErrorTrace = oCurrentException.StackTrace;
					strErrorFile = "Unknown";
				}

				if (strErrorMessage.IndexOf("This is an invalid webresource request.") > -1)
				{
					if (DisableTelerikWebResourceException)
						throw new TelerikWebResourceException();
				}
				else if (strErrorMessage.IndexOf("Application is restarting") > -1)
				{
					throw new ApplicationStartException();
				}

				if ((strInnerErrorType != null) && (strInnerErrorType.Trim().Length > 0))
				{
					AppendTableRow(oSB, "Type", strInnerErrorType, true);
				}

				if ((strErrorMessage != null) && (strErrorMessage.Trim().Length > 0))
				{
					AppendTableRow(oSB, "Message", strErrorMessage, true, "color:red;");
				}

				if ((strErrorFile != null) && (strErrorFile.Trim().Length > 0))
				{
					AppendTableRow(oSB, "Error File", CreateAnchor(strErrorFile), true);
				}

				if ((strErrorLine != null) && (strErrorLine.Trim().Length > 0))
				{
					AppendTableRow(oSB, "Error Line", strErrorLine, true);
				}

				if ((strErrorTrace != null) && (strErrorTrace.Trim().Length > 0))
				{
					AppendTableRow(oSB, "StackTrace", strErrorTrace.Replace("\n", "<br />"), true);
				}

				oSB.Append("<tr>");
				oSB.Append("<td colspan2>&nbsp;</td>");
				oSB.Append("</tr>");

				if (bFirstStepDown)
				{
					oErrorInfo.Name = strInnerErrorType;
					oErrorInfo.ExceptionType = strInnerErrorType;
					oErrorInfo.ErrorLineNumber = string.Empty == strErrorLine ? 0 : Convert.ToInt32(strErrorLine);
					oErrorInfo.SourceFile = strErrorFile;
					oErrorInfo.Description = strErrorMessage;
				}

				oCurrentException = oCurrentException.InnerException;

				bFirstStepDown = false;
			}
			AppendTableFooter(oSB);
			AppendHr(oSB);
			// 1line     
			if (strErrorTrace != null)
				oErrorInfo.StackTrace = strErrorTrace.Replace("\n", "<br />");
			if ((oErrorInfo.Name == null) || (oErrorInfo.Name.Length == 0))
			{
				oErrorInfo.Name = "Unknown Exception";
			}
			oErrorInfo.ExceptionsDescription = oSB.ToString();
		}

		#endregion

		#region GetForm

		public static string GetForm()
		{
			StringBuilder oSB = new StringBuilder();

			if (System.Web.HttpContext.Current.Request.Form.Count > 0)
			{
				AppendTableHeader(oSB);
				oSB.Append("<tr>");
				oSB.Append("<td colspan=2><b>Form</b></td>");
				oSB.Append("</tr>");

				string FormContent = string.Empty;
				foreach (string key in System.Web.HttpContext.Current.Request.Form)
				{
					FormContent += ToStringOrEmpty(System.Web.HttpContext.Current.Request.Form[key]);
				}
				AppendTableRow(oSB, "Form content length", FormContent.Length.ToString(), false);

				foreach (string key in System.Web.HttpContext.Current.Request.Form)
				{
					AppendTableRow(oSB, key, ToStringOrEmpty(System.Web.HttpContext.Current.Request.Form[key]), false);
				}

				AppendTableFooter(oSB);
				AppendHr(oSB);
			}

			return oSB.ToString();
		}

		#endregion

		#region GetQueryString

		public static string GetQueryString()
		{
			StringBuilder oSB = new StringBuilder();

			if (System.Web.HttpContext.Current.Request.QueryString.Count > 0)
			{
				AppendTableHeader(oSB);

				oSB.Append("<tr>");
				oSB.Append("<td colspan=2><b>QueryString</b></td>");
				oSB.Append("</tr>");

				foreach (string key in System.Web.HttpContext.Current.Request.QueryString)
				{
					AppendTableRow(oSB, key, ToStringOrEmpty(System.Web.HttpContext.Current.Request.QueryString[key]), false);
				}

				AppendTableFooter(oSB);
				//AppendHr(oSB);
			}
			return oSB.ToString();
		}

		#endregion

		#region GetSession

		public static string GetSession()
		{
			StringBuilder oSB = new StringBuilder();

			if ((System.Web.HttpContext.Current != null) && (System.Web.HttpContext.Current.Session != null) && (System.Web.HttpContext.Current.Session.Count > 0))
			{

				AppendTableHeader(oSB);
				oSB.Append("<tr>");
				oSB.Append("<td colspan=2><b>Session</b></td>");
				oSB.Append("</tr>");

				try
				{
					string content = string.Empty;

					foreach (string key in System.Web.HttpContext.Current.Session.Keys)
					{
						content += ToStringOrEmpty(System.Web.HttpContext.Current.Session[key]);
					}
					AppendTableRow(oSB, "Session length", content.Length.ToString(), false);

					foreach (string key in System.Web.HttpContext.Current.Session.Keys)
					{
                        AppendStateBagEntry(oSB, key, System.Web.HttpContext.Current.Session[key].ToString().Replace(Environment.NewLine, "<br />"), System.Web.HttpContext.Current.Session[key].GetType());
					}
				}
				catch (Exception ex)
				{
					AppendTableRow(oSB, "Error occured while processing current Session object.", ex.StackTrace, false);
				}

				AppendTableFooter(oSB);
				AppendHr(oSB);
			}

			return oSB.ToString();
		}

		#endregion

		#region GetApplication

		public static string GetApplication()
		{
			StringBuilder oSB = new StringBuilder();

			if (System.Web.HttpContext.Current.Application != null)
			{

				AppendTableHeader(oSB);
				oSB.Append("<tr>");
				oSB.Append("<td colspan=2><b>Application</b></td>");
				oSB.Append("</tr>");
				AppendStateBagEntry(oSB, "Application.Keys count", System.Web.HttpContext.Current.Application.Keys.Count, typeof(int));

				foreach (string key in System.Web.HttpContext.Current.Application.Keys)
				{
					AppendStateBagEntry(oSB, "Key", key, typeof(string));
				}

				AppendTableFooter(oSB);
				AppendHr(oSB);
			}

			return oSB.ToString();
		}

		#endregion

		#region GetRequestCookies

		public static string GetRequestCookies()
		{
			StringBuilder oSB = new StringBuilder();

			if (System.Web.HttpContext.Current.Request != null &&
				System.Web.HttpContext.Current.Request.Cookies != null &&
				System.Web.HttpContext.Current.Request.Cookies.Count > 0)
			{
				AppendTableHeader(oSB);
				oSB.Append("<tr>");
                oSB.Append("<td><b>Request Cookies</b></td><td></td><td></td>");
				oSB.Append("</tr>");
                oSB.Append("<tr><td align='center'><b>Name</b></td><td align='center'><b>Original</b></td><td align='center'><b>Decoded</b></td></tr>");

				foreach (string key in System.Web.HttpContext.Current.Request.Cookies)
				{
                    AppendTableRowRC(oSB, key, ToStringOrEmpty(System.Web.HttpContext.Current.Request.Cookies[key].Value), true);

					if (System.Web.HttpContext.Current.Request.Cookies[key].HasKeys)
					{
						foreach (string subkey in System.Web.HttpContext.Current.Request.Cookies[key].Values)
						{
							var str = ToStringOrEmpty(System.Web.HttpContext.Current.Request.Cookies[key][subkey]);
                            AppendTableRowRC(oSB, subkey, str, false);
						}
					}
				}

				AppendTableFooter(oSB);
				AppendHr(oSB);
			}
			return oSB.ToString();
		}

		#endregion

		#region GetResponseCookies

		public static string GetResponseCookies()
		{
			StringBuilder oSB = new StringBuilder();

			if (System.Web.HttpContext.Current.Response != null &&
				System.Web.HttpContext.Current.Response.Cookies != null &&
				System.Web.HttpContext.Current.Response.Cookies.Count > 0)
			{
				AppendTableHeader(oSB);
				oSB.Append("<tr>");
				oSB.Append("<td colspan=2><b>Response Cookies</b></td>");
				oSB.Append("</tr>");

				foreach (string key in System.Web.HttpContext.Current.Response.Cookies)
				{
					var str = ToStringOrEmpty(System.Web.HttpContext.Current.Response.Cookies[key].Value);
					AppendTableRow(oSB, key + " (original)", str, true);
					AppendTableRow(oSB, "<font color=#006633>" + key + " (decode)" + "</font>", "<font color=#006633>" + Microsoft.JScript.GlobalObject.unescape(str) + "</font>", true);

					if (System.Web.HttpContext.Current.Response.Cookies[key].HasKeys)
					{
						foreach (string subkey in System.Web.HttpContext.Current.Response.Cookies[key].Values)
						{
							var value = ToStringOrEmpty(System.Web.HttpContext.Current.Response.Cookies[key][subkey]);
							AppendTableRow(oSB, subkey + " (original)", ToStringOrEmpty(value), false);
							AppendTableRow(oSB, "<font color=#006633>" + subkey + " (decode)" + "</font>", "<font color=#006633>" + Microsoft.JScript.GlobalObject.unescape(value) + "</font>", false);
						}
					}
				}

				AppendTableFooter(oSB);
				AppendHr(oSB);
			}
			return oSB.ToString();
		}

		#endregion

		#region GetRequestHeaders

		public static string GetRequestHeaders()
		{
			StringBuilder oSB = new StringBuilder();

			if (System.Web.HttpContext.Current.Request.Headers.Count > 0)
			{
				AppendTableHeader(oSB);
				oSB.Append("<tr>");
				oSB.Append("<td colspan=2><b>Request Headers</b></td>");
				oSB.Append("</tr>");

				foreach (string key in System.Web.HttpContext.Current.Request.Headers)
				{
					AppendTableRow(oSB, key, ToStringOrEmpty(System.Web.HttpContext.Current.Request.Headers[key]), false);
				}

				AppendTableFooter(oSB);
				AppendHr(oSB);
			}
			return oSB.ToString();
		}

		#endregion

		#region GetServerVariables

		public static string GetServerVariables()
		{
			StringBuilder oSB = new StringBuilder();

			if (System.Web.HttpContext.Current.Request.ServerVariables.Count > 0)
			{
				AppendTableHeader(oSB);

				oSB.Append("<tr>");
				oSB.Append("<td colspan=2><b>ServerVariables</b></td>");
				oSB.Append("</tr>");

				foreach (string key in System.Web.HttpContext.Current.Request.ServerVariables)
				{
					oSB.Append("<tr>");
					oSB.Append("<td valign='top'><i><font size=-1>" + key + "</font></i></td>");
					oSB.Append("<td><font size=-1>" + ToStringOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables[key]) + "</font></td>");
					oSB.Append("</tr>");
				}
				AppendTableFooter(oSB);
				AppendHr(oSB);
			}
			return oSB.ToString();
		}

		#endregion

		#region GetTrace

		public static string GetTrace()
		{
			StringBuilder oSB = new StringBuilder();
			try
			{
				oSB.Append("<table cellpadding=2 cellspacing=1>");
				oSB.Append("<tr>");
				oSB.Append("<td colspan=2><b>Trace</b></td>");
				oSB.Append("</tr>");

				System.Type trace = System.Web.HttpContext.Current.Trace.GetType();
				MethodInfo mi = trace.GetMethod("Render", BindingFlags.Instance | BindingFlags.NonPublic);
				StringWriter sWriter = new StringWriter();
				mi.Invoke(System.Web.HttpContext.Current.Trace, new object[] { new HtmlTextWriter(sWriter) });

				oSB.Append("<tr>");
				oSB.Append("<td valign='top' colspan='2'><i>" + sWriter.ToString() + "</i></td>");
				oSB.Append("</tr>");
			}
			catch (Exception exe)
			{
				oSB.Append("<tr>");
				oSB.Append("<td valign='top' colspan='2'><i>" + exe.Message + "</i></td>");
				oSB.Append("</tr>");
			}
			finally
			{
				oSB.Append("</table>");
			}
			return oSB.ToString();
		}

		#endregion

		#region GetCache

        public static decimal GetCacheSize(ref string cacheItemsInfo)
		{
            IDictionaryEnumerator cacheEnum = System.Web.HttpContext.Current.Cache.GetEnumerator();
            Type[] altSerializationType = { typeof(String), typeof(Int32), typeof(Boolean), typeof(DateTime), typeof(Decimal), typeof(Byte), typeof(Char), typeof(Single), typeof(Double), typeof(Int16), typeof(Int64),
                                                typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(SByte), typeof(TimeSpan), typeof(Guid), typeof(IntPtr), typeof(UIntPtr)};
            decimal cacheTotal = 0;
            StringBuilder oSB = new StringBuilder();
            AppendTableHeader(oSB);
            oSB.Append("<tr>");
            oSB.Append("<td><b>Items</b></td><td><b>Size, KB</b></td>");
            oSB.Append("</tr>");
            while (cacheEnum.MoveNext())
            {
                var key = ToStringOrEmpty(cacheEnum.Key);
                if (key.IndexOf("System.Web", 0) < 0 && key.IndexOf("ISAPIWorkerRequest", 0) < 0 &&
                    cacheEnum.Value != null)
                {
                    try
                    {
                        decimal cacheItemSize = 0;
                        Stream alternativeSerializationStreamSessionKey = BinaryWrite(key, altSerializationType);
                        if (TypeIsInAlternativeSerializationList(cacheEnum.Value.GetType(), altSerializationType))
                        {
                            Stream alternativeSerializationStream = BinaryWrite(cacheEnum.Value, altSerializationType);
                            cacheItemSize = Convert.ToDecimal(alternativeSerializationStream.Length + alternativeSerializationStreamSessionKey.Length);
                        }
                        else
                        {
                            MemoryStream m;
                            m = BinarySerialize(cacheEnum.Value);
                            cacheItemSize = Convert.ToDecimal(m.Length + alternativeSerializationStreamSessionKey.Length);
                        }
                        cacheTotal += cacheItemSize;
                        oSB.Append("<tr>");
                        oSB.Append("<td valign='top'><i><font size=-1>" + key + "</font></i></td>");
                        oSB.Append("<td><font size=-1>" + cacheItemSize.ToString("N") + "</font></td>");
                        oSB.Append("</tr>");
                    }
                    catch
                    { }
                }
            }
            AppendTableFooter(oSB);
            cacheItemsInfo = oSB.ToString();
            return cacheTotal / Convert.ToDecimal(1000);
		}

		#endregion

		private static string ToStringOrEmpty(object stringValue)
		{
			if (stringValue != null)
				return stringValue.ToString();

			return string.Empty;
		}

        private static Stream BinaryWrite(object valueToWrite, Type[] altSerializationType)
        {

            BinaryWriter writer = new BinaryWriter(new MemoryStream());

            System.Type valueType = valueToWrite.GetType();

            if (valueToWrite == null)
            {
                writer.Write(16);
                return writer.BaseStream;
            }

            if (valueType == altSerializationType[0])
            {
                writer.Write(1);
                writer.Write((String)valueToWrite);
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[1])
            {
                writer.Write(2);
                writer.Write((Int32)valueToWrite);
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[2])
            {
                writer.Write(3);
                writer.Write((Boolean)valueToWrite);
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[3])
            {
                writer.Write(4);
                DateTime tempdateTime = (DateTime)valueToWrite;
                writer.Write(tempdateTime.Ticks);
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[4])
            {
                writer.Write(5);
                int[] decimalBits = Decimal.GetBits((Decimal)valueToWrite);
                int i = 0;
                while (i < 4)
                {
                    writer.Write(decimalBits[i]);
                    i++;
                }
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[5])
            {
                writer.Write(6);
                writer.Write((Byte)valueToWrite);
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[6])
            {
                writer.Write(6);
                writer.Write((Char)valueToWrite);
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[7])
            {
                writer.Write(8);
                writer.Write((Single)valueToWrite);
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[8])
            {
                writer.Write(9);
                writer.Write((Double)valueToWrite);
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[9])
            {
                writer.Write(10);
                writer.Write((Int16)valueToWrite);
                return writer.BaseStream;
            }
            if (valueToWrite == altSerializationType[10])
            {
                writer.Write(11);
                writer.Write((Int64)valueToWrite);
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[11])
            {
                writer.Write(12);
                writer.Write((UInt16)valueToWrite);
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[12])
            {
                writer.Write(13);
                writer.Write((UInt32)valueToWrite);
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[13])
            {
                writer.Write(14);
                writer.Write((UInt64)valueToWrite);
                return writer.BaseStream;
            }

            if (valueType == altSerializationType[14])
            {
                writer.Write(10);
                writer.Write((SByte)valueToWrite);
                return writer.BaseStream;
            }

            if (valueType == altSerializationType[15])
            {
                writer.Write(16);
                TimeSpan timespanToWrite = (TimeSpan)valueToWrite;
                writer.Write(timespanToWrite.Ticks);
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[16])
            {
                writer.Write(17);
                Guid guidToWrite = (Guid)valueToWrite;
                byte[] guidAsByteArray = guidToWrite.ToByteArray();
                writer.Write(guidAsByteArray);
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[17])
            {
                writer.Write(18);
                IntPtr intptrToWrite = (IntPtr)valueToWrite;
                if (IntPtr.Size == 4)
                {
                    writer.Write(intptrToWrite.ToInt32());
                    return writer.BaseStream;
                }
                writer.Write(intptrToWrite.ToInt64());
                return writer.BaseStream;
            }
            if (valueType == altSerializationType[18])
            {
                writer.Write(19);
                UIntPtr uintptrToWrite = (UIntPtr)valueToWrite;
                if (UIntPtr.Size == 4)
                {
                    writer.Write(uintptrToWrite.ToUInt32());
                    return writer.BaseStream;
                }
                writer.Write(uintptrToWrite.ToUInt64());
                return writer.BaseStream;
            }

            return writer.BaseStream;

        }

        private static bool TypeIsInAlternativeSerializationList(Type type, Type[] altSerializationType)
        {

            for (int i = 0; i <= (altSerializationType.Length - 1); i++)
            {
                if (type == altSerializationType[i])
                {
                    return true;
                }
            }
            return false;
        }

        internal static MemoryStream BinarySerialize(object objectToSerialize)
        {
            MemoryStream m = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();

            b.Serialize(m, objectToSerialize);

            return m;
        }
	}
}
