using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace NPUiControl
{
    /// <summary>
    ///     WPF UI帮助类
    /// </summary>
    public class UiHelper
    {
        /// <summary>
        ///     A value indicating whether the UI is currently busy
        /// </summary>
        private static bool IsBusy;

        /// <summary>
        ///     找到符合条件的兄弟控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="control">传入参数</param>
        /// <param name="siblingName">寻找的控件名</param>
        /// <returns>找到的控件对象</returns>
        public static T FindSibling<T>(DependencyObject control, string siblingName)
            where T : DependencyObject
        {
            if (control == null)
                return null;

            var parent = VisualTreeHelper.GetParent(control);

            if (parent != null)
            {
                var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
                for (var i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    // If the child is not of the request child type child
                    var childType = child as T;
                    if (childType != null)
                    {
                        if (!string.IsNullOrEmpty(siblingName))
                        {
                            var frameworkElement = child as FrameworkElement;
                            // If the child's name is set for search
                            if (frameworkElement != null && frameworkElement.Name == siblingName)
                            {
                                // if the child's name is of the request name
                                return (T)child;
                            }
                        }
                        return (T)child;
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Finds a Child of a given item in the visual tree.
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>
        ///     The first parent item that matches the submitted type parameter.
        ///     If not matching item can be found,
        ///     a null parent is being returned.
        /// </returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                var childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T) child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T) child;
                    break;
                }
            }

            return foundChild;
        }

        /// <summary>
        ///     Sets the busystate as busy.
        /// </summary>
        public static void SetBusyState()
        {
            SetBusyState(true);
        }

        /// <summary>
        ///     Sets the busystate to busy or not busy.
        /// </summary>
        /// <param name="busy">if set to <c>true</c> the application is now busy.</param>
        private static void SetBusyState(bool busy)
        {
            if (busy != IsBusy)
            {
                IsBusy = busy;
                Mouse.OverrideCursor = busy ? Cursors.Wait : null;

                if (IsBusy)
                    new DispatcherTimer(TimeSpan.FromSeconds(0), DispatcherPriority.ApplicationIdle,
                        dispatcherTimer_Tick, Application.Current.Dispatcher);
            }
        }

        /// <summary>
        ///     Handles the Tick event of the dispatcherTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private static void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var dispatcherTimer = sender as DispatcherTimer;
            if (dispatcherTimer != null)
            {
                SetBusyState(false);
                dispatcherTimer.Stop();
            }
        }
    }
}