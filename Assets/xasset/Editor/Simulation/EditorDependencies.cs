using System.Collections.Generic;
using UnityEditor;

namespace xasset.editor
{
    public class EditorDependencies
    {
        public string pathOrURL;
        private static readonly Dictionary<string, int> _references = new Dictionary<string, int>();
        public string[] dependencies;
        public bool unused => !_references.TryGetValue(pathOrURL, out var value) || value == 0;
        private static void AddReference(string key)
        {
            if (!_references.ContainsKey(key))
            {
                _references[key] = 1;
            }
            else
            {
                _references[key] += 1;
            }
        }
        
        private static void ReleaseReference(string key)
        {
            if (_references.ContainsKey(key))
            {
                _references[key] -= 1;
            }
        }

        public void Load()
        {
            dependencies = AssetDatabase.GetDependencies(pathOrURL); 
            foreach (var dependency in dependencies)
            {
                AddReference(dependency);
            }
        }

        public void Release()
        {
            foreach (var dependency in dependencies)
            {
                ReleaseReference(dependency);
            }
        }
    }
}