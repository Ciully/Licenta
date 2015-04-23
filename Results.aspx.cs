using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Results : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {


        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();

        List<Videos> vids = new List<Videos>();
        SqlCommand cmd1;
        string src_term;
        if (Request.Params["categ"] == null)
        {
            src_term = Convert.ToString(Request.Params["src"]);
            cmd1 = new SqlCommand("SELECT v.id,v.title,v.description,v.views,v.date_uploaded,v.thumbnail,v.user_id,anu.username FROM Videos v JOIN AspNetUsers anu ON anu.id = v.user_id WHERE Title LIKE @term or Tags LIKE @term ORDER BY likes DESC", conn);
            cmd1.Parameters.Add(new SqlParameter("@term", TypeCode.String)).Value = "%" + src_term + "%";

        }
        else
        {
            src_term = Convert.ToString(Request.Params["categ"]);
            cmd1 = new SqlCommand("SELECT v.id,v.title,v.description,v.views,v.date_uploaded,v.thumbnail,v.user_id,anu.username FROM Videos v JOIN AspNetUsers anu ON anu.id = v.user_id join categories ct on ct.id = v.Category_id where ct.id = @term ORDER BY v.date_uploaded,likes DESC", conn);
            cmd1.Parameters.Add(new SqlParameter("@term", TypeCode.String)).Value = src_term;
        }


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