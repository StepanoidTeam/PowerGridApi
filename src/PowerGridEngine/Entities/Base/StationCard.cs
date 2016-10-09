using System;

namespace PowerGridEngine
{
    [Flags]
    public enum StationType
    {
        None,//for special cards purposes (like card of 3 Stage)
        Coal,
        Oil,
        Trash,//really???
        Atomic,
        Nature
    }

    /// <summary>
    /// Обязательной картой в деке является карта экологической станции со стоимостью 13
    /// </summary>
    public class StationCard 
    {
        public StationType Type { get; private set; }

        public int Cost { get; private set; }

        /// <summary>
        /// Если Type == None - это карта 3 Этапа
        /// </summary>
        public bool Is3StageCard
        {
            get { return Type == StationType.None; }
        }

        public bool IsSpecific
        {
            get { return Is3StageCard || (Type == StationType.Nature && Cost == 13); }
        }

        /// <summary>
        /// Количество ресов, необходимых для протопки данной электростанции за 1 ход
        /// </summary>
        public int ResourceQty { get; private set; }

        /// <summary>
        /// Сколько домов запитает
        /// </summary>
        public int HousesQty { get; private set; }

        public StationCard(StationType type, int cost, int resourceQty, int housesQty)
        {
            Type = type;
            Cost = cost;
            ResourceQty = resourceQty;
            HousesQty = housesQty;
        }
    }
}
