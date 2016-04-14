using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.App;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using System.Diagnostics;
using Android.Preferences;
using static Minesweeper.AppManager;


namespace Minesweeper
{
    [Activity(Label = "Setting", MainLauncher = false, Icon = "@drawable/icon")]
    public class SettingActivity : AppCompatActivity
    {
        private const int MIN_COLS = 8;
        private const int MIN_ROWS = 8;

        Spinner spinnerRow = null;
        Spinner spinnerCol = null;
        SeekBar seekBarMines = null;
        TextView lblMines = null;
        RadioGroup radGroup = null;
        AppSetting settings = null;
        TableLayout customSettingLayout = null;
        CheckBox chkConstraint = null;
        Toolbar toolbar = null;
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Setting, menu);
            base.OnCreateOptionsMenu(menu);
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Setting);

            this.toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetLogo(Resource.Drawable.icon32x32);
            SupportActionBar.Title = GetText(Resource.String.ApplicationName);
            this.spinnerRow = (Spinner)FindViewById(Resource.Id.spinnerRow);
            this.spinnerCol = (Spinner)FindViewById(Resource.Id.spinnerCol);
            this.seekBarMines= (SeekBar)FindViewById(Resource.Id.seekBar1);
            this.lblMines = FindViewById<TextView>(Resource.Id.textView4);
            this.radGroup = FindViewById<RadioGroup>(Resource.Id.radioGroup1);
            this.customSettingLayout = FindViewById<TableLayout>(Resource.Id.tableLayout1);
            this.chkConstraint = FindViewById<CheckBox>(Resource.Id.chkConstraint);

            InitSpinner(spinnerRow, MIN_ROWS, 35);
            InitSpinner(spinnerCol, MIN_ROWS, 35);

            this.spinnerRow.ItemSelected += SpinnerRow_ItemSelected;
            this.spinnerCol.ItemSelected += SpinnerRow_ItemSelected;
            this.seekBarMines.ProgressChanged += SeekBarMines_ProgressChanged;
            this.radGroup.CheckedChange += RadGroup_CheckedChange;
            DisplayCurrentSetting();
            // Create your application here
        }

        public void DisplayCurrentSetting()
        {
            var setting = AppManager.LoadPreferences(this);
            switch (setting.Level)
            {
                case GameLevel.Easy:
                    this.radGroup.Check(Resource.Id.radioButton1);
                    break;
                case GameLevel.Normal:
                    this.radGroup.Check(Resource.Id.radioButton2);
                    break;
                case GameLevel.Hard:
                    this.radGroup.Check(Resource.Id.radioButton3);
                    break;
                case GameLevel.VeryHard:
                    this.radGroup.Check(Resource.Id.radioButton4);
                    break;
                case GameLevel.ExtremHard:
                    this.radGroup.Check(Resource.Id.radioButton5);
                    break;
                case GameLevel.Custom:
                    this.radGroup.Check(Resource.Id.radCustomSetting);
                    this.seekBarMines.Max = (int)(setting.Cols * setting.Rows * 0.5);
                    this.spinnerCol.SetSelection(setting.Cols - MIN_COLS);
                    this.spinnerRow.SetSelection(setting.Rows - MIN_ROWS);
                    UpdateRiskRating();
                    break;
                default:
                    break;
            }
        }
        private void RadGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            if(this.radGroup.CheckedRadioButtonId!= Resource.Id.radCustomSetting)
            {
                this.customSettingLayout.Visibility = ViewStates.Invisible;
            }
            else
            {
                this.customSettingLayout.Visibility = ViewStates.Visible;
            }
        }

        private void SeekBarMines_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            UpdateRiskRating();
        }

        private void UpdateRiskRating()
        {
            var percent = (seekBarMines.Progress * 1.0f) / (seekBarMines.Max *2);
            this.lblMines.Text = string.Format("{1}({0:p})", percent, seekBarMines.Progress);
            if (percent <= 0.1)
            {
                this.lblMines.SetTextColor(Color.ParseColor(GetText(Resource.Color.EasyColor)));
            }

            if (percent > 0.1 && percent<=0.2)
            {
                this.lblMines.SetTextColor(Color.ParseColor(GetText(Resource.Color.NormalColor)));
            }

            if (percent > 0.2 && percent <= 0.3)
            {
                this.lblMines.SetTextColor(Color.ParseColor(GetText(Resource.Color.HardColor)));
            }

            if (percent > 0.3 && percent <= 0.4)
            {
                this.lblMines.SetTextColor(Color.ParseColor(GetText(Resource.Color.VeryHardColor)));
            }

            if (percent > 0.4)
            {
                this.lblMines.SetTextColor(Color.ParseColor(GetText(Resource.Color.ExtremeColor)));
            }
        }

        private void SpinnerRow_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var metrics = Resources.DisplayMetrics;
            var row = Convert.ToInt32(spinnerRow.SelectedItem);
            var col = Convert.ToInt32(spinnerCol.SelectedItem);

            if (chkConstraint.Checked && sender == this.spinnerCol)
            {
                int w = metrics.WidthPixels / col;

                var h = metrics.HeightPixels - toolbar.Height - w/2;
                row = h / w;
                spinnerRow.SetSelection(row - MIN_ROWS);
            }
            
                    
            this.seekBarMines.Max = (int) (row * col * 0.5);
            //recaculate risk.
            UpdateRiskRating();
        }

        private void InitSpinner(Spinner spinner, int min, int max)
        {
            spinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, Enumerable.Range(min, max).ToArray());
            
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case Resource.Id.mnuSaveSetting:
                    this.settings = GetUserSettings();
                    SaveSetting(settings, this);
                    GoBack<MainActivity>();
                    break;
                case Resource.Id.miScoreBoard:
                    GoBack<ScoreActivity>();
                    break;
            }
            return base.OnOptionsItemSelected(item);

        }
        private AppSetting GetUserSettings()
        {
            var currentSetting = new AppSetting();
            switch(this.radGroup.CheckedRadioButtonId)
            {
                case Resource.Id.radioButton1:
                    currentSetting.Level = GameLevel.Easy;
                    break;

                case Resource.Id.radioButton2:
                    currentSetting.Level = GameLevel.Normal;
                    break;

                case Resource.Id.radioButton3:
                    currentSetting.Level = GameLevel.Hard;
                    break;

                case Resource.Id.radioButton4:
                    currentSetting.Level = GameLevel.VeryHard;
                    break;

                case Resource.Id.radioButton5:
                    currentSetting.Level = GameLevel.ExtremHard;
                    break;

                default:
                    currentSetting.Level = GameLevel.Custom;
                    currentSetting.Rows = Convert.ToInt32(FindViewById<Spinner>(Resource.Id.spinnerRow).SelectedItem);
                    currentSetting.Cols = Convert.ToInt32(FindViewById<Spinner>(Resource.Id.spinnerCol).SelectedItem);
                    currentSetting.Mines = this.seekBarMines.Progress;
                    break;
            }

            return currentSetting;
        }
        private void GoBack<T>()
        {
            this.OnNavigateUp();
            var intent = new Intent(this, typeof(T));
            StartActivity(intent);
        }
    }
}