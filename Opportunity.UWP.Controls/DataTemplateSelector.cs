using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Opportunity.UWP.Controls
{
    [ContentProperty(Name = nameof(Templates))]
    public class DataTemplateSelector : Windows.UI.Xaml.Controls.DataTemplateSelector
    {
        public DataTemplateSelector()
        {
            this.Templates = new DataTemplateCollection();
        }

        public DataTemplateCollection Templates { get; }

        public DataTemplate DefaultTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item != null)
            {
                foreach (var template in this.Templates)
                {
                    if (template.KeyType is null)
                        continue;
                    if (template.KeyType.IsInstanceOfType(item))
                        return template.Value;
                }
            }

            if (this.DefaultTemplate != null)
                return this.DefaultTemplate;
            return base.SelectTemplateCore(item);
        }
    }

    public class DataTemplateCollection : ObservableCollection<DataTemplateKeyValuePair>
    {
        internal DataTemplateCollection()
        {
        }
    }

    [ContentProperty(Name = nameof(Value))]
    public class DataTemplateKeyValuePair : DependencyObject
    {
        public string Key
        {
            get => (string)GetValue(KeyProperty); set => SetValue(KeyProperty, value);
        }
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(nameof(Key), typeof(string), typeof(DataTemplateKeyValuePair), new PropertyMetadata("", KeyChangedCallback));

        public Type KeyType
        {
            get => (Type)GetValue(KeyTypeProperty); set => SetValue(KeyTypeProperty, value);
        }
        public static readonly DependencyProperty KeyTypeProperty =
            DependencyProperty.Register(nameof(KeyType), typeof(Type), typeof(DataTemplateKeyValuePair), new PropertyMetadata(null, KeyTypeChangedCallback));

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
            var sender = (DataTemplateKeyValuePair)d;
            var o = (Type)e.OldValue;
            var n = (Type)e.NewValue;
            if (o == n)
                return;
            if (n == null)
            {
                sender.Key = "";
                return;
            }
            sender.Key = n.FullName;
        }

        public DataTemplate Value
        {
            get => (DataTemplate)GetValue(ValueProperty); set => SetValue(ValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(DataTemplate), typeof(DataTemplateKeyValuePair), new PropertyMetadata(null));
    }
}
