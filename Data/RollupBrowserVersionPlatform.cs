using System;
namespace DBlog.Data
{

    ///--------------------------------------------------------------------------------
    ///<summary>
    ///Persistent domain entity class representing 'RollupBrowserVersionPlatform' entities.
    ///</summary>
    ///<remarks>
    ///
    ///Mapping information:
    ///This class maps to the 'RollupBrowserVersionPlatform' table in the data source.
    ///</remarks>
    ///--------------------------------------------------------------------------------
    public class RollupBrowserVersionPlatform
    {
#region " Generated Code Region "
        //Private field variables

        //Holds property values
        private System.Int32 m_Id;
        private BrowserVersionPlatform m_BrowserVersionPlatform;
        private System.Int64 m_RequestCount;

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
        ///The property maps to the column 'RollupBrowserVersionPlatform_Id' in the data source.
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
        ///The inverse property for this property is 'BrowserVersionPlatform.RollupBrowserVersionPlatforms'.
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
        ///This property accepts values of the type 'System.Int64'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_RequestCount' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'RequestCount' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  System.Int64 RequestCount
        {
            get
            {
                return m_RequestCount;
            }
            set
            {
                m_RequestCount = value;
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
