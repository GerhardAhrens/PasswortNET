//-----------------------------------------------------------------------
// <copyright file="SyncItemStatus.cs" company="Lifeprojects.de">
//     Class: SyncItemStatus
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>18.09.2022 09:46:19</date>
//
// <summary>
// Enum Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core.Enums
{
    using System;

    public enum SyncItemStatus : int
    {
        None = 0,
        Add = 1,
        Update = 2,
        Delete = 3,
    }
}
