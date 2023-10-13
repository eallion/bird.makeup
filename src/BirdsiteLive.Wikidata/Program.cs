using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using BirdsiteLive.DAL.Models;
using BirdsiteLive.DAL.Postgres.DataAccessLayers;
using BirdsiteLive.DAL.Postgres.Settings;using BirdsiteLive.Wikidata;

var settings = new PostgresSettings()
{
    ConnString = System.Environment.GetEnvironmentVariable("ConnString"),
};
var dal = new TwitterUserPostgresDal(settings);

var wikiService = new WikidataService(dal);

await wikiService.SyncQcodes();
