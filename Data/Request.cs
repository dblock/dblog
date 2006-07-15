using System;
namespace DBlog.Data
{

    ///--------------------------------------------------------------------------------
    ///<summary>
    ///Persistent domain entity class representing 'Request' entities.
    ///</summary>
    ///<remarks>
    ///
    ///Mapping information:
    ///This class maps to the 'Request' table in the data source.
    ///</remarks>
    ///--------------------------------------------------------------------------------
    public class Request
    {
#region " Generated Code Region "
        //Private field variables

        //Holds property values
        private System.Int32 m_Id;
        private BrowserVersionPlatform m_BrowserVersionPlatform;
        private System.DateTime m_DateTime;
        private System.String m_IpAddress;

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
        ///The property maps to the column 'Request_Id' in the data source.
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
        ///This property accepts references to objects of the type 'BrowserVersionPlatform'.
        ///This property is part of a 'OneToMany' relationship.
        ///The inverse property for this property is 'BrowserVersionPlatform.Requests'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_BrowserVersionPlatform' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'BrowserVersionPlatform_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  BrowserVersionPlatform BrowserVersionPlatform
        {
            get
            {
                return m_BrowserVersionPlatform;
            }
            set
            {
                m_BrowserVersionPlatform = value;
            }
        }

        ///--------------------------------------------------------------------------------
        ///<summary>
        ///Persistent primitive property.
        ///</summary>
        ///<remarks>
        ///This property accepts values of the type 'System.DateTime'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_DateTime' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'DateTime' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  System.DateTime DateTime
        {
            get
            {
                return m_DateTime;
            }
            set
            {
                m_DateTime = value;
            }
        }

        ///--------------------------------------------------------------------------------
        ///<summary>
        ///Persistent primitive property.
        ///</summary>
        ///<remarks>
        ///This property accepts values of the type 'System.String'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_IpAddress' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'IpAddress' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  System.String IpAddress
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

#endregion //Generated Code Region

        //Add your synchronized custom code here:
#region " Synchronized Custom Code Region "
#endregion //Synchronized Custom Code Region

        //Add your unsynchronized custom code here:
#region " Unsynchronized Custom Code Region "



#endregion //Unsynchronized Custom Code Region

    }
}
