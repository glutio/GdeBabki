namespace GdeBabki.Client.Services
{
    public static class GBUtility
    {
        public const string TRANSACTION_CURRENCY_FORMAT = "#,##0.#0";
        public const string TRANSACTION_DATE_FORMAT = "d MMM YYYY";
        public static string ToCurrency(this decimal amount)
        {
            return amount.ToString(TRANSACTION_CURRENCY_FORMAT);
        }
    }
}
