using System;

namespace PowerGridEngine
{
    public abstract class BaseEntityWithIdAndName: BaseEnergoEntity
    {
        
        public string Name { get; private set; }

        
        public string Id
        {
            get
            {
                return Name.NormalizeId();
            }
            set { }
        }
 
        public BaseEntityWithIdAndName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(string.Format("Can't create unnamed {0}", this.GetType().FullName));
            Name = name;
        }
    }

    
    public abstract class ParentableBaseEntity<TParent>: BaseEntityWithIdAndName
    {
        public TParent Parent { get; set; }

        public ParentableBaseEntity(string name, TParent parent): base(name)
        {
            Parent = parent;
        }
    }

    
    public abstract class ChildableBaseEntity<TChilds>: BaseEntityWithIdAndName
    {
        public TChilds Childs { get; private set; }

        public ChildableBaseEntity(string name): base(name)
        {
            Childs = (TChilds)Activator.CreateInstance(typeof(TChilds));
        }

        public ChildableBaseEntity(string name, TChilds childs) : base(name)
        {
            Childs = childs;
        }
    }

    
    public abstract class FullBaseEntity<TParent, TChilds> : ParentableBaseEntity<TParent>
    {
        public TChilds Childs { get; private set; }

        public FullBaseEntity(string name, TParent parent) : base(name, parent)
        {
            Childs = (TChilds)Activator.CreateInstance(typeof(TChilds));
            //to do automatically parent setting
            //if(Childs is System.Collections.IEnumerable)
            //{
            //    var enumerable = Childs as System.Collections.IEnumerable;
            //    foreach(var item in enumerable)
            //        if(item is ParentableBaseEntity<BaseEntity>)
            //        {
            //            (item as ParentableBaseEntity<BaseEntity>).Parent = this;
            //        }
            //}
        }

        public FullBaseEntity(string name, TParent parent, TChilds childs) : base(name, parent)
        {
            Childs = childs;
        }
    }

}
