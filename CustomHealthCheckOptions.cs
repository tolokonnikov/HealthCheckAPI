using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Net.Mime;
using System.Text.Json;

namespace HealthCheckAPI
{
    public class CustomHealthCheckOptions : HealthCheckOptions
    {
        public CustomHealthCheckOptions() : base()
        {
            var jsonSerilazerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            ResponseWriter = async (c, r) =>
            {
                c.Response.ContentType = MediaTypeNames.Application.Json;
                c.Response.StatusCode = StatusCodes.Status200OK;

                var result = JsonSerializer.Serialize(new
                {
                    checks = r.Entries.Select(e => new
                    {
                        name = e.Key,
                        responseTime = e.Value.Duration.TotalNanoseconds,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                    }),
                    totalStatus = r.Status,
                    totalResponseTime = r.TotalDuration.TotalMilliseconds,
                }, jsonSerilazerOptions);

                await c.Response.WriteAsync(result);
            };
        }
    }
}
