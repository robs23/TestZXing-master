using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestZXing
{
    

    public class CustomLabel : Label
    {
        public static readonly BindableProperty BgColorProperty =
            BindableProperty.Create("BgColor", typeof(Color), typeof(CustomLabel), Color.Default);

        public CustomLabel()
        {

        }

        public Color BgColor
        {
            get
            {
                return (Color)GetValue(BgColorProperty);
            }
            set
            {
                SetValue(BgColorProperty, value);
            }
        }
    }
}
