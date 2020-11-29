using LogLib;
using LogReceiver.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace LogReceiver.Views
{
	public class ComponentViewModelBehavior
	{
        private static Dictionary<ComponentViewModel, RichTextBox> items;

        public static readonly DependencyProperty ComponentViewModelProperty = DependencyProperty.RegisterAttached("ComponentViewModel", typeof(ComponentViewModel), typeof(ComponentViewModelBehavior), new UIPropertyMetadata(null, OnComponentViewModelChanged));

        public static ComponentViewModel GetComponentViewModel(DependencyObject obj)
        {
            return (ComponentViewModel)obj.GetValue(ComponentViewModelProperty);
        }

        public static void SetComponentViewModel(DependencyObject obj, ComponentViewModel value)
        {
            obj.SetValue(ComponentViewModelProperty, value);
        }

        static ComponentViewModelBehavior()
		{
            items = new Dictionary<ComponentViewModel, RichTextBox>();
		}

        public static void OnComponentViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComponentViewModel oldValue, newValue;
            RichTextBox richTextBox;

            richTextBox = d as RichTextBox;

            if (richTextBox == null) return;

            oldValue = (ComponentViewModel)e.OldValue; newValue = (ComponentViewModel)e.NewValue;
            if (newValue == oldValue) return;

            if (oldValue != null)
            {
                oldValue.LogAdded -= LogAdded;
                oldValue.LogRemoved -= LogRemoved;
                oldValue.UpdateScroll -= UpdateScroll;
            }
            if (newValue != null)
            {
                foreach(Log log in newValue.Items)
				{
                    WriteLog(richTextBox, log);
				}

				newValue.LogAdded += LogAdded;
                newValue.LogRemoved += LogRemoved;
                newValue.UpdateScroll += UpdateScroll;
                items[newValue] = richTextBox;
            }

        }


		private static void UpdateScroll(object sender, TailEventArgs e)
		{
            RichTextBox richTextBox;

            if (!items.TryGetValue((ComponentViewModel)sender, out richTextBox)) return;

            richTextBox.ScrollToEnd();
        }


        private static void LogAdded(object sender, LogEventArgs e)
        {
            RichTextBox richTextBox;
 
            if (!items.TryGetValue((ComponentViewModel)sender, out richTextBox)) return;
     
            WriteLog(richTextBox, e.Item);
        }

        private static void LogRemoved(object sender, LogEventArgs e)
        {
            RichTextBox richTextBox;

            if (!items.TryGetValue((ComponentViewModel)sender, out richTextBox)) return;
            

        }

        private static void WriteLog(RichTextBox RichTextBox, Log Log)
        {
            SolidColorBrush brush;

            switch (Log.Level)
            {
                case LogLevels.Debug:
                    brush = Brushes.Gray;
                    break;
                case LogLevels.Information:
                    brush = Brushes.White;
                    break;
                case LogLevels.Warning:
                    brush = Brushes.Orange;
                    break;
                case LogLevels.Error:
                    brush = Brushes.Red;
                    break;
                case LogLevels.Fatal:
                    brush = Brushes.Magenta;
                    break;
                default:
                    brush = Brushes.White;
                    break;
            }
            TextRange tr = new TextRange(RichTextBox.Document.ContentEnd, RichTextBox.Document.ContentEnd);
            tr.Text = $"{Log.DateTime}  {Log.Level.ToString().PadRight(12)}  {Log.Message}\r";
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
        }




    }
}
