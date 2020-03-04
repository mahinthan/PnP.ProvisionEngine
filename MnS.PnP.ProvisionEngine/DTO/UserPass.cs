using CredentialManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnS.PnP.ProvisionEngine
{
    class UserPass
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public UserPass(Credential credential)
        {
            this.UserName = credential.Username;
            this.Password = credential.Password;

        }
    }
}
