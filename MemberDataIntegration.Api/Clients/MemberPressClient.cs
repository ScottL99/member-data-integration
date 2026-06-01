using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using MemberDataIntegration.Api.Models.Dtos;

namespace MemberDataIntegration.Api.Clients;

public class MemberPressClient : IMemberSourceClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private const int PageSize = 3;

    public MemberPressClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<List<SourceMemberDto>> GetMembersAsync()
    {
        var page = 1;
        var allMembers = new List<MemberPressMemberDto>();
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        while (true)
        {
            var request = CreateRequest($"members?page={page}&per_page={PageSize}");
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var previewLength = Math.Min(json.Length, 500);
                var bodyPreview = json[..previewLength];

                throw new HttpRequestException(
                    $"MemberPress API returned {(int)response.StatusCode} ({response.ReasonPhrase}) on page {page}. Response body preview: {bodyPreview}"
                );
            }

            var members = JsonSerializer.Deserialize<List<MemberPressMemberDto>>(json, jsonOptions);

            if (members == null || members.Count == 0)
            {
                break;
            }

            allMembers.AddRange(members);

            if (members.Count < PageSize)
            {
                break;
            }

            page++;
        }

        return allMembers
            .Select(member => new SourceMemberDto
            {
                Email = member.Email,
                Name = CombineName(member.FirstName, member.LastName),
                Phone = member.Profile?.MobilePhone,
                Source = "MemberPress",
            })
            .ToList();
    }

    public async Task<MemberPressTestResultDto> TestMeAsync()
    {
        return await TestEndpointAsync("me");
    }

    public async Task<MemberPressTestResultDto> TestMembersAsync()
    {
        return await TestEndpointAsync($"members?page=1&per_page={PageSize}");
    }

    private async Task<MemberPressTestResultDto> TestEndpointAsync(string path)
    {
        var request = CreateRequest(path);
        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        var body = await response.Content.ReadAsStringAsync();

        return new MemberPressTestResultDto
        {
            StatusCode = (int)response.StatusCode,
            IsSuccess = response.IsSuccessStatusCode,
            BodyPreview = body[..Math.Min(body.Length, 500)],
        };
    }

    private HttpRequestMessage CreateRequest(string path)
    {
        var baseUrl = _configuration["MemberPress:BaseUrl"];
        var apiKey = _configuration["MemberPress:ApiKey"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("MemberPress BaseUrl is not configured.");
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("MemberPress ApiKey is not configured.");
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/wp-json/mp/v1/{path}")
        {
            Version = HttpVersion.Version11,
            VersionPolicy = HttpVersionPolicy.RequestVersionOrLower,
        };

        request.Headers.TryAddWithoutValidation("MEMBERPRESS-API-KEY", apiKey);
        request.Headers.TryAddWithoutValidation("Accept", "application/json");
        request.Headers.TryAddWithoutValidation("User-Agent", "MemberDataIntegration.Api/1.0");

        return request;
    }

    private static string? CombineName(string? firstName, string? lastName)
    {
        var parts = new[] { firstName, lastName }
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .Select(part => part!.Trim());

        var name = string.Join(" ", parts);

        return string.IsNullOrWhiteSpace(name) ? null : name;
    }
}

public class MemberPressMemberDto
{
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    public MemberPressProfileDto? Profile { get; set; }
}

public class MemberPressProfileDto
{
    [JsonPropertyName("mepr_mobile_phone")]
    public string? MobilePhone { get; set; }
}
