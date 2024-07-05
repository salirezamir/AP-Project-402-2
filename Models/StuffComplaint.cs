﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_Manager.Models
{
    public class StuffComplaint
    {
        public enum CStatus
        {
            Pending,
            Answered
        }

        [Key, Required]
        public int Id { get; set; }

        [Required]
        public string Detail { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Answer { get; set; }

        [Required]
        public virtual User User { get; set; }

        [Required]
        public virtual Stuff Stuff { get; set; }

        [Required]
        public CStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

