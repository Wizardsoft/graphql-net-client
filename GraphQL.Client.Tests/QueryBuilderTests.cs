using GraphQL.Client.QueryBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;

namespace GraphQL.Client.Tests
{
    [TestClass]
    public class QueryBuilderTests
    {
        [TestMethod]
        public void CreateQueryTest()
        {
            var expectedQuery =
@"query($Org:String!, $Items:Int!, $Cursor:String) {
    organization(first:$Items, after:$Cursor) {
        totalCount
        pageInfo {
            endCursor
            hasNextPage
        }
        nodes {
            name
            login
            avatarUrl
        }
    }
}";

            var resultingQuery = GraphQLQueryBuilder.New("query", arguments =>
            {
                arguments.Var("Org").String().Required();
                arguments.Var("Items").Int().Required();
                arguments.Var("Cursor").String();
            }).Query("organization", arguments =>
            {
                arguments.Param("first", "$Items");
                arguments.Param("after", "$Cursor");
            }, select =>
            {
                select.Field("totalCount");
                select.Field("pageInfo").Select(inner =>
                {
                    inner.Field("endCursor");
                    inner.Field("hasNextPage");
                });
                select.Field("nodes").Select(inner =>
                {
                    inner.Field("name");
                    inner.Field("login");
                    inner.Field("avatarUrl");
                });
            }).Build(GraphQLFormatting.Indented);

            Assert.AreEqual(expectedQuery, resultingQuery);
        }

        [TestMethod]
        public void CreateQueryWithoutFormatTest()
        {
            var expectedQuery = "query($Org:String!, $Items:Int!, $Cursor:String) { organization(first:$Items, after:$Cursor) { totalCount pageInfo { endCursor hasNextPage } nodes { name login avatarUrl } } }";

            var resultingQuery = GraphQLQueryBuilder.New("query", arguments =>
            {
                arguments.Var("Org").String().Required();
                arguments.Var("Items").Int().Required();
                arguments.Var("Cursor").String();
            }).Query("organization", arguments =>
            {
                arguments.Param("first", "$Items");
                arguments.Param("after", "$Cursor");
            }, select =>
            {
                select.Field("totalCount");
                select.Field("pageInfo").Select(inner =>
                {
                    inner.Field("endCursor");
                    inner.Field("hasNextPage");
                });
                select.Field("nodes").Select(inner =>
                {
                    inner.Field("name");
                    inner.Field("login");
                    inner.Field("avatarUrl");
                });
            }).Build();

            Assert.AreEqual(expectedQuery, resultingQuery);
        }

        [TestMethod]
        public void TestETMDB()
        {
            //https://etmdb.com/graphql?

           var etmdbQuery = GraphQLQueryBuilder.New()
                .Query("allFilmographyTypes", args =>
                {
                    args.Param("first", "10");
                }, select =>
                {
                    select.Field("edges").Select(edges =>
                    {
                        edges.Field("node").Select(node =>
                        {
                            node.Field("slug");
                        });
                    });
                }).Build(GraphQLFormatting.None);

            var client = new GraphQLClient("https://etmdb.com/graphql");

            var result = client.Query(etmdbQuery);

            Assert.IsFalse(result.Errors.Any());
            Assert.IsNotNull(result.Data);
        }

        [TestMethod]
        public void TestGithubGraphQlQuery()
        {
            var expected = "query($User:String!, $Items:Int!, $Cursor:String) { user(login:$User) { name pullRequests(states:[OPEN, CLOSED, MERGED], first:$Items, after:$Cursor, orderBy:{ direction: DESC, field: CREATED_AT }) { totalCount pageInfo { endCursor } nodes { number state author { login avatarUrl } title repository { name owner login } additions changedFiles deletions closed closedAt createdAt publishedAt locked activeLockReason merged mergedAt mergedBy { login avatarUrl } mergeCommit { oid } mergeable baseRefName headRefName } } } }";

            var resultQuery = GraphQLQueryBuilder.New("query", args =>
            {
                args.Var("User").String().Required();
                args.Var("Items").Int().Required();
                args.Var("Cursor").String();
            }).Query("user", args =>
            {
                args.Param("login", "$User");
            }, select =>
            {
                select.Field("name");
            }).Query("pullRequests", args =>
            {
                args.Param("states", "[OPEN, CLOSED, MERGED]");
                args.Param("first", "$Items");
                args.Param("after", "$Cursor");
                args.Param("orderBy", "{ direction: DESC, field: CREATED_AT }");
            }, select =>
            {
                select.Field("totalCount");
                select.Field("pageInfo").Select(inner => inner.Field("endCursor"));
                select.Field("nodes").Select(inner =>
                {
                    inner.Field("number");
                    inner.Field("state");
                    inner.Field("author").Select(author =>
                    {
                        author.Field("login");
                        author.Field("avatarUrl");
                    });
                    inner.Field("title");
                    inner.Field("repository").Select(repository =>
                    {
                        repository.Field("name");
                        repository.Field("owner").Select(owner => repository.Field("login"));
                    });
                    inner.Field("additions");
                    inner.Field("changedFiles");
                    inner.Field("deletions");
                    inner.Field("closed");
                    inner.Field("closedAt");
                    inner.Field("createdAt");
                    inner.Field("publishedAt");
                    inner.Field("locked");
                    inner.Field("activeLockReason");
                    inner.Field("merged");
                    inner.Field("mergedAt");
                    inner.Field("mergedBy").Select(mergedBy =>
                    {
                        mergedBy.Field("login");
                        mergedBy.Field("avatarUrl");
                    });
                    inner.Field("mergeCommit").Select(mergeCommit =>
                    {
                        mergeCommit.Field("oid");
                    });
                    inner.Field("mergeable");
                    inner.Field("baseRefName");
                    inner.Field("headRefName");
                });
            }).Build();

            Assert.AreEqual(expected, resultQuery);
        }
    }
}
