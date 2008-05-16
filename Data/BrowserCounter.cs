using System;
namespace DBlog.Data
{

    ///--------------------------------------------------------------------------------
    ///<summary>
    ///Persistent domain entity class representing 'BrowserCounter' entities.
    ///</summary>
    ///<remarks>
    ///
    ///Mapping information:
    ///This class maps to the 'BrowserCounter' table in the data source.
    ///</remarks>
    ///--------------------------------------------------------------------------------
    public class BrowserCounter
    {
#region " Generated Code Region "
        //Private field variables

        //Holds property values
        private System.Int32 m_Id;
        private Browser m_Browser;
        private Counter m_Counter;

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
        ///The property maps to the column 'BrowserCounter_Id' in the data source.
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
        ///The inverse property for this property is 'Browser.BrowserCounters'.
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
        ///Persistent one-many reference property.
        ///</summary>
        ///<remarks>
        ///This property accepts references to objects of the type 'Counter'.
        ///This property is part of a 'OneToMany' relationship.
        ///The inverse property for this property is 'Counter.BrowserCounters'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_Counter' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'Counter_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  Counter Counter
        {
            get
            {
                return m_Counter;
            }
            set
            {
                m_Counter = value;
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
