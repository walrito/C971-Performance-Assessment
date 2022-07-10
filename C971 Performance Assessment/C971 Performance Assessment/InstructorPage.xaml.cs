using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_Performance_Assessment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstructorPage : ContentPage
    {
        public InstructorPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            GenerateInstructorList();
        }

        private void GenerateInstructorList()
        {
            InstructorList.Children.Clear();
            foreach (Instructor i in Database.instructorList)
            {
                Button b = new Button
                {
                    Text = i.Name
                };
                b.Clicked += delegate { LoadInstructorDetails(i); };

                InstructorList.Children.Add(b);
            }
        }

        private void AddInstructor_Clicked(object sender, EventArgs e)
        {
            LoadInstructorDetails(null);
        }

        private async void LoadInstructorDetails(Instructor instructor)
        {
            await Navigation.PushAsync(new InstructorDetailPage(instructor));
        }
    }
}