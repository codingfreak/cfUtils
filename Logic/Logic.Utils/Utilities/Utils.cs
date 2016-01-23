namespace s2.s2Utils.Logic.Utils.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;

    using s2.s2Utils.Logic.Base.Interfaces;
    using s2.s2Utils.Logic.Base.Utilities;
    using s2.s2Utils.Logic.Utils.Interfaces;
    
    /// <summary>
    /// This class serves as a central collection for all types derived from <see cref="BaseUtil{TEntity,TContext}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To handle logging in a loosely coupled manner, any assembly that want's to access this class must call
    /// <see cref="Init"/> once. At this point the DI can decide, which kind of <see cref="ILogger"/> to pass to
    /// the method.
    /// </para>
    /// <para>
    /// Notice that each access to one of the properties will generate a fresh instane of the <see cref="BaseUtil{TEntity,TContext}"/>-based
    /// type.
    /// </para>
    /// </remarks>
    public static class Utils
    {
        #region static fields

        private static IContextResolver _contextResolver;

        /// <summary>
        /// Holds all util-types.
        /// </summary>
        private static List<Type> _utilTypes;

        #endregion

        #region properties

        /// <summary>
        /// Internal property to hold the resolver to use.
        /// </summary>
        private static IContextResolver ContextResolver
        {
            get
            {
                if (_contextResolver != null)
                {
                    return _contextResolver;
                }
                // The caller "forgot" to call Init. So lets try to get the resolver
                // using reflection now.
                Trace.TraceInformation("Trying to auto-detect ContextResolver.");
                var interfaceType = typeof(IContextResolver);
                var type = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && SecurityUtil.HasUserAccessToFile(a.Location)).SelectMany(s => s.GetTypes()).FirstOrDefault(p => !p.IsAbstract && interfaceType.IsAssignableFrom(p));
                if (type != null)
                {
                    // we got the resolver-type
                    _contextResolver = Activator.CreateInstance(type) as IContextResolver;                        
                }
                return _contextResolver;
            }
            set
            {
                _contextResolver = value;
            }
        }

        /// <summary>
        /// Internal property to hold the logger to use.
        /// </summary>
        private static ILogger Logger { get; set; }

        /// <summary>
        /// Retrieves all types of Utils deriving from <see cref="BaseUtil{TEntity,TContext}"/>.
        /// </summary>
        /// <remarks>
        /// The property will load all types if this didn't happen before. The result will be stored locally.
        /// </remarks>
        private static IEnumerable<Type> UtilTypes
        {
            get
            {
                if (_utilTypes != null)
                {
                    return _utilTypes;
                }
                _utilTypes = new List<Type>();
                // get the type of the generic base class of all utils
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && SecurityUtil.HasUserAccessToFile(a.Location)))
                {
                    // search for all types which are deriving from BaseUtil
                    Trace.TraceInformation("Searching assembly {0}", assembly.FullName);
                    try
                    {
                        var types = assembly.GetTypes().Where(t => !t.IsAbstract && t.BaseType != null && t.BaseType.Name.StartsWith("BaseUtil")).ToList();
                        _utilTypes.AddRange(types);
                    }                    
                    catch (Exception ex)
                    {
                        Logger.LogException("S2UTI-001", ex);                           
                    }                    
                }
                return _utilTypes;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Instance of the logic for clients.
        /// </summary>
        public static BaseUtil<TEntity, TContext> Get<TEntity, TContext>() where TEntity : class, IEntity where TContext : DbContext
        {
            var baseType = typeof(BaseUtil<TEntity, TContext>);
            var type = UtilTypes.SingleOrDefault(t => t.BaseType == baseType);
            if (type != null)
            {
                return Activator.CreateInstance(type, Logger, ContextResolver) as BaseUtil<TEntity, TContext>;
            }
            return null;
        }

        /// <summary>
        /// Instance of the logic for clients.
        /// </summary>
        public static TUtil GetDirect<TUtil>() where TUtil : class
        {
            var baseType = typeof(TUtil);
            return Activator.CreateInstance(baseType, Logger, ContextResolver) as TUtil;
        }

        /// <summary>
        /// This method has to be called once at the beginning of the applications
        /// lifecycle to provide the utils.
        /// </summary>
        /// <param name="logger">The logger to use inside all utilities.</param>
        /// <param name="contextResolver"></param>                
        public static void Init(ILogger logger, IContextResolver contextResolver)
        {
            Logger = logger;
            ContextResolver = contextResolver;
        }

        #endregion
    }
}