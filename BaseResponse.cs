using System;
using System.Collections.Generic;
using System.Text;

namespace SQLProject
{
    public class BaseResponse
    {
        public string username { get; set; }
        public string password { get; set; }
        public string database { get; set; }
        public string table { get; set; }

        public string coloumn { get; set; }
        public int selectedcol_index { get; set; }
        public int fromcol_index { get; set; }
        public int tocol_index { get; set; }
        public string servername { get; set; }
        public string authentication { get; set; }
    }
}
