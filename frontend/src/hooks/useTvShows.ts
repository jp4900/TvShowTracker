import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { tvShowsApi, favoritesApi, recommendationsApi } from "../services/api";

export const useTvShows = (params?: {
  pageNumber?: number;
  pageSize?: number;
  genreId?: string;
  type?: string;
  sortBy?: string;
  sortDesc?: boolean;
}) => {
  return useQuery({
    queryKey: ["tvshows", params],
    queryFn: () => tvShowsApi.getAll(params),
  });
};

// Get single TV show
export const useTvShow = (id: string) => {
  return useQuery({
    queryKey: ["tvshow", id],
    queryFn: () => tvShowsApi.getById(id),
    enabled: !!id,
  });
};

// Get episodes
export const useEpisodes = (tvShowId: string, season?: number) => {
  return useQuery({
    queryKey: ["episodes", tvShowId, season],
    queryFn: () => tvShowsApi.getEpisodes(tvShowId, season),
    enabled: !!tvShowId,
  });
};

// Get actors
export const useActors = (tvShowId: string) => {
  return useQuery({
    queryKey: ["actors", tvShowId],
    queryFn: () => tvShowsApi.getActors(tvShowId),
    enabled: !!tvShowId,
  });
};

// Search TV shows
export const useSearchTvShows = (query: string, pageNumber = 1) => {
  return useQuery({
    queryKey: ["search", query, pageNumber],
    queryFn: () => tvShowsApi.search(query, pageNumber),
    enabled: query.length > 0,
  });
};

// Get favorites
export const useFavorites = (pageNumber = 1, pageSize = 20) => {
  return useQuery({
    queryKey: ["favorites", pageNumber],
    queryFn: () => favoritesApi.getAll(pageNumber, pageSize),
  });
};

// Add favorite mutation
export const useAddFavorite = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (tvShowId: string) => favoritesApi.add(tvShowId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["favorites"] });
      queryClient.invalidateQueries({ queryKey: ["tvshows"] });
      queryClient.invalidateQueries({ queryKey: ["tvshow"] });
    },
  });
};

// Remove favorite mutation
export const useRemoveFavorite = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (tvShowId: string) => favoritesApi.remove(tvShowId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["favorites"] });
      queryClient.invalidateQueries({ queryKey: ["tvshows"] });
      queryClient.invalidateQueries({ queryKey: ["tvshow"] });
    },
  });
};

// Get recommendations
export const useRecommendations = (count = 10) => {
  return useQuery({
    queryKey: ["recommendations", count],
    queryFn: () => recommendationsApi.get(count),
  });
};

// Train ML model
export const useTrainModel = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => recommendationsApi.trainModel(),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["recommendations"] });
    },
  });
};
