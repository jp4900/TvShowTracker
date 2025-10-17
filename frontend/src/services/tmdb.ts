const TMDB_API_KEY = import.meta.env.VITE_TMDB_API_KEY;
const TMDB_BASE_URL = "https://api.themoviedb.org/3";

interface TMDBVideo {
  id: string;
  key: string;
  name: string;
  site: string;
  type: string;
  official: boolean;
}

interface TMDBVideoResponse {
  results: TMDBVideo[];
}

export const tmdbApi = {
  searchTvShow: async (showName: string): Promise<number | null> => {
    try {
      const response = await fetch(
        `${TMDB_BASE_URL}/search/tv?api_key=${TMDB_API_KEY}&query=${encodeURIComponent(
          showName
        )}`
      );
      const data = await response.json();

      return data.results?.[0]?.id || null;
    } catch (error) {
      console.error("Error searching TMDB:", error);
      return null;
    }
  },

  getTrailer: async (tmdbId: number): Promise<string | null> => {
    try {
      const response = await fetch(
        `${TMDB_BASE_URL}/tv/${tmdbId}/videos?api_key=${TMDB_API_KEY}`
      );
      const data: TMDBVideoResponse = await response.json();

      const trailer =
        data.results.find(
          (video) =>
            video.site === "YouTube" &&
            video.type === "Trailer" &&
            video.official
        ) ||
        data.results.find(
          (video) => video.site === "YouTube" && video.type === "Trailer"
        );

      return trailer ? trailer.key : null;
    } catch (error) {
      console.error("Error fetching trailer:", error);
      return null;
    }
  },

  getYouTubeEmbedUrl: (videoKey: string): string => {
    return `https://www.youtube.com/embed/${videoKey}?autoplay=0&rel=0`;
  },
};
