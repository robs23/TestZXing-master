using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Glide;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Xamarin.Forms;

namespace TestZXing.Droid.Services
{
    class GlideWithPlaceholder: IGlideHandler
    {
        public GlideWithPlaceholder()
        {
                
        }

        public bool Build(ImageView imageView, ImageSource source, RequestBuilder builder, CancellationToken token)
        {
            if (builder != null)
            {
                //easiest way - add the image to Android resources ....
                //general placeholder:
                //builder.Placeholder(Resource.Drawable.MSicc_Logo_Base_Blue_1024px_pad25).Into(imageView);
                //error placeholder:
                builder.Placeholder(Resource.Drawable.downloading_512).Into(imageView);
                return true;
            }
            else
                return false;
        }
    }
}