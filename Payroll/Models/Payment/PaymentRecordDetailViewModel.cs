﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll.Models.Payment
{
    public class PaymentRecordDetailViewModel : PaymentRecordCreateViewModel
    {
        public string Year { get; set; }
        [Display(Name = "Overtime Rate")]
        public decimal OvertimeRate { get; set; }
    }
}
