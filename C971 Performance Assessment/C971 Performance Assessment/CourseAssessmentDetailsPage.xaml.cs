using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_Performance_Assessment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseAssessmentDetailsPage : ContentPage
    {
        private Assessment assessmentInfo;
        private int courseId;

        public CourseAssessmentDetailsPage(Assessment assessment, int id)
        {
            InitializeComponent();

            assessmentInfo = assessment;
            courseId = id;

            if (assessmentInfo == null) { DeleteButton.IsEnabled = false; }
        }

        protected override void OnAppearing()
        {
            PopulatePickers();
            LoadCourseInfo();
        }

        private void PopulatePickers()
        {
            AssessmentType.Items.Clear();
            AssessmentType.Items.Add("Performance");
            AssessmentType.Items.Add("Objective");
        }

        private void LoadCourseInfo()
        {
            if (assessmentInfo != null)
            {
                AssessmentTitle.Text = assessmentInfo.Title;
                AssessmentType.SelectedItem = assessmentInfo.Type;
                AssessmentStart.Date = assessmentInfo.StartDate;
                AssessmentStartNotification.IsChecked = assessmentInfo.StartNotification;
                AssessmentEnd.Date = assessmentInfo.EndDate;
                AssessmentEndNotification.IsChecked = assessmentInfo.EndNotification;
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            //type already exists
            if (string.IsNullOrEmpty(AssessmentTitle.Text))
            {
                await DisplayAlert("Invalid/Missing Title", "Please enter a valid title.", "Ok");
            }
            else if ((assessmentInfo == null && Database.assessmentList.Any(x => x.Title == AssessmentTitle.Text)) || (assessmentInfo != null && Database.assessmentList.Where(x => x.Id != assessmentInfo.Id).Any(y => y.Title == AssessmentTitle.Text)))
            {
                await DisplayAlert("Duplicate Title", "Please enter a unique title.", "Ok");
            }
            else if (AssessmentType.SelectedIndex < 0)
            {
                await DisplayAlert("Invalid/Missing Type", "Please select a type from the list.", "Ok");
            }
            else if (Database.assessmentList.Where(x => x.CourseId == courseId).Any(y => y.Type == AssessmentType.SelectedItem.ToString()))
            {
                await DisplayAlert("Duplicate Type", AssessmentType.SelectedItem.ToString() + " assessment already exists for course.", "Ok");
            }
            else if (AssessmentStart.Date > AssessmentEnd.Date)
            {
                await DisplayAlert("Invalid Date Range", "Start date cannot be after end date.", "Ok");
            }
            else if (Database.courseList.Where(x => x.Id == courseId).Any(y => AssessmentStart.Date < y.StartDate) || Database.courseList.Where(x => x.Id == courseId).Any(y => AssessmentStart.Date > y.EndDate))
            {
                await DisplayAlert("Invalid Start Date", "Start date must be within date range of course.", "Ok");
            }
            else if (Database.courseList.Where(x => x.Id == courseId).Any(y => AssessmentEnd.Date < y.StartDate) || Database.courseList.Where(x => x.Id == courseId).Any(y => AssessmentEnd.Date > y.EndDate))
            {
                await DisplayAlert("Invalid End Date", "End date must be within date range of course.", "Ok");
            }
            else
            {
                if (assessmentInfo == null)
                {
                    if (await DisplayAlert("Confirm Addition", "Add assessment to system?", "Yes", "No"))
                    {
                        assessmentInfo = new Assessment
                        {
                            CourseId = courseId,
                            Title = AssessmentTitle.Text,
                            Type = AssessmentType.SelectedItem.ToString(),
                            StartDate = AssessmentStart.Date,
                            StartNotification = AssessmentStartNotification.IsChecked,
                            EndDate = AssessmentEnd.Date,
                            EndNotification = AssessmentEndNotification.IsChecked
                        };
                        Database.assessmentList.Add(assessmentInfo);
                        await Database.dbConn.InsertAsync(assessmentInfo);
                        await DisplayAlert("Assessment Added (ID: " + assessmentInfo.Id + ")", assessmentInfo.Title + " successfully added to assessment list.", "Ok");
                        DeleteButton.IsEnabled = true;
                    }
                }
                else
                {
                    if (await DisplayAlert("Confirm Update", "Update assessment in system?", "Yes", "No"))
                    {
                        assessmentInfo.Title = AssessmentTitle.Text;
                        assessmentInfo.Type = AssessmentType.SelectedItem.ToString();
                        assessmentInfo.StartDate = AssessmentStart.Date;
                        assessmentInfo.StartNotification = AssessmentStartNotification.IsChecked;
                        assessmentInfo.EndDate = AssessmentEnd.Date;
                        assessmentInfo.EndNotification = AssessmentEndNotification.IsChecked;
                        Database.assessmentList.Add(assessmentInfo);
                        await Database.dbConn.UpdateAsync(assessmentInfo);
                        await DisplayAlert("Assessment Updated (ID: " + assessmentInfo.Id + ")", assessmentInfo.Title + " successfully updated in assessment list.", "Ok");
                    }
                }
            }
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Confirm Deletion", "Delete assessment from system?", "Yes", "No"))
            {
                Database.assessmentList.RemoveAll(x => x.Id == assessmentInfo.Id);
                await Database.dbConn.DeleteAsync(assessmentInfo);
                await Navigation.PopAsync();
            }
        }
    }
}