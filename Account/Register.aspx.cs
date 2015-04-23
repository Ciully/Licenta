using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.UI;
using ProiectLicenta;
using System.Globalization;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

public partial class Account_Register : Page
{
    protected void Page_Load(object sendet, EventArgs e)
    {
        if (!IsPostBack)
        {
            DropDownList1.DataSource = CountryList();
            DropDownList1.DataBind();
            Year.DataSource = DateInpuntYear();
            Year.DataBind();
            day.DataSource = DateInputDays(Month.SelectedIndex + 1, Convert.ToInt32(Year.SelectedValue));
            day.DataBind();
        }
        //if (IsPostBack)
        //{
        //    day.DataSource = DateInputDays(Month.SelectedIndex + 1, Convert.ToInt32(Year.SelectedValue));
        //    day.DataBind();
        //}

    }

    public static List<string> CountryList()
    {
        List<string> CultureList = new List<string>();
        CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        foreach (CultureInfo getCulture in getCultureInfo)
        {
            RegionInfo GetRegionInfo = new RegionInfo(getCulture.LCID);
            if(!(CultureList.Contains(GetRegionInfo.EnglishName)))
            { CultureList.Add(GetRegionInfo.EnglishName);
            }
        }
        CultureList.Sort();
        return CultureList;
    }

    public static List<int> DateInpuntYear()
    {
        List<int> DateListYear = new List<int>();
        int year_now = System.DateTime.Now.Year;
        for (int i = year_now; i >=1900 ; i--)
            DateListYear.Add(i);
        return DateListYear;
    }

    protected void CreateUser_Click(object sender, EventArgs e)
    {
        var manager = new UserManager();
        var user = new ApplicationUser() { UserName = UserName.Text };
        IdentityResult result = manager.Create(user, Password.Text);
        if (result.Succeeded)
        {
            manager.AddToRole(user.Id, "User");
            SqlConnection conn;
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();

            SqlCommand c = new SqlCommand("Update AspNetUsers set Country = @state ,Birth_date = @Bdate,Sex = @gender,Date_created = @CurentDate where Id = @UsrId;", conn);
            c.Parameters.Add(new SqlParameter("@state", TypeCode.String));
            c.Parameters["@state"].Value = DropDownList1.SelectedValue;
            c.Parameters.Add(new SqlParameter("@Bdate", TypeCode.DateTime));
            c.Parameters["@Bdate"].Value = Convert.ToDateTime(day.SelectedValue+"-"+Month.SelectedValue+"-"+Year.SelectedValue);
            c.Parameters.Add(new SqlParameter("@gender",TypeCode.String));
            c.Parameters["@gender"].Value = GenderList.SelectedValue;
            c.Parameters.Add(new SqlParameter("@CurentDate", TypeCode.DateTime));
            c.Parameters["@CurentDate"].Value = System.DateTime.Now;
            c.Parameters.Add(new SqlParameter("@UsrId", TypeCode.String));
            c.Parameters["@UsrId"].Value = user.Id;
            c.ExecuteNonQuery();

            SqlCommand cmdPlaylistcrt = new SqlCommand("Insert into Playlists (Name,User_id,date_created,Last_update) VALUES('Favorites',@UID,@dateNow,@update) ", conn);
            cmdPlaylistcrt.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = user.Id;
            cmdPlaylistcrt.Parameters.Add(new SqlParameter("@dateNow", TypeCode.DateTime)).Value = System.DateTime.Now;
            cmdPlaylistcrt.Parameters.Add(new SqlParameter("@update", TypeCode.DateTime)).Value = System.DateTime.Now;
            cmdPlaylistcrt.ExecuteNonQuery();

            cmdPlaylistcrt = new SqlCommand("Insert into Playlists (Name,User_id,date_created,Last_update) VALUES('History',@UID,@dateNow,@update) ", conn);
            cmdPlaylistcrt.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = user.Id;
            cmdPlaylistcrt.Parameters.Add(new SqlParameter("@dateNow", TypeCode.DateTime)).Value = System.DateTime.Now;
            cmdPlaylistcrt.Parameters.Add(new SqlParameter("@update", TypeCode.DateTime)).Value = System.DateTime.Now;
            cmdPlaylistcrt.ExecuteNonQuery();

            conn.Close();

            IdentityHelper.SignIn(manager, user, isPersistent: false);
            IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
        }
        else
        {
            ErrorMessage.Text = result.Errors.FirstOrDefault();
        }
    }
    protected void Month_SelectedIndexChanged(object sender, EventArgs e)
    {
        day.DataSource = DateInputDays(Month.SelectedIndex + 1, Convert.ToInt32(Year.SelectedValue));
        day.DataBind();
    }

    public static List<int> DateInputDays(int month, int year)
    {
        List<int> DateDay = new List<int>();
        if (System.DateTime.IsLeapYear(year) && month == 2)
        {
            for (int i = 1; i <= 29; i++)
                DateDay.Add(i);
            return DateDay;
        }

        int lim = 0;
        if (month == 2)
            lim = 28;
        else
            if (month < 8)
                if (month % 2 != 0)
                    lim = 31;
                else lim = 30;
            else
                if (month % 2 != 0)
                    lim = 30;
                else lim = 31;

        for (int i = 1; i <= lim; i++)
            DateDay.Add(i);
        return DateDay;
    }

}