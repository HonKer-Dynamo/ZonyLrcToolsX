﻿using Newtonsoft.Json;

namespace ZonyLrcToolsX.Downloader.Album.NetEase.JsonModels
{
    public class MusicGetSongDetailsRequest
    {
        public MusicGetSongDetailsRequest(int songId)
        {
            SongId = songId;
            SongIds = $"%5B{songId}%5D";
        }
        
        [JsonProperty("id")]
        public int SongId { get; }

        [JsonProperty("ids")]
        public string SongIds { get; }
    }
}