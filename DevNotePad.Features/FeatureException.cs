using System.Text;

namespace DevNotePad.Features
{
    public class FeatureException : ApplicationException
    {
        public FeatureException(string message) : base(message)
        {

        }

        public FeatureException(string message, Exception inner) : base(message, inner)
        {
            Details = inner.Message;
        }

        public string Details { get; set; }

        public string BuildReport()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Error : {0}\n", Message);

            if (Details != null)
            {
                sb.AppendFormat("Details : {0}\n", Details);
            }

            return sb.ToString();
        }
    }
}
