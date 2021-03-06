using System;
using NUnit.Framework;
using DBlog.Data;
using NHibernate;
using NHibernate.Cfg;
using System.Collections;

namespace DBlog.Data.Hibernate.UnitTests
{
    public abstract class NHibernateCrudTest : NHibernateTest
    {
        private ArrayList m_DependentObjects = new ArrayList();

        public abstract object Object { get; }

        public NHibernateCrudTest()
        {

        }

        public void AddDependentObject(NHibernateCrudTest test)
        {
            m_DependentObjects.AddRange(test.m_DependentObjects);
            m_DependentObjects.Add(test.Object);
        }

        public void AddDependentObject(object o)
        {
            m_DependentObjects.Add(o);
        }

        public int Create()
        {
            Console.WriteLine("Create");
            Console.WriteLine(" Creating " + Object.ToString());
            SaveDependentObjects();
            int id = SaveObject(Object);
            Session.Flush();
            return id;
        }

        public void Delete()
        {
            DeleteObject(Object);
            DeleteDependentObjects();
            Session.Flush();
        }

        [Test]
        public void CreateAndDelete()
        {
            Create();
            Delete();
        }

        [Test]
        public void RetrieveAndUpdate()
        {
            Console.WriteLine("RetrieveAndUpdate");
            Console.WriteLine(" Creating " + Object.ToString());
            SaveDependentObjects();
            SaveObject(Object);
            Session.Flush();
            Console.WriteLine(" Retrieving " + Object.ToString());
            object o = Session.Get(Object.GetType(), Object.GetType().GetProperty("Id").GetValue(Object, null));
            Console.WriteLine(" Retrieved " + o.ToString());
            Session.Update(o, Object.GetType().GetProperty("Id").GetValue(Object, null));
            Session.Flush();
            DeleteObject(Object);
            DeleteDependentObjects();
            Session.Flush();
        }

        protected virtual void DeleteDependentObjects()
        {
            for (int i = m_DependentObjects.Count - 1; i >= 0; i--)
            {
                DeleteObject(m_DependentObjects[i]);
            }
        }

        protected virtual void SaveDependentObjects()
        {
            foreach (object obj in m_DependentObjects)
            {
                SaveObject(obj);
            }
        }

        private void DeleteObject(object obj)
        {
            Console.WriteLine(string.Format("  Deleting {0}: {1}", obj.ToString(),
                obj.GetType().GetProperty("Id").GetValue(obj, null)));

            Session.Delete(obj);
        }

        private int SaveObject(object obj)
        {
            Console.Write(string.Format("  Creating {0}", obj.ToString()));
            Session.Save(obj);
            int id = (int) obj.GetType().GetProperty("Id").GetValue(obj, null);
            Console.WriteLine(string.Format(": {0}", id));
            return id;
        }
    }
}
