using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using cf.DataAccess.AdoNet2;
using cf.DataAccess.Interfaces;
using System.Data;
using System.Data.SqlClient;
using cf.Entities.Enum;
using Microsoft.SqlServer.Types;
using cf.DataAccess.EntityFramework;
using Media = cf.Entities.Media;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Media Repository
    /// </summary>
    internal class MediaRepository : AbstractCfEntitiesEf4DA<Media, Guid>,
        IKeyEntityAccessor<Media, Guid>, IKeyEntityWriter<Media, Guid>
    {
        public MediaRepository() : base() { }
        public MediaRepository(string connectionStringKey) : base(connectionStringKey) { }

        public IQueryable<Media> GetObjectsMedia(Guid id)
        {
            return from m in Ctx.Medias where m.ObjectMedias.Any( om => om.OnOjectID == id) select m;
        }

        public void AddMediaTag(ObjectMedia tag)
        {
            Ctx.ObjectMedias.AddObject(tag);
            SaveChanges();
        }

        public void RemoveMediaTag(ObjectMedia tag)
        {
            Ctx.ObjectMedias.DeleteObject(tag);
            SaveChanges();
        }

        public void RemoveAllMediaTagForObject(Guid objectID)
        {
            var allTagsForObjectg = Ctx.ObjectMedias.Where( om => om.OnOjectID == objectID );
            foreach (var tag in allTagsForObjectg) { Ctx.ObjectMedias.DeleteObject(tag); }
            SaveChanges();
        }

        public IQueryable<Media> GetMediaByUserWithOpinions(Guid id)
        {
            return Ctx.Medias.Include("MediaOpinions").Include("ObjectMedias").Where(m => m.AddedByUserID == id);
        }

        public IQueryable<Media> GetMediaByUserWithObjectRefereces(Guid id)
        {
            return Ctx.Medias.Include("ObjectMedias").Where(m => m.AddedByUserID == id);
        }
    }
}
