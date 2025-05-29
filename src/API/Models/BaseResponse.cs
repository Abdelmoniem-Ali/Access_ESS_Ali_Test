namespace ARCRM.Pidgets.MultiUserDiaryAPI.Models
{
    public class BaseResponse<T>
    {
        public T? Response { get; set; }
        public string Error { get; set; }
    }
}
