
using System.Collections.Generic;

namespace PowerGridEngine
{ 
    /// <summary>
    /// Model of some entity designed to be returned outside Engine
    /// </summary>
    public abstract class BaseEnergoModel<TEntity, TViewOptions> where TEntity: BaseEnergoEntity where TViewOptions : AbstractModelViewOptions
    {
        protected TEntity Entity { get; private set; }

        protected BaseEnergoModel(TEntity entity)
        {
            this.Entity = entity;
        }

        /// <summary>
        /// Return some part of model. By default return full info
        /// </summary>
        public abstract Dictionary<string, object> GetInfo(TViewOptions options = null);
    }
}
