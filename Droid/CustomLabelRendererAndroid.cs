using System;
using System.Collections.Generic;
using System.ComponentModel;
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

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRendererAndroid))]
namespace TestZXing.Droid
{
    public class CustomLabelRendererAndroid : LabelRenderer
    {
        public CustomLabelRendererAndroid(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                
                Control.Gravity = GravityFlags.Center;
                var gd = new GradientDrawable();
                gd.SetCornerRadius(60f);
                gd.SetStroke(6, Android.Graphics.Color.Transparent);
                if (e.NewElement != null)
                {
                    var element = e.NewElement as CustomLabel;
                    if(element.BgColor != Color.Transparent)
                    {
                        gd.SetColor(element.BgColor.ToAndroid());
                    }
                }
                Control.SetBackground(gd);
            }
        }
    }
}