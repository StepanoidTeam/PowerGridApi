using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridEngine
{
    public class BuildCityAction : UserAction<BuildCityResponse>
    {
        public City City { get; private set; }

        public BuildCityAction(User user, City city) : base(user)
        {
            City = city;
        }

    }
}
