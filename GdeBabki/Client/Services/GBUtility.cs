using Radzen;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace GdeBabki.Client.Services
{
    public static class GBUtility
    {
        public const string GENERAL_DATE_FORMAT = "d MMM yyyy H:mm";
        public const string THOUSANDS_SEPARATED_FORMAT = "#,##0";
        public const string TRANSACTION_CURRENCY_FORMAT = "#,##0.#0";
        public const string TRANSACTION_DATE_FORMAT = "d MMM yyyy";
        public const string TRANSACTION_MONTH_YEAR_FORMAT = "MMM yyyy";
        readonly static (string, string)[] colorTable = new (string, string)[]
        {
            ("red", "yellow"), ("red", "lime"),("red", "black"),("red", "white"),
            ("orange", "yellow"), ("orange", "green"), ("orange", "blue"), ("orange", "black"), ("orange", "white"),
            ("yellow", "red"), ("yellow", "blue"),("yellow", "violet"),("yellow", "black"),("yellow", "gray"),
            ("green", "blue"), ("green", "black"),
            ("blue", "yellow"), ("blue", "white"),
            ("violet", "yellow"), ("violet", "black"), ("violet", "white"),
            ("black", "red"), ("black", "orange"), ("black", "yellow"), ("black", "green"), ("black", "violet"), ("black", "white"), ("black", "gray"),
            ("white", "red"), ("white", "orange"), ("white", "blue"), ("white", "violet"), ("white", "black"), ("white", "gray"),
            ("gray", "orange"), ("gray", "yellow"), ("gray", "lime"), ("gray", "blue"), ("gray", "black"), ("gray", "white"),
        };

        public static string ToCurrencyString(this decimal amount)
        {
            return amount.ToString(TRANSACTION_CURRENCY_FORMAT);
        }

        public static decimal ToCurrency(this decimal amount)
        {
            return Math.Round(amount, 2);
        }

        public static string ToTransactionDate(this DateTime date)
        {
            return date.ToString(TRANSACTION_DATE_FORMAT);
        }

        public static string ToTransactionMonthYear(this DateTime date)
        {
            return date.ToString(TRANSACTION_MONTH_YEAR_FORMAT);
        }

        public static int Hash(this string s)
        {
            using var sha1 = SHA1.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(s);
            byte[] hashBytes = sha1.ComputeHash(inputBytes);
            int result = hashBytes[0] << 32 | hashBytes[1] << 24 | hashBytes[2] << 16 | hashBytes[3];
            return result;
        }

        public static string ToForegroundColor(this string s)
        {
            return colorTable[(uint)s.Hash() % colorTable.Length].Item2;
        }

        public static string ToBackgroundColor(this string s)
        {
            return colorTable[(uint)s.Hash() % colorTable.Length].Item1;
        }

        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static double GetAxisStepSize(this decimal range, int steps = 5)
        {
            var d = (double)range;
            double x = Math.Pow(10.0, Math.Floor(Math.Log10(d)));
            if (d / x >= steps)
                return x;
            else if (d / (x / 2.0) >= steps)
                return x / 2.0;
            else
                return x / 5.0;
        }

        public static DialogOptions ResizableDialogOptions => new() { Width = "700px", Resizable = true, Draggable = true };

        public static DateTime ToMonth(this DateTime d)
        {
            return new DateTime(d.Year, d.Month, 1);
        }

        public static DateTime ToYear(this DateTime d)
        {
            return new DateTime(d.Year, 1, 1);
        }

    }
}
