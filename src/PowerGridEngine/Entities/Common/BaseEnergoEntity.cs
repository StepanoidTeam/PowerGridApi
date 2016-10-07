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
        /// by default return full info
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract BaseEnergoViewModel ToViewModel(IViewModelOptions options = null);
    }

    
    public abstract class BaseEnergoViewModel
    {
    }
}
