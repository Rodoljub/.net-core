using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Models;
using Quantum.Data.Repositories.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
    public interface IEventRepository : IBaseRepository<Event, IdentityUser>
    {
        Task<List<Event>> GetEvents(List<string> clientsIds);

        Task<List<ClientsEvents>> GetClientEvents(List<string> clientsIds);

        Task InsertProfileImageReadyEvent(Event userNotification, IdentityUser user);

    }
}
