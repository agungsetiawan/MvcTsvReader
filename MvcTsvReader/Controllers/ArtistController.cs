using MvcTsvReader.Common;
using MvcTsvReader.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcTsvReader.Controllers
{
    public class ArtistController : Controller
    {
        //
        // GET: /Artist/

        public ActionResult Index()
        {
            ArtistRepository repo = new ArtistRepository();
            var result = repo.GetArtists(Folder.TvsDir + "artists.tsv");
            return View(result);
        }

    }
}
