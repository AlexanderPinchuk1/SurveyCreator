namespace BSUIR.Survey.Foundation
{
    public interface ICurrentUserProvider
    {
        public Guid? GetUserId();

        public bool IsAuthenticated();
    }
}