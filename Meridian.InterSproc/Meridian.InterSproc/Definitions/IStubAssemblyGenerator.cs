namespace Meridian.InterSproc.Definitions
{
    using System.IO;
    using System.Reflection;

    public interface IStubAssemblyGenerator
    {
        Assembly Create<DatabaseContractType>(FileInfo destinationLocation)
            where DatabaseContractType : class;
    }
}
