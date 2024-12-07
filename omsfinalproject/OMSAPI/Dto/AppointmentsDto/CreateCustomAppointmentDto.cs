namespace OMSAPI.Dto.AppointmentsDto
{
    public class CreateCustomAppointmentDto
    {
        public DateDto Date { get; set; }

        public AppointmentsAvailableBlockDto AppointmentsAvailableBlockDto { get; set; }

        public CreateCustomAppointmentDto(DateDto date, AppointmentsAvailableBlockDto appointmentsAvailableBlockDto)
        {
            Date = date;
            AppointmentsAvailableBlockDto = appointmentsAvailableBlockDto;
        }

        
    }
}
