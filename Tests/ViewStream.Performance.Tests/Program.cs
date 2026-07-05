using System;
using System.Net.Http;
using System.Text;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace ViewStream.Performance.Tests
{
    class Program
    {
        static int Main(string[] args)
        {
            // Set up target URL. Default to local development server, but allow command-line override.
            string baseUrl = args.Length > 0 ? args[0].TrimEnd('/') : "https://localhost:7163";
            Console.WriteLine($"Starting ViewStream Performance Tests targeting: {baseUrl}");

            using var httpClient = new HttpClient();

            // Scenario 1: Browse Episode Details (Anonymous read GET request)
            var browseEpisodeScenario = Scenario.Create("browse_episode_scenario", async context =>
            {
                var request = Http.CreateRequest("GET", $"{baseUrl}/api/v1/Episodes/1")
                    .WithHeader("Accept", "application/json");

                var response = await Http.Send(httpClient, request);
                return response;
            })
            .WithLoadSimulations(
                Simulation.KeepConstant(copies: 20, during: TimeSpan.FromSeconds(10))
            );

            // Scenario 2: User Login (Auth POST request)
            var loginScenario = Scenario.Create("login_scenario", async context =>
            {
                var jsonPayload = "{\"email\":\"testuser@example.com\",\"password\":\"Password123!\"}";
                var request = Http.CreateRequest("POST", $"{baseUrl}/api/v1/Account/login")
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("Accept", "application/json")
                    .WithBody(new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

                var response = await Http.Send(httpClient, request);
                return response;
            })
            .WithLoadSimulations(
                Simulation.KeepConstant(copies: 10, during: TimeSpan.FromSeconds(10))
            );

            Console.WriteLine("Running performance test suite...");

            var result = NBomberRunner
                .RegisterScenarios(browseEpisodeScenario, loginScenario)
                .Run();

            Console.WriteLine("Performance test execution completed.");
            return 0;
        }
    }
}
