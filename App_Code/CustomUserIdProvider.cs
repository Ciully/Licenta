using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CustomUserIdProvider
/// </summary>
public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(IRequest request)
    {
        // your logic to fetch a user identifier goes here.

        // for example:
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmd1 = new SqlCommand("Select id from aspNetUsers where UserName = @Name",conn);
        cmd1.Parameters.Add(new SqlParameter("@Name", TypeCode.String)).Value = request.User.Identity.Name;

        string userId = (string)cmd1.ExecuteScalar();
        if (userId == null)
            userId = "Unknown";
        conn.Close();
        return userId.ToString();
    }
}