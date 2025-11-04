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

            StatusStatsGrid.ItemsSource = stats;
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

            EquipmentStatsGrid.ItemsSource = stats;
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
    }

    public class EquipmentStatItem
    {
        public string Condition { get; set; }
        public int Count { get; set; }
    }

    public class MasterStatItem
    {
        public string MasterName { get; set; }
        public int ActiveCount { get; set; }
        public int TotalCount { get; set; }
    }

    #endregion
}
