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


public partial class SiteMaster : MasterPage
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;

    protected void Page_Init(object sender, EventArgs e)
    {

        // The code below helps to protect against XSRF attacks
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        Guid requestCookieGuidValue;
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
        {
            // Use the Anti-XSRF token from the cookie
            _antiXsrfTokenValue = requestCookie.Value;
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        else
        {
            // Generate a new Anti-XSRF token and save to the cookie
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            Page.ViewStateUserKey = _antiXsrfTokenValue;

            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                HttpOnly = true,
                Value = _antiXsrfTokenValue
            };
            if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
            {
                responseCookie.Secure = true;
            }
            Response.Cookies.Set(responseCookie);
        }

        Page.PreLoad += master_Page_PreLoad;

    }

    protected void master_Page_PreLoad(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Set Anti-XSRF token
            ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
            ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? String.Empty;

        }
        else
        {
            // Validate the Anti-XSRF token
            if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? String.Empty))
            {
                throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd1 = new SqlCommand("Select * from Categories", conn);
        string list = "";
        try
        {
            using (SqlDataReader rdr = cmd1.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        list += "<li><a href='/Results?categ=" + rdr.GetInt32(0) + "' >" + rdr.GetString(1) + "</a></li>";
                    }
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("No rows found.");
        }
        categories_list.InnerHtml += list;
        conn.Close();
        Playlist_Load();
        if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
        {
            var manager = new UserManager();
            if (manager.IsInRole(System.Web.HttpContext.Current.User.Identity.GetUserId(), "Admin"))
            {
                AdminMenu.Visible = true;
                NavMenu.Visible = false;
            }
            else
            {
                AdminMenu.Visible = false;
                NavMenu.Visible = true;
            }

        }
        else
        {
            AdminMenu.Visible = false;
            NavMenu.Visible = true;
        }

        NotificationFetch();

    }

    protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
    {
        Context.GetOwinContext().Authentication.SignOut();
    }

    protected void Unnamed_Click(object sender, EventArgs e)
    {
        if (NavMenu.Visible == false)
        {
            // this.Visible = false;
            // btnRandom.visible = false;
            NavMenu.Visible = true;
        }
        else
        {
            // this.Visible = true;         
            NavMenu.Visible = false;
        }
    }

    protected void Category_list()
    {
        List<Temp_DataType> list = new List<Temp_DataType>();
        Temp_DataType pl1 = new Temp_DataType();
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Select Id,Name from Categories", conn);

    }

    protected void Playlist_Load()
    {
        if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
        {
            string id = HttpContext.Current.User.Identity.GetUserId();

            List<Temp_DataType> list = new List<Temp_DataType>();
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("Select Id,Name,date_created from Playlists where User_id = @usrId and Name <> 'History' order by Date_Created desc", conn);
            cmd.Parameters.Add(new SqlParameter("@usrId", TypeCode.String));
            cmd.Parameters["@usrId"].Value = id;
            try
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            Temp_DataType pl1 = new Temp_DataType();
                            pl1.Id = rdr.GetInt32(0);
                            pl1.Title = rdr.GetString(1);
                            list.Add(pl1);
                        }
                    rdr.Close();
                }
                (this.LoginMenu.FindControl("Playlist_repeater") as Repeater).DataSource = list;
                (this.LoginMenu.FindControl("Playlist_repeater") as Repeater).DataBind();
                conn.Close();
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("No rows found.");
            }
            catch (Exception)
            {
                Console.WriteLine("No rows found.");
            }
            // Playlist_repeater.DataSource = list;
            //Playlist_repeater.DateBind();

        }
    }

    public class Temp_DataType
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    protected void SearchButton_ServerClick(object sender, EventArgs e)
    {
        Response.Redirect("~/Results?src=" + SearchBox.Text);
    }

    protected void DeleteUser(object sender, EventArgs e)
    {
        if (contentCallEvent2 != null)
            contentCallEvent2(this, EventArgs.Empty);
    }
    protected void AdminDeleteVideo(object sender, EventArgs e)
    {
        if (contentCallEvent != null)
            contentCallEvent(this, EventArgs.Empty);
    }
    public event EventHandler contentCallEvent;
    public event EventHandler contentCallEvent2;

    public class NotifStruct
    {
        public string Url { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserIco { get; set; }
        public string InterName { get; set; }
        public string InterDate { get; set; }
        public DateTime DateForSort { get; set; }

    }

    protected void NotificationFetch()
    {
        if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();
            List<NotifStruct> ListNotif = new List<NotifStruct>();

            string message = string.Empty;
           
                    


            //GET VIDEO INTERACTIONS NOTIF
            SqlCommand cmd1 = new SqlCommand(@"Select TOP 15 vs.Video_id,vs.User_id,anu.UserName,anu.Icon,vs.Interaction_date,intr.Name,vids.Title
                                            from VideoStatistics vs
                                            JOIN Videos vids on vids.id = vs.Video_id
                                            join Interactions intr on intr.Id = vs.Interaction_type
                                            Join AspNetUsers anu on anu.id = vs.User_id
                                            where (intr.Name = 'Favorite' or intr.Name = 'Like' or intr.Name = 'Comment') and vs.User_id <> @UID
                                            and (vs.Video_id in (Select id from videos
	                                                            where User_id = @UID))
                                            order by vs.Interaction_date", conn);
            cmd1.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = System.Web.HttpContext.Current.User.Identity.GetUserId();
            try
            {
                using (SqlDataReader rdr = cmd1.ExecuteReader())
                {
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            NotifStruct nInfo = new NotifStruct();
                            nInfo.Url = "~/watch?id=" + rdr.GetInt32(0).ToString();
                            nInfo.UserId = rdr.GetString(1);
                            nInfo.UserName = rdr.GetString(2);
                            nInfo.UserIco = rdr.GetString(3).Replace(@"~\", "");
                            nInfo.InterDate = rdr.GetDateTime(4).ToString("dd MMMM, yyyy hh:mm");
                            nInfo.DateForSort = rdr.GetDateTime(4);
                            nInfo.InterName = rdr.GetString(5);
                            switch (rdr.GetString(5))
                            {
                                case "Like": nInfo.InterName = " has liked your video <br>" + rdr.GetString(6);
                                    break;
                                case "Favorite": nInfo.InterName = " has added your video to favorites <br>" + rdr.GetString(6);
                                    break;
                                case "Comment": nInfo.InterName = " has commented on your video <br>" + rdr.GetString(6);
                                    break;
                                default: nInfo.InterName = " other action ";
                                    break;
                            }
                            ListNotif.Add(nInfo);
                        }
                    rdr.Close();
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("No rows found.");
            }

            //GET NEW SUBSCRIBERS NOTIFICATIONS
            SqlCommand cmdSubs = new SqlCommand(@"Select TOP 15 anu.id,UserName,anu.Icon,subs.Date_added
                                                from AspNetUsers anu
                                                JOIN Subscriptions subs on subs.User_id = anu.Id
                                                where Fallowing_id = @UID", conn);
            cmdSubs.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = System.Web.HttpContext.Current.User.Identity.GetUserId();
            try
            {
                using (SqlDataReader rdr = cmdSubs.ExecuteReader())
                {
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            NotifStruct nInfo = new NotifStruct();
                            nInfo.Url = "~/Feed/ManageSubscriptions"; 
                            nInfo.UserId = rdr.GetString(0);
                            nInfo.UserName = rdr.GetString(1);
                            nInfo.UserIco = rdr.GetString(2).Replace(@"~\", "");
                            nInfo.InterDate = rdr.GetDateTime(3).ToString("dd MMMM, yyyy");
                            nInfo.DateForSort = rdr.GetDateTime(3);
                            nInfo.InterName = "has Subscribed to you";
                            ListNotif.Add(nInfo);
                        }
                    rdr.Close();
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("No rows found.");
            }



            //GET NEW WALL COMMENTS NOTIFICATIONS
            SqlCommand cmdWall = new SqlCommand(@"Select TOP 15 anu.id,anu.UserName,anu.Icon,uwp.Date_Posted
                                                from User_Wall_Posts uwp
                                                JOIN AspNetUsers anu on anu.Id = uwp.User_posted
                                                where User_wall = @UID and uwp.User_posted <> @UID", conn);
            cmdWall.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = System.Web.HttpContext.Current.User.Identity.GetUserId();
            try
            {
                using (SqlDataReader rdr = cmdWall.ExecuteReader())
                {
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            NotifStruct nInfo = new NotifStruct();
                            nInfo.Url = "~/Account/Channel_Activity?id=you";
                            nInfo.UserId = rdr.GetString(0);
                            nInfo.UserName = rdr.GetString(1);
                            nInfo.UserIco = rdr.GetString(2).Replace(@"~\", "");
                            nInfo.InterDate = rdr.GetDateTime(3).ToString("dd MMMM, yyyy");
                            nInfo.DateForSort = rdr.GetDateTime(3);
                            nInfo.InterName = " has commented on your wall";
                            ListNotif.Add(nInfo);
                        }
                    rdr.Close();
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("No rows found.");
            }

            ListNotif.Sort(delegate(NotifStruct p1, NotifStruct p2)
            {
                return p2.DateForSort.CompareTo(p1.DateForSort);
            });

            if (ListNotif.Count > 15) 
            ListNotif.RemoveRange(15, ListNotif.Count - 15);
            Repeater RepNotif = LoginNav.FindControl("NotifRepeater") as Repeater;
            RepNotif.DataSource = ListNotif;
            RepNotif.DataBind();
            conn.Close();
            Notif_Span.Text = GetNotifNumber();


        }
        
    }


    protected string GetNotifNumber()
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
                                                and Interaction_date >= (Select NotifCheck  from AspNetUsers where id = @UID))
                                                +
                                                (Select count(*)
                                                from AspNetUsers anu
                                                JOIN Subscriptions subs on subs.User_id = anu.Id
                                                where Fallowing_id = @UID 
                                                and subs.Date_added >= (Select NotifCheck  from AspNetUsers where id = @UID))
                                                 + 
                                                (Select COUNT(*)
                                                 from User_Wall_Posts uwp
                                                 JOIN AspNetUsers anu on anu.Id = uwp.User_posted
                                                 where User_wall = @UID and uwp.User_posted <> @UID and uwp.Date_Posted >= (Select NotifCheck  from AspNetUsers where id = @UID))", conn);
        cmd2.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = System.Web.HttpContext.Current.User.Identity.GetUserId();

        int CurNotifNr = (int)cmd2.ExecuteScalar();
        conn.Close();

        if (CurNotifNr == 0)
            return " ";
        else
        return CurNotifNr.ToString();

    }

    protected void BtnNotif_Click(object sender, EventArgs e)
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd1 = new SqlCommand("Update AspNetUsers Set NotifCheck = GETDATE() where id = @UID", conn);
        cmd1.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = System.Web.HttpContext.Current.User.Identity.GetUserId();
        cmd1.ExecuteNonQuery();
        conn.Close();

        Notif_Span.Text = GetNotifNumber();
       // Page.ClientScript.RegisterStartupScript(this.GetType(), "CLicCallMyFunctionkAnch", " simulate(document.getElementById('NotifAnch'), 'click');", true);


    }

    protected void UpdateNotif_Click(object sender, EventArgs e)
    {
        NotificationFetch();
        Notif_Span.Text = GetNotifNumber();
    }
}
