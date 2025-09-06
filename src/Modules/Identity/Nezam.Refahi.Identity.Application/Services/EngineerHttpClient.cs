using Microsoft.Extensions.Logging;
using Nezam.Refahi.Settings.Contracts;
using System.Text.Json;
using Nezam.Refahi.Settings.Contracts.Constants;
using Nezam.Refahi.Shared.Application.Ports;

namespace Nezam.Refahi.Identity.Application.Services;

/// <summary>
/// HTTP client implementation for engineer service integration
/// </summary>
public class EngineerHttpClient : IEngineerHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<EngineerHttpClient> _logger;

    public EngineerHttpClient(
        ISettingsService settingsService, 
        ILogger<EngineerHttpClient> logger)
    {
        _httpClient = new HttpClient();
        _settingsService = settingsService;
        _logger = logger;
    }

    public async Task<EngineerDto?> GetByNationalCodeAsync(string nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            _logger.LogWarning("National code is null or empty");
            return null;
        }

        try
        {
            // Get settings from the settings service
            var baseUrl = await _settingsService.GetSettingValueAsync(
                SettingsConstants.Webhooks.EngineerMemberServiceUrl, 
                "https://api.engineer-service.com");

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
              _logger.LogInformation("Engineer member service URL is not set. url: {baseUrl}", baseUrl);
              return null;
            }
            
            var timeout = await _settingsService.GetSettingValueAsync(
                SettingsConstants.Webhooks.EngineerServiceTimeout, 
                30);
            
            var maxRetries = await _settingsService.GetSettingValueAsync(
                SettingsConstants.Webhooks.EngineerServiceMaxRetries, 
                3);
            
            var retryDelay = await _settingsService.GetSettingValueAsync(
                SettingsConstants.Webhooks.EngineerServiceRetryDelay, 
                1000);

            // Configure timeout
            _httpClient.Timeout = TimeSpan.FromSeconds(timeout);

            var url = $"{baseUrl.TrimEnd('/')}?nationalCode={nationalCode}";
            
            _logger.LogInformation("Requesting engineer data for national code: {NationalCode} from URL: {Url}", 
                nationalCode, url);

            var response = await ExecuteWithRetryAsync(async () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "NezamRefahi/1.0");
                
                return await _httpClient.SendAsync(request);
            }, maxRetries, retryDelay);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Received response: {Response}", jsonContent);

                var engineer = JsonSerializer.Deserialize<EngineerDto>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (engineer != null)
                {
                    _logger.LogInformation("Successfully retrieved engineer data for national code: {NationalCode}", 
                        nationalCode);
                }
                else
                {
                    _logger.LogWarning("Deserialized engineer data is null for national code: {NationalCode}", 
                        nationalCode);
                }

                return engineer;
            }
            else
            {
                _logger.LogWarning("Failed to retrieve engineer data. Status: {StatusCode}, Reason: {ReasonPhrase}", 
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for national code: {NationalCode}", nationalCode);
            return null;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Request timeout for national code: {NationalCode}", nationalCode);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response for national code: {NationalCode}", nationalCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving engineer data for national code: {NationalCode}", 
                nationalCode);
            return null;
        }
    }

    private async Task<HttpResponseMessage> ExecuteWithRetryAsync(
        Func<Task<HttpResponseMessage>> operation, 
        int maxRetries, 
        int retryDelayMs)
    {
        var attempt = 0;
        Exception? lastException = null;

        while (attempt <= maxRetries)
        {
            try
            {
                var response = await operation();
                
                // If successful or client error (4xx), don't retry
                if (response.IsSuccessStatusCode || (int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                {
                    return response;
                }

                // For server errors (5xx), retry
                if (attempt < maxRetries)
                {
                    _logger.LogWarning("Server error {StatusCode}, retrying in {Delay}ms (attempt {Attempt}/{MaxRetries})", 
                        response.StatusCode, retryDelayMs, attempt + 1, maxRetries);
                    
                    await Task.Delay(retryDelayMs);
                    attempt++;
                    continue;
                }

                return response;
            }
            catch (HttpRequestException ex) when (attempt < maxRetries)
            {
                lastException = ex;
                _logger.LogWarning(ex, "HTTP request failed, retrying in {Delay}ms (attempt {Attempt}/{MaxRetries})", 
                    retryDelayMs, attempt + 1, maxRetries);
                
                await Task.Delay(retryDelayMs);
                attempt++;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException && attempt < maxRetries)
            {
                lastException = ex;
                _logger.LogWarning(ex, "Request timeout, retrying in {Delay}ms (attempt {Attempt}/{MaxRetries})", 
                    retryDelayMs, attempt + 1, maxRetries);
                
                await Task.Delay(retryDelayMs);
                attempt++;
            }
        }

        // If we get here, all retries failed
        if (lastException != null)
        {
            throw lastException;
        }

        throw new InvalidOperationException("All retry attempts failed");
    }
}
