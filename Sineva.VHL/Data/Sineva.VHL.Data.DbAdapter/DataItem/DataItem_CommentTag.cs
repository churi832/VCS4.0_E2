using Sineva.VHL.Library;
using Sineva.VHL.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sineva.VHL.Data.DbAdapter
{
    [Serializable()]
    public class DataItem_CommentTag : DataItem
    {
        #region Fields
        private string m_TagID = string.Empty;
        private SerializableDictionary<Language, string> m_Comment = new SerializableDictionary<Language, string>();
        #endregion

        #region Properties
        [DatabaseSettingAttribute(true)]
        public string TagID
        {
            get { return m_TagID; }
            set { m_TagID = value; }
        }
        [DatabaseSettingAttribute(true)]
        public SerializableDictionary<Language, string> Comment
        {
            get { return m_Comment; }
            set { m_Comment = value; }
        }
        #endregion

        #region Constructors
        public DataItem_CommentTag()
        {
            try
            {
                List<Language> languages = Enum.GetValues(typeof(Language)).Cast<Language>().ToList();
                foreach (Language language in languages)
                {
                    m_Comment.Add(language, string.Empty);
                }
            }
            catch
            {
            }
        }
        public DataItem_CommentTag(string tagID, string chinese, string english, string korean)
        {
            try
            {
                this.m_TagID = tagID;
                this.m_Comment.Add(Language.Chinese, chinese);
                this.m_Comment.Add(Language.English, english);
                this.m_Comment.Add(Language.Korean, korean);
            }
            catch
            {
            }
        }
        #endregion

        #region Methods
        public void SetCopy(DataItem_CommentTag source)
        {
            try
            {
                this.m_TagID = source.TagID;

                this.m_Comment.Clear();
                foreach (Language language in source.Comment.Keys)
                {
                    this.m_Comment.Add(language, source.Comment[language]);
                }
            }
            catch
            {
            }
        }
        public DataItem_CommentTag GetCopyOrNull()
        {
            try
            {
                return (DataItem_CommentTag)base.MemberwiseClone();
            }
            catch
            {
                return null;
            }
        }
        public string GetCommentByLanguage(Language language)
        {
            try
            {
                if (this.m_Comment.ContainsKey(language) == false) return string.Empty;

                return this.m_Comment[language];
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
