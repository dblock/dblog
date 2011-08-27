using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using DBlog.Tools.Drawing;
using System.Threading;

namespace DBlog.Tools.Web
{
    public abstract class PicturePage : Page
    {
        public enum PicturePageType
        {
            Thumbnail,
            Bitmap
        };

        public class Picture
        {
            public int Id;
            public DateTime Created;
            public DateTime Modified;
            public string Name;
            public Byte[] Bitmap;

            public Picture()
            {

            }
        }

        public abstract Picture GetPictureWithBitmap(int id, DateTime ifModifiedSince);
        public abstract Picture GetPictureWithThumbnail(int id, DateTime ifModifiedSince);
        public abstract Picture GetPictureWithBitmap(int id);
        public abstract Picture GetPictureWithThumbnail(int id);
        public abstract Picture GetRandomPictureWithThumbnail();

        public abstract PicturePageType PageType { get; }

        public virtual string Copyright
        {
            get
            {
                return null;
            }
        }

        public Nullable<DateTime> IfModifiedSince
        {
            get
            {
                Nullable<DateTime> result = new Nullable<DateTime>();
                object o = Request.Headers["If-Modified-Since"];
                if (o == null) return result;
                string s = o.ToString().Split(';')[0];
                DateTime dt;
                if (DateTime.TryParse(s, out dt)) result = dt;
                return result;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Nullable<DateTime> ims = IfModifiedSince;

                if (ims.HasValue)
                {
                    if (ims.Value.ToUniversalTime().AddSeconds(CacheDuration) > DateTime.UtcNow)
                    {
                        Response.StatusCode = 304;
                        return;
                    }
                }

                Picture p = null;

                switch (PageType)
                {
                    case PicturePageType.Thumbnail:

                        if (RequestId == 0)
                        {
                            p = GetRandomPictureWithThumbnail();

                            if (p == null)
                            {
                                p = new Picture();
                                p.Id = 0;
                                p.Created = p.Modified = DateTime.Now;
                                p.Name = Guid.NewGuid().ToString();
                                p.Bitmap = ThumbnailBitmap.GetBitmapDataFromText("?", 72, 100, 150);
                            }
                        }
                        else
                        {
                            p = ims.HasValue ?
                                    GetPictureWithThumbnail(RequestId, ims.Value) :
                                    GetPictureWithThumbnail(RequestId);
                        }

                        break;
                    case PicturePageType.Bitmap:

                        p = ims.HasValue ?
                                GetPictureWithBitmap(RequestId, ims.Value) :
                                GetPictureWithBitmap(RequestId);

                        break;
                }

                if (p == null)
                {
                    Response.StatusCode = (ims.HasValue ? 304 : 404);
                    return;
                }

                if (p.Bitmap == null)
                {
                    Response.Redirect("./images/site/access.gif", true);
                    return;
                }

                Response.Cache.SetLastModified(p.Modified.ToLocalTime());
                Response.Cache.SetCacheability(HttpCacheability.Private);

                p.Name = (string.IsNullOrEmpty(p.Name)) ? p.Id.ToString() + ".jpg" : p.Id.ToString() + "-" + p.Name;

                switch (PageType)
                {
                    case PicturePageType.Thumbnail:
                        p.Name.Insert(0, "thumbnail-");
                        break;
                    case PicturePageType.Bitmap:
                        if (!string.IsNullOrEmpty(Copyright))
                        {
                            ThumbnailBitmap bitmap = new ThumbnailBitmap(p.Bitmap);
                            bitmap.AddCopyright(Copyright);
                            p.Bitmap = bitmap.Bitmap;
                        }
                        break;
                }

                Response.ContentType = "image/" + Path.GetExtension(p.Name).TrimStart(".".ToCharArray());
                Response.AddHeader("Content-disposition", "attachment; filename=" + p.Name);
                Response.AddHeader("Created", p.Created.ToString("r"));
                Response.AddHeader("Modified", p.Modified.ToString("r"));
                Response.BinaryWrite(p.Bitmap);
                Response.End();
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        public int CacheDuration
        {
            get
            {
                object duration = Request["CacheDuration"];
                if (duration == null) return 0; // 60;
                return int.Parse(duration.ToString());
            }
        }

        public override string DefaultId
        {
            get
            {
                return "Id";
            }
        }
    }
}