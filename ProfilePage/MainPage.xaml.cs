namespace ProfilePage
{
    public partial class MainPage : ContentPage
    {
        private bool LoadButtonEnabled;

        public string OutputText { get; private set; }

        private bool SaveButtonEnabled;
        private string _fileName;

        public MainPage()
        {
            InitializeComponent();
            SaveButtonEnabled = true;
            LoadButtonEnabled = true;
            BindingContext = this;
        }

        

        private async void Signup_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoadAndSave());
        }
    }

}
