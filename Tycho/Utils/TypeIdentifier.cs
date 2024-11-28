﻿using System;
using System.IO.Hashing;
using System.Text;

namespace Tycho.Utils
{
    internal static class TypeIdentifier
    {
        public static string GetId<T>()
        {
            return GetId(typeof(T));
        }

        public static string GetId(Type type)
        {
            return $"{GetShortName(type)}+{GetShortId(type)}";
        }

        private static string GetShortName(Type type)
        {
            var typeName = type.Name;
            var genericPartIndex = typeName.IndexOf('`');
            return genericPartIndex == -1 ? typeName : typeName[..genericPartIndex];
        }

        private static string GetShortId(Type type)
        {
            var typeHash = Crc32.Hash(Encoding.UTF8.GetBytes(type.AssemblyQualifiedName));
            return BitConverter.ToString(typeHash).Replace("-", "");
        }
    }
}