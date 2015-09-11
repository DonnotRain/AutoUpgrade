using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rainy.ClientService.Models
{
    public class AddVersionModel
    {
        public string VersionName { get; set; }
        public string Description { get; set; }
        public string IsLastVersion { get; set; }
        public string[] FileToDelete { get; set; }
        public string[] FilesToUpgrade { get; set; }
    }
}
