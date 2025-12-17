namespace BreakFree.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BreakFree.DAL;
    using BreakFree.DAL.Entities;

    public class SosService
    {
        private readonly List<string> defaultTips = new List<string>
        {
            "Зроби 10 присідань або віджимань",
            "Пий воду",
            "Прогуляйся 5 хвилин",
            "Зроби дихальні вправи",
            "Медитація 1 хв",
            "Розтяжка 5 хв",
            "Перерви роботу на очі",
        };

        private readonly BreakFreeContext context;

        public SosService(BreakFreeContext? context = null)
        {
            this.context = context ?? new BreakFreeContext();
        }

        public List<SosTipViewModel> GetSortedTips(int userId, bool skipDefaultActions = false)
        {
            this.context.Database.EnsureCreated();

            if (!skipDefaultActions)
            {
                this.EnsureDefaultActionsExist(userId);
            }

            var actions = this.context.SOSActions
                                 .Where(a => a.UserId == userId)
                                 .ToList();

            var logs = this.context.UserSOSLogs.Where(l => l.UserId == userId).ToList();
            var result = new List<SosTipViewModel>();

            foreach (var action in actions)
            {
                var actionLogs = logs.Where(l => l.ActionId == action.ActionId).ToList();
                int totalTries = actionLogs.Count;
                int successCount = actionLogs.Count(l => l.Worked);

                double efficiency = totalTries > 0 ? (double)successCount / totalTries * 100 : 0;

                result.Add(new SosTipViewModel
                {
                    ActionId = action.ActionId,
                    Text = action.Text,
                    Efficiency = (int)efficiency,
                    UsageCount = totalTries,
                });
            }

            return result.OrderByDescending(x => x.Efficiency)
                         .ThenByDescending(x => x.UsageCount)
                         .ToList();
        }

        public void LogAttempt(int userId, int actionId, bool worked)
        {
            if (this.context.SOSActions.Any(a => a.ActionId == actionId))
            {
                var log = new UserSOSLog
                {
                    UserId = userId,
                    ActionId = actionId,
                    DateTime = DateTime.Now,
                    Worked = worked,
                };
                this.context.UserSOSLogs.Add(log);
                this.context.SaveChanges();
            }
        }

        private void EnsureDefaultActionsExist(int userId)
        {
            if (!this.context.SOSActions.Any(a => a.UserId == userId))
            {
                foreach (var tip in this.defaultTips)
                {
                    this.context.SOSActions.Add(new SOSAction
                    {
                        UserId = userId,
                        Text = tip,
                    });
                }

                this.context.SaveChanges();
            }
        }
    }
}
