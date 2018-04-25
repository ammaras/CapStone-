using TaskLog2ndGen.Models;

namespace TaskLog2ndGen.ViewModels
{
    /// <summary>
    /// Viewmodel class for generating employee time report
    /// </summary>
    public class EmployeeTimeViewModel
    {
        public Task task;
        public decimal regularTime;
        public decimal overTime;
    }
}