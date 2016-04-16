namespace Beatshake.Core
{
    public interface IInstrumentalComponentIdentification
    {
        /// <summary>
        /// The identification of the instrument, in which this component is contained
        /// </summary>
        IInstrumentalIdentification ContainingInstrument { get; set; }

        /// <summary>
        /// The name of the component of the instrument.
        /// e.g. ride
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The number of this component, if there are multiple instances of the same component in an instrument
        /// </summary>
        int Number { get; set; }
    }
}