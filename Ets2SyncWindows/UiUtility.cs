using System.Windows;
using System.Windows.Media;

namespace Ets2SyncWindows
{
    public static class UiUtility
    {
        public static T FindParent<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(dependencyObject);

            // We've reached the root of the tree
            if (parentObject == null)
                return null;

            if (parentObject is T parent)
                return parent;

            return FindParent<T>(parentObject);
        }
    }
}