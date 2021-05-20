using System.Collections.Generic;
using System.Threading.Tasks;
using Netrunner.Shared.Users;
using WampSharp.V2.Rpc;

namespace Netrunner.Shared.Services
{
    public interface IContactService
    {
        [WampProcedure("netrunner.contacts.get_contacts")]
        Task<IEnumerable<Contact>> GetContacts();

        [WampProcedure("netrunner.contacts.remove_contact")]
        Task RemoveContact(string username);

        [WampProcedure("netrunner.contacts.add_contact")]
        Task AddContact(Contact contact);
    }
}