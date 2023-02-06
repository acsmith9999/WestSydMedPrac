using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using WestSydMedPrac.Classes;


namespace WestSydMedPrac
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Patients allPatients = new Patients();
        Practitioners allPractitioners = new Practitioners();
        Appointments appointments;

        public MainWindow()
        {
            InitializeComponent();

            //Populate patients tab with existing patients
            LoadPatientsListView();
            //Populate practitioner tab with existing practitioners
            LoadPractitionersListView();

            //Make dates in the past unavailable for selection
            dpAvailableAppts.BlackoutDates.AddDatesInPast();
            dpAppDate.BlackoutDates.AddDatesInPast();

            //Get all time slots and put in cbobox
            cboAppDays.ItemsSource = GetTimeIntervals();
        }
        #region helper functions
        private void LoadPatientsListView()
        {
            allPatients.Clear();
            allPatients = new Patients();
            //Sort patients alphabetically http://www.codedigest.com/Articles/CSHARP/84_Sorting_in_Generic_List.aspx
            Comparison<Patient> compareLastName = new Comparison<Patient>(Patient.ComparePatientName);
            allPatients.Sort(compareLastName);
            //bind the patients collection to the listview and cbobox
            lvPatients.ItemsSource = allPatients;
            cboPatients.ItemsSource = allPatients;
        }
        private void LoadPractitionersListView()
        {
            allPractitioners.Clear();
            allPractitioners = new Practitioners();
            //Sort practitioners alphabetically
            Comparison<Practitioner> comparePracLastName = new Comparison<Practitioner>(Practitioner.ComparePractitionerName);
            allPractitioners.Sort(comparePracLastName);
            //bind the practitioners collection to the listview
            lvPractitioners.ItemsSource = allPractitioners;
        }
        //updates the dp with selected practitioner's availability
        private List<DateTime> getUnavailableDays(Practitioner newPractitioner)
        {
            dpAvailableAppts.BlackoutDates.Clear();
            dpAvailableAppts.BlackoutDates.AddDatesInPast();
            List<DateTime> list = new List<DateTime>();
            int days = DateTime.DaysInMonth(dpAvailableAppts.DisplayDate.Year, dpAvailableAppts.DisplayDate.Month);

            for (int i = 0; i < days; i++)
            {
                DateTime datetime = new DateTime(dpAvailableAppts.DisplayDate.Year, dpAvailableAppts.DisplayDate.Month, i + 1);
                if (datetime.DayOfWeek == DayOfWeek.Monday && newPractitioner.Monday == false)
                {
                    list.Add(datetime);
                }
                if (datetime.DayOfWeek == DayOfWeek.Tuesday && newPractitioner.Tuesday == false)
                {
                    list.Add(datetime);
                }
                if (datetime.DayOfWeek == DayOfWeek.Wednesday && newPractitioner.Wednesday == false)
                {
                    list.Add(datetime);
                }
                if (datetime.DayOfWeek == DayOfWeek.Thursday && newPractitioner.Thursday == false)
                {
                    list.Add(datetime);
                }
                if (datetime.DayOfWeek == DayOfWeek.Friday && newPractitioner.Friday == false)
                {
                    list.Add(datetime);
                }
                if (datetime.DayOfWeek == DayOfWeek.Saturday && newPractitioner.Saturday == false)
                {
                    list.Add(datetime);
                }
                if (datetime.DayOfWeek == DayOfWeek.Sunday && newPractitioner.Sunday == false)
                {
                    list.Add(datetime);
                }
            }
            return list;
        }
        //gets all time intervals and subtracts those where selected practitioner already has an appointment
        public List<string> GetTimeIntervals(int PractitionerId)
        {
            List<string> timeIntervals = new List<string>();
            TimeSpan startTime = new TimeSpan(8, 30, 0);
            DateTime startDate = new DateTime(DateTime.MinValue.Ticks); // Date to be used to get shortTime format.
            for (int i = 0; i < 19; i++)
            {
                int minutesToBeAdded = 30 * i;      // Increasing minutes by 30 minutes interval
                TimeSpan timeToBeAdded = new TimeSpan(0, minutesToBeAdded, 0);
                TimeSpan t = startTime.Add(timeToBeAdded);
                DateTime result = startDate + t;
                
                timeIntervals.Add(result.ToShortTimeString());

                foreach (Appointment a in appointments)
                {
                    if(t.CompareTo(a.AppointmentTime) == 0 && a.AppointmentDate== dpAvailableAppts.SelectedDate) { timeIntervals.Remove(result.ToShortTimeString()); }
                }
                // Use Date.ToShortTimeString() method to get the desired format              
            }

            return timeIntervals;
        }
        //gets all time intervals
        public List<string> GetTimeIntervals()
        {
            List<string> timeIntervals = new List<string>();
            TimeSpan startTime = new TimeSpan(8, 30, 0);
            DateTime startDate = new DateTime(DateTime.MinValue.Ticks); // Date to be used to get shortTime format.
            for (int i = 0; i < 19; i++)
            {
                int minutesToBeAdded = 30 * i;      // Increasing minutes by 30 minutes interval
                TimeSpan timeToBeAdded = new TimeSpan(0, minutesToBeAdded, 0);
                TimeSpan t = startTime.Add(timeToBeAdded);
                DateTime result = startDate + t;
                timeIntervals.Add(result.ToShortTimeString());
                // Use Date.ToShortTimeString() method to get the desired format              
            }

            return timeIntervals;
        }
        #endregion
        #region selectionChanged
        private void tabctrlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
        private void lvPatients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
        private void cboGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
        private void lvPractitioners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
        private void lvAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
        private void cboPatState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
        //selects patient from lv and displays appointments
        private void cboPatients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboPatients.SelectedItem != null)
            {
                Patient newPatient = new Patient(Convert.ToInt32(txtPatAppId.Text));

                appointments = new Appointments(newPatient);

                lvAppointments.ItemsSource = appointments;
            }
        }
        private void cboPatients_DropDownClosed(object sender, EventArgs e)
        {

        }
        //first step in creating appointment
        private void lvAppPractitioners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //check if practitioner is selected
            if (lvAppPractitioners.SelectedItem != null)
            {
                //enable inputs
                dpAvailableAppts.IsEnabled = true;
                cboAvailableApptTime.IsEnabled = true;

                //create new practitioner object and get appointments
                Practitioner newPractitioner = (Practitioner)lvAppPractitioners.SelectedItem;
                appointments = new Appointments(newPractitioner);

                //gets list of available time slots for selected practitioner
                cboAvailableApptTime.ItemsSource = GetTimeIntervals(newPractitioner.Practitioner_ID);
                //gets unavailable days for selected practitioner and blacks out in dp
                {
                    List<DateTime> datelist = getUnavailableDays(newPractitioner);
                    foreach (DateTime date in datelist)
                    {
                        dpAvailableAppts.BlackoutDates.Add(new CalendarDateRange(date));
                    }
                }
            }
        }
        //displays practitioners in lv who are available at the selected day and time
        private void cboAppDays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboAppDays.SelectedIndex != -1 && dpAppDate.SelectedDate != null)
            {
                string daySelected = dpAppDate.SelectedDate.Value.DayOfWeek.ToString();
                AvailablePractitioners(daySelected);
            }
        }
        private void cboAppDays_DropDownClosed(object sender, EventArgs e)
        {

        }
        //displays practitioners in lv who are available at the selected day and time
        private void dpAppDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboAppDays.SelectedIndex != -1 && dpAppDate.SelectedDate != null)
            {
                string daySelected = dpAppDate.SelectedDate.Value.DayOfWeek.ToString();
                AvailablePractitioners(daySelected);
            }
        }
        //allows user to only select available appointment slots when creating new
        private void dpAvailableAppts_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Practitioner newPractitioner = (Practitioner)lvAppPractitioners.SelectedItem;
            appointments = new Appointments(newPractitioner);
            cboAvailableApptTime.ItemsSource = GetTimeIntervals(newPractitioner.Practitioner_ID);
        }
        #endregion
        #region move through patient records
        private void btnPatFirstRecord_Click(object sender, RoutedEventArgs e)
        {
            //check if list contains records
            if (lvPatients.Items.Count > 0)
            {
                lvPatients.SelectedItem = lvPatients.Items[0];
                //scroll to top
                lvPatients.ScrollIntoView(lvPatients.SelectedItem);
            }
        }
        private void btnPatPreviousRecord_Click(object sender, RoutedEventArgs e)
        {
            if (lvPatients.SelectedIndex >= 1)
            {
                lvPatients.SelectedIndex--;
                lvPatients.ScrollIntoView(lvPatients.SelectedItem);
            }
        }
        private void btnPatNextRecord_Click(object sender, RoutedEventArgs e)
        {
            if (lvPatients.SelectedIndex < lvPatients.Items.Count - 1)
            {
                lvPatients.SelectedIndex++;
                lvPatients.ScrollIntoView(lvPatients.SelectedItem);
            }
        }
        private void btnPatLastRecord_Click(object sender, RoutedEventArgs e)
        {
            if (lvPatients.Items.Count > 0)
            {
                lvPatients.SelectedIndex = lvPatients.Items.Count - 1;
                lvPatients.ScrollIntoView(lvPatients.SelectedItem);
            }
        }
        #endregion
        #region patient controls
        private void btnAddNewPatient_Click(object sender, RoutedEventArgs e)
        {
            //toggle the button between add new and save
            if (btnAddNewPatient.Content.ToString() == "Add New")
            {
                //toggle the button to save
                btnAddNewPatient.Content = "Save";
                //deselect the listview
                lvPatients.SelectedIndex = -1;
                //disable the listview
                lvPatients.IsEnabled = false;
                //clear the controls
                //ClearPatientTabControls();
                //toggle the other buttons
                btnCancelPatient.IsEnabled = true;
                btnUpdatePatient.IsEnabled = false;
                btnDeletePatient.IsEnabled = false;
                btnAppointmentsPatient.IsEnabled = false;
            }
            else
            {
                //do the save
                if (ValidatePatientControls())
                {
                    Patient newPatient = new Patient();
                    //AssignPropertiesToPatient(newPatient);

                    AssignPropertiesToPatient(newPatient);

                    if (newPatient.Insert() == 1)
                    {
                        MessageBox.Show("New patient added");
                        ClearPatientTabControls();
                        LoadPatientsListView();
                        lvPatients.SelectedIndex = lvPatients.Items.Count - 1;
                        lvPatients.ScrollIntoView(lvPatients.SelectedItem);
                    }
                    //toggle the button back to add new
                    if (btnAddNewPatient.Content.ToString() == "Save")
                    {
                        //toggle the button to save
                        btnAddNewPatient.Content = "Add New";
                        //disable the listview
                        lvPatients.IsEnabled = true;
                        //toggle the other buttons
                        btnCancelPatient.IsEnabled = false;
                        btnUpdatePatient.IsEnabled = true;
                        btnDeletePatient.IsEnabled = true;
                        btnAppointmentsPatient.IsEnabled = true;
                    }
                }
            }
        }
        private bool ValidatePatientControls()
        {
            if (cboGender.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a gender", "Patient Gender?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (dtpPatDateOfBirth.SelectedDate == null)
            {
                MessageBox.Show("Please select a Date Of Birth!", "Patient Date Of Birth?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPatFirstName.Text == string.Empty || txtPatFirstName.Text == null)
            {
                MessageBox.Show("Please enter a First Name!", "First Name?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPatLastName.Text == string.Empty || txtPatLastName.Text == null)
            {
                MessageBox.Show("Please enter a Last Name!", "Last Name?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPatStreet.Text == string.Empty || txtPatStreet.Text == null)
            {
                MessageBox.Show("Please enter Street address details!", "Street Number & Name?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPatSuburb.Text == string.Empty || txtPatSuburb.Text == null)
            {
                MessageBox.Show("Please enter a Suburb Name!", "Suburb Name?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (cboPatState.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a State!", "State?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPatPostCode.Text == string.Empty || txtPatPostCode.Text == null)
            {
                MessageBox.Show("Please enter a Post Code!", "Post Code?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPatHomePhone.Text == string.Empty || txtPatHomePhone.Text == null)
            {
                MessageBox.Show("Please enter a Home Phone number!", "Home Phone Number?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPatMobilePhone.Text == string.Empty || txtPatMobilePhone.Text == null)
            {
                MessageBox.Show("Please enter a Mobile Phone Number!", "Mobile Phone Number?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPatMedicareNumber.Text == string.Empty || txtPatMedicareNumber.Text == null)
            {
                MessageBox.Show("Please enter a Medicare Number!", "Medicare Number?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else
            {
                return true;
            }
        }
        private void ClearPatientTabControls()
        {
            txtPatient_ID.Clear();
            cboGender.SelectedIndex = -1;
            dtpPatDateOfBirth.SelectedDate = null;
            txtPatFirstName.Clear();
            txtPatLastName.Clear();
            txtPatStreet.Clear();
            txtPatSuburb.Clear();
            cboPatState.SelectedIndex = -1;
            txtPatPostCode.Clear();
            txtPatHomePhone.Clear();
            txtPatMobilePhone.Clear();
            txtPatMedicareNumber.Clear();
            txtPatNotes.Clear();
        }
        private void AssignPropertiesToPatient(Patient newPatient)
        {
            newPatient.Gender = cboGender.Text;
            newPatient.DateOfBirth = (DateTime)dtpPatDateOfBirth.SelectedDate;
            newPatient.FirstName = txtPatFirstName.Text;
            newPatient.LastName = txtPatLastName.Text;
            newPatient.Street = txtPatStreet.Text;
            newPatient.Suburb = txtPatSuburb.Text;
            newPatient.State = cboPatState.Text;
            newPatient.PostCode = txtPatPostCode.Text;
            newPatient.HomePhone = txtPatHomePhone.Text;
            newPatient.Mobile = txtPatMobilePhone.Text;
            newPatient.MedicareNumber = txtPatMedicareNumber.Text;
            newPatient.PatientNotes = txtPatNotes.Text;
        }
        private void btnUpdatePatient_Click(object sender, RoutedEventArgs e)
        {
            if (lvPatients.SelectedItem != null)
            {
                string message = "The patient's details will be updated! \n Do you wish to continue?";
                string caption = "Update patient?";

                if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Patient selectedPatient;
                    selectedPatient = (Patient)lvPatients.SelectedItem;
                    int selectedIndex = lvPatients.SelectedIndex;
                    AssignPropertiesToPatient(selectedPatient);

                    try
                    {
                        if (selectedPatient.Update() == 1)
                        {
                            MessageBox.Show("Patient's details successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadPatientsListView();
                            lvPatients.SelectedIndex = selectedIndex;
                            lvPatients.ScrollIntoView(lvPatients.SelectedIndex);
                        }
                    }
                    catch (Exception ex)
                    {
                        message = $"Something went wrong! \n The patient's details could not be updated. \n{ex.Message}";
                        MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a patient to update", "Select a patient", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void btnDeletePatient_Click(object sender, RoutedEventArgs e)
        {
            if (lvPatients.SelectedItem != null)
            {
                string message = "Are you sure you want to delete? \n The patient's details and all appointments will be permanently deleted!";
                string caption = "Delete patient?";

                if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Patient selectedPatient;
                    selectedPatient = (Patient)lvPatients.SelectedItem;

                    try
                    {
                        if (selectedPatient.Delete() == 1)
                        {
                            MessageBox.Show("Patient's details successfully deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadPatientsListView();
                            lvPatients.SelectedIndex = 0;
                            lvPatients.ScrollIntoView(lvPatients.SelectedIndex);
                        }
                    }
                    catch (Exception ex)
                    {
                        message = $"Something went wrong! \n The patient's details could not be deleted. \n{ex.Message}";
                        MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a patient to delete", "Select a patient", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void btnCancelPatient_Click(object sender, RoutedEventArgs e)
        {
            btnAddNewPatient.IsEnabled = true;
            btnAddNewPatient.Content = "Add New";
            btnDeletePatient.IsEnabled = true;
            btnDeletePatient.Content = "Delete";
            btnUpdatePatient.IsEnabled = true;
            btnUpdatePatient.Content = "Update";
            lvPatients.IsEnabled = true;
            btnCancelPatient.IsEnabled = false;
            btnAppointmentsPatient.IsEnabled = true;
            ClearPatientTabControls();
        }
        private void btnAppointmentsPatient_Click(object sender, RoutedEventArgs e)
        {
            tabctrlMain.SelectedIndex = 2;
            cboPatients.SelectedItem = lvPatients.SelectedItem;

            Patient newPatient = new Patient(Convert.ToInt32(txtPatient_ID.Text));

            appointments = new Appointments(newPatient);

            lvAppointments.ItemsSource = appointments;
        }
        #endregion
        #region move through practitioner records
        private void btnPracFirstRecord_Click(object sender, RoutedEventArgs e)
        {
            //check if list contains records
            if (lvPractitioners.Items.Count > 0)
            {
                lvPractitioners.SelectedItem = lvPractitioners.Items[0];
                //scroll to top
                lvPractitioners.ScrollIntoView(lvPractitioners.SelectedItem);
            }
        }
        private void btnPracPreviousRecord_Click(object sender, RoutedEventArgs e)
        {
            if (lvPractitioners.SelectedIndex >= 1)
            {
                lvPractitioners.SelectedIndex--;
                lvPractitioners.ScrollIntoView(lvPractitioners.SelectedItem);
            }
        }
        private void btnPracNextRecord_Click(object sender, RoutedEventArgs e)
        {
            if (lvPractitioners.SelectedIndex < lvPractitioners.Items.Count - 1)
            {
                lvPractitioners.SelectedIndex++;
                lvPractitioners.ScrollIntoView(lvPractitioners.SelectedItem);
            }
        }
        private void btnPracLastRecord_Click(object sender, RoutedEventArgs e)
        {
            if (lvPractitioners.Items.Count > 0)
            {
                lvPractitioners.SelectedIndex = lvPractitioners.Items.Count - 1;
                lvPractitioners.ScrollIntoView(lvPractitioners.SelectedItem);
            }
        }
        #endregion
        #region practitioner controls
        private void ClearPractitionerTabControls()
        {
            txtPractitioner_ID.Clear();
            txtPracFirstName.Clear();
            txtPracLastName.Clear();
            txtPracStreet.Clear();
            cboPracState.SelectedIndex = -1;
            txtPracSuburb.Clear();
            txtPracHomePhone.Clear();
            txtPracPostCode.Clear();
            txtPracMobilePhone.Clear();
            txtPracRegistrationNumber.Clear();
            txtPractnrTypeName.SelectedIndex = -1;
            cbMonday.IsChecked = false;
            cbThursday.IsChecked = false;
            cbWednesday.IsChecked = false;
            cbThursday.IsChecked = false;
            cbFriday.IsChecked = false;
            cbSaturday.IsChecked = false;
            cbSunday.IsChecked = false;
        }
        private void btnAddNewPractitioner_Click(object sender, RoutedEventArgs e)
        {
            //toggle the button between add new and save
            if (btnAddNewPractitioner.Content.ToString() == "Add New")
            {
                //toggle the button to save
                btnAddNewPractitioner.Content = "Save";
                //deselect the listview
                lvPractitioners.SelectedIndex = -1;
                //disable the listview
                lvPractitioners.IsEnabled = false;
                //clear the controls
                //ClearPatientTabControls();
                //toggle the other buttons
                btnCancelPractitioner.IsEnabled = true;
                btnUpdatePractitioner.IsEnabled = false;
                btnDeletePractitioner.IsEnabled = false;
                btnAppointmentsPractitioner.IsEnabled = false;
            }
            else
            {
                //do the save
                if (ValidatePractitionerControls())
                {
                    Practitioner newPractitioner = new Practitioner();
                    //AssignProperties;
                    AssignPropertiesToPractitioner(newPractitioner);
                    if (newPractitioner.Insert() == 1)
                    {
                        MessageBox.Show("New practitioner added");
                        ClearPractitionerTabControls();
                        LoadPractitionersListView();
                        lvPractitioners.SelectedIndex = lvPractitioners.Items.Count - 1;
                        lvPractitioners.ScrollIntoView(lvPractitioners.SelectedItem);
                    }
                    //toggle the button back to add new
                    if (btnAddNewPractitioner.Content.ToString() == "Save")
                    {
                        //toggle the button to save
                        btnAddNewPractitioner.Content = "Add New";
                        //enable the listview
                        lvPractitioners.IsEnabled = true;
                        //toggle the other buttons
                        btnCancelPractitioner.IsEnabled = false;
                        btnUpdatePractitioner.IsEnabled = true;
                        btnDeletePractitioner.IsEnabled = true;
                        btnAppointmentsPractitioner.IsEnabled = false;
                    }
                }
            }
        }
        private void AssignPropertiesToPractitioner(Practitioner newPractitioner)
        {
            newPractitioner.FirstName = txtPracFirstName.Text;
            newPractitioner.LastName = txtPracLastName.Text;
            newPractitioner.Street = txtPracStreet.Text;
            newPractitioner.Suburb = txtPracSuburb.Text;
            newPractitioner.State = cboPracState.Text;
            newPractitioner.PostCode = txtPracPostCode.Text;
            newPractitioner.HomePhone = txtPracHomePhone.Text;
            newPractitioner.Mobile = txtPracMobilePhone.Text;
            newPractitioner.RegistrationNumber = txtPracRegistrationNumber.Text;
            newPractitioner.PractnrTypeName_Ref = txtPractnrTypeName.Text;
            newPractitioner.Monday = cbMonday.IsChecked.Value;
            newPractitioner.Tuesday = cbTuesday.IsChecked.Value;
            newPractitioner.Wednesday = cbWednesday.IsChecked.Value;
            newPractitioner.Thursday = cbThursday.IsChecked.Value;
            newPractitioner.Friday = cbFriday.IsChecked.Value;
            newPractitioner.Saturday = cbSaturday.IsChecked.Value;
            newPractitioner.Sunday = cbSunday.IsChecked.Value;
        }
        private bool ValidatePractitionerControls()
        {
            if (txtPracFirstName.Text == string.Empty || txtPracFirstName.Text == null)
            {
                MessageBox.Show("Please enter a First Name!", "First Name?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPracLastName.Text == string.Empty || txtPracLastName.Text == null)
            {
                MessageBox.Show("Please enter a Last Name!", "Last Name?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPracStreet.Text == string.Empty || txtPracStreet.Text == null)
            {
                MessageBox.Show("Please enter Street address details!", "Street Number & Name?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPracSuburb.Text == string.Empty || txtPracSuburb.Text == null)
            {
                MessageBox.Show("Please enter a Suburb Name!", "Suburb Name?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (cboPracState.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a State!", "State?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPracPostCode.Text == string.Empty || txtPracPostCode.Text == null)
            {
                MessageBox.Show("Please enter a Post Code!", "Post Code?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPracHomePhone.Text == string.Empty || txtPracHomePhone.Text == null)
            {
                MessageBox.Show("Please enter a Home Phone number!", "Home Phone Number?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPracMobilePhone.Text == string.Empty || txtPracMobilePhone.Text == null)
            {
                MessageBox.Show("Please enter a Mobile Phone Number!", "Mobile Phone Number?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPracRegistrationNumber.Text == string.Empty || txtPracRegistrationNumber.Text == null)
            {
                MessageBox.Show("Please enter a registration Number!", "Registration Number?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (txtPractnrTypeName.Text == string.Empty || txtPractnrTypeName.Text == null)
            {
                MessageBox.Show("Please enter a practitioner type!", "Practitioner type?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else
            {
                return true;
            }
        }
        private void btnUpdatePractitioner_Click(object sender, RoutedEventArgs e)
        {
            if (lvPractitioners.SelectedItem != null)
            {
                string message = "The practitioner's details will be updated! \n Do you wish to continue?";
                string caption = "Update practitioner?";

                if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {

                    Practitioner selectedPractitioner;
                    selectedPractitioner = (Practitioner)lvPractitioners.SelectedItem;

                    int selectedIndex = lvPractitioners.SelectedIndex;
                    AssignPropertiesToPractitioner(selectedPractitioner);

                    try
                    {
                        if (selectedPractitioner.Update() == 1)
                        {
                            MessageBox.Show("Practitioner's details successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadPractitionersListView();
                            lvPractitioners.SelectedIndex = selectedIndex;
                            lvPractitioners.ScrollIntoView(lvPractitioners.SelectedIndex);
                        }
                    }
                    catch (Exception ex)
                    {
                        message = $"Something went wrong! \n The practitioner's details could not be updated. \n{ex.Message}";
                        MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a practitioner to update", "Select a practitioner", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void btnDeletePractitioner_Click(object sender, RoutedEventArgs e)
        {
            if (lvPractitioners.SelectedItem != null)
            {
                string message = "Are you sure you want to delete? \n The practitioner's details and all appointments will be permanently deleted!";
                string caption = "Delete practitioner?";

                if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Practitioner selectedPractitioner;
                    selectedPractitioner = (Practitioner)lvPractitioners.SelectedItem;

                    try
                    {
                        if (selectedPractitioner.Delete() == 1)
                        {
                            MessageBox.Show("Practitioner's details successfully deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadPractitionersListView();
                            lvPractitioners.SelectedIndex = 0;
                            lvPractitioners.ScrollIntoView(lvPractitioners.SelectedIndex);
                        }
                    }
                    catch (Exception ex)
                    {
                        message = $"Something went wrong! \n The practitioner's details could not be deleted. \n{ex.Message}";
                        MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a practitioner to delete", "Select a practitioner", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void btnCancelPractitioner_Click(object sender, RoutedEventArgs e)
        {
            btnAddNewPractitioner.IsEnabled = true;
            btnAddNewPractitioner.Content = "Add New";
            btnDeletePractitioner.IsEnabled = true;
            btnDeletePractitioner.Content = "Delete";
            btnUpdatePractitioner.IsEnabled = true;
            btnUpdatePractitioner.Content = "Update";
            lvPractitioners.IsEnabled = true;
            btnCancelPractitioner.IsEnabled = false;
            btnAppointmentsPractitioner.IsEnabled = true;
            ClearPractitionerTabControls();
        }
        private void btnAppointmentsPractitioner_Click(object sender, RoutedEventArgs e)
        {
            tabctrlMain.SelectedIndex = 2;
            cboPatients.SelectedIndex = -1;
            Practitioner newPractitioner = new Practitioner(Convert.ToInt32(txtPractitioner_ID.Text));

            lvAppPractitioners.Items.Clear();
            lvAppPractitioners.Items.Add(newPractitioner);
            lvAppPractitioners.SelectedIndex = 0;
        }
        #endregion
        #region appointment controls
        //checks practitioner generally available days and checks existing appointments so lv does not display unavailable
        private void AvailablePractitioners(string date)
        {
            lvAppPractitioners.Items.Clear();

            //check practitioner availability for days
            foreach (Practitioner p in allPractitioners)
            {
                if (date == "Monday" && p.Monday == true)
                {
                    lvAppPractitioners.Items.Add(p);
                }
                if (date == "Tuesday" && p.Tuesday == true)
                {
                    lvAppPractitioners.Items.Add(p);
                }
                if (date == "Wednesday" && p.Wednesday == true)
                {
                    lvAppPractitioners.Items.Add(p);
                }
                if (date == "Thursday" && p.Thursday == true)
                {
                    lvAppPractitioners.Items.Add(p);
                }
                if (date == "Friday" && p.Friday == true)
                {
                    lvAppPractitioners.Items.Add(p);
                }
                if (date == "Saturday" && p.Saturday == true)
                {
                    lvAppPractitioners.Items.Add(p);
                }
                if (date == "Sunday" && p.Sunday == true)
                {
                    lvAppPractitioners.Items.Add(p);
                }
                Practitioner tempPrac = new Practitioner(p.Practitioner_ID);
                foreach (Appointment a in tempPrac.Appointments)
                {
                    TimeSpan time = new TimeSpan(a.AppointmentTime.Hours, a.AppointmentTime.Minutes, a.AppointmentTime.Seconds);
                    DateTime dateTime = Convert.ToDateTime(time.ToString());

                    if (dateTime.ToShortTimeString() == cboAppDays.SelectedItem.ToString() && a.AppointmentDate.ToShortDateString() == dpAppDate.SelectedDate.Value.ToShortDateString())
                    {
                        MessageBox.Show("Warning! " + tempPrac.FirstName + " has an appointment already booked for " + a.AppointmentDate.ToShortDateString() + " at " + dateTime.ToShortTimeString());
                        lvAppPractitioners.Items.Remove(p);
                    }
                }
            }
        }
        private void btnCreateAppt_Click(object sender, RoutedEventArgs e)
        {
            //toggle the button between add new and save
            if (btnCreateAppt.Content.ToString() == "Create Appointment")
            {
                //toggle the button to save
                btnCreateAppt.Content = "Save";
                btnCancelAppt.IsEnabled = false;
                btnApptFormReset.IsEnabled = false;
                btnCancelSaveApp.IsEnabled = true;
            }
            else
            {
                //do the save
                if(ValidateAppointmentControls())
                {
                    Appointment newAppointment = new Appointment();
                    //AssignProperties;
                    newAppointment.AppointmentTime = Convert.ToDateTime(cboAvailableApptTime.Text).TimeOfDay;
                    newAppointment.AppointmentDate = dpAvailableAppts.SelectedDate.Value;
                    newAppointment.Patient_ID = Convert.ToInt32(txtPatAppId.Text);
                    newAppointment.Practitioner_ID = Convert.ToInt32(lblPracAppID.Text);

                    try
                    {
                        if (newAppointment.MakeAppointment() == 1)
                        {
                            MessageBox.Show("New appointment added successfully", "Success", MessageBoxButton.OK,MessageBoxImage.Information);
                            ClearApptTabControls();

                            //refresh lv
                            Patient newPatient = new Patient(newAppointment.Patient_ID);
                            appointments = new Appointments(newPatient);
                            lvAppointments.ItemsSource = appointments;
                            //toggle the button
                            btnCreateAppt.Content = "Create Appointment";
                            //clear the controls
                            ClearApptTabControls();
                            //toggle the other buttons
                            btnCancelAppt.IsEnabled = true;
                            btnApptFormReset.IsEnabled = true;
                            btnCancelSaveApp.IsEnabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("The appointment could not be added!" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private bool ValidateAppointmentControls()
        {
            if (dpAvailableAppts.SelectedDate == null)
            {
                MessageBox.Show("Please select a date!", "Date?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (cboAvailableApptTime.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a time!", "Time?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (lvAppPractitioners.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a practitioner!", "Practitioner?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else if (cboPatients.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a patient!", "Patient?", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            else
            {
                return true;
            }
        }
        private void btnCancelAppt_Click(object sender, RoutedEventArgs e)
        {
            if (lvAppointments.SelectedItem != null)
            {
                string message = "Are you sure you want to cancel? \n The appointment will be permanently deleted!";
                string caption = "Cancel appointment?";

                if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Appointment selectedAppointment = (Appointment)lvAppointments.SelectedItem;
                    try
                    {
                        if (selectedAppointment.CancelAppointment() == 1)
                        {
                            MessageBox.Show("Appointment successfully cancelled!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            //refresh the lv
                            Patient newPatient = new Patient(selectedAppointment.Patient_ID);
                            appointments = new Appointments(newPatient);
                            lvAppointments.ItemsSource = appointments;
                        }
                    }
                    catch (Exception ex)
                    {
                        message = $"Something went wrong! \n The appointment could not be cancelled. \n{ex.Message}";
                        MessageBox.Show(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an appointment to cancel", "Select an appointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void btnApptFormReset_Click(object sender, RoutedEventArgs e)
        {
            ClearApptTabControls();
        }
        private void ClearApptTabControls()
        {
            dpAvailableAppts.SelectedDate = null;
            dpAppDate.SelectedDate = null;
            cboPatients.SelectedIndex = -1;
            cboAvailableApptTime.SelectedIndex = -1;
            cboAppDays.SelectedIndex = -1;
            
            lvAppPractitioners.Items.Clear();
            lvAppointments.SelectedIndex = -1;

            appointments = new Appointments();
            lvAppointments.ItemsSource = appointments;

            dpAvailableAppts.IsEnabled = false;
            cboAvailableApptTime.IsEnabled = false;
        }
        private void btnCancelSaveApp_Click(object sender, RoutedEventArgs e)
        {
            //toggle the button
            btnCreateAppt.Content = "Create Appointment";

            //clear the controls
            ClearApptTabControls();

            //toggle the other buttons
            btnCancelAppt.IsEnabled = true;
            btnApptFormReset.IsEnabled = true;
            btnCancelSaveApp.IsEnabled = false;
        }
        #endregion
    }
}
