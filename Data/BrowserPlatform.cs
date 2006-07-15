using System;
namespace DBlog.Data
{

    ///--------------------------------------------------------------------------------
    ///<summary>
    ///Persistent domain entity class representing 'BrowserPlatform' entities.
    ///</summary>
    ///<remarks>
    ///
    ///Mapping information:
    ///This class maps to the 'BrowserPlatform' table in the data source.
    ///</remarks>
    ///--------------------------------------------------------------------------------
    public class BrowserPlatform
    {
#region " Generated Code Region "
        //Private field variables

        //Holds property values
        private System.Int32 m_Id;
        private Browser m_Browser;
        private System.Collections.IList m_BrowserVersionPlatforms;
        private Platform m_Platform;

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
        ///The property maps to the column 'BrowserPlatform_Id' in the data source.
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
        ///This property accepts references to objects of the type 'Browser'.
        ///This property is part of a 'OneToMany' relationship.
        ///The inverse property for this property is 'Browser.BrowserPlatforms'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_Browser' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'Browser_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  Browser Browser
        {
            get
            {
                return m_Browser;
            }
            set
            {
                m_Browser = value;
            }
        }

        ///--------------------------------------------------------------------------------
        ///<summary>
        ///Persistent many-one reference property.
        ///</summary>
        ///<remarks>
        ///This property accepts multiple references to objects of the type 'BrowserVersionPlatform'.
        ///This property is part of a 'ManyToOne' relationship.
        ///The data type for this property is 'System.Collections.IList'.
        ///The inverse property for this property is 'BrowserVersionPlatform.BrowserPlatform'.
        ///This property inherits its mapping information from its inverse property.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_BrowserVersionPlatforms' that holds the value for this property is 'PrivateAccess'.
        ///This property is marked as slave.
        ///
        ///Mapping information:
        ///This class maps to the 'BrowserVersionPlatform' table in the data source.
        ///The property maps to the identity column 'BrowserPlatform_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  System.Collections.IList BrowserVersionPlatforms
        {
            get
            {
                return m_BrowserVersionPlatforms;
            }
            set
            {
                m_BrowserVersionPlatforms = value;
            }
        }

        ///--------------------------------------------------------------------------------
        ///<summary>
        ///Persistent one-many reference property.
        ///</summary>
        ///<remarks>
        ///This property accepts references to objects of the type 'Platform'.
        ///This property is part of a 'OneToMany' relationship.
        ///The inverse property for this property is 'Platform.BrowserPlatforms'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_Platform' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'Platform_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  Platform Platform
        {
            get
            {
                return m_Platform;
            }
            set
            {
                m_Platform = value;
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
