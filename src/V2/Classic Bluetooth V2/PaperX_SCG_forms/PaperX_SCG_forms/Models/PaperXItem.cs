using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperX_SCG_forms
{
    public class PaperXItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string ticket { get; set; }
        public string customerQR { get; set; }
        public string cusID { get; set; }
        public string cusName { get; set; }
        public string cusType { get; set; }
        public int productId { get; set; }
        public double Weight { get; set; }
        public double TotalWeightInBin { get; set; }
        public DateTime transactionDate { get; set; }
        public bool statusSend { get; set; }
        public bool statusSaved { get; set; }
    }
}
