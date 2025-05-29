using System.Drawing;

namespace ARCRM.Pidgets.MultiUserDiaryAPI.Models
{
    public class DiaryEventInfo
    {
        public int? DiaryEventId { get; set; }

        public int? EventTypeId { get; set; }

        public int? ResourceDiaryId { get; set; }

        public string? EventTypeDescription { get; set; }

        public string? Subject { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? AllDayEvent { get; set; }

        public int? DiaryColor { get; set; }

        public string? IsPrivate { get; set; }

        public int? NotebookItemId { get; set; }

        public string? LinkXML { get; set; }

        public string? KeyId { get; set; }

        public int? PatternFrequency { get; set; }

        public int? PatternDaysOfWeek { get; set; }

        public int? RangeLimit { get; set; }

        public int? RangeMaxOccurrences { get; set; }

        public DateTime? RangeEndDate { get; set; }

        public DateTime? RangeStartDate { get; set; }

        public int? PatternInterval { get; set; }

        public int? PatternType { get; set; }

        public int? PatternOccurrenceOfDayInMonth { get; set; }

        public int? PatternMonthOfYear { get; set; }

        public int? PatternDayOfMonth { get; set; }
        public string DiaryColorHex
        {
            get
            {
                return DiaryColor.HasValue ? ColorTranslator.ToHtml(Color.FromArgb(Convert.ToInt32(DiaryColor))) : string.Empty;
            }

        }


    }
}
