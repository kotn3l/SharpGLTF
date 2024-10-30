using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;

using SharpGLTF.Memory;

using ENCODING = SharpGLTF.Schema2.EncodingType;

namespace SharpGLTF.Geometry.VertexTypes
{
    /// <summary>
    /// Represents the interface that must be implemented by a material vertex fragment.
    /// </summary>
    /// <remarks>
    /// Implemented by:
    /// <list type="table">
    /// <item><see cref="VertexEmpty"/></item>
    /// <item><see cref="VertexColor1"/></item>
    /// <item><see cref="VertexColor2"/></item>
    /// <item><see cref="VertexTexture1"/></item>
    /// <item><see cref="VertexTexture2"/></item>
    /// <item><see cref="VertexColor1Texture1"/></item>
    /// <item><see cref="VertexColor1Texture2"/></item>
    /// <item><see cref="VertexColor2Texture1"/></item>
    /// <item><see cref="VertexColor2Texture2"/></item>
    /// <item>And also by other custom vertex material fragment types.</item>
    /// </list>
    /// </remarks>
    public interface IVertexMaterial : IVertexReflection
    {
        /// <summary>
        /// Gets the number of color attributes available in this vertex
        /// </summary>
        int MaxColors { get; }

        /// <summary>
        /// Gets the number of texture coordinate attributes available in this vertex
        /// </summary>
        int MaxTextCoords { get; }

        /// <summary>
        /// Gets a color attribute.
        /// </summary>
        /// <param name="index">An index from 0 to <see cref="MaxColors"/>.</param>
        /// <returns>A <see cref="Vector4"/> value in the range of 0 to 1</returns>
        Vector4 GetColor(int index);

        /// <summary>
        /// Gets a UV texture coordinate attribute.
        /// </summary>
        /// <param name="index">An index from 0 to <see cref="MaxTextCoords"/>.</param>
        /// <returns>A <see cref="Vector2"/> UV texture coordinate.</returns>
        Vector2 GetTexCoord(int index);

        /// <summary>
        /// Sets a color attribute.
        /// <para><b>⚠️ USE ONLY ON UNBOXED VALUES ⚠️</b></para>
        /// </summary>
        /// <param name="setIndex">An index from 0 to <see cref="MaxColors"/>.</param>
        /// <param name="color">A <see cref="Vector4"/> value in the range of 0 to 1</param>
        void SetColor(int setIndex, Vector4 color);

        /// <summary>
        /// Sets a UV texture coordinate attribute.
        /// <para><b>⚠️ USE ONLY ON UNBOXED VALUES ⚠️</b></para>
        /// </summary>
        /// <param name="setIndex">An index from 0 to <see cref="MaxTextCoords"/>.</param>
        /// <param name="coord">A <see cref="Vector2"/> UV texture coordinate.</param>
        void SetTexCoord(int setIndex, Vector2 coord);

        /// <summary>
        /// calculates the difference between this vertex and <paramref name="baseValue"/>
        /// </summary>
        /// <param name="baseValue">The other vertex.</param>
        /// <returns>The <see cref="VertexMaterialDelta"/> value to subtract.</returns>
        VertexMaterialDelta Subtract(IVertexMaterial baseValue);

        /// <summary>
        /// Adds a vertex delta to this value.
        /// <para><b>⚠️ USE ONLY ON UNBOXED VALUES ⚠️</b></para>
        /// </summary>
        /// <param name="delta">The <see cref="VertexMaterialDelta"/> value to add.</param>
        void Add(in VertexMaterialDelta delta);
    }


    public partial struct VertexColor1
    {
        public static implicit operator VertexColor1(Vector4 color)
        {
            return new VertexColor1(color);
        }
    }

    public partial struct VertexColor2
    {
        public static implicit operator VertexColor2((Vector4 Color0, Vector4 Color1) tuple)
        {
            return new VertexColor2(tuple.Color0, tuple.Color1);
        }
    }

    
    public partial struct VertexTexture1
    {
        public static implicit operator VertexTexture1(Vector2 uv)
        {
            return new VertexTexture1(uv);
        }
    }

    
    public partial struct VertexTexture2
    {
        public static implicit operator VertexTexture2((Vector2 Tex0, Vector2 Tex1) tuple)
        {
            return new VertexTexture2(tuple.Tex0, tuple.Tex1);
        }
    }

    
    public partial struct VertexColor1Texture1
    {
        public static implicit operator VertexColor1Texture1((Vector4 Color, Vector2 Tex) tuple)
        {
            return new VertexColor1Texture1(tuple.Color, tuple.Tex);
        }
    }

    
    public partial struct VertexColor1Texture2
    {
        public static implicit operator VertexColor1Texture2((Vector4 Color, Vector2 Tex0, Vector2 Tex1) tuple)
        {
            return new VertexColor1Texture2(tuple.Color, tuple.Tex0, tuple.Tex1);
        }
    }

    
    public partial struct VertexColor2Texture1
    {
        public static implicit operator VertexColor2Texture1((Vector4 Color0, Vector4 Color1, Vector2 Tex) tuple)
        {
            return new VertexColor2Texture1(tuple.Color0, tuple.Color1, tuple.Tex);
        }
    }

    
    public partial struct VertexColor2Texture2
    {
        public static implicit operator VertexColor2Texture2((Vector4 Color0, Vector4 Color1, Vector2 Tex0, Vector2 Tex1) tuple)
        {
            return new VertexColor2Texture2(tuple.Color0, tuple.Color1, tuple.Tex0, tuple.Tex1);
        }
    }

    /// <summary>
    /// Defines a Vertex attribute with two material Colors and two Texture Coordinates.
    /// </summary>    
    [System.Diagnostics.DebuggerDisplay("{_GetDebuggerDisplay(),nq}")]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct VertexMaterialDelta : IVertexMaterial, IEquatable<VertexMaterialDelta>
    {
        #region debug

        //private readonly string _GetDebuggerDisplay() => $"ΔC₀:{Color0Delta} ΔC₁:{Color1Delta} ΔUV₀:{TexCoord0Delta}  ΔUV₁:{TexCoord1Delta}";

        #endregion

        #region constructors

        public static implicit operator VertexMaterialDelta(in (Vector4 Color0Delta, Vector4 Color1Delta, Vector2 TextCoord0Delta, Vector2 TextCoord1Delta) tuple)
        {
            return new VertexMaterialDelta(tuple.Color0Delta, tuple.Color1Delta, tuple.TextCoord0Delta, tuple.TextCoord1Delta, Vector2.Zero, Vector2.Zero);
        }

        public static implicit operator VertexMaterialDelta(in (Vector4 Color0Delta, Vector4 Color1Delta, Vector2 TextCoord0Delta, Vector2 TextCoord1Delta, Vector2 TextCoord2Delta, Vector2 TextCoord3Delta) tuple)
        {
            return new VertexMaterialDelta(tuple.Color0Delta, tuple.Color1Delta, tuple.TextCoord0Delta, tuple.TextCoord1Delta, tuple.TextCoord2Delta, tuple.TextCoord3Delta);
        }

        public VertexMaterialDelta(IVertexMaterial src) : this(src.MaxColors, src.MaxTextCoords)
        {
            Guard.NotNull(src, nameof(src));
            for (int i = 0; i < src.MaxColors; i++)
            {
                ColorDeltas[i] = src.GetColor(i);
            }
            for (int i = 0; i < src.MaxTextCoords; i++)
            {
                TexCoordDeltas[i] = src.GetTexCoord(i);
            }
        }
        
        public VertexMaterialDelta(in Vector4 color0Delta, in Vector4 color1Delta, in Vector2 texCoord0Delta, in Vector2 texCoord1Delta) : this(2,4)
        {
            ColorDeltas[0] = color0Delta;
            ColorDeltas[1] = color1Delta;
            TexCoordDeltas[0] = texCoord0Delta;
            TexCoordDeltas[1] = texCoord1Delta;
        }

        public VertexMaterialDelta(in Vector4 color0Delta, in Vector4 color1Delta, in Vector2 texCoord0Delta, in Vector2 texCoord1Delta, in Vector2 texCoord2Delta, in Vector2 texCoord3Delta) : this(2,4)
        {
            ColorDeltas[0] = color0Delta;
            ColorDeltas[1] = color1Delta;
            TexCoordDeltas[0] = texCoord0Delta;
            TexCoordDeltas[1] = texCoord1Delta;
            TexCoordDeltas[2] = texCoord2Delta;
            TexCoordDeltas[3] = texCoord3Delta;
        }
        
        internal VertexMaterialDelta(in VertexMaterialDelta rootVal, in VertexMaterialDelta morphVal): this(Math.Max(rootVal.MaxColors, morphVal.MaxColors),
                                                                                                                         Math.Max(rootVal.MaxTextCoords, morphVal.MaxTextCoords))
        {
            if (rootVal.MaxColors != morphVal.MaxColors) throw new ArgumentException("MaxColors do not match!");
            if (rootVal.MaxTextCoords != morphVal.MaxTextCoords) throw new ArgumentException("MaxTextCoords do not match!");

            for (int i = 0; i < Math.Min(rootVal.MaxColors, morphVal.MaxColors); i++)
            {
                ColorDeltas[i] = morphVal.ColorDeltas[i] - rootVal.ColorDeltas[i];
            }
            for (int i = 0; i < Math.Min(rootVal.MaxTextCoords, morphVal.MaxTextCoords); i++)
            {
                TexCoordDeltas[i] = morphVal.TexCoordDeltas[i] - rootVal.TexCoordDeltas[i];
            }
        }

        #endregion

        #region data

        public static VertexMaterialDelta Zero => new VertexMaterialDelta(Vector4.Zero, Vector4.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero);
                
        public Vector4[] ColorDeltas;        
        public Vector2[] TexCoordDeltas;

        internal VertexMaterialDelta(int maxcolors, int maxtexcoord)
        {
            MaxColors = maxcolors;
            MaxTextCoords = maxtexcoord;

            ColorDeltas = new Vector4[maxcolors];
            TexCoordDeltas = new Vector2[maxtexcoord];
        }

        IEnumerable<KeyValuePair<string, AttributeFormat>> IVertexReflection.GetEncodingAttributes()
        {
            for (int i = 0; i < ColorDeltas.Length; i++)
            {
                yield return new KeyValuePair<string, AttributeFormat>($"COLOR_{i}DELTA", new AttributeFormat(Schema2.DimensionType.VEC4, ENCODING.UNSIGNED_BYTE, true));
            }
            for (int i = 0; i < TexCoordDeltas.Length; i++)
            {
                yield return new KeyValuePair<string, AttributeFormat>($"TEXCOORD_{i}DELTA", new AttributeFormat(Schema2.DimensionType.VEC2));
            }
        }

        /// <inheritdoc/>
        public int MaxColors { get; }

        /// <inheritdoc/>
        public int MaxTextCoords { get; }

        /// <inheritdoc/>
        public readonly override int GetHashCode()
        {
            return HashCode.Combine(ColorDeltas, TexCoordDeltas);
        }

        /// <inheritdoc/>
        public readonly override bool Equals(object obj) { return obj is VertexMaterialDelta other && AreEqual(this, other); }

        /// <inheritdoc/>
        public readonly bool Equals(VertexMaterialDelta other) { return AreEqual(this, other); }
        public static bool operator ==(in VertexMaterialDelta a, in VertexMaterialDelta b) { return AreEqual(a, b); }
        public static bool operator !=(in VertexMaterialDelta a, in VertexMaterialDelta b) { return !AreEqual(a, b); }
        public static bool AreEqual(in VertexMaterialDelta a, in VertexMaterialDelta b)
        {
            return a.ColorDeltas.SequenceEqual(b.ColorDeltas) && a.TexCoordDeltas.SequenceEqual(b.TexCoordDeltas);
        }        

        #endregion

        #region API

        /// <inheritdoc/>
        public readonly VertexMaterialDelta Subtract(IVertexMaterial baseValue)
        {
            return new VertexMaterialDelta((VertexMaterialDelta)baseValue, this);
        }

        /// <inheritdoc/>
        public void Add(in VertexMaterialDelta delta)
        {
            for (int i = 0; i < ColorDeltas.Length; i++)
            {
                ColorDeltas[i] += delta.ColorDeltas[i];
            }
            for (int i = 0; i < TexCoordDeltas.Length; i++)
            {
                TexCoordDeltas[i] += delta.TexCoordDeltas[i];
            }
        }

        void IVertexMaterial.SetColor(int setIndex, Vector4 color)
        {
            SetColor(setIndex, color);
        }

        void SetColor(int setIndex, Vector4 color)
        {
            this.ColorDeltas[setIndex] = color;
        }

        void IVertexMaterial.SetTexCoord(int setIndex, Vector2 coord)
        {
            SetTexCoord(setIndex, coord);
        }

        void SetTexCoord(int setIndex, Vector2 coord)
        {
            this.TexCoordDeltas[setIndex] = coord;
        }

        /// <inheritdoc/>
        public readonly Vector4 GetColor(int index)
        {
            return this.ColorDeltas[index];
        }

        /// <inheritdoc/>
        public readonly Vector2 GetTexCoord(int index)
        {
            return this.TexCoordDeltas[index];
        }

        #endregion
    }
}
