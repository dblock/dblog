using System;
using NUnit.Framework;
using DBlog.Web.UnitTests.WebServices;
using System.Net;
using System.IO;
using DBlog.Tools.Web.Html;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using Argotic.Extensions.Core;
using Argotic.Syndication;
using DBlog.Tools.Drawing;
using DBlog.Web.UnitTests.WebServices.Blog;

namespace DBlog.Web.UnitTests
{
    [TestFixture]
    public class AtomPubTest : BlogTest
    {
        [Test]
        public void DefaultAspxLinksTest()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost/DBlog/Default.aspx");
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream s = response.GetResponseStream();
            StreamReader sr = new StreamReader(s);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK,
                string.Format("Response code was {0}", response.StatusCode));

            // look for link rel="service" type="application/atomsvc+xml"
            List<HtmlUri> uris = HtmlUriExtractor.Extract(sr.ReadToEnd());
            HtmlUri linkRelAtomSvc = null;
            foreach (HtmlUri uri in uris)
            {
                if (uri.Control.TagName == "link")
                {
                    string rel = uri.Control.Attributes["rel"];
                    string type = uri.Control.Attributes["type"];

                    if (rel == "service" && type == "application/atomsvc+xml")
                    {
                        linkRelAtomSvc = uri;
                        break;
                    }
                }
            }

            Assert.IsNotNull(linkRelAtomSvc);
            Console.WriteLine(linkRelAtomSvc.Uri);
        }

        [Test]
        public void AtomSvcTest()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost/DBlog/AtomSvc.aspx");
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            XmlDocument atomSvcXml = new XmlDocument();
            XmlTextReader r = new XmlTextReader(response.GetResponseStream());
            atomSvcXml.Load(r);
            XmlNamespaceManager xmlnsmgr = new XmlNamespaceManager(atomSvcXml.NameTable);
            xmlnsmgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            xmlnsmgr.AddNamespace("app", "http://www.w3.org/2007/app");
            Assert.AreEqual(1, atomSvcXml.SelectNodes("/app:service", xmlnsmgr).Count);
            Assert.AreEqual(1, atomSvcXml.SelectNodes("/app:service/app:workspace", xmlnsmgr).Count);
            Assert.AreEqual(2, atomSvcXml.SelectNodes("/app:service/app:workspace/app:collection", xmlnsmgr).Count);
            // the first collection is for posts
            XmlNode postsCollection = atomSvcXml.SelectNodes("/app:service/app:workspace/app:collection", xmlnsmgr)[0];
            string postsCollectionTitle = postsCollection.SelectSingleNode("atom:title", xmlnsmgr).InnerText;
            Assert.AreEqual("Posts", postsCollectionTitle);
            Assert.AreEqual(1, postsCollection.SelectNodes("app:accept", xmlnsmgr).Count);
            Assert.AreEqual("application/atom+xml;type=entry", postsCollection.SelectNodes("app:accept", xmlnsmgr)[0].InnerText);
            // posts collection contains categories
            XmlNode categories = postsCollection.SelectSingleNode("app:categories", xmlnsmgr);
            Assert.IsNotNull(categories);
            Assert.AreEqual(categories.ChildNodes.Count, Blog.GetTopicsCount(Ticket, null));
            // the second collection is for images
            XmlNode imagesCollection = atomSvcXml.SelectNodes("/app:service/app:workspace/app:collection", xmlnsmgr)[1];
            string imagesCollectionTitle = imagesCollection.SelectSingleNode("atom:title", xmlnsmgr).InnerText;
            Assert.AreEqual("Images", imagesCollectionTitle);
            Assert.AreEqual(3, imagesCollection.SelectNodes("app:accept", xmlnsmgr).Count);
            Assert.AreEqual("image/jpeg", imagesCollection.SelectNodes("app:accept", xmlnsmgr)[0].InnerText);
            Assert.AreEqual("image/gif", imagesCollection.SelectNodes("app:accept", xmlnsmgr)[1].InnerText);
            Assert.AreEqual("image/png", imagesCollection.SelectNodes("app:accept", xmlnsmgr)[2].InnerText);
        }

        [Test, ExpectedException(typeof(WebException), ExpectedMessage = "The remote server returned an error: (401) Unauthorized.")]
        public void AtomPostNeedsAuthorizationTest()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost/DBlog/AtomPost.aspx");
            request.Method = "POST";
            request.ContentType = "text/xml";
            XmlDocument postXml = new XmlDocument();
            postXml.LoadXml("<entry />");
            byte[] postData = Encoding.UTF8.GetBytes(postXml.OuterXml);
            request.ContentLength = postData.Length;
            request.GetRequestStream().Write(postData, 0, postData.Length);
            request.GetRequestStream().Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }

        [Test, ExpectedException(typeof(WebException), ExpectedMessage = "The remote server returned an error: (401) Unauthorized.")]
        public void AtomImageNeedsAuthorizationTest()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost/DBlog/AtomImage.aspx");
            request.Method = "POST";
            request.ContentType = "image/jpg";
            byte[] postData = Guid.NewGuid().ToByteArray();
            request.ContentLength = postData.Length;
            request.GetRequestStream().Write(postData, 0, postData.Length);
            request.GetRequestStream().Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }

        [Test]
        public void AtomNewPostTest()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost/DBlog/AtomPost.aspx");
            request.Method = "POST";
            request.ContentType = "application/atom+xml;type=entry";
            string usernamePassword = string.Format("Administrator:");
            string basicAuthorization = string.Format("Basic {0}", Convert.ToBase64String(
                Encoding.ASCII.GetBytes(usernamePassword)));
            request.Headers.Add("Authorization", basicAuthorization);
            XmlDocument postXml = new XmlDocument();
            postXml.LoadXml(
                "<?xml version=\"1.0\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\">" +
                  "<title>Atom-Powered Robots Run Amok</title>" +
                  "<id>urn:uuid:1225c695-cfb8-4ebb-aaaa-80da344efa6a</id>" +
                  "<updated>2003-12-13T18:30:02Z</updated>" +
                  "<author><name>John Doe</name></author>" +
                  "<content>Some text.</content>" +
                "</entry>");
            byte[] postData = Encoding.UTF8.GetBytes(postXml.OuterXml);
            request.ContentLength = postData.Length;
            request.GetRequestStream().Write(postData, 0, postData.Length);
            request.GetRequestStream().Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseLocation = response.Headers["Location"];
            Assert.IsNotEmpty(responseLocation);
            Console.WriteLine(responseLocation);
            // atom entry returned
            AtomEntry atomEntry = new AtomEntry();
            atomEntry.Load(response.GetResponseStream());
            Console.WriteLine(atomEntry.Id.Uri);
            int id = int.Parse(atomEntry.Id.Uri.ToString().Substring(atomEntry.Id.Uri.ToString().LastIndexOf("/") + 1));
            Assert.IsTrue(id > 0);
            Console.WriteLine(string.Format("Id: {0}", id));
            Blog.DeletePost(Ticket, id);
        }

        [Test]
        public void AtomNewImageTest()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost/DBlog/AtomImage.aspx");
            request.Method = "POST";
            request.ContentType = "image/jpg";
            string usernamePassword = string.Format("Administrator:");
            string basicAuthorization = string.Format("Basic {0}", Convert.ToBase64String(
                Encoding.ASCII.GetBytes(usernamePassword)));
            request.Headers.Add("Authorization", basicAuthorization);
            byte[] postData = ThumbnailBitmap.GetBitmapDataFromText("x", 72, 100, 150);
            request.ContentLength = postData.Length;
            request.GetRequestStream().Write(postData, 0, postData.Length);
            request.GetRequestStream().Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseLocation = response.Headers["Location"];
            Assert.IsNotEmpty(responseLocation);
            Console.WriteLine(responseLocation);
            // atom entry returned
            AtomEntry atomEntry = new AtomEntry();
            atomEntry.Load(response.GetResponseStream());
            Console.WriteLine(atomEntry.Id.Uri);
            int id = int.Parse(atomEntry.Id.Uri.ToString().Substring(atomEntry.Id.Uri.ToString().LastIndexOf("/") + 1));
            Assert.IsTrue(id > 0);
            Console.WriteLine(string.Format("Id: {0}", id));
            Blog.DeleteImage(Ticket, id);
        }

        [Test]
        public void AtomUpdateImageTest()
        {
            // create image
            TransitImage t_image = new TransitImage();
            t_image.Data = ThumbnailBitmap.GetBitmapDataFromText("x", 72, 100, 150);
            t_image.Name = Guid.NewGuid().ToString();
            t_image.Id = Blog.CreateOrUpdateImage(Ticket, t_image);
            // update image
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
                string.Format("http://localhost/DBlog/AtomImage.aspx?id={0}", t_image.Id));
            request.Method = "PUT";
            request.ContentType = "image/jpg";
            string usernamePassword = string.Format("Administrator:");
            string basicAuthorization = string.Format("Basic {0}", Convert.ToBase64String(
                Encoding.ASCII.GetBytes(usernamePassword)));
            request.Headers.Add("Authorization", basicAuthorization);
            request.ContentLength = t_image.Data.Length;
            request.GetRequestStream().Write(t_image.Data, 0, t_image.Data.Length);
            request.GetRequestStream().Close();
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, string.Format("Response code was {0}", response.StatusCode));
            Blog.DeleteImage(Ticket, t_image.Id);
        }

        [Test]
        protected void AtomUpdateImageMetadataTest()
        {
            /*
                The client can edit the metadata for the picture. First GET the Media Link Entry:

                GET /media/edit/the_beach.atom HTTP/1.1
                Host: example.org
                Authorization: Basic ZGFmZnk6c2VjZXJldA== The Media Link Entry is returned.


                HTTP/1.1 200 Ok 
                Date: Fri, 7 Oct 2005 17:18:11 GMT
                Content-Length: nnn
                Content-Type: application/atom+xml;type=entry;charset="utf-8"
                ETag: "c181bb840673b5"  

                <?xml version="1.0"?>
                <entry xmlns="http://www.w3.org/2005/Atom">
                  <title>The Beach</title>
                  <id>urn:uuid:1225c695-cfb8-4ebb-aaaa-80da344efa6a</id>
                  <updated>2005-10-07T17:17:08Z</updated>
                  <author><name>Daffy</name></author>
                  <summary type="text" />
                  <content type="image/png"
                     src="http://media.example.org/the_beach.png"/>
                  <link rel="edit-media"
                     href="http://media.example.org/edit/the_beach.png" />
                  <link rel="edit"
                     href="http://example.org/media/edit/the_beach.atom" />
                </entry> The metadata can be updated, in this case to add a summary, and then PUT back to the server.


                PUT /media/edit/the_beach.atom HTTP/1.1
                Host: example.org
                Authorization: Basic ZGFmZnk6c2VjZXJldA==
                Content-Type: application/atom+xml;type=entry
                Content-Length: nnn
                If-Match: "c181bb840673b5"  

                <?xml version="1.0"?>
                <entry xmlns="http://www.w3.org/2005/Atom">
                  <title>The Beach</title>
                  <id>urn:uuid:1225c695-cfb8-4ebb-aaaa-80da344efa6a</id>
                  <updated>2005-10-07T17:17:08Z</updated>
                  <author><name>Daffy</name></author>
                  <summary type="text">
                      A nice sunset picture over the water.
                  </summary>
                  <content type="image/png"
                     src="http://media.example.org/the_beach.png"/>
                  <link rel="edit-media"
                     href="http://media.example.org/edit/the_beach.png" />
                  <link rel="edit"
                     href="http://example.org/media/edit/the_beach.atom" />
                </entry> The update was successful.


                HTTP/1.1 200 Ok 
                Date: Fri, 7 Oct 2005 17:19:11 GMT
                Content-Length: 0 
            */
        }

        [Test]
        protected void AtomMultipleImageTest()
        {
            /*
             Multiple Media Resources can be added to the Collection.

                POST /edit/ HTTP/1.1
                Host: media.example.org
                Content-Type: image/png
                Slug: The Pier 
                Authorization: Basic ZGFmZnk6c2VjZXJldA==
                Content-Length: nnn

                ...binary data... The Resource is created successfully.


                HTTP/1.1 201 Created
                Date: Fri, 7 Oct 2005 17:17:11 GMT
                Content-Length: nnn
                Content-Type: application/atom+xml;type=entry;charset="utf-8"
                Location: http://example.org/media/edit/the_pier.atom

                <?xml version="1.0"?>
                <entry xmlns="http://www.w3.org/2005/Atom">
                  <title>The Pier</title>
                  <id>urn:uuid:1225c695-cfb8-4ebb-aaaa-80da344efe6b</id>
                  <updated>2005-10-07T17:26:43Z</updated>
                  <author><name>Daffy</name></author>
                  <summary type="text" />
                  <content type="image/png"
                     src="http://media.example.org/the_pier.png"/>
                  <link rel="edit-media"
                     href="http://media.example.org/edit/the_pier.png" />
                  <link rel="edit"
                     href="http://example.org/media/edit/the_pier.atom" />
                </entry> 
             
             */
        }
    }
}
