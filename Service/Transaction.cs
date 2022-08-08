using System;
using System.Collections.Generic;

namespace MyService
{
    internal class TransactionCSV
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string address { get; set; }
        public decimal payment { get; set; }
        public DateTime date { get; set; }
        public long account_number { get; set; }
        public string service { get; set; }

        public string GetCity()
        {
            string city = address.Substring(0, address.IndexOf(','));
            return city;
        }
    }

    internal class TransactionJSON
    {
        public string city { get; set; }
        public Dictionary<string, List<Payer>> services { get; set; }
        public decimal total { get; set; }

        public TransactionJSON(List<TransactionCSV> objs)
        {
            city = objs[0].GetCity();
            services = new Dictionary<string, List<Payer>>();
            foreach (var obj in objs)
            {
                total += obj.payment;
                if (!services.ContainsKey(obj.service))
                {
                    services[obj.service] = new List<Payer>();
                }
                services[obj.service].Add(new Payer() {
                    name = obj.first_name + "" + obj.last_name,
                    payment = obj.payment,
                    date = obj.date,
                    account_number = obj.account_number
                });
            }
        }

        internal class Payer
        {
            public string name { get; set; }
            public decimal payment { get; set; }
            public DateTime date { get; set; }
            public long account_number { get; set; }
        }
    }
}
