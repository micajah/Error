using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for ConvertHelper
/// </summary>
public class ConvertHelper
{
    public ConvertHelper()
    {
    }

    public static int GetIntValue(string str)
    {
        try
        {
            return Convert.ToInt32(str);
        }
        catch (Exception)
        {
            return 0;
        }
    }    
}
