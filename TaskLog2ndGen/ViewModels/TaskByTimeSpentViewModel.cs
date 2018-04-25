using System.Collections.Generic;

namespace TaskLog2ndGen.ViewModels
{
    /// <summary>
    /// Viewmodel class for grouping tasks by time spent
    /// </summary>
    public class TaskByTimeSpentViewModel
    {
        public decimal totalTimeSpent;
        public List<TaskViewModel> tasks;

        public TaskByTimeSpentViewModel()
        {
            tasks = new List<TaskViewModel>();
        }
    }
}