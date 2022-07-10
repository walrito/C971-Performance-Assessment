using System;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_Performance_Assessment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseDetailPage : ContentPage
    {
        private Course courseInfo;

        public CourseDetailPage(Course course)
        {
            InitializeComponent();

            courseInfo = course;

            if (courseInfo == null)
            {
                DeleteButton.IsEnabled = false;
                ManageCourseAssessments.IsEnabled = false;
            }
        }

        protected override void OnAppearing()
        {
            PopulatePickers();
            LoadCourseInfo();
        }

        private void PopulatePickers()
        {
            CourseTerm.Items.Clear();
            CourseStatus.Items.Clear();
            CourseInstructor.Items.Clear();

            foreach (Term t in Database.termList) { CourseTerm.Items.Add(t.Title); }

            CourseStatus.Items.Add("In Progress");
            CourseStatus.Items.Add("Completed");
            CourseStatus.Items.Add("Dropped");
            CourseStatus.Items.Add("Plan To Take");

            foreach (Instructor i in Database.instructorList) { CourseInstructor.Items.Add(i.Name); }
        }

        private void LoadCourseInfo()
        {
            if (courseInfo != null)
            {
                CourseTitle.Text = courseInfo.Title;
                CourseTerm.SelectedItem = Database.termList.First(x => x.Id == courseInfo.TermId).Title;
                CourseStatus.SelectedItem = courseInfo.Status;
                CourseStart.Date = courseInfo.StartDate;
                CourseStartNotification.IsChecked = courseInfo.StartNotification;
                CourseEnd.Date = courseInfo.EndDate;
                CourseEndNotification.IsChecked = courseInfo.EndNotification;
                CourseInstructor.SelectedItem = Database.instructorList.First(x => x.Id == courseInfo.InstructorId).Name;
                CourseInstructorPhone.Text = Database.instructorList.First(x => x.Id == courseInfo.InstructorId).Phone;
                CourseInstructorEmail.Text = Database.instructorList.First(x => x.Id == courseInfo.InstructorId).Email;
                CourseNotes.Text = courseInfo.Notes;
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CourseTitle.Text))
            {
                await DisplayAlert("Invalid/Missing Title", "Please enter a valid title.", "Ok");
            }
            else if ((courseInfo == null && Database.courseList.Any(x => x.Title == CourseTitle.Text)) || (courseInfo != null && Database.courseList.Where(x => x.Id != courseInfo.Id).Any(y => y.Title == CourseTitle.Text)))
            {
                await DisplayAlert("Duplicate Title", "Please enter a unique title.", "Ok");
            }
            else if (CourseTerm.SelectedIndex < 0)
            {
                await DisplayAlert("Invalid/Missing Term", "Please select a term from the list.", "Ok");
            }
            else if (courseInfo == null && Database.courseList.Count(x => x.TermId == Database.termList.First(y => y.Title == CourseTerm.SelectedItem.ToString()).Id) >= 6)
            {
                await DisplayAlert("Too Many Courses In Term", CourseTerm.SelectedItem.ToString() + " already has 6 courses assigned.", "Ok");
            }
            else if (CourseStatus.SelectedIndex < 0)
            {
                await DisplayAlert("Invalid/Missing Status", "Please select a status from the list.", "Ok");
            }
            else if (CourseStart.Date > CourseEnd.Date)
            {
                await DisplayAlert("Invalid Date Range", "Start date cannot be after end date.", "Ok");
            }
            else if (Database.termList.Where(x => x.Title == CourseTerm.SelectedItem.ToString()).Any(y => CourseStart.Date < y.StartDate) || Database.termList.Where(x => x.Title == CourseTerm.SelectedItem.ToString()).Any(y => CourseStart.Date > y.EndDate))
            {
                await DisplayAlert("Invalid Start Date", "Start date must be within date range of selected term.", "Ok");
            }
            else if (Database.termList.Where(x => x.Title == CourseTerm.SelectedItem.ToString()).Any(y => CourseEnd.Date < y.StartDate) || Database.termList.Where(x => x.Title == CourseTerm.SelectedItem.ToString()).Any(y => CourseEnd.Date > y.EndDate))
            {
                await DisplayAlert("Invalid End Date", "End date must be within date range of selected term.", "Ok");
            }
            else if (CourseInstructor.SelectedIndex < 0)
            {
                await DisplayAlert("Invalid/Missing Instructor", "Please select an instructor from the list.", "Ok");
            }
            else
            {
                if (courseInfo == null)
                {
                    if (await DisplayAlert("Confirm Addition", "Add course to system?", "Yes", "No"))
                    {
                        courseInfo = new Course
                        {
                            Title = CourseTitle.Text,
                            TermId = Database.termList.First(x => x.Title == CourseTerm.SelectedItem.ToString()).Id,
                            Status = CourseStatus.SelectedItem.ToString(),
                            StartDate = CourseStart.Date,
                            StartNotification = CourseStartNotification.IsChecked,
                            EndDate = CourseEnd.Date,
                            EndNotification = CourseEndNotification.IsChecked,
                            InstructorId = Database.instructorList.Where(x => x.Name == CourseInstructor.SelectedItem.ToString()).Where(y => y.Phone == CourseInstructorPhone.Text).First(z => z.Email == CourseInstructorEmail.Text).Id,
                            Notes = CourseNotes.Text
                        };
                        Database.courseList.Add(courseInfo);
                        await Database.dbConn.InsertAsync(courseInfo);
                        await DisplayAlert("Course Added (ID: " + courseInfo.Id + ")", courseInfo.Title + " successfully added to course list.", "Ok");
                        DeleteButton.IsEnabled = true;
                        ManageCourseAssessments.IsEnabled = true;
                    }
                }
                else
                {
                    if (await DisplayAlert("Confirm Update", "Update course in system?", "Yes", "No"))
                    {
                        courseInfo.Title = CourseTitle.Text;
                        courseInfo.TermId = Database.termList.First(x => x.Title == CourseTerm.SelectedItem.ToString()).Id;
                        courseInfo.Status = CourseStatus.SelectedItem.ToString();
                        courseInfo.StartDate = CourseStart.Date;
                        courseInfo.StartNotification = CourseStartNotification.IsChecked;
                        courseInfo.EndDate = CourseEnd.Date;
                        courseInfo.EndNotification = CourseEndNotification.IsChecked;
                        courseInfo.InstructorId = Database.instructorList.Where(x => x.Name == CourseInstructor.SelectedItem.ToString()).Where(y => y.Phone == CourseInstructorPhone.Text).First(z => z.Email == CourseInstructorEmail.Text).Id;
                        courseInfo.Notes = CourseNotes.Text;
                        await Database.dbConn.UpdateAsync(courseInfo);
                        await DisplayAlert("Course Updated (ID: " + courseInfo.Id + ")", courseInfo.Title + " successfully updated in course list.", "Ok");
                    }
                }
            }
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Confirm Deletion", "Delete course and all related assessments from system?", "Yes", "No"))
            {
                foreach (Assessment a in Database.assessmentList.Where(x => x.CourseId == courseInfo.Id).ToList())
                {
                    Database.assessmentList.Remove(a);
                    await Database.dbConn.DeleteAsync(a);
                }
                Database.courseList.RemoveAll(x => x.Id == courseInfo.Id);
                await Database.dbConn.DeleteAsync(courseInfo);
                await Navigation.PopAsync();
            }
        }

        private void CourseInstructor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CourseInstructor.SelectedIndex > -1)
            {
                CourseInstructorPhone.Text = Database.instructorList.First(x => x.Name == CourseInstructor.SelectedItem.ToString()).Phone;
                CourseInstructorEmail.Text = Database.instructorList.First(x => x.Name == CourseInstructor.SelectedItem.ToString()).Email;
            }
        }

        private async void ManageCourseAssessments_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CourseAssessmentsPage(courseInfo));
        }

        private async void ShareLink_Clicked(object sender, EventArgs e)
        {
            if (courseInfo != null && !string.IsNullOrEmpty(CourseNotes.Text))
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = CourseNotes.Text,
                    Title = "Share your course notes."
                });
            }
            else
            {
                await DisplayAlert("No Course Notes", "Please add and save notes to course before sharing.", "Ok");
            }
        }
    }
}