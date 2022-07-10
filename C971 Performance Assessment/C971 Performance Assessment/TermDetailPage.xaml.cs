using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_Performance_Assessment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermDetailPage : ContentPage
    {
        private Term termInfo;

        public TermDetailPage(Term term)
        {
            InitializeComponent();

            termInfo = term;

            if (termInfo == null)
            {
                DeleteButton.IsEnabled = false;
                ManageTermCourses.IsEnabled = false;
            }
        }

        protected override void OnAppearing()
        {
            LoadTermInfo();
        }

        private void LoadTermInfo()
        {
            if (termInfo != null)
            {
                TermTitle.Text = termInfo.Title;
                TermStart.Date = termInfo.StartDate;
                TermEnd.Date = termInfo.EndDate;
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TermTitle.Text))
            {
                await DisplayAlert("Invalid/Missing Name", "Please enter a valid title.", "Ok");
            }
            else if ((termInfo == null && Database.termList.Any(x => x.Title == TermTitle.Text)) || (termInfo != null && Database.termList.Where(x => x.Id != termInfo.Id).Any(y => y.Title == TermTitle.Text)))
            {
                await DisplayAlert("Duplicate Title", "Please enter a unique title.", "Ok");
            }
            else if (TermStart.Date > TermEnd.Date)
            {
                await DisplayAlert("Invalid Date Range", "Start date cannot be after end date.", "Ok");
            }
            else if (termInfo != null && (Database.courseList.Where(x => x.TermId == termInfo.Id).Any(y => TermStart.Date > y.StartDate) || Database.courseList.Where(x => x.TermId == termInfo.Id).Any(y => TermStart.Date > y.EndDate)))
            {
                await DisplayAlert("Invalid Start Date", "Courses assigned to this term have start dates outside the selected range.", "Ok");
            }
            else if (termInfo != null && (Database.courseList.Where(x => x.TermId == termInfo.Id).Any(y => TermEnd.Date < y.StartDate) || Database.courseList.Where(x => x.TermId == termInfo.Id).Any(y => TermEnd.Date < y.EndDate)))
            {
                await DisplayAlert("Invalid End Date", "Courses assigned to this term have end dates outside the selected range.", "Ok");
            }
            else
            {
                if (termInfo == null)
                {
                    if (await DisplayAlert("Confirm Addition", "Add instructor to system?", "Yes", "No"))
                    {
                        termInfo = new Term
                        {
                            Title = TermTitle.Text,
                            StartDate = TermStart.Date,
                            EndDate = TermEnd.Date
                        };
                        Database.termList.Add(termInfo);
                        await Database.dbConn.InsertAsync(termInfo);
                        await DisplayAlert("Term Added (ID: " + termInfo.Id + ")", termInfo.Title + " successfully added to term list.", "Ok");
                        DeleteButton.IsEnabled = true;
                        ManageTermCourses.IsEnabled = true;
                    }
                }
                else
                {
                    if (await DisplayAlert("Confirm Update", "Update instructor in system?", "Yes", "No"))
                    {
                        termInfo.Title = TermTitle.Text;
                        termInfo.StartDate = TermStart.Date;
                        termInfo.EndDate = TermEnd.Date;
                        await Database.dbConn.UpdateAsync(termInfo);
                        await DisplayAlert("Term Updated (ID: " + termInfo.Id + ")", termInfo.Title + " successfully updated in term list.", "Ok");
                    }
                }
            }
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Confirm Deletion", "Delete term and all related courses from system?", "Yes", "No"))
            {
                foreach (Course c in Database.courseList.Where(x => x.TermId == termInfo.Id).ToList())
                {
                    Database.courseList.Remove(c);
                    await Database.dbConn.DeleteAsync(c);
                }
                Database.termList.RemoveAll(x => x.Id == termInfo.Id);
                await Database.dbConn.DeleteAsync(termInfo);
                await Navigation.PopAsync();
            }
        }

        private async void ManageTermCourses_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CoursePage(termInfo));
        }
    }
}