namespace BreakFree.BLL.Services
{
    using System.Collections.Generic;
    using BreakFree.BLL.Interfaces;
    using BreakFree.DAL;
    using BreakFree.DAL.Entities;
    using BreakFree.DAL.Repositories;

    public class DailyStatusService : IDailyStatusService
    {
        private readonly DailyStatusRepository dailyStatusRepository;

        public DailyStatusService(DailyStatusRepository repository)
        {
            this.dailyStatusRepository = repository;
        }

        public DailyStatusService()
        {
            this.dailyStatusRepository = new DailyStatusRepository(new BreakFreeContext());
        }

        public void AddDailyStatus(DailyStatus status)
        {
            this.dailyStatusRepository.AddDailyStatus(status);
        }

        public List<DailyStatus> GetStatusesByHabit(int habitId)
        {
            return this.dailyStatusRepository.GetStatusesByHabit(habitId);
        }

        public List<DailyStatus> GetStatusesByUser(int userId)
        {
            return this.dailyStatusRepository.GetStatusesByUser(userId);
        }

        public void UpdateDailyStatus(DailyStatus status)
        {
            this.dailyStatusRepository.UpdateDailyStatus(status);
        }

        public void DeleteDailyStatus(int statusId)
        {
            this.dailyStatusRepository.DeleteDailyStatus(statusId);
        }
    }
}
