using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerGridEngine
{
    public partial class EnergoServer
    {
        public void RegisterMap(Map map)
        {
            if (string.IsNullOrWhiteSpace(map.Id))
                throw new Exception(Constants.Instance.ErrorMessage.Cant_Create_Map_Without_Id);
            if (Maps.ContainsKey(map.Id))
                throw new Exception(Constants.Instance.ErrorMessage.Duplicate_Map_Id_Detected);
            Maps.Add(map.Id, map);
        }

        public Map LookupMap(string id, out string errMsg)
        {
            errMsg = string.Empty;
            var idd = id.NormalizeId();
            if (!Maps.ContainsKey(idd))
            {
                errMsg = string.Format(Constants.Instance.ErrorMessage.Cant_Find_Map, id);
                return null;
            }
            return Maps[idd];
        }

        public List<string> GetAllMapIds()
        {
            return Maps.Keys.ToList();
        }
    }
}
