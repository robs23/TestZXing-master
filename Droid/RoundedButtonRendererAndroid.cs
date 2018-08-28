using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TestZXing;
using TestZXing.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RoundedButton), typeof(RoundedButtonRendererAndroid))]
namespace TestZXing.Droid
{
    public class RoundedButtonRendererAndroid : ButtonRenderer
    {
        public RoundedButtonRendererAndroid(Context context) : base(context)
        {
                
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            if(Control != null)
            {
                var gd = new GradientDrawable();
                gd.SetCornerRadius(60f);
                gd.SetColor(this.Element.BackgroundColor.ToAndroid());
                gd.SetStroke(6, this.Element.BackgroundColor.ToAndroid());
                var stateList = new StateListDrawable();
                var rippleItem = new RippleDrawable(ColorStateList.ValueOf(Android.Graphics.Color.White), gd, null);
                stateList.AddState(new[] { Android.Resource.Attribute.StateEnabled }, rippleItem);  
                Control.SetBackground(stateList);
            }
        }
    }
}