using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;


public partial class ManageSubscriptions : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            GetSubs();
            GetSubers();
        }
    }


    protected void GetSubs()
    {

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Select subs.id,Anu.userName,anu.id,anu.Icon From Subscriptions subs JOIN AspNetUsers anu on anu.id = subs.Fallowing_id where User_id = @UID", conn);
        cmd.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();

        List<Subscriber> list = new List<Subscriber>();
        try
        {
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        Subscriber comm = new Subscriber();
                        comm.Id = rdr.GetInt32(0);
                        comm.Name = rdr.GetString(1);
                        comm.UID = rdr.GetString(2);
                        comm.Icon = rdr.GetString(3);

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

        Subs.DataSource = list;
        Subs.DataBind();


    }

    protected void GetSubers()
    {

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Select subs.id,Anu.userName,anu.id,anu.Icon From Subscriptions subs JOIN AspNetUsers anu on anu.id = subs.Fallowing_id Where Fallowing_id = @UID", conn);
        cmd.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();

        List<Subscriber> list = new List<Subscriber>();
        try
        {
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                if (rdr.HasRows)
                    while (rdr.Read())
                    {
                        Subscriber comm = new Subscriber();
                        comm.Id = rdr.GetInt32(0);
                        comm.Name = rdr.GetString(1);
                        comm.UID = rdr.GetString(2);
                        comm.Icon = rdr.GetString(3);
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

        Subers.DataSource = list;
        Subers.DataBind();
    }

    public class Subscriber
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string UID { get; set; }
    }
    protected void Unsub_Command(object sender, CommandEventArgs e)
    {

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Delete From Subscriptions where id = @FID", conn);
        cmd.Parameters.Add(new SqlParameter("@FID", TypeCode.String)).Value = e.CommandArgument.ToString();
        cmd.ExecuteNonQuery();
        conn.Close();
        GetSubs();

    }
    protected void RemoveSub_Command(object sender, CommandEventArgs e)
    {

        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd = new SqlCommand("Delete From Subscriptions where id = @FID", conn);
        cmd.Parameters.Add(new SqlParameter("@FID", TypeCode.String)).Value = e.CommandArgument.ToString();
        cmd.ExecuteNonQuery();
        conn.Close();

        GetSubers();
    }
    
}