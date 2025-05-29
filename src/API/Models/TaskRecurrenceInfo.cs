namespace ARCRM.Pidgets.MultiUserDiaryAPI.Models
{
    public class TaskRecurrenceInfo
    {
        public int? TaskId { get; set; }

        public string? RecurrenceType { get; set; }

        public int? CheckedItemIndex { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? EndAfterNoOfOccurrences { get; set; }

        public bool? Assigned { get; set; }

        public int? D_CheckedItemIndex { get; set; }

        public int? D_NoOfDays { get; set; }

        public string? D_EveryWeekday { get; set; }

        public int? W_NoOfWeeks { get; set; }

        public string? W_Mon { get; set; }

        public string? W_Tue { get; set; }

        public string? W_Wed { get; set; }

        public string? W_Thu { get; set; }

        public string? W_Fri { get; set; }

        public string? W_Sat { get; set; }

        public string? W_Sun { get; set; }

        public int? M_CheckedItemIndex { get; set; }

        public int? M_DayNo { get; set; }

        public int? M_NoOfMonths { get; set; }

        public string? M_WeekName { get; set; }

        public string? M_DayName { get; set; }

        public int? Y_CheckedItemIndex { get; set; }

        public string? Y_MonthName { get; set; }

        public int? Y_DayNo { get; set; }

        public string? Y_WeekName { get; set; }

        public string? Y_DayName { get; set; }

    }
}
