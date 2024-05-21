﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace share.Models.Order
{
    public class OrderAddRequestDto
    {
        public string UserId { get; set; }
        public int Price { get; set; }
        public int Count { get; set; }
        public int ProductId { get; set; }

    }
}
