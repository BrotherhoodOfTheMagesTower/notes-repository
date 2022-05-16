namespace NotesRepository.Data
{
    public class Flags
    {
        public volatile bool navMenuIsLoading = false;

        public bool setNavMenuLoadingStatus(bool status) => navMenuIsLoading = status;

        public bool getNavMenuLoadingStatus() => navMenuIsLoading;
    }
}
