using System.Net;
using System.Net.Http.Json;

namespace MaintenanceDesk.Worker.Api;

public class MaintenanceRequestsClient(HttpClient http)
{
    public async Task<MaintenanceRequestSummary?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        using var response = await http.GetAsync($"api/maintenance-requests/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<MaintenanceRequestSummary>(cancellationToken);
    }
}
