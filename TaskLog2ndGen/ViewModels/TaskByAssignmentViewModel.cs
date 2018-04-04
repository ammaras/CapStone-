using System.Collections.Generic;
using TaskLog2ndGen.Models;

namespace TaskLog2ndGen.ViewModels
{
    public class TaskByAssignmentViewModel
    {
        public Employee employee;
        public List<Task> tasks;

        public TaskByAssignmentViewModel()
        {
            tasks = new List<Task>();
        }
    }
}