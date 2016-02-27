#region

using System;
using Tabster.Core.Types;

#endregion

namespace Tabster.Core.Plugins
{
    /// <summary>
    ///     Represents a third-party plugin to be loaded.
    /// </summary>
    public interface ITabsterPlugin
    {
        /// <summary>
        ///     Plugin author.
        /// </summary>
        string Author { get; }

        /// <summary>
        ///     Plugin copyright.
        /// </summary>
        string Copyright { get; }

        /// <summary>
        ///     Plugin description.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Plugin display name.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        ///     Plugin version.
        /// </summary>
        TabsterVersion Version { get; }

        /// <summary>
        ///     Plugin website.
        /// </summary>
        Uri Website { get; }

        /// <summary>
        ///     Plugin guid.
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        ///     Plugin activation.
        /// </summary>
        void Activate();

        /// <summary>
        ///     Plugin deactivation.
        /// </summary>
        void Deactivate();

        /// <summary>
        ///     Plugin initialization.
        /// </summary>
        void Initialize();
    }
}