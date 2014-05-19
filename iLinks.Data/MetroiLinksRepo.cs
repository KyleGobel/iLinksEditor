using System;
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
            var dbEntry = _context.Metro_iLinks.Single(x => x.ID == iLink.ID);

            dbEntry.BackLinkTarget = iLink.BackLinkTarget;
            dbEntry.BackLinkTitle = iLink.BackLinkTitle;
            dbEntry.BackLinkURL = iLink.BackLinkURL;
            dbEntry.ClientLogoAltText = iLink.ClientLogoAltText;
            dbEntry.ClientLogoGraphicLocation = iLink.ClientLogoGraphicLocation;
            dbEntry.ClientLogoLinkURL = iLink.ClientLogoLinkURL;
            dbEntry.ClientLogoTargetWindow = iLink.ClientLogoTargetWindow;
            dbEntry.FontSizePx = iLink.FontSizePx;
            dbEntry.HomeSearchText = iLink.HomeSearchText;
            dbEntry.HomeSearchURL = iLink.HomeSearchURL;
            dbEntry.OriginationPage = iLink.OriginationPage;
            dbEntry.OriginationPageTarget = iLink.OriginationPageTarget;
            dbEntry.PageBGColor = iLink.PageBGColor;
            dbEntry.PageLinkColor = iLink.PageLinkColor;
            dbEntry.PageTextColor = iLink.PageTextColor;
            dbEntry.ProductLogoAltText = iLink.ProductLogoAltText;
            dbEntry.ProductLogoGraphicLocation = iLink.ProductLogoGraphicLocation;
            dbEntry.ProductLogoLinkURL = iLink.ProductLogoLinkURL;
            dbEntry.ProductLogoTargetWindow = iLink.ProductLogoTargetWindow;
            dbEntry.SEOMetaDesc = iLink.SEOMetaDesc;
            dbEntry.SEOMetaKeys = iLink.SEOMetaKeys;
            dbEntry.HomeSearchLabelText = iLink.HomeSearchLabelText;
         
            _context.SubmitChanges();
        }
    }
}