using BreakFree.BLL.Interfaces;
using BreakFree.DAL;
using BreakFree.DAL.Entities;
using BreakFree.DAL.Repositories;
using System;
using System.Collections.Generic;

namespace BreakFree.BLL.Services
{
    public class DailyStatusService : IDailyStatusService
    {
        private readonly DailyStatusRepository _dailyStatusRepository;

        // Порожній конструктор для продакшн
        public DailyStatusService() : this(new DailyStatusRepository(new BreakFreeContext()))
        {
        }

        // Конструктор для тестів
        public DailyStatusService(DailyStatusRepository dailyStatusRepository)
        {
            _dailyStatusRepository = dailyStatusRepository;
        }

        public void AddDailyStatus(DailyStatus status)
        {
            _dailyStatusRepository.AddDailyStatus(status);
        }

        public List<DailyStatus> GetStatusesByHabit(int habitId)
        {
            return _dailyStatusRepository.GetStatusesByHabit(habitId);
        }

        public List<DailyStatus> GetStatusesByUser(int userId)
        {
            return _dailyStatusRepository.GetStatusesByUser(userId);
        }
    }
}
