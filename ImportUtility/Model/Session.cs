using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportUtility.Model
{
    public class Session
    {
        private static UnkCompanyDBContext _context;
        public static UnkCompanyDBContext Context
        {
            get
            {
                if (_context is null)
                {
                    _context = new UnkCompanyDBContext();
                }
                return _context;
            }
        }
        public static List<string> badStrings = new();
    }
}
