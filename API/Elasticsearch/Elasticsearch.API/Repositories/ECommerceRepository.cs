﻿using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.API.Models.ECommerceModels;
using System.Collections.Immutable;

namespace Elasticsearch.API.Repositories
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _client;

        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        private const string indexName = "kibana_sample_data_ecommerce";

        public async Task<ImmutableList<ECommerce>> TermQueryAsync(string customerFirstName)
        {

            #region Type Unsafe Example
            //var result = await _client.SearchAsync<ECommerce>(s =>
            //    s.Index(indexName)
            //    .Query(q =>
            //        q.Term(t =>
            //            t.Field("customer_first_name.keyword")
            //            .Value(customerFirstName)
            //        )
            //    )
            //);
            #endregion

            #region Type Safety Example
            //var result = await _client.SearchAsync<ECommerce>(s =>
            //    s.Index(indexName)
            //    .Query(q =>
            //        q.Term(t =>
            //            t.Field(f=>
            //                f.CustomerFirstName.Suffix("keyword"))
            //                .Value(customerFirstName)
            //        )
            //    )
            //);
            #endregion

            #region Other Way
            var termQuery = new TermQuery("cutomer_first_name.keyword")
            {
                Value = customerFirstName,
                CaseInsensitive = true,
            };

            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termQuery));
            #endregion

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> TermsQueryAsync(List<string> customerFirstNameList)
        {
            List<FieldValue> terms = new List<FieldValue>();

            customerFirstNameList.ForEach(x =>
            {
                terms.Add(x);
            });

            #region First Way
            //var termsQuery = new TermsQuery()
            //{
            //    Field = "customer_first_name.keyword",
            //    Terms = new TermsQueryField(terms.AsReadOnly())
            //};

            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(termsQuery));
            #endregion

            #region Second Way (Type Safety)
            var result = await _client.SearchAsync<ECommerce>(s =>
                    s.Index(indexName)
                    .Query(q =>
                        q.Terms(t =>
                            t.Field(f =>
                                f.CustomerFirstName.Suffix("keyword"))
                                .Terms(new TermsQueryField(terms.AsReadOnly()))
                        )
                    )
                );
            #endregion

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> PrefixQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                s.Index(indexName)
                .Query(q =>
                    q.Prefix(p =>
                        p.Field(f =>
                            f.CustomerFullName.Suffix("keyword"))
                            .Value(customerFullName)
                        )
                    )
                );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> RangeQueryAsync(double FromPrice, double ToPrice)
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                s.Index(indexName).Size(1000)
                .Query(q =>
                    q.Range(r =>
                        r.NumberRange(nr =>
                            nr.Field(f =>
                                f.TaxfulTotalPrice).Gte(FromPrice).Lte(ToPrice)
                            )
                        )
                    )
                );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchAllQueryAsync()
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                s.Index(indexName)
                .Query(q =>
                    q.MatchAll(m => { })
                )
            );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchAllQueryWithPaginationAsync(int page, int pageSize)
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                s.Index(indexName)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q =>
                    q.MatchAll(m => { })
                )
            );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> WildCardQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                s.Index(indexName)
                .Query(q =>
                    q.Wildcard(w =>
                        w.Field(f =>
                            f.CustomerFullName.Suffix("keyword"))
                            .Wildcard(customerFullName)
                        )
                    )
                );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> FuzzyQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
            s.Index(indexName)
            .Query(q =>
                q.Fuzzy(fu =>
                    fu.Field(f =>
                    f.CustomerFullName.Suffix("keyword"))
                    .Value(customerFullName)
                    .Fuzziness(new Fuzziness(2))
                    )
                )
            .Sort(sort =>
                sort.Field(f =>
                    f.TaxfulTotalPrice, new FieldSort() { Order = SortOrder.Desc })
                )
            );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchQueryFullTextAsync(string categoryName)
        {
            var result = await _client.SearchAsync<ECommerce>(s =>
                s.Index(indexName)
                .Query(q =>
                    q.Match(m =>
                        m.Field(f =>
                            f.Category)
                            .Query(categoryName)
                        )
                    )
                );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchBoolPrefixQueryFullTextAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Query(q =>
                    q.MatchBoolPrefix(m =>
                        m.Field(f =>
                            f.CustomerFullName)
                            .Query(customerFullName)
                        )
                    )
                );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MatchPhraseQueryFullTextAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Query(q =>
                    q.MatchPhrase(m =>
                        m.Field(f =>
                            f.CustomerFullName)
                            .Query(customerFullName)
                        )
                    )
                );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> CompoundQueryExampleFirstAsync(string cityName, double taxfullTotalPrice, string categoryName, string manufacturerName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Query(q =>
                    q.Bool(b =>
                        b.Must(m =>
                            m.Term(t =>
                                t.Field("geoip.city_name").Value(cityName)
                                )
                            )
                        .MustNot(mn =>
                            mn.Range(r =>
                                r.NumberRange(nr =>
                                    nr.Field(f =>
                                        f.TaxfulTotalPrice).Lte(taxfullTotalPrice)
                                    )
                                )
                            )
                        .Should(s =>
                            s.Term(t =>
                                t.Field(f =>
                                    f.Category.Suffix("keyword")).Value(categoryName)
                                    )
                            )
                        .Filter(f =>
                            f.Term(t => t.Field("manufacturer.keyword").Value(manufacturerName)
                                )
                            )
                        )
                    )
                );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> CompoundQueryExampleSecondAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
                .Query(q =>
                    q.Bool(b =>
                        b.Should(s =>
                            s.Match(ma =>
                                ma.Field(f =>
                                    f.CustomerFullName).Query(customerFullName)
                                )
                            .Prefix(p =>
                                p.Field(f =>
                                    f.CustomerFullName.Suffix("keyword")).Value(customerFullName)
                                )
                            )
                        )
                    )
                );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();
        }

        public async Task<ImmutableList<ECommerce>> MultiMatchQueryAsync(string name)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
            .Query(q =>
                    q.MultiMatch(m =>
                        m.Fields(new Field("customer_first_name")
                            .And(new Field("customer_last_name"))
                            .And(new Field("customer_full_name")))
                        .Query(name)
                        )
                    )
                );

            foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

            return result.Documents.ToImmutableList();

        }
    }
}
