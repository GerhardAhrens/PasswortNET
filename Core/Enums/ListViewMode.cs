//-----------------------------------------------------------------------
// <copyright file="ListViewMode.cs" company="Lifeprojects.de">
//     Class: ListViewMode
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>Gerhard Ahrens@Lifeprojects.de</email>
// <date>28.03.2025 14:07:52</date>
//
// <summary>
// Enum Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core.Enums
{
    using System;
    using System.ComponentModel;

    public enum ListViewMode : int
    {
        [Description("Defaultdarstellung - TileView")]
        None = 0,
        [Description("TileView - Darstellung als Kacheln")]
        TileView = 1,
        [Description("GridView - Darstellung als Liste")]
        GridView = 2,
    }
}
