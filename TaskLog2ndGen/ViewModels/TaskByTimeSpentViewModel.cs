using System.Collections.Generic;

namespace TaskLog2ndGen.ViewModels
{
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