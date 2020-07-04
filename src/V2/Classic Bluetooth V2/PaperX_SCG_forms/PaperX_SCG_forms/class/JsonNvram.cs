using Android.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PaperX_SCG_forms.Classes
{
    public class JsonNvram
    {
        string _path;
        public string FilePath
        {
            get { return _path; }
            private set { _path = value; }
        }

        string _fullFileName;
        public string FullFileName
        {
            get { return _fullFileName; }
            set { _fullFileName = value; }
        }

        private readonly string _key;
        private readonly string _subKey;

        public JsonNvram(string key, string subKey)
        {
            _key = key;
            _subKey = subKey;
        }

        public void CheckPath()
        {
            try
            {
                var localAppDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                _path = System.IO.Path.Combine(localAppDataPath, _key);
                if (!System.IO.Directory.Exists(_path))
                {
                    System.IO.Directory.CreateDirectory(_path);
                }
                _fullFileName = System.IO.Path.Combine(_path, $"{_subKey}.json");
            }
            catch (Exception ex)
            {
                Log.Debug("PPX", ex.Message);
            }
        }

        public void Write(string json)
        {
            try
            {
                if (!File.Exists(_fullFileName))
                {
                    File.WriteAllText(_fullFileName, json);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("PPX", ex.Message);
            }
        }

        public string Read()
        {
            try
            {
                if (!File.Exists(_fullFileName))
                {
                    string text = File.ReadAllText(_fullFileName);
                    return text;
                }
                else
                {
                    throw new Exception("File not found!");
                }
            }
            catch (Exception ex)
            {
                Log.Debug("PPX", ex.Message);
                return string.Empty;
            }
        }
    }
}
