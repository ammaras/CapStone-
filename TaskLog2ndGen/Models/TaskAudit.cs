//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaskLog2ndGen.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TaskAudit
    {
        public int taskAuditId { get; set; }
        public int task { get; set; }
        public string taskAuditType { get; set; }
        public System.DateTime dateLogged { get; set; }
        public int loggedBy { get; set; }
        public string notes { get; set; }
        public string taskStatus { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Task Task1 { get; set; }
        public virtual TaskAuditType TaskAuditType1 { get; set; }
        public virtual TaskStatu TaskStatu { get; set; }
    }
}
