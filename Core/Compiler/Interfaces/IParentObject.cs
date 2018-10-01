using System.Collections.Generic;
using Nebula.Compiler.Objects;

namespace Nebula.Compiler.Interfaces
{
    /// <summary>
    /// Any node that can have children, needs to implement this interface so that
    /// the children can be processed.
    /// </summary>
    public interface IParentObject
    {
        List<RootObject> GetChildren();
    }
}