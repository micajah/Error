using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections;

namespace Micajah.ErrorTrackerHelper
{
    internal class ApplicationStartException : Exception
    {
        public ApplicationStartException() : base()
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

		private string strErrorMessage = String.Empty;
		private Exception oCurrentException;
		private bool _DrillDownInCache = false;
		private bool _ReturnCache = false;
		private string FullTrace = String.Empty;
		private int _TheFloodCount = 10;
		private int _FloodMins = 30;
		private string _ContentAfterException = String.Empty;
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

			foreach(string hkey in ht.Keys) 
			{
				AppendTableRow(oSB, hkey.ToString(),ht[hkey].ToString(),false);
			}
									
			oSB.Append("</table>");
			oSB.Append("</td>");
			oSB.Append("</tr>");
			#endregion
        }

        #endregion

        #region AppendArrayList

        private static void AppendArrayList(StringBuilder oSB, System.Collections.ArrayList al)
		{
			#region Add the Table Rows To the String Builder
			foreach(object item in al) 
			{
				oSB.Append("<tr>");
				oSB.Append("<td valign='top'></td>");
				oSB.Append("<td>" + item.ToString() + "</i></td>");
				oSB.Append("</tr>");
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
				AppendTableRow(oSB, Name,(string)Value,false);
			}
			else if (ObjectType == typeof(System.Collections.Hashtable))
			{
                AppendHashTable(oSB, (System.Collections.Hashtable)Value);
			}
			else if (ObjectType == typeof(System.Collections.ArrayList))
			{
                AppendArrayList(oSB, (System.Collections.ArrayList)Value);
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

			AppendTableRow(oSB, "Row Count","RowCount : " + Dt.Rows.Count.ToString(),true);

			oSB.Append("<tr>");
			oSB.Append("<td valign='top'></td>");
			oSB.Append("<td>");
									
			if (Dt.Rows.Count > 0)
			{
				oSB.Append("<table cellspacing=2 cellpadding=2 border=1>");
				oSB.Append("<tr>");
				foreach(System.Data.DataColumn Dc in Dt.Columns)
				{
					oSB.Append("<th>" + Dc.ColumnName + "</th>");
				}
				oSB.Append("</tr>");

				foreach(DataRow Dr in Dt.Rows)
				{	
					oSB.Append("<tr>");
					foreach(System.Data.DataColumn Dc in Dt.Columns)
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
			AppendTableRow(oSB, ".NET Framework Version",System.Environment.Version.ToString(),true);
			AppendTableRow(oSB, "&#160;","&#160;", false);

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			
			foreach(Assembly a in assemblies)
			{	
				if ((a.GetName().Name.ToString().IndexOf("System") < 0) && 
                    (a.GetName().Version.ToString() != "0.0.0.0") && 
                    (a.GetName().Name.ToString().IndexOf("mscorlib") < 0))
				{
					string Version = a.GetName().Version.ToString();
					AssemblyInformationalVersionAttribute[] infoversion =	(AssemblyInformationalVersionAttribute[]) a.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
					if (infoversion.Length == 1 )
					{
						Version += (" (" + infoversion[0].InformationalVersion.ToString() + ")");
					}

					AppendTableRow(oSB, a.GetName().Name.ToString(), Version, false);
					AppendTableRow(oSB, "Codebase", CreateAnchor(a.CodeBase.ToString()),false);
                    try
                    {
                        FileInfo F = new FileInfo(a.Location);
                        AppendTableRow(oSB, "Last Write Time", File.GetLastWriteTime(F.FullName).ToLongDateString() + " " + File.GetLastWriteTime(F.FullName).ToLongTimeString(), false);
                    }
                    catch { }
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
						case "System.Web.HttpCompileException" : 
							System.CodeDom.Compiler.CompilerErrorCollection colErrors = ((System.Web.HttpCompileException)exInnerError).Results.Errors; 
							if (colErrors.Count > 0) 
							{
                                strErrorLine = colErrors[0].Line.ToString(); 
                                strErrorFile = colErrors[0].FileName;
                                strErrorMessage = colErrors[0].ErrorNumber + ": " + colErrors[0].ErrorText; 
							} 
							break; 

						// any other error like XML parsing or bad string manipulations 
						default : 
							System.Diagnostics.StackTrace stError = new System.Diagnostics.StackTrace(exInnerError, true); 
							for (int i=0;i<stError.FrameCount;i++) 
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
								strErrorMessage = "Untrapped Exception: " + exInnerError.Message; 
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

				if (strErrorMessage.IndexOf("Application is restarting") > -1)
				{
                    throw new ApplicationStartException();
				}

				if ((strInnerErrorType != null) && (strInnerErrorType.Trim().Length > 0))
				{
					AppendTableRow(oSB, "Type",strInnerErrorType.ToString(),true);
				}

				if ((strErrorMessage != null) && (strErrorMessage.Trim().Length > 0))
				{
					AppendTableRow(oSB, "Message",strErrorMessage.ToString(),true,"color:red;");
				}

                if ((strErrorFile != null) && (strErrorFile.Trim().Length > 0))
                {
                    AppendTableRow(oSB, "Error File", CreateAnchor(strErrorFile.ToString()), true);
                }

                if ((strErrorLine != null) && (strErrorLine.Trim().Length > 0))
                {
                    AppendTableRow(oSB, "Error Line", strErrorLine.ToString(), true);
                }

                if ((strErrorTrace != null) && (strErrorTrace.Trim().Length > 0))
				{
					AppendTableRow(oSB, "StackTrace",strErrorTrace.ToString().ToString().Replace("\n","<br />"),true);
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

				foreach(string key in System.Web.HttpContext.Current.Request.Form)
				{
			        AppendTableRow(oSB, key, System.Web.HttpContext.Current.Request.Form[key].ToString(), false);
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

				foreach( string key in System.Web.HttpContext.Current.Request.QueryString)
				{
					AppendTableRow(oSB, key, System.Web.HttpContext.Current.Request.QueryString[key].ToString(), false);
				}

				AppendTableFooter(oSB);
				AppendHr(oSB);
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

				foreach(string key in System.Web.HttpContext.Current.Session.Keys)
				{
					AppendStateBagEntry(oSB, key,System.Web.HttpContext.Current.Session[key],System.Web.HttpContext.Current.Session[key].GetType());
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

				foreach(string key in System.Web.HttpContext.Current.Application.Keys)
				{
					AppendStateBagEntry(oSB, key, System.Web.HttpContext.Current.Application[key], System.Web.HttpContext.Current.Application[key].GetType());
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

			if (System.Web.HttpContext.Current.Request.Cookies.Count > 0)
			{
				AppendTableHeader(oSB);
				oSB.Append("<tr>");
				oSB.Append("<td colspan=2><b>Request Cookies</b></td>");
				oSB.Append("</tr>");

				foreach( string key in System.Web.HttpContext.Current.Request.Cookies)
				{
					AppendTableRow(oSB, key, System.Web.HttpContext.Current.Request.Cookies[key].Value.ToString(), true);

					if (System.Web.HttpContext.Current.Request.Cookies[key].HasKeys)
					{
						foreach(string subkey in System.Web.HttpContext.Current.Request.Cookies[key].Values)
						{
							AppendTableRow(oSB, subkey,System.Web.HttpContext.Current.Request.Cookies[key][subkey].ToString(),false);
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

			if (System.Web.HttpContext.Current.Response.Cookies.Count > 0)
			{
				AppendTableHeader(oSB);
				oSB.Append("<tr>");
				oSB.Append("<td colspan=2><b>Response Cookies</b></td>");
				oSB.Append("</tr>");

				foreach( string key in System.Web.HttpContext.Current.Response.Cookies)
				{
					AppendTableRow(oSB, key,System.Web.HttpContext.Current.Response.Cookies[key].Value.ToString(),true);

					if (System.Web.HttpContext.Current.Response.Cookies[key].HasKeys)
					{
						foreach(string subkey in System.Web.HttpContext.Current.Response.Cookies[key].Values)
						{
							AppendTableRow(oSB, subkey,System.Web.HttpContext.Current.Response.Cookies[key][subkey].ToString(),false);
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

				foreach( string key in System.Web.HttpContext.Current.Request.Headers)
				{
					AppendTableRow(oSB, key,System.Web.HttpContext.Current.Request.Headers[key].ToString(),false);
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

				foreach( string key in System.Web.HttpContext.Current.Request.ServerVariables)
				{
					oSB.Append("<tr>");
					oSB.Append("<td valign='top'><i>" + key + "</i></td>");
					oSB.Append("<td>" + System.Web.HttpContext.Current.Request.ServerVariables[key].ToString() + "</td>");
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
				MethodInfo mi = trace.GetMethod("Render",BindingFlags.Instance|BindingFlags.NonPublic);
				StringWriter sWriter = new StringWriter();
                mi.Invoke(System.Web.HttpContext.Current.Trace, new object[] {new HtmlTextWriter(sWriter)});

				oSB.Append("<tr>");
				oSB.Append("<td valign='top' colspan='2'><i>" + sWriter.ToString() + "</i></td>");
				oSB.Append("</tr>");
			}
			catch(Exception exe)
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

        public static string GetCache()
		{
            StringBuilder oSB = new StringBuilder();

			IDictionaryEnumerator cacheEnum = System.Web.HttpContext.Current.Cache.GetEnumerator();
			int CacheCount = 1;

			while (cacheEnum.MoveNext())
			{
				if (CacheCount <= 1)
				{
					oSB.Append("<table cellpadding=2 cellspacing=1>");
					oSB.Append("<tr>");
					oSB.Append("<td colspan=2><b>Cache</b></td>");
					oSB.Append("</tr>");
					CacheCount++;
				}
					
				if (cacheEnum.Key.ToString().IndexOf("System.Web", 0) < 0 && cacheEnum.Key.ToString().IndexOf("ISAPIWorkerRequest", 0) < 0 ) 
				{
					AppendStateBagEntry(oSB, cacheEnum.Key.ToString(),cacheEnum.Value,cacheEnum.Value.GetType());
				}
			}
            return oSB.ToString();
        }

        #endregion

    }
}
