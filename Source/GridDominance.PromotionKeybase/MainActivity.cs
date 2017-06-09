using System;
using Android.App;
using Android.Widget;
using Android.OS;
using GridDominance.Shared.Resources;

namespace GridDominance.PromotionKeybase
{
	[Activity(Label = "Promo Manager", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		private TextView lblCode;
		private ImageButton btnCodeGen;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			lblCode = FindViewById<TextView> (Resource.Id.lblPromoCode);

			btnCodeGen = FindViewById<ImageButton> (Resource.Id.btnCodeGen);
			btnCodeGen.Enabled = true;
			btnCodeGen.Click += GenerateNoise;
			btnCodeGen.LongClick += GenerateCode;
		}

		private void GenerateNoise(object sender, EventArgs eventArgs)
		{
			var dn = DateTime.Now;
			var n = (dn.Year << 17) | (dn.DayOfYear << 8) | (dn.Hour << 3) | (dn.Minute/10);

			lblCode.Text = $"{(new Random(n).Next() % 100000000):00000000}";
			btnCodeGen.SetImageResource (Resource.Drawable.finger);
		}

		private void GenerateCode(object sender, Android.Views.View.LongClickEventArgs e)
		{
			lblCode.Text = __Secrets.UnlockCode();
			btnCodeGen.SetImageResource (Resource.Drawable.finger2);
		}
	}
}