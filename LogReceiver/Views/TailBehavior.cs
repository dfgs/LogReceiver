using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LogReceiver.Views
{
	public class TailBehavior
	{
        private static Dictionary<ITailProvider, ListView> items;

        public static readonly DependencyProperty TailProviderProperty = DependencyProperty.RegisterAttached("TailProvider", typeof(ITailProvider), typeof(TailBehavior), new UIPropertyMetadata(null, OnTailProviderChanged));

        public static ITailProvider GetTailProvider(DependencyObject obj)
        {
            return (ITailProvider)obj.GetValue(TailProviderProperty);
        }

        public static void SetTailProvider(DependencyObject obj, ITailProvider value)
        {
            obj.SetValue(TailProviderProperty, value);
        }

        static TailBehavior()
		{
            items = new Dictionary<ITailProvider, ListView>();
		}

        public static void OnTailProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ITailProvider oldValue, newValue;
            ListView listView;

            listView = d as ListView;

            if (listView == null) return;

            oldValue = (ITailProvider)e.OldValue; newValue = (ITailProvider)e.NewValue;
            if (newValue == oldValue) return;

            if (oldValue != null) oldValue.UpdateScroll -= UpdateScroll;
            if (newValue != null)
            {
                newValue.UpdateScroll += UpdateScroll;
                items[newValue] = listView;
            }
           
        }

		private static void UpdateScroll(object sender, TailEventArgs e)
		{
            ListView listView;

            if (!items.TryGetValue((ITailProvider)sender, out listView)) return;

            listView.ScrollIntoView(e.Item);
		}



	}
}
