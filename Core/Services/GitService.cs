using LibGit2Sharp;

namespace Nebula.Core.Services
{
    public interface IGitService
    {
        Repository GetRepository(string repoLocation);
        string Clone(string repoUrl, string destinationDirectory);
        string Init(string repoLocation);
    }

    public class GitService : IGitService
    {
        public string Clone(string repoUrl, string destinationDirectory)
        {
            return Repository.Clone(repoUrl, destinationDirectory);
        }

        public Repository GetRepository(string repoLocation)
        {
            return new Repository(repoLocation);
        }

        public string Init(string repoLocation)
        {
            return Repository.Init(repoLocation);
        }
    }
}