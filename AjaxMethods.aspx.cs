using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using ProiectLicenta;
using System.Web.UI.HtmlControls;
using System.Web.Services;

public partial class AjaxMethods : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        BtnNotif_Click();
    }

    [WebMethod]
    public static string GetNotifNumber()
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        //GET NUMBER OF NEW NOTIFICATIONS
        SqlCommand cmd2 = new SqlCommand(@"select
                                                (Select count(*)
                                                from VideoStatistics vs
                                                join Interactions intr on intr.Id = vs.Interaction_type
                                                Join AspNetUsers anu on anu.id = vs.User_id
                                                where (intr.Name = 'Favorite' or intr.Name = 'Like' or intr.Name = 'Comment') and vs.User_id <> @UID
                                                and (vs.Video_id in (Select id from videos
	                                                where User_id = @UID))
                                                and Interaction_date >= anu.NotifCheck)
                                                +
                                                (Select count(*)
                                                from AspNetUsers anu
                                                JOIN Subscriptions subs on subs.User_id = anu.Id
                                                where Fallowing_id = @UID 
                                                and subs.Date_added >= anu.NotifCheck)
                                                 + 
                                                (Select COUNT(*)
                                                 from User_Wall_Posts uwp
                                                 JOIN AspNetUsers anu on anu.Id = uwp.User_posted
                                                 where User_wall = @UID and uwp.User_posted <> @UID and uwp.Date_Posted >= anu.NotifCheck)", conn);
        cmd2.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = System.Web.HttpContext.Current.User.Identity.GetUserId();

        int CurNotifNr = (int)cmd2.ExecuteScalar();
        conn.Close();

        //if (CurNotifNr == 0)
        //    return " ";
        //else
            return CurNotifNr.ToString();

    }

    [WebMethod]
    public static string BtnNotif_Click()
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd1 = new SqlCommand("Update AspNetUsers Set NotifCheck = GETDATE() where id = @UID", conn);
        cmd1.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = System.Web.HttpContext.Current.User.Identity.GetUserId();
        cmd1.ExecuteNonQuery();
        conn.Close();

        return GetNotifNumber();

    }
}