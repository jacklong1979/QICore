using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QICore.ElasticSearchCore.WebApi.OptionModel
{
    public class ConnectionStringOption
    {       
        public string MySqlConnectionStrings { get; set; }
        public string SqlServerConnectionStrings { get; set; }
        public string ESWizplantUrl { get; set; }    
    }
}
