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
using System.IO;

public partial class Account_Channel_lists : System.Web.UI.Page
{
    string Channel_id;
    int vid_update_id;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Params["id"].ToString() == "you")
        {
            Channel_id = User.Identity.GetUserId();
            Session["dlt_button"] = "true";
        }
        else
        {
            Channel_id = Request.Params["id"].ToString();
            Session["dlt_button"] = "false";
        }
        if (!Page.IsPostBack)
        {
            huome.HRef = "~/Account/Channel?id=" + Channel_id;
            vdeos.HRef = "~/Account/Channel_videos?id=" + Channel_id;
            Pleleste.HRef = "~/Account/Channel_lists?id=" + Channel_id;
            abut.HRef = "~/Account/Channel_Info?id=" + Channel_id;
            activete.HRef = "~/Account/Channel_Activity?id=" + Channel_id;
            Session["vid_title"] = "nimic";
            Playlist_ChannelClick();
        }
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Select Icon from AspNetUsers where id = @UID", conn);
        cmd.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = Channel_id;
        ProfilePic.ImageUrl = (string)cmd.ExecuteScalar();
        conn.Close();
    }



    protected void Playlist_ChannelClick()
    {
        if (Request.Params["id"].ToString() == "you")
        {
            Channel_id = User.Identity.GetUserId().ToString() ;
            Session["dlt_button"] = "true";
        }
        else
        {
            Channel_id = Request.Params["id"].ToString();
            Session["dlt_button"] = "false";
        }

        List<Playlist> list = new List<Playlist>();
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Select pl.Id,pl.Name,pl.date_created,ISNULL((Select TOP 1 v.Thumbnail from Videos v join Video_to_Playlist vtp on vtp.video_id = v.id where Playlist_id = pl.Id order by vtp.Date_Added asc), '~/AppStorage/Default_thumb.jpg') from Playlists pl where User_id = @UID order by Date_Created desc;", conn);
        cmd.Parameters.Add(new SqlParameter("@UID", TypeCode.String));
        cmd.Parameters["@UID"].Value = Channel_id;

        try
        {
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                if (rdr.HasRows)
                    while(rdr.Read())
                {
                    Playlist pl1 = new Playlist();
                    pl1.Id = rdr.GetInt32(0);
                    pl1.Title = rdr.GetString(1);
                    //pl1.nrVids = rdr.GetInt32(2);
                    pl1.date_posted = rdr.GetDateTime(2).ToString("dd MMMM, yyyy");
                    pl1.Thumbnail = rdr.GetString(3);
                    list.Add(pl1);
                }
                rdr.Close();

            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("No rows found.");
        }

        list_container.DataSource = list;
        list_container.DataBind();
        conn.Close();

    }

    public class Playlist
    {
        public int Id { get; set; }
        public string Title { get; set; }
        //public int nrVids { get; set; }
        public string date_posted { get; set; }
        public string Thumbnail { get; set; }

        //public Comment() { }
    }
}