using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortForward
{
    class PortForward
    {
        private string fromIP;
        private string fromPort;
        
        private string toIP;
        private string toPort;

        public PortForward(string fromIP, string fromPort, string toIP, string toPort)
        {
            this.fromIP = fromIP;
            this.fromPort = fromPort;
            this.toIP = toIP;
            this.toPort = toPort;
        }

        public string FromPort { get => this.fromPort; set => this.fromPort = value; }
        public string FromIP { get => this.fromIP; set => this.fromIP = value; }
        public string ToPort { get => this.toPort; set => toPort = value; }
        public string ToIP { get => this.toIP; set => this.toIP = value; }

        public string __toString()
        {
            return String.Format("FROM: {0}:{1}     \tTO {2}:{3}", this.fromIP, this.fromPort, this.toIP, this.toPort);
        }
    }
}
