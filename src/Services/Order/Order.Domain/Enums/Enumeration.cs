
using System.Reflection;

namespace Order.Domain.Enums
{
    public abstract class Enumeration
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            if(obj is null)
                return false;

            if(obj is not Enumeration enumObj)
                return false;

            Type currentType = GetType();
            Type objType = obj.GetType();

            bool isEqualType = currentType == objType;
            bool isEqualId = Id == enumObj.Id;

            return isEqualType && isEqualId;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                            .Select(f => f.GetValue(null))
                            .Cast<T>();
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }
            
        public static T FromValue<T>(int value) where T : Enumeration
        {
            var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }

        public static T FromDisplayName<T>(string displayName) where T : Enumeration
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;
        }
    }
}