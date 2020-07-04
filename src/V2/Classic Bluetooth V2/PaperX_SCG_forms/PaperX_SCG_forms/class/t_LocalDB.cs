using System;
using System.Collections.Generic;
using System.Text;

   class t_LocalDB
{
    public class Data
    {
        public Double sumWeight { get; set; }
        public DataBuy[] dataoffline {get; set;}

    }

    public class DataBuy
    {
        public string ticket { get; set; }
        public string customerQR { get; set; }
        public int productId { get; set; }
        public double SumWeight { get; set; }


        public string transactionDate { get; set; }
       

      
        public bool statusSend { get; set; }
    }

        public Data data { get; set; }
}

