using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class VeryLongNamePart1_VeryLongNamePart2_VeryLongNamePart3_VeryLongNamePart4_VeryLongNamePart5_VeryLongURL : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        throw new BadImageFormatException("Very Long Exception");
    }
}
