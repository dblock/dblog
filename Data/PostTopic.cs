﻿using System;
namespace DBlog.Data
{

    ///--------------------------------------------------------------------------------
    ///<summary>
    ///Persistent domain entity class representing 'PostTopic' entities.
    ///</summary>
    ///<remarks>
    ///
    ///Mapping information:
    ///This class maps to the 'PostTopic' table in the data source.
    ///</remarks>
    ///--------------------------------------------------------------------------------
    public class PostTopic
    {
#region " Generated Code Region "
        //Private field variables

        //Holds property values
        private System.Int32 m_Id;
        private Post m_Post;
        private Topic m_Topic;

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
        ///The property maps to the column 'PostTopic_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public virtual System.Int32 Id
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
        ///This property accepts references to objects of the type 'Post'.
        ///This property is part of a 'OneToMany' relationship.
        ///The inverse property for this property is 'Post.PostTopics'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_Post' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'Post_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public virtual Post Post
        {
            get
            {
                return m_Post;
            }
            set
            {
                m_Post = value;
            }
        }

        ///--------------------------------------------------------------------------------
        ///<summary>
        ///Persistent one-many reference property.
        ///</summary>
        ///<remarks>
        ///This property accepts references to objects of the type 'Topic'.
        ///This property is part of a 'OneToMany' relationship.
        ///The inverse property for this property is 'Topic.PostTopics'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_Topic' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'Topic_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public virtual Topic Topic
        {
            get
            {
                return m_Topic;
            }
            set
            {
                m_Topic = value;
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
