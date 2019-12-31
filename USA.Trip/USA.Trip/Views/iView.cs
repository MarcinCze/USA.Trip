using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace USA.Trip.Views
{
    public interface iView
    {
        void OnCreate(Activity activity);
        void FetchData(Activity activity);
    }
}