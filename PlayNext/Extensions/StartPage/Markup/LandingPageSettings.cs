using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PlayNext.Extensions.StartPage.Markup
{
    public class LandingPageSettings : MarkupExtension
    {
        public static Type GetTargetType(IProvideValueTarget provider)
        {
            var type = provider.TargetProperty?.GetType();
            if (type == null)
            {
                return typeof(Binding);
            }
            if (type == typeof(DependencyProperty))
            {
                type = ((DependencyProperty)provider.TargetProperty).PropertyType;
            }
            else if (provider.TargetProperty is PropertyInfo info)
            {
                type = info.PropertyType;
            }

            return type;
        }

        internal string path;
        public string Path { get => path; set => path = value; }

        public LandingPageSettings()
        {
        }

        public LandingPageSettings(string path)
        {
            this.path = path;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding()
            {
                Source = LandingPageExtension.Instance,
                Path = new PropertyPath(path),
                Mode = BindingMode.OneWay
            };

            var provider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            var targetType = GetTargetType(provider);

            if (targetType == typeof(Visibility))
            {
                binding.Converter = Application.Current.Resources["BooleanToHiddenConverter"] as IValueConverter;
            }

            return binding;
        }
    }
}