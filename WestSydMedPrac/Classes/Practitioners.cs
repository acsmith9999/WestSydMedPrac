using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WestSydMedPrac.Classes
{
    public sealed class Practitioners : List<Practitioner>
    {
        #region Constructors
        public Practitioners()
        {
            GetAllPractitioners();
        }
        #endregion Constructors

        #region Public Methods
        public void Refresh()
        {
            Clear();
            GetAllPractitioners();
        }

        private void GetAllPractitioners()
        {
            SqlDataAccessLayer myDal = new SqlDataAccessLayer();
            DataTable practitionersTable = myDal.ExecuteStoredProc("usp_GetAllPractitioners");

            foreach (DataRow practitionerRow in practitionersTable.Rows)
            {
                Practitioner aPractitioner = new Practitioner(practitionerRow);
                Add(aPractitioner);
            }
        }
        #endregion Public Methods
    }
}
