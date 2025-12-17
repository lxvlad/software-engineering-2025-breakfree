namespace BreakFree.BLL.Interfaces
{
    using BreakFree.DAL.Entities;

    public interface IDailyStatusService
    {
        void AddDailyStatus(DailyStatus status);

        void UpdateDailyStatus(DailyStatus status);

        void DeleteDailyStatus(int statusId);
    }
}
