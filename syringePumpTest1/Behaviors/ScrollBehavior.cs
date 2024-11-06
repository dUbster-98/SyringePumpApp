using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace SyringePumpTest1.Behaviors
{
    public class ScrollBehavior : Behavior<TextBox>
    {
        private static void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox?.Text == null)
                return;

            Action action = () =>
            {
                textbox.UpdateLayout();

                if (textbox.Text != null)
                {
                    textbox.ScrollToEnd();
                }
            };

            textbox.Dispatcher.BeginInvoke(action);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += AssociatedObject_TextChanged;
        }

        protected override void OnDetaching() 
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        }
    }
}
