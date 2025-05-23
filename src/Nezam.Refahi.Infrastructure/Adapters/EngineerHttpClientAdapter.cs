using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Nezam.Refahi.Application.Ports;

namespace Nezam.Refahi.Infrastructure.Adapters
{
  /// <summary>
  /// HTTP client adapter for fetching engineer data by national code.
  /// Creates its own HttpClient instead of relying on DI.
  /// </summary>
  public class EngineerHttpClientAdapter : IEngineerHttpClient
  {
    private readonly HttpClient _http;

    public EngineerHttpClientAdapter()
    {
      var baseUrl = "https://portal.wa-nezam.org";
      if (string.IsNullOrWhiteSpace(baseUrl))
        throw new ArgumentException("Base URL cannot be null or empty.", nameof(baseUrl));

      _http = new HttpClient
      {
        BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/")
      };
      _http.DefaultRequestHeaders.Accept.Clear();
      _http.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<EngineerDto?> GetByNationalCodeAsync(string nationalCode)
    {
      if (string.IsNullOrWhiteSpace(nationalCode))
        throw new ArgumentException("National code must be provided.", nameof(nationalCode));

      var url = $"api/Engineers/by-national-code/{nationalCode}";
      try
      {
        return await _http.GetFromJsonAsync<EngineerDto>(url);
      }
      catch (HttpRequestException)
      {
        return null;
      }
    }
  }
}
