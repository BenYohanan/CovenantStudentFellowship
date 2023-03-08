using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.IHelpers
{
    public interface IEmailConfiguration
    {
        string SmtpServer { get; }

        int SmtpPort { get; }

        string SmtpUsername { get; set; }

        string SmtpPassword { get; set; }
    }
}
