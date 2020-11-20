using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerReader
{
    public class Address
    {
        public String streetAddress;
        public String city;
        public String state;
        public String zipCode;
    }

    public class Customer : Address
    {
        public String fn;
        public String ln;
        public String email;
        public String phone;
    }
}
