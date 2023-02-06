using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace WestSydMedPrac.Classes
{
    public sealed class Patient : Person
    {
        #region Private Field Variables
        private DataTable _dtPatient;
        #endregion Private Field Variables

        #region Public Properties
        public int Patient_ID { get; set; }
        public string FullNameAndDOB 
        {
            get
            {
                return $"{this.LastName}, {this.FirstName} {this.DateOfBirth.ToShortDateString()}";
            }
        }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string MedicareNumber { get; set; }

        public string PatientNotes { get; set; }

        public IEnumerable<Appointment> Appointments { get; set; }
        #endregion Public Properties

        #region Constructors
        /// <summary>
        /// Default constructor: Instantiates a 'new' empty Patient object
        /// </summary>
        public Patient() : base()
        {
            //Does nothing
        }
        public Patient(int patient_ID) : base()
        {
            //Get the patient's details from the db
            SqlDataAccessLayer myDal = new SqlDataAccessLayer();
            try
            {
                //set up the parameter array for the stored procedure to accept the patient_ID
                SqlParameter[] parameters = { new SqlParameter("@Patient_ID", patient_ID) };

                //call the method on the DAL that reads the db
                this._dtPatient = myDal.ExecuteStoredProc("usp_GetPatient", parameters);

                //check if dt has rows
                if (_dtPatient != null && _dtPatient.Rows.Count > 0)
                {
                    //map patient's details to this class's properties by passing the first row of the table 
                    LoadPatientProperties(_dtPatient.Rows[0]);
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Unable to retrieve patient details", ex);
            }
            finally
            {
                this._dtPatient.Dispose();
                this._dtPatient = null;
                myDal = null;
            }
        }

        /// <summary>
        /// instantiates a patient object from a datarow. used by the patients collection class
        /// </summary>
        /// <param name="patientRow">datarow containing patient details from db</param>
        public Patient(DataRow patientRow) : base()
        {
            LoadPatientProperties(patientRow);
        }

        #endregion Constructors

        #region Private Methods


        private void LoadPatientProperties(DataRow dataRow)
        {
            this.Patient_ID = (int)dataRow["Patient_ID"];
            this.Gender = dataRow["Gender"].ToString();
            this.DateOfBirth = (DateTime)dataRow["DateOfBirth"];
            this.FirstName = dataRow["FirstName"].ToString();
            this.LastName = dataRow["LastName"].ToString();
            this.Street = dataRow["Street"].ToString();
            this.Suburb = dataRow["Suburb"].ToString();
            this.State = dataRow["State"].ToString();
            this.PostCode = dataRow["PostCode"].ToString();
            this.HomePhone = dataRow["HomePhone"].ToString();
            this.Mobile = dataRow["Mobile"].ToString();
            this.MedicareNumber = dataRow["MedicareNumber"].ToString();
            this.PatientNotes = dataRow["Notes"].ToString();

            //get the patient's appts and assign to the appt property
            Appointments appointments = new Appointments(this);
            this.Appointments = appointments;
        }
        #endregion Private Methods

        #region Public Data Methods
        public override int Get()
        {
            SqlDataAccessLayer myDal = new SqlDataAccessLayer();
            try
            {
                //set up the parameter array for the stored procedure to accept the patient_ID
                SqlParameter[] parameters = { new SqlParameter("@Patient_ID", this.Patient_ID) };

                //call the method on the DAL that reads the db
                this._dtPatient = myDal.ExecuteStoredProc("usp_GetPatient", parameters);

                //check if dt has rows
                if (_dtPatient != null && _dtPatient.Rows.Count > 0)
                {
                    //map patient's details to this class's properties by passing the first row of the table 
                    LoadPatientProperties(_dtPatient.Rows[0]);
                    return 1;
                }
                else { return -1; }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve patient details", ex);
            }
            finally
            {
                this._dtPatient.Dispose();
                this._dtPatient = null;
                myDal = null;
            }
        }
        public override int Insert()
        {
            try
            {
                SqlDataAccessLayer myDAL = new SqlDataAccessLayer();
                SqlParameter[] parameters = {
                    new SqlParameter("@Gender", Gender),
                    new SqlParameter("@DateOfBirth", DateOfBirth),
                    new SqlParameter("@FirstName", FirstName),
                    new SqlParameter("@LastName", LastName),
                    new SqlParameter("@Street", Street),
                    new SqlParameter("@Suburb", Suburb),
                    new SqlParameter("@State", State),
                    new SqlParameter("@PostCode", PostCode),
                    new SqlParameter("@HomePhone", HomePhone),
                    new SqlParameter("@Mobile", Mobile),
                    new SqlParameter("@MedicareNumber", MedicareNumber),
                    new SqlParameter("@Notes", PatientNotes)
                };
                //convert date value for DOB
                parameters[1].SqlDbType = SqlDbType.Date;

                //define variable to return
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_InsertPatient", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The Patient could not be added! ", ex);
            }
        }

        public override int Update()
        {
            try
            {
                SqlDataAccessLayer myDAL = new SqlDataAccessLayer();
                SqlParameter[] parameters = {
                    new SqlParameter("@Patient_ID", Patient_ID),
                    new SqlParameter("@Gender", Gender),
                    new SqlParameter("@DateOfBirth", DateOfBirth),
                    new SqlParameter("@FirstName", FirstName),
                    new SqlParameter("@LastName", LastName),
                    new SqlParameter("@Street", Street),
                    new SqlParameter("@Suburb", Suburb),
                    new SqlParameter("@State", State),
                    new SqlParameter("@PostCode", PostCode),
                    new SqlParameter("@HomePhone", HomePhone),
                    new SqlParameter("@Mobile", Mobile),
                    new SqlParameter("@MedicareNumber", MedicareNumber),
                    new SqlParameter("@Notes", PatientNotes)          
                };
                //convert date value for DOB
                parameters[2].SqlDbType = SqlDbType.Date;

                //define variable to return
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_UpdatePatient", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The Patient's details could not be updated! ", ex);                
            }
        }

        public override int Delete()
        {
            try
            {
                SqlDataAccessLayer myDAL = new SqlDataAccessLayer();
                SqlParameter[] parameters = { new SqlParameter("Patient_ID", Patient_ID) };
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_DeletePatient", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The Patient's details could not be deleted! ", ex);
            }
        }

        public void GetAppointments()
        {
            try
            {
                Appointments myAppointments = new Appointments(this);
                //assign to enum
                this.Appointments = myAppointments;
            }
            catch (Exception ex)
            {

                throw new Exception("Unable to retrieve appointments", ex);
            }
        }

        #endregion Public Data Methods

        #region Public Methods
        public static int ComparePatientName(Patient p1, Patient p2)
        {
            return p1.LastName.CompareTo(p2.LastName);
        }
        #endregion
    }
}
