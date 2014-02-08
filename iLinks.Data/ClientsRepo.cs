using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iLinks.Data
{
    public class ClientsRepo
    {
        private readonly iLinksDataContext _context;
        public ClientsRepo()
        {
            _context =
                new iLinksDataContext(
                    new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString));
        }
        public IObservable<List<Client>> GetAll()
        {
            return Observable.Return(_context.Clients.ToList());
        }
    }
    public class PagesRepo
    {
        private readonly iLinksDataContext _context;
        public PagesRepo()
        {
            _context =
                new iLinksDataContext(
                    new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString));
        }
        public List<Page> GetByFolderId(int folderId)
        {
            return _context.Pages.Where(x => x.Folder_ID == folderId).ToList();
        }
    }
}
