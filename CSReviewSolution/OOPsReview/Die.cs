using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPsReview
{
    public class Die
    {
        //Data Memebers:
        //Usualy private
        private int _Side;
        private string _Color;
        //private int _FaceValue;
        

        //Properties
        //are responsible to assigning and retrieving data to/from their associated data member
        //retrieving data from a data member uses the get{}
        //assigning data to a data member uses the set{}
        //Proprties need to be exposed to outside users
        //Properties will have a return datatype BUT no parameter list


        //Fully implemented Property
        //has a defined Data Member that the Developer can directly access.

        public int Side
        {
            get
            {
                return _Side; //returns data of a specific datatape
            }
            set
            {
                if (value >= 6 && value <= 20)
                {
                    _Side = value;
                }
                else
                {
                    throw new Exception("Die cannot be " + value.ToString() + "sided.Die must be between 6 and 20 sided");
                }
                                                     //Assign a supplied value to the data member
                                                     //the supplied value is located in the key word:value
            }
          
                                                    //optionally include data value validation to ensure
        }                                          // an appropriate value has been supplied

        //Auto Implemented Property
        //NO Data Member definition
        //the Data member is internaly created for you
        //the data member datatype is taken from your return datatype specified on the Property header
        //auto implemented properties are usually used when there is no need to imnternal validation
        //access to a value manage by an auto implemented property MUST be done via the property
        public int FaceValue { get; set; }




    }
}
