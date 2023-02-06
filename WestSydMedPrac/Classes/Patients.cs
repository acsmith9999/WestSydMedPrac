using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace WestSydMedPrac.Classes
{
    public sealed class Patients : List<Patient>
    {
        #region Constructors
        public Patients()
        {
            //get all patients
            GetAllPatients();
        }
        #endregion

        #region Public Methods
        public void Refresh()
        {
            //clear the patient objects from this class's internal list. get all patients from db
            this.Clear();
            //get all patients
            GetAllPatients();
        }


        #endregion

        #region Private Methods
        private void GetAllPatients()
        {
            SqlDataAccessLayer myDal = new SqlDataAccessLayer();
            DataTable patientsTable = myDal.ExecuteStoredProc("usp_GetAllPatients");

            foreach (DataRow patientRow in patientsTable.Rows)
            {
                Patient aPatient = new Patient(patientRow);
                Add(aPatient);
            }
        }
        #endregion
    }
}
