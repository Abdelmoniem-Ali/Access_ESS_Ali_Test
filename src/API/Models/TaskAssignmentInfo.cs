namespace ARCRM.Pidgets.MultiUserDiaryAPI.Models
{
    public class TaskAssignmentInfo
    {
        public int? TaskAssignmentId { get; set; }

        public int? UserId { get; set; }

        public string? UserName { get; set; }

        public int? TaskId { get; set; }

        public int? UserGroupId { get; set; }

        public string? UserGroupName { get; set; }

        public string? AlarmSet { get; set; }

        public DateTime? AlarmDate { get; set; }

        public DateTime? AlarmTime { get; set; }
    }
}
