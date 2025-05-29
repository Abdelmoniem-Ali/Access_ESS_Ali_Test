namespace ARCRM.Pidgets.MultiUserDiaryAPI.Models
{
    public class TaskInfo
    {
        public int? TaskId { get; set; }

        public string? Subject { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? StartDate { get; set; }

        public int? StatusValueId { get; set; }

        public string? StatusSystemCode { get; set; }

        public int? PriorityValueId { get; set; }

        public string? PrioritySystemCode { get; set; }

        public int? PercCompleted { get; set; }

        public DateTime? CompletedDate { get; set; }

        public string? IsPrivate { get; set; }

        public int? NotebookItemId { get; set; }

        public int? JobId { get; set; }

        public int? PersonId { get; set; }

        public int? ClientId { get; set; }

        public int? PlacementId { get; set; }

        public int? ApplicantActionId { get; set; }

        public int? InvoiceId { get; set; }

        public int? ReviewListId { get; set; }

        public int? MailshotId { get; set; }

        public string? LinkXML { get; set; }

        public string? StatusDescription { get; set; }

        public string? PriorityDescription { get; set; }

        public object? UpdatedTimestamp { get; set; }

        public TaskAssignmentInfo? TaskAssignment { get; set; }

        public string? Notes { get; set; }

        public TaskRecurrenceInfo? RecurrenceInfo { get; set; }
    }
}
