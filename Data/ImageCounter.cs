using System;
namespace DBlog.Data
{

    ///--------------------------------------------------------------------------------
    ///<summary>
    ///Persistent domain entity class representing 'ImageCounter' entities.
    ///</summary>
    ///<remarks>
    ///
    ///Mapping information:
    ///This class maps to the 'ImageCounter' table in the data source.
    ///</remarks>
    ///--------------------------------------------------------------------------------
    public class ImageCounter
    {
#region " Generated Code Region "
        //Private field variables

        //Holds property values
        private System.Int32 m_Id;
        private Counter m_Counter;
        private Image m_Image;

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
        ///The property maps to the column 'ImageCounter_Id' in the data source.
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
        ///This property accepts references to objects of the type 'Counter'.
        ///This property is part of a 'OneToMany' relationship.
        ///The inverse property for this property is 'Counter.ImageCounters'.
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

        ///--------------------------------------------------------------------------------
        ///<summary>
        ///Persistent one-many reference property.
        ///</summary>
        ///<remarks>
        ///This property accepts references to objects of the type 'Image'.
        ///This property is part of a 'OneToMany' relationship.
        ///The inverse property for this property is 'Image.ImageCounters'.
        ///The accessibility level for this property is 'PublicAccess'.
        ///The accessibility level for the field 'm_Image' that holds the value for this property is 'PrivateAccess'.
        ///
        ///Mapping information:
        ///The property maps to the column 'Image_Id' in the data source.
        ///</remarks>
        ///--------------------------------------------------------------------------------
        public  Image Image
        {
            get
            {
                return m_Image;
            }
            set
            {
                m_Image = value;
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
