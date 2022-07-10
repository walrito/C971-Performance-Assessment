using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971_Performance_Assessment
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermPage : ContentPage
    {
        public TermPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            GenerateTermList();
        }

        private void GenerateTermList()
        {
            TermList.Children.Clear();
            foreach (Term t in Database.termList)
            {
                Button b = new Button
                {
                    Text = t.Title + Environment.NewLine + "Start: " + t.StartDate.ToString("yyyy-MM-dd") + " | End: " + t.EndDate.ToString("yyyy-MM-dd")
                };
                b.Clicked += delegate { LoadTermDetails(t); };

                TermList.Children.Add(b);
            }
        }

        private void AddTerm_Clicked(object sender, EventArgs e)
        {
            LoadTermDetails(null);
        }

        private async void LoadTermDetails(Term term)
        {
            await Navigation.PushAsync(new TermDetailPage(term));
        }
    }
}