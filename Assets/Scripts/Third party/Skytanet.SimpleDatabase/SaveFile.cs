using System.Collections.Generic;
using System.IO;
using BplusDotNet;
using Newtonsoft.Json;
using UnityEngine;

namespace Skytanet.SimpleDatabase
{
    public class SaveFile
    {
        private readonly JsonSerializerSettings _defaultSettings =
            new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };

        private string _filename;
        private bool _open;
        private string _path;
        private BplusTree _tree;

        /// <summary>
        ///     Initializes a new instance of the class. Save files will be saved on the
        ///     Application.persistentDataPath with the
        ///     specified filename.
        /// </summary>
        /// <param name="filename">Name of the save file without file extension</param>
        public SaveFile(string filename) : this(filename, Application.persistentDataPath)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the class. Save files will be saved on the specified
        ///     path with the specified
        ///     filename.
        /// </summary>
        /// <param name="filename">Name of the save file without file extension.</param>
        /// <param name="path">Path to the folder that will contain the save files</param>
        public SaveFile(string filename, string path)
        {
            OpenSaveFile(filename, path);
        }

        /// <summary>
        ///     Checks if the save file is open.
        /// </summary>
        public bool IsOpen()
        {
            return _open;
        }

        /// <summary>
        ///     Gets the name of the current save file. Returns null if the save file wasn't
        ///     initialized.
        /// </summary>
        public string GetName()
        {
            return _filename;
        }

        /// <summary>
        ///     Gets the path where the current save file is being saved. Returns null if the save
        ///     file wasn't initialized.
        /// </summary>
        public string GetPath()
        {
            return _path;
        }

        /// <summary>
        ///     Opens again a closed saveFile
        /// </summary>
        public void ReOpen()
        {
            OpenSaveFile(GetName(), GetPath());
        }

        private void OpenSaveFile(string filename, string path)
        {
            if (IsOpen())
            {
                Debug.LogError(
                    $"You are trying to open an already opened save file ('{filename}')");
                return;
            }

            var treeFileName = Path.Combine(path, filename + ".save");
            var blockFileName = Path.Combine(path, filename + ".block");
            try
            {
                _tree = BplusTree.Initialize(treeFileName, blockFileName, 40);
            }
            catch (IOException)
            {
                try
                {
                    _tree = BplusTree.ReOpen(treeFileName, blockFileName);
                }
                catch (DirectoryNotFoundException e)
                {
                    Debug.LogError(
                        "Error while opening the save file, check that the specified directory exists\n" +
                        e);
                    return;
                }
                catch (IOException e)
                {
                    Debug.LogError(
                        $"Error while opening the save file, check that save file '{filename}' is not already open\n" +
                        e);
                    return;
                }
            }

            _open = true;
            _filename = filename;
            _path = path;
        }

        /// <summary>
        ///     Inserts a new key-value pair into the database and commits the changes.
        /// </summary>
        /// <param name="key">Key that identifies the value</param>
        /// <param name="value">Object to be stored</param>
        public void Set(string key, object value)
        {
            if (!IsOpen())
            {
                Debug.LogError(
                    $"You are trying to use 'Set' on a save file that is not open. ('{_filename}')");
                return;
            }

            _tree[key] = JsonConvert.SerializeObject(value, _defaultSettings);
            _tree.Commit();
        }

        /// <summary>
        ///     Gets an object stored in the database identified by a key.
        /// </summary>
        /// <param name="key">Key that identified the object</param>
        /// <param name="defaultValue">(optional) Value to use if the key was not found</param>
        /// <returns>T</returns>
        public T Get<T>(string key, T defaultValue = default)
        {
            if (!IsOpen())
            {
                Debug.LogError(
                    $"You are trying to use 'Get' on a save file that is not open. ('{_filename}')");

                return default;
            }

            if (!_tree.ContainsKey(key)) return defaultValue;

            var json = _tree[key];
            return JsonConvert.DeserializeObject<T>(json, _defaultSettings);
        }

        /// <summary>
        ///     Deletes a key and its value from the database and commits the changes. Does nothing
        ///     if the key doesn't exist.
        /// </summary>
        /// <param name="key">Key to delete</param>
        public void Delete(string key)
        {
            if (!IsOpen())
            {
                Debug.LogError(
                    $"You are trying to use 'Delete' on a save file that is not open. ('{_filename}')");
                return;
            }

            try
            {
                _tree.RemoveKey(key);
                _tree.Commit();
            }
            catch (BplusTreeKeyMissing)
            {
                //ignore when trying to delete an nonexistent key
            }
        }

        /// <summary>
        ///     Checks if a key exists in the database.
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if it exists, false otherwise.</returns>
        public bool HasKey(string key)
        {
            if (!IsOpen())
            {
                Debug.LogError(
                    $"You are trying to use 'HasKey()' on a save file that is not open. ('{_filename}')");
                return false;
            }

            return _tree.ContainsKey(key);
        }

        /// <summary>
        ///     Gets a list of all the keys in the database. This operation may be slow.
        /// </summary>
        /// <returns>A list of keys in the database.</returns>
        public List<string> GetKeys()
        {
            if (!IsOpen())
            {
                Debug.LogError(
                    $"You are trying to use 'getKeys()' on a save file that is not open. ('{_filename}')");
                return null;
            }

            var keys = new List<string>();
            var key = _tree.FirstKey();
            while (key != null)
            {
                keys.Add(key);
                key = _tree.NextKey(key);
            }

            return keys;
        }

        /// <summary>
        ///     Shuts down the database, closing the file streams.
        /// </summary>
        public void Close()
        {
            if (!IsOpen())
            {
                Debug.LogError(
                    $"You are trying to close a save file that is not open. ('{_filename}')");
                return;
            }

            _tree.Abort();
            _tree.Shutdown();
            _open = false;
        }

        /// <summary>
        ///     Tries to delete a save file in the Application.persistentDataPath with the specified
        ///     filename.
        /// </summary>
        /// <param name="filename">Name of the save file without file extension.</param>
        public static void DeleteSaveFile(string filename)
        {
            DeleteSaveFile(filename, Application.persistentDataPath);
        }

        /// <summary>
        ///     Tries to delete a save file in the specified path
        /// </summary>
        /// <param name="filename">Name of the save file without file extension.</param>
        /// <param name="path">Path where the save file is located</param>
        public static void DeleteSaveFile(string filename, string path)
        {
            try
            {
                File.Delete(Path.Combine(path, filename + ".save"));
                File.Delete(Path.Combine(path, filename + ".block"));
            }
            catch (IOException e)
            {
                Debug.LogError(
                    $"Error while deleting the save file, check that save file '{filename}' is not open and that it exists.\n" +
                    e);
            }
        }

        /// <summary>
        ///     Tries to get a list of save files in the Application.persistentDataPath
        /// </summary>
        /// <returns>A list containing the save file names</returns>
        public static string[] GetSaveFileList()
        {
            return GetSaveFileList(Application.persistentDataPath);
        }

        /// <summary>
        ///     Tries to get a list of save files in the specified path
        /// </summary>
        /// <param name="path">Path of the folder</param>
        /// <returns>A list containing the save file names</returns>
        public static string[] GetSaveFileList(string path)
        {
            var info = new DirectoryInfo(path);
            var fileInfo = info.GetFiles("*.save");
            var result = new string[fileInfo.Length];
            for (var i = 0; i < fileInfo.Length; ++i)
                result[i] = Path.GetFileNameWithoutExtension(fileInfo[i].Name);

            return result;
        }
    }
}