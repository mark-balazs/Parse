using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace MainLibrary
{
    public class Bill
    {
        public string Tenant { get; set; }
        public string CustomerName { get; set; }
        public string ContractNumber { get; set; }
        public string BundleMinutes { get; set; }
        public double UsedMinutes { get; set; }
        public double Price { get; set; }
        public double InternMinutes { get; set; }
        public double InterPrice { get; set; }
        public double TollFreeMinutes { get; set; }
        public double TFPrice { get; set; }
        public string TotalDue { get; set; }

        public Bill()
        {
            Tenant = "";
            CustomerName = "";
            ContractNumber = "";
            TotalDue = "";
            UsedMinutes = 0;
            Price = 0;
            InternMinutes = 0;
            InterPrice = 0;
            TollFreeMinutes = 0;
            TFPrice = 0;
        }

        public void AddMinutes(string minutes)
        {
            if (minutes == "0")
                return;
            string[] tokens = minutes.Split(':');
            UsedMinutes += double.Parse(tokens[2]) / 60 + double.Parse(tokens[1]) + double.Parse(tokens[0]) * 60;
        }
        public void AddInterMinutes(string minutes)
        {
            if (minutes == "0")
                return;
            string[] tokens = minutes.Split(':');
            InternMinutes += double.Parse(tokens[2]) / 60 + double.Parse(tokens[1]) + double.Parse(tokens[0]) * 60;
        }
        public void AddTollFreeMinutes(string minutes)
        {
            if (minutes == "0")
                return;
            string[] tokens = minutes.Split(':');
            TollFreeMinutes += double.Parse(tokens[2]) / 60 + double.Parse(tokens[1]) + double.Parse(tokens[0]) * 60;
        }


    }
}
