using System;
namespace DBlog.Data
{

    ///--------------------------------------------------------------------------------
    ///<summary>
    ///Persistent domain entity class representing 'Thread' entities.
    ///</summary>
    ///<remarks>
    ///
    ///Mapping information:
    ///This class maps to the 'Thread' table in the data source.
    ///</remarks>
    ///--------------------------------------------------------------------------------
    public class Thread
    {
#region " Generated Code Region "
        //Private field variables

        //Holds property values
        private System.Int32 m_Id;
        private Comment m_Comment;
        private Comment m_ParentComment;

        //Public properties
        ///--------------------------------------------------------------------------------
        ///<summary>
        ///Persistent primitive identity property.
        ///</summary>
        ///<remarks>
        ///This property is an identity property.
        ///The identity index for this property is '0'.
        ///This property accepts values of the type 'System.Int32'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_Id' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'Thread_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  System.Int32 Id
        {
            get
            {
                return m_Id;
            }
        }

        ///--------------------------------------------------------------------------------
        ///<summary>
        ///Persistent one-many reference property.
        ///</summary>
        ///<remarks>
        ///This property accepts references to objects of the type 'Comment'.
        ///This property is part of a 'OneToMany' relationship.
        ///The inverse property for this property is 'Comment.Threads'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_Comment' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'Comment_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  Comment Comment
        {
            get
            {
                return m_Comment;
            }
            set
            {
                m_Comment = value;
            }
        }

        ///--------------------------------------------------------------------------------
        ///<summary>
        ///Persistent one-many reference property.
        ///</summary>
        ///<remarks>
        ///This property accepts references to objects of the type 'Comment'.
        ///This property is part of a 'OneToMany' relationship.
        ///The inverse property for this property is 'Comment.ParentCommentThreads'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_ParentComment' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'ParentComment_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  Comment ParentComment
        {
            get
            {
                return m_ParentComment;
            }
            set
            {
                m_ParentComment = value;
            }
        }

#endregion //Generated Code Region

        //Add your synchronized custom code here:
#region " Synchronized Custom Code Region "
#endregion //Synchronized Custom Code Region

        //Add your unsynchronized custom code here:
#region " Unsynchronized Custom Code Region "



#endregion //Unsynchronized Custom Code Region

    }
}
