using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace RedStudio.RedAPI
{
    public struct GameVersion
    {
        public static string UnityVersion => Application.version;

        int[] _version;

        public int Major => _version[0];
        public int Minor => _version[1];
        public int Patch => _version[2];

        public GameVersion(string data)
        {
            if (!IsValidData(data, out  _version)) throw new FormatException();
        }

        public static bool IsValidData(string data) => IsValidData(data, out _);

        static bool IsValidData(string data, out int[] tmp)
        {
            tmp = null;
            if (string.IsNullOrEmpty(data)) return false;
            var tt = data.Split('.');
            if (tt.Length != 3) return false;
            if (!tt.All(i => int.TryParse(i, out _))) return false;
            tmp = tt.Select(i => int.Parse(i)).ToArray();
            if (!tmp.All(i => i >= 0)) return false;
            return true;
        }

        public static bool operator < (GameVersion left, GameVersion right)
        {
            return left.Major < right.Major || left.Minor < right.Minor || left.Patch < right.Patch;
        }
        public static bool operator > (GameVersion left, GameVersion right)
        {
            return left.Major > right.Major || left.Minor > right.Minor || left.Patch > right.Patch;
        }
        public static bool operator ==(GameVersion left, GameVersion right)
        {
            return left._version.Zip(right._version, (a,b) => (a,b)).All(i => i.a==i.b);
        }
        public static bool operator !=(GameVersion left, GameVersion right) => !(left==right);
        public static bool operator <=(GameVersion left, GameVersion right) => left == right || left < right;
        public static bool operator >=(GameVersion left, GameVersion right) => left == right || left > right;


    }
}