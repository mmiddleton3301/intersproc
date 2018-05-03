namespace Meridian.InterSproc.Tests.Infrastructure
{
    using Meridian.InterSproc.Definitions;

    public class FileInfoWrapperFactory : IFileInfoWrapperFactory
    {
        public IFileInfoWrapper Create(string fileName)
        {
            IFileInfoWrapper toReturn = new FileInfoWrapper();

            return toReturn;
        }
    }
}