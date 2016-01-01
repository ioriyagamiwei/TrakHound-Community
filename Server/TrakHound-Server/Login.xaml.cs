﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Threading;
using System.IO;

using TH_Configuration;
using TH_Global;
using TH_Global.Functions;
using TH_UserManagement.Management;

namespace TrakHound_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            DataContext = this;
        }

        public UserManagementSettings userManagementSettings;

        public Database_Settings userDatabaseSettings;

        UserConfiguration currentuser;
        public UserConfiguration CurrentUser
        {
            get { return currentuser; }
            set
            {
                currentuser = value;

                if (CurrentUserChanged != null) CurrentUserChanged(currentuser);
            }
        }


        public delegate void CurrentUserChanged_Handler(UserConfiguration userConfig);
        public event CurrentUserChanged_Handler CurrentUserChanged;

        public delegate void ShownChanged_Handler(bool val);
        public event ShownChanged_Handler ShownChanged;

        public delegate void Clicked_Handler();

        #region "Properties"

        public bool LoggedIn
        {
            get { return (bool)GetValue(LoggedInProperty); }
            set
            {
                SetValue(LoggedInProperty, value);
            }
        }

        public static readonly DependencyProperty LoggedInProperty =
            DependencyProperty.Register("LoggedIn", typeof(bool), typeof(Login), new PropertyMetadata(false));


        public bool UsernameEntered
        {
            get { return (bool)GetValue(UsernameEnteredProperty); }
            set { SetValue(UsernameEnteredProperty, value); }
        }

        public static readonly DependencyProperty UsernameEnteredProperty =
            DependencyProperty.Register("UsernameEntered", typeof(bool), typeof(Login), new PropertyMetadata(false));


        public bool PasswordEntered
        {
            get { return (bool)GetValue(PasswordEnteredProperty); }
            set { SetValue(PasswordEnteredProperty, value); }
        }

        public static readonly DependencyProperty PasswordEnteredProperty =
            DependencyProperty.Register("PasswordEntered", typeof(bool), typeof(Login), new PropertyMetadata(false));



        private void password_TXT_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (password_TXT.Password != "") PasswordEntered = true;
            else PasswordEntered = false;
        }

        bool loadingOpened;

        public bool Loading
        {
            get { return (bool)GetValue(LoadingProperty); }
            set
            {
                SetValue(LoadingProperty, value);
            }
        }

        public static readonly DependencyProperty LoadingProperty =
            DependencyProperty.Register("Loading", typeof(bool), typeof(Login), new PropertyMetadata(false));


        public string LoadingMessage
        {
            get { return (string)GetValue(LoadingMessageProperty); }
            set { SetValue(LoadingMessageProperty, value); }
        }

        public static readonly DependencyProperty LoadingMessageProperty =
            DependencyProperty.Register("LoadingMessage", typeof(string), typeof(Login), new PropertyMetadata(null));


        public bool LoginError
        {
            get { return (bool)GetValue(LoginErrorProperty); }
            set { SetValue(LoginErrorProperty, value); }
        }

        public static readonly DependencyProperty LoginErrorProperty =
            DependencyProperty.Register("LoginError", typeof(bool), typeof(Login), new PropertyMetadata(false));



        public string Fullname
        {
            get { return (string)GetValue(FullnameProperty); }
            set { SetValue(FullnameProperty, value); }
        }

        public static readonly DependencyProperty FullnameProperty =
            DependencyProperty.Register("Fullname", typeof(string), typeof(Login), new PropertyMetadata(null));


        public string Firstname
        {
            get { return (string)GetValue(FirstnameProperty); }
            set { SetValue(FirstnameProperty, value); }
        }

        public static readonly DependencyProperty FirstnameProperty =
            DependencyProperty.Register("Firstname", typeof(string), typeof(Login), new PropertyMetadata(null));

        public string Lastname
        {
            get { return (string)GetValue(LastnameProperty); }
            set { SetValue(LastnameProperty, value); }
        }

        public static readonly DependencyProperty LastnameProperty =
            DependencyProperty.Register("Lastname", typeof(string), typeof(Login), new PropertyMetadata(null));



        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof(string), typeof(Login), new PropertyMetadata(null));


        public string EmailAddress
        {
            get { return (string)GetValue(EmailAddressProperty); }
            set { SetValue(EmailAddressProperty, value); }
        }

        public static readonly DependencyProperty EmailAddressProperty =
            DependencyProperty.Register("EmailAddress", typeof(string), typeof(Login), new PropertyMetadata(null));


        public ImageSource ProfileImage
        {
            get { return (ImageSource)GetValue(ProfileImageProperty); }
            set { SetValue(ProfileImageProperty, value); }
        }

        public static readonly DependencyProperty ProfileImageProperty =
            DependencyProperty.Register("ProfileImage", typeof(ImageSource), typeof(Login), new PropertyMetadata(null));

        #endregion

        private void Login_Clicked(TH_WPF.Button bt)
        {
            LoginUser(username_TXT.Text, password_TXT.Password);
            password_TXT.Clear();
        }

        private void SignOut_Clicked(TH_WPF.Button bt)
        {
            Logout();
        }

        private void Create_Clicked(TH_WPF.Button bt)
        {
            CreateAccount();
        }

        public event Clicked_Handler MyAccountClicked;

        private void MyAccount_Clicked(TH_WPF.Button bt)
        {
            if (MyAccountClicked != null) MyAccountClicked();
        }

        private void ProfileImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeProfileImage();
        }


        const System.Windows.Threading.DispatcherPriority priority = System.Windows.Threading.DispatcherPriority.Background;


        public void LoadUserConfiguration(UserConfiguration userConfig)
        {
            if (userConfig != null)
            {
                Fullname = TH_Global.Formatting.UppercaseFirst(userConfig.first_name) + " " + TH_Global.Formatting.UppercaseFirst(userConfig.last_name);
                Firstname = TH_Global.Formatting.UppercaseFirst(userConfig.first_name);
                Lastname = TH_Global.Formatting.UppercaseFirst(userConfig.last_name);

                Username = TH_Global.Formatting.UppercaseFirst(userConfig.username);
                EmailAddress = userConfig.email;

                username_TXT.Clear();
                password_TXT.Clear();

                rememberme_CHK.IsChecked = false;

                LoadProfileImage(userConfig);
                LoggedIn = true;
            }
            else
            {
                LoggedIn = false;
            }

            CurrentUser = userConfig;
        }

        #region "Login"

        class Login_Info
        {
            public string username { get; set; }
            public string password { get; set; }
            public bool rememberMe { get; set; }
        }

        Thread login_THREAD;

        public void LoginUser(string username, string password)
        {
            Loading = true;
            LoadingMessage = Formatting.UppercaseFirst(username);

            LoginError = false;
            ProfileImage = new BitmapImage(new Uri("pack://application:,,,/TH_UserManagement;component/Resources/blank_profile_01.png"));

            Login_Info info = new Login_Info();
            info.username = username;
            info.password = password;
            info.rememberMe = RememberMe;

            if (login_THREAD != null) login_THREAD.Abort();

            login_THREAD = new Thread(new ParameterizedThreadStart(Login_Worker));
            login_THREAD.Start(info);
        }

        void Login_Worker(object o)
        {
            if (o != null)
            {
                Login_Info info = (Login_Info)o;

                UserConfiguration userConfig = null;

                //If no userconfiguration database configuration found then use default TrakHound User Database
                if (userDatabaseSettings == null)
                {
                    userConfig = Remote.Users.Login(info.username, info.password);
                }
                else
                {
                    userConfig = Local.Users.Login(info.username, info.password, userDatabaseSettings);
                }

                if (userConfig != null && info.rememberMe) Remote.Users.RememberMe.Set(userConfig, rememberMeType);

                this.Dispatcher.BeginInvoke(new Action<UserConfiguration>(Login_Finished), priority, new object[] { userConfig });
            }
        }

        void Login_Finished(UserConfiguration userConfig)
        {
            // If login was successful
            if (userConfig != null)
            {
                LoadUserConfiguration(userConfig);

                Close();
            }
            else
            {
                LoginError = true;
                LoggedIn = false;

                Fullname = null;
                Firstname = null;
                Lastname = null;

                Username = null;

                username_TXT.Clear();
                password_TXT.Clear();
            }

            Loading = false;
        }

        #endregion

        #region "Logout"

        Thread logout_THREAD;

        public void Logout()
        {
            Loading = true;
            LoadingMessage = "Signing Out..";

            if (logout_THREAD != null) logout_THREAD.Abort();

            logout_THREAD = new Thread(new ThreadStart(Logout_Worker));
            logout_THREAD.Start();
        }

        void Logout_Worker()
        {
            Remote.Users.RememberMe.Clear(rememberMeType);

            this.Dispatcher.BeginInvoke(new Action(Logout_Finished), priority, new object[] { });
        }

        void Logout_Finished()
        {
            LoggedIn = false;

            Fullname = null;
            Firstname = null;
            Lastname = null;

            Username = null;

            username_TXT.Clear();
            password_TXT.Clear();

            CurrentUser = null;
            ProfileImage = new BitmapImage(new Uri("pack://application:,,,/TH_UserManagement;component/Resources/blank_profile_01.png"));

            Loading = false;
            LoadingMessage = null;
        }

        #endregion

        #region "Remember Me"

        public RememberMeType rememberMeType { get { return RememberMeType.Server; } }

        public bool RememberMe
        {
            get { return (bool)GetValue(RememberMeProperty); }
            set { SetValue(RememberMeProperty, value); }
        }

        public static readonly DependencyProperty RememberMeProperty =
            DependencyProperty.Register("RememberMe", typeof(bool), typeof(Login), new PropertyMetadata(false));


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RememberMe = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RememberMe = false;
        }


        Thread rememberme_THREAD;

        public void LoadRememberMe()
        {
            Loading = true;
            LoadingMessage = "Logging in..";

            LoginError = false;
            ProfileImage = new BitmapImage(new Uri("pack://application:,,,/TH_UserManagement;component/Resources/blank_profile_01.png"));

            if (rememberme_THREAD != null) rememberme_THREAD.Abort();

            rememberme_THREAD = new Thread(new ThreadStart(LoadRememberMe_Worker));
            rememberme_THREAD.Start();
        }

        void LoadRememberMe_Worker()
        {
            UserConfiguration RememberUser = Remote.Users.RememberMe.Get(rememberMeType);

            if (RememberUser != null) Remote.Users.RememberMe.Set(RememberUser, rememberMeType);

            this.Dispatcher.BeginInvoke(new Action<UserConfiguration>(LoadRememberMe_Finished), priority, new object[] { RememberUser });
        }

        void LoadRememberMe_Finished(UserConfiguration userConfig)
        {
            // If login was successful
            if (userConfig != null)
            {
                LoadUserConfiguration(userConfig);
            }
            else
            {
                LoggedIn = false;

                Fullname = null;
                Firstname = null;
                Lastname = null;

                Username = null;
            }

            Loading = false;
        }

        #endregion

        #region "Profile Image"

        public bool ProfileImageLoading
        {
            get { return (bool)GetValue(ProfileImageLoadingProperty); }
            set { SetValue(ProfileImageLoadingProperty, value); }
        }

        public static readonly DependencyProperty ProfileImageLoadingProperty =
            DependencyProperty.Register("ProfileImageLoading", typeof(bool), typeof(Login), new PropertyMetadata(false));


        Thread profileimage_THREAD;

        void LoadProfileImage(UserConfiguration userConfig)
        {
            ProfileImageLoading = true;

            ProfileImage = new BitmapImage(new Uri("pack://application:,,,/TH_UserManagement;component/Resources/blank_profile_01.png"));

            if (profileimage_THREAD != null) profileimage_THREAD.Abort();

            profileimage_THREAD = new Thread(new ParameterizedThreadStart(LoadProfileImage_Worker));
            profileimage_THREAD.Start(userConfig);
        }

        void LoadProfileImage_Worker(object o)
        {
            if (o != null)
            {
                UserConfiguration userConfig = (UserConfiguration)o;

                if (userConfig != null)
                {
                    System.Drawing.Image img = ProfileImages.GetProfileImage(userConfig, userDatabaseSettings);

                    this.Dispatcher.BeginInvoke(new Action<System.Drawing.Image>(LoadProfileImage_GUI), priority, new object[] { img });
                }

                this.Dispatcher.BeginInvoke(new Action(LoadProfileImage_Finished), priority, new object[] { });
            }
        }

        void LoadProfileImage_GUI(System.Drawing.Image img)
        {
            if (img != null)
            {
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);

                IntPtr bmpPt = bmp.GetHbitmap();
                BitmapSource bmpSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmpPt, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                bmpSource.Freeze();

                ProfileImage = TH_WPF.Image_Functions.SetImageSize(bmpSource, 120, 120);
            }
        }

        void LoadProfileImage_Finished()
        {
            ProfileImageLoading = false;
        }

        void ChangeProfileImage()
        {
            if (LoggedIn && CurrentUser != null)
            {

                // Show OpenFileDialog for selecting new Profile Image
                string imagePath = ProfileImages.OpenImageBrowse();
                if (imagePath != null)
                {
                    // Crop and Resize image
                    System.Drawing.Image img = ProfileImages.ProcessImage(imagePath);
                    if (img != null)
                    {
                        string filename = String_Functions.RandomString(20);

                        string tempdir = FileLocations.TrakHound + @"\temp";
                        if (!Directory.Exists(tempdir)) Directory.CreateDirectory(tempdir);

                        string localPath = tempdir + @"\" + filename;

                        img.Save(localPath);

                        if (ProfileImages.UploadProfileImage(filename, localPath, userDatabaseSettings))
                        {
                            Remote.Users.UpdateImageURL(filename, CurrentUser);

                            LoadProfileImage(CurrentUser);

                            CurrentUser = CurrentUser;
                        }
                    }
                }
            }
        }

        #endregion

        public event Clicked_Handler CreateClicked;

        void CreateAccount()
        {
            if (CreateClicked != null) CreateClicked();
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (username_TXT.IsFocused)
                {
                    password_TXT.Focus();
                }

                if (password_TXT.IsFocused)
                {
                    LoginUser(username_TXT.Text, password_TXT.Password);
                    password_TXT.Clear();
                }
            }
        }

        private void username_TXT_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (username_TXT.Text != String.Empty) UsernameEntered = true;
            else UsernameEntered = false;
        }

        private void password_TXT_GotFocus(object sender, RoutedEventArgs e)
        {
            password_TXT.Password = "";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UsernameFocus();
        }

        System.Timers.Timer focus_TIMER;

        void UsernameFocus()
        {
            if (focus_TIMER != null) focus_TIMER.Enabled = false;

            focus_TIMER = new System.Timers.Timer();
            focus_TIMER.Interval = 50;
            focus_TIMER.Elapsed += focus_TIMER_Elapsed;
            focus_TIMER.Enabled = true;

            //Keyboard.Focus(username_TXT);

            //username_TXT.Focus();
            //username_TXT.Select(0, 0);
        }

        void focus_TIMER_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            focus_TIMER.Enabled = false;

            this.Dispatcher.BeginInvoke(new Action(UsernameFocus_GUI));
        }

        void UsernameFocus_GUI()
        {
            username_TXT.Focus();
        }

    }
}
