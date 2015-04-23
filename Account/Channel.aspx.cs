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
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Net;


public partial class Account_Channel : System.Web.UI.Page
{
    string Channel_id;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack && PicUpload.PostedFile != null)
        {

            if (PicUpload.PostedFile.FileName.Length > 0)
            {
                Response.Write("fdkmnsf");
                // PicUpload.SaveAs(Server.MapPath("~/") + PicUpload.PostedFile.FileName);

                // lblMessage.Text = FileUpload2.PostedFile.FileName + " uploaded successfully !";

            }

        }


        if (Request.Params["id"].ToString() == "you")
            Channel_id = User.Identity.GetUserId();
        else
            Channel_id = Request.Params["id"].ToString();

        ModalVIz.Visible = false;
        if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
        {
            if (Channel_id == User.Identity.GetUserId())
                ModalVIz.Visible = true;
            else
                ModalVIz.Visible = false;
        }

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
        Spotlight_List();
        Home_ChannelClick();
        AllActivity();
        Master.FindControl("AdminVidDelete").Visible = true;

    }

    protected void Home_ChannelClick()
    {
        if (Request.Params["id"].ToString() == "you")
            Channel_id = User.Identity.GetUserId();
        else
            Channel_id = Request.Params["id"].ToString();

        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        List<Videos> vids = new List<Videos>();


        SqlCommand cmd1 = new SqlCommand(@"Select TOP 10 v.id,v.title,v.description,v.views,v.date_uploaded,v.thumbnail,v.user_id,anu.username 
                                        from videos v 
                                        join AspNetUsers anu on anu.id = v.user_id 
                                        WHERE v.status = 'Public' and v.User_id = @UID
                                        order by v.date_uploaded desc", conn);
        cmd1.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = Channel_id;
        //string list = "";
        try
        {
            using (SqlDataReader rdr = cmd1.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        string UserName = rdr.GetString(7);
                        Videos vInfo = new Videos();
                        vInfo.Id = rdr.GetInt32(0);
                        vInfo.UserId = rdr.GetString(6);
                        vInfo.Title = rdr.GetString(1);
                        vInfo.Description = rdr.GetString(2) + " ";
                        vInfo.Views = rdr.GetInt32(3);
                        vInfo.date_posted = rdr.GetDateTime(4).ToString("dd MMMM, yyyy");
                        vInfo.Thumbnail = rdr.GetString(5);
                        vInfo.UserName = UserName;
                        vids.Add(vInfo);
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

        VIdPlayRep.DataSource = vids;
        VIdPlayRep.DataBind();
        conn.Close();
    }




    //protected void Activity_ChannelClick(object sender, EventArgs e)
    //{
    //    HtmlButton button = (HtmlButton)sender;
    //    string buttonId = button.ID;
    //    Button btn = new Button();
    //    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
    //    conn.Open();
    //    SqlCommand cmd = new SqlCommand("select Id,Post_text,User_posted,User_wall,date_posted from User_wall_posts where User_wall = @usrId and replied_id = 0", conn);
    //    cmd.Parameters.Add(new SqlParameter("@usrId", TypeCode.String)).Value = Channel_id;

    //    string list = "<div class=''><form method='post' class='form-inline' role='form'><div class='form-group'><input id='Main_input' class='form-control' type='text' placeholder='Your comments' /></div><div class='form-group'><button type='submit' id='Default' class='btn btn-default' runat='server' onserverclick='Post_comment'>Post</button></div></form><ul class=''>";
    //    string close_button = " ";
    //    if (Channel_id == User.Identity.GetUserId())
    //    {
    //        close_button = "<button type='button' class='close' aria-hidden='true' runat='server' onserverclick='delete_post'>&times;</button>";
    //    }


    //    try
    //    {
    //        using (SqlDataReader rdr = cmd.ExecuteReader())
    //        {
    //            if (rdr.HasRows)
    //            {
    //                string icon_path = @"G:\Stocare_Proiect_Licenta\Users\Default_icon.jpeg";
    //                string user_posted = rdr.GetString(2);
    //                string UserName = "";
    //                list += "<li><div class='commenterImage'><a href='~/Account/Channel?id" + user_posted + "' runat='server'><img src='";


    //                SqlCommand cmd1 = new SqlCommand("select Name,icon from AspNetUsers where Id = @usrId", conn);
    //                cmd1.Parameters.Add(new SqlParameter("@usrId", TypeCode.String)).Value = user_posted;
    //                try
    //                {
    //                    using (SqlDataReader rdr2 = cmd.ExecuteReader())
    //                    {
    //                        if (rdr2.HasRows)
    //                        {
    //                            UserName = rdr2.GetString(0);
    //                            icon_path = rdr2.GetString(1); }
    //                        rdr2.Close();
    //                    }
    //                }
    //                catch (InvalidOperationException)
    //                {
    //                    Console.WriteLine("No rows found.");
    //                }

    //                list += icon_path + "' /></a></div><a href='~/Account/Channel?id=" + user_posted + "'>" + UserName + "</a><div id='deleteBtn_" + Convert.ToString(rdr.GetInt32(0)) + "'>" + close_button + "</div><div class='commentText'><p class=''>" + rdr.GetString(1);
    //                list += "</p> <span class='date sub-text'>on " + rdr.GetDateTime(4).ToString() + "</span></div>";
    //                list += "<button class='btn btn-default' data-toggle='collapse' data-target='#replyBox" + Convert.ToString(rdr.GetInt32(0)) + "' >Reply</button><div id='replyBox" + Convert.ToString(rdr.GetInt32(0)) + "' class='collapse' ><form class='form-inline' role='form'><div class='form-group'><input id='reply_input_" + Convert.ToString(rdr.GetInt32(0)) + "' class='form-control' type='text' placeholder='Your comments' /></div><div class='form-group'><button  id='button-" + Convert.ToString(rdr.GetInt32(0)) + "' class='btn btn-default' runat='server' onserverclick='Post_comment'>Post</button></div></form>";
    //                list += "<ul class='comm-reply'><li><div id='temp-comm-" + Convert.ToString(rdr.GetInt32(0)) + "'></div></li></ul></li>";
    //            }
    //            rdr.Close();
    //        }
    //    }
    //    catch (InvalidOperationException)
    //    {
    //        Console.WriteLine("No rows found.");
    //    }

    //    cmd = new SqlCommand("select Id,Post_text,User_posted,User_wall,date_posted,Replied_id from User_wall_posts where User_wall = @usrId and replied_id <> 0", conn);
    //    cmd.Parameters.Add(new SqlParameter("@usrId", TypeCode.String)).Value = Channel_id;
    //    try
    //    {
    //        using (SqlDataReader rdr = cmd.ExecuteReader())
    //        {
    //            if (rdr.HasRows)
    //            {
    //                string reply_list = "";
    //                string icon_path = @"G:\Stocare_Proiect_Licenta\Users\Default_icon.jpeg";
    //                string user_posted = rdr.GetString(2);
    //                string UserName = "" ;
    //                reply_list += "<li><div class='commenterImage'><a href='~/Account/Channel?id" + user_posted + "' runat='server'><img src='";


    //                SqlCommand cmd1 = new SqlCommand("select Name,icon from AspNetUsers where Id = @usrId", conn);
    //                cmd1.Parameters.Add(new SqlParameter("@usrId", TypeCode.String)).Value = user_posted;
    //                try
    //                {
    //                    using (SqlDataReader rdr2 = cmd.ExecuteReader())
    //                    {
    //                        if (rdr2.HasRows)
    //                        {
    //                            UserName = rdr2.GetString(0);
    //                            icon_path = rdr2.GetString(1); }
    //                        rdr2.Close();
    //                    }
    //                }
    //                catch (InvalidOperationException)
    //                {
    //                    Console.WriteLine("No rows found.");
    //                }

    //                reply_list += icon_path + "' /></a></div><a href='~/Account/Channel?id=" + user_posted + "'>" + UserName + "</a><div id='deleteBtn_" + Convert.ToString(rdr.GetInt32(0)) + "'>" + close_button + "</div><div class='commentText'><p class=''>" + rdr.GetString(1);
    //                reply_list += "</p> <span class='date sub-text'>on " + rdr.GetDateTime(4).ToString() + "</span></div>";
    //                reply_list += "<button class='btn btn-default' data-toggle='collapse' data-target='#replyBox" + Convert.ToString(rdr.GetInt32(0)) + "' >Reply</button><div id='replyBox" + Convert.ToString(rdr.GetInt32(0)) + "' class='collapse' ><form class='form-inline' role='form'><div class='form-group'><input id='reply_input_" + Convert.ToString(rdr.GetInt32(0)) + "' class='form-control' type='text' placeholder='Your comments' /></div><div class='form-group'><button  id='button-" + Convert.ToString(rdr.GetInt32(0)) + "' class='btn btn-default' runat='server' onserverclick='Post_comment'>Post</button></div></form>";
    //                reply_list += "<ul class='comm-reply'><li><div id='temp-comm-" + Convert.ToString(rdr.GetInt32(0)) + "'></div></li></ul></li><li><div id='temp-comm-" + Convert.ToString(rdr.GetInt32(5)) + "'></div></li>";
    //                list = list.Replace("<li><div id='temp-comm-" + Convert.ToString(rdr.GetInt32(5)) + "'></div></li>", reply_list);
    //            }
    //            rdr.Close();
    //        }
    //    }
    //    catch (InvalidOperationException)
    //    {
    //        Console.WriteLine("No rows found.");
    //    }
    //    channel_container.InnerHtml += list;
    //    conn.Close();


    //    //unde vrei
    //   // bullshit.DataSource = 0; // ii dam o lista cu toate commenturile, de genul List<Comment>
    //    /*
    //     * public class Comment
    //     * {
    //     *      int Id;
    //     *      string Post_Text;
    //     *      etc...
    //     *      
    //     *     
    //     * }

    //     * List<Comment> list = new List<Comment>();
    //     * while (reader.read())
    //     * {
    //     * Comment com = new Comment();
    //     * com.Id = reader.GetInt(0);
    //     * com. Post_Text = reader.GetString(1);
    //     * ... si tot asa
    //     * 
    //     * list.Add(com);
    //     * }
    //     * bullshit.DataSource = list;
    //     * bullshit.DataBind();
    //     * 
    //     * 
    //     * 
    //     * 
    //     */

    //}

    public class Comment
    {
        public int Id;
        public string Post_Text;
        public string User_posted, User_wall;
        public string date_posted, icon_path, UserName;

        //public Comment() { }
    }
    public class Videos
    {
        public int Id { get; set; }
        public int Views { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string date_posted { get; set; }
        public string Thumbnail { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        //public string Status { get; set; }

        //public Comment() { }
    }
    public class Activity
    {
        public int Id { get; set; }
        public int Views { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string date_posted { get; set; }
        public string Thumbnail { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string SubId { get; set; }
        public string SubName { get; set; }
        public string InterDate { get; set; }
        public string InterName { get; set; }

        //public Comment() { }
    }

    protected void AllActivity()
    {
        if (Request.Params["id"].ToString() == "you")
            Channel_id = User.Identity.GetUserId();
        else
            Channel_id = Request.Params["id"].ToString();

        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        List<Activity> vids = new List<Activity>();

        SqlCommand cmdActivity = new SqlCommand(@"Select TOP 100 v.id,v.title,v.description,v.views,v.date_uploaded,v.thumbnail,v.user_id,anu2.username,vs.User_id,anu.UserName,vs.Interaction_date,itr.Name
                                                from VideoStatistics vs 
                                                Join AspNetUsers anu on vs.User_id = anu.Id
                                                join Interactions itr on itr.id = vs.Interaction_type
                                                join Videos v on v.Id = vs.Video_id
                                                join AspNetUsers anu2 on anu2.Id = v.User_id
                                                where vs.User_id = @UID and (itr.Name <> 'Watch' and itr.Name <> 'Dislike' and itr.Name <> 'Upload')
                                                and v.status = 'Public'
                                                order by vs.Interaction_date desc", conn);

        cmdActivity.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = Channel_id;

        try
        {
            using (SqlDataReader rdr = cmdActivity.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        string UserName = rdr.GetString(7);
                        Activity vInfo = new Activity();
                        vInfo.Id = rdr.GetInt32(0);
                        vInfo.UserId = rdr.GetString(6);
                        vInfo.Title = rdr.GetString(1);
                        vInfo.Description = rdr.GetString(2) + " ";
                        vInfo.Views = rdr.GetInt32(3);
                        vInfo.date_posted = rdr.GetDateTime(4).ToString("dd MMMM, yyyy");
                        vInfo.Thumbnail = rdr.GetString(5);
                        vInfo.UserName = UserName;
                        vInfo.SubId = rdr.GetString(8);
                        vInfo.SubName = rdr.GetString(9);
                        vInfo.InterDate = rdr.GetDateTime(10).ToString("dd MMMM, yyyy");
                        switch (rdr.GetString(11))
                        {
                            case "Like": vInfo.InterName = " has liked a video ";
                                break;
                            case "Favorite": vInfo.InterName = " has added a video to favorites ";
                                break;
                            case "Share": vInfo.InterName = " has shared a video ";
                                break;
                            case "Comment": vInfo.InterName = " has commented on a video ";
                                break;
                            case "Upload": vInfo.InterName = " has uploaded a video ";
                                break;
                            default: vInfo.InterName = " other action ";
                                break;
                        }
                        vids.Add(vInfo);
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

        Activity_Repeater.DataSource = vids;
        Activity_Repeater.DataBind();
        conn.Close();
    }

    protected void Spotlight_List()
    {
         if (Request.Params["id"].ToString() == "you")
            Channel_id = User.Identity.GetUserId();
        else
            Channel_id = Request.Params["id"].ToString();
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        List<Videos> vids = new List<Videos>();

        SqlCommand cmdList = new SqlCommand(@"Select TOP 3 vid.id,vid.title,vid.description,vid.views,vid.date_uploaded,vid.thumbnail,vid.user_id,anu.username,CAST(views as float)/(Cast((select datediff(hour,min(ss.interaction_date),max(ss.interaction_date)) hrs from videoStatistics ss where ss.video_id = vid.id)as float)+1) asd
                                            from Videos vid 
                                            join AspNetUsers anu on anu.Id = vid.User_id
                                            where (vid.likes<> 0 or vid.dislikes <> 0) and (vid.likes*100)/(vid.likes+vid.dislikes) > 10 and vid.user_id = @UID
                                            order by asd", conn);
        cmdList.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = Channel_id;
        int first = 0;
        try
        {
            using (SqlDataReader rdr = cmdList.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        if (first == 0)
                        {
                            first = 1;
                            Main_spot_link.HRef = "~/watch?id=" + Convert.ToString(rdr.GetInt32(0));
                            Main_spot_thumb.Src = rdr.GetString(5);
                            Spot_title.InnerText = rdr.GetString(1);
                            Spot1UserLink.HRef = "~/Account/Channel?id=" + rdr.GetString(6);
                            Spot1User.Text = "by " + rdr.GetString(7);
                            Spot1Date.Text = "on " + rdr.GetDateTime(4).ToString("dd MMMM, yyyy");
                            Spor1Views.Text = rdr.GetInt32(3).ToString();
                        }
                        else
                        {
                            string UserName = rdr.GetString(7);
                            Videos vInfo = new Videos();
                            vInfo.Id = rdr.GetInt32(0);
                            vInfo.UserId = rdr.GetString(6);
                            vInfo.Title = rdr.GetString(1);
                            vInfo.Description = rdr.GetString(2) + " ";
                            vInfo.Views = rdr.GetInt32(3);
                            vInfo.date_posted = rdr.GetDateTime(4).ToString("dd MMMM, yyyy");
                            vInfo.Thumbnail = rdr.GetString(5);
                            vInfo.UserName = UserName;
                            vids.Add(vInfo);
                        }
                    }
                rdr.Close();
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("No rows found.");
        }
        SpotLight_Repeater.DataSource = vids;
        SpotLight_Repeater.DataBind();
        conn.Close();
    }

    protected void Post_comment(object sender, EventArgs e)
    {
        HtmlButton button = (HtmlButton)sender;
      //  HtmlInputText ceva = (HtmlInputText)button.PreviousControl();
        string buttonId = button.ID;
        string textboxId = "<input id='Main_input' class='form-control' type='text' placeholder='Your comments' />";
        // HtmlInputText = (HtmlInputText)textboxId;
        if (buttonId == "Default")
        {
            textboxId = "Main_input";

        }

        else
        {
            buttonId = buttonId.Replace("button-", "");
            textboxId = "reply_input_" + buttonId;

        }
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("insert into User_wall_posts (Post_text,User_posted,User_wall,date_posted,replied_id) values (@txt,@usrPost,@usrWall,@date,@replid ", conn);

        cmd.Parameters.Add(new SqlParameter("@txt", TypeCode.String)).Value = Main_input.Value;
        cmd.Parameters.Add(new SqlParameter("@UsrPost", TypeCode.String)).Value = User.Identity.GetUserId();
        cmd.Parameters.Add(new SqlParameter("@usrWall", TypeCode.String)).Value = Channel_id;
        cmd.Parameters.Add(new SqlParameter("@date", TypeCode.DateTime)).Value = System.DateTime.Now;
        cmd.Parameters.Add(new SqlParameter("@replied", TypeCode.String)).Value = 0;
        cmd.ExecuteNonQuery();
        conn.Close();

    }

    protected void delete_post(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        string buttonId = button.Parent.ID;
        buttonId = buttonId.Replace("deleteBtn_", "");
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Delete from Videos where id = @pstId", conn);
        cmd.Parameters.Add(new SqlParameter("@pstId", TypeCode.Int32)).Value = Convert.ToInt32(buttonId);
        conn.Close();

    }

    protected void delete_video(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        string buttonId = button.Parent.ID;
        buttonId = buttonId.Replace("videoId_", "");
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Delete from Videos where id = @pstId", conn);
        cmd.Parameters.Add(new SqlParameter("@pstId", TypeCode.Int32)).Value = Convert.ToInt32(buttonId);
        conn.Close();

    }

    protected void delete_playlist(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        string buttonId = button.Parent.ID;
        buttonId = buttonId.Replace("videoId_", "");
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Delete from Playlists where id = @pstId", conn);
        cmd.Parameters.Add(new SqlParameter("@pstId", TypeCode.Int32)).Value = Convert.ToInt32(buttonId);
        conn.Close();

    }

    void WebClientUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
    {
        Console.WriteLine("Download {0}% complete. ", e.ProgressPercentage);
    }

    protected void btnSubmitImage_Click(object sender, EventArgs e)
    {
        Thread t2 = new Thread(delegate()
        {
            HttpPostedFile myFile = PicUpload.PostedFile;
            string path = "temp" + User.Identity.GetUserName() + "ProfilePic" + Path.GetExtension(myFile.FileName);
            string pathfin = User.Identity.GetUserName() + "ProfilePic" + Path.GetExtension(myFile.FileName);
            string FilePath;
            string[] mediaExtensions = { ".JPG", ".PNG", ".JPEG", ".BMP" };

            if (-1 != Array.IndexOf(mediaExtensions, Path.GetExtension(path).ToUpperInvariant()))
            {
                FilePath = System.IO.Path.Combine(@"G:\Stocare_Proiect_Licenta\Channels", User.Identity.Name);
                if (!Directory.Exists(FilePath))
                    System.IO.Directory.CreateDirectory(FilePath);
                FileStream newFile = new FileStream(System.IO.Path.Combine(FilePath, path), FileMode.Create);
                byte[] myData = new byte[myFile.ContentLength];
                myFile.InputStream.Read(myData, 0, myFile.ContentLength);
                newFile.Write(myData, 0, myData.Length);
                newFile.Close();

                //WebClient webClient = new WebClient();
                //webClient.UploadFileAsync(,"Post");
                //webClient.UploadProgressChanged += WebClientUploadProgressChanged;

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("Update AspNetUsers set Icon = @pic where id = @UID", conn);
                cmd.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
                cmd.Parameters.Add(new SqlParameter("@pic", TypeCode.String)).Value = System.IO.Path.Combine(@"~\AppStorage\Channels\" + User.Identity.Name, pathfin);
                cmd.ExecuteNonQuery();
                conn.Close();
                //Bitmap
                var image = System.Drawing.Image.FromFile(System.IO.Path.Combine(FilePath, path));
                var newImage = ScaleImage(image, 100, 100);
                image.Dispose();

                //using (MemoryStream memory = new MemoryStream())
                //{
                //    using (FileStream fs = new FileStream(System.IO.Path.Combine(FilePath, pathfin), FileMode.Create, FileAccess.ReadWrite))
                //    {
                //        newImage.Save(memory, ImageFormat.Jpeg);
                //        byte[] bytes = memory.ToArray();
                //        fs.Write(bytes, 0, bytes.Length);
                //        fs.Close();
                //    }
                //}
                //while (CheckIfFileIsBeingUsed(System.IO.Path.Combine(FilePath, path)) == true) { }
                File.Delete(System.IO.Path.Combine(FilePath, path));
                newImage.Save(System.IO.Path.Combine(FilePath, pathfin), ImageFormat.Jpeg);
                newImage.Dispose();

            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Show", "$(document).ready(function() {$('#myModal').modal('hide');});", true);
        });
        t2.Start();

       

    }

    public bool CheckIfFileIsBeingUsed(string fileName)
    {

        try
        {
            File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
        }

        catch (Exception exp)
        {
            return true;
        }

        return false;

    }

    public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
    {
        var ratioX = (double)maxWidth / image.Width;
        var ratioY = (double)maxHeight / image.Height;
        var ratio = Math.Max(ratioX, ratioY);

        var newWidth = (int)(image.Width * ratio);
        var newHeight = (int)(image.Height * ratio);

        var newImage = new Bitmap(newWidth, newHeight);
        Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);

        if (newWidth > maxHeight || newHeight > maxHeight)
        {
            Rectangle rectCrop = new Rectangle(0, 0, maxWidth, maxHeight);
            Bitmap cropImg = new Bitmap(newImage);
            newImage = cropImg.Clone(rectCrop, cropImg.PixelFormat);
        }
        return newImage;
    }

    private void Master_AdminDelte(object sender, EventArgs e)
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Delete from AspNetUsers where id = @vId", conn);
        cmd.Parameters.Add(new SqlParameter("@vId", TypeCode.Int32)).Value = Convert.ToInt32(Request.Params["id"]);
        cmd.ExecuteNonQuery();
        conn.Close();
    }
    protected void Page_PreInit(object sender, EventArgs e)
    {
        // Create an event handler for the master page's contentCallEvent event
        Master.contentCallEvent2 += new EventHandler(Master_AdminDelte);
    }
}
//public static class Clasamasii
//{
//    public static Control PreviousControl(this Control control)
//    {
//        ControlCollection siblings = control.Parent.Controls;
//        for (int i = siblings.IndexOf(control) - 1; i >= 0; i--)
//        {
//            if (siblings[i].GetType() != typeof(LiteralControl) && siblings[i].GetType().BaseType != typeof(LiteralControl))
//            {
//                return siblings[i];
//            }
//        }
//        return null;
//    }

//}