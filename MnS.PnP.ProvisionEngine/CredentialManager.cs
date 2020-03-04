using CredentialManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnS.PnP.ProvisionEngine
{
    class CredentialManager
    {
        public string CredentialName { get; private set; }

        public CredentialManager(string credentialName)
        {
            this.CredentialName = credentialName;
        }

        public UserPass GetCredentials()
        {
            var cm = new Credential { Target = CredentialName };
            if (!cm.Exists())
                return null;

            cm.Load();
            var up = new UserPass(cm);
            return up;
        }

        public bool SetCredentials(UserPass up)
        {
            var cm = new Credential { Target = CredentialName, PersistanceType = PersistanceType.Enterprise, Username = up.UserName, Password = up.Password };
            return cm.Save();
        }

        public void RemoveCredentials()
        {
            var cm = new Credential { Target = CredentialName };
            cm.Delete();

        }
    }
}
