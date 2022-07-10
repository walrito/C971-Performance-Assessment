using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_Performance_Assessment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstructorDetailPage : ContentPage
    {
        private Instructor instructorInfo;

        public InstructorDetailPage(Instructor instructor)
        {
            InitializeComponent();

            instructorInfo = instructor;

            if (instructorInfo == null) { DeleteButton.IsEnabled = false; }
        }

        protected override void OnAppearing()
        {
            LoadInstructorInfo();
        }

        private void LoadInstructorInfo()
        {
            if (instructorInfo != null)
            {
                InstructorName.Text = instructorInfo.Name;
                InstructorPhone.Text = instructorInfo.Phone;
                InstructorEmail.Text = instructorInfo.Email;
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InstructorName.Text))
            {
                await DisplayAlert("Invalid/Missing Name", "Please enter a valid name.", "Ok");
            }
            else if (string.IsNullOrEmpty(InstructorPhone.Text))
            {
                await DisplayAlert("Invalid/Missing Phone Number", "Please enter a valid phone number.", "Ok");
            }
            else if (string.IsNullOrEmpty(InstructorEmail.Text) && ValidateEmail(InstructorEmail.Text))
            {
                await DisplayAlert("Invalid/Missing Email Address", "Please enter a valid email address.", "Ok");
            }
            else
            {
                if (instructorInfo == null)
                {
                    if (await DisplayAlert("Confirm Addition", "Add instructor to system?", "Yes", "No"))
                    {
                        instructorInfo = new Instructor
                        {
                            Name = InstructorName.Text,
                            Phone = InstructorPhone.Text,
                            Email = InstructorEmail.Text
                        };
                        Database.instructorList.Add(instructorInfo);
                        await Database.dbConn.InsertAsync(instructorInfo);
                        await DisplayAlert("Instructor Added (ID: " + instructorInfo.Id + ")", instructorInfo.Name + " successfully added to instructor list.", "Ok");
                        DeleteButton.IsEnabled = true;
                    }
                }
                else
                {
                    if (await DisplayAlert("Confirm Update", "Update instructor in system?", "Yes", "No"))
                    {
                        instructorInfo.Name = InstructorName.Text;
                        instructorInfo.Phone = InstructorPhone.Text;
                        instructorInfo.Email = InstructorEmail.Text;
                        await Database.dbConn.UpdateAsync(instructorInfo);
                        await DisplayAlert("Instructor Updated (ID: " + instructorInfo.Id + ")", instructorInfo.Name + " successfully updated in instructor list.", "Ok");
                    }
                }
            }
        }


        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Confirm Deletion", "Delete instructor from system?", "Yes", "No"))
            {
                if (Database.courseList.Any(x => x.InstructorId == instructorInfo.Id))
                {
                    await DisplayAlert("Cannot Delete", "Instructor assigned to courses, unable to delete.", "Ok");
                }
                else
                {
                    Database.instructorList.RemoveAll(x => x.Id == instructorInfo.Id);
                    await Database.dbConn.DeleteAsync(instructorInfo);
                    await Navigation.PopAsync();
                }
            }
        }

        private bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }
    }
}