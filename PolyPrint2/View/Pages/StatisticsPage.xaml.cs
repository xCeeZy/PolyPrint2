using PolyPrint2.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace PolyPrint2.View.Pages
{
    public partial class StatisticsPage : Page
    {
        #region Инициализация

        public StatisticsPage()
        {
            InitializeComponent();
            LoadStatistics();
        }

        #endregion

        #region Загрузка статистики

        private void LoadStatistics()
        {
            LoadSummary();
            LoadStatusStats();
            LoadEquipmentStats();
            LoadMasterStats();
        }

        private void LoadSummary()
        {
            int totalClients = App.context.Clients.Count();
            int totalRequests = App.context.Service_Requests.Count();
            int totalEquipment = App.context.Equipment.Count();
            decimal totalRevenue = App.context.Works.Sum(w => (decimal?)w.Cost) ?? 0;

            TotalClientsText.Text = totalClients.ToString();
            TotalRequestsText.Text = totalRequests.ToString();
            TotalEquipmentText.Text = totalEquipment.ToString();
            TotalRevenueText.Text = totalRevenue.ToString("N2") + " руб.";
        }

        private void LoadStatusStats()
        {
            List<StatusStatItem> stats = App.context.Service_Requests
                .GroupBy(r => r.Status)
                .Select(g => new StatusStatItem
                {
                    Status = g.Key ?? "Не указан",
                    Count = g.Count()
                })
                .OrderByDescending(s => s.Count)
                .ToList();

            int maxCount = stats.Any() ? stats.Max(s => s.Count) : 1;
            foreach (StatusStatItem item in stats)
            {
                item.MaxCount = maxCount;
                item.BarColor = GetStatusColor(item.Status);
            }

            StatusStatsControl.ItemsSource = stats;
        }

        private string GetStatusColor(string status)
        {
            if (status == null) return "#95A5A6";
            if (status.Contains("Новая")) return "#3498DB";
            if (status.Contains("В работе")) return "#F39C12";
            if (status.Contains("Ожидание")) return "#E67E22";
            if (status.Contains("Выполнена")) return "#27AE60";
            if (status.Contains("Закрыта")) return "#95A5A6";
            return "#2C3E50";
        }

        private void LoadEquipmentStats()
        {
            List<EquipmentStatItem> stats = App.context.Equipment
                .GroupBy(e => e.Condition)
                .Select(g => new EquipmentStatItem
                {
                    Condition = g.Key ?? "Не указано",
                    Count = g.Count()
                })
                .OrderByDescending(s => s.Count)
                .ToList();

            int maxCount = stats.Any() ? stats.Max(s => s.Count) : 1;
            foreach (EquipmentStatItem item in stats)
            {
                item.MaxCount = maxCount;
                item.BarColor = GetConditionColor(item.Condition);
            }

            EquipmentStatsControl.ItemsSource = stats;
        }

        private string GetConditionColor(string condition)
        {
            if (condition == null) return "#95A5A6";
            if (condition.Contains("Исправно")) return "#27AE60";
            if (condition.Contains("Требует обслуживания")) return "#F39C12";
            if (condition.Contains("Неисправно")) return "#E74C3C";
            if (condition.Contains("На ремонте")) return "#3498DB";
            return "#2C3E50";
        }

        private void LoadMasterStats()
        {
            List<Users> masters = App.context.Users.Where(u => u.Role == "Мастер").ToList();
            List<MasterStatItem> stats = new List<MasterStatItem>();

            foreach (Users master in masters)
            {
                int totalCount = App.context.Service_Requests.Count(r => r.ID_Master == master.ID_User);
                int activeCount = App.context.Service_Requests.Count(r =>
                    r.ID_Master == master.ID_User &&
                    r.Status != "Выполнена" &&
                    r.Status != "Закрыта");

                stats.Add(new MasterStatItem
                {
                    MasterName = master.Login,
                    ActiveCount = activeCount,
                    TotalCount = totalCount
                });
            }

            stats = stats.OrderByDescending(s => s.ActiveCount).ToList();
            MasterStatsGrid.ItemsSource = stats;
        }

        #endregion
    }

    #region Классы для статистики

    public class StatusStatItem
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public int MaxCount { get; set; }
        public string BarColor { get; set; }
    }

    public class EquipmentStatItem
    {
        public string Condition { get; set; }
        public int Count { get; set; }
        public int MaxCount { get; set; }
        public string BarColor { get; set; }
    }

    public class MasterStatItem
    {
        public string MasterName { get; set; }
        public int ActiveCount { get; set; }
        public int TotalCount { get; set; }
    }

    #endregion
}
