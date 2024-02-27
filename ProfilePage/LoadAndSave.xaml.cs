using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Windows.Input;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ProfilePage
   
{
    public partial class LoadAndSave : ContentPage
    {
        private const string FilePath = "profile.json";
        private List<Profile> _profiles;

      

        public List<Profile> Profiles
        {
            get { return _profiles; }
            set { _profiles = value;

                OnPropertyChanged();
            
            }
        }


        public ICommand SaveCommand { get; private set; }
        public ICommand LoadCommand { get; private set; }
        public ICommand DeleteProfileCommand { get; private set; }
        public ICommand SelectProfilePhotoCommand { get; private set; }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private Profile _profile;

        public LoadAndSave()
        {
            InitializeComponent();

            SaveCommand = new Command(Save);
            LoadCommand = new Command(Load);
            DeleteProfileCommand = new Command<Profile>(async (profile) => await DeleteProfileAsync(profile));
            SelectProfilePhotoCommand = new Command(OnSelectProfilePhoto);
            //SelectProfilePhotoButton.Clicked += ImageButton;

            _profiles = new List<Profile>();
            Load();
            
            BindingContext = this;
        }


        private async void OnSelectProfilePhoto( )
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();

                if (photo != null)
                {

                    ProfilePhoto.Source = ImageSource.FromStream(() => photo.OpenReadAsync().Result);
                }

            }

            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load profile: " + ex.Message, "OK");
            }
        }

        private void Load()
        {
            try
            {
                var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FilePath);
                if (File.Exists(file))
                {
                    var json = File.ReadAllText(file);
                    Profiles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Profile>>(json);

                    if (_profile != null)
                    {
                        UserName = _profile.Name;
                        DisplayAlert("Success", "Profile loaded successfully.", "OK");
                    }
                }
                else
                {
                    DisplayAlert("Error", "Profile file not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to load profile: " + ex.Message, "OK");
            }
        }


        private async void Save(object param)
        {
            _profile = new Profile
            {
                Name = NameEntry.Text,
                Surname = SurnameEntry.Text,
                EmailAdress = EmailAddressEntry.Text,
                Bio = BioEntry.Text
            };

            _profiles.Add(_profile);

            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(_profiles);
                var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FilePath);
                await File.WriteAllTextAsync(file, json);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to save profile: " + ex.Message, "OK");
            }
        }

        private bool _saveButtonEnabled = true;

        public bool SaveButtonEnabled
        {
            get { return _saveButtonEnabled; } 
            
            set {
                SaveButtonEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _loadButtonEnabled = true;

        public bool LoadButtonEnabled
        {
            get => _loadButtonEnabled;
        }

        private string _OutputText;

        public string OutputText
        {
            get {  return _OutputText; }
            set { OutputText = value; 
                
                OnPropertyChanged(); 
            }
            
        } 

        private Profile _selectedProfile;

        public Profile SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                _selectedProfile = value;
                OnPropertyChanged(nameof(SelectedProfile));
                DisplayProfileDetails();
            }
        }

        private void SaveProfiles()
        {
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(Profiles);
                var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FilePath);
                File.WriteAllText(file, json);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Failed to save profiles: " + ex.Message, "OK");
            }
        }


        private async Task DeleteProfileAsync(Profile profile)
        {
            if (profile != null)
            {
                bool deleteConfirmed = await DisplayAlert("Delete Profile", "Are you sure you want to delete this profile?", "Yes", "No");
                if (deleteConfirmed)
                {
                    DeleteProfile(profile);
                }
            }
        }

        private void DeleteProfile(Profile profile)
        {
            if (profile != null && _profiles.Contains(profile))
            {
                _profiles.Remove(profile);
                SaveProfiles();
            }
        }

        private void DisplayProfileDetails()
        {
            if (SelectedProfile != null)
            {
                string profileDetails = $"Name: {SelectedProfile.Name}\n" +
                                         $"Surname: {SelectedProfile.Surname}\n" +
                                         $"Email Address: {SelectedProfile.EmailAdress}\n" +
                                         $"Bio: {SelectedProfile.Bio}";

                DisplayAlert("Profile Details", profileDetails, "OK");
            }
        }
    }
}