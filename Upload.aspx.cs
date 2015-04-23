using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using WMPLib;
using System.Threading;
using NReco.VideoConverter;
using System.Drawing;
using System.Drawing.Imaging;
using ProiectLicenta;
using System.Diagnostics;


[Serializable]
public partial class Upload : System.Web.UI.Page
{
    //Int32 new_vid_id;
    static EventWaitHandle _waitHandle = new AutoResetEvent (false);

    protected void Page_Load(object sender, EventArgs e)
    {
        CategoryDrop.DataSource = CategoryList();
        CategoryDrop.DataBind();
    }

    protected void Upload_Click(object sender, EventArgs e)
    {
        HttpPostedFile myFile = VideoUpload.PostedFile;
        string timeName = Convert.ToString(System.DateTime.Now.Date) + "_" + Convert.ToString(System.DateTime.Now.TimeOfDay) + "_" + Convert.ToString(System.DateTime.Now.Ticks);
        timeName = timeName.Replace("/", "_").Replace(":", "_");
        string path = "temp" + timeName + Path.GetFileName(myFile.FileName);
        string pathfin = timeName + Path.GetFileName(myFile.FileName);
        string FilePath;
        string[] mediaExtensions = {".AVI", ".MP4", ".DIVX", ".WMV", ".MPEG4"};

        if(-1 != Array.IndexOf(mediaExtensions, Path.GetExtension(path).ToUpperInvariant())){
            FilePath = System.IO.Path.Combine(@"G:\Stocare_Proiect_Licenta\Channels",User.Identity.Name);
            if(!Directory.Exists(FilePath))
                System.IO.Directory.CreateDirectory(FilePath);
            FileStream newFile = new FileStream(System.IO.Path.Combine(FilePath,path), FileMode.Create);
            byte[] myData = new byte[myFile.ContentLength];
            myFile.InputStream.Read(myData, 0, myFile.ContentLength);
            newFile.Write(myData, 0, myData.Length);
            newFile.Close();
            aditional_info.Visible = true;
            vid_up_containre.Visible = false;
            ConversionWraper.Visible = true;
            VidTitle.Text = myFile.FileName;
            
            
            string path2 = Path.ChangeExtension(pathfin, ".jpg");
           // FileStream thumb = new FileStream(System.IO.Path.Combine(FilePath, path2), FileMode.Create);
            //File.Create(System.IO.Path.Combine(FilePath, path2));
            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            string Video_Path = System.IO.Path.Combine(FilePath, path);
            var player = new WindowsMediaPlayer();
            var clip = player.newMedia(Video_Path);
            int sec = Convert.ToInt32(TimeSpan.FromSeconds(clip.duration).TotalSeconds) / 2;
            ffMpeg.GetVideoThumbnail(Video_Path, System.IO.Path.Combine(FilePath, path2), sec);
            //ffMpeg.ConvertMedia()

            var image = System.Drawing.Image.FromFile(System.IO.Path.Combine(FilePath, path2));
            var newImage = ScaleImage(image, 490, 275);
            image.Dispose();
            newImage.Save(System.IO.Path.Combine(FilePath, path2), ImageFormat.Jpeg);
            newImage.Dispose();
            /////////
            SqlConnection conn;
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();

            SqlCommand c = new SqlCommand("insert into Videos (Title,Path,Status,Date_Uploaded,User_id,Thumbnail) OUTPUT INSERTED.Id values(@FileName,@UserPath,@stat,@data,@UsrId,@Thumb)", conn);
            c.Parameters.Add(new SqlParameter("@FileName", TypeCode.String));
            c.Parameters["@FileName"].Value = myFile.FileName;
            c.Parameters.Add(new SqlParameter("@UserPath", TypeCode.String));
            c.Parameters["@UserPath"].Value = System.IO.Path.Combine(@"AppStorage\Channels\" + User.Identity.Name, pathfin);
            c.Parameters.Add(new SqlParameter("@stat", TypeCode.String));
            c.Parameters["@stat"].Value = "Pending";
            c.Parameters.Add(new SqlParameter("@data", TypeCode.DateTime));
            c.Parameters["@data"].Value = System.DateTime.Now;
            c.Parameters.Add(new SqlParameter("@UsrId", TypeCode.String));
            c.Parameters["@UsrId"].Value = User.Identity.GetUserId();
            c.Parameters.Add(new SqlParameter("@Thumb", TypeCode.String)).Value = System.IO.Path.Combine(@"~\AppStorage\Channels\" + User.Identity.Name, path2);
               int strid = (int)c.ExecuteScalar();
               Session.Add("new_vid_id", strid);

               SqlCommand cmdInsertStats = new SqlCommand("INSERT INTO VideoStatistics (Video_id,User_id,interaction_date,interaction_type) VALUES(@VID,@UID,@CurDate,@action)", conn);
               cmdInsertStats.Parameters.Add(new SqlParameter("@VID", TypeCode.Int32)).Value = strid;
               cmdInsertStats.Parameters.Add(new SqlParameter("@UID", TypeCode.String)).Value = User.Identity.GetUserId();
               cmdInsertStats.Parameters.Add(new SqlParameter("@CurDate", TypeCode.DateTime)).Value = System.DateTime.Now;
               cmdInsertStats.Parameters.Add(new SqlParameter("@action", TypeCode.Int32)).Value = 7;
               cmdInsertStats.ExecuteNonQuery();

            conn.Close();
            //ffMpeg.ConvertMedia(Video_Path, System.IO.Path.Combine(FilePath, pathfin), NReco.VideoConverter.Format.mp4);
           // VideoThreadinConversion.ConvertVideo(Video_Path, FilePath, pathfin, User.Identity.GetUserId());
           // Process.Start("~/ffmpeg.exe");
            AppDomain ad = AppDomain.CreateDomain("NewDomain");
            //AppDomain.Unload(AppDomain.CurrentDomain);

            Thread t1 = new Thread(
        unused => compressVideo(Video_Path, System.IO.Path.Combine(FilePath, pathfin), User.Identity.GetUserId())
        );
            t1.Start();
            //string uid =  User.Identity.GetUserId();
            //ad.DoCallBack(() =>
            //{
            //    Thread t1 = new Thread(
            //unused => compressVideo(Video_Path, System.IO.Path.Combine(FilePath, pathfin),uid)
            //);
            //    t1.Start();

            //});

            //Task.Factory.StartNew(() => compressVideo(Video_Path, System.IO.Path.Combine(FilePath, pathfin),progress),
            //                    TaskCreationOptions.LongRunning);
        }
        else
        {
            Response.Write("The file is not an known video format.");
            System.Web.HttpContext.Current.Response.Write("<SCRIPT LANGUAGE=''JavaScript''>alert('The file is not an known video format.')</SCRIPT>");
        }

      
    }


    public static void compressVideo(string Video_Path, string pathfin,string UID)
    {
        var ffMpeg2 = new NReco.VideoConverter.FFMpegConverter();
        ffMpeg2.ConvertProgress += (o, args) =>
            {
                double perc2 =  (args.Processed.TotalSeconds * 100) / args.TotalDuration.TotalSeconds ;
                LogHub semnal = new LogHub();
                semnal.ProgressSignal(UID, perc2.ToString("F3"));
                Console.WriteLine(String.Format("Progress: {0} / {1}\r\n", args.Processed, args.TotalDuration));
            };

        ffMpeg2.ConvertMedia(Video_Path, pathfin, NReco.VideoConverter.Format.mp4);
        File.Delete(Video_Path);
        _waitHandle.Set();
    }




    private void UpdateProgress(object sender, ConvertProgressEventArgs e)
    {
        
    }
    protected void Submit_Click(object sender, EventArgs e)
    {
        string thumbnail = "";
        if (ThumbnailUp.HasFile)
        {
            HttpPostedFile myFile = ThumbnailUp.PostedFile;
            string timeName = Convert.ToString(System.DateTime.Now.Date) + "_" + Convert.ToString(System.DateTime.Now.TimeOfDay) + "_" + Convert.ToString(System.DateTime.Now.Ticks);
            timeName = timeName.Replace("/", "_").Replace(":", "_");
            string path = timeName + Path.GetFileName(myFile.FileName);
            thumbnail = ",Thumbnail = " + System.IO.Path.Combine(@"~\AppStorage\Channels\" + User.Identity.Name, path);
            string FilePath = System.IO.Path.Combine(@"G:\Stocare_Proiect_Licenta\Channels", User.Identity.Name);
            if (!Directory.Exists(FilePath))
                System.IO.Directory.CreateDirectory(FilePath);
            FileStream newFile = new FileStream(System.IO.Path.Combine(FilePath, path), FileMode.Create);
            byte[] myData = new byte[myFile.ContentLength];
            myFile.InputStream.Read(myData, 0, myFile.ContentLength);
            newFile.Write(myData, 0, myData.Length);
            newFile.Close();

            //string path2 = Path.ChangeExtension(path, ".jpg");
            var image = System.Drawing.Image.FromFile(System.IO.Path.Combine(FilePath, path));
            var newImage = ScaleImage(image, 400, 225);
            image.Dispose();
            newImage.Save(System.IO.Path.Combine(FilePath, path), ImageFormat.Jpeg);
            newImage.Dispose();
        }

        string str_tags;
        str_tags = Tags.Text.Replace(" ", "/");
        int VidId = (int)Session["new_vid_id"];
        SqlConnection conn;
        conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();
        SqlCommand cmdCateg = new SqlCommand("select id from Categories where category_name = '" + CategoryDrop.SelectedValue + "';", conn);
        int categId = Convert.ToInt32((int)cmdCateg.ExecuteScalar());
        conn.Close();
      //  VideoThreadinConversion.UpdateDb(VidTitle.Text, VideoDescription.Text, System.DateTime.Now, categId, User.Identity.GetUserId(), VideoPrivacy.SelectedValue, str_tags, VidId, thumbnail);
        Thread t1 = new Thread(
    unused => UpdateDbVideo(VidTitle.Text, VideoDescription.Text, System.DateTime.Now, categId, User.Identity.GetUserId(), VideoPrivacy.SelectedValue, str_tags, VidId, thumbnail)
  );
        t1.Start();
        
    }


    public static void UpdateDbVideo(string vidTitle, string vidDesc, DateTime curDate, int categId, string UsrId, string Privacy, string tags, int vidId, string thumbnail)
    {

        _waitHandle.WaitOne(); 
        SqlConnection conn;
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        conn.Open();


        SqlCommand c = new SqlCommand("Update Videos set Title = @vidname ,Description  = @info,Date_Uploaded = @CurentDate,Category_id = @categ,User_Id = @UsrId,Status = @stats,Tags = @tagstring " + thumbnail + " where Id = @VidId;", conn);
        c.Parameters.Add(new SqlParameter("@vidname", TypeCode.String));
        c.Parameters["@vidname"].Value = vidTitle;
        c.Parameters.Add(new SqlParameter("@info", TypeCode.String));
        c.Parameters["@info"].Value = vidDesc;
        c.Parameters.Add(new SqlParameter("@CurentDate", TypeCode.DateTime));
        c.Parameters["@CurentDate"].Value = curDate;
        c.Parameters.Add(new SqlParameter("@categ", TypeCode.Int32));
        c.Parameters["@categ"].Value =  categId ;
        c.Parameters.Add(new SqlParameter("@UsrId", TypeCode.String));
        c.Parameters["@UsrId"].Value = UsrId;
        c.Parameters.Add(new SqlParameter("@stats", TypeCode.String));
        c.Parameters["@stats"].Value = Privacy;
        c.Parameters.Add(new SqlParameter("@tagstring", TypeCode.String));
        c.Parameters["@tagstring"].Value = tags;
        c.Parameters.Add(new SqlParameter("@VidId", TypeCode.Int32));
        c.Parameters["@VidId"].Value = vidId;
        c.ExecuteNonQuery();

        conn.Close();
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

}