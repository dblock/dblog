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

public partial class ShowPicture : PicturePage
{
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
        TransitImage img = SessionManager.BlogService.GetImageWithBitmapById(SessionManager.Ticket, id);

        pic.Bitmap = img.Data;
        pic.Created = pic.Modified = img.Modified;
        pic.Name = img.Name;
        pic.Id = img.Id;

        return pic;
    }

    public override DBlog.Tools.Web.PicturePage.Picture GetPictureWithBitmap(int id, DateTime ifModifiedSince)
    {
        Picture pic = new Picture();
        TransitImage img = SessionManager.BlogService.GetImageWithBitmapByIdIfModifiedSince(SessionManager.Ticket, id, ifModifiedSince);

        if (img == null)
        {
            return null;
        }

        pic.Bitmap = img.Data;
        pic.Created = pic.Modified = img.Modified;
        pic.Name = img.Name;
        pic.Id = img.Id;

        return pic;
    }

    public override DBlog.Tools.Web.PicturePage.Picture GetPictureWithThumbnail(int id)
    {
        Picture pic = new Picture();
        TransitImage img = SessionManager.BlogService.GetImageWithThumbnailById(SessionManager.Ticket, id);

        pic.Bitmap = img.Thumbnail;
        pic.Created = pic.Modified = img.Modified;
        pic.Name = img.Name;
        pic.Id = img.Id;

        return pic;
    }

    public override DBlog.Tools.Web.PicturePage.Picture GetPictureWithThumbnail(int id, DateTime ifModifiedSince)
    {
        Picture pic = new Picture();
        TransitImage img = SessionManager.BlogService.GetImageWithThumbnailByIdIfModifiedSince(SessionManager.Ticket, id, ifModifiedSince);

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

    public override DBlog.Tools.Web.PicturePage.Picture GetRandomPictureWithThumbnail()
    {
        throw new NotImplementedException();
    }
}
