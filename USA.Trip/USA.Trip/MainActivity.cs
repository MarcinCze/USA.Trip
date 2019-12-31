using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using USA.Trip.Views;

namespace USA.Trip
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        NavigationView navigationView;
        ViewFlipper viewFlipper;
        List<iView> views;

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

            LoadViews();
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
                case Resource.Id.nav_home:
                    viewFlipper.DisplayedChild = Constants.ContentView.Home;
                    break;
                case Resource.Id.nav_flight_usa:
                    viewFlipper.DisplayedChild = Constants.ContentView.FlightToNyc;
                    break;
                case Resource.Id.nav_flight_pol:
                    viewFlipper.DisplayedChild = Constants.ContentView.FlightHome;
                    break;
                case Resource.Id.nav_budget:
                    viewFlipper.DisplayedChild = Constants.ContentView.Budget;
                    break;
                case Resource.Id.nav_income:
                    viewFlipper.DisplayedChild = Constants.ContentView.Income;
                    break;
                case Resource.Id.nav_expenses:
                    viewFlipper.DisplayedChild = Constants.ContentView.Expenses;
                    break;
                default:
                    Toast.MakeText(this, "No view found", ToastLength.Short).Show();
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

        protected void LoadViews()
        {
            views = new List<iView>
            {
                new Budget(),
                new Expenses(),
                new FlightToPoland(),
                new FlightToUsa(),
                new Home(),
                new Income()
            };

            foreach (var view in views)
            {
                view.OnCreate(this);
            }
        }
    }
}

