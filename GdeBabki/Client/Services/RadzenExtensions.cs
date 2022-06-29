using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace GdeBabki.Client.Services
{
    public static class RadzenExtensions
    {
        public static void SetFilterValue<TItem>(this RadzenDataGridColumn<TItem> grid, object value, bool isFirst = true)
        {
            var methodInfo = grid.GetType().GetMethod("SetFilterValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            methodInfo.Invoke(grid, new object[] { value, isFirst });
        }

        public static object GetFilterValue<TItem>(this RadzenDataGridColumn<TItem> grid)
        {
            var methodInfo = grid.GetType().GetMethod("GetFilterValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return methodInfo.Invoke(grid, null);
        }

        public static void OnFilter<TItem>(this RadzenDataGrid<TItem> grid, ChangeEventArgs args, RadzenDataGridColumn<TItem> column, bool force = false, bool isFirst = true)
        {
            var methodInfo = grid.GetType().GetMethod("OnFilter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            methodInfo.Invoke(grid, new object[] { args, column, force, isFirst });
        }
    }
}
