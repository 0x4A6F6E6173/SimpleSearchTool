namespace SearchTool
{
    public struct Option<T> where T : notnull
    {
        public static Option<T> Some(T value) => new Option<T>(value);
        public static Option<T> None => default;

        private readonly bool isSome;
        private readonly T value;

        Option(T value)
        {
            this.value = value;
            isSome = this.value is { };
        }

        public bool IsSome(out T value)
        {
            value = this.value;
            return isSome;
        }
    }

    public static class OptionExtentions
    {
        public static U Match<T, U>(
            this Option<T> option,
            Func<T, U> onIsSome,
            Func<U> onIsNone) where T : notnull =>
                option.IsSome(out var value) ? onIsSome(value) : onIsNone();

        public static Option<U> Bind<T, U>(
            this Option<T> option,
            Func<T, Option<U>> binder) where T : notnull where U : notnull =>
                option.Match(
                    onIsSome: binder,
                    onIsNone: () => Option<U>.None);

        public static Option<U> Map<T, U>(
            this Option<T> option,
            Func<T, U> mapper) where T : notnull where U : notnull =>
                option.Bind(value => Option<U>.Some(mapper(value)));

        public static Option<T> Filter<T>(
            this Option<T> option,
            Predicate<T> predicate) where T : notnull =>
            option.Bind(value => predicate(value) ? option : Option<T>.None);

        public static T DefaultValue<T>(
            this Option<T> option,
            T defaultValue) where T : notnull =>
            option.Match(
                onIsSome: value => value,
                onIsNone: () => defaultValue);
    }
}
