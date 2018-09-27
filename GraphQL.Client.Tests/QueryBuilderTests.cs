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
    }
}
