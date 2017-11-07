using System.Text;

namespace AWL.Citrix.Service
{
    internal class AuthCredential
    {
        public string AuthToken { get; set; }
        public string SessionID { get; set; }
        public string CSRFToken { get; set; }

        public bool IsValid
        {
            get { return !(string.IsNullOrEmpty(AuthToken) || string.IsNullOrEmpty(SessionID) || string.IsNullOrEmpty(CSRFToken)); }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("AuthToken: {0}\n", AuthToken);
            sb.AppendFormat("SessionID: {0}\n", SessionID);
            sb.AppendFormat("CSRFToken: {0}\n", CSRFToken);
            return sb.ToString();
        }
    }
}
