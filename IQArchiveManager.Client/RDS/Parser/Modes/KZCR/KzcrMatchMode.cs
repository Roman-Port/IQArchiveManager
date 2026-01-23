using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client.RDS.Parser.Modes.KZCR
{
    enum KzcrMatchMode
    {
        NONE,
        PS_SCROLL_TILDE, // Tilde separated scrolling PS with RT "REAL ROCK Z103"; Very old <2023
        PS_SCROLL_NEW, // Sensibily formatted scrolling PS with RT "REAL ROCK Z103"; 2023-2025
        RT_SPONSORED // RT with sponsor inserts
    }
}
