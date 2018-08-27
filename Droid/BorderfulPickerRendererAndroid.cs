using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TestZXing;
using TestZXing.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BorderfulPicker), typeof(BorderfulPickerRendererAndroid))]
namespace TestZXing.Droid
{
    public class BorderfulPickerRendererAndroid : Xamarin.Forms.Platform.Android.AppCompat.PickerRenderer
    {
        public BorderfulPickerRendererAndroid(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SetHintTextColor(Android.Graphics.Color.DeepSkyBlue);
                Control.Gravity = GravityFlags.Center;
                var gd = new GradientDrawable();
                gd.SetCornerRadius(60f);
                gd.SetStroke(6, Android.Graphics.Color.DeepSkyBlue);
                Control.SetBackground(gd);
            }
        }
    }
}