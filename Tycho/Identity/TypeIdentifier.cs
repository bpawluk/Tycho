using System;
using System.IO.Hashing;
using System.Linq;
using System.Text;

namespace Tycho.Identity
{
    internal static class TypeIdentifier
    {
        public static string GetId<T>()
        {
            return GetId(typeof(T));
        }

        public static string GetId(Type type)
        {
            if (type.IsGenericParameter)
            {
                return type.Name;
            }

            if (type.IsGenericType)
            {
                string[] genericArguments = type.GetGenericArguments().Select(GetId).ToArray();
                return $"{GetFlatId(type.GetGenericTypeDefinition())}<{string.Join(",", genericArguments)}>";
            }

            return GetFlatId(type);
        }

        private static string GetFlatId(Type type)
        {
            return $"{GetShortName(type)}+{GetShortId(type)}";
        }

        private static string GetShortName(Type type)
        {
            string typeName = type.Name;
            int genericPartIndex = typeName.IndexOf('`');
            return genericPartIndex == -1 ? typeName : typeName[..genericPartIndex];
        }

        private static string GetShortId(Type type)
        {
            byte[] typeHash = Crc32.Hash(Encoding.UTF8.GetBytes(type.AssemblyQualifiedName));
            return BitConverter.ToString(typeHash).Replace("-", "");
        }
    }
}
