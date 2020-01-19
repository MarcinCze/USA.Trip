using Android.App;
using Android.Views;
using Android.Widget;

using System.Collections.Generic;

namespace USA.Trip
{
    public static class BudgetExpensesAdapterFactory
    {
        public static BudgetExpensesAdapter Create(Activity context, LocalStorage localStorage)
        {
            return new BudgetExpensesAdapter(context, localStorage.LocalSettings.Expenses);
        }
    }

    public class BudgetExpensesAdapter : BaseAdapter<OutcomeEntry>
    {
        List<OutcomeEntry> items;
        Activity context;

        public BudgetExpensesAdapter(Activity context, List<OutcomeEntry> items) : base()
        {
            this.context = context;
            this.items = items;
        }

        public override long GetItemId(int position) => position;

        public override OutcomeEntry this[int position] => items[position];

        public override int Count => items.Count;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new 
                view = context.LayoutInflater.Inflate(Resource.Layout.list_expenses_item, null);

            view.FindViewById<TextView>(Resource.Id.listExpensesItemName).Text = item.Name;
            view.FindViewById<TextView>(Resource.Id.listExpensesItemAmount).Text = $"$ {item.Amount:0.00}";
            view.FindViewById<TextView>(Resource.Id.listExpensesItemDateTime).Text = $"{item.Date:d.MM}";

            view.FindViewById<ImageView>(Resource.Id.listExpensesItemPaymentIcon)
                .SetImageResource(item.Payment == PaymentMethod.Card ? Resource.Drawable.icon_credit_card_256 : Resource.Drawable.icon_money_256);

            return view;
        }
    }
}