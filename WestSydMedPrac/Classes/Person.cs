using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WestSydMedPrac.Classes
{
    public abstract class Person
    {
        #region Private Field Variables
        private int _personID;
        private string _firstName;
        private string _lastName;
        private string _street;
        private string _suburb;
        private string _mobile;
        private string _state;
        private string _postCode;
        private string _homePhone;

        #endregion

        #region Properties

        public int PersonID
        {
            get 
            {
                return _personID; 
            }
            set 
            {
                _personID = value; 
            }
        }

        public virtual string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public virtual string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public virtual string Street
        {
            get { return _street; }
            set { _street = value; }
        }

        public virtual string Suburb
        {
            get { return _suburb; }
            set { _suburb = value; }
        }

        public virtual string Mobile
        {
            get { return _mobile; }
            set { _mobile = value; }
        }

        public virtual string State
        {
            get { return _state; }
            set { _state = value; }
        }

        public virtual string PostCode
        {
            get { return _postCode; }
            set { _postCode = value; }
        }

        public virtual string HomePhone
        {
            get { return _homePhone; }
            set { _homePhone = value; }
        }

        #endregion

        #region Constructors

        #endregion

        #region Private Methods

        #endregion

        #region Public Data Methods
        public virtual int Get()
        {
            throw new NotImplementedException();
        }

        public virtual int Update()
        {
            throw new NotImplementedException();
        }

        public virtual int Delete()
        {
            throw new NotImplementedException();
        }

        public virtual int Insert()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
