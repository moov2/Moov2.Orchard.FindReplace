using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Environment.Configuration;
using Orchard.Security;
using Orchard.Services;
using System.Collections.Generic;
using System.Web;

namespace Moov2.Orchard.FindReplace.Services
{
    public class FindReplaceService : IFindReplaceService
    {
        #region Dependencies

        private readonly IAuthenticationService _authenticationService;
        private readonly IClock _clock;
        private readonly IContentManager _contentManager;
        private readonly ITransactionManager _transactionManager;
        private readonly ShellSettings _shellSettings;

        #endregion

        #region Constructor

        public FindReplaceService(IAuthenticationService authenticationService, IClock clock, IContentManager contentManager, ShellSettings shellSettings, ITransactionManager transactionManager)
        {
            _authenticationService = authenticationService;
            _clock = clock;
            _contentManager = contentManager;
            _shellSettings = shellSettings;
            _transactionManager = transactionManager;
        }

        #endregion

        #region Implementation

        public IEnumerable<IContent> GetContentItems(string term)
        {
            var tableName = GetContentItemVersionRecordTableName();
            var matches = _transactionManager.GetSession()
                .CreateSQLQuery($"SELECT ContentItemRecord_Id FROM {tableName} WHERE Latest=1 AND ([Data] LIKE :unencoded ESCAPE '!' OR [Data] LIKE :encoded ESCAPE '!')")
                .SetParameter("unencoded", $"%{LikeEscape(term)}%")
                .SetParameter("encoded", $"%{LikeEscape(HttpUtility.UrlEncode(term))}%")
                .List<int>();

            return _contentManager.GetMany<ContentItem>(matches, VersionOptions.Published, QueryHints.Empty);
        }

        public void Replace(IList<int> itemIds, string find, string replace)
        {
            var tableName = GetContentItemVersionRecordTableName();
            var session = _transactionManager.GetSession();

            session.CreateSQLQuery($"update {tableName} set [Data]=REPLACE([Data], :find, :replace) WHERE Latest=1 AND ContentItemRecord_Id IN (:itemIds)")
                .SetParameter("find", find)
                .SetParameter("replace", replace)
                .SetParameterList("itemIds", itemIds)
                .ExecuteUpdate();
            if (find != null && find.Equals(HttpUtility.UrlEncode(find), System.StringComparison.OrdinalIgnoreCase))
            {
                // only run URLencode replace if necessary otherwise
                // if 'replace' contains 'term' you end up with doubled up result
                session.CreateSQLQuery($"update {tableName} set [Data]=REPLACE([Data], :find, :replace) WHERE Latest=1 AND ContentItemRecord_Id IN (:itemIds)")
                    .SetParameter("find", HttpUtility.UrlEncode(find))
                    .SetParameter("replace", HttpUtility.UrlEncode(replace))
                    .SetParameterList("itemIds", itemIds)
                    .ExecuteUpdate();
            }

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

        private string GetContentItemVersionRecordTableName()
        {
            return string.IsNullOrEmpty(_shellSettings.DataTablePrefix) ?
                "Orchard_Framework_ContentItemVersionRecord" :
                string.Format("{0}_Orchard_Framework_ContentItemVersionRecord", _shellSettings.DataTablePrefix);
        }

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

    public interface IFindReplaceService : IDependency
    {
        IEnumerable<IContent> GetContentItems(string term);
        void Replace(IList<int> itemIds, string find, string replace);
    }
}