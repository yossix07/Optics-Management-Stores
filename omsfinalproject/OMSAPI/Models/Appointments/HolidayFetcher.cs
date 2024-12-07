using NewtonsoftJson = Newtonsoft.Json;
using Serilog;
using System.Globalization;

namespace OMSAPI.Models.Appointments
{
    public static class HolidayFetcher
    {
        public static async Task<List<Holiday>?> GetHolidays(int year, string countryCode)
        {
            var holidays = new List<Holiday>();

            // API endpoint for Israel holidays
            string url = $"https://calendarific.com/api/v2/holidays?&api_key=482e02575ca308bc3f3f38e00d8b3397a03d60e7&country={countryCode}&year={year}";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(url))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (apiResponse != null)
                        {
                            dynamic? result = NewtonsoftJson.JsonConvert.DeserializeObject(apiResponse);

                            // check the result is different from null
                            if (result != null)
                            {
                                foreach (var holiday in result?.response?.holidays ?? Enumerable.Empty<dynamic>())
                                {
                                    //var holidayDate = (string)holiday.date.iso;
                                    //var dateParts = holidayDate.Split('-');
                                    //var formattedDate = $"{dateParts[2]}:{dateParts[1]}:{dateParts[0]}";

                                    var holidayStr = holiday.ToString();
                                    var name = (string)holiday.name;
                                    var date = DateOnly.FromDateTime(DateTime.Parse((string)holiday.date.iso));
                                    holidays.Add(new Holiday (name, date));
                                }
                            }
                            else throw new ArgumentNullException(result);
                        }
                        else throw new ArgumentNullException(apiResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception here
                Log.Error($"Error in downloading holidays API: {ex.Message}");
                return null;
            }

            return holidays;
        }
    }
}
