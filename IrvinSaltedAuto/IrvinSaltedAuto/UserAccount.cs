using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrvinSaltedAuto
{
    public class UserAccount
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Group { get; set; }
    }

    public class UserAccountStatus
    {
        public UserAccount UserAccount { get; set; }
        public DateTime BookDate { get; set; }
        public DateTime OperationTimeStamp { get; set; }
        public bool HasError { get; set; }
        public string Status { get; set; }
    }
}
