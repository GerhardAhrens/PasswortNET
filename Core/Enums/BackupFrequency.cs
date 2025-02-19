/*
 * <copyright file="BackupFrequency.cs" company="Lifeprojects.de">
 *     Class: BackupFrequency
 *     Copyright © Lifeprojects.de 2025
 * </copyright>
 *
 * <author>Gerhard Ahrens - Lifeprojects.de</author>
 * <email>gerhard.ahrens@lifeprojects.de</email>
 * <date>18.02.2025 20:20:12</date>
 * <Project>EasyPrototypingNET</Project>
 *
 * <summary>
 * Beschreibung zur Klasse
 * </summary>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by the Free Software Foundation, 
 * either version 3 of the License, or (at your option) any later version.
 * This program is distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY; 
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.You should have received a copy of the GNU General Public License along with this program. 
 * If not, see <http://www.gnu.org/licenses/>.
*/

namespace PasswortNET.Core
{
    using System;
    using System.ComponentModel;

    public enum BackupFrequency : int
    {
        [Description("Keine Auswahl")]
        None = 0,
        [Description("Einmal Täglich")]
        OnceDaily = 1,
        [Description("Bei jedem Login")]
        WithEveryLogin = 2,
    }
}
