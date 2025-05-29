namespace ARCRM.Pidgets.MultiUserDiaryAPI.Models
{
    public class DiaryEventRequest
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public SharedDiaryInfo[] Owners { get; set; }
    }
}
