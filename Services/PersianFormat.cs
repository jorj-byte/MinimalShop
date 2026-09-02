using System.Globalization;
using MinimalShop.Models;

namespace MinimalShop.Services;

public static class PersianFormat
{
    public static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("fa-IR");

    public static string Price(decimal amount) =>
        string.Format(Culture, "{0:N0}", amount) + " تومان";

    public static string Number(int value) =>
        string.Format(Culture, "{0:N0}", value);

    public static string StatusLabel(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "در انتظار بررسی",
        OrderStatus.Confirmed => "تأیید شده",
        OrderStatus.Shipped => "ارسال شده",
        OrderStatus.Delivered => "تحویل شده",
        OrderStatus.Cancelled => "لغو شده",
        _ => status.ToString()
    };
}
