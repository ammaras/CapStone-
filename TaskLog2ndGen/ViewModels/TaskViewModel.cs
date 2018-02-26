using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskLog2ndGen.ViewModels
{
    public partial class TaskViewModel
    {
        public int taskId { get; set; }
        public int primaryContact { get; set; }
        public int secondaryContact { get; set; }
        public System.DateTime dateLogged { get; set; }
        public System.DateTime dateSubmmited { get; set; }
        public int serviceTeam { get; set; }
        public int serviceGroup { get; set; }
        public string platform { get; set; }
        public string urgency { get; set; }
        public int businessUnit { get; set; }
        public string environment { get; set; }
        public string category { get; set; }
        public int application { get; set; }
        public string referenceNo { get; set; }
        public string referenceType { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string highLevelEstimate { get; set; }
        public string links { get; set; }
        public string taskStatusCode { get; set; }
    }
}