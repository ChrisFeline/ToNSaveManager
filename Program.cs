using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ToNSaveManager
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainWindow());
        }

        internal static void QuickSort<T>(IList<T> collection, Comparison<T> comparison) where T : IComparable<T>
        {
            QuickSort(collection, 0, collection.Count - 1, comparison);
        }

        private static void QuickSort<T>(IList<T> arr, int left, int right, Comparison<T> comparison) where T : IComparable<T>
        {
            if (left < right)
            {
                int pivot = Partition(arr, left, right, comparison);

                if (pivot > 1)
                {
                    QuickSort(arr, left, pivot - 1, comparison);
                }
                if (pivot + 1 < right)
                {
                    QuickSort(arr, pivot + 1, right, comparison);
                }
            }
        }

        private static int Partition<T>(IList<T> arr, int left, int right, Comparison<T> comparison) where T : IComparable<T>
        {
            T pivot = arr[left];
            while (true)
            {
                while (comparison.Invoke(arr[left], pivot) < 0)
                {
                    left++;
                }

                while (comparison.Invoke(arr[right], pivot) > 0)
                {
                    right--;
                }

                if (left < right)
                {
                    if (comparison.Invoke(arr[left], arr[right]) == 0) return right;
                    (arr[right], arr[left]) = (arr[left], arr[right]);
                }
                else
                {
                    return right;
                }
            }
        }
    }
}