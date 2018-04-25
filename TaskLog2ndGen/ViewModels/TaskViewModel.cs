using TaskLog2ndGen.Models;

namespace TaskLog2ndGen.ViewModels
{
    /// <summary>
    /// Viewmodel class for displaying tasks with total time spend and list of assigned employees
    /// </summary>
    public class TaskViewModel
    {
        public Task task;
        public decimal totalTimeSpent;
        public string assignedEmployees;
    }
}