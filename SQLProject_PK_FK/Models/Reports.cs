using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProject.Models
{
    public class Reports
    {
        public string operation_status { get; set; }
        public string from_keyword { get; set; }
        public string to_keyword { get; set; }
        public string row_before { get; set; }
        public string row_after{get;set;}
        public string effected_number_rows { get; set; }
        //public string total_rows_in_column { get; set; }
    }
}
