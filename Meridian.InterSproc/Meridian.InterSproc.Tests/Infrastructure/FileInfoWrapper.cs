namespace Meridian.InterSproc.Tests.Infrastructure
{
    using Meridian.InterSproc.Definitions;

    public class FileInfoWrapper : IFileInfoWrapper
    {
        public IDirectoryInfoWrapper Directory => throw new System.NotImplementedException();

        public bool Exists => throw new System.NotImplementedException();

        public string FullName => throw new System.NotImplementedException();

        public string Name => throw new System.NotImplementedException();

        public void Delete()
        {
            throw new System.NotImplementedException();
        }
    }
}