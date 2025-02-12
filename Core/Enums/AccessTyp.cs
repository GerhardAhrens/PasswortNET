//-----------------------------------------------------------------------
// <copyright file="AccessTyp.cs" company="Lifeprojects.de">
//     Class: AccessTyp
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>24.04.2022</date>
//
// <summary>
// Enum Klasse für (Passwort Art) AccessTyp
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core
{
    using System;

    public enum AccessTyp : int
    {
        None = 0,
        Pin = 1,
        Website = 2,
        Passwort = 3,
        License = 4,
        All = 99
    }
}
