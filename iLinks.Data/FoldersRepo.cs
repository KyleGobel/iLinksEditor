using System.Collections.Generic;
using System.Linq;

namespace iLinks.Data
{
   
    public class FoldersRepo
    {
        private readonly iLinksDataContext _context;

        public FoldersRepo()
        {
            _context = new iLinksDataContext();
        }

        public IEnumerable<Folder> GetChildFolders(int id)
        {
            return _context.Folders.Where(x => x.Parent_Folder_ID == id).ToList();
        }

        public IEnumerable<Folder> GetRootFolders()
        {
            return _context.Folders.Where(x => x.Parent_Folder_ID == null);
        }
    }
}