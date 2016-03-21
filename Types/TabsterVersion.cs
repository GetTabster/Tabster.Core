#region

using System;
using System.Text.RegularExpressions;

#endregion

namespace Tabster.Core.Types
{
    /// <summary>
    ///     Provides enumerated values to use to set version string formatting options.
    /// </summary>
    [Flags]
    public enum TabsterVersionFormatFlags
    {
        /// <summary>
        ///     Specifies that no options are set.
        /// </summary>
        None = 0x0,

        /// <summary>
        ///     Specifies that the build version should be appended to the version string.
        /// </summary>
        Build = 0x1,

        /// <summary>
        ///     Specifies that the build version should be appended in a human-friendly format to the version string. (Ex: 1.0.2
        ///     (Build 12))
        /// </summary>
        BuildString = 0x2,

        /// <summary>
        ///     Specifies that the commit id should be appended to the version string using the full format. (Ex: 1.0.2
        ///     57bf835dd5a21e031df7d8940c9483125b1575e0)
        /// </summary>
        CommitFull = 0x4,

        /// <summary>
        ///     Specifies that the commit id should be appended to the version string using the short format. (Ex: 1.0.2 57bf835)
        /// </summary>
        CommitShort = 0x8,

        /// <summary>
        ///     Specifies that the version string should be truncated of trailing zeros.
        /// </summary>
        Truncated = 0x10,
    }

    /// <summary>
    ///     Represents the version number.
    /// </summary>
    public class TabsterVersion : IComparable, IComparable<TabsterVersion>, IEquatable<TabsterVersion>
    {
        private static readonly Regex BuildRegex = new Regex(@"\(Build (\d+)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        ///     Initializes a new TabsterVersion.
        /// </summary>
        public TabsterVersion()
        {
        }

        /// <summary>
        ///     Initializes a new TabsterVersion based off of formatted string.
        /// </summary>
        /// <param name="str">Formatted version string.</param>
        public TabsterVersion(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            var dashSplit = str.Split('-');
            var spaceSplit = dashSplit[0].Split(' ');
            var decimalSplit = spaceSplit[0].Split('.');

            if (decimalSplit.Length < 2)
                throw new ArgumentException("String must contain at least 2 decimals.", str);

            var major = 0;
            var minor = 0;
            var revision = 0;
            var build = 0;
            Sha1Hash hash = null;

            if (!int.TryParse(decimalSplit[0], out major))
                throw new ArgumentException("Version components must contain integers.", str);
            if (decimalSplit.Length >= 1 && !int.TryParse(decimalSplit[0], out major))
                throw new ArgumentException("Version components must contain integers.", str);
            if (decimalSplit.Length >= 2 && !int.TryParse(decimalSplit[1], out minor))
                throw new ArgumentException("Version components must contain integers.", str);
            if (decimalSplit.Length >= 3 && !int.TryParse(decimalSplit[2], out revision))
                throw new ArgumentException("Version components must contain integers.", str);
            if (decimalSplit.Length >= 4 && !int.TryParse(decimalSplit[3], out build))
                throw new ArgumentException("Version components must contain integers.", str);

            // extract build version from (Build num) format
            if (str.IndexOf("(Build ", StringComparison.OrdinalIgnoreCase) >= 0 && build == 0)
            {
                var match = BuildRegex.Match(str);
                if (match.Groups.Count > 1)
                {
                    int.TryParse(match.Groups[1].Value, out build);
                }
            }

            // check for commit id after dash
            if (dashSplit.Length > 1)
            {
                var last = dashSplit[dashSplit.Length - 1];
                Sha1Hash.TryParseHash(last, out hash);
            }

            else if (spaceSplit.Length > 1) // check for commit id after space
            {
                var last = spaceSplit[spaceSplit.Length - 1];
                Sha1Hash.TryParseHash(last, out hash);
            }

            Major = major;
            Minor = minor;
            Revision = revision;
            Build = build;
            Commit = hash;
        }

        /// <summary>
        ///     Initializes a new TabsterVersion based off of the specified components.
        /// </summary>
        /// <param name="major">Major component.</param>
        /// <param name="minor">Minor component.</param>
        /// <param name="revision">Revision component.</param>
        /// <param name="build">Build component.</param>
        /// <param name="commit">Commit component.</param>
        public TabsterVersion(int major, int minor, int revision, int build, Sha1Hash commit = null)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
            Build = build;
            Commit = commit;
        }

        /// <summary>
        ///     Initializes a new TabsterVersion based off of a System.Version object.
        /// </summary>
        /// <param name="version">The System.Version object.</param>
        /// <param name="commit">Commit component.</param>
        public TabsterVersion(Version version, Sha1Hash commit = null)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            Major = version.Major;
            Minor = version.Minor;
            Revision = version.Build;
            Build = version.Revision;
            Commit = commit;
        }

        /// <summary>
        ///     Gets the value of the build component of the version number
        /// </summary>
        public int Build { get; private set; }

        /// <summary>
        ///     Gets the value of the major component of the version number.
        /// </summary>
        public int Major { get; private set; }

        /// <summary>
        ///     Gets the value of the minor component of the version number.
        /// </summary>
        public int Minor { get; private set; }

        /// <summary>
        ///     Gets the value of the revision component of the version number.
        /// </summary>
        public int Revision { get; private set; }

        /// <summary>
        ///     Gets the value of the version control commit id.
        /// </summary>
        public Sha1Hash Commit { get; private set; }

        #region Overrides of Object

        public int CompareTo(TabsterVersion other)
        {
            if (Major != other.Major)
                if (Major > other.Major)
                    return 1;
                else
                    return -1;

            if (Minor != other.Minor)
                if (Minor > other.Minor)
                    return 1;
                else
                    return -1;

            if (Revision != other.Revision)
                if (Revision > other.Revision)
                    return 1;
                else
                    return -1;

            if (Build != other.Build)
                if (Build > other.Build)
                    return 1;
                else
                    return -1;

            return 0;
        }

        public override string ToString()
        {
            return ToString(TabsterVersionFormatFlags.Build);
        }

        public override int GetHashCode()
        {
            var hash = 0;
            hash |= (Major & 0x0000000F) << 28;
            hash |= (Minor & 0x000000FF) << 20;
            hash |= (Build & 0x000000FF) << 12;
            hash |= (Revision & 0x00000FFF);
            return hash;
        }

        public override bool Equals(object obj)
        {
            var v = obj as TabsterVersion;
            if (v == null)
                return false;
            return (Major == v.Major) && (Minor == v.Minor) && (Build == v.Build) && (Revision == v.Revision);
        }

        #endregion

        public int CompareTo(Object version)
        {
            if (version == null)
                return 1;

            var v = version as TabsterVersion;
            if (v == null)
                throw new ArgumentException("version");

            return CompareTo(v);
        }

        public bool Equals(TabsterVersion obj)
        {
            if (obj == null)
                return false;
            return (Major == obj.Major) && (Minor == obj.Minor) && (Build == obj.Build) && (Revision == obj.Revision);
        }

        /// <summary>
        ///     Determines whether two specified TabsterVersion objects are equal.
        /// </summary>
        /// <param name="v1">The first TabsterVersion object.</param>
        /// <param name="v2">The second TabsterVersion object.</param>
        /// <returns>true if v1 equals v2; otherwise, false.</returns>
        public static bool operator ==(TabsterVersion v1, TabsterVersion v2)
        {
            return ReferenceEquals(v1, null) ? ReferenceEquals(v2, null) : v1.Equals(v2);
        }

        /// <summary>
        ///     Determines whether two specified TabsterVersion objects are not equal.
        /// </summary>
        /// <param name="v1">The first TabsterVersion object.</param>
        /// <param name="v2">The second TabsterVersion object.</param>
        /// <returns>true if v1 does not equal v2; otherwise, false.</returns>
        public static bool operator !=(TabsterVersion v1, TabsterVersion v2)
        {
            return !(v1 == v2);
        }

        /// <summary>
        ///     Determines whether the first TabsterVersion object is less than the second TabsterVersion object.
        /// </summary>
        /// <param name="v1">The first TabsterVersion object.</param>
        /// <param name="v2">The second TabsterVersion object.</param>
        /// <returns>true if v1 is less than v2; otherwise, false.</returns>
        public static bool operator <(TabsterVersion v1, TabsterVersion v2)
        {
            if ((Object) v1 == null)
                throw new ArgumentNullException("v1");
            return (v1.CompareTo(v2) < 0);
        }

        /// <summary>
        ///     Determines whether the first TabsterVersion object is less than or equal to the second TabsterVersion object.
        /// </summary>
        /// <param name="v1">The first TabsterVersion object.</param>
        /// <param name="v2">The second TabsterVersion object.</param>
        /// <returns>true if v1 is less than or equal to v2; otherwise, false.</returns>
        public static bool operator <=(TabsterVersion v1, TabsterVersion v2)
        {
            if ((Object) v1 == null)
                throw new ArgumentNullException("v1");
            return (v1.CompareTo(v2) <= 0);
        }

        /// <summary>
        ///     Determines whether the first TabsterVersion object is greater than the second TabsterVersion object.
        /// </summary>
        /// <param name="v1">The first TabsterVersion object.</param>
        /// <param name="v2">The second TabsterVersion object.</param>
        /// <returns>true if v1 is greater than v2; otherwise, false.</returns>
        public static bool operator >(TabsterVersion v1, TabsterVersion v2)
        {
            return (v2 < v1);
        }

        /// <summary>
        ///     Determines whether the first TabsterVersion object is greater than or equal to the second TabsterVersion object.
        /// </summary>
        /// <param name="v1">The first TabsterVersion object.</param>
        /// <param name="v2">The second TabsterVersion object.</param>
        /// <returns>true if v1 is greater than or equal to v2; otherwise, false.</returns>
        public static bool operator >=(TabsterVersion v1, TabsterVersion v2)
        {
            return (v2 <= v1);
        }

        /// <summary>
        ///     Returns the version string using the specified format flags.
        /// </summary>
        /// <param name="flags">Formatting option flags.</param>
        public string ToString(TabsterVersionFormatFlags flags)
        {
            var baseStr = string.Format("{0}.{1}.{2}", Major, Minor, Revision);

            // include build component if it's not to be appended
            if ((flags & TabsterVersionFormatFlags.Build) == TabsterVersionFormatFlags.Build)
                baseStr += string.Format(".{0}", Build);

            // truncate version string
            if ((flags & TabsterVersionFormatFlags.Truncated) == TabsterVersionFormatFlags.Truncated)
            {
                while (baseStr.EndsWith("0") || baseStr.EndsWith("."))
                    baseStr = baseStr.Remove(baseStr.Length - 1, 1);

                if (!baseStr.Contains("."))
                    baseStr = string.Format("{0}.0", baseStr);
            }

            if ((flags & TabsterVersionFormatFlags.BuildString) == TabsterVersionFormatFlags.BuildString && Build > 0)
                baseStr += string.Format(" (Build {0})", Build);

            if (Commit != null)
            {
                if ((flags & TabsterVersionFormatFlags.CommitFull) == TabsterVersionFormatFlags.CommitFull)
                    baseStr += string.Format(" {0}", Commit);
                else if ((flags & TabsterVersionFormatFlags.CommitShort) == TabsterVersionFormatFlags.CommitShort)
                    baseStr += string.Format(" {0}", Commit.ToShorthandString());
            }
            return baseStr;
        }

        /// <summary>
        ///     Represents a Sha-1 hash.
        /// </summary>
        public class Sha1Hash
        {
            private const int MinHashLength = 7;
            private const int MaxHashLength = 40;
            private readonly string _hash;

            internal Sha1Hash(string hash)
            {
                if (hash == null)
                    throw new ArgumentNullException("hash");

                if (hash.Length != MinHashLength && hash.Length != MaxHashLength)
                    throw new ArgumentException(string.Format("Hash must be between {0} and {1} characters long.", MinHashLength, MaxHashLength));

                _hash = hash;
            }

            #region Overrides of Object

            /// <summary>
            ///     Returns the hash string.
            /// </summary>
            public override string ToString()
            {
                return _hash;
            }

            #endregion

            /// <summary>
            ///     Returns a shorthanded version of the hash.
            /// </summary>
            /// <returns></returns>
            public string ToShorthandString()
            {
                return _hash.Substring(0, MinHashLength);
            }

            /// <summary>
            ///     Returns whether the hash uses the shorthand format.
            /// </summary>
            /// <returns>true if the has is shorthanded; otherwise, false.</returns>
            public bool IsShortHand()
            {
                return _hash.Length == MinHashLength;
            }

            internal static bool TryParseHash(string str, out Sha1Hash hash)
            {
                if (str.Length == MinHashLength || str.Length == MaxHashLength)
                {
                    hash = new Sha1Hash(str);
                    return true;
                }

                hash = null;
                return false;
            }
        }
    }
}