using System;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Opportunity.UWP.Controls
{
    [ContentProperty(Name = nameof(Value))]
    public class DataTemplateKeyValuePair : DependencyObject
    {
        public string Key
        {
            get => (string)this.GetValue(KeyProperty); set => this.SetValue(KeyProperty, value);
        }
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(nameof(Key), typeof(string), typeof(DataTemplateKeyValuePair), new PropertyMetadata("", KeyChangedCallback));

        public Type KeyType
        {
            get => (Type)this.GetValue(KeyTypeProperty); set => this.SetValue(KeyTypeProperty, value);
        }
        public static readonly DependencyProperty KeyTypeProperty =
            DependencyProperty.Register(nameof(KeyType), typeof(Type), typeof(DataTemplateKeyValuePair), new PropertyMetadata(typeof(object), KeyTypeChangedCallback));

        private static readonly Assembly[] Assemblies = new[]
        {
            Application.Current.GetType().GetTypeInfo().Assembly,
            typeof(DependencyObject).GetTypeInfo().Assembly,
        };

        private Type GetType(string name)
        {
            name = name.Trim();
            var callerAssembly = this.Parent();
            foreach (var item in Assemblies)
            {
                var t = item.GetType(name, false, false);
                if (t != null)
                    return t;
            }
            return Type.GetType(name, true);
        }

        private static void KeyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = (DataTemplateKeyValuePair)d;
            var o = (string)e.OldValue;
            var n = (string)e.NewValue;
            if (o == n)
                return;
            if (string.IsNullOrWhiteSpace(n))
            {
                sender.KeyType = null;
                return;
            }
            sender.KeyType = sender.GetType(n);
        }

        private static void KeyTypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is null)
                throw new ArgumentNullException();
            var sender = (DataTemplateKeyValuePair)d;
            var o = e.OldValue is Type ? (Type)e.OldValue : default;
            var n = e.NewValue is Type ? (Type)e.NewValue : default;
            if (o == n)
                return;
            sender.Key = n.FullName;
        }

        public DataTemplate Value
        {
            get => (DataTemplate)this.GetValue(ValueProperty); set => this.SetValue(ValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(DataTemplate), typeof(DataTemplateKeyValuePair), new PropertyMetadata(null));
    }
}
