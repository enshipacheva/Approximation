using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApproximationAlgorithm
{
    class Node
    {
        public Node(double X, double Y)
        {
            this.x = X;
            this.y = Y;
        }
        private double x;
        private double y;

        public double X
        {
            get
            {
                return this.x;
            }
        }

        public double Y
        {
            get
            {
                return this.y;
            }
        }

        private int ind;
        public int Index
        {
            get
            {
                return this.ind;
            }
            set
            {
                this.ind = value;
            }
        }





    }
}
