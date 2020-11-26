using System.Threading.Tasks;

namespace JudoLibrary.Api.BackgroundServices.SubmissionVoting
{
    public interface ISubmissionVoteSink
    {
        ValueTask Submit(VoteForm voteForm);
    }
}