namespace Test
{
    using System;

    public class Generic<T>
    {
        public static T Value { get; set; }

        public static K StaticMethod<K>(K k)
        {
            return k;
        }

        public K InstanceMethod<K>(K k)
        {
            return k;
        }
    }
}
