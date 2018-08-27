using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TestZXing;
using TestZXing.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BorderfulEntry),typeof(BorderfulEntryRendererAndroid))]
namespace TestZXing.Droid
{
    public class BorderfulEntryRendererAndroid : EntryRenderer
    {
        public BorderfulEntryRendererAndroid(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if(Control != null)
            {
                var gd = new GradientDrawable();
                gd.SetCornerRadius(60f);
                gd.SetStroke(6, Android.Graphics.Color.DeepSkyBlue);
                Control.SetBackground(gd);
            }
        }
    }
}