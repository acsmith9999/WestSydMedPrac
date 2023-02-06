using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace WestSydMedPrac.Classes
{
    public sealed class Appointments : List<Appointment>
    {
        #region Constructors
        public Appointments()
        {

        }

        public Appointments(Patient patient)
        {
            try
            {
                SqlDataAccessLayer myDAL = new SqlDataAccessLayer();
                SqlParameter[] parameter = { new SqlParameter("@Patient_ID", patient.Patient_ID) };
                DataTable dtAppointments = myDAL.ExecuteStoredProc("usp_GetAppointDetailByPatientId", parameter);
                foreach (DataRow appointmentRow in dtAppointments.Rows)
                {
                    Appointment appointment = new Appointment(appointmentRow);
                    this.Add(appointment);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve appointments", ex);
            }
        }

        public Appointments(Practitioner practitioner)
        {
            try
            {
                SqlDataAccessLayer myDAL = new SqlDataAccessLayer();
                SqlParameter[] parameter = { new SqlParameter("@Practitioner_ID", practitioner.Practitioner_ID) };
                DataTable dtAppointments = myDAL.ExecuteStoredProc("usp_GetAppointmentsDetailsByPractitioner_ID", parameter);
                foreach (DataRow appointmentRow in dtAppointments.Rows)
                {
                    Appointment appointment = new Appointment(appointmentRow);
                    this.Add(appointment);
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Unable to retrieve appointents", ex);
            }
        }
        #endregion Constructors
    }
}
