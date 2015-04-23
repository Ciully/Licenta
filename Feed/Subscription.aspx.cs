using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;

public partial class Feed_Subscription : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SubsUploads();
        AllActivity();
    }

 
    protected void SubsUploads()
    {
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        List<Videos> vids = new List<Videos>();

        SqlCommand cmd1 = new SqlCommand(@"Select top 100 v.id,v.title,v.description,v.views,v.date_uploaded,v.thumbnail,v.user_id,anu.username from Videos v 
                                        join AspNetUsers anu on anu.Id = v.User_id
                                        where anu.id in (Select subs.Fallowing_id from Subscriptions subs
				                                        where subs.User_id = @usrId)
                                        and v.status = 'Public'
                                        order by v.Date_Uploaded desc", conn);
        cmd1.Parameters.Add(new SqlParameter("@usrId", TypeCode.String)).Value = User.Identity.GetUserId();

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

        Repeater1.DataSource = vids;
        Repeater1.DataBind();
        conn.Close();
    }

    protected void AllActivity()
    {

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
                                                where vs.User_id in (Select subs.Fallowing_id from Subscriptions subs
   				                                                where subs.User_id = @UID) and (itr.Name <> 'Watch' and itr.Name <> 'Dislike')
                                                and v.status = 'Public'
                                                order by vs.Interaction_date desc", conn);

        cmdActivity.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();

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
                        switch (rdr.GetString(11)){
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

}