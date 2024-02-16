using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportUtility.Model
{
    public class AppSettings
    {
        public ConnectionStringInfo ConnectionStrings { get; set; }

        public class ConnectionStringInfo
        {
            public string DefaultConnection { get; set; }
        }
    }
}
