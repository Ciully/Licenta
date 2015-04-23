using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using System.Web.Services;
using System.Web.UI.DataVisualization.Charting;
using ProiectLicenta;
using System.Data;


public partial class watch : System.Web.UI.Page
{
    int id;
    string proprietary_user;

    protected void Page_Load(object sender, EventArgs e)
    {


        if (!IsPostBack)
        {
            SqlConnection conn;
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();
            if (Request.Params["id"] != null)
                id = Convert.ToInt32(Request.Params["id"].ToString());
            Session["VideoId"] = id;
            SqlCommand cmd = new SqlCommand("Select path from videos where id = @VidId", conn);
            cmd.Parameters.Add(new SqlParameter("@VidId", TypeCode.Int32));
            cmd.Parameters["@VidId"].Value = id;

            string Vid_path = (string)cmd.ExecuteScalar();

            cmd = new SqlCommand("Select status from videos where id = @VidId", conn);
            cmd.Parameters.Add(new SqlParameter("@VidId", TypeCode.Int32)).Value = id;

            string status = (string)cmd.ExecuteScalar();

            cmd = new SqlCommand("Select User_id from videos where id = @VidId", conn);
            cmd.Parameters.Add(new SqlParameter("@VidId", TypeCode.Int32)).Value = id;

            bool acces = true;

            if (User.Identity.GetUserId() == (string)cmd.ExecuteScalar())
            {
                if (!status.Equals("Deleted"))
                    Video_Player_Source.Src = "~/" + Vid_path;
                else
                {
                    video_player.Src = "NO acces";
                    acces = false;
                }
            }
            else
            {
                if (status == "Public" || status == "Unlisted")
                    Video_Player_Source.Src = "~/" + Vid_path;
                else
                {
                    video_player.Src = "No Acces";
                    acces = false;
                }
            }


            if (acces)
            {
                cmd = new SqlCommand("Select title,vds.description,views,likes,dislikes,Date_Uploaded,User_id,Tags,Icon from Videos vds join AspNetUsers anu on anu.id = vds.User_id where vds.id = @VidId ", conn);
                cmd.Parameters.Add(new SqlParameter("@VidId", TypeCode.Int32));
                cmd.Parameters["@VidId"].Value = id;
                string uid = "";
                try
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            Video_title.Text = rdr.GetString(0);
                            Short_description.Text = rdr.GetString(1);
                            View_count.Text = Convert.ToString(rdr.GetInt32(2)) + " Views";
                            upvote.Text = Convert.ToString(rdr.GetInt32(3));
                            downvote.Text = Convert.ToString(rdr.GetInt32(4));
                            Date.Text = "Published on " + rdr.GetDateTime(5).ToString("dd MMMM , yyyy");
                            uid = rdr.GetString(6);
                            proprietary_user = rdr.GetString(6);
                            HyperLink1.HRef = "~/Account/Channel?id=" + proprietary_user;
                            ImageButton1.Src = rdr.GetString(8);
                            Session["Vid_owner"] = proprietary_user;
                            vidTags.Text = rdr.GetString(7) + " ";
                            vidTags.Text = vidTags.Text.Replace("/", " ");
                            if (rdr.GetInt32(3) + rdr.GetInt32(4) > 0)
                            {
                                int likes_perc = (rdr.GetInt32(3) * 100) / (rdr.GetInt32(3) + rdr.GetInt32(4));
                                likes_progress.Style.Add("width", Convert.ToString(likes_perc) + "%;");
                                dislikes_progress.Style.Add("width", Convert.ToString(100 - likes_perc) + "%;");
                            }
                        }
                        rdr.Close();
                    }
                    cmd = new SqlCommand("Select UserName from AspNetUsers where id = @UID", conn);
                    cmd.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = proprietary_user;
                    HyperLink1.InnerText = (string)cmd.ExecuteScalar();
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("No rows found.");
                }


                //ViewCount Increment & History add
                SqlCommand cmdAddView = new SqlCommand("Update Videos set views = views + 1 where id = @VID", conn);
                cmdAddView.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = id;
                cmdAddView.ExecuteNonQuery();

                if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    int lastVid;
                    try
                    {
                        SqlCommand cmdLastHist = new SqlCommand("Select TOP 1 Video_id from Video_to_Playlist where Playlist_id = (Select id from Playlists where Name = 'History' and User_id = @UID) order by Date_Added desc;", conn);
                        cmdLastHist.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
                        lastVid = (int)cmdLastHist.ExecuteScalar();
                    }
                    catch (Exception)
                    {
                        lastVid = -1;
                    }

                    if (lastVid != id)
                    {
                        SqlCommand cmdInsertStats = new SqlCommand("INSERT INTO VideoStatistics (Video_id,User_id,interaction_date,interaction_type) VALUES(@VID,@UID,@CurDate,@action)", conn);
                        cmdInsertStats.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
                        cmdInsertStats.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
                        cmdInsertStats.Parameters.Add(new SqlParameter("@CurDate", TypeCode.DateTime)).Value = System.DateTime.Now;
                        cmdInsertStats.Parameters.Add(new SqlParameter("@action", TypeCode.Int32)).Value = 5;
                        cmdInsertStats.ExecuteNonQuery();

                        SqlCommand cmdAddHistory = new SqlCommand("Insert into video_to_playlist (video_id,Playlist_id,Date_Added) values(@VID,(Select id from Playlists where Name = 'History' and User_id = @UID), SYSDATETIME());", conn);
                        cmdAddHistory.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = id;
                        cmdAddHistory.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
                        cmdAddHistory.ExecuteNonQuery();
                    }
                }

                if (Request.Params["list"] != null)
                    PopulatePlaylist();
                //Subscription Action
                if (User.Identity.GetUserId() == proprietary_user || User.Identity.IsAuthenticated == false)
                    Sub_Button.Visible = false;
                if (User.Identity.IsAuthenticated == false)
                {
                    Main_input.Visible = false;
                    Add_Comment_Button.Visible = false;
                    ProgressLikeSection.Visible = false;
                }
                else
                    if (User.Identity.IsAuthenticated)
                    {
                        SqlCommand cmd_subs = new SqlCommand("Select count(*) FROM Subscriptions where User_id = @curentUID and Fallowing_id = @vidPropId", conn);
                        cmd_subs.Parameters.Add(new SqlParameter("@curentUID", TypeCode.String)).Value = User.Identity.GetUserId();
                        cmd_subs.Parameters.Add(new SqlParameter("@vidPropId", TypeCode.String)).Value = proprietary_user;

                        if ((int)cmd_subs.ExecuteScalar() > 0)
                        {
                            Sub_Button.Text = "Unsubscribe";
                            Sub_Button.CssClass += " close";
                        }
                    }

                conn.Close();
                GetPlaylists();
                Apreciation_buttons_visibility();
                Recomanded_list();
                Chart3_Load();
                Master.FindControl("AdminVidDelete").Visible = true;
                FetchComments();

            }
        }

    }

    public void FetchComments()
    {
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        List<Comment> list = new List<Comment>();
        SqlCommand cmd;
        cmd = new SqlCommand("Select cmm.id,User_id,comment_text,date_posted,comment_upvote,comment_downvote,anu.id,UserName,icon FROM Comments cmm JOIN AspNetUsers anu on anu.id = cmm.User_id Where video_id = @VidId ORDER BY date_posted DESC", conn);
        cmd.Parameters.Add(new SqlParameter("@VidId", TypeCode.Int32));
        cmd.Parameters["@VidId"].Value = Session["VideoId"];
        try
        {
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        Comment comm = new Comment();
                        comm.Id = rdr.GetInt32(0);
                        comm.Post_Text = rdr.GetString(2);
                        comm.User_posted = rdr.GetString(6);
                        comm.date_posted = rdr.GetDateTime(3).ToString("dd MMMM, yyyy") + ".";
                        comm.like = rdr.GetInt32(4);
                        comm.dislike = rdr.GetInt32(5);
                        comm.UserName = rdr.GetString(7);
                        comm.icon_path = rdr.GetString(8);
                        list.Add(comm);
                    }

                rdr.Close();
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("No rows found.");
        }
        conn.Close();
        DataTable dt = new DataTable();
        Wall_posts.DataSource = dt;
        Wall_posts.DataBind();
        Wall_posts.DataSource = list;
        Wall_posts.DataBind();

    }

    private void PopulatePlaylist()
    {
        Playlist_Contet.Visible = true;
        string plId = Request.Params["list"].ToString();
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        SqlCommand cmdPlaylist = new SqlCommand(@"Select vids.id,vids.title,vids.description,vids.views,vids.date_uploaded,vids.thumbnail,vids.user_id,anu.username from videos vids
                                                JOIN AspNetUsers anu on anu.Id = vids.User_id
												JOIN Video_to_playlist vtp on vtp.Video_id = vids.id
                                                JOIN Playlists pl on pl.id = vtp.Playlist_id
                                                where pl.id  = @PLID", conn);
        cmdPlaylist.Parameters.Add(new SqlParameter("@PLID", TypeCode.Int32)).Value = Convert.ToInt32(plId);

        List<Videos> VidList = new List<Videos>();
        int index = 0;
        try
        {
            using (SqlDataReader rdr = cmdPlaylist.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        index++;
                        Videos vidInf = new Videos();
                        vidInf.Index = index;
                        vidInf.Id = rdr.GetInt32(0);
                        vidInf.UserId = rdr.GetString(6);
                        vidInf.Title = rdr.GetString(1);
                        vidInf.Views = rdr.GetInt32(3);
                        vidInf.date_posted = rdr.GetDateTime(4).ToString("dd MMMM, yyyy");
                        vidInf.Thumbnail = rdr.GetString(5);
                        vidInf.UserName = rdr.GetString(7);
                        VidList.Add(vidInf);
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
        PlaylistVidRepeater.DataSource = VidList;
        PlaylistVidRepeater.DataBind();
        conn.Close();
    }

    protected void Apreciation_buttons_visibility()
    {
        if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
        {
            int vid_id = Convert.ToInt32(Request.Params["id"].ToString());
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();

            SqlCommand cmd0 = new SqlCommand("Select count(*) from VideoStatistics where User_id = @usr_id and VIdeo_id = @vid_id and interaction_type = 1", conn);
            cmd0.Parameters.Add(new SqlParameter("@usr_id", TypeCode.String)).Value = User.Identity.GetUserId();
            cmd0.Parameters.Add(new SqlParameter("@vid_id", TypeCode.Int32)).Value = vid_id;
            if (Convert.ToInt32(cmd0.ExecuteScalar()) > 0)
            {
                btn_like_ok.Visible = true;
                btn_like_neg.Visible = false;
            }
            else
            {
                btn_like_ok.Visible = false;
                btn_like_neg.Visible = true;
            }
            SqlCommand cmd1 = new SqlCommand("Select count(*) from VideoStatistics where User_id = @usr_id and VIdeo_id = @vid_id and interaction_type = 2", conn);
            cmd1.Parameters.Add(new SqlParameter("@usr_id", TypeCode.String)).Value = User.Identity.GetUserId();
            cmd1.Parameters.Add(new SqlParameter("@vid_id", TypeCode.Int32)).Value = vid_id;

            if (Convert.ToInt32(cmd1.ExecuteScalar()) > 0)
            {
                btn_dislike_ok.Visible = true;
                btn_dislike_neg.Visible = false;
            }
            else
            {
                btn_dislike_ok.Visible = false;
                btn_dislike_neg.Visible = true;
            }

            conn.Close();
        }
    }

    protected void Increment_like_video(object sender, EventArgs e)
    {
        int vid_id = Convert.ToInt32(Request.Params["id"].ToString());
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        SqlCommand cmd0 = new SqlCommand("Select count(*) from VideoStatistics where User_id = @usr_id and VIdeo_id = @vid_id and interaction_type = 1", conn);
        cmd0.Parameters.Add(new SqlParameter("@usr_id", TypeCode.String)).Value = User.Identity.GetUserId();
        cmd0.Parameters.Add(new SqlParameter("@vid_id", TypeCode.Int32)).Value = vid_id;
        if (Convert.ToInt32(cmd0.ExecuteScalar()) > 0)
        {
            SqlCommand cmdDelStats = new SqlCommand("DELETE from VideoStatistics where User_id = @usr_id and VIdeo_id = @vid_id and interaction_type = 1", conn);
            cmdDelStats.Parameters.Add(new SqlParameter("@usr_id", TypeCode.String)).Value = User.Identity.GetUserId();
            cmdDelStats.Parameters.Add(new SqlParameter("@vid_id", TypeCode.Int32)).Value = vid_id;
            cmdDelStats.ExecuteNonQuery();

            SqlCommand cmd = new SqlCommand("Update Videos set likes = likes - 1 where id = @vidId", conn);
            cmd.Parameters.Add(new SqlParameter("@vidId", TypeCode.Int32)).Value = vid_id;
            cmd.ExecuteNonQuery();
        }
        else
        {
            SqlCommand cmdAddStats = new SqlCommand("Insert  into VideoStatistics (video_id,user_id,interaction_date,interaction_type) VALUES(@vId,@uId,@intDt,@intTp)", conn);
            cmdAddStats.Parameters.Add(new SqlParameter("@uId", TypeCode.String)).Value = User.Identity.GetUserId();
            cmdAddStats.Parameters.Add(new SqlParameter("@vId", TypeCode.Int32)).Value = vid_id;
            cmdAddStats.Parameters.Add(new SqlParameter("@intDt", TypeCode.DateTime)).Value = System.DateTime.Now;
            cmdAddStats.Parameters.Add(new SqlParameter("@intTp", TypeCode.Int32)).Value = 1;
            cmdAddStats.ExecuteNonQuery();

            SqlCommand cmd = new SqlCommand("Update Videos set likes = likes + 1 where id = @vidId", conn);
            cmd.Parameters.Add(new SqlParameter("@vidId", TypeCode.Int32)).Value = vid_id;
            cmd.ExecuteNonQuery();

            SqlCommand cmd02 = new SqlCommand("Select count(*) from VideoStatistics where User_id = @usr_id and VIdeo_id = @vid_id and interaction_type = 2", conn);
            cmd02.Parameters.Add(new SqlParameter("@usr_id", TypeCode.String)).Value = User.Identity.GetUserId();
            cmd02.Parameters.Add(new SqlParameter("@vid_id", TypeCode.Int32)).Value = vid_id;

            if (Convert.ToInt32(cmd02.ExecuteScalar()) > 0)
            {
                SqlCommand cmdDel = new SqlCommand("DELETE from VideoStatistics where User_id = @usr_id and VIdeo_id = @vid_id and interaction_type = 2", conn);
                cmdDel.Parameters.Add(new SqlParameter("@usr_id", TypeCode.String)).Value = User.Identity.GetUserId();
                cmdDel.Parameters.Add(new SqlParameter("@vid_id", TypeCode.Int32)).Value = vid_id;
                cmdDel.ExecuteNonQuery();

                SqlCommand cmd2 = new SqlCommand("Update Videos set dislikes = dislikes - 1 where id = @vidId", conn);
                cmd2.Parameters.Add(new SqlParameter("@vidId", TypeCode.Int32)).Value = vid_id;
                cmd2.ExecuteNonQuery();
            }
        }

        SqlCommand cmdUserId = new SqlCommand("Select anu.Id from AspNetUsers anu JOIN Videos vids on vids.User_id = anu.id where vids.id = @VID", conn);
        cmdUserId.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
        string UID = (string)cmdUserId.ExecuteScalar();
        LogHub semnal = new LogHub();
        semnal.Send(UID, "notif_check");

        conn.Close();
        Apreciation_buttons_visibility();
    }

    protected void Increment_dislike_video(object sender, EventArgs e)
    {
        int vid_id = Convert.ToInt32(Request.Params["id"].ToString());

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        SqlCommand cmd0 = new SqlCommand("Select count(*) from VideoStatistics where User_id = @usr_id and VIdeo_id = @vid_id and interaction_type = 2", conn);
        cmd0.Parameters.Add(new SqlParameter("@usr_id", TypeCode.String)).Value = User.Identity.GetUserId();
        cmd0.Parameters.Add(new SqlParameter("@vid_id", TypeCode.Int32)).Value = vid_id;
        if (Convert.ToInt32(cmd0.ExecuteScalar()) > 0)
        {
            SqlCommand cmdDelStats = new SqlCommand("DELETE from VideoStatistics where User_id = @usr_id and VIdeo_id = @vid_id and interaction_type = 2", conn);
            cmdDelStats.Parameters.Add(new SqlParameter("@usr_id", TypeCode.String)).Value = User.Identity.GetUserId();
            cmdDelStats.Parameters.Add(new SqlParameter("@vid_id", TypeCode.Int32)).Value = vid_id;
            cmdDelStats.ExecuteNonQuery();

            SqlCommand cmd = new SqlCommand("Update Videos set dislikes = dislikes - 1 where id = @vidId", conn);
            cmd.Parameters.Add(new SqlParameter("@vidId", TypeCode.Int32)).Value = vid_id;
            cmd.ExecuteNonQuery();
        }
        else
        {
            SqlCommand cmdAddStats = new SqlCommand("Insert  into VideoStatistics (video_id,user_id,interaction_date,interaction_type) VALUES(@vId,@uId,@intDt,@intTp)", conn);
            cmdAddStats.Parameters.Add(new SqlParameter("@uId", TypeCode.String)).Value = User.Identity.GetUserId();
            cmdAddStats.Parameters.Add(new SqlParameter("@vId", TypeCode.Int32)).Value = vid_id;
            cmdAddStats.Parameters.Add(new SqlParameter("@intDt", TypeCode.DateTime)).Value = System.DateTime.Now;
            cmdAddStats.Parameters.Add(new SqlParameter("@intTp", TypeCode.Int32)).Value = 2;
            cmdAddStats.ExecuteNonQuery();

            SqlCommand cmd = new SqlCommand("Update Videos set dislikes = dislikes + 1 where id = @vidId", conn);
            cmd.Parameters.Add(new SqlParameter("@vidId", TypeCode.Int32)).Value = vid_id;
            cmd.ExecuteNonQuery();

            SqlCommand cmdCountStats = new SqlCommand("Select count(*) from VideoStatistics where User_id = @usr_id and VIdeo_id = @vid_id and interaction_type = 1", conn);
            cmdCountStats.Parameters.Add(new SqlParameter("@usr_id", TypeCode.String)).Value = User.Identity.GetUserId();
            cmdCountStats.Parameters.Add(new SqlParameter("@vid_id", TypeCode.Int32)).Value = vid_id;

            if (Convert.ToInt32(cmdCountStats.ExecuteScalar()) > 0)
            {
                SqlCommand cmdDelStats = new SqlCommand("DELETE from VideoStatistics where User_id = @usr_id and VIdeo_id = @vid_id and interaction_type = 1", conn);
                cmdDelStats.Parameters.Add(new SqlParameter("@usr_id", TypeCode.String)).Value = User.Identity.GetUserId();
                cmdDelStats.Parameters.Add(new SqlParameter("@vid_id", TypeCode.Int32)).Value = vid_id;
                cmdDelStats.ExecuteNonQuery();

                SqlCommand cmd1 = new SqlCommand("Update Videos set likes = likes - 1 where id = @vidId", conn);
                cmd1.Parameters.Add(new SqlParameter("@vidId", TypeCode.Int32)).Value = vid_id;
                cmd1.ExecuteNonQuery();
            }

        }
        conn.Close();

    }

    protected void Increment_like_Post(Object sender, CommandEventArgs e)
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        if (e.CommandName == "Like")
        {
            SqlCommand cmdCountLikes = new SqlCommand("select count(*) from CommentStats where user_id = @UID and comment_id = @CID and interaction_type = 1", conn);
            cmdCountLikes.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
            cmdCountLikes.Parameters.Add(new SqlParameter("@CID", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());

            if (Convert.ToInt32(cmdCountLikes.ExecuteScalar()) > 0)
            {
                SqlCommand cmdDeleteStats = new SqlCommand("Delete from CommentStats where user_id = @UID and comment_id = @CID and interaction_type = 1;", conn);
                cmdDeleteStats.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
                cmdDeleteStats.Parameters.Add(new SqlParameter("@CID", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
                cmdDeleteStats.ExecuteNonQuery();

                SqlCommand cmd = new SqlCommand("Update Comments set Comment_upvote = Comment_upvote - 1 where id = @commId", conn);
                cmd.Parameters.Add(new SqlParameter("@commId", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
                cmd.ExecuteNonQuery();
            }
            else
            {
                SqlCommand cmdAddStats = new SqlCommand("Insert into CommentStats (User_id,Comment_id,Interaction_type,date_added) Values(@UID,@CID,1,SYSDATETIME());", conn);
                cmdAddStats.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
                cmdAddStats.Parameters.Add(new SqlParameter("@CID", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
                cmdAddStats.ExecuteNonQuery();

                SqlCommand cmd = new SqlCommand("Update Comments set Comment_upvote = Comment_upvote + 1 where id = @commId", conn);
                cmd.Parameters.Add(new SqlParameter("@commId", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
                cmd.ExecuteNonQuery();

                SqlCommand cmdCountDislikes = new SqlCommand("select count(*) from CommentStats where user_id = @UID and comment_id = @CID and interaction_type = 2;", conn);
                cmdCountDislikes.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
                cmdCountDislikes.Parameters.Add(new SqlParameter("@CID", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());

                if (Convert.ToInt32(cmdCountDislikes.ExecuteScalar()) > 0)
                {
                    SqlCommand cmdDeleteStats = new SqlCommand("Delete from CommentStats where user_id = @UID and comment_id = @CID and interaction_type = 2;", conn);
                    cmdDeleteStats.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
                    cmdDeleteStats.Parameters.Add(new SqlParameter("@CID", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
                    cmdDeleteStats.ExecuteNonQuery();

                    SqlCommand cmd1 = new SqlCommand("Update Comments set Comment_downvote = Comment_downvote - 1 where id = @commId", conn);
                    cmd1.Parameters.Add(new SqlParameter("@commId", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
                    cmd1.ExecuteNonQuery();
                }

            }

        }
        if (e.CommandName == "Dislike")
        {
            SqlCommand cmdCountLikes = new SqlCommand("select count(*) from CommentStats where user_id = @UID and comment_id = @CID and interaction_type = 2", conn);
            cmdCountLikes.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
            cmdCountLikes.Parameters.Add(new SqlParameter("@CID", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());

            if (Convert.ToInt32(cmdCountLikes.ExecuteScalar()) > 0)
            {
                SqlCommand cmdDeleteStats = new SqlCommand("Delete from CommentStats where user_id = @UID and comment_id = @CID and interaction_type = 2;", conn);
                cmdDeleteStats.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
                cmdDeleteStats.Parameters.Add(new SqlParameter("@CID", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
                cmdDeleteStats.ExecuteNonQuery();

                SqlCommand cmd = new SqlCommand("Update Comments set Comment_downvote = Comment_downvote - 1 where id = @commId", conn);
                cmd.Parameters.Add(new SqlParameter("@commId", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
                cmd.ExecuteNonQuery();
            }
            else
            {
                SqlCommand cmdAddStats = new SqlCommand("Insert into CommentStats (User_id,Comment_id,Interaction_type,date_added) Values(@UID,@CID,1,SYSDATETIME());", conn);
                cmdAddStats.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
                cmdAddStats.Parameters.Add(new SqlParameter("@CID", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
                cmdAddStats.ExecuteNonQuery();

                SqlCommand cmd = new SqlCommand("Update Comments set Comment_downvote = Comment_downvote + 1 where id = @commId", conn);
                cmd.Parameters.Add(new SqlParameter("@commId", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
                cmd.ExecuteNonQuery();

                SqlCommand cmdCountDislikes = new SqlCommand("select count(*) from CommentStats where user_id = @UID and comment_id = @CID and interaction_type = 1;", conn);
                cmdCountDislikes.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
                cmdCountDislikes.Parameters.Add(new SqlParameter("@CID", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());

                if (Convert.ToInt32(cmdCountDislikes.ExecuteScalar()) > 0)
                {
                    SqlCommand cmdDeleteStats = new SqlCommand("Delete from CommentStats where user_id = @UID and comment_id = @CID and interaction_type = 1;", conn);
                    cmdDeleteStats.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
                    cmdDeleteStats.Parameters.Add(new SqlParameter("@CID", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
                    cmdDeleteStats.ExecuteNonQuery();

                    SqlCommand cmd1 = new SqlCommand("Update Comments set Comment_upvote = Comment_upvote - 1 where id = @commId", conn);
                    cmd1.Parameters.Add(new SqlParameter("@commId", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
                    cmd1.ExecuteNonQuery();
                }

            }
        }


        conn.Close();

    }

    //Like Comment adauga celelealte cazuri (remove like/dislike)
    protected void Post_comment(object sender, EventArgs e)
    {

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("INSERT INTO Comments (User_id,Video_id,comment_text,date_posted) VALUES (@usrId,@vidId,@commTxt,@date) ", conn);
        DateTime Curt = System.DateTime.Now;
        cmd.Parameters.Add(new SqlParameter("@usrId", TypeCode.String)).Value = User.Identity.GetUserId();
        cmd.Parameters.Add(new SqlParameter("@vidId", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
        cmd.Parameters.Add(new SqlParameter("@commTxt", TypeCode.String)).Value = Main_input.Value;
        cmd.Parameters.Add(new SqlParameter("@date", TypeCode.DateTime)).Value = Curt;
        //cmd.Parameters.Add(new SqlParameter("@replied", TypeCode.Int32)).Value = 0;
        cmd.ExecuteNonQuery();
        SqlCommand cmdInsertStats = new SqlCommand("INSERT INTO VideoStatistics (Video_id,User_id,interaction_date,interaction_type) VALUES(@VID,@UID,@CurDate,@action)", conn);
        cmdInsertStats.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
        cmdInsertStats.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
        cmdInsertStats.Parameters.Add(new SqlParameter("@CurDate", TypeCode.DateTime)).Value = Curt;
        cmdInsertStats.Parameters.Add(new SqlParameter("@action", TypeCode.Int32)).Value = 6;
        cmdInsertStats.ExecuteNonQuery();

        SqlCommand cmdUserId = new SqlCommand("Select anu.Id from AspNetUsers anu JOIN Videos vids on vids.User_id = anu.id where vids.id = @VID",conn);
        cmdUserId.Parameters.Add(new SqlParameter("@VID",TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
        string UID = (string)cmdUserId.ExecuteScalar();
        LogHub semnal = new LogHub();
        semnal.Send(UID,"notif_check");
        semnal.Send("All", "All_Users");

        conn.Close();
        Main_input.Value = "";
        //FetchComments();

    }

    protected void GetPlaylists()
    {
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        // SqlCommand cmdPlaylists = new SqlCommand("Select Name,count(vtp.id) From Playlists pl join Video_to_Playlist vtp ON vtp.Playlist_id = pl.id where User_id = @UID and vtp.Video_id = @VID and pl.Name <> 'History' group by Name;", conn);
        SqlCommand cmdPlaylists = new SqlCommand("Select pl.id,pl.Name,(Select count(*) nr from Video_to_Playlist vtp where vtp.video_id = 4 and vtp.playlist_id = pl.id) asd from Playlists pl where pl.User_id = @UID and pl.name <> 'History' ; ", conn);
        cmdPlaylists.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = Session["Vid_owner"];
        // cmdPlaylists.Parameters.Add(new SqlParameter("@VID", TypeCode.String)).Value = Session["Vid_owner"];

        List<PlaylistInfo> PlList = new List<PlaylistInfo>();

        try
        {
            using (SqlDataReader rdr = cmdPlaylists.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        PlaylistInfo TempList = new PlaylistInfo();
                        TempList.Id = rdr.GetInt32(0);
                        TempList.Name = rdr.GetString(1);
                        TempList.Nr = rdr.GetInt32(2);
                        PlList.Add(TempList);

                    }
                rdr.Close();
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("No rows found.");
        }
        PlaylistRepeater.DataSource = PlList;
        PlaylistRepeater.DataBind();
        conn.Close();
    }

    protected void delete_post(object sender, CommandEventArgs e)
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        if (e.CommandName == "Delete")
        {
            int vid = -1;
            string uid = "";
            DateTime dtdel = System.DateTime.Now;
            SqlCommand cmdselect = new SqlCommand("Select Video_id,User_id,date_posted from Comments where id = @ID", conn);
            cmdselect.Parameters.Add(new SqlParameter("@ID", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
            using (SqlDataReader rdr = cmdselect.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        vid = rdr.GetInt32(0);
                        uid = rdr.GetString(1);
                        dtdel = rdr.GetDateTime(2);
                    }
            }
            SqlCommand cmd = new SqlCommand("Delete from Comments where id = @pstId", conn);
            cmd.Parameters.Add(new SqlParameter("@pstId", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
            cmd.ExecuteNonQuery();
            SqlCommand cmdvs = new SqlCommand("Delete from VideoStatistics where Video_id = @VID and User_id = @UID and Interaction_date = @DATECUR", conn);
            cmdvs.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = vid;
            cmdvs.Parameters.Add(new SqlParameter("@UID", TypeCode.Int32)).Value = uid;
            cmdvs.Parameters.Add(new SqlParameter("@DATECUR", TypeCode.Int32)).Value = dtdel;
            cmdvs.ExecuteNonQuery();
        }
       
        conn.Close();
        FetchComments();


    }

    protected void Subscription_Button_Click(object sender, EventArgs e)
    {
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        SqlCommand cmd_subs = new SqlCommand("Select count(*) FROM Subscriptions where User_id = @curentUID and Fallowing_id = @vidPropId", conn);
        cmd_subs.Parameters.Add(new SqlParameter("@curentUID", TypeCode.String)).Value = User.Identity.GetUserId();
        cmd_subs.Parameters.Add(new SqlParameter("@vidPropId", TypeCode.String)).Value = Session["Vid_owner"];


        if ((int)cmd_subs.ExecuteScalar() > 0)
        {
            SqlCommand cmdUnsub = new SqlCommand("Delete from Subscriptions where User_id = @curentUID and Fallowing_id = @vidPropId", conn);
            cmdUnsub.Parameters.Add(new SqlParameter("@curentUID", TypeCode.String)).Value = User.Identity.GetUserId();
            cmdUnsub.Parameters.Add(new SqlParameter("@vidPropId", TypeCode.String)).Value = Session["Vid_owner"];
            cmdUnsub.ExecuteNonQuery();
            Sub_Button.Text = "Subscribe";
            Sub_Button.CssClass = "btn btn-default ";
        }
        else
        {
            SqlCommand cmdAddSub = new SqlCommand("Insert INTO Subscriptions (User_id,Fallowing_id,date_added) VALUES(@curentUID,@vidPropId,@CurDate)", conn);
            cmdAddSub.Parameters.Add(new SqlParameter("@curentUID", TypeCode.String)).Value = User.Identity.GetUserId();
            cmdAddSub.Parameters.Add(new SqlParameter("@vidPropId", TypeCode.String)).Value = Session["Vid_owner"];
            cmdAddSub.Parameters.Add(new SqlParameter("@CurDate", TypeCode.DateTime)).Value = System.DateTime.Now;
            cmdAddSub.ExecuteNonQuery();
            Sub_Button.CssClass = "btn btn-default close";
            Sub_Button.Text = "Unsubscribe";

        }


        SqlCommand cmdUserId = new SqlCommand("Select anu.Id from AspNetUsers anu JOIN Videos vids on vids.User_id = anu.id where vids.id = @VID", conn);
        cmdUserId.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
        string UID = (string)cmdUserId.ExecuteScalar();
        LogHub semnal = new LogHub();
        semnal.Send(UID, "notif_check");

        conn.Close();
    }

    protected void Add_to_Favorites(object sender, EventArgs e)
    {
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();


        SqlCommand cmd_subs = new SqlCommand("Select (select count(id) from video_to_playlist vtp where vtp.playlist_id = pl.id and vtp.Video_id = @vId) from Playlists pl where pl.User_id = @curentUID and pl.Name = 'Favorites';", conn);
        cmd_subs.Parameters.Add(new SqlParameter("@curentUID", TypeCode.String)).Value = User.Identity.GetUserId();
        // cmd_subs.Parameters.Add(new SqlParameter("@vidPropId", TypeCode.String)).Value = Session["Vid_owner"];
        cmd_subs.Parameters.Add(new SqlParameter("@vId", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());

        if ((int)cmd_subs.ExecuteScalar() > 0)
        {
            SqlCommand cmdPlFav = new SqlCommand("select id from Playlists where user_id = @Uid and Name = 'Favorites'", conn);
            cmdPlFav.Parameters.Add(new SqlParameter("@Uid", TypeCode.String)).Value = User.Identity.GetUserId();
            int PlId = (int)cmdPlFav.ExecuteScalar();

            SqlCommand cmdRemoveFav = new SqlCommand("Delete from Video_to_playlist where Video_id = @vidId and Playlist_id = @PlId", conn);
            cmdRemoveFav.Parameters.Add(new SqlParameter("@vidId", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
            cmdRemoveFav.Parameters.Add(new SqlParameter("@PlId", TypeCode.Int32)).Value = PlId;
            cmdRemoveFav.ExecuteNonQuery();

            SqlCommand cmdDelStats = new SqlCommand("Delete from VideoStatistics where Video_id = @VID and User_id  = @UID", conn);
            cmdDelStats.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
            cmdDelStats.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
            cmdDelStats.ExecuteNonQuery();
        }

        else
        {
            SqlCommand cmdPlFav = new SqlCommand("select id from Playlists where user_id = @Uid and Name = 'Favorites'", conn);
            cmdPlFav.Parameters.Add(new SqlParameter("@Uid", TypeCode.String)).Value = User.Identity.GetUserId();
            int PlId = (int)cmdPlFav.ExecuteScalar();

            SqlCommand cmdAddFav = new SqlCommand("Insert INTO Video_to_playlist (Video_id,Playlist_id,Date_added) VALUES(@vidId,@PlId,@CurDate)", conn);
            cmdAddFav.Parameters.Add(new SqlParameter("@vidId", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
            cmdAddFav.Parameters.Add(new SqlParameter("@PlId", TypeCode.Int32)).Value = PlId;
            cmdAddFav.Parameters.Add(new SqlParameter("@CurDate", TypeCode.DateTime)).Value = System.DateTime.Now;
            cmdAddFav.ExecuteNonQuery();

            SqlCommand cmdInsertStats = new SqlCommand("INSERT INTO VideoStatistics (Video_id,User_id,interaction_date,interaction_type) VALUES(@VID,@UID,@CurDate,@action)", conn);
            cmdInsertStats.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
            cmdInsertStats.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
            cmdInsertStats.Parameters.Add(new SqlParameter("@CurDate", TypeCode.DateTime)).Value = System.DateTime.Now;
            cmdInsertStats.Parameters.Add(new SqlParameter("@action", TypeCode.Int32)).Value = 3;
            cmdInsertStats.ExecuteNonQuery();
        }

        SqlCommand cmdUserId = new SqlCommand("Select anu.Id from AspNetUsers anu JOIN Videos vids on vids.User_id = anu.id where vids.id = @VID", conn);
        cmdUserId.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
        string UID = (string)cmdUserId.ExecuteScalar();
        LogHub semnal = new LogHub();
        semnal.Send(UID, "notif_check");

        conn.Close();
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Post_Text { get; set; }
        public string User_posted { get; set; }
        public string date_posted { get; set; }
        public string icon_path { get; set; }
        public string UserName { get; set; }
        public int like { get; set; }
        public int dislike { get; set; }

        //public Comment() { }
    }

    public class PlaylistInfo
    {
        public int Id { get; set; }
        public int Nr { get; set; }
        public string Name { get; set; }
    }

    public class Videos
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public int Views { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string date_posted { get; set; }
        public string Thumbnail { get; set; }
        public string UserName { get; set; }

    }

    protected void Recomanded_list()
    {
        if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
        {
            SqlConnection conn;
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();

            SqlCommand cmdRec = new SqlCommand(@"Select TOP 10 vids.id,vids.title,vids.description,vids.views,vids.date_uploaded,vids.thumbnail,vids.user_id,anu.username from videos vids
                                                JOIN AspNetUsers anu on anu.Id = vids.User_id
                                                where vids.Status = 'Public' and vids.Id in
                                                (Select v.Video_id from VideoStatistics v
                                                where v.User_id in
                                                (Select UsersId.UsrId from (Select TOP 100 anu.Id UsrId,(Select count(*) nr from VideoStatistics vs
														                                                where vs.User_id = anu.Id and vs.Interaction_type =1 and vs.Video_id in (Select vs2.video_id from VideoStatistics vs2
																																                                                 where vs2.User_id = @UID and vs2.Interaction_type = 1)) VidLikedCount
					                                                from AspNetUsers anu 
					                                                where anu.id <> @UID
					                                                order by VidLikedCount DESC) UsersId) and V.Video_id not in (Select vs3.Video_id from VideoStatistics vs3 
																				                                                                         where vs3.User_id = @UID and (vs3.Interaction_type = 1 or vs3.Interaction_type = 2)))", conn);

            cmdRec.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
            List<Videos> VidList = new List<Videos>();

            try
            {
                using (SqlDataReader rdr = cmdRec.ExecuteReader())
                {
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            Videos vidInf = new Videos();
                            vidInf.Id = rdr.GetInt32(0);
                            vidInf.UserId = rdr.GetString(6);
                            vidInf.Title = rdr.GetString(1);
                            vidInf.Views = rdr.GetInt32(3);
                            vidInf.date_posted = rdr.GetDateTime(4).ToString("dd MMMM, yyyy");
                            vidInf.Thumbnail = rdr.GetString(5);
                            vidInf.UserName = rdr.GetString(7);
                            VidList.Add(vidInf);
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
            VIdPlayRep.DataSource = VidList;
            VIdPlayRep.DataBind();

            SqlCommand cmdFeature = new SqlCommand(@"Select TOP 10 vids.id,vids.title,vids.description,vids.views,vids.date_uploaded,vids.thumbnail,vids.user_id,anu.username from videos vids
                                                join AspNetUsers anu on anu.id = vids.user_id
                                                where anu.id = @UID and vids.Status = 'Public' ", conn);
            cmdFeature.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();

            List<Videos> VidListFeat = new List<Videos>();

            try
            {
                using (SqlDataReader rdr = cmdFeature.ExecuteReader())
                {
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            Videos vidInfFeat = new Videos();
                            vidInfFeat.Id = rdr.GetInt32(0);
                            vidInfFeat.UserId = rdr.GetString(6);
                            vidInfFeat.Title = rdr.GetString(1);
                            vidInfFeat.Views = rdr.GetInt32(3);
                            vidInfFeat.date_posted = rdr.GetDateTime(4).ToString("dd MMMM, yyyy");
                            vidInfFeat.Thumbnail = rdr.GetString(5);
                            vidInfFeat.UserName = rdr.GetString(7);
                            VidListFeat.Add(vidInfFeat);
                        }
                    rdr.Close();
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("No rows found.");
            }

            Featured_Repeater.DataSource = VidListFeat;
            Featured_Repeater.DataBind();
            conn.Close();
        }
    }

    public void R1_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
    {
        // This event is raised for the header, the footer, separators, and items.
        // Execute the following logic for Items and Alternating Items.
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            if (((Comment)e.Item.DataItem).User_posted == User.Identity.GetUserId() || User.Identity.GetUserId() == Session["Vid_owner"])
            {
                // ((Label)e.Item.FindControl("RatingLabel")).Text = "<b>***Good***</b>";
                ((Button)e.Item.FindControl("DeleteButton")).Visible = true;
            }
        }
    }

    protected void PlaylistRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            if (((PlaylistInfo)e.Item.DataItem).Nr > 0)
            {
                ((LinkButton)e.Item.FindControl("RemoveButton")).Visible = true;
            }
            else
                ((LinkButton)e.Item.FindControl("AddButton")).Visible = true;

        }
    }

    protected void AddToPlaylist_Command(object sender, CommandEventArgs e)
    {
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        if (e.CommandName == "Add")
        {
            SqlCommand cmd = new SqlCommand("insert into Video_to_Playlist (Video_id,Playlist_id,Date_added) VALUES(@VID,@PID,@CurDate)", conn);
            cmd.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
            cmd.Parameters.Add(new SqlParameter("@PID", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
            cmd.Parameters.Add(new SqlParameter("@CurDate", TypeCode.DateTime)).Value = System.DateTime.Now;
            cmd.ExecuteNonQuery();
        }
        if (e.CommandName == "Remove")
        {
            SqlCommand cmd = new SqlCommand("Delete from Video_to_Playlist where Video_id = @VID and Playlist_id = @PID", conn);
            cmd.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
            cmd.Parameters.Add(new SqlParameter("@PID", TypeCode.Int32)).Value = Convert.ToInt32(e.CommandArgument.ToString());
            cmd.ExecuteNonQuery();
        }
        GetPlaylists();


    }

    protected void CreateNewPlaylist(object sender, EventArgs e)
    {
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        SqlCommand cmdCheck = new SqlCommand("Select count(*) from Playlists where name = @newName and user_id = @UID", conn);
        cmdCheck.Parameters.Add(new SqlParameter("@NewName", TypeCode.String)).Value = PLTitle.Text;
        cmdCheck.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();

        if ((Int32)cmdCheck.ExecuteScalar() > 0)
            Response.Write("You already have a playlist with this name.");
        else
        {
            SqlCommand cmd = new SqlCommand("Insert into Playlists (Name,User_id,Views,Date_created) OUTPUT INSERTED.Id values(@Nume,@UID,0,@CurDate) ", conn);
            cmd.Parameters.Add(new SqlParameter("@Nume", TypeCode.String)).Value = PLTitle.Text;
            cmd.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
            cmd.Parameters.Add(new SqlParameter("@CurDate", TypeCode.DateTime)).Value = System.DateTime.Now;
            int NewPlId = (Int32)cmd.ExecuteScalar();

            cmd = new SqlCommand("Insert Into Video_to_Playlist (Video_id,Playlist_id,Date_added) VALUES(@VID,@PID,@CurDate) ", conn);
            cmd.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
            cmd.Parameters.Add(new SqlParameter("@PID", TypeCode.Int32)).Value = NewPlId;
            cmd.Parameters.Add(new SqlParameter("@CurDate", TypeCode.DateTime)).Value = System.DateTime.Now;
            cmd.ExecuteNonQuery();
        }
    }

    protected void Chart3_Load()
    {

        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        SqlCommand cmdDateMax = new SqlCommand("Select max(interaction_date) from VideoStatistics where Video_id = @Video_id", conn);
        cmdDateMax.Parameters.Add(new SqlParameter("@Video_id", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
        DateTime dmax = (DateTime)cmdDateMax.ExecuteScalar();

        SqlCommand cmdDateMin = new SqlCommand("Select min(interaction_date) from VideoStatistics where Video_id = @Video_id", conn);
        cmdDateMin.Parameters.Add(new SqlParameter("@Video_id", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());
        DateTime dmin = (DateTime)cmdDateMin.ExecuteScalar();

        SqlCommand cmd1 = new SqlCommand(@"WITH sample AS (
                                          SELECT CAST(Convert(varchar(10),@StartDate,126) AS DATE) AS dt
                                          UNION ALL
                                          SELECT DATEADD(dd, 1, dt)
                                            FROM sample s
                                           WHERE DATEADD(dd, 1, dt) <= CAST(Convert(varchar(10),@EndDate,126) AS DATE))
                                        SELECT s.dt,(Select count(*)
                                        from VideoStatistics vs
                                        where Convert(varchar(10),vs.Interaction_date,126) <= s.dt and vs.Video_id = @Video_id and vs.Interaction_type = 5) nr
                                          FROM sample s
                                          option (maxrecursion 0)", conn);
        cmd1.Parameters.Add(new SqlParameter("@StartDate", TypeCode.DateTime)).Value = dmin;
        cmd1.Parameters.Add(new SqlParameter("@EndDate", TypeCode.DateTime)).Value = dmax;
        cmd1.Parameters.Add(new SqlParameter("@Video_id", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"].ToString());

        List<int> countsNr = new List<int>();
        List<string> Listdates = new List<string>();
        int n = 0;
        try
        {
            using (SqlDataReader rdr = cmd1.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        int cnr = rdr.GetInt32(1);
                        countsNr.Add(cnr);
                        DateTime myDate = rdr.GetDateTime(0);
                        string dtr = myDate.ToString("dd MMMM, yyyy");
                        Listdates.Add(dtr);
                        n++;
                    }
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("No rows found.");
        }

        int[] nrs = countsNr.ToArray();
        string[] dates = Listdates.ToArray();
        Chart3.Series.Add(new Series());
        Chart3.Series[0].Points.DataBindXY(dates, nrs);

        Chart3.ChartAreas.Add(new ChartArea());

        Chart3.Series[0].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Line;
    }

    private void Master_AdminDelte(object sender, EventArgs e)
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Delete from Videos where id = @vId", conn);
        cmd.Parameters.Add(new SqlParameter("@vId", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"]);
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        // Create an event handler for the master page's contentCallEvent event
        Master.contentCallEvent += new EventHandler(Master_AdminDelte);
    }

    protected void Refresh_Comments_ServerClick(object sender, EventArgs e)
    {

        FetchComments();
        Main_input.Value = "";

    }
}
