using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBlog.Data;
using System.Net.Mail;
using System.Configuration;
using DBlog.Tools.Web;
using DBlog.TransitData.References;

namespace DBlog.TransitData
{
    public abstract class ManagedPostComment
    {
        public static void Notify(PostComment comment)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(ConfigurationManager.AppSettings["Email"]);
            message.To.Add(new MailAddress(ConfigurationManager.AppSettings["Email"]));
            message.Subject = string.Format("New Comment @ {0}", ConfigurationManager.AppSettings["Url"]);
            message.IsBodyHtml = true;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<P><b>{2}</b> ({3}) posted a <a href='{0}ShowPost.aspx?id={1}'>new comment</a> to {0}.</P>",
                ConfigurationManager.AppSettings["Url"], comment.Post.Id, comment.Comment.OwnerLogin.Name, comment.Comment.OwnerLogin.Email);
            ReferencesRedirector redirector = new ReferencesRedirector(comment.Post.Id, "Post");
            sb.AppendFormat("<P>{0}</P>", Renderer.RenderEx(
                comment.Comment.Text, ConfigurationManager.AppSettings["url"], redirector.ReferUri));
            message.Body = sb.ToString();
            SmtpClient client = new SmtpClient();
            client.Send(message);
        }
    }
}
