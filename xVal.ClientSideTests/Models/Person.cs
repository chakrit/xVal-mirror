using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace xVal.ClientSideTests.Models
{
    public class Person
    {
        [Required]
        public string Name { get; set; }

        [Range(0, 150)]
        public int Age { get; set; }
    }
}
