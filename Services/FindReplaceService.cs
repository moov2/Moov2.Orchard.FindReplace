using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Environment.Configuration;
using Orchard.Security;
using Orchard.Services;
using System.Collections.Generic;
using System.Web;

namespace Moov2.Orchard.FindReplace.Services {
    public class FindReplaceService : IFindReplaceService {
        #region Dependencies

        private readonly IAuthenticationService _authenticationService;
        private readonly IClock _clock;
        private readonly IContentManager _contentManager;
        private readonly ITransactionManager _transactionManager;
        private readonly ShellSettings _shellSettings;

        #endregion

        #region Constructor

        public FindReplaceService(IAuthenticationService authenticationService, IClock clock, IContentManager contentManager, ShellSettings shellSettings, ITransactionManager transactionManager) {
            _authenticationService = authenticationService;
            _clock = clock;
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
            var session = _transactionManager.GetSession();

            session.CreateSQLQuery(string.Format("update {0} set [Data]=REPLACE([Data], '{1}', '{2}') WHERE Latest=1 AND ContentItemRecord_Id IN ({3})", tableName, find, replace, string.Join(",", itemIds)))
                .ExecuteUpdate();

            session.CreateSQLQuery(string.Format("update {0} set [Data]=REPLACE([Data], '{1}', '{2}') WHERE Latest=1 AND ContentItemRecord_Id IN ({3})", tableName, HttpUtility.UrlEncode(find), HttpUtility.UrlEncode(replace), string.Join(",", itemIds)))
                .ExecuteUpdate();

            var contentItems = _contentManager.GetMany<ContentItem>(itemIds, VersionOptions.Published, QueryHints.Empty);
            var utcNow = _clock.UtcNow;

            foreach (var contentItem in contentItems)
            {
                contentItem.As<CommonPart>().ModifiedUtc = utcNow;
                contentItem.As<CommonPart>().VersionModifiedUtc = utcNow;
                contentItem.As<CommonPart>().VersionModifiedBy = GetUserName();
            }
        }

        #endregion

        #region

        private string GetUserName()
        {
            var user = _authenticationService.GetAuthenticatedUser();
            return user == null ? string.Empty : user.UserName;
        }

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