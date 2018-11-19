using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL.Models
{
 public   class LoginInfoModel
    {
        public LoginInfoModel() { }
        public LoginInfoModel(string UserName)
        {
            this.UserName = UserName;

        }

        public string CustomerId { get; set; }
        public string OwnerId { get; set; }
        public string UserName { get; set; }
    }
}
