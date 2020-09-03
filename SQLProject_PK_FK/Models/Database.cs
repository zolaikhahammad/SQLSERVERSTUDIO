using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProject.Models
{
    public class Database:BaseResponse
    {
        public List<string> databaselist { get; set; }
        public List<string> tablelist { get; set; }
    }
}
