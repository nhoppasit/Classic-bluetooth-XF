using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaperX_SCG_forms
{
  public  class LogItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public DateTime DateTimeLog { get; set; }
        public string tagLog { get; set; }
        public string messageLog { get; set; }
      
    }
}
