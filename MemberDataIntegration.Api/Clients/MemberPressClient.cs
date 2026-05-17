using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using MemberDataIntegration.Api.Models.Dtos;

namespace MemberDataIntegration.Api.Clients;

public class MemberPressClient : IMemberSourceClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public MemberPressClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<List<SourceMemberDto>> GetMembersAsync()
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

        var page = 1;
        var pageSize = 3;
        var allMembers = new List<MemberPressMemberDto>();
        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        while (true)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{baseUrl}/wp-json/mp/v1/members?page={page}&per_page={pageSize}"
            )
            {
                Version = HttpVersion.Version11,
                VersionPolicy = HttpVersionPolicy.RequestVersionOrLower,
            };

            request.Headers.TryAddWithoutValidation("MEMBERPRESS-API-KEY", apiKey);
            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            request.Headers.TryAddWithoutValidation("User-Agent", "MemberDataIntegration.Api/1.0");

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

            if (members.Count < pageSize)
            {
                break;
            }

            page++;
        }

        return allMembers
            .Select(member => new SourceMemberDto
            {
                Email = member.Email,
                FirstName = member.FirstName,
                LastName = member.LastName,
                Phone = member.Profile?.MobilePhone,
                Source = "MemberPress",
            })
            .ToList();
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
