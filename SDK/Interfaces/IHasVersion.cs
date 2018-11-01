using System.Collections.Generic;
using Nebula.SDK.Objects.Shared;

namespace Nebula.SDK.Interfaces
{
    public interface IHasVersion
    {
        List<BaseVersion> GetSortedVersions();
    }
}