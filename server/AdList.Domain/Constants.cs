namespace AdList.Domain
{
    public static class Constants
    {
        public static class SmartTask
        {
            public const int TitleMaxLength = 200;
            public const int DescriptionMaxLength = 2000;
        }

        public static class TrackingEntity
        {
            public const int AuthorMaxLength = 255;
        }

        public static class ApplicationUser
        {
            public const int EmailMaxLength = Constants.TrackingEntity.AuthorMaxLength;
            public const int NameMaxLength = 100;
        }

        public static class Claims
        {
            public const string Name = "name";
        }
    }
}
