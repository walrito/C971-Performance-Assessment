using Plugin.LocalNotifications;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace C971_Performance_Assessment
{
    public partial class MainPage : ContentPage
    {
        private bool PushNotifications = true;

        public MainPage()
        {
            InitializeComponent();
            Database.SetDatabase();
        }

        protected override async void OnAppearing()
        {
            await PopulateData();
        }

        private async Task PopulateData()
        {
            await Database.dbConn.CreateTableAsync<Term>();
            await Database.dbConn.CreateTableAsync<Course>();
            await Database.dbConn.CreateTableAsync<Assessment>();
            await Database.dbConn.CreateTableAsync<Instructor>();

            Database.termList = await Database.dbConn.Table<Term>().ToListAsync();
            Database.courseList = await Database.dbConn.Table<Course>().ToListAsync();
            Database.assessmentList = await Database.dbConn.Table<Assessment>().ToListAsync();
            Database.instructorList = await Database.dbConn.Table<Instructor>().ToListAsync();

            if (!Database.termList.Any())
            {
                Instructor i = new Instructor
                {
                    Name = "Brendan Brown",
                    Phone = "619-750-0985",
                    Email = "bbro376@wgu.edu"
                };
                Database.instructorList.Add(i);
                await Database.dbConn.InsertAsync(i);

                Term t = new Term
                {
                    Title = "Term 1",
                    StartDate = new DateTime(2022, 06, 01),
                    EndDate = new DateTime(2022, 12, 31),
                };
                Database.termList.Add(t);
                await Database.dbConn.InsertAsync(t);

                Course c = new Course
                {
                    TermId = 1,
                    Title = "Mobile Application Development in C#",
                    Status = "In Progress",
                    StartDate = new DateTime(2022, 06, 01),
                    StartNotification = true,
                    EndDate = new DateTime(2022, 06, 30),
                    EndNotification = true,
                    InstructorId = 1,
                    Notes = ""
                };
                Database.courseList.Add(c);
                await Database.dbConn.InsertAsync(c);

                Assessment a1 = new Assessment
                {
                    CourseId = 1,
                    Title = "Performance Assessment",
                    Type = "Performance",
                    StartDate = new DateTime(2022, 06, 01),
                    StartNotification = true,
                    EndDate = new DateTime(2022, 06, 30),
                    EndNotification = true
                };
                Database.assessmentList.Add(a1);
                await Database.dbConn.InsertAsync(a1);

                Assessment a2 = new Assessment
                {
                    CourseId = 1,
                    Title = "Objective Assessment",
                    Type = "Objective",
                    StartDate = new DateTime(2022, 06, 01),
                    StartNotification = true,
                    EndDate = new DateTime(2022, 06, 30),
                    EndNotification = true
                };
                Database.assessmentList.Add(a2);
                await Database.dbConn.InsertAsync(a2);
            }
            ShowNotifications();
        }

        private void ShowNotifications()
        {
            if (PushNotifications)
            {
                foreach (Course c in Database.courseList)
                {
                    if (c.StartNotification && c.StartDate == DateTime.Today) { CrossLocalNotifications.Current.Show("Start Course", c.Title + " begins today, " + DateTime.Today.ToString("yyyy-MM-dd")); }
                    if (c.EndNotification && c.EndDate == DateTime.Today) { CrossLocalNotifications.Current.Show("End Course", c.Title + " ends today, " + DateTime.Today.ToString("yyyy-MM-dd")); }
                }

                foreach (Assessment a in Database.assessmentList)
                {
                    if (a.StartNotification && a.StartDate == DateTime.Today) { CrossLocalNotifications.Current.Show("Start Assessment", a.Title + " begins today, " + DateTime.Today.ToString("yyyy-MM-dd")); }
                    if (a.EndNotification && a.EndDate == DateTime.Today) { CrossLocalNotifications.Current.Show("End Assessment", a.Title + " ends today, " + DateTime.Today.ToString("yyyy-MM-dd")); }
                }

                PushNotifications = false;
            }
        }

        private async void ManageTerms_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TermPage());
        }

        private async void ManageCourses_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CoursePage(null));
        }

        private async void ManageInstructors_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InstructorPage());
        }
    }
}