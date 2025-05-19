using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace ClientFX.client
{
    class ProbaRestTest
    {
        private static readonly HttpClient client = new();

        static async Task Main(string[] args)
        {
            string baseUrl = "http://localhost:5243/api/proba";

            // GET ALL
            var probe = await client.GetFromJsonAsync<ProbaE[]>(baseUrl);
            Console.WriteLine("GET ALL Status: OK");
            Console.WriteLine(JsonSerializer.Serialize(probe, new JsonSerializerOptions { WriteIndented = true }));

            // GET ID = 4
            var proba4 = await client.GetFromJsonAsync<ProbaE>($"{baseUrl}/4");
            Console.WriteLine("\nGET ID=4 Status: OK");
            Console.WriteLine(JsonSerializer.Serialize(proba4, new JsonSerializerOptions { WriteIndented = true }));

            // POST new proba
            var newProba = new { distanta = "100m", stil = "fluture" };
            var postResponse = await client.PostAsJsonAsync(baseUrl, newProba);
            var createdProba = await postResponse.Content.ReadFromJsonAsync<ProbaE>();

            Console.WriteLine("\nPOST Status: " + postResponse.StatusCode);
            Console.WriteLine(JsonSerializer.Serialize(createdProba, new JsonSerializerOptions { WriteIndented = true }));

            int newProbaId = createdProba?.Id ?? 0;
            Console.WriteLine($"New Proba ID (for DELETE): {newProbaId}");

            // PUT update proba ID = 4
            var updatedProba = new { id = 4, distanta = "200m", stil = "mixt" };
            var putResponse = await client.PutAsJsonAsync($"{baseUrl}/4", updatedProba);
            Console.WriteLine("\nPUT ID=4 Status: " + putResponse.StatusCode);

            // DELETE the created proba (not ID=4, but the one you created above)
            if (newProbaId != 0)
            {
                var deleteResponse = await client.DeleteAsync($"{baseUrl}/{newProbaId}");
                Console.WriteLine($"\nDELETE ID={newProbaId} Status: {deleteResponse.StatusCode}");
            }
            else
            {
                Console.WriteLine("\nDELETE skipped, invalid ID.");
            }
        }
    }

    public class ProbaE
    {
        public int Id { get; set; }
        public string Distanta { get; set; }
        public string Stil { get; set; }
    }
}
