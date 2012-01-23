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
            Sounds = new Dictionary<string, Sound>();
            SoundBuffers = new Dictionary<string, SoundBuffer>();
            Musics = new Dictionary<string, Music>();
        }

        public void Init()
        {
            Init(GameData.SOUNDS_DEFAULT_PATH);
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
            Sounds = new Dictionary<String, Sound>();
            SoundsPath = soundsPath;
        }

        public void PlaySound(String soundPath)
        {
            if (!Sounds.ContainsKey(soundPath))
            {
                SoundBuffers.Add(soundPath, new SoundBuffer(SoundsPath + soundPath));
                Sounds.Add(soundPath, new Sound(SoundBuffers[soundPath]));
            }
            if (IsActived)
                Sounds[soundPath].Play();
        }

        private Music GetMusic(String musicPath)
        {
            if (Musics.ContainsKey(musicPath))
                return Musics[musicPath];
            Musics.Add(musicPath, new Music(SoundsPath + musicPath));
            return Musics[musicPath];
        }

        public void PlayMusic(String musicPath)
        {
            foreach (Music music in Musics.Values)
                music.Stop();
            if (IsActived)
                GetMusic(musicPath).Play();
        }

        public void PlayMusic(String musicPath, int volume)
        {
            Stop();
            if (IsActived)
            {
                GetMusic(musicPath).Volume = volume;
                GetMusic(musicPath).Play();
            }
        }

        public void Stop()
        {
            foreach (Music music in Musics.Values)
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