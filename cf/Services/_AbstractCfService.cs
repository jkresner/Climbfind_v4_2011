using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Identity;
using System.Diagnostics;
using cf.Caching;
using NetFrameworkExtensions.Diagnostics;
using cf.Instrumentation;

namespace cf.Services
{
    /// <summary>
    /// Cf service that is executing in the content of a Climbfind application with CFPrincpal and Cache context
    /// </summary>
    public abstract class AbstractCfService
    {
        protected PostService postSvc { get { if (_postSvc == null) { _postSvc = new PostService(); } return _postSvc; } } PostService _postSvc;

        /// <summary>
        /// Easy property based access to the current user or throw an exception if the code is executing without a valid user context
        /// </summary>
        protected Guid currentUserID { get { return currentUser.UserID; } }
        private CfPrincipal _currentUser;
        protected CfPrincipal currentUser
        {
            get
            {
                if (_currentUser != null) { return _currentUser; }
                
                if (!CfPrincipal.CurrentIsAuthenticated) 
                {
                    //-- Get calling method name
                    var callingMethod = new StackTrace().GetFrame(1).GetMethod().Name;
                                        
                    throw new AccessViolationException(callingMethod + ": Thread principal does not appear to be a valid authenticated cfPrincipal"); 
                }

                _currentUser =  new CfPrincipal();

                return _currentUser;
            }
        }
    }
}
