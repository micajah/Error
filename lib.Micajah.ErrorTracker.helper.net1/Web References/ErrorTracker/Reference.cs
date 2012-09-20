﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.2032
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 1.1.4322.2032.
// 
namespace Micajah.ErrorTrackerHelper.ErrorTracker {
    using System.Diagnostics;
    using System.Collections;
    using System.Xml.Serialization;
    using System;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Web.Services;
    
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ServiceSoap", Namespace="http://www.micajah.com/ErrorTracker/")]
    public class Service : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        /// <remarks/>
        public Service() {
            string urlSetting = System.Configuration.ConfigurationSettings.AppSettings["Micajah_ErrorTracker_Service"];
            if ((urlSetting != null)) {
                this.Url = string.Concat(urlSetting, "");
            }
            else {
                this.Url = "http://dev1.atl2.micajah.net/MITS_ET_Prod/Service.asmx";
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.micajah.com/ErrorTracker/AddErrorInfoError", RequestElementName="AddErrorInfoError", RequestNamespace="http://www.micajah.com/ErrorTracker/", ResponseElementName="AddErrorInfoErrorResponse", ResponseNamespace="http://www.micajah.com/ErrorTracker/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("AddErrorInfoErrorResult")]
        public int AddError(ErrorInfo oErrorInfo) {
            object[] results = this.Invoke("AddError", new object[] {
                        oErrorInfo});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginAddError(ErrorInfo oErrorInfo, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("AddError", new object[] {
                        oErrorInfo}, callback, asyncState);
        }
        
        /// <remarks/>
        public int EndAddError(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((int)(results[0]));
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.micajah.com/ErrorTracker/")]
    public class ErrorInfo {
        
        /// <remarks/>
        public int ApplicationID;
        
        /// <remarks/>
        public string Browser;
        
        /// <remarks/>
        public string Method;
        
        /// <remarks/>
        public string Name;
        
        /// <remarks/>
        public string Description;
        
        /// <remarks/>
        public string URL;
        
        /// <remarks/>
        public string URLReferrer;
        
        /// <remarks/>
        public string PhysicalFileName;
        
        /// <remarks/>
        public string SourceFile;
        
        /// <remarks/>
        public string ErrorFile;
        
        /// <remarks/>
        public int ErrorLineNumber;
        
        /// <remarks/>
        public string QueryString;
        
        /// <remarks/>
        public string MachineName;
        
        /// <remarks/>
        public string UserIPAddress;
        
        /// <remarks/>
        public string UserHostName;
        
        /// <remarks/>
        public string ExceptionType;
        
        /// <remarks/>
        public string StackTrace;
        
        /// <remarks/>
        public string ExceptionsDescription;
        
        /// <remarks/>
        public string QueryStringDescription;
        
        /// <remarks/>
        public string Form;
        
        /// <remarks/>
        public string Session;
        
        /// <remarks/>
        public string ApplicationDescription;
        
        /// <remarks/>
        public string Version;
        
        /// <remarks/>
        public string RequestCookies;
        
        /// <remarks/>
        public string ResponseCookies;
        
        /// <remarks/>
        public string RequestHeader;
        
        /// <remarks/>
        public string ServerVariables;
        
        /// <remarks/>
        public string Cache;
    }
}