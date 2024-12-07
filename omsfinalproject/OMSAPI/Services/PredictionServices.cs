using System.Text.Json;
using System.Text;

namespace OMSAPI.Services
{

    public class PredictionServices
    {
        private ILogger<PredictionServices> _logger;
        public PredictionServices(ILogger<PredictionServices> logger)
        {
            _logger = logger;
        }

        public PredictionServices()
        {
            
        }
        public async Task<float?> UsePredictionModel(string json)
        {
            try
            {
                // Send HTTP POST request to localhost:5000, and wait for response
                using (var client = new HttpClient())
                {
                    using StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    // send request
                    using HttpResponseMessage response = await client.PostAsync("http://192.168.1.108:5000/predict", content);
                    response.EnsureSuccessStatusCode();
                    // wait for response
                    var responseString = await response.Content.ReadAsStringAsync();
                    // Handle the response if necessary
                    if (response.IsSuccessStatusCode)
                    {
                        // Request successful
                        JsonDocument jsonDocument = JsonDocument.Parse(responseString);

                        float prediction = jsonDocument.RootElement.GetProperty("prediction").GetSingle();

                        _logger.LogInformation("Prediction successful");
                        return prediction;

                    }
                    else
                    {
                        // Request failed
                        _logger.LogError("Prediction failed");
                        return null;
                    }
                }
            } 
            catch (Exception ex)
            {
                // Request failed
                _logger.LogError("Prediction failed");
                return null;
            }
        }
    }
}