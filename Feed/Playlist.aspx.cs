using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;


public partial class Playlist : System.Web.UI.Page
{
    string PlayId;
    protected void Page_Load(object sender, EventArgs e)
    {
        PlayId = Request.Params["id"].ToString();
        DeletePlaylist.Visible = false;
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        SqlCommand cmd1;
        if (PlayId == "Liked")
        {
            Button1.Visible = false;
            PlayListInfo.Visible = false;
            NonPlaylistList();
        }
        else
        {

            if (PlayId == "History")
            {
                cmd1 = new SqlCommand("select TOP 100 v.id,v.title,v.description,v.views,v.date_uploaded,v.thumbnail,vtp.Date_Added,v.user_id,anu.username,pl.id from videos v join AspNetUsers anu on anu.Id = v.User_id join Video_to_Playlist vtp on vtp.Video_id = v.Id join Playlists pl on pl.Id = vtp.Playlist_id where pl.Name = 'History' and v.User_id = @UID ", conn);
                cmd1.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
            }
            else
            {
                cmd1 = new SqlCommand(@"SELECT v.id,v.title,v.description,v.views,v.date_uploaded,v.thumbnail,vtp.Date_Added,v.user_id,anu.username,vtp.Playlist_id
                                        from Videos v 
                                        join Video_to_Playlist vtp on vtp.Video_id = v.Id 
                                        join AspNetUsers anu on anu.id = v.user_id 
                                        where vtp.Playlist_id = @PlyId;", conn);
                cmd1.Parameters.Add(new SqlParameter("@PlyId", TypeCode.Int32)).Value = Convert.ToInt32(PlayId);
            }

            List<Videos> List = new List<Videos>();
            int index = 0;
            try
            {
                using (SqlDataReader rdr = cmd1.ExecuteReader())
                {
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            index++;
                            if (index == 1)
                            {
                                int firstvid = rdr.GetInt32(0);
                                Session["First_vid_id"] = firstvid;
                                int playlistid = rdr.GetInt32(9);
                                Session["PlaylistId"] = playlistid;
                            }
                            Videos vid = new Videos();
                            vid.Index = index;
                            vid.Id = rdr.GetInt32(0);
                            vid.UserId = rdr.GetString(7);
                            vid.Title = rdr.GetString(1);
                            vid.Description = rdr.GetString(2) + " ";
                            vid.Views = rdr.GetInt32(3);
                            vid.date_posted = rdr.GetDateTime(4).ToString("dd MMMM, yyyy");
                            vid.Thumbnail = rdr.GetString(5);
                            vid.UserName = rdr.GetString(8);
                            vid.Date_added_to_playlist = rdr.GetDateTime(6).ToString("dd MMMM, yyyy");
                            vid.PlayId = rdr.GetInt32(9);
                            List.Add(vid);
                        }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                    rdr.Close();
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("No rows found.");
            }
            VIdPlayRep.DataSource = List;
            VIdPlayRep.DataBind();
            PlaylistInfo();
        }
        conn.Close();

    }

    protected void NonPlaylistList()
    {
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        SqlCommand cmd1 = new SqlCommand(@"Select TOP 100 v.id,v.title,v.description,v.views,v.date_uploaded,v.thumbnail,asd.IDT,v.user_id,anu.username 
                                        From videos v,(Select st.video_id VID,st.Interaction_date IDT 
                                                      from VideoStatistics st join interactions i on i.id = st.interaction_type where i.id = 1 and st.User_id = @UID) asd ,
                                        AspNetUsers anu   
                                        where v.id = asd.VID and  anu.id = v.user_id", conn);
        cmd1.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
        List<Videos> List = new List<Videos>();
        int index = 0;
        try
        {
            using (SqlDataReader rdr = cmd1.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        index++;
                        if (index == 1)
                        {
                            int firstvid = rdr.GetInt32(0);
                            Session["First_vid_id"] = firstvid;
                        }
                        Videos vid = new Videos();
                        vid.Index = index;
                        vid.Id = rdr.GetInt32(0);
                        vid.UserId = rdr.GetString(7);
                        vid.Title = rdr.GetString(1);
                        vid.Description = rdr.GetString(2) + " ";
                        vid.Views = rdr.GetInt32(3);
                        vid.date_posted = rdr.GetDateTime(4).ToString("dd MMMM, yyyy");
                        vid.Thumbnail = rdr.GetString(5);
                        vid.UserName = rdr.GetString(8);
                        vid.Date_added_to_playlist = rdr.GetDateTime(6).ToString("dd MMMM, yyyy");
                        vid.PlayId = 0;
                        List.Add(vid);
                    }
                rdr.Close();
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("No rows found.");
        }
        NonPlaylistRepeater.DataSource = List;
        NonPlaylistRepeater.DataBind();
        conn.Close();
    }

    protected void PlaylistInfo()
    {
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        int plid = 0;
        if (Request.Params["id"].ToString() == "History")
        {
            SqlCommand cmdHist = new SqlCommand("Select id from Playlists where user_id = '" + User.Identity.GetUserId() + "' and name = 'History'", conn);
            plid = (int)cmdHist.ExecuteScalar();
        }
        else
            plid = Convert.ToInt32(Request.Params["id"].ToString());

        SqlCommand cmd = new SqlCommand(@"Select pl.id,pl.name,pl.User_id,pl.date_created ,anu.UserName,
                                        (Select count(*) from Video_to_Playlist vtp where vtp.Playlist_id = pl.id) nr,
                                        (Select top 1 Thumbnail from videos vids JOIN Video_to_Playlist vtp on vtp.Video_id = vids.id where vtp.Playlist_id = pl.id order by vtp.Date_Added) thumb
                                        from Playlists pl
                                        JOIN AspNetUsers anu on anu.id = pl.user_id
                                        where pl.id = @PLID", conn);
        cmd.Parameters.Add(new SqlParameter("@PLID", TypeCode.Int32)).Value = plid;
        string UID = "";
        try
        {
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        PlaylistTitle.Text = rdr.GetString(1);
                        UID = rdr.GetString(2);
                        UserLink.HRef = "~/Account/Channel?id=" + rdr.GetString(2);
                        UserName.Text = rdr.GetString(4);
                        DateCreated.Text = rdr.GetDateTime(3).ToString("dd MMMM, yyyy");
                        NrVids.Text = rdr.GetInt32(5).ToString();
                        PlayThumb.Src = rdr.GetString(6);
                    }
                rdr.Close();
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("No rows found.");
        }
        catch (Exception e)
        {
            Console.WriteLine("No rows found.");
        }

        if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
        {
            if (User.Identity.GetUserId() == UID)
            {
                DeletePlaylist.Visible = true;
            }
        }
        if (PlaylistTitle.Text == "History" || PlaylistTitle.Text == "Favorites")
        {
            //PlayListInfo.Visible = false;
            DeletePlaylist.Visible = false;

        }
        conn.Close();
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/watch?id=" + Session["First_vid_id"] + "&index=1" + "&list=" + Session["PlaylistId"]);
    }

    public class Videos
    {
        public int PlayId { get; set; }
        public int Index { get; set; }
        public int Id { get; set; }
        public int Views { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string date_posted { get; set; }
        public string Thumbnail { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Date_added_to_playlist { get; set; }


        //public string Status { get; set; }

        //public Comment() { }
    }
    protected void DeletePlaylist_Click(object sender, EventArgs e)
    {
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmdDel = new SqlCommand("Delete From Playlists where id = @PLID", conn);
        cmdDel.Parameters.Add(new SqlParameter("@PLID", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
        cmdDel.ExecuteNonQuery();
        conn.Close();
        Response.Redirect("~/Account/Channel_lists?id=you");

    }
}