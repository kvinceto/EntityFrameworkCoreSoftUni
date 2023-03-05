namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
            new MusicHubDbContext();

            //01. MusicHub Database
            DbInitializer.ResetDatabase(context);

            //Test your solutions here

            //Console.WriteLine(ExportAlbumsInfo(context, 9));

            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }


        //02. Albums Info
        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums
                .Where(a => a.ProducerId == producerId)
                .Include(a => a.Producer)
                .Include(a => a.Songs)
                .Select(a => new
                {
                    a.Name,
                    a.ReleaseDate,
                    ProducerName = a.Producer!.Name,
                    Songs = a.Songs
                    .Select(s => new
                    {
                        SongName = s.Name,
                        SongPrice = s.Price,
                        WriterName = s.Writer.Name
                    })
                    .OrderByDescending(s => s.SongName)
                    .ThenBy(s => s.WriterName)
                    .ToArray(),
                    TotalPrice = a.Price
                })
                .OrderByDescending(a => a.TotalPrice)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine("-Songs:");
                int counter = 0;
                foreach (var song in album.Songs)
                {
                    counter++;
                    sb.AppendLine($"---#{counter}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.SongPrice:f2}");
                    sb.AppendLine($"---Writer: {song.WriterName}");
                }
                sb.AppendLine($"-AlbumPrice: {album.TotalPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //03. Songs Above Duration
        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songsAboveDuration = context
                .Songs
                .Where(s => (int)s.Duration.Seconds > duration)
                .Select(s => new
                {
                    SongName = s.Name,
                    PerformerFullName = s.SongPerformers
                        .Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName)
                        .OrderBy(name => name)
                        .ToArray(),
                    WriterName = s.Writer.Name,
                    AlbumProducerName = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.WriterName)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            int counter = 1;
            foreach (var s in songsAboveDuration)
            {
                sb
                    .AppendLine($"-Song #{counter++}")
                    .AppendLine($"---SongName: {s.SongName}")
                    .AppendLine($"---Writer: {s.WriterName}");

                if (s.PerformerFullName.Any())
                {
                    string line = string.Join(", ", s.PerformerFullName);
                    sb.AppendLine($"---Performer: {line}");
                }

                sb
                    .AppendLine($"---AlbumProducer: {s.AlbumProducerName}")
                    .AppendLine($"---Duration: {s.Duration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
