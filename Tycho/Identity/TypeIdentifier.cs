using System;
using System.IO.Hashing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tycho.Identity
{
    internal static class TypeIdentifier
    {
        private static readonly ConditionalWeakTable<Type, string> s_flatIds = new();

        public static string GetId<T>()
        {
            return GetId(typeof(T));
        }

        public static string GetId(Type type)
        {
            if (type.IsArray)
            {
                string elementId = GetId(type.GetElementType()!);
                string arraySuffix = type.IsSZArray
                    ? "[]"
                    : type.GetArrayRank() == 1
                        ? "[*]"
                        : $"[{new string(',', type.GetArrayRank() - 1)}]";
                return $"{elementId}{arraySuffix}";
            }

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
            return s_flatIds.GetValue(type, ResolveFlatId);
        }

        private static string ResolveFlatId(Type type)
        {
            return type.GetCustomAttribute<TychoIdAttribute>(inherit: false)?.Id ?? $"{GetShortName(type)}+{GetShortId(type)}";
        }

        private static string GetShortName(Type type)
        {
            string typeName = type.Name;
            int genericPartIndex = typeName.IndexOf('`');
            return genericPartIndex == -1 ? typeName : typeName[..genericPartIndex];
        }

        private static string GetShortId(Type type)
        {
            string stableName = $"{type.Assembly.GetName().Name}:{type.FullName}";
            byte[] typeHash = XxHash64.Hash(Encoding.UTF8.GetBytes(stableName));
            return Convert.ToBase64String(typeHash).TrimEnd('=').Replace('+', '#').Replace('/', '&');
        }
    }
}
