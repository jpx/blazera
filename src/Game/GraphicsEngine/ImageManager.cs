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
            this.Init(GameDatas.IMAGES_DEFAULT_PATH);
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
            this.Textures = new Dictionary<String, SFML.Graphics.Texture>();
            this.TexturesPath = texturePath;
        }

        public SFML.Graphics.Texture GetTexture(String texturePath)
        {
            if (this.Textures.ContainsKey(texturePath))
                return this.Textures[texturePath];

            SFML.Graphics.Texture tmp = null;

            try
            {
                tmp = new SFML.Graphics.Texture(this.TexturesPath + texturePath);
            }
            catch
            {
                Log.Clerr("Failed to load image : " + this.TexturesPath + texturePath);

                return null;
            }

            this.Textures.Add(texturePath, tmp);
            return tmp;
        }

        public void DeleteTexture(String texturePath)
        {
            if (this.Textures.ContainsKey(texturePath))
                this.Textures.Remove(texturePath);
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
