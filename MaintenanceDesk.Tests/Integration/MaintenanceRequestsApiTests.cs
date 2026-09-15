using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MaintenanceDesk.Api.Data;
using MaintenanceDesk.Api.Domain;
using MaintenanceDesk.Api.Dtos;

namespace MaintenanceDesk.Tests.Integration;

public class MaintenanceRequestsApiTests(MaintenanceDeskApiFactory factory) : IClassFixture<MaintenanceDeskApiFactory>
{
    // Matches how the API writes JSON: camelCase property names, enums as names.
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    [Fact]
    public async Task CreatedRequest_CanBeFetchedFromItsLocation()
    {
        var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/maintenance-requests", new
        {
            residentId = SeedData.Ids.AnnaLindqvist,
            title = "Radiator cold in bedroom",
            description = "The bedroom radiator stays cold even with the valve fully open.",
            category = "Heating",
            priority = "High",
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);
        var created = await createResponse.Content.ReadFromJsonAsync<MaintenanceRequestDto>(Json);
        Assert.NotNull(created);

        var getResponse = await client.GetAsync(createResponse.Headers.Location);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<MaintenanceRequestDto>(Json);
        Assert.NotNull(fetched);

        Assert.Equal(created, fetched);
        Assert.Equal(MaintenanceStatus.Submitted, fetched.Status);
        Assert.Equal(SeedData.Ids.Tallbacken1101, fetched.UnitId);
        Assert.Equal(TimeSpan.FromHours(24), fetched.ResponseDeadline - fetched.ReportedAt);
    }
}
