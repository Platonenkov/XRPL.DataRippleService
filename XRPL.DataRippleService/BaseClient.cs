using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace XRPL.DataRippleService;

/// <summary>
/// Базовый клиент с реализациями
/// </summary>
public abstract class BaseClient
{
    public readonly string ServiceAddress;

    /// <summary> Http клиент </summary>
    protected readonly HttpClient _Client;
    JsonSerializerSettings serializerSettings;

    /// <summary>
    /// Базовый конструктор
    /// </summary>
    /// <param name="serviceAddress">адрес сервиса</param>
    protected BaseClient(string serviceAddress)
    {
        ServiceAddress = serviceAddress;
        _Client = new HttpClient
        {
            BaseAddress = new Uri(serviceAddress)
        };

        _Client.DefaultRequestHeaders.Accept.Clear();

        serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };
    }

    /// <summary> Get </summary>
    /// <typeparam name="TEntity">Тип нужных данных</typeparam>
    /// <param name="url">адрес</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns></returns>
    protected async Task<TEntity> GetAsync<TEntity>(string url, CancellationToken Cancel = default)
    {
        var response = await _Client.GetAsync(url, Cancel);
        if (response.StatusCode == HttpStatusCode.NotFound || !response.IsSuccessStatusCode) return default;
        Cancel.ThrowIfCancellationRequested();
        var data = await response.Content.ReadAsStringAsync(cancellationToken: Cancel);
        return string.IsNullOrWhiteSpace(data) ? default : JsonConvert.DeserializeObject<TEntity>(data, serializerSettings);
    }

    /// <summary> Post </summary>
    /// <typeparam name="TItem">Тип нужных данных</typeparam>
    /// <param name="url">адрес</param>
    /// <param name="item">данные</param>
    /// <param name="Cancel">Признак отмены асинхронной операции</param>
    /// <returns></returns>
    protected async Task<HttpResponseMessage> PostAsync<TItem>(string url, TItem item, CancellationToken Cancel = default)
    {
        var response = await _Client.PostAsJsonAsync(url, item, Cancel);
        return response.EnsureSuccessStatusCode();
    }
}