using Radzen;

namespace GdeBabki.Client.Pages
{
    public partial class Review
    {
        public class DataGridColumnState
        {
            public object FilterValue { get; set; }
            public FilterOperator FilterOperator { get; set; }
            public SortOrder? SortOrder { get; set; }
            public double? Width { get; set; }
        }
    }
}
