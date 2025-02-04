using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly string frontendDomain;

        public EnvironmentService(string frontendDomain)
        {
            this.frontendDomain = frontendDomain;
        }
        public string GetFrontendDomain()
        {
            return frontendDomain;
        }
    }
}
