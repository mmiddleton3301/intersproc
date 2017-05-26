namespace Meridian.InterSproc
{
    using System.Reflection;
    using StructureMap;

    public class CustomStubRegistry : Registry
    {
        public CustomStubRegistry(Assembly temporaryStubAssembly)
        {
            this.Scan((x) =>
            {
                // Just scan our temporary assembly.
                x.Assembly(temporaryStubAssembly);

                // Just register against the first implementation - there's
                // only one, after all.
                x.RegisterConcreteTypesAgainstTheFirstInterface();
            });
        }
    }
}
