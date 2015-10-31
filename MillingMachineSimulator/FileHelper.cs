using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using Windows.Storage;

namespace MillingMachineSimulator
{
    public class FileHelper
    {
        GraphicsDevice device;

        public enum FrezType { F, K };
        public FrezType Frez = FrezType.K; 
        public int Diameter = 10;
        private static Regex regex = new Regex(@"N[0-9]+G([0-9][0-9])X([-0-9]+[.0-9]*)Y([-0-9]+[.0-9]*)Z([-0-9]+[.0-9]*)");
        private StreamReader reader;


        public FileHelper(GraphicsDevice device)
        {
            this.device = device;        
        }

        public async void FileLoad(StorageFile file)
        {
            var stream = await file.OpenStreamForReadAsync();
            reader = new StreamReader(stream);
            var regex = new Regex(@"([kf])([0-9]+)");
            var match = regex.Match(file.FileType);
            if (match.Groups[1].Value.Equals("f"))
                Frez = FrezType.F;
            else if (match.Groups[1].Value.Equals("k"))
                Frez = FrezType.K;           
            Diameter = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
        }

        public Vector3 ReadNextLine(int resolution)
        {
            if (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var match = regex.Match(line);
                var g = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                var x = float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                var y = float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                var z = float.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);
                return new Vector3(x, z, y);
            }
            else { return new Vector3(0, 0, 0); }
        }

        public void ReadAllLines(int resolution)
        {
            Vector3 begin;
            Vector3 end;
            var line = reader.ReadLine();
            var match = regex.Match(line);
            var g = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            var x = float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
            var y = float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
            var z = float.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);
            end = new Vector3(x, z, y);

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();

                match = regex.Match(line);
                g = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                x = float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                y = float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                z = float.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);

                begin = end;
                end = new Vector3(x, z, y);

                begin *= resolution;
                end *= resolution;
                //Brick.MoveFrez(GetPositions(begin, end));
            }
        }

        public List<Vector3> GetPositions(Vector3 begin, Vector3 end, int resolution)
        {
            begin *= resolution;
            end *= resolution;
            Vector3 delta = end - begin;
           
            List<Vector3> positions = new List<Vector3>();
            float max = Math.Max(Math.Abs(end.X - begin.X), Math.Abs(end.Z - begin.Z)); //max from 3 directions?
            for (int i = 0; i < (int)max; i++)
            {
                positions.Add(begin + delta / max * i);
            }
            return positions;
        }

        
    }
}
