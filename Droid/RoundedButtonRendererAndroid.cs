using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                SetColors();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(Xamarin.Forms.Button.IsEnabled))
            {
                SetColors();
            }
        }

        private void SetColors()
        {
            var gd = new GradientDrawable();
            gd.SetCornerRadius(60f);
            if (Element.IsEnabled)
            {
                gd.SetColor(this.Element.BackgroundColor.ToAndroid());
                gd.SetStroke(6, this.Element.BackgroundColor.ToAndroid());
                var stateList = new StateListDrawable();
                var rippleItem = new RippleDrawable(ColorStateList.ValueOf(Android.Graphics.Color.White), gd, null);
                stateList.AddState(new[] { Android.Resource.Attribute.StateEnabled }, rippleItem);
                Control.SetBackground(stateList);
            }
            else
            {
                gd.SetColor(Android.Graphics.Color.LightGray);
                gd.SetStroke(6, Android.Graphics.Color.LightGray);
                Control.SetBackground(gd);
            }
            
        }
    }
}