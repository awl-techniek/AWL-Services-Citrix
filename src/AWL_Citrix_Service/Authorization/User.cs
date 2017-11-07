using System.Text;

namespace AWL.Citrix.Service
{
    internal class User
    {
        public string UserName { get; set; }
        public AuthCredential Credential { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UserName: {0}\n", UserName);
            sb.AppendFormat(Credential.ToString());
            return sb.ToString();
        }
    }
}
