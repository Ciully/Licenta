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

public partial class Account_AdminControlPanel : System.Web.UI.Page
{
    int vid_update_id;

protected void Page_Load(object sender, EventArgs e)
    {
        Session["vid_title"] = "Nein";
        if (!IsPostBack)
        {
            hidTAB.Value = "home";
            Session["vid_title"] = "Nein";
            GetCategories();
            Get_Users("");
            CategoryDrop.DataSource = CategoryList();
            CategoryDrop.DataBind();
        }
        if (IsPostBack)
        {
            string tabc = "#" + hidTAB.Value;
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Show", "$( '#myTab a[href=" + '"' + "'" + tabc + "'" + '"' + "]' ).tab( 'show' );", true);

        }
    }

protected void GetCategories()
{
    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
    conn.Open();
    SqlCommand cmd = new SqlCommand("Select id,Category_Name from Categories", conn);
    List<Categories> listCateg = new List<Categories>();
    try
    {
        using (SqlDataReader rdr = cmd.ExecuteReader())
        {
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    Categories vInfo = new Categories();
                    vInfo.Id = rdr.GetInt32(0);
                    vInfo.categ_name = rdr.GetString(1);
                    listCateg.Add(vInfo);
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

    Categ_Repeater.DataSource = listCateg;
    Categ_Repeater.DataBind();
    conn.Close();
}

public class Categories
{
    public int Id { get; set; }
    public string categ_name { get; set; }
}

 protected void Chart1_Load(object sender, EventArgs e)
    {

    }

protected void UserSearch_ServerClick(object sender, EventArgs e)
{
    Get_Users(UserSrc.Text);
}

protected void Get_Users(string src)
{
    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
    conn.Open();
    SqlCommand cmd = new SqlCommand(@"Select anu.id,anu.Username,anu.Date_created,anu.icon 
                                    from AspNetUsers anu
                                    Join AspNetUserRoles anur on anur.userId = anu.id
                                    join AspNetRoles anr on anr.id = anur.RoleId 
                                    where UserName like @srcterm and anr.Name <> 'Admin' ", conn);
    cmd.Parameters.Add(new SqlParameter("@srcterm", TypeCode.String)).Value = "%" + src + "%";
    List<UsersClass> listCateg = new List<UsersClass>();
    try
    {
        using (SqlDataReader rdr = cmd.ExecuteReader())
        {
            if (rdr.HasRows)
                while (rdr.Read())
                {
                    UsersClass vInfo = new UsersClass();
                    vInfo.Id = rdr.GetString(0);
                    vInfo.UserName = rdr.GetString(1);
                    vInfo.Date_created = rdr.GetDateTime(2).ToString("dd MMMM, yyyy");
                    vInfo.icon_path = rdr.GetString(3);
                    listCateg.Add(vInfo);
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

    UsersRepeater.DataSource = listCateg;
    UsersRepeater.DataBind();
    conn.Close();

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
    public string tags { get; set; }
}

public class UsersClass
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Date_created { get; set; }
    public string icon_path { get; set; }
}

protected void Button3_ServerClick(object sender, EventArgs e)
{
    Get_Videos(Video_stcInput.Text);
}

protected void Get_Videos(string src_term)
{
    SqlConnection conn;
    conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
    conn.Open();

    List<Videos> vids = new List<Videos>();
    SqlCommand cmd1;

    cmd1 = new SqlCommand("SELECT v.id,v.title,v.description,v.views,v.date_uploaded,v.thumbnail,v.user_id,anu.username,v.tags FROM Videos v JOIN AspNetUsers anu ON anu.id = v.user_id WHERE Title LIKE @term or Tags LIKE @term ORDER BY likes DESC", conn);
    cmd1.Parameters.Add(new SqlParameter("@term", TypeCode.String)).Value = "%" + src_term + "%";

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
                    vInfo.tags = rdr.GetString(8);
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

    video_list.DataSource = vids;
    video_list.DataBind();
    conn.Close();

}

protected void Update_video_Click(object sender, EventArgs e)
{
    Button btn = (Button)sender;
    string vidId = btn.CommandArgument.ToString();
    vid_update_id = Convert.ToInt32(vidId);
    Get_Videos(Video_stcInput.Text);
}

protected void CommandBtn_Click(Object sender, CommandEventArgs e)
{
    if (e.CommandName == "Update")
    {
        string[] info = e.CommandArgument.ToString().Split('#');
        Session["new_vid_id"] = Convert.ToInt32(info[0]);
        VidTitleLabel.Text = info[1];
        //Session["vid_title"] = info[1];
        VideoDescription.Text = info[2];
        Tags.Text = info[3];
        //Page.ClientScript.RegisterStartupScript(this.GetType(), "Show", "$(document).ready(function() {$('#aditional_info').modal('show');});", true);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "$(document).ready(function() {console.log('fafga'); $('#aditional_info').modal('show');});", true);
    }
    if (e.CommandName == "Delete")
    {
        int info = Convert.ToInt32(e.CommandArgument.ToString());
        Session["new_vid_id"] = Convert.ToInt32(info);
        deleteVideo(info);

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
    Get_Videos(Video_stcInput.Text);

}

protected void Category_Command(object sender, CommandEventArgs e)
{
    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
    conn.Open();
    SqlCommand cmd = new SqlCommand("Delete from Categories where id = @Id", conn);
    cmd.Parameters.Add(new SqlParameter("@Id", TypeCode.Int32)).Value = e.CommandArgument.ToString();
    cmd.ExecuteNonQuery();
    conn.Close();
    GetCategories();
}

protected void CategoryAdd_ServerClick(object sender, EventArgs e)
{
    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
    conn.Open();
    SqlCommand cmd = new SqlCommand("Insert into Categories (Category_Name) values(@Name)", conn);
    cmd.Parameters.Add(new SqlParameter("@Name", TypeCode.String)).Value = CategoryInput.Text;
    cmd.ExecuteNonQuery();
    conn.Close();
    GetCategories();

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
    Get_Videos(Video_stcInput.Text);
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
        if (rdr.HasRows)
            while (rdr.Read())
            {
                ctg = rdr.GetString(0);
                List.Add(ctg);
            }
    }
    return List;
}

protected void DeleteUser_Command(object sender, CommandEventArgs e)
{
    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
    conn.Open();
    SqlCommand cmd = new SqlCommand("Delete from AspNetUsers where id = @UID", conn);
    cmd.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = e.CommandArgument.ToString();
    cmd.ExecuteNonQuery();
    conn.Close();
    Get_Users(UserSrc.Text);

}
}
