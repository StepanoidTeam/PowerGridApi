using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    /// <summary>
    /// Inherited in case class should allow to handle UserActions
    /// </summary>
    public interface IUserActionHandler
    {
        T RouteAction<T>(UserAction action) where T : ActionResponse;
    }
}
