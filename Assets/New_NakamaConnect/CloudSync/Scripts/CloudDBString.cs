using ProjectCore.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectCore.SocialFeature.Internal
{
    [InlineEditor]
    [CreateAssetMenu(menuName = "ProjectCore/SocialFeature/Variables/CloudCloudDBString", fileName = "v_", order = 0)]
    public sealed class CloudDBString : String, IDBVariable
    {
        [SerializeField] private string Key;
        [SerializeField] private bool CanCloudSave;
        
        public void SetKey(string key)
        {
            Key = key;
        }

        public string GetKey()
        {
            return Key;
        }

        private void OnEnable()
        {
            Load();
        }

        public void Refresh()
        {
            Load();
        }
        
        [Button]
        public override void SetValue(string value)
        {
            base.SetValue(value);
            Save();
        }

        public override void SetValue(String value)
        {
            base.SetValue(value);
            Save();
        }

        public void Save()
        {
            DBManager.SetString(this, Key, Value);
        }

        public void Load()
        {
            if (!string.IsNullOrEmpty(Key) && DBManager.HasKey(this, Key))
            {
                Value = DBManager.GetString(this, Key);
            }
            else
            {
                if (ResetToDefaultOnPlay)
                {
                    Value = DefaultValue;
                }
                else
                {
                    Value = string.Empty;
                }
            }
        }

        void IDBVariable.Update(object value)
        {
            if (!CanCloudSave) return;
            if (value is string val)
            {
                SetValue(val);
            }
        }

        object IDBVariable.GetValue()
        {
            return GetValue();
        }
    }
}