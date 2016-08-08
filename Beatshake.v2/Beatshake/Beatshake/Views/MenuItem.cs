using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Beatshake.Views
{
    public class MenuItem : BindableObject
    {

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            "Title", typeof(string), typeof(MenuItem), "default entry");

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(
            "IconSource", typeof(string), typeof(MenuItem), default(string));

        public string IconSource
        {
            get { return (string) GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        public static readonly BindableProperty TargetTypeProperty = BindableProperty.Create(
            "TargetType", typeof(Type), typeof(MenuItem), default(Type));

        public Type TargetType
        {
            get { return (Type) GetValue(TargetTypeProperty); }
            set { SetValue(TargetTypeProperty, value); }
        }

    }
}
