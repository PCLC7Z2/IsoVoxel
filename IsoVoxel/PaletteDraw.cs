﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

namespace IsoVoxel
{
    enum Outlining
    {
        Full, Light, Partial, None
    }
    enum Direction
    {
        SE, SW, NW, NE
    }
    enum OrthoDirection
    {
        S, W, N, E
    }

    public struct MagicaVoxelData
    {
        public byte x;
        public byte y;
        public byte z;
        public byte color;

        public MagicaVoxelData(BinaryReader stream)
        {
            x = stream.ReadByte(); //(byte)(subsample ? stream.ReadByte() / 2 : stream.ReadByte());
            y = stream.ReadByte(); //(byte)(subsample ? stream.ReadByte() / 2 : stream.ReadByte());
            z = stream.ReadByte(); //(byte)(subsample ? stream.ReadByte() / 2 : stream.ReadByte());
            color = stream.ReadByte();
        }
        public MagicaVoxelData(BinaryReader stream, int xOffset, int yOffset, int zOffset)
        {
            x = (byte)(stream.ReadByte() + xOffset); //(byte)(subsample ? stream.ReadByte() / 2 : stream.ReadByte());
            y = (byte)(stream.ReadByte() + yOffset); //(byte)(subsample ? stream.ReadByte() / 2 : stream.ReadByte());
            z = (byte)(stream.ReadByte() + zOffset); //(byte)(subsample ? stream.ReadByte() / 2 : stream.ReadByte());
            color = stream.ReadByte();
        }
        public MagicaVoxelData(int x, int y, int z, int color)
        {
            this.x = (byte)x;
            this.y = (byte)y;
            this.z = (byte)z;
            this.color = (byte)color;
        }
    }
    class PaletteDraw
    {
        public static float[][] colors = new float[][]
        {
            new float[]{1.0F, 1.0F, 1.0F, 1.0F},
            new float[]{1.0F, 1.0F, 0.8F, 1.0F},
            new float[]{1.0F, 1.0F, 0.6F, 1.0F},
            new float[]{1.0F, 1.0F, 0.4F, 1.0F},
            new float[]{1.0F, 1.0F, 0.2F, 1.0F},
            new float[]{1.0F, 1.0F, 0.0F, 1.0F},
            new float[]{1.0F, 0.8F, 1.0F, 1.0F},
            new float[]{1.0F, 0.8F, 0.8F, 1.0F},
            new float[]{1.0F, 0.8F, 0.6F, 1.0F},
            new float[]{1.0F, 0.8F, 0.4F, 1.0F},
            new float[]{1.0F, 0.8F, 0.2F, 1.0F},
            new float[]{1.0F, 0.8F, 0.0F, 1.0F},
            new float[]{1.0F, 0.6F, 1.0F, 1.0F},
            new float[]{1.0F, 0.6F, 0.8F, 1.0F},
            new float[]{1.0F, 0.6F, 0.6F, 1.0F},
            new float[]{1.0F, 0.6F, 0.4F, 1.0F},
            new float[]{1.0F, 0.6F, 0.2F, 1.0F},
            new float[]{1.0F, 0.6F, 0.0F, 1.0F},
            new float[]{1.0F, 0.4F, 1.0F, 1.0F},
            new float[]{1.0F, 0.4F, 0.8F, 1.0F},
            new float[]{1.0F, 0.4F, 0.6F, 1.0F},
            new float[]{1.0F, 0.4F, 0.4F, 1.0F},
            new float[]{1.0F, 0.4F, 0.2F, 1.0F},
            new float[]{1.0F, 0.4F, 0.0F, 1.0F},
            new float[]{1.0F, 0.2F, 1.0F, 1.0F},
            new float[]{1.0F, 0.2F, 0.8F, 1.0F},
            new float[]{1.0F, 0.2F, 0.6F, 1.0F},
            new float[]{1.0F, 0.2F, 0.4F, 1.0F},
            new float[]{1.0F, 0.2F, 0.2F, 1.0F},
            new float[]{1.0F, 0.2F, 0.0F, 1.0F},
            new float[]{1.0F, 0.0F, 1.0F, 1.0F},
            new float[]{1.0F, 0.0F, 0.8F, 1.0F},
            new float[]{1.0F, 0.0F, 0.6F, 1.0F},
            new float[]{1.0F, 0.0F, 0.4F, 1.0F},
            new float[]{1.0F, 0.0F, 0.2F, 1.0F},
            new float[]{1.0F, 0.0F, 0.0F, 1.0F},
            new float[]{0.8F, 1.0F, 1.0F, 1.0F},
            new float[]{0.8F, 1.0F, 0.8F, 1.0F},
            new float[]{0.8F, 1.0F, 0.6F, 1.0F},
            new float[]{0.8F, 1.0F, 0.4F, 1.0F},
            new float[]{0.8F, 1.0F, 0.2F, 1.0F},
            new float[]{0.8F, 1.0F, 0.0F, 1.0F},
            new float[]{0.8F, 0.8F, 1.0F, 1.0F},
            new float[]{0.8F, 0.8F, 0.8F, 1.0F},
            new float[]{0.8F, 0.8F, 0.6F, 1.0F},
            new float[]{0.8F, 0.8F, 0.4F, 1.0F},
            new float[]{0.8F, 0.8F, 0.2F, 1.0F},
            new float[]{0.8F, 0.8F, 0.0F, 1.0F},
            new float[]{0.8F, 0.6F, 1.0F, 1.0F},
            new float[]{0.8F, 0.6F, 0.8F, 1.0F},
            new float[]{0.8F, 0.6F, 0.6F, 1.0F},
            new float[]{0.8F, 0.6F, 0.4F, 1.0F},
            new float[]{0.8F, 0.6F, 0.2F, 1.0F},
            new float[]{0.8F, 0.6F, 0.0F, 1.0F},
            new float[]{0.8F, 0.4F, 1.0F, 1.0F},
            new float[]{0.8F, 0.4F, 0.8F, 1.0F},
            new float[]{0.8F, 0.4F, 0.6F, 1.0F},
            new float[]{0.8F, 0.4F, 0.4F, 1.0F},
            new float[]{0.8F, 0.4F, 0.2F, 1.0F},
            new float[]{0.8F, 0.4F, 0.0F, 1.0F},
            new float[]{0.8F, 0.2F, 1.0F, 1.0F},
            new float[]{0.8F, 0.2F, 0.8F, 1.0F},
            new float[]{0.8F, 0.2F, 0.6F, 1.0F},
            new float[]{0.8F, 0.2F, 0.4F, 1.0F},
            new float[]{0.8F, 0.2F, 0.2F, 1.0F},
            new float[]{0.8F, 0.2F, 0.0F, 1.0F},
            new float[]{0.8F, 0.0F, 1.0F, 1.0F},
            new float[]{0.8F, 0.0F, 0.8F, 1.0F},
            new float[]{0.8F, 0.0F, 0.6F, 1.0F},
            new float[]{0.8F, 0.0F, 0.4F, 1.0F},
            new float[]{0.8F, 0.0F, 0.2F, 1.0F},
            new float[]{0.8F, 0.0F, 0.0F, 1.0F},
            new float[]{0.6F, 1.0F, 1.0F, 1.0F},
            new float[]{0.6F, 1.0F, 0.8F, 1.0F},
            new float[]{0.6F, 1.0F, 0.6F, 1.0F},
            new float[]{0.6F, 1.0F, 0.4F, 1.0F},
            new float[]{0.6F, 1.0F, 0.2F, 1.0F},
            new float[]{0.6F, 1.0F, 0.0F, 1.0F},
            new float[]{0.6F, 0.8F, 1.0F, 1.0F},
            new float[]{0.6F, 0.8F, 0.8F, 1.0F},
            new float[]{0.6F, 0.8F, 0.6F, 1.0F},
            new float[]{0.6F, 0.8F, 0.4F, 1.0F},
            new float[]{0.6F, 0.8F, 0.2F, 1.0F},
            new float[]{0.6F, 0.8F, 0.0F, 1.0F},
            new float[]{0.6F, 0.6F, 1.0F, 1.0F},
            new float[]{0.6F, 0.6F, 0.8F, 1.0F},
            new float[]{0.6F, 0.6F, 0.6F, 1.0F},
            new float[]{0.6F, 0.6F, 0.4F, 1.0F},
            new float[]{0.6F, 0.6F, 0.2F, 1.0F},
            new float[]{0.6F, 0.6F, 0.0F, 1.0F},
            new float[]{0.6F, 0.4F, 1.0F, 1.0F},
            new float[]{0.6F, 0.4F, 0.8F, 1.0F},
            new float[]{0.6F, 0.4F, 0.6F, 1.0F},
            new float[]{0.6F, 0.4F, 0.4F, 1.0F},
            new float[]{0.6F, 0.4F, 0.2F, 1.0F},
            new float[]{0.6F, 0.4F, 0.0F, 1.0F},
            new float[]{0.6F, 0.2F, 1.0F, 1.0F},
            new float[]{0.6F, 0.2F, 0.8F, 1.0F},
            new float[]{0.6F, 0.2F, 0.6F, 1.0F},
            new float[]{0.6F, 0.2F, 0.4F, 1.0F},
            new float[]{0.6F, 0.2F, 0.2F, 1.0F},
            new float[]{0.6F, 0.2F, 0.0F, 1.0F},
            new float[]{0.6F, 0.0F, 1.0F, 1.0F},
            new float[]{0.6F, 0.0F, 0.8F, 1.0F},
            new float[]{0.6F, 0.0F, 0.6F, 1.0F},
            new float[]{0.6F, 0.0F, 0.4F, 1.0F},
            new float[]{0.6F, 0.0F, 0.2F, 1.0F},
            new float[]{0.6F, 0.0F, 0.0F, 1.0F},
            new float[]{0.4F, 1.0F, 1.0F, 1.0F},
            new float[]{0.4F, 1.0F, 0.8F, 1.0F},
            new float[]{0.4F, 1.0F, 0.6F, 1.0F},
            new float[]{0.4F, 1.0F, 0.4F, 1.0F},
            new float[]{0.4F, 1.0F, 0.2F, 1.0F},
            new float[]{0.4F, 1.0F, 0.0F, 1.0F},
            new float[]{0.4F, 0.8F, 1.0F, 1.0F},
            new float[]{0.4F, 0.8F, 0.8F, 1.0F},
            new float[]{0.4F, 0.8F, 0.6F, 1.0F},
            new float[]{0.4F, 0.8F, 0.4F, 1.0F},
            new float[]{0.4F, 0.8F, 0.2F, 1.0F},
            new float[]{0.4F, 0.8F, 0.0F, 1.0F},
            new float[]{0.4F, 0.6F, 1.0F, 1.0F},
            new float[]{0.4F, 0.6F, 0.8F, 1.0F},
            new float[]{0.4F, 0.6F, 0.6F, 1.0F},
            new float[]{0.4F, 0.6F, 0.4F, 1.0F},
            new float[]{0.4F, 0.6F, 0.2F, 1.0F},
            new float[]{0.4F, 0.6F, 0.0F, 1.0F},
            new float[]{0.4F, 0.4F, 1.0F, 1.0F},
            new float[]{0.4F, 0.4F, 0.8F, 1.0F},
            new float[]{0.4F, 0.4F, 0.6F, 1.0F},
            new float[]{0.4F, 0.4F, 0.4F, 1.0F},
            new float[]{0.4F, 0.4F, 0.2F, 1.0F},
            new float[]{0.4F, 0.4F, 0.0F, 1.0F},
            new float[]{0.4F, 0.2F, 1.0F, 1.0F},
            new float[]{0.4F, 0.2F, 0.8F, 1.0F},
            new float[]{0.4F, 0.2F, 0.6F, 1.0F},
            new float[]{0.4F, 0.2F, 0.4F, 1.0F},
            new float[]{0.4F, 0.2F, 0.2F, 1.0F},
            new float[]{0.4F, 0.2F, 0.0F, 1.0F},
            new float[]{0.4F, 0.0F, 1.0F, 1.0F},
            new float[]{0.4F, 0.0F, 0.8F, 1.0F},
            new float[]{0.4F, 0.0F, 0.6F, 1.0F},
            new float[]{0.4F, 0.0F, 0.4F, 1.0F},
            new float[]{0.4F, 0.0F, 0.2F, 1.0F},
            new float[]{0.4F, 0.0F, 0.0F, 1.0F},
            new float[]{0.2F, 1.0F, 1.0F, 1.0F},
            new float[]{0.2F, 1.0F, 0.8F, 1.0F},
            new float[]{0.2F, 1.0F, 0.6F, 1.0F},
            new float[]{0.2F, 1.0F, 0.4F, 1.0F},
            new float[]{0.2F, 1.0F, 0.2F, 1.0F},
            new float[]{0.2F, 1.0F, 0.0F, 1.0F},
            new float[]{0.2F, 0.8F, 1.0F, 1.0F},
            new float[]{0.2F, 0.8F, 0.8F, 1.0F},
            new float[]{0.2F, 0.8F, 0.6F, 1.0F},
            new float[]{0.2F, 0.8F, 0.4F, 1.0F},
            new float[]{0.2F, 0.8F, 0.2F, 1.0F},
            new float[]{0.2F, 0.8F, 0.0F, 1.0F},
            new float[]{0.2F, 0.6F, 1.0F, 1.0F},
            new float[]{0.2F, 0.6F, 0.8F, 1.0F},
            new float[]{0.2F, 0.6F, 0.6F, 1.0F},
            new float[]{0.2F, 0.6F, 0.4F, 1.0F},
            new float[]{0.2F, 0.6F, 0.2F, 1.0F},
            new float[]{0.2F, 0.6F, 0.0F, 1.0F},
            new float[]{0.2F, 0.4F, 1.0F, 1.0F},
            new float[]{0.2F, 0.4F, 0.8F, 1.0F},
            new float[]{0.2F, 0.4F, 0.6F, 1.0F},
            new float[]{0.2F, 0.4F, 0.4F, 1.0F},
            new float[]{0.2F, 0.4F, 0.2F, 1.0F},
            new float[]{0.2F, 0.4F, 0.0F, 1.0F},
            new float[]{0.2F, 0.2F, 1.0F, 1.0F},
            new float[]{0.2F, 0.2F, 0.8F, 1.0F},
            new float[]{0.2F, 0.2F, 0.6F, 1.0F},
            new float[]{0.2F, 0.2F, 0.4F, 1.0F},
            new float[]{0.2F, 0.2F, 0.2F, 1.0F},
            new float[]{0.2F, 0.2F, 0.0F, 1.0F},
            new float[]{0.2F, 0.0F, 1.0F, 1.0F},
            new float[]{0.2F, 0.0F, 0.8F, 1.0F},
            new float[]{0.2F, 0.0F, 0.6F, 1.0F},
            new float[]{0.2F, 0.0F, 0.4F, 1.0F},
            new float[]{0.2F, 0.0F, 0.2F, 1.0F},
            new float[]{0.2F, 0.0F, 0.0F, 1.0F},
            new float[]{0.0F, 1.0F, 1.0F, 1.0F},
            new float[]{0.0F, 1.0F, 0.8F, 1.0F},
            new float[]{0.0F, 1.0F, 0.6F, 1.0F},
            new float[]{0.0F, 1.0F, 0.4F, 1.0F},
            new float[]{0.0F, 1.0F, 0.2F, 1.0F},
            new float[]{0.0F, 1.0F, 0.0F, 1.0F},
            new float[]{0.0F, 0.8F, 1.0F, 1.0F},
            new float[]{0.0F, 0.8F, 0.8F, 1.0F},
            new float[]{0.0F, 0.8F, 0.6F, 1.0F},
            new float[]{0.0F, 0.8F, 0.4F, 1.0F},
            new float[]{0.0F, 0.8F, 0.2F, 1.0F},
            new float[]{0.0F, 0.8F, 0.0F, 1.0F},
            new float[]{0.0F, 0.6F, 1.0F, 1.0F},
            new float[]{0.0F, 0.6F, 0.8F, 1.0F},
            new float[]{0.0F, 0.6F, 0.6F, 1.0F},
            new float[]{0.0F, 0.6F, 0.4F, 1.0F},
            new float[]{0.0F, 0.6F, 0.2F, 1.0F},
            new float[]{0.0F, 0.6F, 0.0F, 1.0F},
            new float[]{0.0F, 0.4F, 1.0F, 1.0F},
            new float[]{0.0F, 0.4F, 0.8F, 1.0F},
            new float[]{0.0F, 0.4F, 0.6F, 1.0F},
            new float[]{0.0F, 0.4F, 0.4F, 1.0F},
            new float[]{0.0F, 0.4F, 0.2F, 1.0F},
            new float[]{0.0F, 0.4F, 0.0F, 1.0F},
            new float[]{0.0F, 0.2F, 1.0F, 1.0F},
            new float[]{0.0F, 0.2F, 0.8F, 1.0F},
            new float[]{0.0F, 0.2F, 0.6F, 1.0F},
            new float[]{0.0F, 0.2F, 0.4F, 1.0F},
            new float[]{0.0F, 0.2F, 0.2F, 1.0F},
            new float[]{0.0F, 0.2F, 0.0F, 1.0F},
            new float[]{0.0F, 0.0F, 1.0F, 1.0F},
            new float[]{0.0F, 0.0F, 0.8F, 1.0F},
            new float[]{0.0F, 0.0F, 0.6F, 1.0F},
            new float[]{0.0F, 0.0F, 0.4F, 1.0F},
            new float[]{0.0F, 0.0F, 0.2F, 1.0F},
            new float[]{0.9333333333333333F, 0.0F, 0.0F, 1.0F},
            new float[]{0.8666666666666667F, 0.0F, 0.0F, 1.0F},
            new float[]{0.7333333333333333F, 0.0F, 0.0F, 1.0F},
            new float[]{0.6666666666666666F, 0.0F, 0.0F, 1.0F},
            new float[]{0.5333333333333333F, 0.0F, 0.0F, 1.0F},
            new float[]{0.4666666666666667F, 0.0F, 0.0F, 1.0F},
            new float[]{0.3333333333333333F, 0.0F, 0.0F, 1.0F},
            new float[]{0.26666666666666666F, 0.0F, 0.0F, 1.0F},
            new float[]{0.13333333333333333F, 0.0F, 0.0F, 1.0F},
            new float[]{0.06666666666666667F, 0.0F, 0.0F, 1.0F},
            new float[]{0.0F, 0.9333333333333333F, 0.0F, 1.0F},
            new float[]{0.0F, 0.8666666666666667F, 0.0F, 1.0F},
            new float[]{0.0F, 0.7333333333333333F, 0.0F, 1.0F},
            new float[]{0.0F, 0.6666666666666666F, 0.0F, 1.0F},
            new float[]{0.0F, 0.5333333333333333F, 0.0F, 1.0F},
            new float[]{0.0F, 0.4666666666666667F, 0.0F, 1.0F},
            new float[]{0.0F, 0.3333333333333333F, 0.0F, 1.0F},
            new float[]{0.0F, 0.26666666666666666F, 0.0F, 1.0F},
            new float[]{0.0F, 0.13333333333333333F, 0.0F, 1.0F},
            new float[]{0.0F, 0.06666666666666667F, 0.0F, 1.0F},
            new float[]{0.0F, 0.0F, 0.9333333333333333F, 1.0F},
            new float[]{0.0F, 0.0F, 0.8666666666666667F, 1.0F},
            new float[]{0.0F, 0.0F, 0.7333333333333333F, 1.0F},
            new float[]{0.0F, 0.0F, 0.6666666666666666F, 1.0F},
            new float[]{0.0F, 0.0F, 0.5333333333333333F, 1.0F},
            new float[]{0.0F, 0.0F, 0.4666666666666667F, 1.0F},
            new float[]{0.0F, 0.0F, 0.3333333333333333F, 1.0F},
            new float[]{0.0F, 0.0F, 0.26666666666666666F, 1.0F},
            new float[]{0.0F, 0.0F, 0.13333333333333333F, 1.0F},
            new float[]{0.0F, 0.0F, 0.06666666666666667F, 1.0F},
            new float[]{0.9333333333333333F, 0.9333333333333333F, 0.9333333333333333F, 1.0F},
            new float[]{0.8666666666666667F, 0.8666666666666667F, 0.8666666666666667F, 1.0F},
            new float[]{0.7333333333333333F, 0.7333333333333333F, 0.7333333333333333F, 1.0F},
            new float[]{0.6666666666666666F, 0.6666666666666666F, 0.6666666666666666F, 1.0F},
            new float[]{0.5333333333333333F, 0.5333333333333333F, 0.5333333333333333F, 1.0F},
            new float[]{0.4666666666666667F, 0.4666666666666667F, 0.4666666666666667F, 1.0F},
            new float[]{0.3333333333333333F, 0.3333333333333333F, 0.3333333333333333F, 1.0F},
            new float[]{0.26666666666666666F, 0.26666666666666666F, 0.26666666666666666F, 1.0F},
            new float[]{0.13333333333333333F, 0.13333333333333333F, 0.13333333333333333F, 1.0F},
            new float[]{0.06666666666666667F, 0.06666666666666667F, 0.06666666666666667F, 1.0F},
            new float[]{0.0F, 0.0F, 0.0F, 0.0F},
        };

        public static byte[][] byteColors = new byte[256][];
        static PaletteDraw()
        {
            for(int i = 0; i < colors.Length; i++)
            {
                byteColors[i] = new byte[4];
                for(int n = 0; n < 4; n++)
                {
                    byteColors[i][n] = (byte)Math.Round(colors[i][n] * 255);
                }
            }
        }

        public static char SEP = Path.DirectorySeparatorChar;

        public const int
        Cube = 0,
        BrightTop = 1,
        DimTop = 2,
        BrightDim = 3,
        BrightDimTop = 4,
        BrightBottom = 5,
        DimBottom = 6,
        BrightDimBottom = 7,
        BrightBack = 8,
        DimBack = 9,
        BrightTopBack = 10,
        DimTopBack = 11,
        BrightBottomBack = 12,
        DimBottomBack = 13,
        BackBack = 14,
        BackBackTop = 15,
        BackBackBottom = 16,
        RearBrightTop = 17,
        RearDimTop = 18,
        RearBrightBottom = 19,
        RearDimBottom = 20,

        BrightDimTopThick = 21,
        BrightDimBottomThick = 22,
        BrightTopBackThick = 23,
        BrightBottomBackThick = 24,
        DimTopBackThick = 25,
        DimBottomBackThick = 26,
        BackBackTopThick = 27,
        BackBackBottomThick = 28;

        public static Dictionary<Slope, int> slopes = new Dictionary<Slope, int> { { Slope.Cube, Cube },
            { Slope.BrightTop, BrightTop }, { Slope.DimTop, DimTop }, { Slope.BrightDim, BrightDim }, { Slope.BrightDimTop, BrightDimTop }, { Slope.BrightBottom, BrightBottom }, { Slope.DimBottom, DimBottom },
            { Slope.BrightDimBottom, BrightDimBottom }, { Slope.BrightBack, BrightBack }, { Slope.DimBack, DimBack },
            { Slope.BrightTopBack, BrightTopBack }, { Slope.DimTopBack, DimTopBack }, { Slope.BrightBottomBack, BrightBottomBack }, { Slope.DimBottomBack, DimBottomBack }, { Slope.BackBack, BackBack },
            { Slope.BackBackTop, BackBackTop }, { Slope.BackBackBottom, BackBackBottom },
            { Slope.RearBrightTop, RearBrightTop }, { Slope.RearDimTop, RearDimTop }, { Slope.RearBrightBottom, RearBrightBottom }, { Slope.RearDimBottom, RearDimBottom },
            { Slope.BrightDimTopThick, BrightDimTopThick }, { Slope.BrightDimBottomThick, BrightDimBottomThick },
            { Slope.BrightTopBackThick, BrightTopBackThick }, { Slope.BrightBottomBackThick, BrightBottomBackThick },
            { Slope.DimTopBackThick, DimTopBackThick }, { Slope.DimBottomBackThick, DimBottomBackThick },
            { Slope.BackBackTopThick, BackBackTopThick }, { Slope.BackBackBottomThick, BackBackBottomThick } };

        public static int sizex = 0, sizey = 0, sizez = 0;

        public static Bitmap cube, ortho, white, gradient;

        /// <summary>
        /// Load a MagicaVoxel .vox format file into a MagicaVoxelData[] that we use for voxel chunks.
        /// </summary>
        /// <param name="stream">An open BinaryReader stream that is the .vox file.</param>
        /// <returns>The voxel chunk data for the MagicaVoxel .vox file.</returns>
        public static MagicaVoxelData[][] FromMagica(BinaryReader stream, int xSize, int ySize, int zSize)
        {
            // check out http://voxel.codeplex.com/wikipage?title=VOX%20Format&referringTitle=Home for the file format used below

            MagicaVoxelData[][] voxelData = null;

            string magic = new string(stream.ReadChars(4));
            int version = stream.ReadInt32();

            // a MagicaVoxel .vox file starts with a 'magic' 4 character 'VOX ' identifier
            if (magic == "VOX ")
            {
                int frames = 1;
                int currentFrame = 0;
                while (stream.BaseStream.Position < stream.BaseStream.Length)
                {
                    // each chunk has an ID, size and child chunks
                    char[] chunkId = stream.ReadChars(4);
                    int chunkSize = stream.ReadInt32();
                    int childChunks = stream.ReadInt32();
                    string chunkName = new string(chunkId);

                    if (chunkName == "PACK")
                    {
                        frames = stream.ReadInt32();
                        voxelData = new MagicaVoxelData[frames][];
                    }
                    else if (chunkName == "SIZE")
                    {
                        sizex = stream.ReadInt32();
                        if (xSize <= 0) xSize = sizex;
                        sizey = stream.ReadInt32();
                        if (ySize <= 0) ySize = sizey;
                        xSize = ySize = Math.Max(xSize, ySize);
                        //sizex = sizey = Math.Max(sizex, sizey);
                        sizez = stream.ReadInt32();
                        if (zSize <= 0) zSize = sizez;
                        //Console.WriteLine("x is " + sizex + ", y is " + sizey + ", z is " + sizez);
                        stream.ReadBytes(chunkSize - 4 * 3);
                    }
                    else if (chunkName == "XYZI")
                    {
                        // XYZI contains n voxels
                        int numVoxels = stream.ReadInt32();
                        if (voxelData == null) voxelData = new MagicaVoxelData[1][];
                        // each voxel has x, y, z and color index values
                        voxelData[currentFrame] = new MagicaVoxelData[numVoxels];
                        for (int i = 0; i < numVoxels; i++)
                            voxelData[currentFrame][i] = new MagicaVoxelData(stream, xSize - sizex >> 1, ySize - sizey >> 1, zSize - sizez >> 1);
                        currentFrame++;
                    }
                    else if (chunkName == "RGBA")
                    {
                        colors = new float[256][];

                        for(int i = 0; i < 256; i++)
                        {
                            byte r = stream.ReadByte();
                            byte g = stream.ReadByte();
                            byte b = stream.ReadByte();
                            byte a = stream.ReadByte();
                            byteColors[i] = new byte[] { r, g, b, a};
                            colors[i] = new float[] { r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f };
                        }
                    }
                    else stream.ReadBytes(chunkSize);   // read any excess bytes
                }

                if (voxelData.Length == 0) return voxelData; // failed to read any valid voxel data
                /*
                // now push the voxel data into our voxel chunk structure
                for (int i = 0; i < voxelData.Length; i++)
                {
                    // do not store this voxel if it lies out of range of the voxel chunk (32x128x32)
//                    if (voxelData[i].x > 31 || voxelData[i].y > 31 || voxelData[i].z > 127) continue;
                    
                    // use the voxColors array by default, or overrideColor if it is available
//                    int voxel = (voxelData[i].x + voxelData[i].z * 32 + voxelData[i].y * 32 * 128);
                    //data[voxel] = (colors == null ? voxColors[voxelData[i].color - 1] : colors[voxelData[i].color - 1]);
                }*/
            }

            return voxelData;
        }
        static public int colorcount = 254;

        public static byte[][] rendered, renderedOrtho, rendered45;
        public static byte[][][] renderedFace, renderedFaceSmall, renderedFaceOrtho;


        public static double Clamp(double x)
        {
            return Clamp(x, 0.0, 1.0);
        }

        public static double MercifulClamp(double x)
        {
            return Clamp(x, 0.01, 1.0);
        }

        public static double Clamp(double x, double min, double max)
        {
            return Math.Min(Math.Max(min, x), max);
        }

        public static int Clamp(int x)
        {
            return Clamp(x, 0, 255);
        }

        public static int MercifulClamp(int x)
        {
            return Clamp(x, 1, 255);
        }

        public static int Clamp(int x, int min, int max)
        {
            return Math.Min(Math.Max(min, x), max);
        }

        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = MercifulClamp(Convert.ToInt32(value));
            int p = MercifulClamp(Convert.ToInt32(value * (1 - saturation)));
            int q = MercifulClamp(Convert.ToInt32(value * (1 - f * saturation)));
            int t = MercifulClamp(Convert.ToInt32(value * (1 - (1 - f) * saturation)));

            if(hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if(hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if(hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if(hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if(hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }


        private static byte[][] StoreColorCubes()
        {
            colorcount = colors.Length;
            byte[,] cubes = new byte[colorcount, 80];

            Image image = cube;
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = 4;
            int height = 5;
            float[][] colorMatrixElements = {
   new float[] {1F, 0,  0,  0,  0},
   new float[] {0, 1F,  0,  0,  0},
   new float[] {0,  0,  1F, 0,  0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0,  0,  0,  0, 1F}};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);

            for(int current_color = 0; current_color < colorcount; current_color++)
            {
                Bitmap b =
                new Bitmap(width, height, PixelFormat.Format32bppArgb);

                Graphics g = Graphics.FromImage((Image)b);

                if(colors[current_color][3] == 0F)
                {
                    colorMatrix = new ColorMatrix(new float[][]{
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 1F}});
                }
                else
                {
                    colorMatrix = new ColorMatrix(new float[][]{
   new float[] {0.22F+colors[current_color][0],  0,  0,  0, 0},
   new float[] {0,  0.251F+colors[current_color][1],  0,  0, 0},
   new float[] {0,  0,  0.31F+colors[current_color][2],  0, 0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0, 0, 0, 0, 1F}});
                }
                imageAttributes.SetColorMatrix(
                   colorMatrix,
                   ColorMatrixFlag.Default,
                   ColorAdjustType.Bitmap);
                g.DrawImage(image,
                   new Rectangle(0, 0,
                       width, height),  // destination rectangle 
                   0, 0,        // upper-left corner of source rectangle 
                   width,       // width of source rectangle
                   height,      // height of source rectangle
                   GraphicsUnit.Pixel,
                   imageAttributes);
                Color c;
                for (int i = 0; i < width; i++)
                {
                    for(int j = 0; j < height; j++)
                    {
                        double h, s, v;
                        ColorToHSV(b.GetPixel(i, j), out h, out s, out v);
                        double s_alter = (Math.Pow(s + 0.1, 2.2 - 2.2 * s)),
                            v_alter = Math.Pow(v, 2.0 - 2.0 * v);
                        v_alter *= Math.Pow(v_alter, 0.48);
                        c = ColorFromHSV(h, s_alter, v_alter);
                        cubes[current_color, i * 4 + j * 4 * width + 0] = c.B;
                        cubes[current_color, i * 4 + j * 4 * width + 1] = c.G;
                        cubes[current_color, i * 4 + j * 4 * width + 2] = c.R;
                        cubes[current_color, i * 4 + j * 4 * width + 3] = c.A;
                    }
                }
            }
            byte[][] cubes2 = new byte[colorcount][];
            for(int c = 0; c < colorcount; c++)
            {
                cubes2[c] = new byte[width * height * 4];
                for(int j = 0; j < width * height * 4; j++)
                {
                    cubes2[c][j] = cubes[c, j];
                }
            }

            return cubes2;
        }

        private static byte[][] StoreColorCubesOrtho()
        {
            colorcount = colors.Length;
            byte[,] cubes = new byte[colorcount, 60];

            Image image = ortho;
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = 3;
            int height = 5;
            float[][] colorMatrixElements = { 
   new float[] {1F, 0,  0,  0,  0},
   new float[] {0, 1F,  0,  0,  0},
   new float[] {0,  0,  1F, 0,  0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0,  0,  0,  0, 1F}};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);

            for (int current_color = 0; current_color < colorcount; current_color++)
            {
                Bitmap b =
                new Bitmap(width, height, PixelFormat.Format32bppArgb);

                Graphics g = Graphics.FromImage((Image)b);

                if (colors[current_color][3] == 0F)
                {
                    colorMatrix = new ColorMatrix(new float[][]{ 
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 1F}});
                }
                else
                {
                    colorMatrix = new ColorMatrix(new float[][]{ 
   new float[] {0.22F+colors[current_color][0],  0,  0,  0, 0},
   new float[] {0,  0.251F+colors[current_color][1],  0,  0, 0},
   new float[] {0,  0,  0.31F+colors[current_color][2],  0, 0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0, 0, 0, 0, 1F}});
                }
                imageAttributes.SetColorMatrix(
                   colorMatrix,
                   ColorMatrixFlag.Default,
                   ColorAdjustType.Bitmap);

                g.DrawImage(image,
                   new Rectangle(0, 0,
                       width, height),  // destination rectangle 
                    //                   new Rectangle((vx.x + vx.y) * 4, 128 - 6 - 32 - vx.y * 2 + vx.x * 2 - 4 * vx.z, width, height),  // destination rectangle 
                   0, 0,        // upper-left corner of source rectangle 
                   width,       // width of source rectangle
                   height,      // height of source rectangle
                   GraphicsUnit.Pixel,
                   imageAttributes);
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        Color c = b.GetPixel(i, j);
                        double h = 0.0, s = 1.0, v = 1.0;
                        ColorToHSV(c, out h, out s, out v);
                        double s_alter = (Math.Pow(s + 0.1, 2.2 - 2.2 * s)),
                            v_alter = Math.Pow(v, 2.0 - 2.0 * v);
                        v_alter *= Math.Pow(v_alter, 0.48);
                        c = ColorFromHSV(h, s_alter, v_alter);

                        cubes[current_color, i * 4 + j * 4 * width + 0] = c.B;
                        cubes[current_color, i * 4 + j * 4 * width + 1] = c.G;
                        cubes[current_color, i * 4 + j * 4 * width + 2] = c.R;
                        cubes[current_color, i * 4 + j * 4 * width + 3] = c.A;
                    }
                }
            }

            byte[][] cubes2 = new byte[colorcount][];
            for (int c = 0; c < colorcount; c++)
            {
                cubes2[c] = new byte[width * height * 4];
                for (int j = 0; j < width * height * 4; j++)
                {
                    cubes2[c][j] = cubes[c, j];
                }
            }

            return cubes2;
        }

        private static byte[][] StoreColorCubes45()
        {
            colorcount = colors.Length;
            byte[,] cubes = new byte[colorcount, 24];

            Image image = cube;
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = 2;
            int height = 3;
            float[][] colorMatrixElements = {
   new float[] {1F, 0,  0,  0,  0},
   new float[] {0, 1F,  0,  0,  0},
   new float[] {0,  0,  1F, 0,  0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0,  0,  0,  0, 1F}};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);

            for(int current_color = 0; current_color < colorcount; current_color++)
            {
                Bitmap b =
                new Bitmap(4, 5, PixelFormat.Format32bppArgb);

                Graphics g = Graphics.FromImage((Image)b);

                if(colors[current_color][3] == 0F)
                {
                    colorMatrix = new ColorMatrix(new float[][]{
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 1F}});
                }
                else
                {
                    colorMatrix = new ColorMatrix(new float[][]{
   new float[] {0.22F+colors[current_color][0],  0,  0,  0, 0},
   new float[] {0,  0.251F+colors[current_color][1],  0,  0, 0},
   new float[] {0,  0,  0.31F+colors[current_color][2],  0, 0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0, 0, 0, 0, 1F}});
                }
                imageAttributes.SetColorMatrix(
                   colorMatrix,
                   ColorMatrixFlag.Default,
                   ColorAdjustType.Bitmap);
                g.DrawImage(image,
                   new Rectangle(0, 0,
                       4, 5),  // destination rectangle 
                   0, 0,        // upper-left corner of source rectangle 
                   4,       // width of source rectangle
                   5,      // height of source rectangle
                   GraphicsUnit.Pixel,
                   imageAttributes);
                for(int i = 0; i < width; i++)
                {
                    for(int j = 0; j < height; j++)
                    {
                        Color c = b.GetPixel(i+1, j * 2);
                        double h = 0.0, s = 1.0, v = 1.0;
                        ColorToHSV(c, out h, out s, out v);
                        double s_alter = (Math.Pow(s + 0.1, 2.2 - 2.2 * s)),
                            v_alter = Math.Pow(v, 2.0 - 2.0 * v);
                        v_alter *= Math.Pow(v_alter, 0.48);
                        c = ColorFromHSV(h, s_alter, v_alter);
                        cubes[current_color, i * 4 + j * 4 * width + 0] = c.B;
                        cubes[current_color, i * 4 + j * 4 * width + 1] = c.G;
                        cubes[current_color, i * 4 + j * 4 * width + 2] = c.R;
                        cubes[current_color, i * 4 + j * 4 * width + 3] = c.A;
                    }
                }
            }
            byte[][] cubes2 = new byte[colorcount][];
            for(int c = 0; c < colorcount; c++)
            {
                cubes2[c] = new byte[width * height * 4];
                for(int j = 0; j < width * height * 4; j++)
                {
                    cubes2[c][j] = cubes[c, j];
                }
            }

            return cubes2;
        }

        private static Color FromLightness(Bitmap b, int light)
        {
            double h, s, v;
            ColorToHSV(b.GetPixel(245, 0), out h, out s, out v);
            s = (Math.Pow(s + 0.1, 2.2 - 2.2 * s));
            v = Math.Pow(v, 2.0 - 2.0 * v);
            v *= Math.Pow(v, 0.48);
            return ColorFromHSV(h, s, v);
        }
        public static void StoreColorCubesFaces()
        {
            colorcount = colors.Length;
            // 29 is the number of Slope enum types.
            renderedFace = new byte[colorcount][][];
            for(int c = 0; c < colorcount; c++)
            {
                renderedFace[c] = new byte[29][];
                for(int sp = 0; sp < 29; sp++)
                {
                    renderedFace[c][sp] = new byte[80];
                }
            }

            Image image = gradient;
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = 4;
            int height = 5;
            float[][] colorMatrixElements = {
   new float[] {1F, 0,  0,  0,  0},
   new float[] {0, 1F,  0,  0,  0},
   new float[] {0,  0,  1F, 0,  0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0,  0,  0,  0, 1F}};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);
            for(int current_color = 0; current_color < colorcount; current_color++)
            {
                Bitmap b = new Bitmap(256, 1, PixelFormat.Format32bppArgb);

                Graphics g = Graphics.FromImage(b);

                if(colors[current_color][3] == 0F)
                {
                    colorMatrix = new ColorMatrix(new float[][]{
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 1F}});
                }
                else
                {
                    colorMatrix = new ColorMatrix(new float[][]{
   new float[] {0.22F+colors[current_color][0],  0,  0,  0, 0},
   new float[] {0,  0.251F+colors[current_color][1],  0,  0, 0},
   new float[] {0,  0,  0.31F+colors[current_color][2],  0, 0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0, 0, 0, 0, 1F}});
                }
                imageAttributes.SetColorMatrix(
                   colorMatrix,
                   ColorMatrixFlag.Default,
                   ColorAdjustType.Bitmap);

                g.DrawImage(image,
                   new Rectangle(0, 0,
                       256, 1), // destination rectangle 
                   0, 0,        // upper-left corner of source rectangle 
                   256,         // width of source rectangle
                   1,           // height of source rectangle
                   GraphicsUnit.Pixel,
                   imageAttributes);

                Color shining = FromLightness(b, 255);
                Color top = FromLightness(b, 245);
                Color bright = FromLightness(b, 220);
                Color gentle = FromLightness(b, 212);
                Color dim = FromLightness(b, 196);
                Color under = FromLightness(b, 188);
                Color dark = FromLightness(b, 180);

                for (int i = 0; i < width; i++)
                {
                    for(int j = 0; j < height; j++)
                    {
                        for(int slp = 0; slp < 29; slp++)
                        {
                            Color c2 = Color.Transparent;
                            if (j == height - 1)
                            {
                                c2 = dark;
                            }
                            else
                            {
                                switch(slp)
                                {
                                    case Cube:
                                        {
                                            if(j == 0)
                                            {
                                                c2 = top;
                                            }
                                            else if(i < width / 2)
                                            {
                                                c2 = bright;
                                            }
                                            else if(i >= width / 2)
                                            {
                                                c2 = dim;
                                            }
                                        }
                                        break;
                                    case BrightTop:
                                        {
                                            if(i + j / 2 >= 4)
                                            {
                                                c2 = dim;
                                            }
                                            else if(i + (j + 1) / 2 >= 2)
                                            {
                                                c2 = shining;
                                            }
                                        }
                                        break;
                                    case DimTop:
                                        {
                                            if(i < j / 2)
                                            {
                                                c2 = bright;
                                            }
                                            else if(i - 1 <= (j + 1) / 2)
                                            {
                                                c2 = gentle;
                                            }
                                        }
                                        break;
                                    case BrightDim:
                                        {
                                            c2 = gentle;
                                        }
                                        break;
                                    case BrightDimTop:
                                    case BrightDimTopThick:
                                        {
                                            if(((i > 0 && i < 3) || j >= 3) && j > 0)
                                                c2 = gentle;
                                        }
                                        break;
                                    case BrightBottom:
                                        {
                                            if(i > (j + 1) / 2 + 1)
                                            {
                                                c2 = dim;
                                            }
                                            else if(i + 1 > (j + 1) / 2)
                                            {
                                                c2 = under;
                                            }
                                        }
                                        break;
                                    case DimBottom:
                                        {
                                            if(i + (j + 1) / 2 < 2)
                                            {
                                                c2 = bright;
                                            }
                                            else if(i + (j + 1) / 2 < 4)
                                            {
                                                c2 = under;
                                            }
                                        }
                                        break;
                                    case BrightDimBottom:
                                    case BrightDimBottomThick:
                                        {
                                            c2 = under;
                                        }
                                        break;

                                    case BrightBack:
                                        {
                                            c2 = shining;
                                        }
                                        break;
                                    case DimBack:
                                        {
                                            c2 = bright;
                                        }
                                        break;
                                    case BrightTopBack:
                                        {
                                            if(i + (j + 3) / 4 >= 2)
                                            {
                                                c2 = top;
                                            }
                                        }
                                        break;
                                    case DimTopBack:
                                        {
                                            if(i - (j + 3) / 4 <= 1)
                                            {
                                                c2 = gentle;
                                            }
                                        }
                                        break;
                                    case BrightBottomBack:
                                        {
                                            if(i >= j)
                                            {
                                                c2 = under;
                                            }
                                        }
                                        break;
                                    case DimBottomBack:
                                        {
                                            if(i + j <= 3)
                                            {
                                                c2 = under;
                                            }
                                        }
                                        break;
                                    case BrightTopBackThick:
                                        {
                                            c2 = top;
                                        }
                                        break;
                                    case DimTopBackThick:
                                        {
                                            c2 = gentle;
                                        }
                                        break;
                                    case BrightBottomBackThick:
                                        {
                                            c2 = under;
                                        }
                                        break;
                                    case DimBottomBackThick:
                                        {
                                            if(i + j <= 3)
                                            {
                                                c2 = under;
                                            }
                                        }
                                        break;
                                    case RearBrightTop:
                                        {
                                            if(i + (j + 3) / 4 >= 3 && j > 0)
                                            {
                                                c2 = top;
                                            }
                                        }
                                        break;
                                    case RearDimTop:
                                        {
                                            if(i - (j + 3) / 4 <= 0 && j > 0)
                                            {
                                                c2 = gentle;
                                            }
                                        }
                                        break;
                                    case RearBrightBottom:
                                        {
                                            if(i > j)
                                            {
                                                c2 = under;
                                            }
                                        }
                                        break;
                                    case RearDimBottom:
                                        {
                                            if(i + j <= 2)
                                            {
                                                c2 = under;
                                            }
                                        }
                                        break;
                                    case BackBackTop:
                                    case BackBack:
                                        {
                                            if(j > 0)
                                            {
                                                c2 = bright;
                                            }
                                        }
                                        break;
                                    case BackBackTopThick:
                                        {
                                            c2 = top;
                                        }
                                        break;
                                        /*
                                    case BackBackBottom:
                                    case BackBackBottomThick:
                                    default:
                                        {

                                        }
                                        break;
                                        */
                                }
                            }

                            if(c2.A != 0)
                            {
                                renderedFace[current_color][slp][i * 4 + j * width * 4 + 0] = Math.Max((byte)1, c2.B);
                                renderedFace[current_color][slp][i * 4 + j * width * 4 + 1] = Math.Max((byte)1, c2.G);
                                renderedFace[current_color][slp][i * 4 + j * width * 4 + 2] = Math.Max((byte)1, c2.R);
                                renderedFace[current_color][slp][i * 4 + j * width * 4 + 3] = c2.A;
                            }
                            else
                            {
                                renderedFace[current_color][slp][i * 4 + j * 4 * width + 0] = 0;
                                renderedFace[current_color][slp][i * 4 + j * 4 * width + 1] = 0;
                                renderedFace[current_color][slp][i * 4 + j * 4 * width + 2] = 0;
                                renderedFace[current_color][slp][i * 4 + j * 4 * width + 3] = 0;
                            }
                        }
                    }
                }
            }
        }

        public static void StoreColorCubesFacesOrtho()
        {
            colorcount = colors.Length;
            // 29 is the number of Slope enum types.
            renderedFaceOrtho = new byte[colorcount][][];
            for (int c = 0; c < colorcount; c++)
            {
                renderedFaceOrtho[c] = new byte[29][];
                for (int sp = 0; sp < 29; sp++)
                {
                    renderedFaceOrtho[c][sp] = new byte[60];
                }
            }

            Image image = gradient;
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = 3;
            int height = 5;
            float[][] colorMatrixElements = {
   new float[] {1F, 0,  0,  0,  0},
   new float[] {0, 1F,  0,  0,  0},
   new float[] {0,  0,  1F, 0,  0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0,  0,  0,  0, 1F}};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);
            for (int current_color = 0; current_color < colorcount; current_color++)
            {
                Bitmap b = new Bitmap(256, 1, PixelFormat.Format32bppArgb);

                Graphics g = Graphics.FromImage(b);

                if (colors[current_color][3] == 0F)
                {
                    colorMatrix = new ColorMatrix(new float[][]{
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 1F}});
                }
                else
                {
                    colorMatrix = new ColorMatrix(new float[][]{
   new float[] {0.22F+colors[current_color][0],  0,  0,  0, 0},
   new float[] {0,  0.251F+colors[current_color][1],  0,  0, 0},
   new float[] {0,  0,  0.31F+colors[current_color][2],  0, 0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0, 0, 0, 0, 1F}});
                }
                imageAttributes.SetColorMatrix(
                   colorMatrix,
                   ColorMatrixFlag.Default,
                   ColorAdjustType.Bitmap);

                g.DrawImage(image,
                   new Rectangle(0, 0,
                       256, 1),  // destination rectangle 
                                        //                   new Rectangle((vx.x + vx.y) * 4, 128 - 6 - 32 - vx.y * 2 + vx.x * 2 - 4 * vx.z, width, height),  // destination rectangle 
                   0, 0,        // upper-left corner of source rectangle 
                   256,       // width of source rectangle
                   1,      // height of source rectangle
                   GraphicsUnit.Pixel,
                   imageAttributes);

                Color shining = FromLightness(b, 255);
                Color top = FromLightness(b, 245);
                Color bright = FromLightness(b, 220);
                Color front = FromLightness(b, 212);
                Color dim = FromLightness(b, 196);
                Color under = FromLightness(b, 188);
                Color dark = FromLightness(b, 180);


                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        for (int slp = 0; slp < 29; slp++)
                        {
                            Color c2 = Color.Transparent;
                            if (j == height - 1)
                            {
                                c2 = dark;
                            }
                            else
                            {
                                switch (slp)
                                {
                                    case Cube:
                                        {
                                            if (j == 0)
                                            {
                                                c2 = top;
                                            }
                                            else
                                            {
                                                c2 = front;
                                            }
                                        }
                                        break;
                                    case BrightTop:
                                        {
                                            if (i + j >= 3)
                                            {
                                                c2 = front;
                                            }
                                            else if (i + j >= 2)
                                            {
                                                c2 = shining;
                                            }
                                        }
                                        break;
                                    case DimTop:
                                        {
                                            c2 = bright;
                                        }
                                        break;
                                    case BrightDim:
                                        {
                                            if (j == 0)
                                            {
                                                c2 = top;
                                            }
                                            else if(i - j >= -2)
                                            {
                                                c2 = bright;
                                            }
                                        }
                                        break;
                                    case BrightDimTop:
                                    case BrightDimTopThick:
                                        {
                                            if (i - j >= -1 && i + j >= 2)
                                            {
                                                c2 = shining;
                                            }
                                        }
                                        break;
                                    case BrightBottomBack:
                                    case BrightBottom:
                                        {
                                            if (j == 0)
                                            {
                                                c2 = top;
                                            }
                                            else if(i - j >= -1)
                                            {
                                                c2 = front;
                                            }
                                        }
                                        break;
                                    case DimBottomBack:
                                    case DimBottom:
                                        {
                                            if (j == 0)
                                            {
                                                c2 = top;
                                            }
                                            else if (j < 3)
                                            {
                                                c2 = under;
                                            }
                                        }
                                        break;
                                    case BrightDimBottom:
                                    case BrightDimBottomThick:
                                        {
                                            if (j == 0)
                                            {
                                                c2 = top;
                                            }
                                            else if (i - j >= 0)
                                            {
                                                c2 = dim;
                                            }
                                        }
                                        break;
                                    case BrightBack:
                                        {
                                            if (j == 0)
                                            {
                                                if(i > 0)
                                                    c2 = top;
                                            }
                                            else
                                            {
                                                c2 = front;
                                            }
                                        }
                                        break;
                                    case DimBack:
                                        {
                                            if (j == 0)
                                            {
                                                if (i < 2)
                                                    c2 = top;
                                            }
                                            else if(i + j <= 4)
                                            {
                                                c2 = dim;
                                            }
                                        }
                                        break;
                                    case BrightTopBack:
                                        break;
                                    case DimTopBack:
                                        {
                                            if (i - j <= 0 && i + j <= 3)
                                            {
                                                c2 = bright;
                                            }
                                        }
                                        break;
                                    case BrightTopBackThick:
                                    case DimTopBackThick:
                                    case BrightBottomBackThick:
                                    case DimBottomBackThick:
                                    case BackBackTopThick:
                                        {
                                            if (j == 0)
                                            {
                                                c2 = top;
                                            }
                                            else
                                            {
                                                c2 = front;
                                            }
                                        }
                                        break;
                                    case RearBrightBottom:
                                    case RearBrightTop:
                                        break;
                                    case RearDimTop:
                                        {
                                            if (i - j <= -1)
                                            {
                                                c2 = front;
                                            }
                                            else if (i - j <= 0)
                                            {
                                                c2 = bright;
                                            }
                                        }
                                        break;
                                    case RearDimBottom:
                                        {
                                            if (j == 0)
                                            {
                                                c2 = top;
                                            }
                                            else if (i + j <= 3)
                                            {
                                                c2 = front;
                                            }
                                        }
                                        break;
                                    case BackBackTop:
                                    case BackBack:
                                        {
                                            if (j == 0)
                                            {
                                                if(i < 2)
                                                    c2 = top;
                                            }
                                            else
                                            {
                                                c2 = front;
                                            }
                                        }
                                        break;
                                        /*
                                    case BackBackBottom:
                                    case BackBackBottomThick:
                                    default:
                                        {

                                        }
                                        break;
                                        */
                                }
                            }

                            if (c2.A != 0)
                            {
                                renderedFaceOrtho[current_color][slp][i * 4 + j * width * 4 + 0] = Math.Max((byte)1, c2.B);
                                renderedFaceOrtho[current_color][slp][i * 4 + j * width * 4 + 1] = Math.Max((byte)1, c2.G);
                                renderedFaceOrtho[current_color][slp][i * 4 + j * width * 4 + 2] = Math.Max((byte)1, c2.R);
                                renderedFaceOrtho[current_color][slp][i * 4 + j * width * 4 + 3] = c2.A;
                            }
                            else
                            {
                                renderedFaceOrtho[current_color][slp][i * 4 + j * 4 * width + 0] = 0;
                                renderedFaceOrtho[current_color][slp][i * 4 + j * 4 * width + 1] = 0;
                                renderedFaceOrtho[current_color][slp][i * 4 + j * 4 * width + 2] = 0;
                                renderedFaceOrtho[current_color][slp][i * 4 + j * 4 * width + 3] = 0;
                            }
                        }
                    }
                }
            }
        }


        private static void StoreColorCubesFacesSmall()
        {
            colorcount = colors.Length;
            // 29 is the number of Slope enum types.
            renderedFaceSmall = new byte[colorcount][][];
            for(int c = 0; c < colorcount; c++)
            {
                renderedFaceSmall[c] = new byte[29][];
                for(int sp = 0; sp < 29; sp++)
                {
                    renderedFaceSmall[c][sp] = new byte[96];
                }
            }


            Image image = white;
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = 4;
            int height = 6;
            float[][] colorMatrixElements = {
   new float[] {1F, 0,  0,  0,  0},
   new float[] {0, 1F,  0,  0,  0},
   new float[] {0,  0,  1F, 0,  0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0,  0,  0,  0, 1F}};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);
            for(int current_color = 0; current_color < colorcount; current_color++)
            {
                Bitmap b = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                Graphics g = Graphics.FromImage(b);

                if(colors[current_color][3] == 0F)
                {
                    colorMatrix = new ColorMatrix(new float[][]{
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 0},
   new float[] {0,  0,  0,  0, 1F}});
                }
                else
                {
                    colorMatrix = new ColorMatrix(new float[][]{
   new float[] {0.22F+colors[current_color][0],  0,  0,  0, 0},
   new float[] {0,  0.251F+colors[current_color][1],  0,  0, 0},
   new float[] {0,  0,  0.31F+colors[current_color][2],  0, 0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0, 0, 0, 0, 1F}});
                }
                imageAttributes.SetColorMatrix(
                   colorMatrix,
                   ColorMatrixFlag.Default,
                   ColorAdjustType.Bitmap);

                g.DrawImage(image,
                   new Rectangle(0, 0,
                       width, height),  // destination rectangle 
                                        //                   new Rectangle((vx.x + vx.y) * 4, 128 - 6 - 32 - vx.y * 2 + vx.x * 2 - 4 * vx.z, width, height),  // destination rectangle 
                   0, 0,        // upper-left corner of source rectangle 
                   width,       // width of source rectangle
                   height,      // height of source rectangle
                   GraphicsUnit.Pixel,
                   imageAttributes);

                for(int i = 0; i < width; i++)
                {
                    for(int j = 0; j < height; j++)
                    {
                        Color c = b.GetPixel(i, j);
                        double h = 0.0, s = 1.0, v = 1.0;
                        ColorToHSV(c, out h, out s, out v);

                        for(int slp = 0; slp < 29; slp++)
                        {
                            Color c2 = Color.Transparent;
                            //double s_alter = (Math.Pow(s + 0.04, 2.08 - 2.08 * s)),
                            //            v_alter = Math.Pow(v, 2.0 - 2.0 * v);
                            //v_alter = MercifulClamp(v_alter * (0.25 + Math.Pow(v_alter, 0.5)) * 0.76);

                            double s_alter = (Math.Pow(s + 0.1, 2.2 - 2.2 * s)),
                                v_alter = Math.Pow(v, 2.0 - 2.0 * v);
                            v_alter *= Math.Pow(v_alter, 0.48);


                            if (j == height - 1)
                            {
                                c2 = ColorFromHSV(h, Clamp((s + s * s * s * Math.Pow(s, 0.3)) * 1.55, 0.0112, 1.0), Clamp(v_alter * 0.65, 0.01, 1.0));
                            }
                            else
                            {
                                switch(slp)
                                {
                                    case Cube:
                                        {
                                            if(j == 0)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 0.85, 0.0112, 1.0), Clamp(v_alter * 1.05, 0.09, 1.0));
                                            }
                                            else if(i < width / 2)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 0.95, 0.0112, 1.0), Clamp(v_alter * 0.95, 0.06, 1.0));
                                            }
                                            else if(i >= width / 2)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.05, 0.0112, 1.0), Clamp(v_alter * 0.85, 0.03, 1.0));
                                            }
                                        }
                                        break;
                                    case BrightTop:
                                        {
                                            if(i + j / 2 >= 4)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.05, 0.0112, 1.0), Clamp(v_alter * 0.85, 0.03, 1.0));
                                            }
                                            else if(i + (j + 1) / 2 >= 2)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 0.9, 0.0112, 1.0), Clamp(v_alter * 1.1, 0.10, 1.0));
                                            }
                                        }
                                        break;
                                    case DimTop:
                                        {
                                            if(i < j / 2)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.0, 0.0112, 1.0), Clamp(v_alter * 0.95, 0.06, 1.0));
                                            }
                                            else if(i - 1 <= (j + 1) / 2)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.1, 0.0112, 1.0), Clamp(v_alter * 0.9, 0.05, 1.0));
                                            }
                                        }
                                        break;
                                    case BrightDim:
                                        {
                                            c2 = ColorFromHSV(h, Clamp(s_alter * 1.15, 0.0112, 1.0), Clamp(v_alter * 0.9, 0.05, 1.0));
                                        }
                                        break;
                                    case BrightDimTop:
                                    case BrightDimTopThick:
                                        {
                                            if(((i > 0 && i < 3) || j >= 3) && j > 0)
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.0, 0.0112, 1.0), Clamp(v_alter * 0.9, 0.08, 1.0));
                                        }
                                        break;
                                    case BrightBottom:
                                        {
                                            if(i > (j + 1) / 2 + 1)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.05, 0.0112, 1.0), Clamp(v_alter * 0.85, 0.03, 1.0));
                                            }
                                            else if(i + 1 > (j + 1) / 2)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.2, 0.0112, 1.0), Clamp(v_alter * 0.6, 0.02, 1.0));
                                            }
                                        }
                                        break;
                                    case DimBottom:
                                        {
                                            if(i + (j + 1) / 2 < 2)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.05, 0.0112, 1.0), Clamp(v_alter * 0.95, 0.06, 1.0));
                                            }
                                            else if(i + (j + 1) / 2 < 4)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.2, 0.0112, 1.0), Clamp(v_alter * 0.5, 0.01, 1.0));
                                            }
                                        }
                                        break;
                                    case BrightDimBottom:
                                    case BrightDimBottomThick:
                                        {
                                            c2 = ColorFromHSV(h, Clamp(s_alter * 1.15, 0.0112, 1.0), Clamp(v_alter * 0.55, 0.015, 1.0));
                                        }
                                        break;

                                    case BrightBack:
                                        {
                                            c2 = ColorFromHSV(h, Clamp(s_alter * 0.95, 0.0112, 1.0), Clamp(v_alter * 1.0, 0.09, 1.0));
                                        }
                                        break;
                                    case DimBack:
                                        {
                                            c2 = ColorFromHSV(h, Clamp(s_alter * 1.0, 0.0112, 1.0), Clamp(v_alter * 1.0, 0.09, 1.0));
                                        }
                                        break;
                                    case BrightTopBack:
                                        {
                                            if(i + (j + 3) / 4 >= 2)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 0.95, 0.0112, 1.0), Clamp(v_alter * 1.05, 0.09, 1.0));
                                            }
                                        }
                                        break;
                                    case DimTopBack:
                                        {
                                            if(i - (j + 3) / 4 <= 1)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.05, 0.0112, 1.0), Clamp(v_alter * 0.9, 0.09, 1.0));
                                            }
                                        }
                                        break;
                                    case BrightBottomBack:
                                        {
                                            if(i >= j)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.2, 0.0112, 1.0), Clamp(v_alter * 0.55, 0.01, 1.0));
                                            }
                                        }
                                        break;
                                    case DimBottomBack:
                                        {
                                            if(i + j <= 3)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.25, 0.0112, 1.0), Clamp(v_alter * 0.55, 0.01, 1.0));
                                            }
                                        }
                                        break;
                                    case BrightTopBackThick:
                                        {
                                            c2 = ColorFromHSV(h, Clamp(s_alter * 0.95, 0.0112, 1.0), Clamp(v_alter * 1.05, 0.09, 1.0));
                                        }
                                        break;
                                    case DimTopBackThick:
                                        {
                                            c2 = ColorFromHSV(h, Clamp(s_alter * 1.05, 0.0112, 1.0), Clamp(v_alter * 0.9, 0.09, 1.0));
                                        }
                                        break;
                                    case BrightBottomBackThick:
                                        {
                                            c2 = ColorFromHSV(h, Clamp(s_alter * 1.2, 0.0112, 1.0), Clamp(v_alter * 0.55, 0.01, 1.0));
                                        }
                                        break;
                                    case DimBottomBackThick:
                                        {
                                            if(i + j <= 3)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.2, 0.0112, 1.0), Clamp(v_alter * 0.55, 0.01, 1.0));
                                            }
                                        }
                                        break;
                                    case RearBrightTop:
                                        {
                                            if(i + (j + 3) / 4 >= 3 && j > 0)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 0.85, 0.0112, 1.0), Clamp(v_alter * 1.05, 0.09, 1.0));
                                            }
                                        }
                                        break;
                                    case RearDimTop:
                                        {
                                            if(i - (j + 3) / 4 <= 0 && j > 0)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.0, 0.0112, 1.0), Clamp(v_alter * 0.9, 0.09, 1.0));
                                            }
                                        }
                                        break;
                                    case RearBrightBottom:
                                        {
                                            if(i > j)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.05, 0.0112, 1.0), Clamp(v_alter * 0.55, 0.01, 1.0));
                                            }
                                        }
                                        break;
                                    case RearDimBottom:
                                        {
                                            if(i + j <= 2)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 1.2, 0.0112, 1.0), Clamp(v_alter * 0.55, 0.01, 1.0));
                                            }
                                        }
                                        break;
                                    case BackBackTop:
                                    case BackBack:
                                        {
                                            if(j > 0)
                                            {
                                                c2 = ColorFromHSV(h, Clamp(s_alter * 0.85, 0.0112, 1.0), Clamp(v_alter * 1.0, 0.09, 1.0));
                                            }
                                        }
                                        break;
                                    case BackBackTopThick:
                                        {
                                            c2 = ColorFromHSV(h, Clamp(s_alter * 0.85, 0.0112, 1.0), Clamp(v_alter * 1.0, 0.09, 1.0));
                                        }
                                        break;
                                        /*
                                    case BackBackBottom:
                                    case BackBackBottomThick:
                                    default:
                                        {

                                        }
                                        break;
                                        */
                                }
                            }
                            if(c2.A != 0)
                            {
                                renderedFaceSmall[current_color][slp][i * 4 + j * width * 4 + 0] = Math.Max((byte)1, c2.B);
                                renderedFaceSmall[current_color][slp][i * 4 + j * width * 4 + 1] = Math.Max((byte)1, c2.G);
                                renderedFaceSmall[current_color][slp][i * 4 + j * width * 4 + 2] = Math.Max((byte)1, c2.R);
                                renderedFaceSmall[current_color][slp][i * 4 + j * width * 4 + 3] = 255;
                            }
                            else
                            {
                                renderedFaceSmall[current_color][slp][i * 4 + j * 4 * width + 0] = 0;
                                renderedFaceSmall[current_color][slp][i * 4 + j * 4 * width + 1] = 0;
                                renderedFaceSmall[current_color][slp][i * 4 + j * 4 * width + 2] = 0;
                                renderedFaceSmall[current_color][slp][i * 4 + j * 4 * width + 3] = 0;
                            }
                        }
                    }
                }
            }
           
            //return renderedFaceSmall;
        }



        /// <summary>
        /// Render voxel chunks in a MagicaVoxelData[] to a Bitmap with X pointing in any direction.
        /// </summary>
        /// <param name="voxels">The result of calling FromMagica.</param>
        /// <param name="xSize">The bounding X size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="ySize">The bounding Y size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="zSize">The bounding Z size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="dir">The Direction enum that specifies which way the X-axis should point.</param>
        /// <returns>A Bitmap view of the voxels in isometric pixel view.</returns>
        public static Bitmap Render(MagicaVoxelData[] voxels, byte xSize, byte ySize, byte zSize, Direction dir)
        {
            int bWidth = (xSize + ySize) * 2;
            int bHeight = (xSize + ySize) + zSize * 3;
            Bitmap b = new Bitmap(bWidth, bHeight);
            Graphics g = Graphics.FromImage((Image)b);
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = 4;
            int height = 4;

            float[][] colorMatrixElements = { 
   new float[] {1F,  0,  0,  0, 0},
   new float[] {0,  1F,  0,  0, 0},
   new float[] {0,   0, 1F,  0, 0},
   new float[] {0,   0,  0, 1F, 0},
   new float[] {0,   0,  0,  0, 1F}};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);
            MagicaVoxelData[] vls = new MagicaVoxelData[voxels.Length];
            switch (dir)
            {
                case Direction.SE:
                    vls = voxels;
                    break;
                case Direction.SW:
                    for (int i = 0; i < voxels.Length; i++)
                    {
                        byte tempX = (byte)(voxels[i].x - (xSize / 2));
                        byte tempY = (byte)(voxels[i].y - (ySize / 2));
                        vls[i].x = (byte)((tempY) + (ySize / 2));
                        vls[i].y = (byte)((tempX * -1) + (xSize / 2) - 1);
                        vls[i].z = voxels[i].z;
                        vls[i].color = voxels[i].color;
                    }
                    break;

                case Direction.NW:
                    for (int i = 0; i < voxels.Length; i++)
                    {
                        byte tempX = (byte)(voxels[i].x - (xSize / 2));
                        byte tempY = (byte)(voxels[i].y - (ySize / 2));
                        vls[i].x = (byte)((tempX * -1) + (xSize / 2) - 1);
                        vls[i].y = (byte)((tempY * -1) + (ySize / 2) - 1);
                        vls[i].z = voxels[i].z;
                        vls[i].color = voxels[i].color;
                    }
                    break;
                case Direction.NE:
                    for (int i = 0; i < voxels.Length; i++)
                    {
                        byte tempX = (byte)(voxels[i].x - (xSize / 2));
                        byte tempY = (byte)(voxels[i].y - (ySize / 2));
                        vls[i].x = (byte)((tempY * -1) + (ySize / 2) - 1);
                        vls[i].y = (byte)(tempX + (xSize / 2));
                        vls[i].z = voxels[i].z;
                        vls[i].color = voxels[i].color;
                    }
                    break;
            }
            foreach (MagicaVoxelData vx in vls.OrderBy(v => v.x * 32 - v.y + v.z * 32 * 128))
            {
                int currentColor = vx.color - 1;
                colorMatrix = new ColorMatrix(new float[][]{ 
   new float[] {colors[currentColor][0],  0,  0,  0, 0},
   new float[] {0,  colors[currentColor][1],  0,  0, 0},
   new float[] {0,  0,  colors[currentColor][2],  0, 0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0, 0, 0, 0, 1F}});

                imageAttributes.SetColorMatrix(
                   colorMatrix,
                   ColorMatrixFlag.Default,
                   ColorAdjustType.Bitmap);

                g.DrawImage(
                   cube,
                    //(3 * zSize - 2)
                   new Rectangle((vx.x + vx.y) * 2, (bHeight - xSize - 2) - vx.y + vx.x - 3 * vx.z, width, height),  // destination rectangle 
                   0, 0,        // upper-left corner of source rectangle 
                   width,       // width of source rectangle
                   height,      // height of source rectangle
                   GraphicsUnit.Pixel,
                   imageAttributes);
            }
            return b;
        }
        /*
        /// <summary>
        /// Render outline chunks in a MagicaVoxelData[] to a Bitmap with X pointing in any direction.
        /// </summary>
        /// <param name="voxels">The result of calling FromMagica.</param>
        /// <param name="xSize">The bounding X size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="ySize">The bounding Y size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="zSize">The bounding Z size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="dir">The Direction enum that specifies which way the X-axis should point.</param>
        /// <returns>A Bitmap view of the voxels in isometric pixel view.</returns>
        public static Bitmap renderOutline(MagicaVoxelData[] voxels, byte xSize, byte ySize, byte zSize, Direction dir)
        {
            int bWidth = (xSize + ySize) * 2 + 4;
            int bHeight = (xSize + ySize) + zSize * 3 + 4;
            Bitmap b = new Bitmap(bWidth, bHeight);
            Graphics g = Graphics.FromImage((Image)b);
            ImageAttributes imageAttributes = new ImageAttributes();
            int width = 8;
            int height = 8;

            float[][] colorMatrixElements = { 
   new float[] {1.2F,  0,  0,  0, 0},
   new float[] {0,  1.2F,  0,  0, 0},
   new float[] {0,   0, 1.2F,  0, 0},
   new float[] {0,   0,    0, 1F, 0},
   new float[] {0,   0,    0,  0, 1F}};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);
            MagicaVoxelData[] vls = new MagicaVoxelData[voxels.Length];
            switch (dir)
            {
                case Direction.SE:
                    vls = voxels;
                    break;
                case Direction.SW:
                    for (int i = 0; i < voxels.Length; i++)
                    {
                        byte tempX = (byte)(voxels[i].x - (xSize / 2));
                        byte tempY = (byte)(voxels[i].y - (ySize / 2));
                        vls[i].x = (byte)((tempY) + (ySize / 2));
                        vls[i].y = (byte)((tempX * -1) + (xSize / 2) - 1);
                        vls[i].z = voxels[i].z;
                        vls[i].color = voxels[i].color;
                    }
                    break;

                case Direction.NW:
                    for (int i = 0; i < voxels.Length; i++)
                    {
                        byte tempX = (byte)(voxels[i].x - (xSize / 2));
                        byte tempY = (byte)(voxels[i].y - (ySize / 2));
                        vls[i].x = (byte)((tempX * -1) + (xSize / 2) - 1);
                        vls[i].y = (byte)((tempY * -1) + (ySize / 2) - 1);
                        vls[i].z = voxels[i].z;
                        vls[i].color = voxels[i].color;
                    }
                    break;
                case Direction.NE:
                    for (int i = 0; i < voxels.Length; i++)
                    {
                        byte tempX = (byte)(voxels[i].x - (xSize / 2));
                        byte tempY = (byte)(voxels[i].y - (ySize / 2));
                        vls[i].x = (byte)((tempY * -1) + (ySize / 2) - 1);
                        vls[i].y = (byte)(tempX + (xSize / 2));
                        vls[i].z = voxels[i].z;
                        vls[i].color = voxels[i].color;
                    }
                    break;
            }
            foreach (MagicaVoxelData vx in vls.OrderBy(v => v.x * 32 - v.y + v.z * 32 * 128))
            {
                g.DrawImage(
                   outline,
                    //(3 * zSize - 2)
                   new Rectangle((vx.x + vx.y) * 2, (bHeight - xSize - 2) - vx.y + vx.x - 3 * vx.z, width, height),  // destination rectangle 
                   0, 0,        // upper-left corner of source rectangle 
                   width,       // width of source rectangle
                   height,      // height of source rectangle
                   GraphicsUnit.Pixel,
                   imageAttributes);
            }
            return b;
        }
        */
        private static int VoxelToPixel(int innerX, int innerY, int x, int y, int z, int current_color, int stride, int xSize, int ySize, int zSize)
        {
            return 4 * ((x + y) * 2 + 4)
                + innerX +
                stride * (((xSize + ySize) + zSize * 3) - Math.Max(xSize, ySize) - y + x - z * 3 + innerY); //(xSize + ySize) * 2
        }
        private static int VoxelToPixelSmall(int innerX, int innerY, int x, int y, int z, int current_color, int stride, byte xSize, byte ySize, byte zSize)
        {
            return 4 * ((x + y) * 2 + 4)
                + innerX +
                stride * (((xSize + ySize) + zSize * 4) - (Math.Max(xSize, ySize)) - y + x - z * 4 + innerY); //(xSize + ySize) * 2
        }

        private static int VoxelToPixelOrtho(int innerX, int innerY, int x, int y, int z, int current_color, int stride, byte xSize, byte ySize, byte zSize)
        {
            /*
            4 * (vx.y * 3 + 6 + ((current_color == 136) ? jitter - 1 : 0))
                             + i +
                           bmpData.Stride * (308 - 60 - 8 + vx.x - vx.z * 3 - ((xcolors[current_color + faction][3] == flat_alpha) ? -3 : jitter) + j)
             */
            return 4 * (y * 3 + 4)
                 + innerX +
                stride * ((zSize * 3 - 1) + x - z * 3 + innerY);
        }

        private static int VoxelToPixelOrtho45(int innerX, int innerY, int x, int y, int z, int current_color, int stride, byte xSize, byte ySize, byte zSize)
        {
            return 4 * (y * 3 + 2)
                 + innerX +
                stride * (zSize * 2 + x * 3 - z * 2 + innerY);
        }

        private static int VoxelToPixelGenericIso(int innerX, int innerY, int x, int y, int z, int stride, int xSize, int ySize, int zSize)
        {
            return (4 * ((x + y) * 2 + 4) + innerX +
                stride * (((xSize + ySize) + zSize * 3) - (Math.Max(xSize, ySize)) - y + x - z * 3 + innerY));
        }

        private static int VoxelToPixelGeneric(int innerX, int innerY, int x, int y, int z, int stride, int zSize)
        {
            return (12 * y + 16 + innerX +
                stride * (zSize * 3 - 1 + 2 + x - z * 3 + innerY));
        }

        private static int VoxelToPixel45(int innerX, int innerY, int x, int y, int z, int current_color, int stride, byte xSize, byte ySize, byte zSize)
        {
            return 8 * (x + y + 1)
                + innerX +
                stride * (((xSize + ySize + zSize) - Math.Max(xSize, ySize)) * 2 + 2 * (x - z - y) + innerY); //(xSize + ySize) * 2
        }

        private static byte ShadeIso(byte[] sprite, int innerX, int lowX, int highY, int highZ)
        {
            //            switch((((7 * innerX) * (3 * innerY) + x + y + z) ^ ((11 * innerX) * (5 * innerY) + x + y + z) ^ (7 - innerX - innerY)) % 16)
            if (highZ > 0)
            {
                if(lowX > 0)
                    return sprite[24 + innerX];
                else
                    return sprite[16 + innerX];
            }
            else
                return sprite[innerX];
        }
        private static byte Shade(byte[] sprite, int innerX, int aboveback, int above, int abovefront)
        {
            //            switch((((7 * innerX) * (3 * innerY) + x + y + z) ^ ((11 * innerX) * (5 * innerY) + x + y + z) ^ (7 - innerX - innerY)) % 16)
            if (above > 0)
                return sprite[12 + innerX];
            else if (aboveback > 0 || abovefront > 0)
                return sprite[12 + innerX];
            else
                return sprite[innerX];
        }

        /// <summary>
        /// Render sloped voxels in a FaceVoxel[,,] to a Bitmap with X assumed to be already rotated. No size limits.
        /// </summary>
        /// <param name="voxels">The result of calling FromMagica.</param>
        /// <param name="xSize">The bounding X size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="ySize">The bounding Y size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="zSize">The bounding Z size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="dir">The Direction enum that specifies which way the X-axis should point.</param>
        /// <returns>A Bitmap view of the voxels in isometric pixel view.</returns>
        private static Bitmap RenderSmartFaces(FaceVoxel[,,] faces, int xSize, int ySize, int zSize, Outlining o, bool shrink)
        {
            int hSize = Math.Max(ySize, xSize);

            xSize = hSize;
            ySize = hSize;

            int bWidth = (xSize + ySize) * 2 + 8;
            int bHeight = (xSize + ySize) + zSize * 3 + 8;
            Bitmap bmp = new Bitmap(bWidth, bHeight, PixelFormat.Format32bppArgb);

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            // int numBytes = bmp.Width * bmp.Height * 3; 
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] argbValues = new byte[numBytes];
            argbValues.Fill<byte>(0);
            byte[] outlineValues = new byte[numBytes];
            outlineValues.Fill<byte>(0);

            int[] zbuffer = new int[numBytes];
            zbuffer.Fill<int>(-999);

            for(int fz = zSize - 1; fz >= 0; fz--)
            {
                for(int fx = xSize - 1; fx >= 0; fx--)
                {
                    for(int fy = 0; fy < ySize; fy++)
                    {
                        if(faces[fx, fy, fz] == null) continue;
                        MagicaVoxelData vx = faces[fx, fy, fz].vox;
                        if(vx.color == 0) continue;
                        Slope slope = faces[fx, fy, fz].slope;
                        int current_color = vx.color - 1;
                        int p;

                        if(renderedFace[current_color][0][3] == 0F)
                            continue;
                        else
                        {
                            int sp = slopes[slope];
                            for(int j = 0; j < 4; j++)
                            {
                                for(int i = 0; i < 16; i++)
                                {
                                    p = VoxelToPixel(i, j, fx, fy, fz, current_color, bmpData.Stride, xSize, ySize, zSize);

                                    if(argbValues[p] == 0)
                                    {

                                        if(renderedFace[current_color][sp][(i | 3) + j * 16] != 0)
                                        {
                                            argbValues[p] = renderedFace[current_color][sp][i + j * 16];
                                            zbuffer[p] = fz + fx - fy;
                                        }
                                        if(outlineValues[p] == 0)
                                            outlineValues[p] = renderedFace[current_color][0][i + 64];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            switch(o)
            {
                case Outlining.Full:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {
                                if(i + 4 >= 0 && i + 4 < numBytes && argbValues[i + 4] == 0) { outlineValues[i + 4] = 255; } else if(i + 4 >= 0 && i + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + 4]) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4 >= 0 && i - 4 < numBytes && argbValues[i - 4] == 0) { outlineValues[i - 4] = 255; } else if(i - 4 >= 0 && i - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - 4]) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && argbValues[i + bmpData.Stride] == 0) { outlineValues[i + bmpData.Stride] = 255; } else if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride]) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && argbValues[i - bmpData.Stride] == 0) { outlineValues[i - bmpData.Stride] = 255; } else if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride]) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && argbValues[i + bmpData.Stride + 4] == 0) { outlineValues[i + bmpData.Stride + 4] = 255; } else if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride + 4]) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && argbValues[i - bmpData.Stride - 4] == 0) { outlineValues[i - bmpData.Stride - 4] = 255; } else if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride - 4]) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && argbValues[i + bmpData.Stride - 4] == 0) { outlineValues[i + bmpData.Stride - 4] = 255; } else if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride - 4]) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && argbValues[i - bmpData.Stride + 4] == 0) { outlineValues[i - bmpData.Stride + 4] = 255; } else if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride + 4]) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if(i + 8 >= 0 && i + 8 < numBytes && argbValues[i + 8] == 0) { outlineValues[i + 8] = 255; } else if(i + 8 >= 0 && i + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + 8]) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if(i - 8 >= 0 && i - 8 < numBytes && argbValues[i - 8] == 0) { outlineValues[i - 8] = 255; } else if(i - 8 >= 0 && i - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - 8]) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && argbValues[i + bmpData.Stride * 2] == 0) { outlineValues[i + bmpData.Stride * 2] = 255; } else if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2]) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && argbValues[i - bmpData.Stride * 2] == 0) { outlineValues[i - bmpData.Stride * 2] = 255; } else if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2]) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && argbValues[i + bmpData.Stride + 8] == 0) { outlineValues[i + bmpData.Stride + 8] = 255; } else if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride + 8]) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && argbValues[i - bmpData.Stride + 8] == 0) { outlineValues[i - bmpData.Stride + 8] = 255; } else if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride + 8]) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && argbValues[i + bmpData.Stride - 8] == 0) { outlineValues[i + bmpData.Stride - 8] = 255; } else if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride - 8]) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && argbValues[i - bmpData.Stride - 8] == 0) { outlineValues[i - bmpData.Stride - 8] = 255; } else if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride - 8]) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && argbValues[i + bmpData.Stride * 2 + 8] == 0) { outlineValues[i + bmpData.Stride * 2 + 8] = 255; } else if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2 + 8]) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && argbValues[i + bmpData.Stride * 2 + 4] == 0) { outlineValues[i + bmpData.Stride * 2 + 4] = 255; } else if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2 + 4]) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && argbValues[i + bmpData.Stride * 2 - 4] == 0) { outlineValues[i + bmpData.Stride * 2 - 4] = 255; } else if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2 - 4]) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && argbValues[i + bmpData.Stride * 2 - 8] == 0) { outlineValues[i + bmpData.Stride * 2 - 8] = 255; } else if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2 - 8]) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && argbValues[i - bmpData.Stride * 2 + 8] == 0) { outlineValues[i - bmpData.Stride * 2 + 8] = 255; } else if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2 + 8]) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && argbValues[i - bmpData.Stride * 2 + 4] == 0) { outlineValues[i - bmpData.Stride * 2 + 4] = 255; } else if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2 + 4]) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && argbValues[i - bmpData.Stride * 2 - 4] == 0) { outlineValues[i - bmpData.Stride * 2 - 4] = 255; } else if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2 - 4]) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && argbValues[i - bmpData.Stride * 2 - 8] == 0) { outlineValues[i - bmpData.Stride * 2 - 8] = 255; } else if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2 - 8]) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
                case Outlining.Light:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(i + 4 >= 0 && i + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + 4]) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4 >= 0 && i - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - 4]) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride]) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride]) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride + 4]) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride - 4]) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride - 4]) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride + 4]) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if(i + 8 >= 0 && i + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + 8]) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if(i - 8 >= 0 && i - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - 8]) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2]) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2]) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                //if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride + 8]) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride + 8]) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride - 8]) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride - 8]) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2 + 8]) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2 + 4]) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2 - 4]) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2 - 8]) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                //if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2 + 8]) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2 + 4]) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2 - 4]) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2 - 8]) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
                case Outlining.Partial:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(i + 4 >= 0 && i + 4 < numBytes && argbValues[i + 4] == 0) { } else if(i + 4 >= 0 && i + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + 4]) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4 >= 0 && i - 4 < numBytes && argbValues[i - 4] == 0) { } else if(i - 4 >= 0 && i - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - 4]) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && argbValues[i + bmpData.Stride] == 0) { } else if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride]) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && argbValues[i - bmpData.Stride] == 0) { } else if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride]) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && argbValues[i + bmpData.Stride + 4] == 0) { } else if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride + 4]) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && argbValues[i - bmpData.Stride - 4] == 0) { } else if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride - 4]) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && argbValues[i + bmpData.Stride - 4] == 0) { } else if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride - 4]) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && argbValues[i - bmpData.Stride + 4] == 0) { } else if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride + 4]) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if(i + 8 >= 0 && i + 8 < numBytes && argbValues[i + 8] == 0) { } else if(i + 8 >= 0 && i + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + 8]) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if(i - 8 >= 0 && i - 8 < numBytes && argbValues[i - 8] == 0) { } else if(i - 8 >= 0 && i - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - 8]) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && argbValues[i + bmpData.Stride * 2] == 0) { } else if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2]) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && argbValues[i - bmpData.Stride * 2] == 0) { } else if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2]) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                //if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && argbValues[i + bmpData.Stride + 8] == 0) { } else if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride + 8]) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && argbValues[i - bmpData.Stride + 8] == 0) { } else if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride + 8]) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && argbValues[i + bmpData.Stride - 8] == 0) { } else if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride - 8]) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && argbValues[i - bmpData.Stride - 8] == 0) { } else if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride - 8]) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && argbValues[i + bmpData.Stride * 2 + 8] == 0) { } else if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2 + 8]) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && argbValues[i + bmpData.Stride * 2 + 4] == 0) { } else if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2 + 4]) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && argbValues[i + bmpData.Stride * 2 - 4] == 0) { } else if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2 - 4]) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && argbValues[i + bmpData.Stride * 2 - 8] == 0) { } else if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i + bmpData.Stride * 2 - 8]) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                //if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && argbValues[i - bmpData.Stride * 2 + 8] == 0) { } else if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2 + 8]) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && argbValues[i - bmpData.Stride * 2 + 4] == 0) { } else if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2 + 4]) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && argbValues[i - bmpData.Stride * 2 - 4] == 0) { } else if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2 - 4]) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && argbValues[i - bmpData.Stride * 2 - 8] == 0) { } else if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 4 > zbuffer[i - bmpData.Stride * 2 - 8]) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
            }

            for(int i = 3; i < numBytes; i += 4)
            {
                if(argbValues[i] > 0)
                    argbValues[i] = 255;
                if(outlineValues[i] == 255) argbValues[i] = 255;
            }

            Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            if(!shrink)
            {
                return bmp;
            }
            else
            {
                Graphics g = Graphics.FromImage(bmp);
                Bitmap b2 = new Bitmap(bWidth / 2, bHeight / 2, PixelFormat.Format32bppArgb);
                Graphics g2 = Graphics.FromImage(b2);
                g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g2.DrawImage(bmp.Clone(new Rectangle(0, 0, bWidth, bHeight), bmp.PixelFormat), 0, 0, bWidth / 2, bHeight / 2);
                g2.Dispose();
                return b2;
            }
        }

        /// <summary>
        /// Render outline chunks in a MagicaVoxelData[] to a Bitmap with a different perspective, with X pointing in any direction.
        /// </summary>
        /// <param name="voxels">The result of calling FromMagica.</param>
        /// <param name="xSize">The bounding X size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="ySize">The bounding Y size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="zSize">The bounding Z size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="dir">The Direction enum that specifies which way the X-axis should point.</param>
        /// <returns>A Bitmap view of the voxels in isometric pixel view.</returns>
        private static Bitmap RenderSmartFacesSmall(FaceVoxel[,,] faces, byte xSize, byte ySize, byte zSize, Outlining o, bool shrink)
        {
            byte tsx = (byte)sizex, tsy = (byte)sizey;
            byte hSize = Math.Max(ySize, xSize);

            xSize = hSize;
            ySize = hSize;

            int bWidth = (xSize + ySize) * 2 + 8;
            int bHeight = (xSize + ySize) + zSize * 4 + 8;
            Bitmap bmp = new Bitmap(bWidth, bHeight, PixelFormat.Format32bppArgb);

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] argbValues = new byte[numBytes];
            argbValues.Fill<byte>(0);
            byte[] outlineValues = new byte[numBytes];
            outlineValues.Fill<byte>(0);

            byte[] editValues = new byte[numBytes];

            int[] zbuffer = new int[numBytes];
            zbuffer.Fill<int>(-999);

            for(int fz = zSize - 1; fz >= 0; fz--)
            {
                for(int fx = xSize - 1; fx >= 0; fx--)
                {
                    for(int fy = 0; fy < ySize; fy++)
                    {
                        if(faces[fx, fy, fz] == null) continue;
                        MagicaVoxelData vx = faces[fx, fy, fz].vox;
                        if(vx.color == 0) continue;
                        Slope slope = faces[fx, fy, fz].slope;
                        int current_color = vx.color - 1;
                        int p = 0;

                        if(renderedFaceSmall[current_color][0][3] == 0F)
                            continue;
                        else
                        {
                            int sp = slopes[slope];

                            for(int j = 0; j < 5; j++)
                            {
                                for(int i = 0; i < 16; i++)
                                {
                                    p = VoxelToPixelSmall(i, j, vx.x, vx.y, vx.z, current_color, bmpData.Stride, xSize, ySize, zSize);

                                    if(argbValues[p] == 0)
                                    {

                                        if(renderedFaceSmall[current_color][sp][((i / 4) * 4 + 3) + j * 16] != 0)
                                        {
                                            argbValues[p] = renderedFaceSmall[current_color][sp][i + j * 16];

                                            zbuffer[p] = vx.z * 3 + (vx.x - vx.y) * 2;
                                        }

                                        if(outlineValues[p] == 0)
                                            outlineValues[p] = renderedFaceSmall[current_color][0][i + 80];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            switch(o)
            {
                case Outlining.Full:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {
                                bool shade = false;

                                if(i + 4 >= 0 && i + 4 < numBytes && argbValues[i + 4] == 0) { outlineValues[i + 4] = 255; } else if(i + 4 >= 0 && i + 4 < numBytes && zbuffer[i] - 12 > zbuffer[i + 4]) { editValues[i + 4] = 255; editValues[i + 4 - 1] = outlineValues[i - 1]; editValues[i + 4 - 2] = outlineValues[i - 2]; editValues[i + 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - 4 >= 0 && i - 4 < numBytes && argbValues[i - 4] == 0) { outlineValues[i - 4] = 255; } else if(i - 4 >= 0 && i - 4 < numBytes && zbuffer[i] - 12 > zbuffer[i - 4]) { editValues[i - 4] = 255; editValues[i - 4 - 1] = outlineValues[i - 1]; editValues[i - 4 - 2] = outlineValues[i - 2]; editValues[i - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && argbValues[i + bmpData.Stride] == 0) { outlineValues[i + bmpData.Stride] = 255; } else if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && zbuffer[i] - 12 > zbuffer[i + bmpData.Stride]) { editValues[i + bmpData.Stride] = 255; editValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && argbValues[i - bmpData.Stride] == 0) { outlineValues[i - bmpData.Stride] = 255; } else if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && zbuffer[i] - 12 > zbuffer[i - bmpData.Stride]) { editValues[i - bmpData.Stride] = 255; editValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && argbValues[i + bmpData.Stride + 4] == 0) { outlineValues[i + bmpData.Stride + 4] = 255; } else if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && zbuffer[i] - 12 > zbuffer[i + bmpData.Stride + 4]) { editValues[i + bmpData.Stride + 4] = 255; editValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && argbValues[i - bmpData.Stride - 4] == 0) { outlineValues[i - bmpData.Stride - 4] = 255; } else if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && zbuffer[i] - 12 > zbuffer[i - bmpData.Stride - 4]) { editValues[i - bmpData.Stride - 4] = 255; editValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && argbValues[i + bmpData.Stride - 4] == 0) { outlineValues[i + bmpData.Stride - 4] = 255; } else if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && zbuffer[i] - 12 > zbuffer[i + bmpData.Stride - 4]) { editValues[i + bmpData.Stride - 4] = 255; editValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && argbValues[i - bmpData.Stride + 4] == 0) { outlineValues[i - bmpData.Stride + 4] = 255; } else if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && zbuffer[i] - 12 > zbuffer[i - bmpData.Stride + 4]) { editValues[i - bmpData.Stride + 4] = 255; editValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; shade = true; }

                                if(i + 8 >= 0 && i + 8 < numBytes && argbValues[i + 8] == 0) { outlineValues[i + 8] = 255; } else if(i + 8 >= 0 && i + 8 < numBytes && zbuffer[i] - 20 > zbuffer[i + 8]) { editValues[i + 8] = 255; editValues[i + 8 - 1] = outlineValues[i - 1]; editValues[i + 8 - 2] = outlineValues[i - 2]; editValues[i + 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - 8 >= 0 && i - 8 < numBytes && argbValues[i - 8] == 0) { outlineValues[i - 8] = 255; } else if(i - 8 >= 0 && i - 8 < numBytes && zbuffer[i] - 20 > zbuffer[i - 8]) { editValues[i - 8] = 255; editValues[i - 8 - 1] = outlineValues[i - 1]; editValues[i - 8 - 2] = outlineValues[i - 2]; editValues[i - 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && argbValues[i + bmpData.Stride * 2] == 0) { outlineValues[i + bmpData.Stride * 2] = 255; } else if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride * 2]) { editValues[i + bmpData.Stride * 2] = 255; editValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && argbValues[i - bmpData.Stride * 2] == 0) { outlineValues[i - bmpData.Stride * 2] = 255; } else if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride * 2]) { editValues[i - bmpData.Stride * 2] = 255; editValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && argbValues[i + bmpData.Stride + 8] == 0) { outlineValues[i + bmpData.Stride + 8] = 255; } else if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride + 8]) { editValues[i + bmpData.Stride + 8] = 255; editValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && argbValues[i - bmpData.Stride + 8] == 0) { outlineValues[i - bmpData.Stride + 8] = 255; } else if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride + 8]) { editValues[i - bmpData.Stride + 8] = 255; editValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && argbValues[i + bmpData.Stride - 8] == 0) { outlineValues[i + bmpData.Stride - 8] = 255; } else if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride - 8]) { editValues[i + bmpData.Stride - 8] = 255; editValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && argbValues[i - bmpData.Stride - 8] == 0) { outlineValues[i - bmpData.Stride - 8] = 255; } else if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride - 8]) { editValues[i - bmpData.Stride - 8] = 255; editValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && argbValues[i + bmpData.Stride * 2 + 8] == 0) { outlineValues[i + bmpData.Stride * 2 + 8] = 255; } else if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride * 2 + 8]) { editValues[i + bmpData.Stride * 2 + 8] = 255; editValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && argbValues[i + bmpData.Stride * 2 + 4] == 0) { outlineValues[i + bmpData.Stride * 2 + 4] = 255; } else if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride * 2 + 4]) { editValues[i + bmpData.Stride * 2 + 4] = 255; editValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && argbValues[i + bmpData.Stride * 2 - 4] == 0) { outlineValues[i + bmpData.Stride * 2 - 4] = 255; } else if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride * 2 - 4]) { editValues[i + bmpData.Stride * 2 - 4] = 255; editValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && argbValues[i + bmpData.Stride * 2 - 8] == 0) { outlineValues[i + bmpData.Stride * 2 - 8] = 255; } else if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride * 2 - 8]) { editValues[i + bmpData.Stride * 2 - 8] = 255; editValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && argbValues[i - bmpData.Stride * 2 + 8] == 0) { outlineValues[i - bmpData.Stride * 2 + 8] = 255; } else if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride * 2 + 8]) { editValues[i - bmpData.Stride * 2 + 8] = 255; editValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && argbValues[i - bmpData.Stride * 2 + 4] == 0) { outlineValues[i - bmpData.Stride * 2 + 4] = 255; } else if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride * 2 + 4]) { editValues[i - bmpData.Stride * 2 + 4] = 255; editValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && argbValues[i - bmpData.Stride * 2 - 4] == 0) { outlineValues[i - bmpData.Stride * 2 - 4] = 255; } else if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride * 2 - 4]) { editValues[i - bmpData.Stride * 2 - 4] = 255; editValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && argbValues[i - bmpData.Stride * 2 - 8] == 0) { outlineValues[i - bmpData.Stride * 2 - 8] = 255; } else if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride * 2 - 8]) { editValues[i - bmpData.Stride * 2 - 8] = 255; editValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; shade = true; }

                                if(shade) editValues[i] = 255;
                            }
                        }
                    }
                    break;
                case Outlining.Light:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {
                                bool shade = false;
                                if(i + 4 >= 0 && i + 4 < numBytes && zbuffer[i] - 12 > zbuffer[i + 4]) { editValues[i + 4] = 255; editValues[i + 4 - 1] = outlineValues[i - 1]; editValues[i + 4 - 2] = outlineValues[i - 2]; editValues[i + 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - 4 >= 0 && i - 4 < numBytes && zbuffer[i] - 12 > zbuffer[i - 4]) { editValues[i - 4] = 255; editValues[i - 4 - 1] = outlineValues[i - 1]; editValues[i - 4 - 2] = outlineValues[i - 2]; editValues[i - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && zbuffer[i] - 12 > zbuffer[i + bmpData.Stride]) { editValues[i + bmpData.Stride] = 255; editValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && zbuffer[i] - 12 > zbuffer[i - bmpData.Stride]) { editValues[i - bmpData.Stride] = 255; editValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && zbuffer[i] - 12 > zbuffer[i + bmpData.Stride + 4]) { editValues[i + bmpData.Stride + 4] = 255; editValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && zbuffer[i] - 12 > zbuffer[i - bmpData.Stride - 4]) { editValues[i - bmpData.Stride - 4] = 255; editValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && zbuffer[i] - 12 > zbuffer[i + bmpData.Stride - 4]) { editValues[i + bmpData.Stride - 4] = 255; editValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && zbuffer[i] - 12 > zbuffer[i - bmpData.Stride + 4]) { editValues[i - bmpData.Stride + 4] = 255; editValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; shade = true; }

                                if(i + 8 >= 0 && i + 8 < numBytes && zbuffer[i] - 20 > zbuffer[i + 8]) { editValues[i + 8] = 255; editValues[i + 8 - 1] = outlineValues[i - 1]; editValues[i + 8 - 2] = outlineValues[i - 2]; editValues[i + 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - 8 >= 0 && i - 8 < numBytes && zbuffer[i] - 20 > zbuffer[i - 8]) { editValues[i - 8] = 255; editValues[i - 8 - 1] = outlineValues[i - 1]; editValues[i - 8 - 2] = outlineValues[i - 2]; editValues[i - 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride * 2]) { editValues[i + bmpData.Stride * 2] = 255; editValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride * 2]) { editValues[i - bmpData.Stride * 2] = 255; editValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride + 8]) { editValues[i + bmpData.Stride + 8] = 255; editValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride + 8]) { editValues[i - bmpData.Stride + 8] = 255; editValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride - 8]) { editValues[i + bmpData.Stride - 8] = 255; editValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride - 8]) { editValues[i - bmpData.Stride - 8] = 255; editValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride * 2 + 4]) { editValues[i + bmpData.Stride * 2 + 4] = 255; editValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride * 2 - 4]) { editValues[i + bmpData.Stride * 2 - 4] = 255; editValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride * 2 + 4]) { editValues[i - bmpData.Stride * 2 + 4] = 255; editValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride * 2 - 4]) { editValues[i - bmpData.Stride * 2 - 4] = 255; editValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; shade = true; }

                                if(shade)
                                {
                                    editValues[i] = 255; editValues[i - 1] = outlineValues[i - 1]; editValues[i - 2] = outlineValues[i - 2]; editValues[i - 3] = outlineValues[i - 3];
                                }
                            }
                        }
                    }
                    break;
                case Outlining.Partial:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {
                                bool shade = false;

                                if(i + 4 >= 0 && i + 4 < numBytes && argbValues[i + 4] == 0) { } else if(i + 4 >= 0 && i + 4 < numBytes && zbuffer[i] - 12 > zbuffer[i + 4]) { editValues[i + 4] = 255; editValues[i + 4 - 1] = outlineValues[i - 1]; editValues[i + 4 - 2] = outlineValues[i - 2]; editValues[i + 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - 4 >= 0 && i - 4 < numBytes && argbValues[i - 4] == 0) { } else if(i - 4 >= 0 && i - 4 < numBytes && zbuffer[i] - 12 > zbuffer[i - 4]) { editValues[i - 4] = 255; editValues[i - 4 - 1] = outlineValues[i - 1]; editValues[i - 4 - 2] = outlineValues[i - 2]; editValues[i - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && argbValues[i + bmpData.Stride] == 0) { } else if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && zbuffer[i] - 12 > zbuffer[i + bmpData.Stride]) { editValues[i + bmpData.Stride] = 255; editValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && argbValues[i - bmpData.Stride] == 0) { } else if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && zbuffer[i] - 12 > zbuffer[i - bmpData.Stride]) { editValues[i - bmpData.Stride] = 255; editValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && argbValues[i + bmpData.Stride + 4] == 0) { } else if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && zbuffer[i] - 12 > zbuffer[i + bmpData.Stride + 4]) { editValues[i + bmpData.Stride + 4] = 255; editValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && argbValues[i - bmpData.Stride - 4] == 0) { } else if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && zbuffer[i] - 12 > zbuffer[i - bmpData.Stride - 4]) { editValues[i - bmpData.Stride - 4] = 255; editValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && argbValues[i + bmpData.Stride - 4] == 0) { } else if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && zbuffer[i] - 12 > zbuffer[i + bmpData.Stride - 4]) { editValues[i + bmpData.Stride - 4] = 255; editValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && argbValues[i - bmpData.Stride + 4] == 0) { } else if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && zbuffer[i] - 12 > zbuffer[i - bmpData.Stride + 4]) { editValues[i - bmpData.Stride + 4] = 255; editValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; shade = true; }

                                if(i + 8 >= 0 && i + 8 < numBytes && argbValues[i + 8] == 0) { } else if(i + 8 >= 0 && i + 8 < numBytes && zbuffer[i] - 20 > zbuffer[i + 8]) { editValues[i + 8] = 255; editValues[i + 8 - 1] = outlineValues[i - 1]; editValues[i + 8 - 2] = outlineValues[i - 2]; editValues[i + 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - 8 >= 0 && i - 8 < numBytes && argbValues[i - 8] == 0) { } else if(i - 8 >= 0 && i - 8 < numBytes && zbuffer[i] - 20 > zbuffer[i - 8]) { editValues[i - 8] = 255; editValues[i - 8 - 1] = outlineValues[i - 1]; editValues[i - 8 - 2] = outlineValues[i - 2]; editValues[i - 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && argbValues[i + bmpData.Stride * 2] == 0) { } else if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride * 2]) { editValues[i + bmpData.Stride * 2] = 255; editValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && argbValues[i - bmpData.Stride * 2] == 0) { } else if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride * 2]) { editValues[i - bmpData.Stride * 2] = 255; editValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && argbValues[i + bmpData.Stride + 8] == 0) { } else if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride + 8]) { editValues[i + bmpData.Stride + 8] = 255; editValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && argbValues[i - bmpData.Stride + 8] == 0) { } else if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride + 8]) { editValues[i - bmpData.Stride + 8] = 255; editValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && argbValues[i + bmpData.Stride - 8] == 0) { } else if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride - 8]) { editValues[i + bmpData.Stride - 8] = 255; editValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && argbValues[i - bmpData.Stride - 8] == 0) { } else if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride - 8]) { editValues[i - bmpData.Stride - 8] = 255; editValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && argbValues[i + bmpData.Stride * 2 + 8] == 0) { } else if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride * 2 + 8]) { editValues[i + bmpData.Stride * 2 + 8] = 255; editValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && argbValues[i + bmpData.Stride * 2 + 4] == 0) { } else if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride * 2 + 4]) { editValues[i + bmpData.Stride * 2 + 4] = 255; editValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && argbValues[i + bmpData.Stride * 2 - 4] == 0) { } else if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride * 2 - 4]) { editValues[i + bmpData.Stride * 2 - 4] = 255; editValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && argbValues[i + bmpData.Stride * 2 - 8] == 0) { } else if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 20 > zbuffer[i + bmpData.Stride * 2 - 8]) { editValues[i + bmpData.Stride * 2 - 8] = 255; editValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; editValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; editValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && argbValues[i - bmpData.Stride * 2 + 8] == 0) { } else if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride * 2 + 8]) { editValues[i - bmpData.Stride * 2 + 8] = 255; editValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && argbValues[i - bmpData.Stride * 2 + 4] == 0) { } else if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride * 2 + 4]) { editValues[i - bmpData.Stride * 2 + 4] = 255; editValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && argbValues[i - bmpData.Stride * 2 - 4] == 0) { } else if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride * 2 - 4]) { editValues[i - bmpData.Stride * 2 - 4] = 255; editValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; shade = true; }
                                if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && argbValues[i - bmpData.Stride * 2 - 8] == 0) { } else if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 20 > zbuffer[i - bmpData.Stride * 2 - 8]) { editValues[i - bmpData.Stride * 2 - 8] = 255; editValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; editValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; editValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; shade = true; }

                                if(shade)
                                {
                                    editValues[i] = 255; editValues[i - 1] = outlineValues[i - 1]; editValues[i - 2] = outlineValues[i - 2]; editValues[i - 3] = outlineValues[i - 3];
                                }

                            }
                        }
                    }
                    break;
            }
            int runningX, runningY;
            byte currentEdit, edit1, edit2, edit3;

            for(int i = 3; i < numBytes; i += 4)
            {
                if(argbValues[i] > 0)
                    argbValues[i] = 255;
                if(outlineValues[i] == 255) argbValues[i] = 255;
                if((currentEdit = editValues[i]) > 0)
                {
                    argbValues[i] = currentEdit;
                    edit1 = argbValues[i - 1] = editValues[i - 1];
                    edit2 = argbValues[i - 2] = editValues[i - 2];
                    edit3 = argbValues[i - 3] = editValues[i - 3];
                    runningX = i % bWidth;
                    runningY = i / bWidth;
                    if(runningX < 1 || runningX >= bWidth - 1 || runningY < 1 || runningY >= bHeight - 1)
                        continue;
                    if(editValues[i - 4 - bmpData.Stride] > 0)
                    {
                        if(argbValues[i - 4] == 0)
                        {
                            argbValues[i - 4] = currentEdit;
                            argbValues[i - 5] = edit1;
                            argbValues[i - 6] = edit2;
                            argbValues[i - 7] = edit3;
                        }
                        if(argbValues[i - bmpData.Stride] == 0)
                        {
                            argbValues[i - bmpData.Stride] = currentEdit;
                            argbValues[i - bmpData.Stride - 1] = edit1;
                            argbValues[i - bmpData.Stride - 2] = edit2;
                            argbValues[i - bmpData.Stride - 3] = edit3;
                        }
                    }
                    if(editValues[i + 4 - bmpData.Stride] > 0)
                    {
                        if(argbValues[i + 4] == 0)
                        {
                            argbValues[i + 4] = currentEdit;
                            argbValues[i + 3] = edit1;
                            argbValues[i + 2] = edit2;
                            argbValues[i + 1] = edit3;
                        }
                        if(argbValues[i - bmpData.Stride] == 0)
                        {
                            argbValues[i - bmpData.Stride] = currentEdit;
                            argbValues[i - bmpData.Stride - 1] = edit1;
                            argbValues[i - bmpData.Stride - 2] = edit2;
                            argbValues[i - bmpData.Stride - 3] = edit3;
                        }
                    }
                    if(editValues[i - 4 + bmpData.Stride] > 0)
                    {
                        if(argbValues[i - 4] == 0)
                        {
                            argbValues[i - 4] = currentEdit;
                            argbValues[i - 5] = edit1;
                            argbValues[i - 6] = edit2;
                            argbValues[i - 7] = edit3;
                        }
                        if(argbValues[i + bmpData.Stride] == 0)
                        {
                            argbValues[i + bmpData.Stride] = currentEdit;
                            argbValues[i + bmpData.Stride - 1] = edit1;
                            argbValues[i + bmpData.Stride - 2] = edit2;
                            argbValues[i + bmpData.Stride - 3] = edit3;
                        }
                    }
                    if(editValues[i + 4 + bmpData.Stride] > 0)
                    {
                        if(argbValues[i + 4] == 0)
                        {
                            argbValues[i + 4] = currentEdit;
                            argbValues[i + 3] = edit1;
                            argbValues[i + 2] = edit2;
                            argbValues[i + 1] = edit3;
                        }
                        if(argbValues[i + bmpData.Stride] == 0)
                        {
                            argbValues[i + bmpData.Stride] = currentEdit;
                            argbValues[i + bmpData.Stride - 1] = edit1;
                            argbValues[i + bmpData.Stride - 2] = edit2;
                            argbValues[i + bmpData.Stride - 3] = edit3;
                        }
                    }
                }

            }

            Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            if(!shrink)
            {
                return bmp;
            }
            else
            {
                Graphics g = Graphics.FromImage(bmp);
                Bitmap b2 = new Bitmap(bWidth / 4, bHeight / 4, PixelFormat.Format32bppArgb);
                Graphics g2 = Graphics.FromImage(b2);
                g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g2.DrawImage(bmp.Clone(new Rectangle(0, 0, bWidth, bHeight), bmp.PixelFormat), 0, 0, bWidth / 4, bHeight / 4);
                g2.Dispose();
                return b2;
            }
        }
        /// <summary>
        /// Render sloped voxels in a FaceVoxel[,,] to a Bitmap with X assumed to be already rotated. No size limits.
        /// </summary>
        /// <param name="voxels">The result of calling FromMagica.</param>
        /// <param name="xSize">The bounding X size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="ySize">The bounding Y size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="zSize">The bounding Z size, in voxels, of the .vox file or of other .vox files that should render at the same pixel size.</param>
        /// <param name="dir">The Direction enum that specifies which way the X-axis should point.</param>
        /// <returns>A Bitmap view of the voxels in isometric pixel view.</returns>
        private static Bitmap RenderSmartFacesOrtho(FaceVoxel[,,] faces, int xSize, int ySize, int zSize, Outlining o, bool shrink)
        {
            int hSize = Math.Max(ySize, xSize);

            int bWidth = hSize * 3 + 8;
            int bHeight = hSize + zSize * 3 + 8;
            Bitmap bmp = new Bitmap(bWidth, bHeight, PixelFormat.Format32bppArgb);

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            // int numBytes = bmp.Width * bmp.Height * 3; 
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] argbValues = new byte[numBytes];
            argbValues.Fill<byte>(0);
            byte[] outlineValues = new byte[numBytes];
            outlineValues.Fill<byte>(0);

            int[] xbuffer = new int[numBytes];
            xbuffer.Fill<int>(-999);
            int[] zbuffer = new int[numBytes];
            zbuffer.Fill<int>(-999);

            for (int fz = zSize - 1; fz >= 0; fz--)
            {
                for (int fx = xSize - 1; fx >= 0; fx--)
                {
                    for (int fy = 0; fy < ySize; fy++)
                    {
                        if (faces[fx, fy, fz] == null) continue;
                        byte color = faces[fx, fy, fz].vox.color;
                        if (color == 0) continue;
                        Slope slope = faces[fx, fy, fz].slope;
                        int current_color = color - 1;
                        int p = 0;

                        if (renderedFaceOrtho[current_color][0][3] == 0F)
                            continue;
                        else
                        {
                            int sp = slopes[slope];
                            for (int j = 0; j < 4; j++)
                            {
                                for (int i = 0; i < 12; i++)
                                {
                                    p = VoxelToPixelGeneric(i, j, fx, fy, fz, bmpData.Stride, zSize);

                                    if (argbValues[p] == 0)
                                    {

                                        if (renderedFaceOrtho[current_color][sp][((i / 4) * 4 + 3) + j * 12] != 0)
                                        {
                                            argbValues[p] = renderedFaceOrtho[current_color][sp][i + j * 12];
                                            zbuffer[p] = fz;
                                            xbuffer[p] = fx;
                                        }
                                        if (outlineValues[p] == 0)
                                            outlineValues[p] = renderedFaceOrtho[current_color][0][i + 48];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            switch (o)
            {
                case Outlining.Full:
                    {
                        for (int i = 3; i < numBytes; i += 4)
                        {
                            if (argbValues[i] > 0)
                            {

                                if (argbValues[i + 4] == 0) { outlineValues[i + 4] = 255; } else if ((zbuffer[i] - zbuffer[i + 4]) > 1 || (xbuffer[i] - xbuffer[i + 4]) > 3) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - 4] == 0) { outlineValues[i - 4] = 255; } else if ((zbuffer[i] - zbuffer[i - 4]) > 1 || (xbuffer[i] - xbuffer[i - 4]) > 3) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride] == 0) { outlineValues[i + bmpData.Stride] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride] == 0) { outlineValues[i - bmpData.Stride] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride + 4] == 0) { outlineValues[i + bmpData.Stride + 4] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride - 4] == 0) { outlineValues[i - bmpData.Stride - 4] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride - 4] == 0) { outlineValues[i + bmpData.Stride - 4] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride + 4] == 0) { outlineValues[i - bmpData.Stride + 4] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if (argbValues[i + 8] == 0) { outlineValues[i + 8] = 255; } else if ((zbuffer[i] - zbuffer[i + 8]) > 1 || (xbuffer[i] - xbuffer[i + 8]) > 3) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - 8] == 0) { outlineValues[i - 8] = 255; } else if ((zbuffer[i] - zbuffer[i - 8]) > 1 || (xbuffer[i] - xbuffer[i - 8]) > 3) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride * 2] == 0) { outlineValues[i + bmpData.Stride * 2] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 3) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride * 2] == 0) { outlineValues[i - bmpData.Stride * 2] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride + 8] == 0) { outlineValues[i + bmpData.Stride + 8] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride + 8] == 0) { outlineValues[i - bmpData.Stride + 8] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride - 8] == 0) { outlineValues[i + bmpData.Stride - 8] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride - 8] == 0) { outlineValues[i - bmpData.Stride - 8] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride * 2 + 8] == 0) { outlineValues[i + bmpData.Stride * 2 + 8] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride * 2 + 4] == 0) { outlineValues[i + bmpData.Stride * 2 + 4] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride * 2 - 4] == 0) { outlineValues[i + bmpData.Stride * 2 - 4] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride * 2 - 8] == 0) { outlineValues[i + bmpData.Stride * 2 - 8] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride * 2 + 8] == 0) { outlineValues[i - bmpData.Stride * 2 + 8] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride * 2 + 4] == 0) { outlineValues[i - bmpData.Stride * 2 + 4] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride * 2 - 4] == 0) { outlineValues[i - bmpData.Stride * 2 - 4] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride * 2 - 8] == 0) { outlineValues[i - bmpData.Stride * 2 - 8] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
                case Outlining.Light:
                    {
                        for (int i = 3; i < numBytes; i += 4)
                        {
                            if (argbValues[i] > 0)
                            {

                                if ((zbuffer[i] - zbuffer[i + 4]) > 1 || (xbuffer[i] - xbuffer[i + 4]) > 4) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if ((zbuffer[i] - zbuffer[i - 4]) > 1 || (xbuffer[i] - xbuffer[i - 4]) > 4) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if ((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if ((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if ((zbuffer[i] - zbuffer[i + 8]) > 1 || (xbuffer[i] - xbuffer[i + 8]) > 4) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if ((zbuffer[i] - zbuffer[i - 8]) > 1 || (xbuffer[i] - xbuffer[i - 8]) > 4) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 4) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }

                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
                case Outlining.Partial:
                    {
                        for (int i = 3; i < numBytes; i += 4)
                        {
                            if (argbValues[i] > 0)
                            {

                                if (argbValues[i + 4] == 0) { } else if ((zbuffer[i] - zbuffer[i + 4]) > 1 || (xbuffer[i] - xbuffer[i + 4]) > 4) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - 4] == 0) { } else if ((zbuffer[i] - zbuffer[i - 4]) > 1 || (xbuffer[i] - xbuffer[i - 4]) > 4) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride + 4] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride - 4] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride - 4] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride + 4] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if (argbValues[i + 8] == 0) { } else if ((zbuffer[i] - zbuffer[i + 8]) > 1 || (xbuffer[i] - xbuffer[i + 8]) > 4) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - 8] == 0) { } else if ((zbuffer[i] - zbuffer[i - 8]) > 1 || (xbuffer[i] - xbuffer[i - 8]) > 4) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride * 2] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 3) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride * 2] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride + 8] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride + 8] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride - 8] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride - 8] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride * 2 + 8] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride * 2 + 4] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride * 2 - 4] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride * 2 - 8] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride * 2 + 8] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride * 2 + 4] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride * 2 - 4] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride * 2 - 8] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
            }

            for (int i = 3; i < numBytes; i += 4)
            {
                if (argbValues[i] > 0)
                    argbValues[i] = 255;
                if (outlineValues[i] == 255) argbValues[i] = 255;
            }

            Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            if (!shrink)
            {
                return bmp;
            }
            else
            {
                Graphics g = Graphics.FromImage(bmp);
                Bitmap b2 = new Bitmap(bWidth / 2, bHeight / 2, PixelFormat.Format32bppArgb);
                Graphics g2 = Graphics.FromImage(b2);
                g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g2.DrawImage(bmp.Clone(new Rectangle(0, 0, bWidth, bHeight), bmp.PixelFormat), 0, 0, bWidth / 2, bHeight / 2);
                g2.Dispose();
                return b2;
            }
        }

        private static Bitmap RenderOrthoMultiSize(byte[,,] colors, int xDim, int yDim, int zDim, Outlining o, int multiplier)
        {
            int rows = (xDim / 2 + zDim) * multiplier + 2, cols = yDim * multiplier + 2;

            Bitmap bmp = new Bitmap(cols, rows, PixelFormat.Format32bppArgb);

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            // int numBytes = bmp.Width * bmp.Height * 3; 
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] argbValues = new byte[numBytes];
            byte[] outlineValues = new byte[numBytes];

            int xSize = xDim * multiplier, ySize = yDim * multiplier, zSize = zDim * multiplier;
            int[] zbuffer = new int[numBytes];
            zbuffer.Fill<int>(-999);
            int[] xbuffer = new int[numBytes];
            xbuffer.Fill<int>(-999);

            for(int fz = zSize - 1; fz >= 0; fz--)
            {
                for(int fx = xSize - 1; fx >= 0; fx--)
                {
                    for(int fy = 0; fy < ySize; fy++)
                    {
                        if(colors[fx, fy, fz] == 0) continue;
                        int current_color = colors[fx, fy, fz] - 1;
                        for(int i = 0; i < 4; i++)
                        {
                            int p = VoxelToPixelGeneric(i, 0, fx, fy, fz, bmpData.Stride, zDim * multiplier);
                            if(argbValues[p] == 0)
                            {
                                if(renderedOrtho[current_color][12] != 0)
                                {
                                    if(fz == zSize - 1 || fx == xSize - 1 || fx == 0)
                                        argbValues[p] = Shade(renderedOrtho[current_color], i, 0, 0, 0);
                                    else
                                        argbValues[p] = Shade(renderedOrtho[current_color], i, colors[fx - 1, fy, fz + 1], colors[fx, fy, fz + 1], colors[fx + 1, fy, fz + 1]);

                                    zbuffer[p] = fz;
                                    xbuffer[p] = fx;

                                    if(outlineValues[p] == 0)
                                        outlineValues[p] = renderedOrtho[current_color][i + 48];
                                }
                            }

                        }
                    }
                }
            }

            int[] xmods = new int[] { -1, 0, 1, -1, 0, 1, -1, 0, 1 }, ymods = new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 };
            
            switch(o)
            {
                case Outlining.Full:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {
                                if(i + 4                  < numBytes) if(argbValues[i + 4                 ] == 0) { outlineValues[i + 4] = 255; } else if((zbuffer[i] - zbuffer[i + 4]) > 1 || (xbuffer[i] - xbuffer[i + 4]) > 3) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4                  > 0       ) if(argbValues[i - 4                 ] == 0) { outlineValues[i - 4] = 255; } else if((zbuffer[i] - zbuffer[i - 4]) > 1 || (xbuffer[i] - xbuffer[i - 4]) > 3) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride     < numBytes) if(argbValues[i + bmpData.Stride    ] == 0) { outlineValues[i + bmpData.Stride] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride     > 0       ) if(argbValues[i - bmpData.Stride    ] == 0) { outlineValues[i - bmpData.Stride] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 < numBytes) if(argbValues[i + bmpData.Stride + 4] == 0) { outlineValues[i + bmpData.Stride + 4] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 > 0       ) if(argbValues[i - bmpData.Stride - 4] == 0) { outlineValues[i - bmpData.Stride - 4] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 < numBytes) if(argbValues[i + bmpData.Stride - 4] == 0) { outlineValues[i + bmpData.Stride - 4] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 < numBytes) if(argbValues[i - bmpData.Stride + 4] == 0) { outlineValues[i - bmpData.Stride + 4] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                /*
                                if(argbValues[i + 8] == 0) { outlineValues[i + 8] = 255; } else if((zbuffer[i] - zbuffer[i + 8]) > 1 || (xbuffer[i] - xbuffer[i + 8]) > 3) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - 8] == 0) { outlineValues[i - 8] = 255; } else if((zbuffer[i] - zbuffer[i - 8]) > 1 || (xbuffer[i] - xbuffer[i - 8]) > 3) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2] == 0) { outlineValues[i + bmpData.Stride * 2] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 3) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2] == 0) { outlineValues[i - bmpData.Stride * 2] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride + 8] == 0) { outlineValues[i + bmpData.Stride + 8] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride + 8] == 0) { outlineValues[i - bmpData.Stride + 8] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride - 8] == 0) { outlineValues[i + bmpData.Stride - 8] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride - 8] == 0) { outlineValues[i - bmpData.Stride - 8] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 + 8] == 0) { outlineValues[i + bmpData.Stride * 2 + 8] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 + 4] == 0) { outlineValues[i + bmpData.Stride * 2 + 4] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 - 4] == 0) { outlineValues[i + bmpData.Stride * 2 - 4] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 - 8] == 0) { outlineValues[i + bmpData.Stride * 2 - 8] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 + 8] == 0) { outlineValues[i - bmpData.Stride * 2 + 8] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 + 4] == 0) { outlineValues[i - bmpData.Stride * 2 + 4] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 - 4] == 0) { outlineValues[i - bmpData.Stride * 2 - 4] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 - 8] == 0) { outlineValues[i - bmpData.Stride * 2 - 8] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                */
                            }
                        }
                    }
                    break;
                case Outlining.Light:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(i + 4                  < numBytes) if((zbuffer[i] - zbuffer[i - 4                 ]) > 1 || (xbuffer[i] - xbuffer[i - 4]) > 3) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4                  > 0       ) if((zbuffer[i] - zbuffer[i + 4                 ]) > 1 || (xbuffer[i] - xbuffer[i + 4]) > 3) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride     < numBytes) if((zbuffer[i] - zbuffer[i + bmpData.Stride    ]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride     > 0       ) if((zbuffer[i] - zbuffer[i - bmpData.Stride    ]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 < numBytes) if((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 > 0       ) if((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 < numBytes) if((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 < numBytes) if((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                /*
                                if((zbuffer[i] - zbuffer[i + 8]) > 1 || (xbuffer[i] - xbuffer[i + 8]) > 3) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - 8]) > 1 || (xbuffer[i] - xbuffer[i - 8]) > 3) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 4) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                */
                            }
                        }
                    }
                    break;
                case Outlining.Partial:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(i + 4                  < numBytes) if(argbValues[i + 4                 ] == 0) { } else if((zbuffer[i] - zbuffer[i + 4]) > 1 || (xbuffer[i] - xbuffer[i + 4]) > 3) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4                  > 0       ) if(argbValues[i - 4                 ] == 0) { } else if((zbuffer[i] - zbuffer[i - 4]) > 1 || (xbuffer[i] - xbuffer[i - 4]) > 3) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride     < numBytes) if(argbValues[i + bmpData.Stride    ] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride     > 0       ) if(argbValues[i - bmpData.Stride    ] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 < numBytes) if(argbValues[i + bmpData.Stride + 4] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 > 0       ) if(argbValues[i - bmpData.Stride - 4] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 < numBytes) if(argbValues[i + bmpData.Stride - 4] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 < numBytes) if(argbValues[i - bmpData.Stride + 4] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                /*
                                if(argbValues[i + 8] == 0) { } else if((zbuffer[i] - zbuffer[i + 8]) > 1 || (xbuffer[i] - xbuffer[i + 8]) > 3) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - 8] == 0) { } else if((zbuffer[i] - zbuffer[i - 8]) > 1 || (xbuffer[i] - xbuffer[i - 8]) > 3) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 3) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride + 8] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride + 8] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride - 8] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride - 8] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 + 8] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 + 4] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 - 4] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 - 8] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 + 8] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 + 4] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 - 4] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 - 8] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                */
                            }
                        }
                    }
                    break;
            }

            for(int i = 3; i < numBytes; i += 4)
            {
                if(argbValues[i] > 0) // && argbValues[i] <= 255 * flat_alpha
                    argbValues[i] = 255;
                if(outlineValues[i] == 255) argbValues[i] = 255;
            }

            Marshal.Copy(argbValues, 0, ptr, numBytes);
            
            bmp.UnlockBits(bmpData);
            return bmp;

        }




        private static Bitmap renderSmart(MagicaVoxelData[] voxels, byte xSize, byte ySize, byte zSize, Direction dir, Outlining o, bool shrink)
        {

            byte tsx = (byte)Math.Max(sizey, sizex), tsy = tsx;
            byte hSize = Math.Max(ySize, xSize);

            xSize = hSize;
            ySize = hSize;

            int bWidth = (xSize + ySize) * 2 + 8;
            int bHeight = (xSize + ySize) + zSize * 3 + 8;
            Bitmap bmp = new Bitmap(bWidth, bHeight, PixelFormat.Format32bppArgb);

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            // int numBytes = bmp.Width * bmp.Height * 3; 
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] argbValues = new byte[numBytes];
            argbValues.Fill<byte>(0);
            byte[] outlineValues = new byte[numBytes];
            outlineValues.Fill<byte>(0);
            byte[] bareValues = new byte[numBytes];
            bareValues.Fill<byte>(0);
            MagicaVoxelData[] vls = new MagicaVoxelData[voxels.Length];

            switch(dir)
            {
                case Direction.SE:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            vls[i].x = (byte)(voxels[i].x + (hSize - tsx) / 2);
                            vls[i].y = (byte)(voxels[i].y + (hSize - tsy) / 2);
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
                case Direction.SW:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            byte tempX = (byte)(voxels[i].x + ((hSize - tsx) / 2) - (xSize / 2));
                            byte tempY = (byte)(voxels[i].y + ((hSize - tsy) / 2) - (ySize / 2));
                            vls[i].x = (byte)((tempY) + (ySize / 2));
                            vls[i].y = (byte)((tempX * -1) + (xSize / 2) - 1);
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
                case Direction.NW:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            byte tempX = (byte)(voxels[i].x + ((hSize - tsx) / 2) - (xSize / 2));
                            byte tempY = (byte)(voxels[i].y + ((hSize - tsy) / 2) - (ySize / 2));
                            vls[i].x = (byte)((tempX * -1) + (xSize / 2) - 1);
                            vls[i].y = (byte)((tempY * -1) + (ySize / 2) - 1);
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
                case Direction.NE:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            byte tempX = (byte)(voxels[i].x + ((hSize - tsx) / 2) - (xSize / 2));
                            byte tempY = (byte)(voxels[i].y + ((hSize - tsy) / 2) - (ySize / 2));
                            vls[i].x = (byte)((tempY * -1) + (ySize / 2) - 1);
                            vls[i].y = (byte)(tempX + (xSize / 2));
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
            }
            int[] zbuffer = new int[numBytes];
            zbuffer.Fill<int>(-999);

            foreach(MagicaVoxelData vx in vls.OrderByDescending(v => v.x * ySize * 2 - v.y + v.z * ySize * xSize * 4)) //voxelData[i].x + voxelData[i].z * 32 + voxelData[i].y * 32 * 128
            {
                int p = 0;
                int mod_color = vx.color - 1;
                /*
                colorMatrix = new ColorMatrix(new float[][]{ 
   new float[] {colors[currentColor][0],  0,  0,  0, 0},
   new float[] {0,  colors[currentColor][1],  0,  0, 0},
   new float[] {0,  0,  colors[currentColor][2],  0, 0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0, 0, 0, 0, 1F}});

                imageAttributes.SetColorMatrix(
                   colorMatrix,
                   ColorMatrixFlag.Default,
                   ColorAdjustType.Bitmap);*/



                for(int j = 0; j < 4; j++)
                {
                    for(int i = 0; i < 16; i++)
                    {
                        p = VoxelToPixel(i, j, vx.x, vx.y, vx.z, mod_color, bmpData.Stride, xSize, ySize, zSize);

                        if(argbValues[p] == 0)
                        {
                            zbuffer[p] = vx.z + vx.x - vx.y;
                            argbValues[p] = rendered[mod_color][i + j * 16];
                            if(outlineValues[p] == 0)
                                outlineValues[p] = rendered[mod_color][i + 64]; //(argbValues[p] * 1.2 + 2 < 255) ? (byte)(argbValues[p] * 1.2 + 2) : (byte)255;

                        }
                    }
                }

            }
            switch(o)
            {
                case Outlining.Full:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(i + 4 >= 0 && i + 4 < numBytes                                          ) if(argbValues[i + 4] == 0                     ) { outlineValues[i + 4] = 255; } else if(zbuffer[i] - 2 > zbuffer[i + 4]) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4 >= 0 && i - 4 < numBytes                                          ) if(argbValues[i - 4] == 0                     ) { outlineValues[i - 4] = 255; } else if(zbuffer[i] - 2 > zbuffer[i - 4]) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes                ) if(argbValues[i + bmpData.Stride] == 0        ) { outlineValues[i + bmpData.Stride] = 255; } else if(zbuffer[i] - 2 > zbuffer[i + bmpData.Stride]) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes                ) if(argbValues[i - bmpData.Stride] == 0        ) { outlineValues[i - bmpData.Stride] = 255; } else if(zbuffer[i] - 2 > zbuffer[i - bmpData.Stride]) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes        ) if(argbValues[i + bmpData.Stride + 4] == 0    ) { outlineValues[i + bmpData.Stride + 4] = 255; } else if(zbuffer[i] - 2 > zbuffer[i + bmpData.Stride + 4]) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes        ) if(argbValues[i - bmpData.Stride - 4] == 0    ) { outlineValues[i - bmpData.Stride - 4] = 255; } else if(zbuffer[i] - 2 > zbuffer[i - bmpData.Stride - 4]) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes        ) if(argbValues[i + bmpData.Stride - 4] == 0    ) { outlineValues[i + bmpData.Stride - 4] = 255; } else if(zbuffer[i] - 2 > zbuffer[i + bmpData.Stride - 4]) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes        ) if(argbValues[i - bmpData.Stride + 4] == 0    ) { outlineValues[i - bmpData.Stride + 4] = 255; } else if(zbuffer[i] - 2 > zbuffer[i - bmpData.Stride + 4]) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if(i + 8 >= 0 && i + 8 < numBytes                                          ) if(argbValues[i + 8] == 0                     ) { outlineValues[i + 8] = 255; } else if(zbuffer[i] - 2 > zbuffer[i + 8]) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if(i - 8 >= 0 && i - 8 < numBytes                                          ) if(argbValues[i - 8] == 0                     ) { outlineValues[i - 8] = 255; } else if(zbuffer[i] - 2 > zbuffer[i - 8]) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes        ) if(argbValues[i + bmpData.Stride * 2] == 0    ) { outlineValues[i + bmpData.Stride * 2] = 255; } else if(zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2]) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes        ) if(argbValues[i - bmpData.Stride * 2] == 0    ) { outlineValues[i - bmpData.Stride * 2] = 255; } else if(zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2]) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes        ) if(argbValues[i + bmpData.Stride + 8] == 0    ) { outlineValues[i + bmpData.Stride + 8] = 255; } else if(zbuffer[i] - 2 > zbuffer[i + bmpData.Stride + 8]) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes        ) if(argbValues[i - bmpData.Stride + 8] == 0    ) { outlineValues[i - bmpData.Stride + 8] = 255; } else if(zbuffer[i] - 2 > zbuffer[i - bmpData.Stride + 8]) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes        ) if(argbValues[i + bmpData.Stride - 8] == 0    ) { outlineValues[i + bmpData.Stride - 8] = 255; } else if(zbuffer[i] - 2 > zbuffer[i + bmpData.Stride - 8]) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes        ) if(argbValues[i - bmpData.Stride - 8] == 0    ) { outlineValues[i - bmpData.Stride - 8] = 255; } else if(zbuffer[i] - 2 > zbuffer[i - bmpData.Stride - 8]) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes) if(argbValues[i + bmpData.Stride * 2 + 8] == 0) { outlineValues[i + bmpData.Stride * 2 + 8] = 255; } else if(zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 + 8]) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes) if(argbValues[i + bmpData.Stride * 2 + 4] == 0) { outlineValues[i + bmpData.Stride * 2 + 4] = 255; } else if(zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 + 4]) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes) if(argbValues[i + bmpData.Stride * 2 - 4] == 0) { outlineValues[i + bmpData.Stride * 2 - 4] = 255; } else if(zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 - 4]) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes) if(argbValues[i + bmpData.Stride * 2 - 8] == 0) { outlineValues[i + bmpData.Stride * 2 - 8] = 255; } else if(zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 - 8]) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes) if(argbValues[i - bmpData.Stride * 2 + 8] == 0) { outlineValues[i - bmpData.Stride * 2 + 8] = 255; } else if(zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 + 8]) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes) if(argbValues[i - bmpData.Stride * 2 + 4] == 0) { outlineValues[i - bmpData.Stride * 2 + 4] = 255; } else if(zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 + 4]) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes) if(argbValues[i - bmpData.Stride * 2 - 4] == 0) { outlineValues[i - bmpData.Stride * 2 - 4] = 255; } else if(zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 - 4]) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes) if(argbValues[i - bmpData.Stride * 2 - 8] == 0) { outlineValues[i - bmpData.Stride * 2 - 8] = 255; } else if(zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 - 8]) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
                case Outlining.Light:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(i + 4 >= 0 && i + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + 4]) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4 >= 0 && i - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - 4]) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride]) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride]) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride + 4]) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride - 4]) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride - 4]) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride + 4]) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if(i + 8 >= 0 && i + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + 8]) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if(i - 8 >= 0 && i - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - 8]) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2]) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2]) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride + 8]) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride + 8]) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride - 8]) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride - 8]) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 + 8]) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 + 4]) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 - 4]) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 - 8]) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 + 8]) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 + 4]) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 - 4]) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 - 8]) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
                case Outlining.Partial:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(i + 4 >= 0 && i + 4 < numBytes && argbValues[i + 4] == 0) { } else if(i + 4 >= 0 && i + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + 4]) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4 >= 0 && i - 4 < numBytes && argbValues[i - 4] == 0) { } else if(i - 4 >= 0 && i - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - 4]) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && argbValues[i + bmpData.Stride] == 0) { } else if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride]) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && argbValues[i - bmpData.Stride] == 0) { } else if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride]) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && argbValues[i + bmpData.Stride + 4] == 0) { } else if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride + 4]) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && argbValues[i - bmpData.Stride - 4] == 0) { } else if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride - 4]) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && argbValues[i + bmpData.Stride - 4] == 0) { } else if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride - 4]) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && argbValues[i - bmpData.Stride + 4] == 0) { } else if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride + 4]) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if(i + 8 >= 0 && i + 8 < numBytes && argbValues[i + 8] == 0) { } else if(i + 8 >= 0 && i + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + 8]) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if(i - 8 >= 0 && i - 8 < numBytes && argbValues[i - 8] == 0) { } else if(i - 8 >= 0 && i - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - 8]) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && argbValues[i + bmpData.Stride * 2] == 0) { } else if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2]) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && argbValues[i - bmpData.Stride * 2] == 0) { } else if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2]) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && argbValues[i + bmpData.Stride + 8] == 0) { } else if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride + 8]) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && argbValues[i - bmpData.Stride + 8] == 0) { } else if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride + 8]) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && argbValues[i + bmpData.Stride - 8] == 0) { } else if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride - 8]) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && argbValues[i - bmpData.Stride - 8] == 0) { } else if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride - 8]) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && argbValues[i + bmpData.Stride * 2 + 8] == 0) { } else if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 + 8]) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && argbValues[i + bmpData.Stride * 2 + 4] == 0) { } else if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 + 4]) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && argbValues[i + bmpData.Stride * 2 - 4] == 0) { } else if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 - 4]) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && argbValues[i + bmpData.Stride * 2 - 8] == 0) { } else if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 - 8]) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && argbValues[i - bmpData.Stride * 2 + 8] == 0) { } else if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 + 8]) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && argbValues[i - bmpData.Stride * 2 + 4] == 0) { } else if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 + 4]) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && argbValues[i - bmpData.Stride * 2 - 4] == 0) { } else if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 - 4]) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && argbValues[i - bmpData.Stride * 2 - 8] == 0) { } else if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 - 8]) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
            }

            for(int i = 3; i < numBytes; i += 4)
            {
                if(argbValues[i] > 0) // && argbValues[i] <= 255 * flat_alpha
                    argbValues[i] = 255;
                if(outlineValues[i] == 255) argbValues[i] = 255;
            }

            Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            if(!shrink)
            {
                return bmp;
            }
            else
            {
                Graphics g = Graphics.FromImage(bmp);
                Bitmap b2 = new Bitmap(bWidth / 2, bHeight / 2, PixelFormat.Format32bppArgb);
                Graphics g2 = Graphics.FromImage(b2);
                g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g2.DrawImage(bmp.Clone(new Rectangle(0, 0, bWidth, bHeight), bmp.PixelFormat), 0, 0, bWidth / 2, bHeight / 2);
                g2.Dispose();
                return b2;
            }
        }

        private static Bitmap renderSmartOrtho(MagicaVoxelData[] voxels, byte xSize, byte ySize, byte zSize, OrthoDirection dir, Outlining o, bool shrink)
        {
            //byte tsx = (byte)sizex, tsy = (byte)sizey;
            byte tsx = (byte)Math.Max(sizey, sizex), tsy = tsx;
            byte hSize = Math.Max(ySize, xSize);

            xSize = hSize;
            ySize = hSize;

            int bWidth = ySize * 3 + 8;
            int bHeight = xSize + zSize * 3 + 8;
            Bitmap bmp = new Bitmap(bWidth, bHeight, PixelFormat.Format32bppArgb);

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            // int numBytes = bmp.Width * bmp.Height * 3; 
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] argbValues = new byte[numBytes];
            argbValues.Fill<byte>(0);
            byte[] outlineValues = new byte[numBytes];
            outlineValues.Fill<byte>(0);
            MagicaVoxelData[] vls = new MagicaVoxelData[voxels.Length];

            switch(dir)
            {
                case OrthoDirection.S:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            vls[i].x = (byte)(voxels[i].x + (hSize - tsx) / 2);
                            vls[i].y = (byte)(voxels[i].y + (hSize - tsy) / 2);
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
                case OrthoDirection.W:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            byte tempX = (byte)(voxels[i].x + ((hSize - tsx) / 2) - (xSize / 2));
                            byte tempY = (byte)(voxels[i].y + ((hSize - tsy) / 2) - (ySize / 2));
                            vls[i].x = (byte)((tempY) + (ySize / 2));
                            vls[i].y = (byte)((tempX * -1) + (xSize / 2) - 1);
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
                case OrthoDirection.N:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            byte tempX = (byte)(voxels[i].x + ((hSize - tsx) / 2) - (xSize / 2));
                            byte tempY = (byte)(voxels[i].y + ((hSize - tsy) / 2) - (ySize / 2));
                            vls[i].x = (byte)((tempX * -1) + (xSize / 2) - 1);
                            vls[i].y = (byte)((tempY * -1) + (ySize / 2) - 1);
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
                case OrthoDirection.E:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            byte tempX = (byte)(voxels[i].x + ((hSize - tsx) / 2) - (xSize / 2));
                            byte tempY = (byte)(voxels[i].y + ((hSize - tsy) / 2) - (ySize / 2));
                            vls[i].x = (byte)((tempY * -1) + (ySize / 2) - 1);
                            vls[i].y = (byte)(tempX + (xSize / 2));
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
            }

            int[] xbuffer = new int[numBytes];
            xbuffer.Fill<int>(-999);
            int[] zbuffer = new int[numBytes];
            zbuffer.Fill<int>(-999);

            foreach(MagicaVoxelData vx in vls.OrderByDescending(v => v.x * ySize * 4 + v.y + v.z * ySize * xSize * 4)) //voxelData[i].x + voxelData[i].z * 32 + voxelData[i].y * 32 * 128
            {

                int p = 0;
                int mod_color = vx.color - 1;

                for(int j = 0; j < 4; j++)
                {
                    for(int i = 0; i < 12; i++)
                    {
                        p = VoxelToPixelOrtho(i, j, vx.x, vx.y, vx.z, mod_color, bmpData.Stride, xSize, ySize, zSize);
                        if(argbValues[p] == 0)
                        {
                            argbValues[p] = renderedOrtho[mod_color][i + j * 12];
                            zbuffer[p] = vx.z;
                            xbuffer[p] = vx.x;
                            if(outlineValues[p] == 0)
                                outlineValues[p] = renderedOrtho[mod_color][i + 48];
                        }
                    }
                }

            }

            switch(o)
            {
                case Outlining.Full:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(argbValues[i + 4] == 0) { outlineValues[i + 4] = 255; } else if((zbuffer[i] - zbuffer[i + 4]) > 1 || (xbuffer[i] - xbuffer[i + 4]) > 3) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - 4] == 0) { outlineValues[i - 4] = 255; } else if((zbuffer[i] - zbuffer[i - 4]) > 1 || (xbuffer[i] - xbuffer[i - 4]) > 3) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride] == 0) { outlineValues[i + bmpData.Stride] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride] == 0) { outlineValues[i - bmpData.Stride] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride + 4] == 0) { outlineValues[i + bmpData.Stride + 4] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride - 4] == 0) { outlineValues[i - bmpData.Stride - 4] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride - 4] == 0) { outlineValues[i + bmpData.Stride - 4] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride + 4] == 0) { outlineValues[i - bmpData.Stride + 4] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if(argbValues[i + 8] == 0) { outlineValues[i + 8] = 255; } else if((zbuffer[i] - zbuffer[i + 8]) > 1 || (xbuffer[i] - xbuffer[i + 8]) > 3) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - 8] == 0) { outlineValues[i - 8] = 255; } else if((zbuffer[i] - zbuffer[i - 8]) > 1 || (xbuffer[i] - xbuffer[i - 8]) > 3) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2] == 0) { outlineValues[i + bmpData.Stride * 2] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 3) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2] == 0) { outlineValues[i - bmpData.Stride * 2] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride + 8] == 0) { outlineValues[i + bmpData.Stride + 8] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride + 8] == 0) { outlineValues[i - bmpData.Stride + 8] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride - 8] == 0) { outlineValues[i + bmpData.Stride - 8] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride - 8] == 0) { outlineValues[i - bmpData.Stride - 8] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 + 8] == 0) { outlineValues[i + bmpData.Stride * 2 + 8] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 + 4] == 0) { outlineValues[i + bmpData.Stride * 2 + 4] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 - 4] == 0) { outlineValues[i + bmpData.Stride * 2 - 4] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 - 8] == 0) { outlineValues[i + bmpData.Stride * 2 - 8] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 + 8] == 0) { outlineValues[i - bmpData.Stride * 2 + 8] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 + 4] == 0) { outlineValues[i - bmpData.Stride * 2 + 4] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 - 4] == 0) { outlineValues[i - bmpData.Stride * 2 - 4] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 - 8] == 0) { outlineValues[i - bmpData.Stride * 2 - 8] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
                case Outlining.Light:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if((zbuffer[i] - zbuffer[i + 4]) > 1 || (xbuffer[i] - xbuffer[i + 4]) > 3) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - 4]) > 1 || (xbuffer[i] - xbuffer[i - 4]) > 3) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if((zbuffer[i] - zbuffer[i + 8]) > 1 || (xbuffer[i] - xbuffer[i + 8]) > 3) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - 8]) > 1 || (xbuffer[i] - xbuffer[i - 8]) > 3) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 4) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
                case Outlining.Partial:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(argbValues[i + 4] == 0) { } else if((zbuffer[i] - zbuffer[i + 4]) > 1 || (xbuffer[i] - xbuffer[i + 4]) > 3) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - 4] == 0) { } else if((zbuffer[i] - zbuffer[i - 4]) > 1 || (xbuffer[i] - xbuffer[i - 4]) > 3) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride + 4] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride - 4] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride - 4] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride + 4] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if(argbValues[i + 8] == 0) { } else if((zbuffer[i] - zbuffer[i + 8]) > 1 || (xbuffer[i] - xbuffer[i + 8]) > 3) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - 8] == 0) { } else if((zbuffer[i] - zbuffer[i - 8]) > 1 || (xbuffer[i] - xbuffer[i - 8]) > 3) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 3) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride + 8] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride + 8] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride - 8] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride - 8] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 + 8] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 + 4] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 - 4] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i + bmpData.Stride * 2 - 8] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 + 8] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 + 4] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 - 4] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if(argbValues[i - bmpData.Stride * 2 - 8] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
            }

            for(int i = 3; i < numBytes; i += 4)
            {
                if(argbValues[i] > 0) // && argbValues[i] <= 255 * flat_alpha
                    argbValues[i] = 255;
                if(outlineValues[i] == 255) argbValues[i] = 255;
            }

            Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            if(!shrink)
            {
                return bmp;
            }
            else
            {
                Graphics g = Graphics.FromImage(bmp);
                Bitmap b2 = new Bitmap(bWidth / 2, bHeight / 2, PixelFormat.Format32bppArgb);
                Graphics g2 = Graphics.FromImage(b2);
                g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g2.DrawImage(bmp.Clone(new Rectangle(0, 0, bWidth, bHeight), bmp.PixelFormat), 0, 0, bWidth / 2, bHeight / 2);
                g2.Dispose();
                return b2;
            }
        }


        private static Bitmap renderSmart45(MagicaVoxelData[] voxels, byte xSize, byte ySize, byte zSize, Direction dir, Outlining o, bool shrink)
        {

            //byte tsx = (byte)sizex, tsy = (byte)sizey;
            byte tsx = (byte)Math.Max(sizey, sizex), tsy = tsx;
            byte hSize = Math.Max(ySize, xSize);

            xSize = hSize;
            ySize = hSize;

            int bWidth = hSize * 4 + 4;
            int bHeight = hSize * 4 + zSize * 2 + 4;
            Bitmap bmp = new Bitmap(bWidth, bHeight, PixelFormat.Format32bppArgb);

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            // int numBytes = bmp.Width * bmp.Height * 3; 
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] argbValues = new byte[numBytes];
            argbValues.Fill<byte>(0);
            byte[] outlineValues = new byte[numBytes];
            outlineValues.Fill<byte>(0);
            byte[] bareValues = new byte[numBytes];
            bareValues.Fill<byte>(0);
            MagicaVoxelData[] vls = new MagicaVoxelData[voxels.Length];

            switch(dir)
            {
                case Direction.SE:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            vls[i].x = (byte)(voxels[i].x + (hSize - tsx >> 1));
                            vls[i].y = (byte)(voxels[i].y + (hSize - tsy >> 1));
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
                case Direction.SW:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            byte tempX = (byte)(voxels[i].x + (hSize - tsx >> 1) - (xSize >> 1));
                            byte tempY = (byte)(voxels[i].y + (hSize - tsy >> 1) - (ySize >> 1));
                            vls[i].x = (byte)((tempY) + (ySize >> 1));
                            vls[i].y = (byte)((-1 - tempX) + (xSize >> 1));
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
                case Direction.NW:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            byte tempX = (byte)(voxels[i].x + (hSize - tsx >> 1) - (xSize >> 1));
                            byte tempY = (byte)(voxels[i].y + (hSize - tsy >> 1) - (ySize >> 1));
                            vls[i].x = (byte)((-1 - tempX) + (xSize >> 1));
                            vls[i].y = (byte)((-1 - tempY) + (ySize >> 1));
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
                case Direction.NE:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            byte tempX = (byte)(voxels[i].x + (hSize - tsx >> 1) - (xSize >> 1));
                            byte tempY = (byte)(voxels[i].y + (hSize - tsy >> 1) - (ySize >> 1));
                            vls[i].x = (byte)((-1 - tempY) + (ySize >> 1));
                            vls[i].y = (byte)(tempX + (xSize >> 1));
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
            }
            int[] zbuffer = new int[numBytes];
            zbuffer.Fill<int>(-999);

            foreach(MagicaVoxelData vx in vls.OrderByDescending(v => v.x * xSize * 2 - v.y + v.z * xSize * ySize * 4)) //voxelData[i].x + voxelData[i].z * 32 + voxelData[i].y * 32 * 128
            {
                int p = 0;
                int mod_color = vx.color - 1;
                /*
                colorMatrix = new ColorMatrix(new float[][]{ 
   new float[] {colors[currentColor][0],  0,  0,  0, 0},
   new float[] {0,  colors[currentColor][1],  0,  0, 0},
   new float[] {0,  0,  colors[currentColor][2],  0, 0},
   new float[] {0,  0,  0,  1F, 0},
   new float[] {0, 0, 0, 0, 1F}});

                imageAttributes.SetColorMatrix(
                   colorMatrix,
                   ColorMatrixFlag.Default,
                   ColorAdjustType.Bitmap);*/



                for(int j = 0; j < 4; j++)
                {
                    for(int i = 0; i < 16; i++)
                    {
                        p = VoxelToPixel45(i, j, vx.x, vx.y, vx.z, mod_color, bmpData.Stride, xSize, ySize, zSize);

                        if(argbValues[p] == 0)
                        {
                            zbuffer[p] = vx.z + (vx.x - vx.y) * 2;
                            argbValues[p] = rendered45[mod_color][((i & 3)|4*(i>>3)) + (j>>1) * 8];
                            if(outlineValues[p] == 0)
                                outlineValues[p] = rendered45[mod_color][(i & 3) + 16]; //(argbValues[p] * 1.2 + 2 < 255) ? (byte)(argbValues[p] * 1.2 + 2) : (byte)255;

                        }
                    }
                }

            }
            switch(o)
            {
                case Outlining.Full:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(i + 4 >= 0 && i + 4 < numBytes                                              ) if(argbValues[i + 4] == 0) { outlineValues[i + 4] = 255; } else if(i + 4 >= 0 && i + 4 < numBytes && zbuffer[i] - 7 > zbuffer[i + 4]) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4 >= 0 && i - 4 < numBytes                                              ) if(argbValues[i - 4] == 0) { outlineValues[i - 4] = 255; } else if(i - 4 >= 0 && i - 4 < numBytes && zbuffer[i] - 7 > zbuffer[i - 4]) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes                    ) if(argbValues[i + bmpData.Stride] == 0) { outlineValues[i + bmpData.Stride] = 255; } else if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride]) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes                    ) if(argbValues[i - bmpData.Stride] == 0) { outlineValues[i - bmpData.Stride] = 255; } else if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride]) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes            ) if(argbValues[i + bmpData.Stride + 4] == 0) { outlineValues[i + bmpData.Stride + 4] = 255; } else if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride + 4]) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes            ) if(argbValues[i - bmpData.Stride - 4] == 0) { outlineValues[i - bmpData.Stride - 4] = 255; } else if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride - 4]) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes            ) if(argbValues[i + bmpData.Stride - 4] == 0) { outlineValues[i + bmpData.Stride - 4] = 255; } else if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride - 4]) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes            ) if(argbValues[i - bmpData.Stride + 4] == 0) { outlineValues[i - bmpData.Stride + 4] = 255; } else if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride + 4]) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(shrink)
                                {
                                    if(i + 8 >= 0 && i + 8 < numBytes                                          ) if(argbValues[i + 8] == 0) { outlineValues[i + 8] = 255; } else if(zbuffer[i] - 7 > zbuffer[i + 8]) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - 8 >= 0 && i - 8 < numBytes                                          ) if(argbValues[i - 8] == 0) { outlineValues[i - 8] = 255; } else if(zbuffer[i] - 7 > zbuffer[i - 8]) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes        ) if(argbValues[i + bmpData.Stride * 2] == 0) { outlineValues[i + bmpData.Stride * 2] = 255; } else if(zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2]) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes        ) if(argbValues[i - bmpData.Stride * 2] == 0) { outlineValues[i - bmpData.Stride * 2] = 255; } else if(zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2]) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes        ) if(argbValues[i + bmpData.Stride + 8] == 0) { outlineValues[i + bmpData.Stride + 8] = 255; } else if(zbuffer[i] - 7 > zbuffer[i + bmpData.Stride + 8]) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes        ) if(argbValues[i - bmpData.Stride + 8] == 0) { outlineValues[i - bmpData.Stride + 8] = 255; } else if(zbuffer[i] - 7 > zbuffer[i - bmpData.Stride + 8]) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes        ) if(argbValues[i + bmpData.Stride - 8] == 0) { outlineValues[i + bmpData.Stride - 8] = 255; } else if(zbuffer[i] - 7 > zbuffer[i + bmpData.Stride - 8]) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes        ) if(argbValues[i - bmpData.Stride - 8] == 0) { outlineValues[i - bmpData.Stride - 8] = 255; } else if(zbuffer[i] - 7 > zbuffer[i - bmpData.Stride - 8]) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes) if(argbValues[i + bmpData.Stride * 2 + 8] == 0) { outlineValues[i + bmpData.Stride * 2 + 8] = 255; } else if(zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2 + 8]) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes) if(argbValues[i + bmpData.Stride * 2 + 4] == 0) { outlineValues[i + bmpData.Stride * 2 + 4] = 255; } else if(zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2 + 4]) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes) if(argbValues[i + bmpData.Stride * 2 - 4] == 0) { outlineValues[i + bmpData.Stride * 2 - 4] = 255; } else if(zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2 - 4]) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes) if(argbValues[i + bmpData.Stride * 2 - 8] == 0) { outlineValues[i + bmpData.Stride * 2 - 8] = 255; } else if(zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2 - 8]) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes) if(argbValues[i - bmpData.Stride * 2 + 8] == 0) { outlineValues[i - bmpData.Stride * 2 + 8] = 255; } else if(zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2 + 8]) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes) if(argbValues[i - bmpData.Stride * 2 + 4] == 0) { outlineValues[i - bmpData.Stride * 2 + 4] = 255; } else if(zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2 + 4]) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes) if(argbValues[i - bmpData.Stride * 2 - 4] == 0) { outlineValues[i - bmpData.Stride * 2 - 4] = 255; } else if(zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2 - 4]) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes) if(argbValues[i - bmpData.Stride * 2 - 8] == 0) { outlineValues[i - bmpData.Stride * 2 - 8] = 255; } else if(zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2 - 8]) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                }
                            }
                        }
                    }
                    break;
                case Outlining.Light:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(i + 4 >= 0 && i + 4 < numBytes && zbuffer[i] - 7 > zbuffer[i + 4]) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4 >= 0 && i - 4 < numBytes && zbuffer[i] - 7 > zbuffer[i - 4]) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride]) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride]) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride + 4]) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride - 4]) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride - 4]) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride + 4]) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(shrink)
                                {
                                    if(i + 8 >= 0 && i + 8 < numBytes && zbuffer[i] - 7 > zbuffer[i + 8]) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - 8 >= 0 && i - 8 < numBytes && zbuffer[i] - 7 > zbuffer[i - 8]) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2]) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2]) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride + 8]) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride + 8]) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride - 8]) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride - 8]) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2 + 8]) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2 + 4]) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2 - 4]) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2 - 8]) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2 + 8]) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2 + 4]) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2 - 4]) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2 - 8]) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                }
                            }
                        }
                    }
                    break;
                case Outlining.Partial:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(i + 4 >= 0 && i + 4 < numBytes && argbValues[i + 4] == 0) { } else if(i + 4 >= 0 && i + 4 < numBytes && zbuffer[i] - 7 > zbuffer[i + 4]) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4 >= 0 && i - 4 < numBytes && argbValues[i - 4] == 0) { } else if(i - 4 >= 0 && i - 4 < numBytes && zbuffer[i] - 7 > zbuffer[i - 4]) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && argbValues[i + bmpData.Stride] == 0) { } else if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride]) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && argbValues[i - bmpData.Stride] == 0) { } else if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride]) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && argbValues[i + bmpData.Stride + 4] == 0) { } else if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride + 4]) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && argbValues[i - bmpData.Stride - 4] == 0) { } else if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride - 4]) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && argbValues[i + bmpData.Stride - 4] == 0) { } else if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride - 4]) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && argbValues[i - bmpData.Stride + 4] == 0) { } else if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride + 4]) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(shrink)
                                {
                                    if(i + 8 >= 0 && i + 8 < numBytes && argbValues[i + 8] == 0) { } else if(i + 8 >= 0 && i + 8 < numBytes && zbuffer[i] - 7 > zbuffer[i + 8]) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - 8 >= 0 && i - 8 < numBytes && argbValues[i - 8] == 0) { } else if(i - 8 >= 0 && i - 8 < numBytes && zbuffer[i] - 7 > zbuffer[i - 8]) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && argbValues[i + bmpData.Stride * 2] == 0) { } else if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2]) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && argbValues[i - bmpData.Stride * 2] == 0) { } else if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2]) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && argbValues[i + bmpData.Stride + 8] == 0) { } else if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride + 8]) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && argbValues[i - bmpData.Stride + 8] == 0) { } else if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride + 8]) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && argbValues[i + bmpData.Stride - 8] == 0) { } else if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride - 8]) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && argbValues[i - bmpData.Stride - 8] == 0) { } else if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride - 8]) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && argbValues[i + bmpData.Stride * 2 + 8] == 0) { } else if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2 + 8]) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && argbValues[i + bmpData.Stride * 2 + 4] == 0) { } else if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2 + 4]) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && argbValues[i + bmpData.Stride * 2 - 4] == 0) { } else if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2 - 4]) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && argbValues[i + bmpData.Stride * 2 - 8] == 0) { } else if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 7 > zbuffer[i + bmpData.Stride * 2 - 8]) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && argbValues[i - bmpData.Stride * 2 + 8] == 0) { } else if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2 + 8]) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && argbValues[i - bmpData.Stride * 2 + 4] == 0) { } else if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2 + 4]) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && argbValues[i - bmpData.Stride * 2 - 4] == 0) { } else if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2 - 4]) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && argbValues[i - bmpData.Stride * 2 - 8] == 0) { } else if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 7 > zbuffer[i - bmpData.Stride * 2 - 8]) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                }
                            }
                        }
                    }
                    break;
            }

            for(int i = 3; i < numBytes; i += 4)
            {
                if(argbValues[i] > 0) // && argbValues[i] <= 255 * flat_alpha
                    argbValues[i] = 255;
                if(outlineValues[i] == 255) argbValues[i] = 255;
            }

            Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            if(!shrink)
            {
                return bmp;
            }
            else
            {
                Graphics g = Graphics.FromImage(bmp);
                Bitmap b2 = new Bitmap(bWidth / 2, bHeight / 2, PixelFormat.Format32bppArgb);
                Graphics g2 = Graphics.FromImage(b2);
                g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g2.DrawImage(bmp.Clone(new Rectangle(0, 0, bWidth, bHeight), bmp.PixelFormat), 0, 0, bWidth / 2, bHeight / 2);
                g2.Dispose();
                return b2;
            }
        }

        private static Bitmap renderSmartOrtho45(MagicaVoxelData[] voxels, byte xSize, byte ySize, byte zSize, OrthoDirection dir, Outlining o, bool shrink)
        {
            //byte tsx = (byte)sizex, tsy = (byte)sizey;
            byte tsx = (byte)Math.Max(sizey, sizex), tsy = tsx;
            byte hSize = Math.Max(ySize, xSize);

            xSize = hSize;
            ySize = hSize;

            int bWidth = hSize * 3 + 4;
            int bHeight = hSize * 3 + zSize * 2 + 4;
            Bitmap bmp = new Bitmap(bWidth, bHeight, PixelFormat.Format32bppArgb);

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            // int numBytes = bmp.Width * bmp.Height * 3; 
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] argbValues = new byte[numBytes];
            argbValues.Fill<byte>(0);
            byte[] outlineValues = new byte[numBytes];
            outlineValues.Fill<byte>(0);
            MagicaVoxelData[] vls = new MagicaVoxelData[voxels.Length];

            switch(dir)
            {
                case OrthoDirection.S:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            vls[i].x = (byte)(voxels[i].x + (hSize - tsx) / 2);
                            vls[i].y = (byte)(voxels[i].y + (hSize - tsy) / 2);
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
                case OrthoDirection.W:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            byte tempX = (byte)(voxels[i].x + ((hSize - tsx) / 2) - (xSize / 2));
                            byte tempY = (byte)(voxels[i].y + ((hSize - tsy) / 2) - (ySize / 2));
                            vls[i].x = (byte)((tempY) + (ySize / 2));
                            vls[i].y = (byte)((tempX * -1) + (xSize / 2) - 1);
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
                case OrthoDirection.N:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            byte tempX = (byte)(voxels[i].x + ((hSize - tsx) / 2) - (xSize / 2));
                            byte tempY = (byte)(voxels[i].y + ((hSize - tsy) / 2) - (ySize / 2));
                            vls[i].x = (byte)((tempX * -1) + (xSize / 2) - 1);
                            vls[i].y = (byte)((tempY * -1) + (ySize / 2) - 1);
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
                case OrthoDirection.E:
                    {
                        for(int i = 0; i < voxels.Length; i++)
                        {
                            byte tempX = (byte)(voxels[i].x + ((hSize - tsx) / 2) - (xSize / 2));
                            byte tempY = (byte)(voxels[i].y + ((hSize - tsy) / 2) - (ySize / 2));
                            vls[i].x = (byte)((tempY * -1) + (ySize / 2) - 1);
                            vls[i].y = (byte)(tempX + (xSize / 2));
                            vls[i].z = voxels[i].z;
                            vls[i].color = voxels[i].color;
                        }
                    }
                    break;
            }

            int[] xbuffer = new int[numBytes];
            xbuffer.Fill<int>(-999);
            int[] zbuffer = new int[numBytes];
            zbuffer.Fill<int>(-999);

            foreach(MagicaVoxelData vx in vls.OrderByDescending(v => v.x * ySize * 4 + v.y + v.z * ySize * xSize * 4)) //voxelData[i].x + voxelData[i].z * 32 + voxelData[i].y * 32 * 128
            {

                int p = 0;
                int mod_color = vx.color - 1;

                for(int j = 0; j < 5; j++)
                {
                    for(int i = 0; i < 12; i++)
                    {
                        p = VoxelToPixelOrtho45(i, j, vx.x, vx.y, vx.z, mod_color, bmpData.Stride, xSize, ySize, zSize);
                        if(argbValues[p] == 0)
                        {
                            argbValues[p] = renderedOrtho[mod_color][i + (j/3) * 12];
                            zbuffer[p] = vx.z;
                            xbuffer[p] = vx.x;
                            if(outlineValues[p] == 0)
                                outlineValues[p] = renderedOrtho[mod_color][i + 48];
                        }
                    }
                }
            }
            switch(o)
            {
                case Outlining.Full:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(i + 4 >= 0 && i + 4 < numBytes                                              ) if(argbValues[i + 4] == 0) { outlineValues[i + 4] = 255; } else if((zbuffer[i] - zbuffer[i + 4]) > 2 || (xbuffer[i] - xbuffer[i + 4]) > 3) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4 >= 0 && i - 4 < numBytes                                              ) if(argbValues[i - 4] == 0) { outlineValues[i - 4] = 255; } else if((zbuffer[i] - zbuffer[i - 4]) > 2 || (xbuffer[i] - xbuffer[i - 4]) > 3) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes                    ) if(argbValues[i + bmpData.Stride] == 0) { outlineValues[i + bmpData.Stride] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes                    ) if(argbValues[i - bmpData.Stride] == 0) { outlineValues[i - bmpData.Stride] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes            ) if(argbValues[i + bmpData.Stride + 4] == 0) { outlineValues[i + bmpData.Stride + 4] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes            ) if(argbValues[i - bmpData.Stride - 4] == 0) { outlineValues[i - bmpData.Stride - 4] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes            ) if(argbValues[i + bmpData.Stride - 4] == 0) { outlineValues[i + bmpData.Stride - 4] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes            ) if(argbValues[i - bmpData.Stride + 4] == 0) { outlineValues[i - bmpData.Stride + 4] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(shrink)
                                {
                                    if(i + 8 >= 0 && i + 8 < numBytes                                          ) if(argbValues[i + 8] == 0) { outlineValues[i + 8] = 255; } else if((zbuffer[i] - zbuffer[i + 8]) > 2 || (xbuffer[i] - xbuffer[i + 8]) > 3) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - 8 >= 0 && i - 8 < numBytes                                          ) if(argbValues[i - 8] == 0) { outlineValues[i - 8] = 255; } else if((zbuffer[i] - zbuffer[i - 8]) > 2 || (xbuffer[i] - xbuffer[i - 8]) > 3) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes        ) if(argbValues[i + bmpData.Stride * 2] == 0) { outlineValues[i + bmpData.Stride * 2] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 3) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes        ) if(argbValues[i - bmpData.Stride * 2] == 0) { outlineValues[i - bmpData.Stride * 2] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes        ) if(argbValues[i + bmpData.Stride + 8] == 0) { outlineValues[i + bmpData.Stride + 8] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes        ) if(argbValues[i - bmpData.Stride + 8] == 0) { outlineValues[i - bmpData.Stride + 8] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes        ) if(argbValues[i + bmpData.Stride - 8] == 0) { outlineValues[i + bmpData.Stride - 8] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes        ) if(argbValues[i - bmpData.Stride - 8] == 0) { outlineValues[i - bmpData.Stride - 8] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes) if(argbValues[i + bmpData.Stride * 2 + 8] == 0) { outlineValues[i + bmpData.Stride * 2 + 8] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes) if(argbValues[i + bmpData.Stride * 2 + 4] == 0) { outlineValues[i + bmpData.Stride * 2 + 4] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes) if(argbValues[i + bmpData.Stride * 2 - 4] == 0) { outlineValues[i + bmpData.Stride * 2 - 4] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes) if(argbValues[i + bmpData.Stride * 2 - 8] == 0) { outlineValues[i + bmpData.Stride * 2 - 8] = 255; } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes) if(argbValues[i - bmpData.Stride * 2 + 8] == 0) { outlineValues[i - bmpData.Stride * 2 + 8] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes) if(argbValues[i - bmpData.Stride * 2 + 4] == 0) { outlineValues[i - bmpData.Stride * 2 + 4] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes) if(argbValues[i - bmpData.Stride * 2 - 4] == 0) { outlineValues[i - bmpData.Stride * 2 - 4] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes) if(argbValues[i - bmpData.Stride * 2 - 8] == 0) { outlineValues[i - bmpData.Stride * 2 - 8] = 255; } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                }
                            }
                        }
                    }
                    break;
                case Outlining.Light:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(i + 4 >= 0 && i + 4 < numBytes                                              ) if((zbuffer[i] - zbuffer[i + 4]) > 2 || (xbuffer[i] - xbuffer[i + 4]) > 3) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4 >= 0 && i - 4 < numBytes                                              ) if((zbuffer[i] - zbuffer[i - 4]) > 2 || (xbuffer[i] - xbuffer[i - 4]) > 3) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes                    ) if((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 2 || ((xbuffer[i] - xbuffer[i + bmpData.Stride]) > 3 && (zbuffer[i] - zbuffer[i + bmpData.Stride]) <= 0)) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes                    ) if((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes            ) if((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 2 || ((xbuffer[i] - xbuffer[i + bmpData.Stride + 4]) > 3 && (zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) <= 0)) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes            ) if((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes            ) if((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 2 || ((xbuffer[i] - xbuffer[i + bmpData.Stride - 4]) > 3 && (zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) <= 0)) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes            ) if((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(shrink)
                                {
                                    if(i + 8 >= 0 && i + 8 < numBytes                                          ) if((zbuffer[i] - zbuffer[i + 8]) > 2 || (xbuffer[i] - xbuffer[i + 8]) > 3) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - 8 >= 0 && i - 8 < numBytes                                          ) if((zbuffer[i] - zbuffer[i - 8]) > 2 || (xbuffer[i] - xbuffer[i - 8]) > 3) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes        ) if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 2 || ((xbuffer[i] - xbuffer[i + bmpData.Stride * 2]) > 3 && (zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) <= 0)) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes        ) if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes        ) if((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 2 || ((xbuffer[i] - xbuffer[i + bmpData.Stride + 8]) > 3 && (zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) <= 0)) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes        ) if((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes        ) if((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 2 || ((xbuffer[i] - xbuffer[i + bmpData.Stride - 8]) > 3 && (zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) <= 0)) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes        ) if((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes) if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 2 || ((xbuffer[i] - xbuffer[i + bmpData.Stride * 2 + 8]) > 3 && (zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes) if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 2 || ((xbuffer[i] - xbuffer[i + bmpData.Stride * 2 + 4]) > 3 && (zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes) if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 2 || ((xbuffer[i] - xbuffer[i + bmpData.Stride * 2 - 4]) > 3 && (zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes) if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 2 || ((xbuffer[i] - xbuffer[i + bmpData.Stride * 2 - 8]) > 3 && (zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes) if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes) if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes) if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes) if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 3 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                }
                            }
                        }
                    }
                    break;
                case Outlining.Partial:
                    {
                        for(int i = 3; i < numBytes; i += 4)
                        {
                            if(argbValues[i] > 0)
                            {

                                if(i + 4 >= 0 && i + 4 < numBytes                                              ) if(argbValues[i + 4] == 0) { } else if((zbuffer[i] - zbuffer[i + 4]) > 2 || (xbuffer[i] - xbuffer[i + 4]) > 3) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if(i - 4 >= 0 && i - 4 < numBytes                                              ) if(argbValues[i - 4] == 0) { } else if((zbuffer[i] - zbuffer[i - 4]) > 2 || (xbuffer[i] - xbuffer[i - 4]) > 3) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes                    ) if(argbValues[i + bmpData.Stride] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes                    ) if(argbValues[i - bmpData.Stride] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes            ) if(argbValues[i + bmpData.Stride + 4] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes            ) if(argbValues[i - bmpData.Stride - 4] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes            ) if(argbValues[i + bmpData.Stride - 4] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if(i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes            ) if(argbValues[i - bmpData.Stride + 4] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if(shrink)
                                {
                                    if(i + 8 >= 0 && i + 8 < numBytes                                          ) if(argbValues[i + 8] == 0) { } else if((zbuffer[i] - zbuffer[i + 8]) > 2 || (xbuffer[i] - xbuffer[i + 8]) > 3) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - 8 >= 0 && i - 8 < numBytes                                          ) if(argbValues[i - 8] == 0) { } else if((zbuffer[i] - zbuffer[i - 8]) > 2 || (xbuffer[i] - xbuffer[i - 8]) > 3) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes        ) if(argbValues[i + bmpData.Stride * 2] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 3) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes        ) if(argbValues[i - bmpData.Stride * 2] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes        ) if(argbValues[i + bmpData.Stride + 8] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes        ) if(argbValues[i - bmpData.Stride + 8] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes        ) if(argbValues[i + bmpData.Stride - 8] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes        ) if(argbValues[i - bmpData.Stride - 8] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 2 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes) if(argbValues[i + bmpData.Stride * 2 + 8] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes) if(argbValues[i + bmpData.Stride * 2 + 4] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes) if(argbValues[i + bmpData.Stride * 2 - 4] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                    if(i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes) if(argbValues[i + bmpData.Stride * 2 - 8] == 0) { } else if((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes) if(argbValues[i - bmpData.Stride * 2 + 8] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes) if(argbValues[i - bmpData.Stride * 2 + 4] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes) if(argbValues[i - bmpData.Stride * 2 - 4] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                    if(i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes) if(argbValues[i - bmpData.Stride * 2 - 8] == 0) { } else if((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                }
                            }
                        }
                    }
                    break;
            }

            for(int i = 3; i < numBytes; i += 4)
            {
                if(argbValues[i] > 0) // && argbValues[i] <= 255 * flat_alpha
                    argbValues[i] = 255;
                if(outlineValues[i] == 255) argbValues[i] = 255;
            }

            Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            if(!shrink)
            {
                return bmp;
            }
            else
            {
                Graphics g = Graphics.FromImage(bmp);
                Bitmap b2 = new Bitmap(bWidth / 2, bHeight / 2, PixelFormat.Format32bppArgb);
                Graphics g2 = Graphics.FromImage(b2);
                g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g2.DrawImage(bmp.Clone(new Rectangle(0, 0, bWidth, bHeight), bmp.PixelFormat), 0, 0, bWidth / 2, bHeight / 2);
                g2.Dispose();
                return b2;
            }
            
        }


        /*
        public static Bitmap processSingleOutlined(MagicaVoxelData[] parsed, byte xSize, byte ySize, byte zSize, Direction dir)
        {
            Graphics g;
            Bitmap b, o;
            b = render(parsed, xSize, ySize, zSize, dir);
            o = renderOutline(parsed, xSize, ySize, zSize, dir);
            g = Graphics.FromImage(o);
            g.DrawImage(b, 2,6);
            return o;
        }
        private static void processUnitOutlined(MagicaVoxelData[] parsed, string u, byte xSize, byte ySize, byte zSize)
        {
            u = u.Substring(0, u.Length - 4);
            System.IO.Directory.CreateDirectory(u);

            processSingleOutlined(parsed, xSize, ySize, zSize, Direction.SE).Save(u + SEP + u + "_outline_SE" + ".png", ImageFormat.Png); //se
            processSingleOutlined(parsed, xSize, ySize, zSize, Direction.SW).Save(u + SEP + u + "_outline_SW" + ".png", ImageFormat.Png); //sw
            processSingleOutlined(parsed, xSize, ySize, zSize, Direction.NW).Save(u + SEP + u + "_outline_NW" + ".png", ImageFormat.Png); //nw
            processSingleOutlined(parsed, xSize, ySize, zSize, Direction.NE).Save(u + SEP + u + "_outline_NE" + ".png", ImageFormat.Png); //ne

        }
        private static void processUnit(MagicaVoxelData[] parsed, string u, byte xSize, byte ySize, byte zSize)
        {
            u = u.Substring(0, u.Length - 4);
            System.IO.Directory.CreateDirectory(u);

            render(parsed, xSize, ySize, zSize, Direction.SE).Save(u + SEP + u + "_SE" + ".png", ImageFormat.Png); //se
            render(parsed, xSize, ySize, zSize, Direction.SW).Save(u + SEP + u + "_SW" + ".png", ImageFormat.Png); //sw
            render(parsed, xSize, ySize, zSize, Direction.NW).Save(u + SEP + u + "_NW" + ".png", ImageFormat.Png); //nw
            render(parsed, xSize, ySize, zSize, Direction.NE).Save(u + SEP + u + "_NE" + ".png", ImageFormat.Png); //ne

        }
        */

        private static Bitmap renderLarge(byte[,,] voxels, int xSize, int ySize, int zSize, Outlining o)
        {

            int hSize = Math.Max(ySize, xSize);
            int tsx = hSize, tsy = hSize;
            
            int bWidth = (hSize + hSize) * 2 + 8;
            int bHeight = (hSize + hSize) + zSize * 3 + 8;
            Bitmap bmp = new Bitmap(bWidth, bHeight, PixelFormat.Format32bppArgb);

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            // int numBytes = bmp.Width * bmp.Height * 3; 
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] argbValues = new byte[numBytes];
            argbValues.Fill<byte>(0);
            byte[] outlineValues = new byte[numBytes];
            outlineValues.Fill<byte>(0);
            byte[] bareValues = new byte[numBytes];
            bareValues.Fill<byte>(0);
            int[] zbuffer = new int[numBytes];
            zbuffer.Fill<int>(-999);
            for (int fz = zSize - 1; fz >= 0; fz--)
            {
                for (int fx = xSize - 1; fx >= 0; fx--)
                {
                    for (int fy = 0; fy < ySize; fy++)
                    {
                        if (voxels[fx, fy, fz] == 0) continue;
                        int current_color = voxels[fx, fy, fz] - 1;
                        for (int j = 0; j < 4; j++)
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                int p = VoxelToPixelGenericIso(i, j, fx, fy, fz, bmpData.Stride, xSize, ySize, zSize);
                                if (argbValues[p] == 0)
                                {
                                    //if (rendered[current_color][16] != 0)
                                    {
                                        argbValues[p] = rendered[current_color][i + j * 16];
                                        //if (fz == zSize - 1 || fx == 0 || fy == ySize - 1)
                                        //    argbValues[p] = ShadeIso(rendered[current_color], i, 0, 0, 0);
                                        //else
                                        //    argbValues[p] = ShadeIso(rendered[current_color], i, voxels[fx - 1, fy, fz], voxels[fx, fy + 1, fz], voxels[fx, fy, fz + 1]);

                                        zbuffer[p] = (fz + fx - fy);

                                        if (outlineValues[p] == 0)
                                            outlineValues[p] = rendered[current_color][i + 64];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            switch (o)
            {
                case Outlining.Full:
                    {
                        for (int i = 3; i < numBytes; i += 4)
                        {
                            if (argbValues[i] > 0)
                            {

                                if (i + 4 >= 0 && i + 4 < numBytes) if (argbValues[i + 4] == 0) { outlineValues[i + 4] = 255; } else if (zbuffer[i] - 2 > zbuffer[i + 4]) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if (i - 4 >= 0 && i - 4 < numBytes) if (argbValues[i - 4] == 0) { outlineValues[i - 4] = 255; } else if (zbuffer[i] - 2 > zbuffer[i - 4]) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if (i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes) if (argbValues[i + bmpData.Stride] == 0) { outlineValues[i + bmpData.Stride] = 255; } else if (zbuffer[i] - 2 > zbuffer[i + bmpData.Stride]) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if (i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes) if (argbValues[i - bmpData.Stride] == 0) { outlineValues[i - bmpData.Stride] = 255; } else if (zbuffer[i] - 2 > zbuffer[i - bmpData.Stride]) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if (i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes) if (argbValues[i + bmpData.Stride + 4] == 0) { outlineValues[i + bmpData.Stride + 4] = 255; } else if (zbuffer[i] - 2 > zbuffer[i + bmpData.Stride + 4]) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if (i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes) if (argbValues[i - bmpData.Stride - 4] == 0) { outlineValues[i - bmpData.Stride - 4] = 255; } else if (zbuffer[i] - 2 > zbuffer[i - bmpData.Stride - 4]) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if (i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes) if (argbValues[i + bmpData.Stride - 4] == 0) { outlineValues[i + bmpData.Stride - 4] = 255; } else if (zbuffer[i] - 2 > zbuffer[i + bmpData.Stride - 4]) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if (i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes) if (argbValues[i - bmpData.Stride + 4] == 0) { outlineValues[i - bmpData.Stride + 4] = 255; } else if (zbuffer[i] - 2 > zbuffer[i - bmpData.Stride + 4]) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if (i + 8 >= 0 && i + 8 < numBytes) if (argbValues[i + 8] == 0) { outlineValues[i + 8] = 255; } else if (zbuffer[i] - 2 > zbuffer[i + 8]) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if (i - 8 >= 0 && i - 8 < numBytes) if (argbValues[i - 8] == 0) { outlineValues[i - 8] = 255; } else if (zbuffer[i] - 2 > zbuffer[i - 8]) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if (i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes) if (argbValues[i + bmpData.Stride * 2] == 0) { outlineValues[i + bmpData.Stride * 2] = 255; } else if (zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2]) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if (i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes) if (argbValues[i - bmpData.Stride * 2] == 0) { outlineValues[i - bmpData.Stride * 2] = 255; } else if (zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2]) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if (i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes) if (argbValues[i + bmpData.Stride + 8] == 0) { outlineValues[i + bmpData.Stride + 8] = 255; } else if (zbuffer[i] - 2 > zbuffer[i + bmpData.Stride + 8]) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if (i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes) if (argbValues[i - bmpData.Stride + 8] == 0) { outlineValues[i - bmpData.Stride + 8] = 255; } else if (zbuffer[i] - 2 > zbuffer[i - bmpData.Stride + 8]) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if (i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes) if (argbValues[i + bmpData.Stride - 8] == 0) { outlineValues[i + bmpData.Stride - 8] = 255; } else if (zbuffer[i] - 2 > zbuffer[i + bmpData.Stride - 8]) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if (i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes) if (argbValues[i - bmpData.Stride - 8] == 0) { outlineValues[i - bmpData.Stride - 8] = 255; } else if (zbuffer[i] - 2 > zbuffer[i - bmpData.Stride - 8]) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if (i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes) if (argbValues[i + bmpData.Stride * 2 + 8] == 0) { outlineValues[i + bmpData.Stride * 2 + 8] = 255; } else if (zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 + 8]) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if (i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes) if (argbValues[i + bmpData.Stride * 2 + 4] == 0) { outlineValues[i + bmpData.Stride * 2 + 4] = 255; } else if (zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 + 4]) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if (i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes) if (argbValues[i + bmpData.Stride * 2 - 4] == 0) { outlineValues[i + bmpData.Stride * 2 - 4] = 255; } else if (zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 - 4]) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if (i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes) if (argbValues[i + bmpData.Stride * 2 - 8] == 0) { outlineValues[i + bmpData.Stride * 2 - 8] = 255; } else if (zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 - 8]) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                if (i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes) if (argbValues[i - bmpData.Stride * 2 + 8] == 0) { outlineValues[i - bmpData.Stride * 2 + 8] = 255; } else if (zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 + 8]) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if (i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes) if (argbValues[i - bmpData.Stride * 2 + 4] == 0) { outlineValues[i - bmpData.Stride * 2 + 4] = 255; } else if (zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 + 4]) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if (i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes) if (argbValues[i - bmpData.Stride * 2 - 4] == 0) { outlineValues[i - bmpData.Stride * 2 - 4] = 255; } else if (zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 - 4]) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if (i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes) if (argbValues[i - bmpData.Stride * 2 - 8] == 0) { outlineValues[i - bmpData.Stride * 2 - 8] = 255; } else if (zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 - 8]) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
                case Outlining.Light:
                    {
                        for (int i = 3; i < numBytes; i += 4)
                        {
                            if (argbValues[i] > 0)
                            {

                                if (i + 4 >= 0 && i + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + 4]) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if (i - 4 >= 0 && i - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - 4]) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if (i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride]) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if (i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride]) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride + 4]) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride - 4]) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride - 4]) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride + 4]) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                //if (i + 8 >= 0 && i + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + 8]) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                //if (i - 8 >= 0 && i - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - 8]) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2]) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2]) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride + 8]) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride + 8]) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride - 8]) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride - 8]) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 + 8]) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 + 4]) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 - 4]) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 - 8]) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 + 8]) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 + 4]) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 - 4]) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 - 8]) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
                case Outlining.Partial:
                    {
                        for (int i = 3; i < numBytes; i += 4)
                        {
                            if (argbValues[i] > 0)
                            {

                                if (i + 4 >= 0 && i + 4 < numBytes && argbValues[i + 4] == 0) { } else if (i + 4 >= 0 && i + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + 4]) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if (i - 4 >= 0 && i - 4 < numBytes && argbValues[i - 4] == 0) { } else if (i - 4 >= 0 && i - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - 4]) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if (i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && argbValues[i + bmpData.Stride] == 0) { } else if (i + bmpData.Stride >= 0 && i + bmpData.Stride < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride]) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if (i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && argbValues[i - bmpData.Stride] == 0) { } else if (i - bmpData.Stride >= 0 && i - bmpData.Stride < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride]) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && argbValues[i + bmpData.Stride + 4] == 0) { } else if (i + bmpData.Stride + 4 >= 0 && i + bmpData.Stride + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride + 4]) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && argbValues[i - bmpData.Stride - 4] == 0) { } else if (i - bmpData.Stride - 4 >= 0 && i - bmpData.Stride - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride - 4]) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && argbValues[i + bmpData.Stride - 4] == 0) { } else if (i + bmpData.Stride - 4 >= 0 && i + bmpData.Stride - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride - 4]) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && argbValues[i - bmpData.Stride + 4] == 0) { } else if (i - bmpData.Stride + 4 >= 0 && i - bmpData.Stride + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride + 4]) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                //if (i + 8 >= 0 && i + 8 < numBytes && argbValues[i + 8] == 0) { } else if (i + 8 >= 0 && i + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + 8]) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                //if (i - 8 >= 0 && i - 8 < numBytes && argbValues[i - 8] == 0) { } else if (i - 8 >= 0 && i - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - 8]) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && argbValues[i + bmpData.Stride * 2] == 0) { } else if (i + bmpData.Stride * 2 >= 0 && i + bmpData.Stride * 2 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2]) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && argbValues[i - bmpData.Stride * 2] == 0) { } else if (i - bmpData.Stride * 2 >= 0 && i - bmpData.Stride * 2 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2]) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && argbValues[i + bmpData.Stride + 8] == 0) { } else if (i + bmpData.Stride + 8 >= 0 && i + bmpData.Stride + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride + 8]) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && argbValues[i - bmpData.Stride + 8] == 0) { } else if (i - bmpData.Stride + 8 >= 0 && i - bmpData.Stride + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride + 8]) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && argbValues[i + bmpData.Stride - 8] == 0) { } else if (i + bmpData.Stride - 8 >= 0 && i + bmpData.Stride - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride - 8]) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && argbValues[i - bmpData.Stride - 8] == 0) { } else if (i - bmpData.Stride - 8 >= 0 && i - bmpData.Stride - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride - 8]) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && argbValues[i + bmpData.Stride * 2 + 8] == 0) { } else if (i + bmpData.Stride * 2 + 8 >= 0 && i + bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 + 8]) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && argbValues[i + bmpData.Stride * 2 + 4] == 0) { } else if (i + bmpData.Stride * 2 + 4 >= 0 && i + bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 + 4]) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && argbValues[i + bmpData.Stride * 2 - 4] == 0) { } else if (i + bmpData.Stride * 2 - 4 >= 0 && i + bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 - 4]) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if (i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && argbValues[i + bmpData.Stride * 2 - 8] == 0) { } else if (i + bmpData.Stride * 2 - 8 >= 0 && i + bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i + bmpData.Stride * 2 - 8]) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && argbValues[i - bmpData.Stride * 2 + 8] == 0) { } else if (i - bmpData.Stride * 2 + 8 >= 0 && i - bmpData.Stride * 2 + 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 + 8]) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && argbValues[i - bmpData.Stride * 2 + 4] == 0) { } else if (i - bmpData.Stride * 2 + 4 >= 0 && i - bmpData.Stride * 2 + 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 + 4]) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && argbValues[i - bmpData.Stride * 2 - 4] == 0) { } else if (i - bmpData.Stride * 2 - 4 >= 0 && i - bmpData.Stride * 2 - 4 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 - 4]) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if (i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && argbValues[i - bmpData.Stride * 2 - 8] == 0) { } else if (i - bmpData.Stride * 2 - 8 >= 0 && i - bmpData.Stride * 2 - 8 < numBytes && zbuffer[i] - 2 > zbuffer[i - bmpData.Stride * 2 - 8]) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
            }

            for (int i = 3; i < numBytes; i += 4)
            {
                if (argbValues[i] > 0) // && argbValues[i] <= 255 * flat_alpha
                    argbValues[i] = 255;
                if (outlineValues[i] == 255) argbValues[i] = 255;
            }

            Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        private static Bitmap renderLargeOrtho(byte[,,] voxels, int xSize, int ySize, int zSize, Outlining o)
        {
            int hSize = Math.Max(ySize, xSize);

            int bWidth = hSize * 3 + 8;
            int bHeight = hSize + zSize * 3 + 8;
            Bitmap bmp = new Bitmap(bWidth, bHeight, PixelFormat.Format32bppArgb);

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            // int numBytes = bmp.Width * bmp.Height * 3; 
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] argbValues = new byte[numBytes];
            argbValues.Fill<byte>(0);
            byte[] outlineValues = new byte[numBytes];
            outlineValues.Fill<byte>(0);

            int[] xbuffer = new int[numBytes];
            xbuffer.Fill<int>(-999);
            int[] zbuffer = new int[numBytes];
            zbuffer.Fill<int>(-999);

            for (int fz = zSize - 1; fz >= 0; fz--)
            {
                for (int fx = xSize - 1; fx >= 0; fx--)
                {
                    for (int fy = 0; fy < ySize; fy++)
                    {
                        if (voxels[fx, fy, fz] == 0) continue;
                        int current_color = voxels[fx, fy, fz] - 1;

                        for (int j = 0; j < 4; j++)
                        {
                            for (int i = 0; i < 12; i++)
                            {
                                int p = VoxelToPixelGeneric(i, j, fx, fy, fz, bmpData.Stride, zSize);
                                if (argbValues[p] == 0)
                                {
                                    argbValues[p] = renderedOrtho[current_color][i + j * 12];
                                    zbuffer[p] = fz;
                                    xbuffer[p] = fx;
                                    if (outlineValues[p] == 0)
                                        outlineValues[p] = renderedOrtho[current_color][i + 48];
                                }
                            }
                        }
                    }
                }
            }
            switch (o)
            {
                case Outlining.Full:
                    {
                        for (int i = 3; i < numBytes; i += 4)
                        {
                            if (argbValues[i] > 0)
                            {

                                if (argbValues[i + 4] == 0) { outlineValues[i + 4] = 255; } else if ((zbuffer[i] - zbuffer[i + 4]) > 1 || (xbuffer[i] - xbuffer[i + 4]) > 3) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - 4] == 0) { outlineValues[i - 4] = 255; } else if ((zbuffer[i] - zbuffer[i - 4]) > 1 || (xbuffer[i] - xbuffer[i - 4]) > 3) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride] == 0) { outlineValues[i + bmpData.Stride] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride] == 0) { outlineValues[i - bmpData.Stride] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride + 4] == 0) { outlineValues[i + bmpData.Stride + 4] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride - 4] == 0) { outlineValues[i - bmpData.Stride - 4] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride - 4] == 0) { outlineValues[i + bmpData.Stride - 4] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride + 4] == 0) { outlineValues[i - bmpData.Stride + 4] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                if (argbValues[i + 8] == 0) { outlineValues[i + 8] = 255; } else if ((zbuffer[i] - zbuffer[i + 8]) > 1 || (xbuffer[i] - xbuffer[i + 8]) > 3) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - 8] == 0) { outlineValues[i - 8] = 255; } else if ((zbuffer[i] - zbuffer[i - 8]) > 1 || (xbuffer[i] - xbuffer[i - 8]) > 3) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride * 2] == 0) { outlineValues[i + bmpData.Stride * 2] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 3) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride * 2] == 0) { outlineValues[i - bmpData.Stride * 2] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride + 8] == 0) { outlineValues[i + bmpData.Stride + 8] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride + 8] == 0) { outlineValues[i - bmpData.Stride + 8] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride - 8] == 0) { outlineValues[i + bmpData.Stride - 8] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride - 8] == 0) { outlineValues[i - bmpData.Stride - 8] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride * 2 + 8] == 0) { outlineValues[i + bmpData.Stride * 2 + 8] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride * 2 + 4] == 0) { outlineValues[i + bmpData.Stride * 2 + 4] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride * 2 - 4] == 0) { outlineValues[i + bmpData.Stride * 2 - 4] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride * 2 - 8] == 0) { outlineValues[i + bmpData.Stride * 2 - 8] = 255; } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride * 2 + 8] == 0) { outlineValues[i - bmpData.Stride * 2 + 8] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride * 2 + 4] == 0) { outlineValues[i - bmpData.Stride * 2 + 4] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride * 2 - 4] == 0) { outlineValues[i - bmpData.Stride * 2 - 4] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride * 2 - 8] == 0) { outlineValues[i - bmpData.Stride * 2 - 8] = 255; } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
                case Outlining.Light:
                    {
                        for (int i = 3; i < numBytes; i += 4)
                        {
                            if (argbValues[i] > 0)
                            {

                                if ((zbuffer[i] - zbuffer[i + 4]) > 1 || (xbuffer[i] - xbuffer[i + 4]) > 3) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if ((zbuffer[i] - zbuffer[i - 4]) > 1 || (xbuffer[i] - xbuffer[i - 4]) > 3) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if ((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if ((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                //if ((zbuffer[i] - zbuffer[i + 8]) > 1 || (xbuffer[i] - xbuffer[i + 8]) > 3) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - 8]) > 1 || (xbuffer[i] - xbuffer[i - 8]) > 3) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 4) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
                case Outlining.Partial:
                    {
                        for (int i = 3; i < numBytes; i += 4)
                        {
                            if (argbValues[i] > 0)
                            {

                                if (argbValues[i + 4] == 0) { } else if ((zbuffer[i] - zbuffer[i + 4]) > 1 || (xbuffer[i] - xbuffer[i + 4]) > 3) { argbValues[i + 4] = 255; argbValues[i + 4 - 1] = outlineValues[i - 1]; argbValues[i + 4 - 2] = outlineValues[i - 2]; argbValues[i + 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - 4] == 0) { } else if ((zbuffer[i] - zbuffer[i - 4]) > 1 || (xbuffer[i] - xbuffer[i - 4]) > 3) { argbValues[i - 4] = 255; argbValues[i - 4 - 1] = outlineValues[i - 1]; argbValues[i - 4 - 2] = outlineValues[i - 2]; argbValues[i - 4 - 3] = outlineValues[i - 3]; }
                                if (argbValues[i + bmpData.Stride] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride]) > 3) { argbValues[i + bmpData.Stride] = 255; argbValues[i + bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 3] = outlineValues[i - 3]; }
                                if (argbValues[i - bmpData.Stride] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride]) <= 0)) { argbValues[i - bmpData.Stride] = 255; argbValues[i - bmpData.Stride - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride + 4] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride + 4]) > 3) { argbValues[i + bmpData.Stride + 4] = 255; argbValues[i + bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride - 4] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 4]) <= 0)) { argbValues[i - bmpData.Stride - 4] = 255; argbValues[i - bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride - 4] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride - 4]) > 3) { argbValues[i + bmpData.Stride - 4] = 255; argbValues[i + bmpData.Stride - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride + 4] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 4]) <= 0)) { argbValues[i - bmpData.Stride + 4] = 255; argbValues[i - bmpData.Stride + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 4 - 3] = outlineValues[i - 3]; }

                                //if (argbValues[i + 8] == 0) { } else if ((zbuffer[i] - zbuffer[i + 8]) > 1 || (xbuffer[i] - xbuffer[i + 8]) > 3) { argbValues[i + 8] = 255; argbValues[i + 8 - 1] = outlineValues[i - 1]; argbValues[i + 8 - 2] = outlineValues[i - 2]; argbValues[i + 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - 8] == 0) { } else if ((zbuffer[i] - zbuffer[i - 8]) > 1 || (xbuffer[i] - xbuffer[i - 8]) > 3) { argbValues[i - 8] = 255; argbValues[i - 8 - 1] = outlineValues[i - 1]; argbValues[i - 8 - 2] = outlineValues[i - 2]; argbValues[i - 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride * 2] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2]) > 3) { argbValues[i + bmpData.Stride * 2] = 255; argbValues[i + bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride * 2] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2]) <= 0)) { argbValues[i - bmpData.Stride * 2] = 255; argbValues[i - bmpData.Stride * 2 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride + 8] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride + 8]) > 3) { argbValues[i + bmpData.Stride + 8] = 255; argbValues[i + bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride + 8] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride + 8]) <= 0)) { argbValues[i - bmpData.Stride + 8] = 255; argbValues[i - bmpData.Stride + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride + 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride - 8] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride - 8]) > 3) { argbValues[i + bmpData.Stride - 8] = 255; argbValues[i + bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride - 8] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) > 1 || ((xbuffer[i] - xbuffer[i - bmpData.Stride - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride - 8]) <= 0)) { argbValues[i - bmpData.Stride - 8] = 255; argbValues[i - bmpData.Stride - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride - 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride * 2 + 8] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 8]) > 5) { argbValues[i + bmpData.Stride * 2 + 8] = 255; argbValues[i + bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride * 2 + 4] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 + 4]) > 5) { argbValues[i + bmpData.Stride * 2 + 4] = 255; argbValues[i + bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride * 2 - 4] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 4]) > 5) { argbValues[i + bmpData.Stride * 2 - 4] = 255; argbValues[i + bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i + bmpData.Stride * 2 - 8] == 0) { } else if ((zbuffer[i] - zbuffer[i + bmpData.Stride * 2 - 8]) > 5) { argbValues[i + bmpData.Stride * 2 - 8] = 255; argbValues[i + bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i + bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i + bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride * 2 + 8] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 8] = 255; argbValues[i - bmpData.Stride * 2 + 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 8 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride * 2 + 4] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 + 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 + 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 + 4] = 255; argbValues[i - bmpData.Stride * 2 + 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 + 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 + 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride * 2 - 4] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 4]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 4]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 4] = 255; argbValues[i - bmpData.Stride * 2 - 4 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 4 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 4 - 3] = outlineValues[i - 3]; }
                                //if (argbValues[i - bmpData.Stride * 2 - 8] == 0) { } else if ((zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) > 0 || ((xbuffer[i] - xbuffer[i - bmpData.Stride * 2 - 8]) > 2 && (zbuffer[i] - zbuffer[i - bmpData.Stride * 2 - 8]) <= 0)) { argbValues[i - bmpData.Stride * 2 - 8] = 255; argbValues[i - bmpData.Stride * 2 - 8 - 1] = outlineValues[i - 1]; argbValues[i - bmpData.Stride * 2 - 8 - 2] = outlineValues[i - 2]; argbValues[i - bmpData.Stride * 2 - 8 - 3] = outlineValues[i - 3]; }
                            }
                        }
                    }
                    break;
            }

            for (int i = 3; i < numBytes; i += 4)
            {
                if (argbValues[i] > 0) // && argbValues[i] <= 255 * flat_alpha
                    argbValues[i] = 255;
                if (outlineValues[i] == 255) argbValues[i] = 255;
            }

            Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public static void ProcessUnitSmart(MagicaVoxelData[][] parsed, string u, int xSize, int ySize, int zSize, Outlining o, int multiplier)
        {
            xSize = Math.Max(sizex, xSize);
            ySize = Math.Max(sizey, ySize);
            ySize = xSize = Math.Max(ySize, xSize);
            zSize = Math.Max(sizez, zSize);
            if ((xSize & 1) == 1)
            {
                xSize++;
                ySize++;
            }
            if ((zSize & 1) == 1) zSize++;
            byte bx = (byte)xSize;
            byte by = (byte)ySize;
            byte bz = (byte)zSize;

            u = u.Substring(0, u.Length - 4);
            DirectoryInfo root = Directory.CreateDirectory(u);
            DirectoryInfo di = root.CreateSubdirectory("sizeBase");
            u = root.Name;

            int frames = parsed.Length;
            if (frames == 1)
            {
                bool cubic = multiplier < 0;
                multiplier = Math.Abs(multiplier);
                if (multiplier == 0) multiplier = 1;
                di = root.CreateSubdirectory("sizeBase");
                renderSmart(parsed[0], bx, by, bz, Direction.SE, o, true).Save(di.FullName + SEP + u + "_SE" + ".png", ImageFormat.Png); //se
                renderSmart(parsed[0], bx, by, bz, Direction.SW, o, true).Save(di.FullName + SEP + u + "_SW" + ".png", ImageFormat.Png); //sw
                renderSmart(parsed[0], bx, by, bz, Direction.NW, o, true).Save(di.FullName + SEP + u + "_NW" + ".png", ImageFormat.Png); //nw
                renderSmart(parsed[0], bx, by, bz, Direction.NE, o, true).Save(di.FullName + SEP + u + "_NE" + ".png", ImageFormat.Png); //ne

                renderSmartOrtho(parsed[0], bx, by, bz, OrthoDirection.S, o, true).Save(di.FullName + SEP + u + "_S" + ".png", ImageFormat.Png); //s
                renderSmartOrtho(parsed[0], bx, by, bz, OrthoDirection.W, o, true).Save(di.FullName + SEP + u + "_W" + ".png", ImageFormat.Png); //w
                renderSmartOrtho(parsed[0], bx, by, bz, OrthoDirection.N, o, true).Save(di.FullName + SEP + u + "_N" + ".png", ImageFormat.Png); //n
                renderSmartOrtho(parsed[0], bx, by, bz, OrthoDirection.E, o, true).Save(di.FullName + SEP + u + "_E" + ".png", ImageFormat.Png); //e

                di = root.CreateSubdirectory("sizeAbove");

                renderSmart45(parsed[0], bx, by, bz, Direction.SE, o, true).Save(di.FullName + SEP + u + "_Above_SE" + ".png", ImageFormat.Png); //se
                renderSmart45(parsed[0], bx, by, bz, Direction.SW, o, true).Save(di.FullName + SEP + u + "_Above_SW" + ".png", ImageFormat.Png); //sw
                renderSmart45(parsed[0], bx, by, bz, Direction.NW, o, true).Save(di.FullName + SEP + u + "_Above_NW" + ".png", ImageFormat.Png); //nw
                renderSmart45(parsed[0], bx, by, bz, Direction.NE, o, true).Save(di.FullName + SEP + u + "_Above_NE" + ".png", ImageFormat.Png); //ne

                renderSmartOrtho45(parsed[0], bx, by, bz, OrthoDirection.S, o, true).Save(di.FullName + SEP + u + "_Above_S" + ".png", ImageFormat.Png); //s
                renderSmartOrtho45(parsed[0], bx, by, bz, OrthoDirection.W, o, true).Save(di.FullName + SEP + u + "_Above_W" + ".png", ImageFormat.Png); //w
                renderSmartOrtho45(parsed[0], bx, by, bz, OrthoDirection.N, o, true).Save(di.FullName + SEP + u + "_Above_N" + ".png", ImageFormat.Png); //n
                renderSmartOrtho45(parsed[0], bx, by, bz, OrthoDirection.E, o, true).Save(di.FullName + SEP + u + "_Above_E" + ".png", ImageFormat.Png); //e

                byte[,,] colors = TransformLogic.VoxListToArray(parsed[0], xSize, ySize, zSize);
                FaceVoxel[,,] faces0 = FaceLogic.GetFaces(colors),
                    faces1 = FaceLogic.GetFaces(TransformLogic.RotateYaw(colors, 90)),
                    faces2 = FaceLogic.GetFaces(TransformLogic.RotateYaw(colors, 180)),
                    faces3 = FaceLogic.GetFaces(TransformLogic.RotateYaw(colors, 270));
                di = root.CreateSubdirectory("sizeBaseSloped");
                RenderSmartFaces(faces0, xSize, ySize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_SE" + ".png", ImageFormat.Png); //se
                RenderSmartFacesOrtho(faces0, xSize, ySize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_S" + ".png", ImageFormat.Png); //s
                RenderSmartFaces(faces1, ySize, xSize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_SW" + ".png", ImageFormat.Png); //sw
                RenderSmartFacesOrtho(faces1, xSize, ySize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_W" + ".png", ImageFormat.Png); //w
                RenderSmartFaces(faces2, xSize, ySize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_NW" + ".png", ImageFormat.Png); //nw
                RenderSmartFacesOrtho(faces2, xSize, ySize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_N" + ".png", ImageFormat.Png); //n
                RenderSmartFaces(faces3, ySize, xSize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_NE" + ".png", ImageFormat.Png); //ne
                RenderSmartFacesOrtho(faces3, xSize, ySize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_E" + ".png", ImageFormat.Png); //e

                di = root.CreateSubdirectory("sizeSmallSloped");

                RenderSmartFacesSmall(faces0, bx, by, bz, o, true).Save(di.FullName + SEP + u + "_Small_Slope_SE" + ".png", ImageFormat.Png); //se
                RenderSmartFacesSmall(faces1, by, bx, bz, o, true).Save(di.FullName + SEP + u + "_Small_Slope_SW" + ".png", ImageFormat.Png); //sw
                RenderSmartFacesSmall(faces2, bx, by, bz, o, true).Save(di.FullName + SEP + u + "_Small_Slope_NW" + ".png", ImageFormat.Png); //nw
                RenderSmartFacesSmall(faces3, by, bx, bz, o, true).Save(di.FullName + SEP + u + "_Small_Slope_NE" + ".png", ImageFormat.Png); //ne

                string fs = ".png";
                byte[,,] colors2;
                colors2 = FaceLogic.FaceArrayToByteArray(FaceLogic.DoubleSize(faces0));

                di = root.CreateSubdirectory("sizeBig");

                renderLargeOrtho(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_S" + fs, ImageFormat.Png); //s
                renderLarge(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_SE" + fs, ImageFormat.Png); //sw
                colors2 = TransformLogic.RotateYaw(colors2, 90);
                renderLargeOrtho(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_W" + fs, ImageFormat.Png); //w
                renderLarge(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_SW" + fs, ImageFormat.Png); //nw
                colors2 = TransformLogic.RotateYaw(colors2, 90);
                renderLargeOrtho(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_N" + fs, ImageFormat.Png); //n
                renderLarge(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_NW" + fs, ImageFormat.Png); //ne
                colors2 = TransformLogic.RotateYaw(colors2, 90);
                renderLargeOrtho(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_E" + fs, ImageFormat.Png); //e
                renderLarge(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_NE" + fs, ImageFormat.Png); //se

                for (int s = 1; s <= multiplier; s++)
                {
                    if (s > 1)
                    {
                        colors2 = cubic
                          ? TransformLogic.ScalePartial(colors, s)
                          : TransformLogic.RunCA(TransformLogic.ScalePartial(colors, s), s);
                    }
                    else
                    {
                        colors2 = colors.Replicate();
                    }
                    di = root.CreateSubdirectory("size" + s + "blocky");
                    DirectoryInfo sdi = root.CreateSubdirectory("size" + s + "sloped");
                    //RenderOrthoMultiSize(TransformLogic.SealGaps(colors2), ySize, xSize, zSize, o, s)
                    renderLargeOrtho(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_S" + fs, ImageFormat.Png); //s
                    renderLarge(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_SE" + fs, ImageFormat.Png);
                    faces0 = FaceLogic.GetFaces(colors2);
                    RenderSmartFaces(faces0, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_SE" + fs, ImageFormat.Png);
                    RenderSmartFacesOrtho(faces0, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_S" + fs, ImageFormat.Png);
                    colors2 = TransformLogic.RotateYaw(colors2, 90);
                    renderLargeOrtho(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_W" + fs, ImageFormat.Png); //w
                    renderLarge(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_SW" + fs, ImageFormat.Png);
                    faces1 = FaceLogic.GetFaces(colors2);
                    RenderSmartFaces(faces1, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_SW" + fs, ImageFormat.Png);
                    RenderSmartFacesOrtho(faces1, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_W" + fs, ImageFormat.Png);
                    colors2 = TransformLogic.RotateYaw(colors2, 90);
                    renderLargeOrtho(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_N" + fs, ImageFormat.Png); //n
                    renderLarge(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_NW" + fs, ImageFormat.Png);
                    faces2 = FaceLogic.GetFaces(colors2);
                    RenderSmartFaces(faces2, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_NW" + fs, ImageFormat.Png);
                    RenderSmartFacesOrtho(faces2, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_N" + fs, ImageFormat.Png);
                    colors2 = TransformLogic.RotateYaw(colors2, 90);
                    renderLargeOrtho(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_E" + fs, ImageFormat.Png); //e
                    renderLarge(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_NE" + fs, ImageFormat.Png);
                    faces3 = FaceLogic.GetFaces(colors2);
                    RenderSmartFaces(faces3, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_NE" + fs, ImageFormat.Png);
                    RenderSmartFacesOrtho(faces3, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_E" + fs, ImageFormat.Png);
                }
            }
            else
            {
                bool cubic = multiplier < 0;
                multiplier = Math.Abs(multiplier);
                if (multiplier == 0) multiplier = 1;
                for (int f = 0; f < frames; f++)
                {
                    string fs = $"_{f:D2}.png";
                    di = root.CreateSubdirectory("sizeBase");
                    renderSmart(parsed[f], bx, by, bz, Direction.SE, o, true).Save(di.FullName + SEP + u + "_SE" + fs, ImageFormat.Png); //se
                    renderSmart(parsed[f], bx, by, bz, Direction.SW, o, true).Save(di.FullName + SEP + u + "_SW" + fs, ImageFormat.Png); //sw
                    renderSmart(parsed[f], bx, by, bz, Direction.NW, o, true).Save(di.FullName + SEP + u + "_NW" + fs, ImageFormat.Png); //nw
                    renderSmart(parsed[f], bx, by, bz, Direction.NE, o, true).Save(di.FullName + SEP + u + "_NE" + fs, ImageFormat.Png); //ne

                    renderSmartOrtho(parsed[f], bx, by, bz, OrthoDirection.S, o, true).Save(di.FullName + SEP + u + "_S" + fs, ImageFormat.Png); //s
                    renderSmartOrtho(parsed[f], bx, by, bz, OrthoDirection.W, o, true).Save(di.FullName + SEP + u + "_W" + fs, ImageFormat.Png); //w
                    renderSmartOrtho(parsed[f], bx, by, bz, OrthoDirection.N, o, true).Save(di.FullName + SEP + u + "_N" + fs, ImageFormat.Png); //n
                    renderSmartOrtho(parsed[f], bx, by, bz, OrthoDirection.E, o, true).Save(di.FullName + SEP + u + "_E" + fs, ImageFormat.Png); //e

                    di = root.CreateSubdirectory("sizeAbove");

                    renderSmart45(parsed[f], bx, by, bz, Direction.SE, o, true).Save(di.FullName + SEP + u + "_Above_SE" + fs, ImageFormat.Png); //se
                    renderSmart45(parsed[f], bx, by, bz, Direction.SW, o, true).Save(di.FullName + SEP + u + "_Above_SW" + fs, ImageFormat.Png); //sw
                    renderSmart45(parsed[f], bx, by, bz, Direction.NW, o, true).Save(di.FullName + SEP + u + "_Above_NW" + fs, ImageFormat.Png); //nw
                    renderSmart45(parsed[f], bx, by, bz, Direction.NE, o, true).Save(di.FullName + SEP + u + "_Above_NE" + fs, ImageFormat.Png); //ne

                    renderSmartOrtho45(parsed[f], bx, by, bz, OrthoDirection.S, o, true).Save(di.FullName + SEP + u + "_Above_S" + fs, ImageFormat.Png); //s
                    renderSmartOrtho45(parsed[f], bx, by, bz, OrthoDirection.W, o, true).Save(di.FullName + SEP + u + "_Above_W" + fs, ImageFormat.Png); //w
                    renderSmartOrtho45(parsed[f], bx, by, bz, OrthoDirection.N, o, true).Save(di.FullName + SEP + u + "_Above_N" + fs, ImageFormat.Png); //n
                    renderSmartOrtho45(parsed[f], bx, by, bz, OrthoDirection.E, o, true).Save(di.FullName + SEP + u + "_Above_E" + fs, ImageFormat.Png); //e

                    byte[,,] colors = TransformLogic.VoxListToArray(parsed[f], xSize, ySize, zSize);
                    FaceVoxel[,,] faces0 = FaceLogic.GetFaces(colors),
                        faces1 = FaceLogic.GetFaces(TransformLogic.RotateYaw(colors, 90)),
                        faces2 = FaceLogic.GetFaces(TransformLogic.RotateYaw(colors, 180)),
                        faces3 = FaceLogic.GetFaces(TransformLogic.RotateYaw(colors, 270));
                    di = root.CreateSubdirectory("sizeBaseSloped");
                    RenderSmartFaces(faces0, xSize, ySize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_SE" + fs, ImageFormat.Png); //se
                    RenderSmartFacesOrtho(faces0, xSize, ySize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_S" + fs, ImageFormat.Png); //s
                    RenderSmartFaces(faces1, ySize, xSize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_SW" + fs, ImageFormat.Png); //sw
                    RenderSmartFacesOrtho(faces1, xSize, ySize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_W" + fs, ImageFormat.Png); //w
                    RenderSmartFaces(faces2, xSize, ySize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_NW" + fs, ImageFormat.Png); //nw
                    RenderSmartFacesOrtho(faces2, xSize, ySize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_N" + fs, ImageFormat.Png); //n
                    RenderSmartFaces(faces3, ySize, xSize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_NE" + fs, ImageFormat.Png); //ne
                    RenderSmartFacesOrtho(faces3, xSize, ySize, zSize, o, true).Save(di.FullName + SEP + u + "_Slope_E" + fs, ImageFormat.Png); //e

                    di = root.CreateSubdirectory("sizeSmallSloped");

                    RenderSmartFacesSmall(faces0, bx, by, bz, o, true).Save(di.FullName + SEP + u + "_Small_Slope_SE" + fs, ImageFormat.Png); //se
                    RenderSmartFacesSmall(faces1, by, bx, bz, o, true).Save(di.FullName + SEP + u + "_Small_Slope_SW" + fs, ImageFormat.Png); //sw
                    RenderSmartFacesSmall(faces2, bx, by, bz, o, true).Save(di.FullName + SEP + u + "_Small_Slope_NW" + fs, ImageFormat.Png); //nw
                    RenderSmartFacesSmall(faces3, by, bx, bz, o, true).Save(di.FullName + SEP + u + "_Small_Slope_NE" + fs, ImageFormat.Png); //ne


                    //for (int s = 1; s <= multiplier; s++)
                    //{
                    //    byte[,,] colors2;
                    //    if (s > 1) colors2 = TransformLogic.RunCA(TransformLogic.ScalePartial(colors, s), s);
                    //    else colors2 = colors.Replicate();
                    //    RenderOrthoMultiSize(TransformLogic.SealGaps(colors2), xSize, ySize, zSize, o, s).Save(di.FullName + SEP + u + "_Size" + s + "_S" + fs, ImageFormat.Png); //s
                    //    RenderOrthoMultiSize(TransformLogic.SealGaps(TransformLogic.RotateYaw(colors2, 90)), ySize, xSize, zSize, o, s).Save(di.FullName + SEP + u + "_Size" + s + "_W" + fs, ImageFormat.Png); //w
                    //    RenderOrthoMultiSize(TransformLogic.SealGaps(TransformLogic.RotateYaw(colors2, 180)), xSize, ySize, zSize, o, s).Save(di.FullName + SEP + u + "_Size" + s + "_N" + fs, ImageFormat.Png); //n
                    //    RenderOrthoMultiSize(TransformLogic.SealGaps(TransformLogic.RotateYaw(colors2, 270)), ySize, xSize, zSize, o, s).Save(di.FullName + SEP + u + "_Size" + s + "_E" + fs, ImageFormat.Png); //e
                    //}
                    byte[,,] colors2;
                    colors2 = FaceLogic.FaceArrayToByteArray(FaceLogic.DoubleSize(faces0));

                    di = root.CreateSubdirectory("sizeBig");

                    renderLargeOrtho(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_S" + fs, ImageFormat.Png); //s
                    renderLarge(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_SE" + fs, ImageFormat.Png); //sw
                    colors2 = TransformLogic.RotateYaw(colors2, 90);
                    renderLargeOrtho(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_W" + fs, ImageFormat.Png); //w
                    renderLarge(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_SW" + fs, ImageFormat.Png); //nw
                    colors2 = TransformLogic.RotateYaw(colors2, 90);
                    renderLargeOrtho(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_N" + fs, ImageFormat.Png); //n
                    renderLarge(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_NW" + fs, ImageFormat.Png); //ne
                    colors2 = TransformLogic.RotateYaw(colors2, 90);
                    renderLargeOrtho(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_E" + fs, ImageFormat.Png); //e
                    renderLarge(colors2, xSize * 2, ySize * 2, zSize * 2, o).Save(di.FullName + SEP + u + "_Big_NE" + fs, ImageFormat.Png); //se

                    for (int s = 1; s <= multiplier; s++)
                    {
                        if (s > 1)
                            colors2 = cubic
                               ? TransformLogic.ScalePartial(colors, s)
                               : TransformLogic.RunCA(TransformLogic.ScalePartial(colors, s), s);
                        else
                            colors2 = colors.Replicate();
                        di = root.CreateSubdirectory("size" + s + "blocky");
                        DirectoryInfo sdi = root.CreateSubdirectory("size" + s + "sloped");
                        //RenderOrthoMultiSize(TransformLogic.SealGaps(colors2), ySize, xSize, zSize, o, s)
                        renderLargeOrtho(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_S" + fs, ImageFormat.Png); //s
                        renderLarge(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_SE" + fs, ImageFormat.Png);
                        faces0 = FaceLogic.GetFaces(colors2);
                        RenderSmartFaces(faces0, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_SE" + fs, ImageFormat.Png);
                        RenderSmartFacesOrtho(faces0, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_S" + fs, ImageFormat.Png);
                        colors2 = TransformLogic.RotateYaw(colors2, 90);
                        renderLargeOrtho(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_W" + fs, ImageFormat.Png); //w
                        renderLarge(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_SW" + fs, ImageFormat.Png);
                        faces1 = FaceLogic.GetFaces(colors2);
                        RenderSmartFaces(faces1, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_SW" + fs, ImageFormat.Png);
                        RenderSmartFacesOrtho(faces1, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_W" + fs, ImageFormat.Png);
                        colors2 = TransformLogic.RotateYaw(colors2, 90);
                        renderLargeOrtho(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_N" + fs, ImageFormat.Png); //n
                        renderLarge(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_NW" + fs, ImageFormat.Png);
                        faces2 = FaceLogic.GetFaces(colors2);
                        RenderSmartFaces(faces2, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_NW" + fs, ImageFormat.Png);
                        RenderSmartFacesOrtho(faces2, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_N" + fs, ImageFormat.Png);
                        colors2 = TransformLogic.RotateYaw(colors2, 90);
                        renderLargeOrtho(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_E" + fs, ImageFormat.Png); //e
                        renderLarge(colors2, xSize * s, ySize * s, zSize * s, o).Save(di.FullName + SEP + u + "_Size" + s + "_NE" + fs, ImageFormat.Png);
                        faces3 = FaceLogic.GetFaces(colors2);
                        RenderSmartFaces(faces3, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_NE" + fs, ImageFormat.Png);
                        RenderSmartFacesOrtho(faces3, xSize * s, ySize * s, zSize * s, o, true).Save(sdi.FullName + SEP + u + "_Size" + s + "_Slope_E" + fs, ImageFormat.Png);
                    }
                }
            }
        }
        static void Main(string[] args)
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream imageStream = assembly.GetManifestResourceStream("IsoVoxel.cube_soft.png");
            cube = new Bitmap(imageStream);
            imageStream = assembly.GetManifestResourceStream("IsoVoxel.cube_ortho.png");
            ortho = new Bitmap(imageStream);
            imageStream = assembly.GetManifestResourceStream("IsoVoxel.white.png");
            white = new Bitmap(imageStream);
            imageStream = assembly.GetManifestResourceStream("IsoVoxel.gradient.png");
            gradient = new Bitmap(imageStream);
            //string voxfile = "Red_Fish_Animated.vox";
            string voxfile = "Zombie.vox";
            if (args.Length >= 1)
            {
                voxfile = args[0];
            }
            else
            {
                Console.WriteLine("Args: 'file x y z m o'. file is a MagicaVoxel .vox file, x y z are sizes,");
                Console.WriteLine("m is an integer multiplier to draw larger renders up to that size");
                Console.WriteLine("  (if m is negative it renders the same sizes but without smoothing),");
                Console.WriteLine("o must be one of these words, changing how outlines are drawn (default light):");
                Console.WriteLine("  outline=full    Draw a black outer outline and shaded inner outlines.");
                Console.WriteLine("  outline=light   Draw a shaded outer outline and shaded inner outlines.");
                Console.WriteLine("  outline=partial Draw no outer outline and shaded inner outlines.");
                Console.WriteLine("  outline=none    Draw no outlines.");
                Console.WriteLine("x y z m o are all optional, but o must be the last if present.");
                Console.WriteLine("x, y, z, and m are given as just a number; they don't use an '=' sign.");
                Console.WriteLine("This outputs each size multiple in its own subdirectory.");
                Console.WriteLine("Defaults: runs on Zombie.vox with x y z set by the model, m is 3, o is light.");
                Console.WriteLine("An example command:");
                Console.WriteLine("        IsoVoxel.exe Truck.vox 60 60 40 2 outline=partial");
                Console.WriteLine("This sets the size of the rendered space to 60x60x40 (which is larger than the");
                Console.WriteLine("  actual model), renders 2 multiplied sizes, and uses only some outlines.");
                Console.WriteLine("Given no arguments, running on " + voxfile + " ...");
            }
            int x = 0, y = 0, z = 0;
            int m = 3;
            Outlining o = Outlining.Light;
            int al = args.Length;
            if (al >= 2 && args.Last().StartsWith("outline", StringComparison.OrdinalIgnoreCase))
            {
                o = GetOutlining(args.Last().ToLowerInvariant().Split('=').Last());
                --al;
            }
            try
            {
                if (al >= 2)
                {
                    x = int.Parse(args[1]);
                }
                if (al >= 3)
                {
                    y = int.Parse(args[2]);
                }
                if(al >= 4)
                {
                    z = int.Parse(args[3]);
                }
                if(al >= 5)
                {
                    m = int.Parse(args[4]);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Args: 'file x y z m o'. file is a MagicaVoxel .vox file, x y z are sizes,");
                Console.WriteLine("m is an integer multiplier to draw larger renders up to that size");
                Console.WriteLine("  (if m is negative it renders the same sizes but without smoothing),");
                Console.WriteLine("o must be one of these words, changing how outlines are drawn (default light):");
                Console.WriteLine("  outline=full    Draw a black outer outline and shaded inner outlines.");
                Console.WriteLine("  outline=light   Draw a shaded outer outline and shaded inner outlines.");
                Console.WriteLine("  outline=partial Draw no outer outline and shaded inner outlines.");
                Console.WriteLine("  outline=none    Draw no outlines.");
                Console.WriteLine("x y z m o are all optional, but o must be the last if present.");
                Console.WriteLine("x, y, z, and m are given as just a number; they don't use an '=' sign.");
                Console.WriteLine("This outputs each size multiple in its own subdirectory.");
                Console.WriteLine("Defaults: runs on Zombie.vox with x y z set by the model, m is 3, o is light.");
                Console.WriteLine("An example command:");
                Console.WriteLine("        IsoVoxel.exe Truck.vox 60 60 40 2 outline=partial");
                Console.WriteLine("This sets the size of the rendered space to 60x60x40 (which is larger than the");
                Console.WriteLine("  actual model), renders 2 multiplied sizes, and uses only some outlines.");
            }
            x = y = Math.Max(x, y);
            BinaryReader bin = new BinaryReader(File.Open(voxfile, FileMode.Open));
            MagicaVoxelData[][] mvd = FromMagica(bin, x, y, z);
            rendered = StoreColorCubes();
            rendered45 = StoreColorCubes45();
            renderedOrtho = StoreColorCubesOrtho();
            StoreColorCubesFaces();
            StoreColorCubesFacesSmall();
            StoreColorCubesFacesOrtho();
            ProcessUnitSmart(mvd, voxfile, x, y, z, o, m);
            bin.Close();
        }

        private static Outlining GetOutlining(string s)
        {
            switch(s)
            {
                case "full": return Outlining.Full;
                case "light": return Outlining.Light;
                case "partial": return Outlining.Partial;
                case "none": return Outlining.None;
            }
            return Outlining.Full;
        }
    }
}
