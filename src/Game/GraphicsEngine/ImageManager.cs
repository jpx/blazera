using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public class ImageManager
    {
        private ImageManager()
        {

        }

        public void Init()
        {
            this.Init(GameDatas.IMAGES_DEFAULT_PATH);
        }

        private static ImageManager _instance;
        public static ImageManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    ImageManager.Instance = new ImageManager();
                }

                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        public void Init(String imagesPath)
        {
            this.Images = new Dictionary<String, Image>();
            this.ImagesPath = imagesPath;
        }

        public Image GetImage(String imagePath)
        {
            if (this.Images.ContainsKey(imagePath))
                return this.Images[imagePath];

            Image tmp = null;

            try
            {
                tmp = new Image(this.ImagesPath + imagePath);
            }
            catch
            {
                Log.Clerr("Failed to load image : " + this.ImagesPath + imagePath);

                return null;
            }

            tmp.Smooth = false;
            this.Images.Add(imagePath, tmp);
            return tmp;
        }

        public void DeleteImage(String imagePath)
        {
            if (this.Images.ContainsKey(imagePath))
                this.Images.Remove(imagePath);
        }

        private Dictionary<String, Image> Images
        {
            get;
            set;
        }

        private String ImagesPath
        {
            get;
            set;
        }
    }
}
