using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        List<Videos> vids = new List<Videos>();
        

        SqlCommand cmd1 = new SqlCommand("Select v.id,v.title,v.description,v.views,v.date_uploaded,v.thumbnail,v.user_id,anu.username from videos v join AspNetUsers anu on anu.id = v.user_id where v.status = 'Public' order by v.date_uploaded desc", conn);
        string list = "";
        try
        {
            using (SqlDataReader rdr = cmd1.ExecuteReader())
            {
                if (rdr.HasRows)
                 while(rdr.Read())
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
        Spotlight_List();
        Recomanded_list();

    }


    protected void Spotlight_List()
    {
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        List<Videos> vids = new List<Videos>();

        SqlCommand cmdList = new SqlCommand(@"Select TOP 3 vid.id,vid.title,vid.description,vid.views,vid.date_uploaded,vid.thumbnail,vid.user_id,anu.username,CAST(views as float)/(Cast((select datediff(hour,min(ss.interaction_date),max(ss.interaction_date)) hrs from videoStatistics ss where ss.video_id = vid.id)as float)+1) asd
                                            from Videos vid 
                                            join AspNetUsers anu on anu.Id = vid.User_id
                                            where (vid.likes<> 0 or vid.dislikes <> 0) and (vid.likes*100)/(vid.likes+vid.dislikes) > 10 and DATEDIFF(week,vid.date_uploaded,SYSDATETIME()) < 5
                                            order by asd", conn);

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

            conn.Close();
        }
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
}