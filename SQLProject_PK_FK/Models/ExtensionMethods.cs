using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace SQLProject.Models
{
    public static class ExtensionMethods

    {
        private static Action EmptyDelegate = delegate () { };


        public static void Refresh(this UIElement uiElement)

        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
        public static void LoopingMethod(this UIElement uiElement)
        {
            uiElement.Refresh();
            Thread.Sleep(500);

        }
    }
    
}
