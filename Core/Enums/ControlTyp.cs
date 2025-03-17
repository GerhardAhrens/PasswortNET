//-----------------------------------------------------------------------
// <copyright file="ControlTyp.cs" company="Lifeprojects.de">
//     Class: ControlTyp
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>24.04.2022</date>
//
// <summary>
// Enum Klasse für Dialog Controls (Ein- oder Ausblenden)
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core.Enums
{
    using System;

    public enum ControlTyp : int
    {
        None = 0,
        WebsiteTxt = 1,
        PinTxt = 2,
        UsernameTxt = 3,
        PasswortTxt = 4,
    }
}
