using MvcTsvReader.Common;
using MvcTsvReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcTsvReader.Repository
{
    public class ArtistRepository
    {

        public List<Artist> GetArtists(string fileName)
        {
            var result=TsvReader.Read<Artist>(fileName, true);
            return result;
        }
    }
}