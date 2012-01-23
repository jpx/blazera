using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace BlazeraLib
{
    public class FileReader
    {
        private FileReader()
        {

        }

        public void Init()
        {

        }

        private static FileReader _instance;
        public static FileReader Instance
        {
            get
            {
                if (_instance == null)
                {
                    FileReader.Instance = new FileReader();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public String GetString(String file)
        {
            try
            {
                String str;

                using (StreamReader sr = new StreamReader(file))
                {
                    str = sr.ReadToEnd();
                }

                return str;
            }
            catch
            {
                Log.Cl("Impossible d'atteindre le fichier " + file);
            }

            return null;
        }

        public Char[] GetCharArray(String file)
        {
            String str = GetString(file);

            if (str == null)
                return null;

            return str.ToCharArray();
        }

        public String GetNextWord(Char[] buffer, Int32 id, Char border)
        {
            Int32 i = id;
            while (i < buffer.Length && buffer[i] != border)
            {
                i++;
            }

            return new String(buffer, id, i - id);
        }

        public List<String> GetWords(Char[] buffer, Char border)
        {
            List<String> words = new List<String>();
            Int32 id = 0;

            while (id < buffer.Length - 1 && buffer[id] != '\0')
            {
                String word = GetNextWord(buffer, id, border);
                words.Add(word);
                id += (id + word.Length + 1 < buffer.Length) ? word.Length + 1 : buffer.Length - id - 1;
            }

            return words;
        }

        public List<List<String>> GetGround(String groundType)
        {
            Char[] buffer = GetCharArray(GameData.SCRIPTS_DEFAULT_PATH + "Ground/Ground_" + groundType);

            if (buffer == null)
                return null;

            List<List<String>> ground = new List<List<String>>();

            IEnumerator<String> iEnum = GetWords(buffer, ';').GetEnumerator();

            while (iEnum.MoveNext())
            {
                ground.Add(GetWords(iEnum.Current.ToCharArray(), ','));
            }

            return ground;
        }

        private List<String> GetFiles(String path, Boolean isScript = true)
        {
            String folder = isScript ? GameData.SCRIPTS_DEFAULT_PATH : GameData.IMAGES_DEFAULT_PATH;
            String longPath = folder + path;

            int subFolderOffset = 0;
            if (!isScript && path != "")
                subFolderOffset = 1;

            List<String> fileNames = new List<String>();

            List<String> files = Directory.GetFiles(longPath).ToList();

            IEnumerator<String> filesEnum = files.GetEnumerator();
            while (filesEnum.MoveNext())
            {
                String extension = isScript ? ".lua" : "";
                fileNames.Add(filesEnum.Current.Substring(longPath.Length + subFolderOffset, filesEnum.Current.Length - longPath.Length - extension.Length - subFolderOffset));
            }

            return fileNames;
        }

        public List<String> GetMapTypes()
        {
            List<String> maps = GetFiles("Map/");

            for (Int32 count = 0; count < maps.Count; count++)
            {
                maps[count] = maps[count].Substring("Map_".Length, maps[count].Length - "Map_".Length);
            }

            return maps;
        }

        public List<String> GetTextureTypes(String[] filter = null)
        {
            List<String> textures = GetFiles("Texture/");

            for (Int32 count = 0; count < textures.Count; count++)
            {
                textures[count] = textures[count].Substring("Texture_".Length, textures[count].Length - "Texture_".Length);
            }

            if (filter == null)
                return textures;

            Int32 totFilterLength = 0;
            foreach (String str in filter)
                totFilterLength += str.Length + 1;

            Int32 curFilterLength = 0;

            List<String> filteredTextures = new List<String>();

            Boolean[] validTextures = new Boolean[textures.Count];
            for (Int32 count = 0; count < validTextures.Length; ++count)
                validTextures[count] = true;

            for (Int32 filterCount = 0; filterCount < filter.Length; ++filterCount)
            {
                for (Int32 texturesCount = 0; texturesCount < textures.Count; ++texturesCount)
                {
                    if (textures[texturesCount].Length <= totFilterLength)
                    {
                        validTextures[texturesCount] = false;
                        continue;
                    }

                    if (textures[texturesCount].Substring(curFilterLength, filter[filterCount].Length + 1) != filter[filterCount] + "_")
                    {
                        validTextures[texturesCount] = false;
                    }
                }

                curFilterLength += filter[filterCount].Length + 1;
            }

            for (Int32 count = 0; count < textures.Count; ++count)
                if (validTextures[count])
                    filteredTextures.Add(textures[count]);

            return filteredTextures;
        }

        public List<String> GetObjectTypes(String elementBaseType)
        {
            List<String> elements = GetFiles(elementBaseType + "/");

            for (Int32 count = 0; count < elements.Count; count++)
            {
                elements[count] = elements[count].Substring((elementBaseType + "_").Length, elements[count].Length - (elementBaseType + "_").Length);
            }

            return elements;
        }

        public List<String> GetTileTypes()
        {
            List<String> tiles = GetFiles("Tile/");

            for (Int32 count = 0; count < tiles.Count; count++)
            {
                tiles[count] = tiles[count].Substring("Tile_".Length, tiles[count].Length - "Tile_".Length);
            }

            return tiles;
        }

        public List<String> GetTileSetTypes()
        {
            List<String> tileSets = GetFiles("TileSet/");

            for (Int32 count = 0; count < tileSets.Count; count++)
            {
                tileSets[count] = tileSets[count].Substring("TileSet_".Length, tileSets[count].Length - "TileSet_".Length);
            }

            return tileSets;
        }

        public List<String> GetImages(string subFolderName = "")
        {
            if (subFolderName != "")
                subFolderName = "/" + subFolderName;

            return GetFiles(subFolderName, false);
        }
    }
}
