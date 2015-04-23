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

public partial class Account_Channel_videos : System.Web.UI.Page
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
        Videos_ChannelClick("");
        CategoryDrop.DataSource = CategoryList();
        CategoryDrop.DataBind();

        }
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Select Icon from AspNetUsers where id = @UID", conn);
        cmd.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = Channel_id;
        ProfilePic.ImageUrl = (string)cmd.ExecuteScalar();
        conn.Close();
    }

    public static List<string> CategoryList()
    {
        List<string> List = new List<string>();

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        SqlCommand cmd = new SqlCommand("SELECT Category_name from Categories", conn);
        using (SqlDataReader rdr = cmd.ExecuteReader())
        {
            string ctg;
            if(rdr.HasRows)
            while(rdr.Read()){
                ctg = rdr.GetString(0);
                List.Add(ctg);
            }
        }
        return List;
    }

    protected void Button3_ServerClick(object sender, EventArgs e)
    {
        Videos_ChannelClick(Video_stcInput.Text);
    }

    protected void Videos_ChannelClick(string src)
    {
        //Query pt channelul userului logat
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        List<Videos> vids = new List<Videos>();

        try
        {
            string vidPriv = "";
            if (Channel_id == User.Identity.GetUserId())
            {
                 vidPriv = "status <> 'Pending' and";
                 SqlCommand cmd = new SqlCommand("Select Id,Title,Description,date_Uploaded,thumbnail from Videos WHERE status = 'Pending' and user_id = @UID and (Title LIKE @term or Tags LIKE @term)  order by Date_Uploaded desc", conn);
                cmd.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = Request.Params["id"].ToString();
                cmd.Parameters.Add(new SqlParameter("@term", TypeCode.String)).Value = "%" + src + "%";
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            Videos vInfo = new Videos();
                            vInfo.Id = rdr.GetInt32(0);
                            vInfo.Title = rdr.GetString(1);
                            vInfo.Description = rdr.GetString(2) + " ";
                            vInfo.date_posted = rdr.GetDateTime(3).ToString("dd MMMM, yyyy");
                            vInfo.Thumbnail = rdr.GetString(4);
                            vInfo.Status = "Pending";
                            vids.Add(vInfo);
                        }
                    rdr.Close();
                }
            }
            else
            {
                 vidPriv = "status = 'Public' and ";

            }
            SqlCommand cmd1 = new SqlCommand("Select Id,Title,Description,date_Uploaded,thumbnail,Status,tags from Videos WHERE " + vidPriv + " user_id = @UID and (Title LIKE @term or Tags LIKE @term) order by Date_Uploaded desc", conn);
            cmd1.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = Request.Params["id"].ToString();
            cmd1.Parameters.Add(new SqlParameter("@term", TypeCode.String)).Value = "%" + src + "%";       
            using (SqlDataReader rdr = cmd1.ExecuteReader())
            {
                if (rdr.HasRows)
                    while(rdr.Read())
                    {
                        Videos vInfo = new Videos();
                        vInfo.Id = rdr.GetInt32(0);
                        vInfo.Title = rdr.GetString(1);
                        vInfo.Description = rdr.GetString(2);
                        vInfo.date_posted = rdr.GetDateTime(3).ToString("dd MMMM, yyyy");
                        vInfo.Thumbnail = rdr.GetString(4);
                        vInfo.Status = rdr.GetString(5);
                        vInfo.Tags = rdr.GetString(6);
                        vids.Add(vInfo);

                    }
                rdr.Close();
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("No rows found.");
        }
        video_list.DataSource = vids;
        video_list.DataBind();
        conn.Close();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "PendingColor()", true);

    }

    public class Videos
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string date_posted { get; set; }
        public string Thumbnail { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Tags { get; set; }

    }

    protected void Update_video_Click(object sender, EventArgs e)
{
    Button btn = (Button)sender;
    string vidId = btn.CommandArgument.ToString();
    vid_update_id = Convert.ToInt32(vidId);
    Videos_ChannelClick(Video_stcInput.Text);
}

    protected void CommandBtn_Click(Object sender, CommandEventArgs e)
{
    if (e.CommandName == "Update")
    {
        string[] info = e.CommandArgument.ToString().Split('#');
        Session["new_vid_id"]= Convert.ToInt32(info[0]);
        //VidTitleLabel.Text = info[1];
        Session["vid_title"] = info[1];
        VideoDescription.Text = info[2];    
        Tags.Text = info[3];
        //Page.ClientScript.RegisterStartupScript(this.GetType(), "Show", "$(document).ready(function() {$('#aditional_info').modal('show');});", true);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$(document).ready(function() {console.log('fafga'); $('#aditional_info').modal('show');});", true);

    }
    if (e.CommandName == "Delete")
    {
        int info = Convert.ToInt32(e.CommandArgument.ToString());
        Session["new_vid_id"] = info;
        deleteVideo(info);
        Videos_ChannelClick(Video_stcInput.Text);

    }
}

    protected void deleteVideo(int vidID)
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Delete from Videos where id = @vId", conn);
        cmd.Parameters.Add(new SqlParameter("@vId", TypeCode.Int32)).Value = vidID;
        cmd.ExecuteNonQuery();
        conn.Close();

    }

    protected void Submit_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$(document).ready(function() {console.log('fafga'); $('#Save_modal').modal('show');});", true);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Hideded", "$(document).ready(function() {$('#Save_modal').modal('hide');});", true);
        string str_tags;
        str_tags = Tags.Text.Replace(" ", "/");
        //FileStream thumb;
        //var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
        //ffMpeg.GetVideoThumbnail(pathToVideoFile, thumb, 5);
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();


        SqlCommand c = new SqlCommand("Update Videos set Title = @vidname ,Description  = @info,Date_Uploaded = @CurentDate,Category_id = @categ,Status = @stats,Tags = @tagstring where Id = @VidId;", conn);
        c.Parameters.Add(new SqlParameter("@vidname", TypeCode.String));
        c.Parameters["@vidname"].Value = VidTitle.Text;
        c.Parameters.Add(new SqlParameter("@info", TypeCode.String));
        c.Parameters["@info"].Value = VideoDescription.Text;
        c.Parameters.Add(new SqlParameter("@CurentDate", TypeCode.DateTime));
        c.Parameters["@CurentDate"].Value = System.DateTime.Now;
        c.Parameters.Add(new SqlParameter("@categ", TypeCode.Int32));
        SqlCommand cmdCateg = new SqlCommand("select id from Categories where category_name = '" + CategoryDrop.SelectedValue + "';", conn);
        c.Parameters["@categ"].Value = Convert.ToInt32((int)cmdCateg.ExecuteScalar());  //todo categoriile matii
        c.Parameters.Add(new SqlParameter("@stats", TypeCode.String));
        c.Parameters["@stats"].Value = VideoPrivacy.SelectedValue;
        c.Parameters.Add(new SqlParameter("@tagstring", TypeCode.String));
        c.Parameters["@tagstring"].Value = str_tags;
        c.Parameters.Add(new SqlParameter("@VidId", TypeCode.Int32));
        c.Parameters["@VidId"].Value = Session["new_vid_id"];
        c.ExecuteNonQuery();

        conn.Close();
        Videos_ChannelClick(Video_stcInput.Text);

    }

    public bool showDelete()
    {
        if (Channel_id == User.Identity.GetUserId())
            return true;
        else return false;
    }
}