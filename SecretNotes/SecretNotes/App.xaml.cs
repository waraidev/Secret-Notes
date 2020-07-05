using System;
using System.IO;
using Xamarin.Forms;
using SecretNotes.Views;
using SecretNotes.ViewModels;

namespace SecretNotes
{
    public partial class App : Application
    {
        public static string FolderPath { get; private set; }

        public App()
        {
            InitializeComponent();
            FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

            if (Current.Properties.ContainsKey("token"))
                MainPage = new NavigationPage(new NotesPage());
            else
                MainPage = new NavigationPage(new LoginPage());

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}