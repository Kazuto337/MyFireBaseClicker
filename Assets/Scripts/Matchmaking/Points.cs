using System;
using UnityEngine;

namespace Serializables
{
    [Serializable]
    public class Points
    {
        public int points;

        public Points(int points) => this.points = points;
    }
}