using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MillingMachineSimulator
{
    [DataContract]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionColorNormal : IVertexType
    {
        [DataMember]
        public Vector3 Position;
        [DataMember]
        public Color Color;
        [DataMember]
        public Vector3 Normal;

        private static readonly VertexElement[] elements = new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float)*3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        };
        public static VertexDeclaration VertexDeclaration = new VertexDeclaration(elements);
        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

        public VertexPositionColorNormal(Vector3 position, Color color, Vector3 normal)
        {
            this.Position = position;
            this.Color = color;
            this.Normal = normal;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != base.GetType())
                return false;
            return this == (VertexPositionColorNormal)obj;
        }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return string.Format("[{0}] [{1}] [{2}]", Position.ToString(), Color.ToString(), Normal.ToString()); }

        public static bool operator ==(VertexPositionColorNormal left, VertexPositionColorNormal right) {
            return (left.Position == right.Position && left.Color == right.Color && left.Normal == right.Normal);
        }
        public static bool operator !=(VertexPositionColorNormal left, VertexPositionColorNormal right) { return !(left == right); }
    }
}
