using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.AuthorizationServer.Services.Contracts
{
    public interface IDocumentService
    {
        Task<string> GetFileContentAsString(string fileName);
    }
}
