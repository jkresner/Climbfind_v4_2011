using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using LocationBingView = cf.Entities.PlaceBingMapView;
using cf.DataAccess.Interfaces;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// LocationBingView (Map view settings) reader / writer
    /// </summary>
    internal class LocationBingViewRepository : AbstractCfEntitiesEf4DA<LocationBingView, Guid>,
        IKeyEntityAccessor<LocationBingView, Guid>, IKeyEntityWriter<LocationBingView, Guid>
    {
        public LocationBingViewRepository() : base() { }
        public LocationBingViewRepository(string connectionStringKey) : base(connectionStringKey) { }
    }
}
