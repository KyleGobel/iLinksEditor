using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace iLinks.Data
{
    public class MetroiLinksRepo
    {
        private readonly iLinksDataContext _context;

        public MetroiLinksRepo()
        {
            _context = new iLinksDataContext();
        }

        public IEnumerable<Metro_iLink> GetAll()
        {
            return _context.Metro_iLinks.ToList();
        }
        public void Update(Metro_iLink iLink)
        {
            _context.SubmitChanges();

        }
    }
}