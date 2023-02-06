using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace WestSydMedPrac.Classes
{
    public sealed class Practitioner : Person
    {
        #region Private Field Variables
        private DataTable _dtPractitioner;
        #endregion Private Field Variables

        #region Public Properties
        public int Practitioner_ID { get; set; }

        public string RegistrationNumber { get; set; }

        public string PractnrTypeName_Ref { get; set; }

        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }


        public IEnumerable<Appointment> Appointments { get; set; }
        #endregion Public Properties

        #region Constructors
        public Practitioner() : base()
        {
            //Does nothing
        }

        public Practitioner(int practitioner_ID) : base()
        {
            //Get the patient's details from the db
            SqlDataAccessLayer myDal = new SqlDataAccessLayer();
            try
            {
                //set up the parameter array for the stored procedure to accept the patient_ID
                SqlParameter[] parameters = { new SqlParameter("@Practitioner_ID", practitioner_ID) };

                //call the method on the DAL that reads the db
                this._dtPractitioner = myDal.ExecuteStoredProc("usp_GetPractitioner", parameters);

                //check if dt has rows
                if (_dtPractitioner != null && _dtPractitioner.Rows.Count > 0)
                {
                    //map patient's details to this class's properties by passing the first row of the table 
                    LoadPractitionerProperties(_dtPractitioner.Rows[0]);
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Unable to retrieve practitioner details", ex);
            }
            finally
            {
                this._dtPractitioner.Dispose();
                this._dtPractitioner = null;
                myDal = null;
            }
        }

        public Practitioner(DataRow practitionerRow): base()
        {
            LoadPractitionerProperties(practitionerRow);
        }
        #endregion Constructors

        #region Private Methods
        private void LoadPractitionerProperties(DataRow dataRow)
        {
            this.Practitioner_ID = (int)dataRow["Practitioner_ID"];
            this.FirstName = dataRow["FirstName"].ToString();
            this.LastName = dataRow["LastName"].ToString();
            this.Street = dataRow["Street"].ToString();
            this.Suburb = dataRow["Suburb"].ToString();
            this.State = dataRow["State"].ToString();
            this.PostCode = dataRow["PostCode"].ToString();
            this.HomePhone = dataRow["HomePhone"].ToString();
            this.Mobile = dataRow["Mobile"].ToString();
            this.RegistrationNumber = dataRow["RegistrationNumber"].ToString();
            this.PractnrTypeName_Ref = dataRow["PractnrTypeName_Ref"].ToString();
            Monday = (bool)dataRow["Monday"];
            Tuesday = (bool)dataRow["Tuesday"];
            Wednesday = (bool)dataRow["Wednesday"];
            Thursday = (bool)dataRow["Thursday"];
            Friday = (bool)dataRow["Friday"];
            Saturday = (bool)dataRow["Saturday"];
            Sunday = (bool)dataRow["Sunday"];

            //get the practitioner's appts and assign to the appt property
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
                SqlParameter[] parameters = { new SqlParameter("@Practitioner_ID", this.Practitioner_ID) };

                //call the method on the DAL that reads the db
                this._dtPractitioner = myDal.ExecuteStoredProc("usp_GetPractitioner", parameters);

                //check if dt has rows
                if (_dtPractitioner != null && _dtPractitioner.Rows.Count > 0)
                {
                    //map patient's details to this class's properties by passing the first row of the table 
                    LoadPractitionerProperties(_dtPractitioner.Rows[0]);
                    return 1;
                }
                else { return -1; }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve practitioner details", ex);
            }
            finally
            {
                this._dtPractitioner.Dispose();
                this._dtPractitioner = null;
                myDal = null;
            }
        }

        public override int Insert()
        {
            try
            {
                SqlDataAccessLayer myDAL = new SqlDataAccessLayer();
                SqlParameter[] parameters = {
                    new SqlParameter("@FirstName", FirstName),
                    new SqlParameter("@LastName", LastName),
                    new SqlParameter("@Street", Street),
                    new SqlParameter("@Suburb", Suburb),
                    new SqlParameter("@State", State),
                    new SqlParameter("@PostCode", PostCode),
                    new SqlParameter("@HomePhone", HomePhone),
                    new SqlParameter("@Mobile", Mobile),
                    new SqlParameter("@RegistrationNumber", RegistrationNumber),
                    new SqlParameter("@PractnrTypeName_Ref", PractnrTypeName_Ref),
                    new SqlParameter("Monday", Monday),
                    new SqlParameter("Tuesday", Tuesday),
                    new SqlParameter("Wednesday", Wednesday),
                    new SqlParameter("Thursday", Thursday),
                    new SqlParameter("Friday", Friday),
                    new SqlParameter("Saturday", Saturday),
                    new SqlParameter("Sunday", Sunday)
                };
                //convert date value for days
                parameters[10].SqlDbType = SqlDbType.Bit;
                parameters[11].SqlDbType = SqlDbType.Bit;
                parameters[12].SqlDbType = SqlDbType.Bit;
                parameters[13].SqlDbType = SqlDbType.Bit;
                parameters[14].SqlDbType = SqlDbType.Bit;
                parameters[15].SqlDbType = SqlDbType.Bit;
                parameters[16].SqlDbType = SqlDbType.Bit;

                //define variable to return
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_InsertPractitioner", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The practitioner could not be added! ", ex);
            }
        }
        public override int Update()
        {
            try
            {
                SqlDataAccessLayer myDAL = new SqlDataAccessLayer();
                SqlParameter[] parameters = {
                    new SqlParameter("@Practitioner_ID", Practitioner_ID),
                    new SqlParameter("@FirstName", FirstName),
                    new SqlParameter("@LastName", LastName),
                    new SqlParameter("@Street", Street),
                    new SqlParameter("@Suburb", Suburb),
                    new SqlParameter("@State", State),
                    new SqlParameter("@PostCode", PostCode),
                    new SqlParameter("@HomePhone", HomePhone),
                    new SqlParameter("@Mobile", Mobile),
                    new SqlParameter("@RegistrationNumber", RegistrationNumber),
                    new SqlParameter("@PractnrTypeName_Ref", PractnrTypeName_Ref),
                    new SqlParameter("Monday", Monday),
                    new SqlParameter("Tuesday", Tuesday),
                    new SqlParameter("Wednesday", Wednesday),
                    new SqlParameter("Thursday", Thursday),
                    new SqlParameter("Friday", Friday),
                    new SqlParameter("Saturday", Saturday),
                    new SqlParameter("Sunday", Sunday)
                };
                //convert date value for days
                parameters[11].SqlDbType = SqlDbType.Bit;
                parameters[12].SqlDbType = SqlDbType.Bit;
                parameters[13].SqlDbType = SqlDbType.Bit;
                parameters[14].SqlDbType = SqlDbType.Bit;
                parameters[15].SqlDbType = SqlDbType.Bit;
                parameters[16].SqlDbType = SqlDbType.Bit;
                parameters[17].SqlDbType = SqlDbType.Bit;

                //define variable to return
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_UpdatePractioner", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The practitioner could not be updated! ", ex);
            }
        }
        public override int Delete()
        {
            try
            {
                SqlDataAccessLayer myDAL = new SqlDataAccessLayer();
                SqlParameter[] parameters = { new SqlParameter("Practitioner_ID", Practitioner_ID) };
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_DeletePractitioner", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The practioner's details could not be deleted! ", ex);
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
        public static int ComparePractitionerName(Practitioner p1, Practitioner p2)
        {
            return p1.LastName.CompareTo(p2.LastName);
        }
        #endregion
    }
}
