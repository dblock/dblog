using System;
using System.Collections.Generic;
using System.Text;
using DBlog.Data;
using NHibernate;
using NHibernate.Expression;
using DBlog.Data.Hibernate;

namespace DBlog.TransitData
{
    public class TransitEntryImageQueryOptions : WebServiceQueryOptions
    {
        private int mEntryId = 0;

        public int EntryId
        {
            get
            {
               return mEntryId;}
            set
            {
               mEntryId = value;}
        }

        public TransitEntryImageQueryOptions()
        {
        }

        public TransitEntryImageQueryOptions(
            int entryid)
        {
            mEntryId = entryid;
        }

        public TransitEntryImageQueryOptions(
            int entryid,
            int pagesize,
            int pagenumber)
            : base(pagesize, pagenumber)
        {
            mEntryId = entryid;
        }

        public override void Apply(ICriteria criteria)
        {
            if (EntryId != 0)
            {
                criteria.Add(Expression.Eq("Entry.Id", EntryId));
            }

            base.Apply(criteria);
        }

        public override void Apply(CountQuery query)
        {
            if (EntryId != 0)
            {
                query.Add(Expression.Eq("Entry.Id", EntryId));
            }

            base.Apply(query);
        }
    }


    public class TransitEntryImage : TransitObject
    {
        private int mImageId;

        public int ImageId
        {
            get
            {
                return mImageId;
            }
            set
            {
                mImageId = value;
            }
        }
       
        private int mEntryId;

        public int EntryId
        {
            get
            {
                return mEntryId;
            }
            set
            {
                mEntryId = value;
            }
        }

        public TransitEntryImage()
        {

        }

        public TransitEntryImage(DBlog.Data.EntryImage o)
            : base(o.Id)
        {
            EntryId = o.Entry.Id;
            ImageId = o.Image.Id;
        }

        public EntryImage GetEntryImage(ISession session)
        {
            EntryImage ei = (Id != 0) ? (EntryImage)session.Load(typeof(EntryImage), Id) : new EntryImage();
            ei.Image = (Image)session.Load(typeof(Image), ImageId);
            ei.Entry = (Entry)session.Load(typeof(Entry), EntryId);
            return ei;
        }
    }
}
