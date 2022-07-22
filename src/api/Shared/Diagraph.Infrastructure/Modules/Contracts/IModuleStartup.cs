using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Diagraph.Infrastructure.Modules.Contracts;

public interface IModuleStartup : IStartup
{
    public IConfiguration Configuration { get; set; }
}