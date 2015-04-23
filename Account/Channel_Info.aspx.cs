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

public partial class Account_Channel_about : System.Web.UI.Page
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
            About_ChannelClick();
        }

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Select Icon from AspNetUsers where id = @UID", conn);
        cmd.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = Channel_id;
        ProfilePic.ImageUrl = (string)cmd.ExecuteScalar();
        conn.Close();
    }




    protected void About_ChannelClick()
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("select description,Date_created from AspNetUsers where Id = @usrId", conn);
        cmd.Parameters.Add(new SqlParameter("@usrId", TypeCode.String)).Value = Channel_id;

        try
        {
            SqlCommand cmdnrs = new SqlCommand("Select sum(Views) from Videos where User_id = @usrId", conn);
            cmdnrs.Parameters.Add(new SqlParameter("@usrId", TypeCode.Int32)).Value = Channel_id;
            int nrviews = (int)cmdnrs.ExecuteScalar();
           // cmdnrs.Dispose();
            cmdnrs = new SqlCommand("SELECT count(*) from Subscriptions where User_id = @usrId",conn);
           cmdnrs.Parameters.Add(new SqlParameter("@usrId", TypeCode.Int32)).Value = Channel_id;
            int nrsubs = (int)cmdnrs.ExecuteScalar();
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                if (rdr.HasRows)
                     while(rdr.Read())
                        {
                            channel_descr.InnerText = rdr.GetString(0);
                            date_join.InnerText = "Joined on "+rdr.GetDateTime(1).ToString("dd MMMM, yyyy");
                            Label1.Text = nrviews.ToString() + ":Views    " + nrsubs.ToString() + ":Subcribers";
                    
                        }
                rdr.Close();

            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("No rows found.");
        }
        conn.Close();

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        channel_descr_container.Visible = false;
        Update_form.Visible = true;
        message_text.InnerText = channel_descr.InnerText;
    }
    protected void Button2_Click(object sender, EventArgs e)
    {

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("update AspNetUsers set description = @newDescr where id = @usrId", conn);
        cmd.Parameters.Add(new SqlParameter("@usrId", TypeCode.String)).Value = Channel_id;
        cmd.Parameters.Add(new SqlParameter("@newDescr", TypeCode.String)).Value = message_text.InnerText;
        cmd.ExecuteNonQuery();

        channel_descr_container.Visible = true;
        Update_form.Visible = false;

        About_ChannelClick();
    }
    protected void Button2_Click1(object sender, EventArgs e)
    {
        channel_descr_container.Visible = true;
        Update_form.Visible = false;
    }
}