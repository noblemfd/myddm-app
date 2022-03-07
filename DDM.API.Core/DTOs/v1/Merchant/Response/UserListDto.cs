﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.DTOs.v1.Merchant.Response
{
    public class UserListDto
    {
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool? IsPasswordChanged { get; set; }
        public string MobileNumber { get; set; }
        public DateTime LastLogin { get; set; }

        [Display(Name = "Full Name")]
        public string Fullname
        {
            get { return Firstname + " " + Lastname; }
        }
    }
}
