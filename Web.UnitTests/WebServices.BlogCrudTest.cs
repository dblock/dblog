using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Web.Services.Protocols;
using DBlog.Web.UnitTests.WebServices.Blog;
using System.Reflection;

namespace DBlog.Web.UnitTests.WebServices
{
    public abstract class BlogCrudTest : BlogTest
    {
        public abstract string ObjectType { get; }
        public abstract TransitObject TransitInstance { get; }

        private string mTicket = string.Empty;

        public string Ticket
        {
            get
            {
                if (string.IsNullOrEmpty(mTicket))
                {
                    mTicket = Blog.Login("Administrator", string.Empty);
                }

                return mTicket;
            }
        }

        public virtual int Create()
        {
            string method = string.Format("CreateOrUpdate{0}", ObjectType);
            Console.Write(string.Format("{0}: ", method));
            object[] args = { Ticket, TransitInstance };
            TransitInstance.Id = (int) Blog.GetType().InvokeMember(method, BindingFlags.InvokeMethod, null, Blog, args);
            Console.WriteLine(TransitInstance.Id);
            return TransitInstance.Id;
        }

        public virtual void Delete()
        {
            string method = string.Format("Delete{0}", ObjectType);
            Console.WriteLine(string.Format("{0}: {1}", method, TransitInstance.Id));
            object[] args = { Ticket, TransitInstance.Id };
            Blog.GetType().InvokeMember(method, BindingFlags.InvokeMethod, null, Blog, args);
        }

        public int Count()
        {
            string method = string.Format("Get{0}sCount", ObjectType);
            Console.Write(string.Format("{0}: ", method));
            object[] args = { Ticket };
            int count = (int) Blog.GetType().InvokeMember(method, BindingFlags.InvokeMethod, null, Blog, args);
            Console.WriteLine(string.Format("{0}", count));
            return count;
        }

        public TransitObject Retrieve(int id)
        {
            string method = string.Format("Get{0}ById", ObjectType);
            Console.Write(string.Format("{0}: {1} -> ", method, id));
            object[] args = { Ticket, id };
            TransitObject to = (TransitObject) Blog.GetType().InvokeMember(method, BindingFlags.InvokeMethod, null, Blog, args);
            Console.WriteLine(string.Format("{0}: {1}", to.ToString(), to.Id));
            return to;
        }

        [Test]
        public void TestCrud()
        {
            int id = Create();
            int count = Count();
            Assert.IsTrue(count > 0);
            TransitObject to = Retrieve(id);
            Assert.AreEqual(to.Id, id);
            TransitInstance.Id = id;
            int id2 = Create();
            Assert.AreEqual(id, id2);
            Delete();
        }
    }
}