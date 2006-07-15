using System;
namespace DBlog.Data
{

    ///--------------------------------------------------------------------------------
    ///<summary>
    ///Persistent domain entity class representing 'BrowserVersionPlatform' entities.
    ///</summary>
    ///<remarks>
    ///
    ///Mapping information:
    ///This class maps to the 'BrowserVersionPlatform' table in the data source.
    ///</remarks>
    ///--------------------------------------------------------------------------------
    public class BrowserVersionPlatform
    {
#region " Generated Code Region "
        //Private field variables

        //Holds property values
        private System.Int32 m_Id;
        private BrowserPlatform m_BrowserPlatform;
        private BrowserVersion m_BrowserVersion;
        private System.Collections.IList m_Requests;
        private System.Collections.IList m_RollupBrowserVersionPlatforms;

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
        ///The property maps to the column 'BrowserVersionPlatform_Id' in the data source.
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
        ///This property accepts references to objects of the type 'BrowserPlatform'.
        ///This property is part of a 'OneToMany' relationship.
        ///The inverse property for this property is 'BrowserPlatform.BrowserVersionPlatforms'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_BrowserPlatform' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'BrowserPlatform_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  BrowserPlatform BrowserPlatform
        {
            get
            {
                return m_BrowserPlatform;
            }
            set
            {
                m_BrowserPlatform = value;
            }
        }

        ///--------------------------------------------------------------------------------
        ///<summary>
        ///Persistent one-many reference property.
        ///</summary>
        ///<remarks>
        ///This property accepts references to objects of the type 'BrowserVersion'.
        ///This property is part of a 'OneToMany' relationship.
        ///The inverse property for this property is 'BrowserVersion.BrowserVersionPlatforms'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_BrowserVersion' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'BrowserVersion_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  BrowserVersion BrowserVersion
        {
            get
            {
                return m_BrowserVersion;
            }
            set
            {
                m_BrowserVersion = value;
            }
        }

        ///--------------------------------------------------------------------------------
        ///<summary>
        ///Persistent many-one reference property.
        ///</summary>
        ///<remarks>
        ///This property accepts multiple references to objects of the type 'Request'.
        ///This property is part of a 'ManyToOne' relationship.
        ///The data type for this property is 'System.Collections.IList'.
        ///The inverse property for this property is 'Request.BrowserVersionPlatform'.
        ///This property inherits its mapping information from its inverse property.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_Requests' that holds the value for this property is 'PrivateAccess'.
        ///This property is marked as slave.
        ///
        ///Mapping information:
        ///This class maps to the 'Request' table in the data source.
        ///The property maps to the identity column 'BrowserVersionPlatform_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  System.Collections.IList Requests
        {
            get
            {
                return m_Requests;
            }
            set
            {
                m_Requests = value;
            }
        }

        ///--------------------------------------------------------------------------------
        ///<summary>
        ///Persistent many-one reference property.
        ///</summary>
        ///<remarks>
        ///This property accepts multiple references to objects of the type 'RollupBrowserVersionPlatform'.
        ///This property is part of a 'ManyToOne' relationship.
        ///The data type for this property is 'System.Collections.IList'.
        ///The inverse property for this property is 'RollupBrowserVersionPlatform.BrowserVersionPlatform'.
        ///This property inherits its mapping information from its inverse property.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_RollupBrowserVersionPlatforms' that holds the value for this property is 'PrivateAccess'.
        ///This property is marked as slave.
        ///
        ///Mapping information:
        ///This class maps to the 'RollupBrowserVersionPlatform' table in the data source.
        ///The property maps to the identity column 'BrowserVersionPlatform_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  System.Collections.IList RollupBrowserVersionPlatforms
        {
            get
            {
                return m_RollupBrowserVersionPlatforms;
            }
            set
            {
                m_RollupBrowserVersionPlatforms = value;
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
