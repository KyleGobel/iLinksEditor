using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reactive.Linq;

namespace iLinks.Data
{
    public class CommunityProfilesRepo
    {
        private readonly iLinksDataContext _context;
        public CommunityProfilesRepo()
        {
            _context =
               new iLinksDataContext(
                   new SqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString));
        }

        public List<Page> GetCommunityProfiles(int clientId)
        {
            var pages = _context.CommunityProfiles.Where(x => x.ClientId == clientId)
                .Join(_context.Pages, x => x.PageId, x => x.ID, (profile, page) => page)
                .OrderBy(x => x.Title);

            return pages.ToList();
        }

        public void UpdateCommunityProfiles(int clientId, IEnumerable<Page> pages)
        {
            var existingProfiles = _context.CommunityProfiles.Where(x => x.ClientId == clientId);
            foreach (var p in existingProfiles)
            {
                _context.CommunityProfiles.DeleteOnSubmit(p);
            }

            _context.SubmitChanges();
            foreach (var p in pages)
            {
                _context.CommunityProfiles.InsertOnSubmit(new CommunityProfile
                {
                    ClientId = clientId,
                    PageId = p.ID
                });
            }

            _context.SubmitChanges();
        }
         
    }
}