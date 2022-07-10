using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_Performance_Assessment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CoursePage : ContentPage
    {
        private Term termInfo;

        public CoursePage(Term term)
        {
            InitializeComponent();

            termInfo = term;
        }

        protected override void OnAppearing()
        {
            GenerateCourseList();
        }

        private void GenerateCourseList()
        {
            List<Course> tempCourseList = new List<Course>();
            CourseList.Children.Clear();

            if (termInfo == null)
            {
                tempCourseList = Database.courseList;
            }
            else
            {
                tempCourseList = Database.courseList.Where(x => x.TermId == termInfo.Id).ToList();
            }

            foreach (Course c in tempCourseList)
            {
                Button b = new Button
                {
                    Text = c.Title + Environment.NewLine + c.Status
                };
                b.Clicked += delegate
                {
                    LoadCourseDetails(c);
                };

                CourseList.Children.Add(b);
            }
        }

        private void AddCourse_Clicked(object sender, EventArgs e)
        {
            LoadCourseDetails(null);
        }

        private async void LoadCourseDetails(Course course)
        {
            await Navigation.PushAsync(new CourseDetailPage(course));
        }
    }
}