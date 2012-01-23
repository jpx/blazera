using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public class TextureManager
    {
        private TextureManager()
        {

        }

        public void Init()
        {
            Init(GameData.IMAGES_DEFAULT_PATH);
        }

        private static TextureManager _instance;
        public static TextureManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    TextureManager.Instance = new TextureManager();
                }

                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        public void Init(String texturePath)
        {
            Textures = new Dictionary<String, SFML.Graphics.Texture>();
            TexturesPath = texturePath;
        }

        public SFML.Graphics.Texture GetTexture(String texturePath)
        {
            if (Textures.ContainsKey(texturePath))
                return Textures[texturePath];

            SFML.Graphics.Texture tmp = null;

            try
            {
                tmp = new SFML.Graphics.Texture(TexturesPath + texturePath);
            }
            catch
            {
                Log.Clerr("Failed to load image : " + TexturesPath + texturePath);

                return null;
            }

            Textures.Add(texturePath, tmp);
            return tmp;
        }

        public void DeleteTexture(String texturePath)
        {
            if (Textures.ContainsKey(texturePath))
                Textures.Remove(texturePath);
        }

        private Dictionary<String, SFML.Graphics.Texture> Textures
        {
            get;
            set;
        }

        private String TexturesPath
        {
            get;
            set;
        }
    }
}
