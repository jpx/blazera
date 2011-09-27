using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Audio;

namespace BlazeraLib
{
    public class SoundManager
    {
        public Boolean IsActived = true;

        private SoundManager()
        {
            this.Sounds = new Dictionary<string, Sound>();
            this.SoundBuffers = new Dictionary<string, SoundBuffer>();
            this.Musics = new Dictionary<string, Music>();
        }

        public void Init()
        {
            this.Init(GameDatas.SOUNDS_DEFAULT_PATH);
        }

        private static SoundManager _instance;
        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    SoundManager.Instance = new SoundManager();
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        public void Init(String soundsPath)
        {
            this.Sounds = new Dictionary<String, Sound>();
            this.SoundsPath = soundsPath;
        }

        public void PlaySound(String soundPath)
        {
            if (!this.Sounds.ContainsKey(soundPath))
            {
                this.SoundBuffers.Add(soundPath, new SoundBuffer(this.SoundsPath + soundPath));
                this.Sounds.Add(soundPath, new Sound(this.SoundBuffers[soundPath]));
            }
            if (this.IsActived)
                this.Sounds[soundPath].Play();
        }

        private Music GetMusic(String musicPath)
        {
            if (this.Musics.ContainsKey(musicPath))
                return Musics[musicPath];
            this.Musics.Add(musicPath, new Music(this.SoundsPath + musicPath));
            return this.Musics[musicPath];
        }

        public void PlayMusic(String musicPath)
        {
            foreach (Music music in this.Musics.Values)
                music.Stop();
            if (this.IsActived)
                this.GetMusic(musicPath).Play();
        }

        public void PlayMusic(String musicPath, int volume)
        {
            this.Stop();
            if (this.IsActived)
            {
                this.GetMusic(musicPath).Volume = volume;
                this.GetMusic(musicPath).Play();
            }
        }

        public void Stop()
        {
            foreach (Music music in this.Musics.Values)
                music.Stop();
        }

        private Dictionary<String, Sound> Sounds
        {
            get;
            set;
        }

        private Dictionary<String, SoundBuffer> SoundBuffers
        {
            get;
            set;
        }

        private Dictionary<String, Music> Musics
        {
            get;
            set;
        }

        private String SoundsPath
        {
            get;
            set;
        }
    }
}