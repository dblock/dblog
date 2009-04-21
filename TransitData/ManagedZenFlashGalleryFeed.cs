using System;
using System.Collections.Generic;
using System.Text;
using DBlog.TransitData;
using DBlog.Data;
using System.Xml;
using System.Configuration;
using System.Web;
using NHibernate;
using NHibernate.Expression;
using System.Collections;
using DBlog.Tools.Web;
using System.Net;
using System.IO;

namespace DBlog.TransitData
{
    public class ManagedZenFlashGalleryFeed : ManagedFeed
    {
        public ManagedZenFlashGalleryFeed(Feed feed)
            : base(feed)
        {

        }

        protected override int UpdateFeedItems(ISession session)
        {
            Dictionary<string, XmlDocument> allalbums = new Dictionary<string, XmlDocument>();

            IList<Post> posts = session.CreateCriteria(typeof(Post))
                .Add(Expression.Eq("Export", true))
                .AddOrder(Order.Desc("Modified"))
                .List<Post>();

            foreach (Post post in posts)
            {
                XmlDocument albums = null;
                if (!allalbums.TryGetValue(post.Topic.Name, out albums))
                {
                    albums = new XmlDocument();
                    albums.Load(Path.Combine(mFeed.Url, "gallery.xml"));
                    allalbums[post.Topic.Name] = albums;
                }
                
                XmlNode albumsNode = albums.SelectSingleNode("/gallery/albums");

                XmlNode album = albums.CreateNode(XmlNodeType.Element, "album", string.Empty);
                albumsNode.AppendChild(album);
                
                XmlAttribute imagesFolder = albums.CreateAttribute("imagesFolder");
                imagesFolder.Value = string.Format("galleries/{0}/images/", post.Id);
                album.Attributes.Append(imagesFolder);

                string imagesFolderPath = Path.Combine(mFeed.Url, string.Format("{0}/images/", post.Id));
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                XmlAttribute thumbnailsFolder = albums.CreateAttribute("thumbnailsFolder");
                thumbnailsFolder.Value = string.Format("galleries/{0}/thumbnails/", post.Id);
                album.Attributes.Append(thumbnailsFolder);

                string thumbnailsFolderPath = Path.Combine(mFeed.Url, string.Format("{0}/thumbnails/", post.Id));
                if (!Directory.Exists(thumbnailsFolderPath))
                {
                    Directory.CreateDirectory(thumbnailsFolderPath);
                }

                XmlAttribute description = albums.CreateAttribute("description");
                description.Value = post.Title;
                album.Attributes.Append(description);

                string postImagePath = (string) ConfigurationManager.AppSettings["images"];

                foreach (PostImage postImage in post.PostImages)
                {
                    XmlNode image = albums.CreateNode(XmlNodeType.Element, "image", string.Empty);

                    XmlAttribute imageName = albums.CreateAttribute("name");
                    imageName.Value = postImage.Image.Name;
                    image.Attributes.Append(imageName);

                    XmlAttribute imageThumbnail = albums.CreateAttribute("thumbnail");
                    imageThumbnail.Value = postImage.Image.Name;
                    image.Attributes.Append(imageThumbnail);

                    XmlAttribute imageDescription = albums.CreateAttribute("description");
                    imageDescription.Value = postImage.Image.Name;
                    image.Attributes.Append(imageDescription);

                    File.Copy(
                        Path.Combine(Path.Combine(postImagePath, postImage.Image.Path), postImage.Image.Name),
                        Path.Combine(imagesFolderPath, postImage.Image.Name),
                        true);

                    File.WriteAllBytes(
                        Path.Combine(thumbnailsFolderPath, postImage.Image.Name),
                        postImage.Image.Thumbnail);

                    album.AppendChild(image);
                }

                Dictionary<string, XmlDocument>.Enumerator allalbumsenum = allalbums.GetEnumerator();
                while (allalbumsenum.MoveNext())
                {
                    allalbumsenum.Current.Value.Save(Path.Combine(mFeed.Url,
                        string.Format(@"{0}.xml", allalbumsenum.Current.Key)));
                }
            }

            return posts.Count;
        }

        public override void DeleteFeedItems(ISession session)
        {
            throw new NotImplementedException();
        }
    }
}
