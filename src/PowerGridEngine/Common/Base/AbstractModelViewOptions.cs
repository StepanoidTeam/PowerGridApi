
namespace PowerGridEngine
{
    /// <summary>
    /// Options determines which parts of Model need to return
    /// </summary>
    public abstract class AbstractModelViewOptions
    {
    }

    /// <summary>
    /// With Id and Name
    /// </summary>
    public abstract class AbstractIdNameModelViewOptions: AbstractModelViewOptions
    {
        public bool Id { get; set; }

        public bool Name { get; set; }

        public AbstractIdNameModelViewOptions(bool defaultValue)
        {
            Id = defaultValue;
            Name = defaultValue;
        }
    }
}
