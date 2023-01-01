using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollapseShrinkDemo.Board
{
    public class Tile : MonoBehaviour
    {
        [HideInInspector] public int x, y;

        public void Init(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}