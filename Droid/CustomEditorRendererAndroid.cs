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

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRendererAndroid))]
namespace TestZXing.Droid
{
    public class CustomEditorRendererAndroid : EditorRenderer
    {
        public CustomEditorRendererAndroid(Context context) : base(context)
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if(Control != null)
            {
                if(e.NewElement != null)
                {
                    var element = e.NewElement as CustomEditor;
                    this.Control.Hint = element.Placeholder;
                    this.Control.SetHintTextColor(Android.Graphics.Color.DeepSkyBlue);
                }
                Control.Gravity = GravityFlags.Center;
                var gd = new GradientDrawable();
                gd.SetCornerRadius(60f);
                gd.SetStroke(6, Android.Graphics.Color.DeepSkyBlue);
                Control.SetBackground(gd);
            }
        }
    }
}