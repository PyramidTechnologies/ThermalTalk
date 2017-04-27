namespace ThermalTalk
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains an ordered sequence of sections
    /// </summary>
    public interface IDocument
    {
        IList<ISection> Sections { get; set; }
    }
}
