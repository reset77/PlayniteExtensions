﻿using Newtonsoft.Json;
using Playnite.SDK.Models;
using PlayniteExtensions.Common;
using PlayniteExtensions.Metadata.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GiantBombMetadata.Api;

public class GiantBombResponse<T>
{
    /*
"error": "OK",
"limit": 1,
"offset": 0,
"number_of_page_results": 1,
"number_of_total_results": 1,
"status_code": 1,
     */
    public string Error { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }

    [JsonProperty("number_of_page_results")]
    public int NumberOfPageResults { get; set; }

    [JsonProperty("number_of_total_results")]
    public int NumberOfTotalResults { get; set; }

    [JsonProperty("status_code")]
    public int StatusCode { get; set; }

    public T Results { get; set; }

    public string Version { get; set; }
}

public class GiantBombObject: IHasName
{
    public int Id { get; set; }

    public string Name { get; set; }

    [JsonProperty("api_detail_url")]
    public string ApiDetailUrl { get; set; }

    [JsonProperty("site_detail_url")]
    public string SiteDetailUrl { get; set; }
}

public class GiantBombPlatform : GiantBombObject
{
    public string Abbreviation { get; set; }
}

public class GiantBombObjectDetails : GiantBombObject
{
    public string Guid { get; set; }
    public string Aliases { get; set; }

    /// <summary>
    /// Short description
    /// </summary>
    public string Deck { get; set; }
    public string Description { get; set; }
    public GiantBombCoverImage Image { get; set; }

    public string[] AliasesSplit
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Aliases))
                return new string[0];

            return Aliases.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    #region game search results and details only

    [JsonProperty("original_release_date")]
    public string ReleaseDate { get; set; }
    public GiantBombPlatform[] Platforms { get; set; } = new GiantBombPlatform[0];

    #endregion game search results and details only
}

public class GiantBombSearchResultItem : GiantBombObjectDetails, IGameSearchResult
{
    [JsonProperty("resource_type")]
    public string ResourceType { get; set; }

    public IEnumerable<string> AlternateNames => AliasesSplit;

    public string Title => Name;

    IEnumerable<string> IGameSearchResult.Platforms => Platforms?.Select(p => p.Name) ?? new string[0];

    ReleaseDate? IGameSearchResult.ReleaseDate => ReleaseDate.ParseReleaseDate();
}

public class GiantBombGameDetails : GiantBombObjectDetails
{
    public GiantBombObject[] Genres { get; set; } = new GiantBombObject[0];
    public GiantBombObject[] Developers { get; set; } = new GiantBombObject[0];
    public GiantBombObject[] Publishers { get; set; } = new GiantBombObject[0];
    public GiantBombObject[] Franchises { get; set; } = new GiantBombObject[0];

    [JsonProperty("original_game_rating")]
    public GiantBombObject[] Ratings { get; set; } = new GiantBombObject[0];

    public GiantBombImage[] Images { get; set; } = new GiantBombImage[0];

    public GiantBombObject[] Characters { get; set; } = new GiantBombObject[0];
    public GiantBombObject[] Concepts { get; set; } = new GiantBombObject[0];
    public GiantBombObject[] Locations { get; set; } = new GiantBombObject[0];
    public GiantBombObject[] Objects { get; set; } = new GiantBombObject[0];
    public GiantBombObject[] People { get; set; } = new GiantBombObject[0];
    public GiantBombObject[] Themes { get; set; } = new GiantBombObject[0];
}

public class GiantBombImage : IImageData
{
    [JsonProperty("icon_url")]
    public string IconUrl { get; set; }

    [JsonProperty("thumb_url")]
    public string ThumbUrl { get; set; }

    public string Original { get; set; }

    public string Tags { get; set; }

    string IImageData.Url => Original;

    string IImageData.ThumbnailUrl => ThumbUrl;

    int IImageData.Width => 0;

    int IImageData.Height => 0;

    IEnumerable<string> IImageData.Platforms => new string[0];
}

public class GiantBombCoverImage : IImageData
{
    [JsonProperty("icon_url")]
    public string IconUrl { get; set; }

    [JsonProperty("thumb_url")]
    public string ThumbUrl { get; set; }

    [JsonProperty("original_url")] //this is why there's two image classes
    public string Original { get; set; }

    public string Tags { get; set; }

    string IImageData.Url => Original;

    string IImageData.ThumbnailUrl => ThumbUrl;

    int IImageData.Width => 0;

    int IImageData.Height => 0;

    IEnumerable<string> IImageData.Platforms => new string[0];
}

public class GiantBombGamePropertyDetails : GiantBombObjectDetails
{
    public GiantBombObject[] Games { get; set; } = new GiantBombObject[0];
}
