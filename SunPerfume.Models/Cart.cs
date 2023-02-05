﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunPerfume.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }
        [Range(1, 1000, ErrorMessage="Pleaser enter a value between 1 and 1000")]
        public int Count { get; set; }
        [NotMapped]
        public double Price { get; set; }
    }
}
