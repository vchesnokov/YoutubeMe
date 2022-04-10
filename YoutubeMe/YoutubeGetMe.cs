using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace YoutubeMe
{
    public class YoutubeGetMe
    {
        private System.Collections.Generic.List<YoutubeExplode.Playlists.PlaylistVideo> m_videos = null;
        private YoutubeExplode.YoutubeClient m_youtubeClient = new YoutubeClient();

        public event EventHandler PushEvent;
        public event EventHandler PushVideoCountEvent;

        protected virtual void OnPushEvent(EventArgs e)
        {
            EventHandler handler = PushEvent;
            handler?.Invoke(this, e);
        }

        protected virtual void OnPushVideoCountEvent(EventArgs e)
        {
            EventHandler handler = PushVideoCountEvent;
            handler?.Invoke(this, e);
        }

        public async void GetList(string channel_id) //, int intOrderVideoNumber)
        {
            if (m_youtubeClient == null)
                m_youtubeClient = new YoutubeClient();

            m_videos = (System.Collections.Generic.List<YoutubeExplode.Playlists.PlaylistVideo>)await m_youtubeClient.Channels.GetUploadsAsync(channel_id);

            OnPushVideoCountEvent(new YoutubeMeDownloadEventArgs { Message = $"{m_videos.Count}" });

            ///DownloadList(intOrderVideoNumber);

            OnPushEvent(new YoutubeMeDownloadEventArgs { Message = $"Найдены Ютуб ролики:" });
            int index = 0;

            foreach (YoutubeExplode.Playlists.PlaylistVideo video_item in m_videos)
            {
                ++index;
                OnPushEvent(new YoutubeMeDownloadEventArgs { Message = $"{index}): {video_item.Url} - {video_item.Title}" });
            } // foreach (YoutubeExplode.Playlists.PlaylistVideo video_item in m_videos)

        }

        public async void DownloadList(int intOrderVideoNumber)
        {
            if (m_videos == null)
                return;

            string localFileName;
            int index = 0;

            foreach (YoutubeExplode.Playlists.PlaylistVideo video_item in m_videos)
            {
                ++index;

                if (index < intOrderVideoNumber)
                {
                    OnPushEvent(new YoutubeMeDownloadEventArgs { Message = $"{index}) Пропускаем файл: {video_item.Url} - {video_item.Title}" });
                    continue;
                }

                localFileName = $"{video_item.Title}.mp4"; // video_item.Title video_item.Id
                
                OnPushEvent(new YoutubeMeDownloadEventArgs { Message = $"{index}) Скачивание файла: {video_item.Url} - {video_item.Title}" });

                var streamManifest = await m_youtubeClient.Videos.Streams.GetManifestAsync(video_item.Id);
                //var streamInfo = streamManifest.GetVideoOnlyStreams().First();
                var streamInfo = streamManifest.GetMuxedStreams().TryGetWithHighestVideoQuality();

                if (streamInfo != null)
                {
                    await m_youtubeClient.Videos.Streams.DownloadAsync(streamInfo, localFileName);
                }
                else
                {
                    OnPushEvent(new YoutubeMeDownloadEventArgs { Message = $"Ошибка скачивания файла: {video_item.Url}" });
                }
            } // foreach (YoutubeExplode.Playlists.PlaylistVideo video_item in m_videos)
        }
    }
}
