using System;

namespace DBlog.Data.Hibernate
{
    public class AssociatedComment
    {
        public AssociatedComment()
        {

        }

        private System.Int32 m_Id;

        public virtual System.Int32 Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                m_Id = value;
            }
        }

        private System.Int32 m_AssociatedId;

        public virtual System.Int32 AssociatedId
        {
            get
            {
                return m_AssociatedId;
            }
            set
            {
                m_AssociatedId = value;
            }
        }

        private System.String m_Type;

        public virtual System.String Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }

        private System.String m_IpAddress;

        public virtual System.String IpAddress
        {
            get
            {
                return m_IpAddress;
            }
            set
            {
                m_IpAddress = value;
            }
        }

        private System.String m_Text;

        public virtual System.String Text
        {
            get
            {
                return m_Text;
            }
            set
            {
                m_Text = value;
            }
        }

        private System.DateTime m_Created;

        public virtual System.DateTime Created
        {
            get
            {
                return m_Created;
            }
            set
            {
                m_Created = value;
            }
        }

        private System.DateTime m_Modified;

        public virtual System.DateTime Modified
        {
            get
            {
                return m_Modified;
            }
            set
            {
                m_Modified = value;
            }
        }
    }
}