namespace TaskLog2ndGen.ViewModels
{
    /// <summary>
    /// Viewmodel class for requesting clarification for a task
    /// </summary>
    public partial class ClarificationViewModel
    {
        public int taskId { get; set; }
        public string to { get; set; }
        public string cc { get; set; }
        public string from { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }
}