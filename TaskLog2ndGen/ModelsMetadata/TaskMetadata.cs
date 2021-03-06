﻿using System.ComponentModel.DataAnnotations;

namespace TaskLog2ndGen.Models
{
    /// <summary>
    /// Entity for task table
    /// </summary>
    [MetadataType(typeof(TaskMetadata))]
    public partial class Task
    {

    }

    /// <summary>
    /// Metadata class with validation annotations for task entity
    /// </summary>
    public class TaskMetadata
    {
        [Display(Name = "Primary Contact")]
        [Required(ErrorMessage = "Primary contact is required.")]
        public int primaryContact { get; set; }

        [Display(Name = "Secondary Contact")]
        public int secondaryContact { get; set; }

        [Display(Name = "Date Logged")]
        [Required(ErrorMessage = "Date logged is required.")]
        public System.DateTime dateLogged { get; set; }

        [Display(Name = "Date Submitted")]
        [Required(ErrorMessage = "Date submitted is required.")]
        public System.DateTime dateSubmmited { get; set; }

        [Display(Name = "Service Team")]
        [Required(ErrorMessage = "Service team is required.")]
        public int serviceTeam { get; set; }

        [Display(Name = "Service Group")]
        [Required(ErrorMessage = "Service group is required.")]
        public int serviceGroup { get; set; }

        [Display(Name = "Platform")]
        [Required(ErrorMessage = "Platform is required.")]
        public string platform { get; set; }

        [Display(Name = "Urgency")]
        [Required(ErrorMessage = "Urgency is required.")]
        public string urgency { get; set; }

        [Display(Name = "Business Unit")]
        [Required(ErrorMessage = "Business unit is required.")]
        public int businessUnit { get; set; }

        [Display(Name = "Environment")]
        [Required(ErrorMessage = "Environment is required.")]
        public string environment { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Category is required.")]
        public string category { get; set; }

        [Display(Name = "Application")]
        [Required(ErrorMessage = "Application is required.")]
        public int application { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is required.")]
        public string title { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required.")]
        public string description { get; set; }

        [Display(Name = "High Level Estimate")]
        public string highLevelEstimate { get; set; }

        [Display(Name = "Links")]
        public string links { get; set; }

        [Display(Name = "Task Status")]
        public string taskStatus { get; set; }
    }
}