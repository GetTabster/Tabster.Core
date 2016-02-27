#region

using System;

#endregion

namespace Tabster.Core.Types
{
    /// <summary>
    ///     Represents the version number.
    /// </summary>
    public class TabsterVersion : IComparable, IComparable<TabsterVersion>, IEquatable<TabsterVersion>
    {
        /// <summary>
        ///     Initializes a new TabsterVersion.
        /// </summary>
        public TabsterVersion()
        {
        }

        /// <summary>
        /// Initializes a new TabsterVersion based off of the specified components.
        /// </summary>
        /// <param name="major">Major component.</param>
        /// <param name="minor">Minor component.</param>
        /// <param name="revision">Revision component.</param>
        /// <param name="build">Build component.</param>
        /// <param name="hash">Hash component.</param>
        public TabsterVersion(int major, int minor, int revision, int build, string hash = null)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
            Build = build;
            Hash = hash;
        }

        /// <summary>
        ///     Initializes a new TabsterVersion based off of a System.Version object.
        /// </summary>
        /// <param name="version">The System.Version object.</param>
        /// <param name="hash"></param>
        public TabsterVersion(Version version, string hash = null)
        {
            Major = version.Major;
            Minor = version.Minor;
            Revision = version.Build;
            Build = version.Revision;
            Hash = hash;
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
        ///     Gets the value of the version control hash.
        /// </summary>
        public string Hash { get; private set; }

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

            if (Build != other.Build)
                if (Build > other.Build)
                    return 1;
                else
                    return -1;

            if (Revision != other.Revision)
                if (Revision > other.Revision)
                    return 1;
                else
                    return -1;

            return 0;
        }

        public override string ToString()
        {
            var str = string.Format("{0}.{1}.{2}.{3}", Major, Minor, Revision, Build);

            if (!string.IsNullOrEmpty(Hash))
                str += string.Format("-{0}", Hash);

            return str;
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
    }
}