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
using DBlog.Tools.Web;
using DBlog.TransitData;
using DBlog.Tools.Drawing;

public partial class ShowPicture : BlogPicturePage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
    }

    public override PicturePage.PicturePageType PageType
    {
        get
        {
            return ShowThumbnail ? PicturePageType.Thumbnail : PicturePageType.Bitmap;
        }
    }

    public bool ShowThumbnail
    {
        get
        {
            string param = Request.QueryString["ShowThumbnail"];
            if (string.IsNullOrEmpty(param)) return true;
            return bool.Parse(param);
        }
    }

    public override DBlog.Tools.Web.PicturePage.Picture GetPictureWithBitmap(int id)
    {
        Picture pic = new Picture();
        
        TransitImage img = SessionManager.GetCachedObject<TransitImage>(
            "GetImageWithBitmapById", SessionManager.PostTicket, id);

        if (img == null)
            return null;

        if (img.Data == null && !string.IsNullOrEmpty(img.Path))
        {
            img.Data = new ThumbnailBitmap(Path.Combine(Path.Combine(
                SessionManager.GetSetting("Images", string.Empty),
                img.Path), img.Name)).Bitmap;
        }

        pic.Bitmap = img.Data;
        pic.Created = pic.Modified = img.Modified;
        pic.Name = img.Name;
        pic.Id = img.Id;

        if (SessionManager.CountersEnabled)
        {
            IncrementCounter();
        }

        return pic;
    }

    public override DBlog.Tools.Web.PicturePage.Picture GetPictureWithBitmap(int id, DateTime ifModifiedSince)
    {
        Picture pic = new Picture();

        string key = string.Format("{0}:{1}:{2}",
            string.IsNullOrEmpty(SessionManager.PostTicket) ? 0 : SessionManager.PostTicket.GetHashCode(), 
            "GetImageWithBitmapByIdIfModifiedSince", id);
        
        TransitImage img = (TransitImage) Cache[key];
        
        if (img == null || SessionManager.IsAdministrator)
        {
            img = SessionManager.BlogService.GetImageWithBitmapByIdIfModifiedSince(
                SessionManager.PostTicket, id, ifModifiedSince);
            Cache.Insert(key, img, SessionManager.GetTransitTypeCacheDependency<TransitImage>(), DateTime.Now.AddMinutes(10), TimeSpan.Zero);
        }

        if (img == null)
        {
            return null;
        }

        if (img.Data == null && !string.IsNullOrEmpty(img.Path))
        {
            img.Data = new ThumbnailBitmap(Path.Combine(Path.Combine(
                SessionManager.GetSetting("Images", string.Empty),
                img.Path), img.Name)).Bitmap;
        }

        pic.Bitmap = img.Data;
        pic.Created = pic.Modified = img.Modified;
        pic.Name = img.Name;
        pic.Id = img.Id;

        if (SessionManager.CountersEnabled)
        {
            IncrementCounter();
        }

        return pic;
    }

    public override DBlog.Tools.Web.PicturePage.Picture GetPictureWithThumbnail(int id)
    {
        Picture pic = new Picture();

        TransitImage img = SessionManager.GetCachedObject<TransitImage>(
            "GetImageWithThumbnailById", SessionManager.PostTicket, id);

        pic.Bitmap = img.Thumbnail;
        pic.Created = pic.Modified = img.Modified;
        pic.Name = img.Name;
        pic.Id = img.Id;

        return pic;
    }

    public override DBlog.Tools.Web.PicturePage.Picture GetPictureWithThumbnail(int id, DateTime ifModifiedSince)
    {
        Picture pic = new Picture();

        string key = string.Format("{0}:{1}:{2}",
            string.IsNullOrEmpty(SessionManager.PostTicket) ? 0 : SessionManager.PostTicket.GetHashCode(), 
            "GetImageWithThumbnailByIdIfModifiedSince", id);
        
        TransitImage img = (TransitImage)Cache[key];
        
        if (img == null || SessionManager.IsAdministrator)
        {
            img = SessionManager.BlogService.GetImageWithThumbnailByIdIfModifiedSince(
                SessionManager.PostTicket, id, ifModifiedSince);
            Cache.Insert(key, img, SessionManager.GetTransitTypeCacheDependency<TransitImage>(), DateTime.Now.AddMinutes(10), TimeSpan.Zero);
        }

        if (img == null)
        {
            return null;
        }

        pic.Bitmap = img.Thumbnail;
        pic.Created = pic.Modified = img.Modified;
        pic.Name = img.Name;
        pic.Id = img.Id;

        return pic;
    }

    public void IncrementCounter()
    {
        object ic = Request.QueryString["IncrementCounter"];
        if (!IsPostBack && !ShowThumbnail && (ic == null || bool.Parse(ic.ToString()) == true))
        {
            CounterCache.IncrementImageCounter(RequestId, Cache, SessionManager);
        }
    }

    public override DBlog.Tools.Web.PicturePage.Picture GetRandomPictureWithThumbnail()
    {
        throw new NotImplementedException();
    }
}
