using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TKGroopBG.Models
{
    public class Order
    {
        public int Id { get; set; }


        public string? Address { get; set; }
        public string? Comment { get; set; }

        public string CartJson { get; set; } = "";

        public decimal TotalPrice { get; set; }


    }
}


