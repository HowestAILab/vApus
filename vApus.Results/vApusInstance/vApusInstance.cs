using System.Collections.Generic;

namespace vApus.Results.Model
{
    public class vApusInstance
    {
        public virtual int Id { get; set; }
        public virtual string HostName { get; set; }
        public virtual string IP { get; set; }
        public virtual int Port { get; set; }
        public virtual bool IsMaster { get; set; }

        public virtual List<StresstestConfiguration> StresstestConfiguration { get; set; }

        public vApusInstance()
        {
            StresstestConfiguration = new List<StresstestConfiguration>();
        }
    }
}
