using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

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
}
