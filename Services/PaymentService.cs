using System.Text.Json;
using Microsoft.Extensions.Options;

namespace MinimalShop.Services;

public class PaymentSettings
{
    public const string SectionName = "Payment:Zarinpal";
    public string MerchantId { get; set; } = string.Empty;
    public bool Sandbox { get; set; } = true;
}

public class ZarinpalRequestResult
{
    public bool Succeeded { get; init; }
    public string? Authority { get; init; }
    public string? RedirectUrl { get; init; }
    public string? Error { get; init; }
}

public class ZarinpalVerifyResult
{
    public bool Succeeded { get; init; }
    public bool AlreadyVerified { get; init; }
    public long? RefId { get; init; }
    public string? Error { get; init; }
}

/// <summary>Minimal Zarinpal (v4 API) integration: request a payment authority and verify it.</summary>
public class PaymentService(HttpClient http, IOptions<PaymentSettings> options)
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    private PaymentSettings Settings => options.Value;

    private string BaseUrl => Settings.Sandbox
        ? "https://sandbox.zarinpal.com"
        : "https://www.zarinpal.com";

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Settings.MerchantId) &&
        !Settings.MerchantId.Contains("your-merchant-id");

    public async Task<ZarinpalRequestResult> RequestPaymentAsync(
        int orderId, decimal toman, string description, string callbackUrl, CancellationToken ct = default)
    {
        if (!IsConfigured)
            return new ZarinpalRequestResult { Error = "درگاه پرداخت تنظیم نشده است (MerchantId خالی است)." };

        var body = new
        {
            merchant_id = Settings.MerchantId,
            amount = (long)(toman * 10), // v4 API amount unit is IRR (Rial)
            callback_url = callbackUrl,
            description = description.Length > 250 ? description[..250] : description
        };

        try
        {
            using var response = await http.PostAsJsonAsync($"{BaseUrl}/pg/v4/payment/request.json", body, ct);
            var payload = await response.Content.ReadFromJsonAsync<ZarinpalEnvelope<AuthorityData>>(JsonOpts, ct);

            var authority = payload?.Data?.Authority;
            var code = payload?.Data?.Code ?? 0;
            if (code == 100 && !string.IsNullOrWhiteSpace(authority))
            {
                return new ZarinpalRequestResult
                {
                    Succeeded = true,
                    Authority = authority,
                    RedirectUrl = $"{BaseUrl}/pg/StartPay/{authority}"
                };
            }

            var errorMessage = payload?.Errors?.FirstOrDefault()?.Message;
            return new ZarinpalRequestResult
            {
                Error = string.IsNullOrWhiteSpace(errorMessage) ? $"درگاه پاسخ ناموفق داد (کد {code})." : errorMessage
            };
        }
        catch (Exception ex)
        {
            return new ZarinpalRequestResult { Error = "اتصال به درگاه پرداخت برقرار نشد: " + ex.Message };
        }
    }

    public async Task<ZarinpalVerifyResult> VerifyPaymentAsync(string authority, decimal toman, CancellationToken ct = default)
    {
        var body = new
        {
            merchant_id = Settings.MerchantId,
            amount = (long)(toman * 10),
            authority
        };

        try
        {
            using var response = await http.PostAsJsonAsync($"{BaseUrl}/pg/v4/payment/verify.json", body, ct);
            var payload = await response.Content.ReadFromJsonAsync<ZarinpalEnvelope<VerifyData>>(JsonOpts, ct);

            var code = payload?.Data?.Code ?? 0;
            if (code is 100 or 101)
            {
                return new ZarinpalVerifyResult
                {
                    Succeeded = true,
                    AlreadyVerified = code == 101,
                    RefId = payload?.Data?.RefId
                };
            }

            var errorMessage = payload?.Errors?.FirstOrDefault()?.Message;
            return new ZarinpalVerifyResult
            {
                Error = string.IsNullOrWhiteSpace(errorMessage) ? $"پرداخت تأیید نشد (کد {code})." : errorMessage
            };
        }
        catch (Exception ex)
        {
            return new ZarinpalVerifyResult { Error = "اتصال به درگاه برای تأیید پرداخت برقرار نشد: " + ex.Message };
        }
    }

    private sealed class ZarinpalEnvelope<T>
    {
        public T? Data { get; set; }
        public List<ZarinpalError>? Errors { get; set; }
    }

    private sealed class ZarinpalError
    {
        public int Code { get; set; }
        public string? Message { get; set; }
    }

    private sealed class AuthorityData
    {
        public string? Authority { get; set; }
        public int Code { get; set; }
    }

    private sealed class VerifyData
    {
        public int Code { get; set; }
        public long? RefId { get; set; }
    }
}
