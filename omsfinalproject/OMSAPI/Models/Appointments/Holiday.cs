namespace OMSAPI.Models.Appointments
{
    // Holiday class
    public class Holiday
    {
        public string Name { get; set; }
        public DateOnly Date { get; set; }

        public Holiday(string name, DateOnly date)
        {
            Name = name;
            Date = date;
        }
    }
}
