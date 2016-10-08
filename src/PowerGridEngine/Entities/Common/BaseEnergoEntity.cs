using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    
    public abstract class BaseEnergoEntity
    {
        /// <summary>
        /// Returns info about specific entity. By default return full info
        /// </summary>
        /// <param name="options">By options we can specify which exactly information parts we want to see in response</param>
        /// <returns></returns>
        public abstract BaseEnergoModel ToModel(IViewModelOptions options = null);
    }

    /// <summary>
    /// Model of some entity designed to be returned outside Engine
    /// </summary>
    public abstract class BaseEnergoModel
    {
    }
}
