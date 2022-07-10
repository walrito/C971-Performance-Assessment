using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_Performance_Assessment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseAssessmentsPage : ContentPage
    {
        private Course courseInfo;

        public CourseAssessmentsPage(Course course)
        {
            InitializeComponent();

            courseInfo = course;
        }

        protected override void OnAppearing()
        {
            GenerateAssessmentList();
        }

        private void GenerateAssessmentList()
        {
            AssessmentList.Children.Clear();
            foreach (Assessment a in Database.assessmentList.Where(x => x.CourseId == courseInfo.Id))
            {
                Button b = new Button
                {
                    Text = a.Title + Environment.NewLine + a.Type + Environment.NewLine + "Start: " + a.StartDate.ToString("yyyy-MM-dd") + " | End: " + a.EndDate.ToString("yyyy-MM-dd")
                };
                b.Clicked += delegate { LoadAssessmentDetails(a, courseInfo.Id); };

                AssessmentList.Children.Add(b);
            }
        }

        private void AddAssessment_Clicked(object sender, EventArgs e)
        {
            LoadAssessmentDetails(null, courseInfo.Id);
        }

        private async void LoadAssessmentDetails(Assessment assessment, int courseId)
        {
            await Navigation.PushAsync(new CourseAssessmentDetailsPage(assessment, courseId));
        }
    }
}