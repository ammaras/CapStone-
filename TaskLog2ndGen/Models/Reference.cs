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
    
    public partial class Reference
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Reference()
        {
            this.TaskReferences = new HashSet<TaskReference>();
        }
    
        public int referenceId { get; set; }
        public string referenceNo { get; set; }
        public string referenceType { get; set; }
    
        public virtual ReferenceType ReferenceType1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TaskReference> TaskReferences { get; set; }
    }
}
