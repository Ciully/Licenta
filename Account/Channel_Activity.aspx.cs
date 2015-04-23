using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.AspNet.Identity;
using ProiectLicenta;


public partial class Account_Channel_Activity : System.Web.UI.Page
{

    string Channel_id;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.Params["id"].ToString() == "you")
                Channel_id = User.Identity.GetUserId();
            else
                Channel_id = Request.Params["id"].ToString();
            Session["Channel_owner"] = Channel_id;

            huome.HRef = "~/Account/Channel?id=" + Channel_id;
            vdeos.HRef = "~/Account/Channel_videos?id=" + Channel_id;
            Pleleste.HRef = "~/Account/Channel_lists?id=" + Channel_id;
            abut.HRef = "~/Account/Channel_Info?id=" + Channel_id;
            activete.HRef = "~/Account/Channel_Activity?id=" + Channel_id;

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("Select Icon from AspNetUsers where id = @UID", conn);
            cmd.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = Channel_id;
            ProfilePic.ImageUrl = (string)cmd.ExecuteScalar();
            conn.Close();
            Activity_ChannelClick();
        }
    }


    public class Comment
    {
        public int Id { get; set; }
        public string Post_Text { get; set; }
        public string User_posted { get; set; }
        public string User_wall  { get; set; }
        public string date_posted { get; set; }
        public string icon_path { get; set; } 
        public string UserName  { get; set; }

        //public Comment() { }
    }

    protected void Activity_ChannelClick()
    {
        if (Request.Params["id"].ToString() == "you")
            Channel_id = User.Identity.GetUserId();
        else
            Channel_id = Request.Params["id"].ToString();

        List<Comment> list = new List<Comment>();

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("select uwp.Id,Post_text,User_posted,User_wall,date_posted,UserName,Icon from User_wall_posts uwp join AspNetUsers anu on anu.id = uwp.User_posted where User_wall = @usrId ORDER BY date_posted DESC", conn);
        cmd.Parameters.Add(new SqlParameter("@usrId", TypeCode.String)).Value = Channel_id;

        try
        {
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        Comment comm = new Comment();
                        comm.Id = rdr.GetInt32(0);
                        comm.Post_Text = rdr.GetString(1);
                        comm.User_posted = rdr.GetString(2);
                        comm.User_wall = rdr.GetString(3);
                        comm.date_posted = rdr.GetDateTime(4).ToString("dd MMMM, yyyy") + ".";
                        comm.UserName = rdr.GetString(5);
                        comm.icon_path = rdr.GetString(6);
                        list.Add(comm);
                    }
                
                rdr.Close();
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("No rows found.");
        }
        Wall_posts.DataSource = list;
        Wall_posts.DataBind();
        conn.Close();
    }

    protected void Post_comment(object sender, EventArgs e)
    {
        if (Request.Params["id"].ToString() == "you")
            Channel_id = User.Identity.GetUserId();
        else
            Channel_id = Request.Params["id"].ToString();

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("insert into User_wall_posts (Post_text,User_posted,User_wall,date_posted) values (@txt,@usrPost,@usrWall,@date) ", conn);

        cmd.Parameters.Add(new SqlParameter("@txt", TypeCode.String)).Value = Main_input.Value;
        cmd.Parameters.Add(new SqlParameter("@UsrPost", TypeCode.String)).Value = User.Identity.GetUserId();
        cmd.Parameters.Add(new SqlParameter("@usrWall", TypeCode.String)).Value = Channel_id;
        cmd.Parameters.Add(new SqlParameter("@date", TypeCode.DateTime)).Value = System.DateTime.Now;
        //cmd.Parameters.Add(new SqlParameter("@replied", TypeCode.Int32)).Value = 0;
        cmd.ExecuteNonQuery();

        string UID = Channel_id;
        LogHub semnal = new LogHub();
        semnal.Send(UID, "notif_check");
        semnal.Send("All", "All_Users");

        conn.Close();
        Main_input.Value = "";


    }

    protected void Refresh_Comments_ServerClick(object sender, EventArgs e)
    {

        //FetchComments();
        Activity_ChannelClick();
        Main_input.Value = "";

    }

    protected void delete_post(object sender, CommandEventArgs e)
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        if (e.CommandName == "Delete")
        {
            SqlCommand cmd = new SqlCommand("Delete from User_wall_posts where id = @pstId", conn);
            cmd.Parameters.Add(new SqlParameter("@pstId", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
            cmd.ExecuteNonQuery();
        }
        conn.Close();
        Activity_ChannelClick();

    }

    public void R2_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
    {
        // This event is raised for the header, the footer, separators, and items.
        // Execute the following logic for Items and Alternating Items.
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            if (((Comment)e.Item.DataItem).User_posted == User.Identity.GetUserId() || User.Identity.GetUserId() == Session["Channel_owner"])
            {
                // ((Label)e.Item.FindControl("RatingLabel")).Text = "<b>***Good***</b>";
                ((Button)e.Item.FindControl("DeleteButton")).Visible = true;
            }
        }
    }  
}