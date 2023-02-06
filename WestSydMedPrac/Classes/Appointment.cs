using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace WestSydMedPrac.Classes
{
    public sealed class Appointment
    {
        #region Private Field Variables

        #endregion Private Field Variables

        #region Public Properties
        public int Practitioner_ID { get; set; }
        public string PractitionerType { get; set; }
        public string PractitionerFirstName { get; set; }
        public string PractitionerLastName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public int Patient_ID { get; set; }
        #endregion Public Properties

        #region Constructors
        public Appointment()
        {
            //Default constructor - no implementation
        }

        public Appointment(DateTime appointmentDate,TimeSpan appointmentTime, int patient_id, int practitioner_id)
        {
            AppointmentDate = appointmentDate;
            AppointmentTime = appointmentTime;
            Patient_ID = patient_id;
            Practitioner_ID = practitioner_id;
        }

        /// <summary>
        /// Overloaded appointment constructor - creates an appointment from the values passed in a DataRow (from an existing appointment). This method is used by the Appointments class (List class). 
        /// </summary>
        /// <param name="appointmentRow"></param>
        public Appointment(DataRow appointmentRow)
        {
            Practitioner_ID = (int)appointmentRow["Practitioner_ID"];
            PractitionerType = (string)appointmentRow["PractnrTypeName_Ref"];
            PractitionerFirstName = (string)appointmentRow["FirstName"];
            PractitionerLastName = (string)appointmentRow["LastName"];
            AppointmentDate = (DateTime)appointmentRow["AppointmentDate"];
            AppointmentTime = (TimeSpan)appointmentRow["AppointmentTime_Ref"];
            Patient_ID = (int)appointmentRow["Patient_ID"];
        }
        #endregion Constructors

        #region Private Methods

        #endregion Private Methods

        #region Public Data Methods
        public int MakeAppointment()
        {
            try
            {
                SqlDataAccessLayer myDAL = new SqlDataAccessLayer();
                SqlParameter[] parameters = {
                    new SqlParameter("@Practitioner_ID", Practitioner_ID),
                    new SqlParameter("@AppointmentDate", AppointmentDate.ToShortDateString()),
                    new SqlParameter("@AppointmentTime", AppointmentTime),
                    new SqlParameter("@Patient_ID", Patient_ID),
                };

                //convert values for apptDate and apptTime
                parameters[1].SqlDbType = SqlDbType.Date;
                parameters[2].SqlDbType = SqlDbType.Time;

                //define variable to return
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_CreateAppointment", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("The appointment could not be created! ", ex);
            }
        }

        public int CancelAppointment()
        {
            try
            {
                SqlDataAccessLayer myDAL = new SqlDataAccessLayer();
                SqlParameter[] parameters = {
                    new SqlParameter("@Practitioner_ID", Practitioner_ID),
                    new SqlParameter("@AppointmentDate", AppointmentDate.ToShortDateString()),
                    new SqlParameter("@AppointmentTime", AppointmentTime),
                    new SqlParameter("@Patient_ID", Patient_ID),
                };

                //convert values for apptDate and apptTime
                parameters[1].SqlDbType = SqlDbType.Date;
                parameters[2].SqlDbType = SqlDbType.Time;

                //define variable to return
                int rowsAffected = myDAL.ExecuteNonQuerySP("usp_CancelAppointment", parameters);
                return rowsAffected;
            }
            catch (Exception ex)
            {

                throw new Exception("The appointment could not be cancelled! ", ex);
            }
        }
        #endregion Public Data Methods


    }
}
