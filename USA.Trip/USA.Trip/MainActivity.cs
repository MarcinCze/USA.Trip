using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

using USA.Trip.Models;

using static Android.Widget.SeekBar;

namespace USA.Trip
{
    [Activity(Label = "@string/app_name", 
        Theme = "@style/AppTheme.NoActionBar", 
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.Orientation, 
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        LocalStorage localStorage;

        ViewFlipper viewFlipper;
        NavigationView navigationView;
        Switch flightNycSwitch, flightKrkSwitch;
        ImageButton othersSubwayButtonOpen;
        FloatingActionButton budgetExpensesFloatBtn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            viewFlipper = FindViewById<ViewFlipper>(Resource.Id.viewFlipper);

            Window.SetBackgroundDrawableResource(Resource.Drawable.background);

            Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
            Window.AddFlags(WindowManagerFlags.LayoutInScreen);
            Window.DecorView.SetFitsSystemWindows(true);

            Initialize();
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.nav_dashboard:
                    viewFlipper.DisplayedChild = Constants.Views.Dashboard;
                    break;
                case Resource.Id.nav_flight_nyc:
                    viewFlipper.DisplayedChild = Constants.Views.FlightNyc;
                    break;
                case Resource.Id.nav_flight_krk:
                    viewFlipper.DisplayedChild = Constants.Views.FlightKrk;
                    break;
                case Resource.Id.nav_budget_summary:
                    viewFlipper.DisplayedChild = Constants.Views.BudgetSummary;
                    break;
                case Resource.Id.nav_budget_income:
                    viewFlipper.DisplayedChild = Constants.Views.BudgetIncome;
                    break;
                case Resource.Id.nav_budget_expenses:
                    viewFlipper.DisplayedChild = Constants.Views.BudgetExpenses;
                    break;
                case Resource.Id.nav_others_hotel:
                    viewFlipper.DisplayedChild = Constants.Views.OthersHotel;
                    break;
                case Resource.Id.nav_others_subway:
                    viewFlipper.DisplayedChild = Constants.Views.OthersSubway;
                    break;
                default:
                    Toast.MakeText(Application.Context, "View not found", ToastLength.Short).Show();
                    break;
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void Initialize()
        {
            localStorage = new LocalStorage(this);

            Task.WaitAll(new Task[]
            {
                Task.Factory.StartNew(() =>
                {
                    flightNycSwitch = FindViewById<Switch>(Resource.Id.flightNycSwitch);
                    flightNycSwitch.Click += FlightNycSwitch_Click;
                    flightKrkSwitch = FindViewById<Switch>(Resource.Id.flightKrkSwitch);
                    flightKrkSwitch.Click += FlightKrkSwitch_Click;
                    budgetExpensesFloatBtn = FindViewById<FloatingActionButton>(Resource.Id.budgetExpensesAddButton);
                    budgetExpensesFloatBtn.Click += BudgetExpensesFloatBtn_Click;
                }),

                Task.Factory.StartNew(() => {
                    FindViewById<TextView>(Resource.Id.budgetIncomeCashLabel).Text = $"$ {localStorage.LocalSettings.Budget.Cash:0.00}";
                    FindViewById<TextView>(Resource.Id.budgetIncomeCardLabel).Text = $"$ {localStorage.LocalSettings.Budget.Card:0.00}";
                    FindViewById<Button>(Resource.Id.budgetIncomeCashChangeBtn).Click += BudgetIncomeChangeBtn_Click;
                    FindViewById<Button>(Resource.Id.budgetIncomeCardChangeBtn).Click += BudgetIncomeChangeBtn_Click;
                }),

                Task.Factory.StartNew(() => {
                    FindViewById<RelativeLayout>(Resource.Id.flightNycFlight1).Visibility = ViewStates.Visible;
                    FindViewById<RelativeLayout>(Resource.Id.flightNycFlight2).Visibility = ViewStates.Invisible;
                    FindViewById<RelativeLayout>(Resource.Id.flightKrkFlight1).Visibility = ViewStates.Visible;
                    FindViewById<RelativeLayout>(Resource.Id.flightKrkFlight2).Visibility = ViewStates.Invisible;
                }),

                Task.Factory.StartNew(() => {
                    othersSubwayButtonOpen = FindViewById<ImageButton>(Resource.Id.othersSubwayButtonOpen);
                    othersSubwayButtonOpen.Click += OthersSubwayButtonOpen_Click;
                }),

                Task.Factory.StartNew(() => {
                    UpdateBudgetSummary();
                })
            });

            var listView = FindViewById<ListView>(Resource.Id.budgetExpensesListView);
            listView.Adapter = BudgetExpensesAdapterFactory.Create(this, localStorage);
            listView.ItemClick += ListView_ItemClick;
            listView.ItemLongClick += ListView_ItemLongClick;
        }

        private void BudgetIncomeChangeBtn_Click(object sender, EventArgs e)
        {
            bool isCash = ((Button)sender).Tag.ToString().Equals("CASH");

            try
            {
                LayoutInflater layoutInflater = LayoutInflater.From(this);
                View view = layoutInflater.Inflate(Resource.Layout.user_income_input_box, null);
                Android.Support.V7.App.AlertDialog.Builder alertbuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
                alertbuilder.SetView(view);

                view.FindViewById<TextView>(Resource.Id.incomeInputTitle).Text = isCash ? "Cash balance" : "Card balance";
                view.FindViewById<EditText>(Resource.Id.incomeInputAmount).Text = isCash ? 
                    localStorage.LocalSettings.Budget.Cash.ToString(CultureInfo.InvariantCulture)
                    : localStorage.LocalSettings.Budget.Card.ToString(CultureInfo.InvariantCulture);

                alertbuilder.SetCancelable(false)
                .SetPositiveButton("OK", delegate
                {
                    double? amount;
                    try
                    {
                        amount = double.Parse(view.FindViewById<EditText>(Resource.Id.incomeInputAmount).Text, CultureInfo.InvariantCulture);
                    }
                    catch (Exception ex)
                    {
                        amount = null;
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
                    }

                    if (amount != null)
                    {
                        if (isCash)
                            localStorage.LocalSettings.Budget.Cash = amount.Value;
                        else
                            localStorage.LocalSettings.Budget.Card = amount.Value;
                    }

                    Task.Run(UpdateBudgetSummary);
                    Task.Run(() => localStorage.Save(this));
                    FindViewById<TextView>(Resource.Id.budgetIncomeCashLabel).Text = $"$ {localStorage.LocalSettings.Budget.Cash:0.00}";
                    FindViewById<TextView>(Resource.Id.budgetIncomeCardLabel).Text = $"$ {localStorage.LocalSettings.Budget.Card:0.00}";
                })
                .SetNegativeButton("Cancel", delegate
                {
                    alertbuilder.Dispose();
                });
                Android.Support.V7.App.AlertDialog dialog = alertbuilder.Create();
                dialog.Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        private void ListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            OutcomeEntry entry = localStorage.LocalSettings.Expenses.ElementAt((int)e.Id);
            Android.Support.V7.App.AlertDialog.Builder alertDiag = new Android.Support.V7.App.AlertDialog.Builder(this);
            alertDiag.SetTitle("Delete");
            alertDiag.SetMessage($"Are you sure that you want to remove {entry.Name}?");
            alertDiag.SetPositiveButton("Delete", (senderAlert, args) => {

                localStorage.LocalSettings.Expenses.RemoveAt((int)e.Id);
                Task.Run(UpdateBudgetSummary);
                Task.Run(() => localStorage.Save(this));
                FindViewById<ListView>(Resource.Id.budgetExpensesListView).Adapter = BudgetExpensesAdapterFactory.Create(this, localStorage);
            });
            alertDiag.SetNegativeButton("Cancel", (senderAlert, args) => {
                alertDiag.Dispose();
            });
            Dialog diag = alertDiag.Create();
            diag.Show();
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            OutcomeEntry entry = localStorage.LocalSettings.Expenses.ElementAt((int)e.Id);
            ExpensesModifyWindow(entry);
        }

        private void BudgetExpensesFloatBtn_Click(object sender, EventArgs e)
        {
            ExpensesModifyWindow(null);
        }

        private void OthersSubwayButtonOpen_Click(object sender, EventArgs e)
        {
            Toast.MakeText(Application.Context, $"NOT IMPLEMENTED", ToastLength.Long).Show();
        }

        private void FlightKrkSwitch_Click(object sender, EventArgs e)
        {
            var flight1 = FindViewById<RelativeLayout>(Resource.Id.flightKrkFlight1);
            var flight2 = FindViewById<RelativeLayout>(Resource.Id.flightKrkFlight2);

            if (flightKrkSwitch.Checked)
            {
                flight1.Visibility = ViewStates.Invisible;
                flight2.Visibility = ViewStates.Visible;
            }
            else
            {
                flight1.Visibility = ViewStates.Visible;
                flight2.Visibility = ViewStates.Invisible;
            }
        }

        private void FlightNycSwitch_Click(object sender, EventArgs e)
        {
            var flight1 = FindViewById<RelativeLayout>(Resource.Id.flightNycFlight1);
            var flight2 = FindViewById<RelativeLayout>(Resource.Id.flightNycFlight2);

            if (flightNycSwitch.Checked)
            {
                flight1.Visibility = ViewStates.Invisible;
                flight2.Visibility = ViewStates.Visible;
            }
            else
            {
                flight1.Visibility = ViewStates.Visible;
                flight2.Visibility = ViewStates.Invisible;
            }
        }

        private void ExpensesModifyWindow(OutcomeEntry entryToModify)
        {
            try
            {
                LayoutInflater layoutInflater = LayoutInflater.From(this);
                View view = layoutInflater.Inflate(Resource.Layout.user_expenses_input_box, null);
                Android.Support.V7.App.AlertDialog.Builder alertbuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
                alertbuilder.SetView(view);

                Task.Run(() => {
                    view.FindViewById<TextView>(Resource.Id.inputDialogText1).Text = "Title";
                    view.FindViewById<TextView>(Resource.Id.inputDialogText2).Text = "Amount";
                });

                DateTime creationTime = DateTime.Now;
                if (entryToModify != null)
                {
                    Task.Run(() => {
                        creationTime = entryToModify.Date;
                        view.FindViewById<TextView>(Resource.Id.inputDialogInputTitleTxt).Text = entryToModify.Name;
                        view.FindViewById<TextView>(Resource.Id.inputDialogInputAmountTxt).Text = entryToModify.Amount.ToString(CultureInfo.InvariantCulture);
                        view.FindViewById<RadioButton>(Resource.Id.inputDialogInputPaymentTypeRadioCash).Checked = entryToModify.Payment == PaymentMethod.Cash;
                        view.FindViewById<RadioButton>(Resource.Id.inputDialogInputPaymentTypeRadioCard).Checked = entryToModify.Payment == PaymentMethod.Card;
                    });
                }

                view.FindViewById<TextView>(Resource.Id.inputDialogText3).Text = $"{creationTime:d.MM}";
                var bar = view.FindViewById<SeekBar>(Resource.Id.inputDialogInputDateSeekBar);
                bar.Max = creationTime.AddDays(+15).DayOfYear;
                bar.Min = creationTime.AddDays(-15).DayOfYear;
                bar.SetProgress(creationTime.DayOfYear, true);
                bar.ProgressChanged += new EventHandler<ProgressChangedEventArgs>(delegate (object sender, ProgressChangedEventArgs a)
                {
                    var theDate = new DateTime(2020, 1, 1).AddDays(bar.Progress - 1);
                    view.FindViewById<TextView>(Resource.Id.inputDialogText3).Text = $"{theDate:d.MM}";
                });

                alertbuilder.SetCancelable(false)
                .SetPositiveButton("OK", delegate
                {
                    string title = view.FindViewById<EditText>(Resource.Id.inputDialogInputTitleTxt).Text;
                    string amountString = view.FindViewById<EditText>(Resource.Id.inputDialogInputAmountTxt).Text;
                    double amount = double.Parse(amountString, CultureInfo.InvariantCulture);
                    DateTime creationTime = new DateTime(2020, 1, 1).AddDays(bar.Progress - 1);
                    PaymentMethod method = view.FindViewById<RadioButton>(Resource.Id.inputDialogInputPaymentTypeRadioCash).Checked ? PaymentMethod.Cash : PaymentMethod.Card;

                    if (entryToModify == null)
                    {
                        localStorage.LocalSettings.Expenses.Add(new OutcomeEntry(title, amount, method, creationTime));
                    }
                    else
                    {
                        var entry = localStorage.LocalSettings.Expenses.First(x => x.Id == entryToModify.Id);
                        entry.Name = title;
                        entry.Amount = amount;
                        entry.Date = creationTime;
                        entry.Payment = method;
                    }
                    
                    localStorage.LocalSettings.Expenses.OrderBy(x => x.Date);

                    Task.Run(UpdateBudgetSummary);
                    Task.Run(() => localStorage.Save(this));
                    var listView = FindViewById<ListView>(Resource.Id.budgetExpensesListView);
                    listView.Adapter = BudgetExpensesAdapterFactory.Create(this, localStorage);
                })
                .SetNegativeButton("Cancel", delegate
                {
                    alertbuilder.Dispose();
                });
                Android.Support.V7.App.AlertDialog dialog = alertbuilder.Create();
                dialog.Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        private void UpdateBudgetSummary()
        {
            FindViewById<TextView>(Resource.Id.budgetSummaryIncomeLabel).Text =
                $"$ {localStorage.LocalSettings.Budget.Cash + localStorage.LocalSettings.Budget.Card:0.00}";
            FindViewById<TextView>(Resource.Id.budgetSummaryExpensesLabel).Text =
                $"$ {localStorage.LocalSettings.Expenses.Sum(x => x.Amount):0.00}";
            FindViewById<TextView>(Resource.Id.budgetSummaryRestCashLabel).Text =
                $"$ {localStorage.LocalSettings.Budget.Cash - localStorage.LocalSettings.Expenses.Where(x => x.Payment == PaymentMethod.Cash).Sum(x => x.Amount):0.00}";
            FindViewById<TextView>(Resource.Id.budgetSummaryRestCardLabel).Text =
                $"$ {localStorage.LocalSettings.Budget.Card - localStorage.LocalSettings.Expenses.Where(x => x.Payment == PaymentMethod.Card).Sum(x => x.Amount):0.00}";
        }
    }
}

