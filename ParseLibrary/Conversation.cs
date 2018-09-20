using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary
{
    public class Conversation
    {
        public string Tenant { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Date { get; set; }
        public string Duration { get; set; }
        public string Billing { get; set; }
        public string Cost { get; set; }
        public string Status { get; set; }
    }
}
