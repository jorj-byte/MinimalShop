using Microsoft.JSInterop;

namespace MinimalShop.Services;

public class CartLine
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal => Price * Quantity;
}

/// <summary>
/// Shopping cart persisted in the browser's localStorage (per circuit in Blazor Server).
/// A JavaScript helper (wwwroot/js/cart-storage.js) is the storage layer and also
/// notifies about cart changes made in other browser tabs via the "storage" event.
/// </summary>
public class CartService(IJSRuntime js) : IDisposable
{
    private const string LoadFn = "cartStorage.load";
    private const string SaveFn = "cartStorage.save";
    private const string WatchFn = "cartStorage.watch";
    private const string UnwatchFn = "cartStorage.unwatch";

    private readonly Dictionary<int, CartLine> _lines = [];
    private DotNetObjectReference<CartService>? _self;
    private bool _loaded;
    private bool _disposed;

    private Task _persistChain = Task.CompletedTask;

    public event Action? OnChange;

    public IReadOnlyCollection<CartLine> Lines => _lines.Values;
    public int ItemCount => _lines.Values.Sum(l => l.Quantity);
    public decimal Subtotal => _lines.Values.Sum(l => l.LineTotal);

    /// <summary>
    /// Loads the persisted cart (once) and subscribes to cross-tab updates.
    /// Safe to call from every page: subsequent calls are no-ops.
    /// </summary>
    public async Task EnsureLoadedAsync(CancellationToken ct = default)
    {
        if (_loaded)
            return;
        _loaded = true;

        try
        {
            var stored = await js.InvokeAsync<CartLine[]>(LoadFn, ct);
            _lines.Clear();
            foreach (var line in stored ?? [])
            {
                if (line.ProductId != 0 && line.Quantity > 0)
                    _lines[line.ProductId] = line;
            }

            _self ??= DotNetObjectReference.Create(this);
            // NOTE: pass the CancellationToken as an invocation-level parameter (before
            // positional args), never as a serialized argument — CancellationToken cannot
            // be JSON-serialized (WaitHandle/IntPtr) and kills the Blazor circuit.
            await js.InvokeVoidAsync(WatchFn, ct, _self);

            if (_lines.Count > 0)
                OnChange?.Invoke();
        }
        catch (ObjectDisposedException)
        {
            // Circuit is closing; keep the in-memory cart only.
        }
        catch (JSException)
        {
            // JS interop unavailable — fall back to an in-memory cart.
        }
        catch (InvalidOperationException)
        {
            // Expected during prerendering, where IJSRuntime cannot be provided.
        }
        catch (JSDisconnectedException)
        {
            // Client disconnected; nothing to persist.
        }
        catch (Exception)
        {
            // Interop failure of any kind must never break the page — fall back
            // to an in-memory cart instead of terminating the circuit.
        }
    }

    [JSInvokable]
    public Task OnStorageChanged(CartLine[] lines)
    {
        _lines.Clear();
        foreach (var line in lines ?? [])
        {
            if (line.ProductId != 0 && line.Quantity > 0)
                _lines[line.ProductId] = line;
        }

        OnChange?.Invoke();
        return Task.CompletedTask;
    }

    public void Add(int productId, string name, decimal price, int quantity = 1)
    {
        if (_lines.TryGetValue(productId, out var line))
            line.Quantity += quantity;
        else
            _lines[productId] = new CartLine { ProductId = productId, Name = name, Price = price, Quantity = quantity };

        Notify();
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        if (!_lines.TryGetValue(productId, out var line))
            return;

        if (quantity <= 0)
            _lines.Remove(productId);
        else
            line.Quantity = quantity;

        Notify();
    }

    public void Remove(int productId)
    {
        if (_lines.Remove(productId))
            Notify();
    }

    public void Clear()
    {
        if (_lines.Count == 0)
            return;

        _lines.Clear();
        Notify();
    }

    private void Notify()
    {
        Persist();
        OnChange?.Invoke();
    }

    /// <summary>Queues a debounced save so concurrent changes are written in order.</summary>
    private void Persist()
    {
        if (_disposed)
            return;

        var snapshot = _lines.Values.ToArray();
        // Wrap explicitly: CartLine[] is object[]-assignable, so without the cast the
        // params binding would spread every cart line as a separate JS argument and
        // cartStorage.save() would only ever receive the first line.
        object[] singleArg = { snapshot };
        _persistChain = _persistChain
            .ContinueWith(_ => js.InvokeVoidAsync(SaveFn, singleArg).AsTask(), TaskScheduler.Default)
            .Unwrap();
    }

    public void Dispose()
    {
        _disposed = true;
        try
        {
            _ = js.InvokeVoidAsync(UnwatchFn);
        }
        catch
        {
            // Interop may already be unavailable.
        }

        _self?.Dispose();
    }
}
