using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moov2.Orchard.FindReplace.Services {
    public class FindReplaceService : IFindReplaceService {
        #region Dependencies

        private readonly IContentManager _contentManager;
        private readonly ITransactionManager _transactionManager;
        private readonly ShellSettings _shellSettings;

        #endregion

        #region Constructor

        public FindReplaceService(IContentManager contentManager, ShellSettings shellSettings, ITransactionManager transactionManager) {
            _contentManager = contentManager;
            _shellSettings = shellSettings;
            _transactionManager = transactionManager;
        }

        #endregion

        #region Implementation

        public IEnumerable<IContent> GetContentItems(string term) {
            var encodedTerm = HttpUtility.UrlEncode(term);

            var matches = _transactionManager.GetSession()
                .CreateSQLQuery(string.Format("SELECT ContentItemRecord_Id FROM Orchard_Framework_ContentItemVersionRecord WHERE Latest=1 AND ([Data] LIKE '%{0}%' ESCAPE '!' OR [Data] LIKE '%{1}%' ESCAPE '!')", LikeEscape(term), LikeEscape(encodedTerm)))
                .List<int>();

            return _contentManager.GetMany<ContentItem>(matches, VersionOptions.Published, QueryHints.Empty);
        }

        public void Replace(IList<int> itemIds, string find, string replace)
        {
            var tableName = string.IsNullOrEmpty(_shellSettings.DataTablePrefix) ? "Orchard_Framework_ContentItemVersionRecord" : string.Format("{0}_Orchard_Framework_ContentItemVersionRecord", _shellSettings.DataTablePrefix);

            _transactionManager.GetSession()
                .CreateSQLQuery(string.Format("update {0} set [Data]=REPLACE([Data], '{1}', '{2}') WHERE Latest=1 AND ContentItemRecord_Id IN ({3})", tableName, find, replace, string.Join(",", itemIds)))
                .ExecuteUpdate();

            _transactionManager.GetSession()
                .CreateSQLQuery(string.Format("update {0} set [Data]=REPLACE([Data], '{1}', '{2}') WHERE Latest=1 AND ContentItemRecord_Id IN ({3})", tableName, HttpUtility.UrlEncode(find), HttpUtility.UrlEncode(replace), string.Join(",", itemIds)))
                .ExecuteUpdate();
        }

        #endregion

        #region

        /// <summary>
        /// Escapes any % values that are used in the LIKE expression
        /// https://docs.microsoft.com/en-us/sql/t-sql/language-elements/like-transact-sql#pattern-matching-with-the-escape-clause
        /// </summary>
        private string LikeEscape(string value)
        {
            return value.Replace("%", "!%");
        }

        #endregion

    }

    public interface IFindReplaceService : IDependency {
        IEnumerable<IContent> GetContentItems(string term);
        void Replace(IList<int> itemIds, string find, string replace);
    }
}