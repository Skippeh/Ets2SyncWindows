namespace PrismLibrary
{
    public class GameSavedEventArgs
    {
        public GameProfile Profile { get; }
        public GameSave Save { get; }

        public GameSavedEventArgs(GameProfile profile, GameSave save)
        {
            Profile = profile;
            Save = save;
        }
    }
}